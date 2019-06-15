#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A special movement which starts the character swimming when the enter the water. The directional version applies swim
	/// force in the direction the character is facing.
	/// </summary>
	public class SpecialMovement_DirectionalSwim : SpecialMovement_Swim
	{

		#region members

		/// <summary>
		/// When stroking should we face in held direction, if false we face in velocity direction.
		/// </summary>
		public bool faceInHeldDirection;

		/// <summary>
		/// What type of swim is this?
		/// </summary>
		public DirectionalSwimType swimType;

		/// <summary>
		/// Swim speed used when not stroking (for DirectionSwimType.SWIM_PLUS_STROKE).
		/// </summary>
		public float swimSpeed;
		
		/// <summary>
		/// The swim  timer for non-stroke, swims.
		/// </summary>
		protected float limitVelocityTimer;

		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Swim/Directional";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "A swim in which character can swim in any direction. Must be triggered by an EventResponser with the START_SWIM response.";
		
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
		/// The index for face in held direction.
		/// </summary>
		protected const int FaceInHeldDirectionIndex = 8;

		/// <summary>
		/// The index of hold jump to swim.
		/// </summary>
		protected const int HoldJumpToSwimIndex = 9;

		/// <summary>
		/// The index ofswim speed for SWIM_PLUS_STROKE
		/// </summary>
		protected const int SwimSpeedIndex = 10;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 11;

		/// <summary>
		/// The default swim gravity.
		/// </summary>
		new protected const float DefaultSwimGravity = 0f;

		/// <summary>
		/// The default terminal speed.
		/// </summary>
		new protected const float DefaultTerminalVelocity = -3.5f;

		/// <summary>
		/// The default stroke speed.
		/// </summary>
		new protected const float DefaultStrokeSpeed = 0.75f;

		/// <summary>
		/// The default stroke acceleration.
		/// </summary>
		new protected static Vector2 DefaultStrokeAcceleration = new Vector2 (5, 0);

		/// <summary>
		/// If going slower than this don't rotate (in velocity mode).
		/// </summary>
		protected const float MinSpeedForRotate = 0.5f;

		#endregion

		/// <summary>
		/// Gets the direction we are swimming in.
		/// </summary>
		/// <value>The swim direction.</value>
		virtual public Vector2 SwimDirection
		{
			get
			{
                Vector2 dir = new Vector2(character.Input.HorizontalAxis, character.Input.VerticalAxis).normalized;
                if (dir.magnitude == 0 && swimType == DirectionalSwimType.SWIM_PLUS_STROKE && swimStrokeTimer > 0)
                { 
                    dir = new Vector2(character.FacingDirection, 2).normalized;
                }
                return dir;
			}
		}

		/// <summary>
		/// Gets the direction we should face.
		/// </summary>
		/// <value>The direction we should face.</value>
		virtual public Vector2 SwimFacingDirection
		{
			get
			{
				if (swimType == DirectionalSwimType.SWIM_PLUS_STROKE)
				{
					if (faceInHeldDirection) 
					{
						if (character.Input.HorizontalAxis == 0 && character.Input.VerticalAxis == 0) return Vector2.up;
						return new Vector2(character.Input.HorizontalAxis, character.Input.VerticalAxis).normalized;
					}
					else
					{
						if (character.Velocity.magnitude < MinSpeedForRotate) return Vector2.up;
						return character.Velocity.normalized;
					}
				}

				if (!IsStroking()) return Vector2.up;

				if (faceInHeldDirection) 
				{
					if (character.Input.HorizontalAxis == 0 && character.Input.VerticalAxis == 0) return Vector2.up;
					return new Vector2(character.Input.HorizontalAxis, character.Input.VerticalAxis).normalized;
				}
				else
				{
					if (character.Velocity.magnitude < MinSpeedForRotate) return Vector2.up;
					return character.Velocity.normalized;
				}
			}
		}

		#region Unity Hooks

		/// <summary>
		/// Unity Update hook
		/// </summary>
		void Update()
		{
			if (swimStarted && swimStrokeTimer > 0) swimStrokeTimer -= TimeManager.FrameTime;
			if (swimStarted && limitVelocityTimer > 0) limitVelocityTimer -= TimeManager.FrameTime;
		}

		#endregion

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
				useGroundMovementWhenGrounded = movementData[UseGroundMovementWhenGroundedIndex].BoolValue;
				maxSpeedInX = movementData[MaxSpeedInXIndex].FloatValue;
				swimTerminalVelocity = movementData[SwimTerminalVelocityIndex].FloatValue;
				xAcceleration = movementData[XAccelerationIndex].FloatValue;
				drag = movementData[DragIndex].FloatValue;
				strokeSpeed = movementData[StrokeSpeedIndex].FloatValue;
				// By setting this we ensure we can't jump as soon as we hit the water
				swimStrokeTimer = strokeSpeed / 2.0f;	
				limitVelocityTimer = strokeSpeed / 2.0f;	
				strokeAcceleration = movementData[StrokeAccelerationIndex].Vector2Value;
				faceInHeldDirection = movementData[FaceInHeldDirectionIndex].BoolValue;
				swimType = (DirectionalSwimType) movementData[HoldJumpToSwimIndex].IntValue;
				swimSpeed = movementData[SwimSpeedIndex].FloatValue;
			}
			else
			{
				Debug.LogError("Invalid movement data.");
			}
			return this;
		}


		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{

            if (character.Velocity.magnitude > maxSpeedInX)
            {
                // We are being pushed hard do nothing but drag
                ApplyDrag();
                if (isOnSurface && character.Velocity.y > 0) character.SetVelocityY(0);
            }
            else 
            { 

                if (character.Velocity.magnitude < swimSpeed) limitVelocityTimer = 0;

    			// Stroke
    			if (swimStrokeTimer <= 0 && IsStroking())
    			{
    				swimStrokeTimer = strokeSpeed;
    				// TODO Is 2 the right number? Should at least make a Constant or maybe a variable?
    				limitVelocityTimer = strokeSpeed * 25.0f;
    				Vector2 swimForce = strokeAcceleration.x * SwimDirection;
    				if (isOnSurface && swimForce.y > 0) swimForce.y = 0;
    				character.AddVelocity(swimForce.x, swimForce.y, true);
    			}
    			// Base swim acceleration
    			else if (swimType == DirectionalSwimType.SWIM_PLUS_STROKE && limitVelocityTimer <= 0 && SwimDirection.magnitude > 0.01f)
    			{
    				Vector2 swimForce = strokeAcceleration.x * SwimDirection;
    				if (isOnSurface && swimForce.y > 0) swimForce.y = 0;
    				// Quarter acceleration of normal stroke (Should we make this a variable)
    				swimForce *= 0.5f;
    				character.AddVelocity(swimForce.x, swimForce.y, true);
    			}

    			// Apply gravity if not grounded and not swimming and not on surface
    			if ((!character.Grounded) && 
    			    (SwimDirection.magnitude < 0.01f || (!IsStroking() && swimType != DirectionalSwimType.SWIM_PLUS_STROKE)) && 
    			    // ( || ((swimType == DirectionalSwimType.JUMP_TO_STROKE && SwimDirection.magnitude < 0.01f))) && 
    			    // If we hit our head we don't want to sink straight away as it looks weird (numbers are a bit magic but look right)
    			    (swimStrokeTimer <= (strokeSpeed / 2.5f) || !character.WouldHitHeadThisFrame))
    			{
    				// Gravity
    				character.AddVelocity(0, TimeManager.FrameTime * swimGravity, false);
    				// Limit to swim terminal
    				if (character.Velocity.y < swimTerminalVelocity) character.AddVelocity(0, -character.Velocity.y * drag * TimeManager.FrameTime, true);
    				// if (character.Velocity.y > 0) character.SetVelocityY(0);

                }

    			// Fall acceleration / Grounded acceleration
    			if ( character.Grounded || (swimGravity < 0 && swimStrokeTimer <= 0 && !IsStroking()))
    			{
    				character.AddVelocity((float)character.Input.HorizontalAxisDigital * xAcceleration * TimeManager.FrameTime, 0, true);
    			}

    			// Only limit to swim speed for non-stroke swimming.
    			if (swimType == DirectionalSwimType.SWIM_PLUS_STROKE && SwimDirection.magnitude > 0.01f && limitVelocityTimer <= 0 && character.Velocity.magnitude > swimSpeed)
    			{
    				Vector2 actualSwimDirection = character.Velocity.normalized;
    				// Apply drag for non-stroke swimming
    				Vector2 actualSwimSpeed = actualSwimDirection * swimSpeed;
    				character.SetVelocityX(actualSwimSpeed.x);
    				character.SetVelocityY(actualSwimSpeed.y);

    			}
    			else
    			{
                    ApplyDrag();
    			}


    			// Limit to max speed
    			if (character.Velocity.magnitude > maxSpeedInX)
    			{
    				Vector2 actualSwimDirection = character.Velocity.normalized;
    				Vector2 actualSwimSpeed = actualSwimDirection * maxSpeedInX;
    				character.SetVelocityX(actualSwimSpeed.x);
    				character.SetVelocityY(actualSwimSpeed.y);
    			}

            }
            // Don't allow to swim above surface
            if (isOnSurface && character.Velocity.y > 0) character.SetVelocityY(0);

            character.Translate(character.Velocity.x * TimeManager.FrameTime, character.Velocity.y * TimeManager.FrameTime, true);
           
            // End surface swim if moving down
            if (isOnSurface && character.Velocity.y < 0) EndSurfaceSwim();

        }

        virtual protected void ApplyDrag() 
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
            if (character.Velocity.y > 0)
            {
                character.AddVelocity(0, -character.Velocity.y * drag * TimeManager.FrameTime, true);
                if (character.Velocity.y < 0) character.SetVelocityY(0);
            }
            else if (character.Velocity.y < 0)
            {
                character.AddVelocity(0, -character.Velocity.y * drag * TimeManager.FrameTime, true);
                if (character.Velocity.y > 0) character.SetVelocityY(0);
            }
        }

        /// <summary>
        /// Awe we holding the stroke key?
        /// </summary>
        virtual protected bool IsStroking()
		{
			return (swimType == DirectionalSwimType.AUTO_STROKE || 
			        (character.Input.JumpButton == ButtonState.HELD && swimType == DirectionalSwimType.JUMP_TO_STROKE) ||
			        (character.Input.JumpButton == ButtonState.DOWN && swimType == DirectionalSwimType.SWIM_PLUS_STROKE));
		}

        /// <summary>
        /// Starts the swim.
        /// </summary>
        public override void StartSwim()
        {
            base.StartSwim();
            limitVelocityTimer = strokeSpeed * 25.0f;
            if (character.Velocity.y < -swimSpeed)
            {
                // When we hit the water if we are going really fast slow
                character.SetVelocityY((character.Velocity.y - swimSpeed) / 2.0f);
            }
        }

        /// <summary>
        /// 
        /// Starts Surface Swim.
        /// </summary>
        override public void SurfaceSwim()
		{
			isOnSurface = true;

        }

        /// <summary>
        /// 
        /// Starts Surface Swim.
        /// </summary>
        override public void EndSurfaceSwim()
        {
            isOnSurface = false;
        }



        /// <summary>
        /// Gets a value indicating whether this movement wants to do a special move.
        /// </summary>
        /// <value><c>true</c> if this instance wants control; otherwise, <c>false</c>.</value>
        override public bool WantsSpecialMove()
		{
			if (isOnSurface && character.Input.JumpButton == ButtonState.DOWN) return false;
			return swimStarted;
		}

		/// <summary>
		/// If this is true then the movement wants to maintain control of the character even
		/// if default transition conditions suggest it shouldn't.
		/// </summary>
		/// <returns><c>true</c> if control is wanted, <c>false</c> otherwise.</returns>
		override public bool ForceMaintainControl()
		{
			// if (isOnSurface && character.Input.JumpButton == ButtonState.DOWN) return false;
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
			limitVelocityTimer = strokeSpeed / 2.0f;
			// Jump out of the water
			character.DefaultAirMovement.DoOverridenJump (1.5f, 2);
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
		new public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{
			if (movementData != null && (movementData.Length == MovementVariableCount - 1 || movementData.Length == MovementVariableCount - 2))
			{
				Debug.LogWarning("Upgrading SpecialMovement_Swim movement data. Double check new values.");
				MovementVariable[] tmp = movementData;
				movementData = new MovementVariable[MovementVariableCount];
				System.Array.Copy(tmp, movementData, tmp.Length);
			}
			else if (movementData == null || movementData.Length != MovementVariableCount)
			{
				movementData = new MovementVariable[MovementVariableCount];
			}

			EditorGUILayout.LabelField ("General", EditorStyles.boldLabel);

			// Grounded
			if (movementData[UseGroundMovementWhenGroundedIndex] == null) movementData[UseGroundMovementWhenGroundedIndex] = new MovementVariable();
			movementData[UseGroundMovementWhenGroundedIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Use Ground Movement", "When we stand on the ground should we use a ground movement."), movementData[UseGroundMovementWhenGroundedIndex].BoolValue);

			// Facing
			if (movementData[FaceInHeldDirectionIndex] == null) movementData[FaceInHeldDirectionIndex] = new MovementVariable();
			movementData[FaceInHeldDirectionIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Face in Held Direction", "Should we face the direction of swim (false) or the direciton of input (true)."), movementData[FaceInHeldDirectionIndex].BoolValue);


			GUILayout.Label ("Swimming", EditorStyles.boldLabel);

			// Swim Type
			if (movementData[HoldJumpToSwimIndex] == null) movementData[HoldJumpToSwimIndex] = new MovementVariable();
			movementData[HoldJumpToSwimIndex].IntValue = (int) (DirectionalSwimType) EditorGUILayout.EnumPopup(new GUIContent("Swim Type", "What type of swim is this?"), (DirectionalSwimType) movementData[HoldJumpToSwimIndex].IntValue);
			EditorGUILayout.HelpBox (((DirectionalSwimType)(movementData [HoldJumpToSwimIndex].IntValue)).GetDescription(), MessageType.Info);

			// Stroke Speed
			if (movementData[SwimSpeedIndex] == null) movementData[SwimSpeedIndex] = new MovementVariable(0);
			if (((DirectionalSwimType)movementData [HoldJumpToSwimIndex].IntValue) == DirectionalSwimType.SWIM_PLUS_STROKE)
			{
				movementData [SwimSpeedIndex].FloatValue = EditorGUILayout.FloatField (new GUIContent ("Base Swim Speed", "How fast to swim when not stroking."), movementData [SwimSpeedIndex].FloatValue);
				if (movementData [SwimSpeedIndex].FloatValue < 0) movementData [SwimSpeedIndex].FloatValue = 1.0f;
			}

			// Stroke Speed
			if (movementData[StrokeSpeedIndex] == null) movementData[StrokeSpeedIndex] = new MovementVariable(DefaultStrokeSpeed);
			movementData[StrokeSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Stroke Time", "How long does a swimming stroke say. 2.0 would mean the character can stroke every 2 seconds. Negative values for no strokes."), movementData[StrokeSpeedIndex].FloatValue);
			if (movementData [StrokeSpeedIndex].FloatValue < 0) movementData [StrokeSpeedIndex].FloatValue = 1.0f;

			// Stroke Acceleration
			if (movementData[StrokeAccelerationIndex] == null) movementData[StrokeAccelerationIndex] = new MovementVariable(DefaultStrokeAcceleration);
			movementData[StrokeAccelerationIndex].Vector2Value = new Vector2(EditorGUILayout.FloatField(new GUIContent("Stroke Velocity", "The velocity to add with each stroke in the facing direction"), movementData[StrokeAccelerationIndex].Vector2Value.x), 0);

			// Max Speed
			if (movementData[MaxSpeedInXIndex] == null) movementData[MaxSpeedInXIndex] = new MovementVariable(DefaultMaxSpeed);
			movementData[MaxSpeedInXIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Maximum Speed", "The characters peak speed."), movementData[MaxSpeedInXIndex].FloatValue);
			if (movementData[MaxSpeedInXIndex].FloatValue < 0) movementData[MaxSpeedInXIndex].FloatValue = 0.0f;

			// Drag
			if (movementData[DragIndex] == null) movementData[DragIndex] = new MovementVariable(DefaultDrag);
			movementData[DragIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Drag", "Drag to apply (i.e. how fast do we slow down)."), movementData[DragIndex].FloatValue);
			if (movementData[DragIndex].FloatValue < 0) movementData[DragIndex].FloatValue = 0.0f;

			GUILayout.Label ("Falling", EditorStyles.boldLabel);

			// Swim Gravity
			if (movementData[SwimGravityIndex] == null) movementData[SwimGravityIndex] = new MovementVariable(DefaultSwimGravity);
			movementData[SwimGravityIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Swim Gravity", "Gravity to use for swimming (negative value), typically much lower than standard gravity. Use 0 if you wan't to be able to float on the spot."), movementData[SwimGravityIndex].FloatValue);
			if (movementData [SwimGravityIndex].FloatValue > 0) movementData [SwimGravityIndex].FloatValue *= - 1;

			// Acceleration
			if (movementData[XAccelerationIndex] == null) movementData[XAccelerationIndex] = new MovementVariable(DefaultAcceleration);
			if (movementData [SwimGravityIndex].FloatValue == 0 && !movementData[UseGroundMovementWhenGroundedIndex].BoolValue)
			{
				EditorGUILayout.HelpBox("You must set a negative gravity or use grounded movement if you want to allow control while not stroking.", MessageType.Info);
				movementData[XAccelerationIndex].FloatValue = 0.0f;
			}
			else
			{
				movementData[XAccelerationIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("X Acceleration", "How fast the character accelerates in X, when not stroking."), movementData[XAccelerationIndex].FloatValue);
				if (movementData[XAccelerationIndex].FloatValue < 0) movementData[XAccelerationIndex].FloatValue = 0.0f;
			}

			// Swim Terminal Velocity
			if (movementData[SwimTerminalVelocityIndex] == null) movementData[SwimTerminalVelocityIndex] = new MovementVariable(DefaultTerminalVelocity);
			if (movementData [SwimGravityIndex].FloatValue < 0)
			{
				movementData[SwimTerminalVelocityIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Terminal Velocity", "Max speed we can fall at in water when not stroking."), movementData[SwimTerminalVelocityIndex].FloatValue);
				if (movementData[SwimTerminalVelocityIndex].FloatValue > 0) movementData[SwimTerminalVelocityIndex].FloatValue = 0.0f;
			}
			else
			{
				movementData[SwimTerminalVelocityIndex].FloatValue = 0.0f;
			}

			return movementData;
		}

		#endregion

#endif

	}

	/// <summary>
	/// Directional swim type.
	/// </summary>
	public enum DirectionalSwimType
	{
		AUTO_STROKE,
		JUMP_TO_STROKE,
		SWIM_PLUS_STROKE
	}

	/// <summary>
	/// Directional swim type extensions. Used to get human readable descriptions of swim types.
	/// </summary>
	public static class DirectionalSwimTypeExtensions
	{
		public static string GetDescription(this DirectionalSwimType me)
		{
			switch(me)
			{
			case DirectionalSwimType.AUTO_STROKE: return "Use strokes to swim automatically. No need to press a button.";
			case DirectionalSwimType.JUMP_TO_STROKE: return "Hover in place, use JUMP to stroke and thus swim.";
			case DirectionalSwimType.SWIM_PLUS_STROKE: return "Gently swim in any direction, press JUMP to stroke and swim faster.";
			}
			return "No information available.";
		}
	}
}

