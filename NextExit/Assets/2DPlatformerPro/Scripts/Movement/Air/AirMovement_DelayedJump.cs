#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Air movement which delays jump until a timer goes off and then gives control to the deault jump. Cannot be the default jump.
	/// </summary>
	public class AirMovement_DelayedJump : AirMovement
	{

		#region members
		
		/// <summary>
		/// Amount of time the jump is delayed for
		/// </summary>
		public float jumpDelay;

		/// <summary>
		/// Should we always set JUMP as the animation state or should we set the underlying animation state from the default air movement.
		/// </summary>
		public bool useJumpAnimationOnly;

		/// <summary>
		/// The jump timer.
		/// </summary>
		protected float jumpTimer;
		
		/// <summary>
		/// Keep track of the current control state.
		/// </summary>
		protected bool controlChecked;

		/// <summary>
		/// True when we should trigger jump next frame.
		/// </summary>
		protected bool shouldJump;

		/// <summary>
		/// True the frame that jump started.
		/// </summary>
		protected bool hasJumped;
		
		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Special/Delayed Jump";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Air movement which delays jump until a timer goes off and then gives control to the deault jump. Cannot be the default jump.";
		
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
		/// The index for the air speed value in the movement data.
		/// </summary>
		protected const int JumpDelayIndex = 0;

		/// <summary>
		/// The index of the jump animation only variable in the movement data.
		/// </summary>
		protected const int JumpAnimationOnlyIndex = 1;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 2;
		
		#endregion
		
		#region properties
		
		/// <summary>
		/// This class will handle gravity internally.
		/// </summary>
		override public bool ShouldApplyGravity
		{
			get
			{
				return character.DefaultAirMovement.ShouldApplyGravity;
			}
		}
		
		
		/// <summary>
		/// Gets a value indicating the current gravity, only used if this
		/// movement doesn't apply the default gravity.
		/// <seealso cref="ShouldApplyGravity()"/>
		/// </summary>
		override public float CurrentGravity
		{
			get
			{
				return character.DefaultAirMovement.CurrentGravity;
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
				return character.DefaultAirMovement.FacingDirection;
			}
		}

		
		
		/// <summary>
		/// How does this movement use Velocity.
		/// </summary>
		override public VelocityType VelocityType
		{
			get
			{
				return character.DefaultAirMovement.VelocityType;
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether this <see cref="PlatformerPro.Movement"/> expects the
		/// rotations to be calculated and applied by the character.
		/// </summary>
		override public bool ShouldDoRotations
		{
			get
			{
				return character.DefaultAirMovement.ShouldDoRotations;
			}
		}

		
		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				if (useJumpAnimationOnly || jumpTimer > 0.0f) return AnimationState.JUMP;
				return character.DefaultAirMovement.AnimationState;
			}
		}
		
		/// <summary>
		/// Gets the priority for the animation state.
		/// </summary>
		override public int AnimationPriority
		{
			get 
			{
				if (useJumpAnimationOnly || jumpTimer > 0.0f) return 5;
				return character.DefaultAirMovement.AnimationPriority;
			}
		}

		#endregion

		#region Unity Hooks
		
		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update()
		{
			if (hasJumped && !character.Grounded) hasJumped = false;
			if (shouldJump) 
			{
				character.DefaultAirMovement.DoJump();
				shouldJump = false;
				controlChecked = false;
				hasJumped = true;
			}
			if (jumpTimer <= 0 && controlChecked)
			{
				controlChecked = false;
				shouldJump = true;
			}
			if (jumpTimer > 0) jumpTimer -= TimeManager.FrameTime;
		}
		
		#endregion
		
		#region public methods
		
		
		
		/// <summary>
		/// Gets a value indicating whether this movement wants to intiate the jump.
		/// </summary>
		/// <value><c>true</c> ijumpTimerf this instance should jump; otherwise, <c>false</c>.</value>
		override public bool WantsJump()
		{
			#if UNITY_EDITOR
			if (character.DefaultAirMovement == this)
			{
				Debug.LogWarning("Delayed jump cannot be the default ait movement, this will cause crashes in a release build.");
				return false;
			}
			#endif
			// If jump timer is active we dont want jump we want control
			if (shouldJump || jumpTimer > 0.0f) return false;
			if (character.DefaultAirMovement.WantsJump())
			{
				controlChecked = true;
				jumpTimer = jumpDelay;
				return true;
			}
			return false;
		}


		/// <summary>
		/// Do the jump.
		/// </summary>
		override public void DoJump()
		{
			// Delay actualy jump until timer expires
		}
		
		
		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{

		}
		
		/// <summary>
		/// Initialise this instance.
		/// </summary>
		override public Movement Init(Character character)
		{
			this.character = character;
			return this;
		}

		/// <summary>
		/// Called after initialisation to check if this movement is configured correctly. 
		/// Typically used to stop wrapper movements being the default and ending up in infinite loops.
		/// </summary>
		override public string PostInitValidation()
		{
			if (character.DefaultAirMovement == this) return "DelayedJump movement cannot be the default AirMovement. This will cause an infinite loop and editor crash.";
			return null;
		}

		/// <summary>
		/// Initialise the movement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			this.character = character;
			
			// Set variables
			if (movementData != null && movementData.Length == MovementVariableCount)
			{
				jumpDelay = movementData[JumpDelayIndex].FloatValue;
				useJumpAnimationOnly = movementData[JumpAnimationOnlyIndex].BoolValue;
			}
			else
			{
				Debug.LogError("Invalid movement data.");
			}
			return this;
		}
		
		/// <summary>
		/// If the jump just started force control.
		/// </summary>
		override public bool WantsControl()
		{
#if UNITY_EDITOR
			if (character.DefaultAirMovement == this)
			{
				Debug.LogWarning("Delayed jump cannot be the default ait movement, this will cause crashes in a release build.");
				return false;
			}
#endif
			controlChecked = true;
			if (hasJumped) return false;
			if (jumpTimer >= 0) return true;
			return true;
		}
		
		/// <summary>
		/// Called when the movement loses control. Reset the jump count.
		/// </summary>
		override public void LosingControl()
		{
			jumpTimer = 0.0f;
			controlChecked = false;
			shouldJump = false;
			hasJumped = false;
		}

		
		
		/// <summary>
		/// Called when the movement gets control. Typically used to do initialisation of velocity and the like.
		/// </summary>
		override public void GainControl()
		{	
		}

		
		#endregion
		
		#region protected methods
		
	
		/// <summary>
		/// For overriden jump don't delay
		/// </summary>
		override public void DoOverridenJump(float newHeight, int newJumpCount)
		{
			if ( newJumpCount == 1 )
				RPGSoundManager.Instance.PlayEffectSound( 1 );
			else if ( newJumpCount == 2 )
				RPGSoundManager.Instance.PlayEffectSound( 2 );
			character.DefaultAirMovement.DoOverridenJump (newHeight, newJumpCount);
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
			
			// Air speed
			if (movementData[JumpDelayIndex] == null) movementData[JumpDelayIndex] = new MovementVariable(0);
			movementData[JumpDelayIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Jump Delay", "How long to wait before jumping."), movementData[JumpDelayIndex].FloatValue);
			if (movementData[JumpDelayIndex].FloatValue < 0) movementData[JumpDelayIndex].FloatValue = 0.0f;

			// Animation
			if (movementData[JumpAnimationOnlyIndex] == null) movementData[JumpAnimationOnlyIndex] = new MovementVariable();
			movementData[JumpAnimationOnlyIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Jump Animation Only", "Should we always set JUMP as the animation state or should we set the underlying animation state from the default air movement."), movementData[JumpAnimationOnlyIndex].BoolValue);
		
			return movementData;
		}
		
		#endregion
		#endif
	}
	
}