#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A special movement which starts the character swimming when the enter the water.
	/// </summary>
	public class SpecialMovement_Swim : SpecialMovement
	{

		#region members

		/// <summary>
		/// Gravity to apply when swimming. Usually much lower than standard gravity,
		/// </summary>
		public float swimGravity;

		/// <summary>
		/// Maximum speed of descent when swimming. Usually much lower than the standard terminal velocity.
		/// </summary>
		public float swimTerminalVelocity;

		/// <summary>
		/// How long does a swimming stroke say. 2.0 would mean the character can stroke every 2 seconds.
		/// </summary>
		public float strokeSpeed;

		/// <summary>
		/// The stroke acceleration. X value is switched based on facing direction.
		/// </summary>
		public Vector2 strokeAcceleration;

		/// <summary>
		/// Maximum x speed.
		/// </summary>
		public float maxSpeedInX;

		/// <summary>
		/// How fast do we accelerate in X.
		/// </summary>
		public float xAcceleration;

		/// <summary>
		/// X drag.
		/// </summary>
		public float drag;

		/// <summary>
		/// If the character is on the ground when underwater should they use the default ground movement. If false they will always swim.
		/// </summary>
		public bool useGroundMovementWhenGrounded;

		/// <summary>
		/// Tracks if we are swimming or not.
		/// </summary>
		protected bool swimStarted;

		/// <summary>
		/// The swim stroke timer, can't stroke until this is less than or equal to 0.
		/// </summary>
		protected float swimStrokeTimer;

		/// <summary>
		/// Tracks if we are on surface of water or not.
		/// </summary>
		protected bool isOnSurface;

		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Swim/Basic";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Mario style swimming, which must be triggered by an EventResponser with the START_SWIM response. Extend this to implement your own swim.";
		
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
		/// The index of the swim gravity.
		/// </summary>
		protected const int SwimGravityIndex = 0;

		/// <summary>
		/// The index of the swim terminal velocity.
		/// </summary>
		protected const int SwimTerminalVelocityIndex = 1;

		/// <summary>
		/// The index of the stroke speed.
		/// </summary>
		protected const int StrokeSpeedIndex = 2;

		/// <summary>
		/// The index of the stroke acceleration.
		/// </summary>
		protected const int StrokeAccelerationIndex = 3;

		/// <summary>
		/// The index of the max speed in X.
		/// </summary>
		protected const int MaxSpeedInXIndex = 4;

		/// <summary>
		/// The index of the X acceleration.
		/// </summary>
		protected const int XAccelerationIndex = 5;

		/// <summary>
		/// The index of the drag.
		/// </summary>
		protected const int DragIndex = 6;

		/// <summary>
		/// The index of the use ground movement when grounded.
		/// </summary>
		protected const int UseGroundMovementWhenGroundedIndex = 7;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 8;

		/// <summary>
		/// The default drag.
		/// </summary>
		protected const float DefaultDrag = 4.0f;

		/// <summary>
		/// The default acceleration.
		/// </summary>
		protected const float  DefaultAcceleration = 12.0f;

		/// <summary>
		/// The default max speed.
		/// </summary>
		protected const float DefaultMaxSpeed = 4.0f;

		/// <summary>
		/// The default swim gravity.
		/// </summary>
		protected const float DefaultSwimGravity = -6.0f;

		/// <summary>
		/// The default terminal speed.
		/// </summary>
		protected const float DefaultTerminalVelocity = -3.5f;

		/// <summary>
		/// The default stroke speed.
		/// </summary>
		protected const float DefaultStrokeSpeed = 0.75f;

		/// <summary>
		/// The default stroke acceleration.
		/// </summary>
		protected static Vector2 DefaultStrokeAcceleration = new Vector2 (5, 3.5f);

		#endregion

		#region Unity Hooks

		/// <summary>
		/// Unity Update hook
		/// </summary>
		void Update()
		{
			if (swimStarted && swimStrokeTimer > 0) swimStrokeTimer -= TimeManager.FrameTime;
		}

		#endregion

		/// <summary>
		/// Gets a value indicating whether this <see cref="PlatformerPro.SpecialMovement_Swim"/> is swimming on surface.
		/// </summary>
		/// <value><c>true</c> if on surface; otherwise, <c>false</c>.</value>
		virtual public bool OnSurface
		{
			get
			{
				return isOnSurface;
			}
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
				swimGravity = movementData[SwimGravityIndex].FloatValue;
				swimTerminalVelocity = movementData[SwimTerminalVelocityIndex].FloatValue;
				useGroundMovementWhenGrounded = movementData[UseGroundMovementWhenGroundedIndex].BoolValue;
				maxSpeedInX = movementData[MaxSpeedInXIndex].FloatValue;
				xAcceleration = movementData[XAccelerationIndex].FloatValue;
				drag = movementData[DragIndex].FloatValue;
				strokeSpeed = movementData[StrokeSpeedIndex].FloatValue;
				// By setting this we ensure we can't jump as soon as we hit the water
				swimStrokeTimer = strokeSpeed / 2.0f;	
				strokeAcceleration = movementData[StrokeAccelerationIndex].Vector2Value;
			}
			else
			{
				Debug.LogError("Invalid movement data.");
			}
			return this;
		}

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				if (character.Grounded && useGroundMovementWhenGrounded) return character.DefaultGroundMovement.AnimationState;
				if (isOnSurface) 
				{
					if (swimStrokeTimer > (strokeSpeed / 2.0f)) return AnimationState.SWIM_SURFACE_STROKE;
					return AnimationState.SWIM_SURFACE;
				}
				if (swimStrokeTimer > (strokeSpeed / 2.0f)) return AnimationState.SWIM_STROKE;
				return AnimationState.SWIM;
			}
		}

		/// <summary>
		/// Gets the priority of the animation state that this movement wants to set.
		/// </summary>
		/// <value>The animation priority.</value>
		override public int AnimationPriority
		{
			get
			{
				if (character.Grounded && useGroundMovementWhenGrounded) return character.DefaultGroundMovement.AnimationPriority;
				if (swimStrokeTimer > (strokeSpeed / 2.0f)) return 5;
				return 1;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this movement wants to do a special move.
		/// </summary>
		/// <value><c>true</c> if this instance wants control; otherwise, <c>false</c>.</value>
		override public bool WantsSpecialMove()
		{
			return swimStarted;
		}

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{

			// Stroke
			if (swimStrokeTimer <= 0.0f && character.Input.JumpButton == ButtonState.HELD)
			{
				swimStrokeTimer = strokeSpeed;
				character.AddVelocity(strokeAcceleration.x * character.LastFacedDirection, 0, true);
				character.SetVelocityY(strokeAcceleration.y);
			}
			MoveInX();
			MoveInY();
		}

		/// <summary>
		/// Handle X movement
		/// </summary>
		virtual protected void MoveInX ()
		{
			// Apply drag (we use a friction like equation which seems to look better than a drag like one)
			if (character.Velocity.x > 0) 
			{
				character.AddVelocity(-character.Velocity.x * drag * TimeManager.FrameTime, 0, true);
				if (character.Velocity.x < 0) character.SetVelocityX(0);
			}
			else if (character.Velocity.x < 0) 
			{
				character.AddVelocity(-character.Velocity.x * drag * TimeManager.FrameTime, 0, true);
				if (character.Velocity.x > 0) character.SetVelocityX(0);
			}

			character.AddVelocity((float)character.Input.HorizontalAxisDigital * xAcceleration * TimeManager.FrameTime, 0, true);

			// Limit to max speed
			if (character.Velocity.x > maxSpeedInX) 
			{
				character.SetVelocityX(maxSpeedInX);
			}
			else if (character.Velocity.x < -maxSpeedInX) 
			{
				character.SetVelocityX(-maxSpeedInX);
			}
			// Translate
			character.Translate(character.Velocity.x * TimeManager.FrameTime, 0, true);
		}

		virtual protected void MoveInY ()
		{
			// Apply gravity if not grounded
			if (!character.Grounded &&
			    // If we hit our head we don't want to sink straight away as it looks weird (numbers are a bit magic but look right)
			    (swimStrokeTimer <= (strokeSpeed / 2.5f) || !character.WouldHitHeadThisFrame))
			{
				character.AddVelocity(0, TimeManager.FrameTime * swimGravity, false);
			}

			// Limit to terminal
			if (character.Velocity.y < swimTerminalVelocity) character.SetVelocityY (swimTerminalVelocity);

			// Translate
			character.Translate(0, character.Velocity.y * TimeManager.FrameTime, true);
		}

		/// <summary>
		/// If this is true then the movement wants to maintain control of the character even
		/// if default transition conditions suggest it shouldn't.
		/// </summary>
		/// <returns><c>true</c> if control is wanted, <c>false</c> otherwise.</returns>
		override public bool ForceMaintainControl()
		{
			return swimStarted;
		}

		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions.
		/// </summary>
		override public void LosingControl()
		{
			character.SetVelocityX (0);
			character.SetVelocityY (0.0f);
			// Ensure we can't instantly swim as soon as we hit the water again
			swimStrokeTimer = strokeSpeed / 2.0f;
			// Jump out of the water
			character.DefaultAirMovement.DoOverridenJump (1.0f, 2);
		}

		/// <summary>
		/// Starts the swim.
		/// </summary>
		virtual public void StartSwim()
		{
			swimStarted = true;
			isOnSurface = false;
		}

		/// <summary>
		/// Stops the swim.
		/// </summary>
		virtual public void StopSwim()
		{
			swimStarted = false;
			isOnSurface = false;
		}

		/// <summary>
		/// Starts Surface Swim.
		/// </summary>
		virtual public void SurfaceSwim()
		{
			Debug.LogWarning ("Basic swim doesn't support 'swimming on surface', use Swim/Directional.");
		}

        /// <summary>
        /// Ends Surface Swim.
        /// </summary>
        virtual public void EndSurfaceSwim()
        {
            Debug.LogWarning("Basic swim doesn't support 'swimming on surface', use Swim/Directional.");
        }

        /// <summary>
        /// Shoulds the apply gravity.
        /// </summary>
        /// <returns><c>true</c>, if apply gravity was shoulded, <c>false</c> otherwise.</returns>
        override public bool ShouldApplyGravity
		{
			get 
			{
				return false;
			}
		}

		/// <summary>
		/// Gets the should do base collisions.
		/// </summary>
		/// <value>The should do base collisions.</value>
		override public RaycastType ShouldDoBaseCollisions
		{
			get
			{
				return RaycastType.ALL;
			}
		}

		/// <summary>
		/// Returns the direction the character is facing. 0 for none, 1 for right, -1 for left.
		/// This overriden version always returns the input direction.
		/// </summary>
		override public int FacingDirection
		{
			get 
			{
				return character.Input.HorizontalAxisDigital;
			}
		}


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

			EditorGUILayout.LabelField ("General", EditorStyles.boldLabel);
			// Swim Gravity
			if (movementData[SwimGravityIndex] == null) movementData[SwimGravityIndex] = new MovementVariable(DefaultSwimGravity);
			movementData[SwimGravityIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Swim Gravity", "Gravity to use for swimming (negative value), typically much lower than standard gravity."), movementData[SwimGravityIndex].FloatValue);
			if (movementData [SwimGravityIndex].FloatValue > 0) movementData [SwimGravityIndex].FloatValue *= - 1;

			// Swim Terminal Velocity
			if (movementData[SwimTerminalVelocityIndex] == null) movementData[SwimTerminalVelocityIndex] = new MovementVariable(DefaultTerminalVelocity);
			movementData[SwimTerminalVelocityIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Terminal Velocity", "Maximum speed of descent when swimming. Usually much lower than the standard terminal velocity."), movementData[SwimTerminalVelocityIndex].FloatValue);
			if (movementData[SwimTerminalVelocityIndex].FloatValue > 0) movementData [SwimTerminalVelocityIndex].FloatValue *= - 1;

			// Grounded
			if (movementData[UseGroundMovementWhenGroundedIndex] == null) movementData[UseGroundMovementWhenGroundedIndex] = new MovementVariable();
			movementData[UseGroundMovementWhenGroundedIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Use Ground Movement", "When we stand on the ground should we use a ground movement."), movementData[UseGroundMovementWhenGroundedIndex].BoolValue);

			GUILayout.Label ("Stroke", EditorStyles.boldLabel);

			// Stroke Speed
			if (movementData[StrokeSpeedIndex] == null) movementData[StrokeSpeedIndex] = new MovementVariable(DefaultStrokeSpeed);
			movementData[StrokeSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Stroke Time", "How long does a swimming stroke say. 2.0 would mean the character can stroke every 2 seconds. Negative values for no strokes."), movementData[StrokeSpeedIndex].FloatValue);
			if (movementData [StrokeSpeedIndex].FloatValue < 0) movementData [StrokeSpeedIndex].FloatValue = 1.0f;

			// Stroke Acceleration
			if (movementData[StrokeAccelerationIndex] == null) movementData[StrokeAccelerationIndex] = new MovementVariable(DefaultStrokeAcceleration);
			movementData[StrokeAccelerationIndex].Vector2Value = EditorGUILayout.Vector2Field(new GUIContent("Stroke Velocity", "The velocity to add with each stroke. X value is switched based on last faced direction."), movementData[StrokeAccelerationIndex].Vector2Value);


			GUILayout.Label ("X Movement", EditorStyles.boldLabel);
			// Max Speed
			if (movementData[MaxSpeedInXIndex] == null) movementData[MaxSpeedInXIndex] = new MovementVariable(DefaultMaxSpeed);
			movementData[MaxSpeedInXIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Maximum Speed", "The characters peak speed in x."), movementData[MaxSpeedInXIndex].FloatValue);
			if (movementData[MaxSpeedInXIndex].FloatValue < 0) movementData[MaxSpeedInXIndex].FloatValue = 0.0f;

			// Acceleration
			if (movementData[XAccelerationIndex] == null) movementData[XAccelerationIndex] = new MovementVariable(DefaultAcceleration);
			movementData[XAccelerationIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("X Acceleration", "How fast the character accelerates in X, when not stroking."), movementData[XAccelerationIndex].FloatValue);
			if (movementData[XAccelerationIndex].FloatValue < 0) movementData[XAccelerationIndex].FloatValue = 0.0f;

			// Drag
			if (movementData[DragIndex] == null) movementData[DragIndex] = new MovementVariable(DefaultDrag);
			movementData[DragIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Drag Index", "Drag to apply."), movementData[DragIndex].FloatValue);
			if (movementData[DragIndex].FloatValue < 0) movementData[DragIndex].FloatValue = 0.0f;

			return movementData;
		}

		#endregion

#endif

	}

}

