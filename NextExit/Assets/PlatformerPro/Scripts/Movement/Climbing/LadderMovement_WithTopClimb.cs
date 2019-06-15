using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A climb movement for ladder climbing which plays an (uninterruptible) animation at the top of the ladder is reached.
	/// WARNING: THIS MOVEMNET CURRENTLY ONLY WORKS SEAMLESLLY WITH 3D CHARACTERS
	/// </summary>
	public class LadderMovement_WithTopClimb : LadderMovement_DigitalForTiles
	{

		#region members

		/// <summary>
		/// When within this distance we start the up climb.
		/// </summary>
		public float upClimbDistance;

		/// <summary>
		/// After the up climb is complete this is the amount we offset the character.
		/// </summary>
		public Vector2 upClimbOffset;

		/// <summary>
		/// After the down climb is complete this is the amount we offset the character.
		/// </summary>
		public Vector2 downClimbOffset;

		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Climb/Ladder/With Animation At Top";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "A climb movement for ladder climbing which plays an (uninterruptible) animation at the top of the ladder is reached. WARNING: THIS MOVEMNET CURRENTLY ONLY WORKS SEAMLESLLY WITH 3D CHARACTERS (contact me if you need it for 2D).";
		
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
		/// The index of the up climb distance in the movement data.
		/// </summary>
		protected const int UpClimbDistanceIndex = 6;

		/// <summary>
		/// The index of the up climb offset in the movement data.
		/// </summary>
		protected const int UpClimbOffsetIndex = 7;

		/// <summary>
		/// The index of the down climb offset in the movement data.
		/// </summary>
		protected const int DownClimbOffsetIndex = 8;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 9;

		#endregion

		#region properties

		/// <summary>
		/// Tracks if the up climb has started.
		/// </summary>
		protected bool upClimbStarted;

		/// <summary>
		/// Tracks if the down climb has started.
		/// </summary>
		protected bool downClimbStarted;

		/// <summary>
		///  Have we applied the up climb offset.
		/// </summary>
		protected bool upClimbOffsetApplied;

		/// <summary>
		/// Have we applied the down climb offset.
		/// </summary>
		protected bool downClimbOffsetApplied;

		/// <summary>
		/// Position of current ladders top.
		/// </summary>
		protected Vector2 ladderTopPosition;

		/// <summary>
		/// Reference to an animator used to determine if an animation is finished.
		/// </summary>
		protected Animator myAnimator;

		#endregion

		/// <summary>
		/// Unity Late update. Used to apply post animation offsets.
		/// </summary>
		void LateUpdate()
		{
			if (upClimbStarted || upClimbOffsetApplied)
			{
				AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo (0);
				if (((info.IsName (AnimationState.LADDER_TOP_UP.AsString ())) && info.normalizedTime >= 1.0f)
				    || info.IsName (AnimationState.LADDER_TOP_UP_DONE.AsString ()))
				{
					upClimbOffsetApplied = true;
					upClimbStarted = false;
					Vector2 offset = ladderTopPosition + upClimbOffset - (Vector2)character.Transform.position;
					character.Translate (offset.x, offset.y, true);	
				}
			}

			if (downClimbStarted || downClimbOffsetApplied)
			{
				AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo (0);
				if (((info.IsName (AnimationState.LADDER_TOP_DOWN.AsString ())) && info.normalizedTime >= 1.0f)
					|| info.IsName (AnimationState.LADDER_TOP_DOWN_DONE.AsString ()))
				{
					downClimbOffsetApplied = true;
					downClimbStarted = false;
					Vector2 offset = ladderTopPosition + downClimbOffset - (Vector2)character.Transform.position;
					character.Translate (offset.x, offset.y, true);	
				}
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
			base.Init (character, movementData);
			myAnimator = character.GetComponentInChildren<Animator> ();
			if (movementData != null && movementData.Length >= MovementVariableCount)
			{
				upClimbDistance = movementData[UpClimbDistanceIndex].FloatValue;
				upClimbOffset = movementData[UpClimbOffsetIndex].Vector2Value;
				downClimbOffset = movementData[DownClimbOffsetIndex].Vector2Value;
			}
			else
			{
				Debug.LogError("Invalid movement data, not enough values.");
			}
			return this;
		}

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			// Playing animation don't do anything here
			if (upClimbStarted) return;
			ladderTopPosition.x = character.CurrentLadder.transform.position.x;
			// Centre
			if (!centered)
			{
				float offsetFromCentre = transform.position.x - character.CurrentLadder.transform.position.x;
				// Close enough to snap to centre 
				if (Mathf.Abs (offsetFromCentre) < TimeManager.FrameTime * centeringRate)
				{
					character.Translate(-offsetFromCentre, 0, true);
					centered = true;

				}
				// Move towards centre
				else
				{
					character.Translate(TimeManager.FrameTime * centeringRate * (offsetFromCentre > 0 ? -1 : 1), 0, true);
				}
			}

			// When down climbing we do centre but nothing else
			if (downClimbStarted) return;


			// Determine if we are at bottom of ladder (ie. the bottom of the feet colliders are below or at the ladders end)
			// Climbing
			if (character.Input.VerticalAxisDigital != 0)
			{
				
				// Update top of ladder for animations
				float ladderTop = (character.CurrentLadder.transform.position.y + ((BoxCollider2D)character.CurrentLadder).Offset().y) +
					(character.CurrentLadder.transform.lossyScale.y * (((BoxCollider2D)character.CurrentLadder).size.y / 2.0f));
				ladderTopPosition.y = ladderTop;
				if (character.Input.VerticalAxisDigital == 1)
				{
					character.Translate(0, TimeManager.FrameTime * climbSpeed, true);
					// Make sure we don't go above ladder

					// Check for up climb
					if (ladderTop <= (character.BottomOfFeet + upClimbDistance))
					{
						// TODO Snap to position
						upClimbStarted = true;
						character.SetVelocityX(0);
						character.SetVelocityY(0);
						return;
					}
					if (ladderTop <= character.BottomOfFeet)
					{
						character.Translate(0, ladderTop - character.BottomOfFeet, true);
						// Dismount - climbed to bottom/top of ladder
						if (((int)LadderDismountType.TOP_BOTTOM & availableDismounts) == (int)LadderDismountType.TOP_BOTTOM) 
						{
							dismount = true;
							character.ApplyBaseCollisionsForRaycastType(RaycastType.FOOT);
						}
					}

				}
				else if (character.Input.VerticalAxisDigital == -1)
				{
					character.Translate(0, TimeManager.FrameTime * -climbSpeed, true);
					// Make sure we don't go below ladder
					float ladderBottom = (character.CurrentLadder.transform.position.y + ((BoxCollider2D)character.CurrentLadder).Offset().y) -
										 (character.CurrentLadder.transform.lossyScale.y * (((BoxCollider2D)character.CurrentLadder).size.y / 2.0f));
					if (ladderBottom >= character.BottomOfFeet)
					{
						if (!CheckForLadderBelow(character.CurrentLadder.gameObject.layer))
						{

							// Dismount - climbed to bottom/top of ladder
							if (((int)LadderDismountType.TOP_BOTTOM & availableDismounts) == (int)LadderDismountType.TOP_BOTTOM) 
							{
								dismount = true;
							}
							else
							{
								character.Translate(0, ladderBottom - character.BottomOfFeet, true);
							}
						}
					}
				}
			}

			// We have no residual velocity if on a ladder
			character.SetVelocityX(0);
			character.SetVelocityY(0);
		}

		/// <summary>
		/// Called when the movement gets control. Typically used to do initialisation of velocity and the like.
		/// </summary>
		override public void GainControl() 
		{
			// Update top of ladder for animations
			float ladderTop = (character.CurrentLadder.transform.position.y + ((BoxCollider2D)character.CurrentLadder).Offset().y) +
				(character.CurrentLadder.transform.lossyScale.y * (((BoxCollider2D)character.CurrentLadder).size.y / 2.0f));
			ladderTopPosition.y = ladderTop;
		}

		/// <summary>
		/// The digital ladder controller can only work with 2D Box Colliders.
		/// </summary>
		/// <value><c>true</c> if the character should climb; otherwise, <c>false</c>.</value>
		override public bool WantsClimb()
		{
			bool atTop = false;
			bool atBottom = false;

			// Early out if the climbable is a rope, this movement can't climb ropes
			if (character.CurrentLadder.GetComponentInParent<Rope>() != null) return false;

			facingDirectionOverride = character.CurrentLadder.GetComponentInParent<LadderFacingDirectionOverride> ();

			// Determine if we are at top of ladder (ie. the bottom of the feet colliders are below or at the ladders end)
			// TODO we could cache this if ladder wont move in Y
			float ladderTop = ((character.CurrentLadder.transform.position.y + ((BoxCollider2D)character.CurrentLadder).Offset().y) +
			                      (character.CurrentLadder.transform.lossyScale.y * (((BoxCollider2D)character.CurrentLadder).size.y / 2)));
			if (ladderTop <= character.BottomOfFeet) atTop = true;
			if (ladderTop <= (character.BottomOfFeet + upClimbDistance) && !character.Grounded) return false;
			// If pressing down at top of ladder do the down climb animation)
			if (atTop && character.Input.VerticalAxisDigital == -1)
			{
				downClimbStarted = true;
				centered = false;
				dismount = false;
				return true;
			}
			// Determine if we are at bottom of ladder (ie. the bottom of the feet colliders are below or at the ladders end)
			float ladderBottom = ((character.CurrentLadder.transform.position.y + ((BoxCollider2D)character.CurrentLadder).Offset().y) -
			                     (character.CurrentLadder.transform.lossyScale.y * (((BoxCollider2D)character.CurrentLadder).size.y / 2)));
			if (ladderBottom >= character.BottomOfFeet) atBottom = true;
			if (character.CurrentLadder is BoxCollider2D &&
			    Mathf.Abs (transform.position.x - character.CurrentLadder.transform.position.x) < leeway && character.Input.VerticalAxisDigital != 0.0f &&
			    // Make sure character is not at top of ladder or if they are they are pressing down
			    (!atTop || character.Input.VerticalAxisDigital == -1) &&
			    // Make sure character is not at bottom of ladder or if they are they are pressing up
			    (!atBottom || character.Input.VerticalAxisDigital == 1) &&
			    (!jumpDismount || character.Velocity.y <= 0.0f)
			    )
			{
				centered = false;
				dismount = false;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Keep control until dismount.
		/// </summary>
		override public bool ForceMaintainControl()
		{
			if (upClimbOffsetApplied) return false;
			if (upClimbStarted || downClimbStarted) return true;
			return base.ForceMaintainControl ();
		}

		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions.
		/// </summary>
		override public void LosingControl() 
		{
			upClimbStarted = false;
			downClimbStarted = false;
			upClimbOffsetApplied = false;
			downClimbOffsetApplied = false;
			base.LosingControl ();
		}

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				if (upClimbStarted) return AnimationState.LADDER_TOP_UP;
				if (downClimbStarted) return AnimationState.LADDER_TOP_DOWN;
				if (dismount && character.Grounded) 
				{
					return AnimationState.IDLE;
				}
				else 
				if (character.Input.VerticalAxisDigital == 1)
				{
					return AnimationState.CLIMB_UP;
				}
				else if (character.Input.VerticalAxisDigital == -1)
				{
					return AnimationState.CLIMB_DOWN;
				}
				else
				{
					return AnimationState.CLIMB_HOLD;
				}
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
			if (movementData == null || movementData.Length != MovementVariableCount)
			{
				movementData = new MovementVariable[MovementVariableCount];
			}

			MovementVariable[] baseMovementData = LadderMovement_DigitalForTiles.DrawInspector (movementData, ref showDetails, target);
			System.Array.Copy (baseMovementData, movementData, baseMovementData.Length);

			EditorGUILayout.LabelField ("Climb Animations", EditorStyles.boldLabel);
			// Up Distance speed
			if (movementData[UpClimbDistanceIndex] == null) movementData[UpClimbDistanceIndex] = new MovementVariable();
			movementData[UpClimbDistanceIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Up Climb Distance", "Distance from the top of the ladder at which we trigger the upwards climb."), movementData[UpClimbDistanceIndex].FloatValue);

			// Up Climb Offset
			if (movementData[UpClimbOffsetIndex] == null) movementData[UpClimbOffsetIndex] = new MovementVariable();
			movementData[UpClimbOffsetIndex].Vector2Value = EditorGUILayout.Vector2Field(new GUIContent("Up Climb Offset", "After the up climb finishes we apply this offset."), movementData[UpClimbOffsetIndex].Vector2Value);

			// Up Climb Offset
			if (movementData[DownClimbOffsetIndex] == null) movementData[DownClimbOffsetIndex] = new MovementVariable();
			movementData[DownClimbOffsetIndex].Vector2Value = EditorGUILayout.Vector2Field(new GUIContent("Down Climb Offset", "After the down climb finishes we apply this offset."), movementData[DownClimbOffsetIndex].Vector2Value);

			return movementData;
		}

		/// <summary>
		/// Returns the direction the character is facing. 0 for none, 1 for right, -1 for left.
		/// </summary>
		/// <value>The facing direction.</value>
		override public int FacingDirection
		{
			get
			{
				if (facingDirectionOverride != null) return facingDirectionOverride.facingDirection;
				return character.Input.HorizontalAxisDigital;
			}
		}

		#endregion

#endif


	}

}

