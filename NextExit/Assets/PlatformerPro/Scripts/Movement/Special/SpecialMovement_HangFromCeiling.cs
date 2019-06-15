#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Hang from the ceiling on collision.
	/// </summary>
	public class SpecialMovement_HangFromCeiling : SpecialMovement
	{
		/// <summary>
		/// How long we fall for after letting go of ceiling.
		/// </summary>
		public float fallTime = 0.1f;

		/// <summary>
		/// Difference between y position of jump and hang.
		/// </summary>
		public float yOffset = 0.2f;

		/// <summary>
		/// Can we climb?
		/// </summary>
		public bool canClimb;

		/// <summary>
		/// Offset applied at start of climb before we go to CLIMB.
		/// </summary>
		public Vector2 climbStartOffset;

		/// <summary>
		/// Offset applied at end of climb before we go to CLIMB_UP.
		/// </summary>
		public Vector2 climbEndOffset;

		/// <summary>
		/// Direction to face. Use 0 to maintain facing direction.
		/// This is really just a work around for the graphics in bionic cop.
		/// </summary>
		public int facingDirection;

		/// <summary>
		/// Layers we can climb up.
		/// </summary>
		public int climbableLayers;
		
		/// <summary>
		/// Layers we can hang from.
		/// </summary>
		public int hangableLayers;

		/// <summary>
		/// Current state.
		/// </summary>
		protected CeilingHangState state;

		/// <summary>
		/// The fall timer.
		/// </summary>
		protected float fallTimer;
		
		/// <summary>
		/// If the animator drives animation state then keep a cached copy.
		/// </summary>
		protected Animator myAnimator;

		/// <summary>
		/// Stores current heaad collider.
		/// </summary>
		protected Collider2D currentHeadCollider;

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Ceiling Hang";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Hang from the ceiling on collision.";
		
		/// <summary>
		/// Static movement info used by the editor.
		/// </summary>
		new public static MovementInfo Info
		{
			get
			{
				return new MovementInfo(Name, Description);
			}
		}

		/// <summary>
		/// The index of the fall time in the movement data.
		/// </summary>
		protected const int FallTimeIndex = 0;
		
		/// <summary>
		/// The index of the Y Offset in the movement data.
		/// </summary>
		protected const int YOffsetIndex = 1;

		/// <summary>
		/// The index of the can climb in the movement data.
		/// </summary>
		protected const int CanClimbIndex = 2;

		/// <summary>
		/// The index of the climb start in the movement data.
		/// </summary>
		protected const int ClimbStartOffsetIndex = 3;

		/// <summary>
		/// The index of the climb end in the movement data.
		/// </summary>
		protected const int ClimbEndOffsetIndex = 4;

		/// <summary>
		/// The index of the climbable layers in the movement data.
		/// </summary>
		protected const int ClimbableLayersIndex = 5;

		/// <summary>
		/// The index of the hangable layers in the movement data.
		/// </summary>
		protected const int HangableLayersIndex = 7;

		/// <summary>
		/// The index of the facing direction in the movement data.
		/// </summary>
		protected const int FacingDirectionIndex = 6;

		/// <summary>
		/// The size of the movement variable array in the movement data.
		/// </summary>
		private const int MovementVariableCount = 8;

		#endregion

		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update()
		{
			if (fallTimer > 0) fallTimer -= TimeManager.FrameTime;
		}

		/// <summary>
		/// Unity Late Update hook.
		/// </summary>
		void LateUpdate()
		{
			if (state != CeilingHangState.NONE) CheckForAnimationStateTransition ();
		}

		/// <summary>
		/// Initialise the movement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		/// <param name="ignored">Ignored.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			if (movementData != null && movementData.Length >= MovementVariableCount)
			{
				fallTime = movementData [FallTimeIndex].FloatValue;
				yOffset = movementData [YOffsetIndex].FloatValue;
				canClimb = movementData [CanClimbIndex].BoolValue;
				climbStartOffset = movementData [ClimbStartOffsetIndex].Vector2Value;
				climbEndOffset = movementData [ClimbEndOffsetIndex].Vector2Value;
				climbableLayers = movementData [ClimbableLayersIndex].IntValue;
				hangableLayers = movementData [HangableLayersIndex].IntValue;
				facingDirection = movementData [FacingDirectionIndex].IntValue;
			}
			else
			{
				Debug.LogError("Invalid movement data.");
			}

			myAnimator = character.gameObject.GetComponentInChildren<Animator> ();
			AssignReferences (character);
			return this;
		}

		/// <summary>
		/// Gets a value indicating whether this movement wants to do a special move.
		/// </summary>
		/// <returns><c>true</c>, if special move was wanted, <c>false</c> otherwise.</returns>
		override public bool WantsSpecialMove()
		{
			if (fallTimer > 0) return false;
			if (state == CeilingHangState.DONE) return false;
			if (state == CeilingHangState.NONE)
			{
				// TODO Reevalaute these conditions
				if ((character.PreviousVelocity.y >= -1.0f || character.Velocity.y >= 1.0f || character.ActiveMovement is SpecialMovement_GrapplingHook) && CheckHeadCollisions())
				{
					if (hangableLayers == 0 ||
					 ((1 << (currentHeadCollider.gameObject.layer)) & (int) hangableLayers) == ((1 << currentHeadCollider.gameObject.layer)))
					{	
						return true;
					}
				}
				return false;
			}

			else if (state == CeilingHangState.HANG)
			{
				// Down to release
				if (character.Input.VerticalAxisDigital == -1) 
				{
					fallTimer = fallTime;
					state = CeilingHangState.DROP;
					return false;
				}
				// Up to climb (second condition not required but makes it easier to understand)
				if (canClimb &&
				    character.Input.VerticalAxisState == ButtonState.DOWN && 
				    character.Input.VerticalAxisDigital == 1 &&
				    (climbableLayers == 0 || ((1 << (currentHeadCollider.gameObject.layer)) & (int) climbableLayers) == ((1 << currentHeadCollider.gameObject.layer))))
				{		
					state = CeilingHangState.CLIMB_START;
				}

			}
			return true;
		}

		/// <summary>
		/// Returns the direction the character is facing. 0 for none, 1 for right, -1 for left.
		/// wThis overriden version always returns the input direction.
		/// </summary>
		override public int FacingDirection
		{
			get
			{
				if (facingDirection == 0)
				{
					if (character.Velocity.x > 0)
						return 1;
					if (character.Velocity.x < 0)
						return -1;
				}
				return facingDirection;
			}
		}

		/// <summary>
		/// Checks for animation state transition. I.e. when  state should be changed based on the
		/// animation state.
		/// </summary>
		virtual protected void CheckForAnimationStateTransition() 
		{
			if (state == CeilingHangState.CLIMB_START)
			{
				AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo (0);
				if (info.IsName (AnimationState.CEILING_CLIMB.AsString ())) {
					character.Translate (climbStartOffset.x, climbStartOffset.y, true);
					state = CeilingHangState.CLIMBING;
				}
			}
			else if (state == CeilingHangState.CLIMBING) {
				AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo (0);
				if (info.normalizedTime >= 1.0f)
				{
					state = CeilingHangState.LAUNCH_START;
				}
			}
			else if (state == CeilingHangState.LAUNCH_START)
			{
				AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo (0);
				if (info.IsName (AnimationState.CEILING_CLIMB_LAUNCH.AsString ())) {
					character.Translate (climbEndOffset.x, climbEndOffset.y, true);
					state = CeilingHangState.LAUNCH;
				}
			}
			else if (state == CeilingHangState.LAUNCH)
			{
				AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo (0);
				if (info.normalizedTime >= 1.0f)
				{
					state = CeilingHangState.DONE;
				}
			}
		}

		/// <summary>
		/// Start the special mvoe
		/// </summary>
		override public void DoSpecialMove()
		{
			if (state == CeilingHangState.NONE)
			{
				state = CeilingHangState.HANG;
				character.SetVelocityX (0);
				character.SetVelocityY (0);
				character.Translate(0, yOffset, true);

				for (int i = 0; i < character.Colliders.Length; i++)
				{
					if (character.Colliders[i].RaycastType == RaycastType.HEAD)
					{
						RaycastHit2D hit = character.GetClosestCollision(i);
						if (hit.collider != null)
						{
							Platform tmpPlatform = hit.collider.GetComponent<Platform>();
							if (tmpPlatform != null && !tmpPlatform.IgnoreCollision(character, character.Colliders[i]))
							{
								PlatformCollisionArgs platformCollisionArgs = new PlatformCollisionArgs();
								platformCollisionArgs.Character = character;
								platformCollisionArgs.RaycastCollider = character.Colliders[i];
								platformCollisionArgs.Penetration = 0;
								bool parent = tmpPlatform.Collide(platformCollisionArgs);
								if (parent) 
								{
									character.ParentPlatform = tmpPlatform;
									character.ParentRaycastType = RaycastType.HEAD;
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Called when the movement loses control.
		/// </summary>
		override public void LosingControl()
		{
			state = CeilingHangState.NONE;
		}

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				if (state == CeilingHangState.CLIMB_START) return AnimationState.CEILING_CLIMB;
				if (state == CeilingHangState.CLIMBING) return AnimationState.CEILING_CLIMB;
				if (state == CeilingHangState.LAUNCH_START) return AnimationState.CEILING_CLIMB_LAUNCH;
				if (state == CeilingHangState.LAUNCH) return AnimationState.CEILING_CLIMB_LAUNCH;
				return AnimationState.CEILING_HANG;
			}
		}

		/// <summary>
		/// This class will handle gravity internally.
		/// </summary>
		override public bool ShouldApplyGravity
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="PlatformerPro.Movement"/> expects the
		/// base collisions to be executed after its movement finishes.
		/// </summary>
		override public RaycastType ShouldDoBaseCollisions
		{
			get
			{
				// We don't want to check head collissions more than once so we don't unparent if parented to a moving platform 
				// if there is a chance that something can push you downwards while hanging from ceiling you may want
				// to make this check a little more clever.
				return RaycastType.SIDES | RaycastType.FOOT;
				
			}
		}

		/// <summary>
		/// Checks the head collisions.
		/// </summary>
		/// <returns><c>true</c>, if head colliders were hitting something , <c>false</c> otherwise.</returns>
		virtual protected bool CheckHeadCollisions()
		{
			int hitCount = 0;
			for (int i = 0; i < character.Colliders.Length; i++)
			{
				if (character.Colliders[i].RaycastType == RaycastType.HEAD)
				{
					RaycastHit2D hit = character.GetClosestCollision(i);
					if (hit.collider != null)
					{
						currentHeadCollider = hit.collider;
						hitCount++;
					}
				}
			}
			if (hitCount > 1) return true;
			return false;
		}

#if UNITY_EDITOR

		/// <summary>
		/// Draws the inspector.
		/// </summary>
		public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{

			if (movementData == null || movementData.Length < MovementVariableCount)
			{
				movementData = new MovementVariable[MovementVariableCount];
			}

			// Facing Direction
			if (movementData [FacingDirectionIndex] == null) movementData [FacingDirectionIndex] = new MovementVariable ();
			movementData [FacingDirectionIndex].IntValue = EditorGUILayout.IntField (new GUIContent ("Facing Direction", "Direction to face when hanging (0 for either)."), movementData [FacingDirectionIndex].IntValue);

			// Fall time
			if (movementData [FallTimeIndex] == null) movementData [FallTimeIndex] = new MovementVariable ();
			movementData [FallTimeIndex].FloatValue = EditorGUILayout.FloatField (new GUIContent ("Fall Time", "How long to fall for after letting go of ceiling."), movementData [FallTimeIndex].FloatValue);
			if  (movementData [FallTimeIndex].FloatValue < 0) movementData [FallTimeIndex].FloatValue  = 0;

			// Y Offset
			if (movementData [YOffsetIndex] == null) movementData [YOffsetIndex] = new MovementVariable ();
			movementData [YOffsetIndex].FloatValue = EditorGUILayout.FloatField (new GUIContent ("Y Offset Time", "Difference between y position of jump and hang."), movementData [YOffsetIndex].FloatValue);

			// Hangable layers
			if (movementData [HangableLayersIndex] == null) movementData [HangableLayersIndex] = new MovementVariable ((int)target.geometryLayerMask);

			string[] layerNames = GenerateLayerNames ();

			// Climbable layers
			movementData [HangableLayersIndex].IntValue = EditorGUILayout.MaskField(new GUIContent ("Hangable Layers", "Layers we can hang from."), 
			                                                                         movementData [HangableLayersIndex].IntValue,
			                                                                         layerNames);

			// Can Climb
			if (movementData [CanClimbIndex] == null) movementData [CanClimbIndex] = new MovementVariable ();
			movementData [CanClimbIndex].BoolValue = EditorGUILayout.Toggle (new GUIContent ("Can Climb", "Can climb ceiling."), movementData [CanClimbIndex].BoolValue);

			if (movementData [ClimbEndOffsetIndex] == null) movementData [ClimbEndOffsetIndex] = new MovementVariable ();
			if (movementData [ClimbStartOffsetIndex] == null) movementData [ClimbStartOffsetIndex] = new MovementVariable ();
			if (movementData [ClimbableLayersIndex] == null) movementData [ClimbableLayersIndex] = new MovementVariable ((int)target.geometryLayerMask);

			if (movementData [CanClimbIndex].BoolValue)
			{
				// Climb start offset
				movementData [ClimbStartOffsetIndex].Vector2Value = EditorGUILayout.Vector2Field (new GUIContent ("Climb Start Offset", "Offset to apply when we start the climb."), movementData [ClimbStartOffsetIndex].Vector2Value);

				// Climb end offset
				movementData [ClimbEndOffsetIndex].Vector2Value = EditorGUILayout.Vector2Field (new GUIContent ("Climb End Offset", "Offset to apply when we start the launch."), movementData [ClimbEndOffsetIndex].Vector2Value);

				// Climbable layers
				movementData [ClimbableLayersIndex].IntValue = EditorGUILayout.MaskField(new GUIContent ("Climbable Layers", "Layers we can climb up."), 
				                                                                         movementData [ClimbableLayersIndex].IntValue,
				                                                                         layerNames);
			}

			return movementData;
		}

		/// <summary>
		/// Generates the layer names list (we can't use internals becasue that list doesn't include blanks).
		/// NOTE: No attempt made to make this perform well.
		/// </summary>
		/// <returns>The layer names.</returns>
		public static string[] GenerateLayerNames()
		{
			List<string> layers = new List<string> ();
			for (int i = 0; i < NoAllocationRaycast.MAX_LAYER; i++)
			{
				string name = LayerMask.LayerToName(i);
				if (name == null || name == "") name = "<Layer " + i + ">";
				layers.Add (name);
			}
			return layers.ToArray ();
		}

#endif
	}


	public enum CeilingHangState
	{
		NONE,
		HANG,
		CLIMB_START,
		CLIMBING,
		LAUNCH_START,
		LAUNCH,
		DONE,
		DROP
	}
}
