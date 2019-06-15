using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Ground movement for walking up and down stairs.
	/// </summary>
	public class GroundMovement_Stairs : GroundMovement
	{
		/// <summary>
		/// The current step.
		/// </summary>
		protected int step;

		/// <summary>
		/// Are we climbing.
		/// </summary>
		protected bool isClimbing;

		/// <summary>
		/// Are we descending.
		/// </summary>
		protected bool isDescending;

		/// <summary>
		/// Reference to the animator we use to determine when stiar movements have finished.
		/// </summary>
		protected Animator myAnimator;

		/// <summary>
		/// Current stair case.
		/// </summary>
		protected Stairs currentStairs;

		/// <summary>
		/// The facing direction.
		/// </summary>
		protected int facingDirection;

		/// <summary>
		/// Are we dismounting the bootm of steps?
		/// </summary>
		protected bool dismountDown;

		/// <summary>
		/// Are we dismounting the top of the steps?
		/// </summary>
		protected bool dismountUp;

		/// <summary>
		/// Did we just change direction.
		/// </summary>
		protected bool switchDirection;

		#region movement info constants and properties

		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Stairs";

		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "GroundMovement for walking up and down stairs";

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

		/// <summary>
		/// Initialise the movement with the given movement data (movemnt data is ignored as its not used by this movement).
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			AssignReferences (character);
			myAnimator = character.GetComponentInChildren<Animator>();
			if (myAnimator == null) 
			{
				Debug.LogWarning("Cannot find animator. The stair movement requires an animator to determine when climbing animations have finished.");
				this.Enabled = false;
			}
			return this;
		}

		/// <summary>
		/// Gets a value indicating whether this movement wants to control the movement on the ground.
		/// Default is false, with control falling back to default ground. Override if you want particular control.
		/// </summary>
		/// <value><c>true</c> if this instance wants control; otherwise, <c>false</c>.</value>
		override public bool WantsGroundControl()
		{
			if (character.StoodOnPlatform is Stairs) {
				currentStairs = (Stairs)character.StoodOnPlatform;
				step = currentStairs.GetStepForPosition (character.transform.position);
				if (step > 0 && step <= currentStairs.LastStep) return true;
			}
			return false;
		}

		/// <summary>
		/// Called when the movement gets control. Typically used to do initialisation of velocity and the like.
		/// </summary>
		override public void GainControl()
		{
			facingDirection = character.LastFacedDirection;
			character.GroundCollider = currentStairs.GetComponent<Collider2D> ();
			character.GroundLayer = character.GroundCollider.gameObject.layer;
		}

		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions.
		/// </summary>
		override public void LosingControl()
		{
			Reset ();
		}

		/// <summary>
		/// Clear all our variables.
		/// </summary>
		protected void Reset() {
			dismountUp = false;
			dismountDown = false;
			isClimbing = false;
			isDescending = false;
		}

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			// Align to step
			if (!(character.StoodOnPlatform is Stairs))
			{
				Debug.LogWarning ("Stairs had control but player wasn't on stairs");
				return;
			}

			switchDirection = false;

			currentStairs = (Stairs)character.StoodOnPlatform;
			step = currentStairs.GetStepForPosition (character.transform.position);
			if (step < 0) return;

			Vector2 targetPosition = currentStairs.GetPositionForStep (step);
			targetPosition.y += character.transform.position.y - character.BottomOfFeet;
			Vector2 delta = (Vector2)character.transform.position - targetPosition;
			character.Translate (-delta.x, -delta.y, true);

			if (isClimbing)
			{
				if ((step >= currentStairs.LastStep && currentStairs.StepDirection > 0) ||
				    (step == 1 && currentStairs.StepDirection < 0))
				{
					dismountUp = true;
				}
				AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
				if (info.IsName (AnimationState.STAIRS_UP_LEFT.AsString()) || 
					info.IsName (AnimationState.STAIRS_UP_RIGHT.AsString()) ||
					info.IsName (AnimationState.STAIRS_UP_TO_GROUND_RIGHT.AsString()) ||
					info.IsName (AnimationState.STAIRS_UP_TO_GROUND_LEFT.AsString()))
				{
					if (info.normalizedTime >= 1.0f) {
						if (dismountUp)
						{
							character.Translate (((Stairs)character.StoodOnPlatform).StepDirection * currentStairs.StepSize.x, 0, true);
							step = -1;
						}
						else
						{
							character.Translate (((Stairs)character.StoodOnPlatform).StepDirection * currentStairs.StepSize.x, currentStairs.StepSize.y, true);
						}
						isClimbing = false;
						dismountUp = false;
					}
				}
			}
			else if (isDescending)
			{
				if ((step == 1 && currentStairs.StepDirection > 0) ||
					(step >= currentStairs.LastStep  && currentStairs.StepDirection < 0))
				{
					dismountDown = true;
				}
				AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
				if (info.IsName (AnimationState.STAIRS_DOWN_LEFT.AsString()) || 
					info.IsName (AnimationState.STAIRS_DOWN_RIGHT.AsString()) ||
					info.IsName (AnimationState.STAIRS_DOWN_TO_GROUND.AsString()))
				{
					if (info.normalizedTime >= 1.0f) {
						character.Translate (((Stairs)character.StoodOnPlatform).StepDirection * -currentStairs.StepSize.x, -currentStairs.StepSize.y, true);
						isDescending = false;
						dismountDown = false;
					}
				}

			}
			else if (character.Input.HorizontalAxisDigital == 1 && currentStairs.StepDirection > 0)
			{
				isClimbing = true;
				if (facingDirection == -1)
				{
					switchDirection = true;
					character.Translate (((Stairs)character.StoodOnPlatform).StepDirection * currentStairs.StepSize.x, currentStairs.StepSize.y, true);
					Reset ();	
				}
				facingDirection = 1;
			}
			else if (character.Input.HorizontalAxisDigital == -1 && currentStairs.StepDirection < 0)
			{
				isClimbing = true;
				if (facingDirection == 1)
				{
					switchDirection = true;
					character.Translate (((Stairs)character.StoodOnPlatform).StepDirection * currentStairs.StepSize.x, currentStairs.StepSize.y, true);
					Reset ();	
				}
				facingDirection = -1;
			}
			else if ((character.Input.HorizontalAxisDigital == -1 || step == currentStairs.LastStep) && currentStairs.StepDirection > 0)
			{
				isDescending = true;
				if (facingDirection == 1)
				{
					switchDirection = true;
					character.Translate (((Stairs)character.StoodOnPlatform).StepDirection * -currentStairs.StepSize.x, -currentStairs.StepSize.y, true);
					Reset ();	
				}
				facingDirection = -1;
			}
			else if ((character.Input.HorizontalAxisDigital == 1 || step == currentStairs.LastStep) && currentStairs.StepDirection < 0)
			{
				isDescending = true;
				if (facingDirection == -1)
				{
					switchDirection = true;
					character.Translate (((Stairs)character.StoodOnPlatform).StepDirection * -currentStairs.StepSize.x, -currentStairs.StepSize.y, true);
					Reset ();	
				}
				facingDirection = 1;
			}
		}

		/// <summary>
		/// Always grounded when on a step.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		override public bool IsGrounded()
		{
			return true;
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="PlatformerPro.Movement"/> expects
		/// gravity to be applied after its movement finishes. In this case false, Stairs ignore gravity.
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
		/// base collisions to be executed after its movement finishes. In this case NONE.
		/// </summary>
		override public RaycastType ShouldDoBaseCollisions
		{
			get
			{
				return RaycastType.NONE;
			}
		}

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		/// <value>The state of the animation.</value>
		override public AnimationState AnimationState
		{
			get 
			{
				// If we aren't on a step just keep going with the previous animation until movement changes (this happens during dimsount up).
				if (step == -1) return character.AnimationState;

				if (dismountDown) return AnimationState.STAIRS_DOWN_TO_GROUND;

				if (step % 2 == 0)
				{
					if (!switchDirection)
					{
						if (dismountUp) return AnimationState.STAIRS_UP_TO_GROUND_RIGHT;
						if (isDescending) return AnimationState.STAIRS_DOWN_RIGHT;
						if (isClimbing) return AnimationState.STAIRS_UP_RIGHT;
					}
					if (facingDirection == 1 && currentStairs.StepDirection > 0) return AnimationState.STAIRS_UP_RIGHT_IDLE;
					if (facingDirection == -1 && currentStairs.StepDirection < 0) return AnimationState.STAIRS_UP_RIGHT_IDLE;
					return AnimationState.STAIRS_DOWN_RIGHT_IDLE;
				}
				if (!switchDirection)
				{
					if (dismountUp) return AnimationState.STAIRS_UP_TO_GROUND_LEFT;
					if (isDescending) return AnimationState.STAIRS_DOWN_LEFT;
					if (isClimbing) return AnimationState.STAIRS_UP_LEFT;
				}
				if (facingDirection == 1 && currentStairs.StepDirection > 0) return AnimationState.STAIRS_UP_LEFT_IDLE;
				if (facingDirection == -1 && currentStairs.StepDirection < 0) return AnimationState.STAIRS_UP_LEFT_IDLE;
				return AnimationState.STAIRS_DOWN_LEFT_IDLE;
			}
		}


		/// <summary>
		/// Returns the direction the character is facing. 0 for none, 1 for right, -1 for left.
		/// </summary>
		override public int FacingDirection
		{
			get 
			{
				return facingDirection;
			}
		}
	}
}
