using UnityEngine;
using System.Collections;


namespace PlatformerPro
{
	/// <summary>
	/// Air movement which wraps another air movement AND a crouch in order to allow crouching while jumping.
	/// </summary>
	public abstract class AirMovement_Wrapper : AirMovement
	{
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "";
		
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
		
		#endregion

		#region Properties

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
				return character.DefaultAirMovement.AnimationPriority;
			}
		}

		#endregion

		#region public methods
		
		/// <summary>
		/// Initialise the movement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			AssignReferences (character);
			return this;
		}

		/// <summary>
		/// Called after initialisation to check if this movement is configured correctly. 
		/// Typically used to stop wrapper movements being the default and ending up in infinite loops.
		/// </summary>
		override public string PostInitValidation()
		{
			if (character.DefaultAirMovement == this) return "Wrapper movement cannot be the default AirMovement. This will cause an infinite loop and editor crash.";
			return base.PostInitValidation();
		}

		/// <summary>
		/// Gets a value indicating whether this movement wants to intiate the jump.
		/// </summary>
		/// <value><c>true</c> ijumpTimerf this instance should jump; otherwise, <c>false</c>.</value>
		override public bool WantsJump()
		{
			return character.DefaultAirMovement.WantsJump ();
		}


		/// <summary>
		/// Do the jump.
		/// </summary>
		override public void DoJump()
		{
			character.DefaultAirMovement.DoJump ();
		}
		
		
		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			character.DefaultAirMovement.DoMove ();
		}

		
		/// <summary>
		/// If the jump just started force control.
		/// </summary>
		override public bool ForceMaintainControl()
		{
			return character.DefaultAirMovement.ForceMaintainControl ();
		}
		
		/// <summary>
		/// Called when the movement loses control. Reset the jump count.
		/// </summary>
		override public void LosingControl()
		{
			character.DefaultAirMovement.LosingControl ();
		}

		/// <summary>
		/// Called when the movement gets control. Typically used to do initialisation of velocity and the like.
		/// </summary>
		override public void GainControl()
		{
			character.DefaultAirMovement.GainControl ();
		}

		/// <summary>
		/// Gets a value indicating whether this movement wants to control the movement in the air.
		/// Default is false with movement falling back to default air. Override if you want control.
		/// </summary>
		/// <value><c>true</c> if this instance wants control; otherwise, <c>false</c>.</value>
		override public bool WantsAirControl()
		{
			return character.DefaultAirMovement.WantsAirControl ();
		}

		/// <summary>
		/// Partially moves the character. A lot of movements (e.g. wall movements) require partial air movement.
		/// This allows them to use the air movement instead of re-implementing the  movement.
		/// </summary>
		/// <param name="moveInX">If set to <c>true</c> move in x.</param>
		/// <param name="moveInY">If set to <c>true</c> move in y.</param>
		/// <param name="xInput">Simulated X input.</param>
		/// <param name="runButton">Simulated Run button.</param>
		/// <summary>
		/// Partially moves the character. A lot of movements (e.g. wall movements) require partial air movement.
		/// This allows them to use the air movement instead of re-implementing the  movement.
		/// </summary>
		override public void DoOverridenMove(bool moveInX, bool moveInY, float xInput, ButtonState runButton)
		{
			character.DefaultAirMovement.DoOverridenMove (moveInX, moveInY, xInput, runButton);
		}

		#endregion
		
		#region protected methods

		/// <summary>
		/// For overriden jump don't delay
		/// </summary>
		override public void DoOverridenJump(float newHeight, int newJumpCount, bool skipPowerUps = false)
		{
			character.DefaultAirMovement.DoOverridenJump (newHeight, newJumpCount);
		}

		#endregion

	}
}
