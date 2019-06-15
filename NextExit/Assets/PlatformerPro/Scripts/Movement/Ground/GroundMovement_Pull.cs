#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Ground movement which allows character to pull things.
	/// </summary>
	public class GroundMovement_Pull : GroundMovement
	{
		
		#region members
		
		/// <summary>
		/// Speed if weight is not affecting us.
		/// </summary>
		[TaggedProperty ("agility")]
		public float speed;

		/// <summary>
		/// How much can the character pull.
		/// </summary>
		public float maxWeight;


		/// <summary>
		/// Should we consider object weight when adjusting speed.
		/// </summary>
		public bool weightAffectsSpeed;

		/// <summary>
		/// The current thing being pushed.
		/// </summary>
		protected IPullable pullable;

		/// <summary>
		/// How far is the pullable offset when the pull starts.
		/// </summary>
		protected float initialPullOffset;

		/// <summary>
		/// The direction we are pushing.
		/// </summary>
		protected float pullDirection;

		/// <summary>
		/// Are we latched on to something?
		/// </summary>
		protected bool latched;

		/// <summary>
		/// Are we still latched even though we let go of button.
		/// </summary>
		protected float pullDelayTimer;

		/// <summary>
		/// How lond do we have before we can't latch without pressing towards object.
		/// </summary>
		protected float latchTimer;

		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Special/Simple Pull";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Ground movement which allows character to pull things. This should not be the default movements be should instead used in conjunction with another ground movement.";
		
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
		/// The index of the speed adjustment.
		/// </summary>
		protected const int SpeedIndex = 0;

		/// <summary>
		/// The index of the max weight.
		/// </summary>
		protected const int MaxWeightIndex = 1;

		/// <summary>
		/// The index of weight affects speed.
		/// </summary>
		protected const int WeightAffectsSpeedIndex = 2;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 3;

		/// <summary>
		/// A small delay before pull so a user can double tap to stop pulling.
		/// </summary>
		protected const float PullDelayTime = 0.125f;

		/// <summary>
		/// How long do we give the user to start pulling before resetting back to a normal state.
		/// </summary>
		protected const float LatchTime = 0.75f;

		#endregion

		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update()
		{
			if (pullDelayTimer > 0) pullDelayTimer -= TimeManager.FrameTime;
			if (latchTimer > 0) latchTimer -= TimeManager.FrameTime;
		}

		#region public methods
		
		/// <summary>
		/// Initialise the mvoement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			AssignReferences (character);
			
			if (movementData != null && movementData.Length >= MovementVariableCount)
			{
				speed = movementData[SpeedIndex].FloatValue;
				maxWeight = movementData[MaxWeightIndex].FloatValue;
				weightAffectsSpeed = movementData[WeightAffectsSpeedIndex].BoolValue;
			}
			else
			{
				Debug.LogError("Invalid movement data for Ground Movement Push");
			}

			return this;
		}

		/// <summary>
		/// Gets a value indicating whether this movement wants to control the movement on the ground.
		/// This movement wants control if the side colliders are pulling a box.
		/// </summary>
		/// <value><c>true</c> if this instance wants control; otherwise, <c>false</c>.</value>
		override public bool WantsGroundControl()
		{
			if (!enabled) return false;
			if (character.DefaultGroundMovement == this)
			{
				Debug.LogError("The Pull movement can't be the default ground movement. Disabling movement.");
				enabled = false;
			}

			// Early out, only pull if grounded.
			if (!character.Grounded) 
			{
				LosingControl();
				return false;
			}

			// Early out, if last frame we were pulling one way, we want to lose control for at least one frame before pulling another way
			if (!latched && pullDirection == -character.Input.HorizontalAxisDigital)
			{
				LosingControl();
				return false;
			}

			// Early out, if we were pulling, and our velocity is non zero and we are still holding down pull keep going
			if (pullable != null && !latched && pullDirection == character.Input.HorizontalAxisDigital && character.Velocity.x != 0) return true;

			// If we are latched check if we are still latched
			if (latched && pullable != null)
			{
				if (character.Input.HorizontalAxisDigital == -pullDirection || character.Input.HorizontalAxisDigital == 0) 
				{
					if (latchTimer <= 0) LosingControl();
					return false;
				}
				// Start pulling
				if (character.Input.HorizontalAxisDigital == pullDirection)
				{
					latched = false;
					pullDelayTimer = PullDelayTime;
					return true;
				}
			}

			// Try to find a pullable
			int hitCount = 0;
			IPullable matchingPullable = null;
			float pullableDistance = 1;
			float nonPullableDistance = 1;
			RaycastType typeToCheck = (character.Input.HorizontalAxisDigital == 1 ? RaycastType.SIDE_RIGHT : RaycastType.SIDE_LEFT);
			initialPullOffset = 0;
			for (int i = 0; i < character.Colliders.Length; i++)
			{
				if (character.Colliders[i].RaycastType == typeToCheck)
				{
					RaycastHit2D hit = character.GetClosestCollision(i);
					if (hit.collider != null)
					{
						IPullable currentPullable = (IPullable) hit.collider.GetComponent(typeof(IPullable));
						// Pulling  something that isn't pushable TODO add distance checks.
						if (currentPullable == null) 
						{
							if (hit.fraction < nonPullableDistance) nonPullableDistance = hit.fraction;
						}
						else
						{
							if (currentPullable != matchingPullable)
							{
								// Found a closer pushable (or if matching pushable is null we just found a pushable)
								if (hit.fraction < pullableDistance) 
								{
									pullableDistance = hit.fraction;
									matchingPullable = currentPullable;
									initialPullOffset = (hit.fraction * (character.Colliders[i].Length + character.Colliders[i].LookAhead)) - character.Colliders[i].Length - character.Colliders[i].LookAhead;
									hitCount = 1;
								}
							}
							else
							{
								hitCount++;
							}
						}
					}
				}
			}

			if (matchingPullable != null && pullableDistance < nonPullableDistance && initialPullOffset < 0 ) 
			{
				pullable = matchingPullable;
				initialPullOffset *= -character.Input.HorizontalAxisDigital;
				pullDirection = -character.Input.HorizontalAxisDigital;
				latched = true;
				latchTimer = LatchTime;
			}
			return false;
		}

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			if (enabled)
			{
				float frameSpeed = GetSpeed(speed);
				if (character.Friction > 2.0f) speed *= (2.0f / character.Friction );
				if (weightAffectsSpeed) 
				{
					frameSpeed /= Mathf.Log10(pullable.Mass * 10);
				}
				// TODO Take slope in to account
				// Quaternion.Euler(0,0, -character.SlopeTargetRotation) 

				if (character.Input.HorizontalAxisDigital == 1)
				{
					character.SetVelocityX(character.IsGravityFlipped ? -frameSpeed : frameSpeed);
					// With this removed the box is actually pushing us - seems to work better though
					// character.Translate((character.IsGravityFlipped ? -frameSpeed : frameSpeed) * TimeManager.FrameTime, 0, false);
				}
				else if (character.Input.HorizontalAxisDigital == -1)
				{
					character.SetVelocityX(character.IsGravityFlipped ? frameSpeed : -frameSpeed);
					// With this removed the box is actually pushing us - seems to work better though
					// character.Translate((character.IsGravityFlipped ? frameSpeed : -frameSpeed) * TimeManager.FrameTime, 0, false);
				}
				else
				{
					character.SetVelocityX(0);
				}
				// Pull, but only if timer is under 0
				if (pullDelayTimer <= 0) pullable.Pull (character, new Vector2(character.Velocity.x, 0) * TimeManager.FrameTime);
			}
		}

		/// <summary>
		/// Does any movement that MUST be done after collissions are calculated.
		/// </summary>
		override public void PostCollisionDoMove() {
			if (enabled && !character.rotateToSlopes) SnapToGround ();
		}

		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions.
		/// </summary>
		override public void LosingControl()
		{
			pullDirection = 0;
			pullable = null;
			latched = false;
			pullDelayTimer = 0;
		}

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				if (pullDelayTimer <= 0)  return AnimationState.PULL;
				return AnimationState.IDLE;
			}
		}

		// <summary>
		/// Returns the direction the character is facing. 0 for none, 1 for right, -1 for left.
		/// This overriden version always returns the default ground movements facing direction.
		/// </summary>
		override public int FacingDirection
		{
			get 
			{
				if (character.DefaultGroundMovement != this) return -character.DefaultGroundMovement.FacingDirection;
				return 0;
			}
		}

		/// <summary>
		/// Don't allow base collisions to reset X velocity.
		/// </summary>
//		override public bool PreventXVelocityReset
//		{
//			get
//			{
//				return true;
//			}
//		}

		#endregion

#if UNITY_EDITOR
		
		#region draw inspector
		
		/// <summary>
		/// Draws the inspector.
		/// </summary>
		public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{
			if (movementData == null || movementData.Length < MovementVariableCount)
			{
				movementData = new MovementVariable[MovementVariableCount];
			}

			// Speed Adjustment Factor
			if (movementData[SpeedIndex] == null) movementData[SpeedIndex] = new MovementVariable(0.7f);
			movementData[SpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Speed", "How fast do we pull."), movementData[SpeedIndex].FloatValue);
			if (movementData[SpeedIndex].FloatValue < 0) movementData[SpeedIndex].FloatValue = 0.0f;

			// Max weight
			if (movementData[MaxWeightIndex] == null) movementData[MaxWeightIndex] = new MovementVariable(100.0f);
			movementData[MaxWeightIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Max Weight", "The maximum weight the character can pull."), movementData[MaxWeightIndex].FloatValue);
			if (movementData[MaxWeightIndex].FloatValue < 0) movementData[MaxWeightIndex].FloatValue = 0.0f;

			// Weight affects speed
			if (movementData[WeightAffectsSpeedIndex] == null) movementData[WeightAffectsSpeedIndex] = new MovementVariable(false);
			movementData[WeightAffectsSpeedIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Weight Affects Speed", "If true the weight will have an affect on characters speed."), movementData[WeightAffectsSpeedIndex].BoolValue);
			if (movementData[MaxWeightIndex].FloatValue < 0) movementData[MaxWeightIndex].FloatValue = 0.0f;

			return movementData;
		}
		
		#endregion
		
#endif

	}
}