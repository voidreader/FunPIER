using UnityEngine;
using System.Collections;
using PlatformerPro;

public class AirMovement_DigitalInFacingDirection : AirMovement_Digital
{
	/// <summary>
	/// Direction we are facing.
	/// </summary>
	protected int facingDirection;

	/// <summary>
	/// Human readable name.
	/// </summary>
	private const string Name = "Digital/Jump In Facing Direction";
	
	/// <summary>
	/// Human readable description.
	/// </summary>
	private const string Description = "Air movement which moves only in the facing direction.";
	
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
	/// Does the X movement.
	/// </summary>
	override protected void MoveInX (float horizontalAxis, int horizontalAxisDigital, ButtonState runButton)
	{
		// Check side collissions if we find one, switch facing direction
		if (CheckSideCollisions (character, 1, character.LastFacedDirection == 1 ? RaycastType.SIDE_RIGHT : RaycastType.SIDE_LEFT))
		{
			facingDirection *= -1;
		}

		if (character.LastFacedDirection == 1)
		{
			character.SetVelocityX(character.IsGravityFlipped ? -airSpeed : airSpeed);
			character.Translate((character.IsGravityFlipped ? -airSpeed : airSpeed) * TimeManager.FrameTime, 0, false);
		}
		else
		{
			character.SetVelocityX(character.IsGravityFlipped ? airSpeed : -airSpeed);
			character.Translate((character.IsGravityFlipped ? airSpeed : -airSpeed) * TimeManager.FrameTime, 0, false);
		}
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
		// Make sure we call the base
		base.GainControl ();
	}

	/// <summary>
	/// Do the jump.
	/// </summary>
	override public void DoJump()
	{
		// Set to zero
		facingDirection = 0;
		// Now get direction from the character
		facingDirection = character.LastFacedDirection;
		base.DoJump ();
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
}
