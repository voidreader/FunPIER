using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A climb movement for ladder climbing with strict digital style controls and made for supporting tile based levels
	/// where one ladder may be made of multiple tiles.
	/// </summary>
	public class LadderMovement_DigitalForTiles : ClimbMovement
	{

		#region members

		/// <summary>
		/// The speed the character climbs at.
		/// </summary>
		public float climbSpeed;

		/// <summary>
		/// The character can only engage the ladder if the distance in X between
		/// their transform and the ladders transform is smaller than this.
		/// </summary>
		public float leeway;

		/// <summary>
		/// The speed at which the character moves towards the centre of the ladder.
		/// </summary>
		public float centeringRate;

		/// <summary>
		/// Enum mask
		/// </summary>
		public int availableDismounts;

		/// <summary>
		/// Reference to the box2D collider defining the ladder.
		/// </summary>
		protected BoxCollider2D ladderCollider;

		/// <summary>
		/// Has the character centered on the ladder yet?
		/// </summary>
		protected bool centered;

		/// <summary>
		/// If the user is dismounting we will relinquish control.
		/// </summary>
		protected bool dismount;

		/// <summary>
		/// When a character jumps while holding up or down you don't want them to automatically stick
		/// back on the ladder. We track if the way they dismounted was a jump, and if it was we only stick back 
		/// to the ladder when the velocity is smaller than 0.
		/// </summary>
		protected bool jumpDismount;

		/// <summary>
		/// Nullable reference to a facing direciton override.
		/// </summary>
		protected LadderFacingDirectionOverride facingDirectionOverride;

		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Climb/Ladder/Digital with Tiles";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Ladder movement which allows simple fixed speed climbing and supports tile based ladders (where one ladder is made of multiple bricks. It has several options defining how you dismount the ladder.";
		
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
		/// The index for the speed value in the movement data.
		/// </summary>
		private const int ClimbSpeedIndex = 0;

		/// <summary>
		/// The index for the leeway value in the movement data.
		/// </summary>
		private const int LeewayIndex = 1;

		/// <summary>
		/// The index for the Centering Rate value in the movement data.
		/// </summary>
		private const int CenteringRateIndex = 2;

		/// <summary>
		/// The index of the available dismounts mask.
		/// </summary>
		private const int AvailableDismountsIndex = 3;

		/// <summary>
		/// The index of the switch that controls detection type.
		/// </summary>
		private const int DetectLaddersByTagIndex = 4;

		/// <summary>
		/// The index of the ladder or tag name variable.
		/// </summary>
		private const int LadderLayerOrTagNameIndex = 5;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 6;

		/// <summary>
		/// The default climb speed.
		/// </summary>
		protected const float DefaultClimbSpeed = 3.0f;

		/// <summary>
		/// The default leeway.
		/// </summary>
		protected const float DefaultLeeway = 0.5f;

		/// <summary>
		/// The default leeway.
		/// </summary>
		protected const float DefaultCenteringRate = 1.0f;

		/// <summary>
		/// The default available dismounts.
		/// </summary>
		protected const int DefaultAvailableDismounts = (int)LadderDismountType.TOP_BOTTOM_AND_JUMP_AND_LEFT_RIGHT;

	
		#endregion

		#region properties

		/// <summary>
		/// No gravity when climbing.
		/// </summary>
		override public bool ShouldApplyGravity
		{
			get
			{
				return false;
			}
		}
		
		
		/// <summary>
		/// Ignore collisions when climbing.
		/// </summary>
		override public RaycastType ShouldDoBaseCollisions
		{
			get
			{
				return RaycastType.NONE;
			}
		}

		#endregion

		#region public methods

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
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
			// Determine if we are at bottom of ladder (ie. the bottom of the feet colliders are below or at the ladders end)
			// Climbing
			if (character.Input.VerticalAxisDigital != 0)
			{
				if (character.Input.VerticalAxisDigital == 1)
				{
					character.Translate(0, TimeManager.FrameTime * climbSpeed, true);
					// Make sure we don't go above ladder
					float ladderTop = (character.CurrentLadder.transform.position.y + ((BoxCollider2D)character.CurrentLadder).Offset().y) +
									  (character.CurrentLadder.transform.lossyScale.y * (((BoxCollider2D)character.CurrentLadder).size.y / 2.0f));
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
		/// Initialise this instance.
		/// </summary>
		override public Movement Init(Character character)
		{
			this.character = character;
			return this;
		}

		/// <summary>
		/// Initialise the mvoement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			this.character = character;

			if (movementData != null && movementData.Length == MovementVariableCount)
			{
				climbSpeed = movementData[ClimbSpeedIndex].FloatValue;
				leeway = movementData[LeewayIndex].FloatValue;
				centeringRate = movementData[CenteringRateIndex].FloatValue;
				availableDismounts = movementData[AvailableDismountsIndex].IntValue;
				DetectLaddersByTag = movementData[DetectLaddersByTagIndex].BoolValue;
				LadderLayerOrTagName = movementData[LadderLayerOrTagNameIndex].StringValue;
			}
			else
			{
				Debug.LogError("Invalid movement data, not enough values.");
			}
			return this;
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
			// if (character.CurrentLadder.GetComponentInParent<Rope>() != null) return false;

			facingDirectionOverride = character.CurrentLadder.GetComponentInParent<LadderFacingDirectionOverride> ();

			// Determine if we are at top of ladder (ie. the bottom of the feet colliders are below or at the ladders end)
			// TODO we could cache this if ladder wont move in Y
			float ladderTop = ((character.CurrentLadder.transform.position.y + ((BoxCollider2D)character.CurrentLadder).Offset().y) +
			                      (character.CurrentLadder.transform.lossyScale.y * (((BoxCollider2D)character.CurrentLadder).size.y / 2)));
			if (ladderTop <= character.BottomOfFeet) atTop = true;

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
		override public bool WantsControl()
		{
			bool leftRightDismount = true;
			// Manual dismounts - left and right
			if (((int)LadderDismountType.LEFT_RIGHT & availableDismounts) == (int)LadderDismountType.LEFT_RIGHT)
			{
				if (character.Input.HorizontalAxisDigital != 0)
				{
					dismount = true;
				}
			}
			// Manual dismounts - jump
			if (((int)LadderDismountType.JUMP & availableDismounts) == (int)LadderDismountType.JUMP)
			{
				if (character.Input.JumpButton == ButtonState.DOWN)
				{
					dismount = true;
					jumpDismount = true;
				}
			}
		
			if (dismount) 
			{
				if (!leftRightDismount && !jumpDismount && character.Input.VerticalAxisDigital == -1)
				{

				}
				character.ApplyBaseCollisionsForRaycastType(RaycastType.FOOT);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
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

		#region protected methods
	
		/// <summary>
		/// Checks for ladder below.
		/// </summary>
		/// <returns><c>true</c>, if ladder tile found below, <c>false</c> otherwise.</returns>
		/// <param name="layer">Layer of ladder.</param>
		virtual protected bool CheckForLadderBelow(int layer)
		{
			RaycastHit2D hit =  Physics2D.Raycast (new Vector2 (character.transform.position.x, character.BottomOfFeet), new Vector2 (0, -1), 0.25f, 1 << layer);
			if (hit.collider != null)
			{
				return true;
			}
			return false;
		}

		#endregion

#if UNITY_EDITOR
		#region draw inspector

		protected static bool showDismounts;

		/// <summary>
		/// Draws the inspector.
		/// </summary>
		public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{
			if (movementData == null || movementData.Length != MovementVariableCount)
			{
				movementData = new MovementVariable[MovementVariableCount];
			}

			// Climb speed
			if (movementData[ClimbSpeedIndex] == null) movementData[ClimbSpeedIndex] = new MovementVariable(DefaultClimbSpeed);
			movementData[ClimbSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Climb Speed", "How fast the character climbs."), movementData[ClimbSpeedIndex].FloatValue);
			if (movementData[ClimbSpeedIndex].FloatValue < 0) movementData[ClimbSpeedIndex].FloatValue = 0.0f;
			if (movementData[ClimbSpeedIndex].FloatValue == 0.0f) EditorGUILayout.HelpBox("Climb speed should be larger than 0.", MessageType.Warning, true);

			// Dismounts
			if (movementData[AvailableDismountsIndex] == null) movementData[AvailableDismountsIndex] = new MovementVariable(DefaultAvailableDismounts);

			showDismounts = EditorGUILayout.Foldout(showDismounts, new GUIContent("Dismounts", "Options for defining how the character exits a ladder."));
			if (showDismounts)
			{
				bool jump = EditorGUILayout.Toggle(new GUIContent("Jump", "Can the character jump off of the ladder."), (movementData[AvailableDismountsIndex].IntValue & (int)LadderDismountType.JUMP) == (int)LadderDismountType.JUMP);
				if (jump) movementData[AvailableDismountsIndex].IntValue |= (int) LadderDismountType.JUMP;
				else  movementData[AvailableDismountsIndex].IntValue &= ~((int) LadderDismountType.JUMP);

				bool leftRight = EditorGUILayout.Toggle(new GUIContent("Left/Right", "Can the character move left and right off of the ladder."), (movementData[AvailableDismountsIndex].IntValue & (int)LadderDismountType.LEFT_RIGHT) == (int)LadderDismountType.LEFT_RIGHT);
				if (leftRight) movementData[AvailableDismountsIndex].IntValue |= (int) LadderDismountType.LEFT_RIGHT;
				else  movementData[AvailableDismountsIndex].IntValue &= ~((int) LadderDismountType.LEFT_RIGHT);

				bool topBottom = EditorGUILayout.Toggle(new GUIContent("Bottom/Top", "Can the character climb off the top and bottom of the ladder."), (movementData[AvailableDismountsIndex].IntValue & (int)LadderDismountType.TOP_BOTTOM) == (int)LadderDismountType.TOP_BOTTOM);
				if (topBottom) movementData[AvailableDismountsIndex].IntValue |= (int) LadderDismountType.TOP_BOTTOM;
				else  movementData[AvailableDismountsIndex].IntValue &= ~((int) LadderDismountType.TOP_BOTTOM);
			}

			// Make sure we reset defaults even if details not shown
			if (movementData[LeewayIndex] == null) movementData[LeewayIndex] = new MovementVariable(DefaultLeeway);
			if (movementData[CenteringRateIndex] == null) movementData[CenteringRateIndex] = new MovementVariable(DefaultCenteringRate);

			showDetails = EditorGUILayout.Foldout(showDetails, "Details");
			if (showDetails)
			{
				// Leeway
				movementData[LeewayIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Leeway", "How close to the ladder the character needs to be before they snap to the middle."), movementData[LeewayIndex].FloatValue);
				if (movementData[LeewayIndex].FloatValue < 0) movementData[LeewayIndex].FloatValue = 0.0f;

				// Centering Rate
				movementData[CenteringRateIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Centering Rate", "The rate at which the character snaps to the middle of the ladder."), movementData[CenteringRateIndex].FloatValue);
				if (movementData[CenteringRateIndex].FloatValue < 0) movementData[CenteringRateIndex].FloatValue = 0.0f;

			}

			// Help Box if a weird combination is found
			string warning = CheckForUnexpectedSettings(movementData);
			if (warning != null) EditorGUILayout.HelpBox(warning, MessageType.Warning, true);

			return movementData;
		}

		/// <summary>
		/// Checks for unexpected settings combinations and returns a warning if they are found.
		/// </summary>
		/// <returns>The string describing unexpected settings, or null if none found..</returns>
		protected static string CheckForUnexpectedSettings(MovementVariable[] movementData)
		{
			if (movementData[AvailableDismountsIndex].IntValue == 0) return "No method for dismounting ladders has been selected. This may mean you cannot get off from a ladder.";

			return null;
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

