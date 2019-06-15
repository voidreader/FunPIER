using UnityEngine;
using System.Collections;
using PlatformerPro;

public class GroundMovement_DigitalInFacingDirection : GroundMovement_Digital
{
	/// <summary>
	/// Direction we are facing.
	/// </summary>
	protected int facingDirection;

	/// <summary>
	/// Human readable name.
	/// </summary>
	private const string Name = "Digital/Run In Facing Direction";
	
	/// <summary>
	/// Human readable description.
	/// </summary>
	private const string Description = "Ground movement which runs only in the facing direction.";
	
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

			
	override public void DoMove()
	{
		// Set frame speed - if friction is bigger than 2 we will slow the character down.
		float frameSpeed = speed;
		if (character.Friction > 2.0f) frameSpeed *= (2.0f / character.Friction );

		// Check side collissions if we find one, switch facing direction
		if (CheckSideCollisions (character, 1, character.LastFacedDirection == 1 ? RaycastType.SIDE_RIGHT : RaycastType.SIDE_LEFT))
		{
			facingDirection *= -1;
		}

		// If facing right run right
		if (character.LastFacedDirection == 1)
		{
			character.SetVelocityX(character.IsGravityFlipped ? -frameSpeed : frameSpeed);
			character.Translate((character.IsGravityFlipped ? -frameSpeed : frameSpeed) * TimeManager.FrameTime, 0, false);
		}
		// Else run left
		else
		{
			character.SetVelocityX(character.IsGravityFlipped ? frameSpeed : -frameSpeed);
			character.Translate((character.IsGravityFlipped ? frameSpeed : -frameSpeed) * TimeManager.FrameTime, 0, false);
		}
	}

	
	/// <summary>
	/// Does any movement that MUST be done after collissions are calculated.
	/// </summary>
	override public void PostCollisionDoMove() {
		if (enabled && !character.rotateToSlopes) SnapToGround ();
	}

	/// <summary>
	/// Called when the movement gets control. Typically used to do initialisation of velocity and the like.
	/// </summary>
	override public void GainControl()
	{
		// Set to zero
		facingDirection = 0;
		// Now get direction from the character
		facingDirection = character.LastFacedDirection;
	}

	/// <summary>
	/// Returns the direction the character is facing. 0 for none, 1 for right, -1 for left.
	/// This overriden version always returns the input direction.
	/// </summary>
	override public int FacingDirection
	{
		get 
		{ 
			return facingDirection;
		}
	}

	/// <summary>
	/// Gets the animation state that this movement wants to set.
	/// </summary>
	override public PlatformerPro.AnimationState AnimationState
	{
		get { return PlatformerPro.AnimationState.WALK; }
	}
}
