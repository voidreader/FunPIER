using UnityEngine;
using System.Collections;

namespace PlatformerPro
{

	public class AirMovement_CrouchJump : AirMovement_Wrapper {

		/// <summary>
		/// Stores if we are crouching.
		/// </summary>
		protected bool isCrouching;

		protected GroundMovement_Crouch crouchMovement;

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Wrapper/Crouch Jump";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Air movement which wraps another jump but adds the ability to crouch during the jump. Cannot be the default air movement.";
		
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
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				AnimationState baseState = base.AnimationState;
				if (isCrouching)
				{
					switch (baseState)
					{
						case AnimationState.JUMP : return AnimationState.JUMP_CROUCH;
						case AnimationState.FALL : return AnimationState.FALL_CROUCH;
						default:
							return AnimationState.AIRBORNE_CROUCH;
					}

				}
				return baseState;
			}
		}


		#endregion

		#region public methods

		/// <summary>
		/// Initialise this instance.
		/// </summary>
		override public Movement Init(Character character)
		{
			base.Init (character);
			crouchMovement = character.GetComponentInChildren<GroundMovement_Crouch> ();
			if (crouchMovement == null)
			{
				Debug.LogWarning ("Couldn't find a crouch movement, crouch jump will be disabled.");
				Enabled = false;
			}
			return this;
		}
		
		/// <summary>
		/// Initialise the movement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			base.Init (character, movementData);
			crouchMovement = character.GetComponentInChildren<GroundMovement_Crouch> ();
			if (crouchMovement == null)
			{
				Debug.LogWarning ("Couldn't find a crouch movement, crouch jump will be disabled.");
				Enabled = false;
			}
			return this;
		}

		/// <summary>
		/// Gets a value indicating whether this movement wants to control the movement in the air.
		/// Default is false with movement falling back to default air. Override if you want control.
		/// </summary>
		/// <value>true</value>
		/// <c>false</c>
		/// <returns><c>true</c>, if air control was wantsed, <c>false</c> otherwise.</returns>
		override public bool WantsAirControl()
		{
			if (!character.Grounded)
			{
				if (!isCrouching && crouchMovement.CheckInput())
				{
					crouchMovement.Shrink();
					isCrouching = true;
				}
				else if (isCrouching && !crouchMovement.CheckInput())
				{
					crouchMovement.Grow();
					isCrouching = false;
				}
				if (isCrouching) return true;
			}
			// return base.WantsAirControl ();
			return false;
		}

		/// <summary>
		/// Gets a value indicating whether this movement wants to intiate the jump.
		/// </summary>
		/// <value><c>true</c> ijumpTimerf this instance should jump; otherwise, <c>false</c>.</value>
		override public bool WantsJump()
		{
			if (!isCrouching && crouchMovement.CheckInput())
			{
				crouchMovement.Shrink();
				isCrouching = true;
			}
			else if (isCrouching && !crouchMovement.CheckInput())
			{
				crouchMovement.Grow();
				isCrouching = false;
			}
			if (isCrouching) return character.DefaultAirMovement.WantsJump ();
			return false; // character.DefaultAirMovement.WantsJump ();
		}

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
// 			Debug.Log ("move: " + isCrouching + "   " + crouchMovement.CheckInput());
			// Check for crouch/uncrouch
			if (!isCrouching && crouchMovement.CheckInput())
			{
				crouchMovement.Shrink();
				isCrouching = true;
			}
			else if (isCrouching && !crouchMovement.CheckInput())
			{
				crouchMovement.Grow();
				isCrouching = false;
			}
			base.DoMove ();
		}

		override public void LosingControl()
		{
			if (isCrouching)
			{
				crouchMovement.Grow();
				isCrouching = false;
				base.LosingControl ();
			}
			// base.LosingControl ();
		}

		#endregion
	}
}
