using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A climb movement which allows you to move in all four directions. For example on fences, vines, walls.
	/// </summary>
	public class ClimbMovement_FourWay : ClimbMovement
	{

		#region members

		/// <summary>
		/// The speed the character climbs at.
		/// </summary>
		public float climbSpeed;

		/// <summary>
		/// The speed the character moves horizontally at.
		/// </summary>
		public float horizontalSpeed;

		/// <summary>
		/// The required colliders hitting the fence for climb to start.
		/// </summary>
		public int requiredColliders;

		/// <summary>
		/// Enum mask.
		/// </summary>
		public int availableDismounts;

		/// <summary>
		/// Reference to the box2D collider defining the ladder.
		/// </summary>
		protected BoxCollider2D ladderCollider;

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

		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Climb/FourWay";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "A climb movement which allows you to move in all four directions. For example on fences, vines, walls.";
		
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
		/// The index for the Horizontal speed value in the movement data.
		/// </summary>
		private const int HorizontalSpeedIndex = 1;

		/// <summary>
		/// The index for the leeway value in the movement data.
		/// </summary>
		private const int RequiredCollidersIndex = 2;

		/// <summary>
		/// The index of the available dismounts mask.
		/// </summary>
		private const int AvailableDismountsIndex = 3;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 4;

		/// <summary>
		/// The default climb speed.
		/// </summary>
		protected const float DefaultClimbSpeed = 3.0f;

		/// <summary>
		/// The default leeway.
		/// </summary>
		protected const int DefaultRequiredColliders = 3;

		/// <summary>
		/// The default available dismounts.
		/// </summary>
		protected const int DefaultAvailableDismounts = (int)LadderDismountType.TOP_BOTTOM_AND_JUMP;

	
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
			MoveInX();
			if (!dismount) MoveInY();
		}
		/// <summary>
		/// Does the Y movement.
		/// </summary>
		virtual protected void MoveInY()
		{
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
						character.Translate(0, ladderBottom - character.BottomOfFeet, true);
						// Dismount - climbed to bottom/top of ladder
						if (((int)LadderDismountType.TOP_BOTTOM & availableDismounts) == (int)LadderDismountType.TOP_BOTTOM) 
						{
							dismount = true;
						}
					}
				}
			}

			// We have no residual velocity if on a ladder
			character.SetVelocityY(0);
		}

		/// <summary>
		/// Does the X movement.
		/// </summary>
		virtual protected void MoveInX()
		{
			// Determine if we are at bottom of ladder (ie. the bottom of the feet colliders are below or at the ladders end)
			// Climbing
			if (character.Input.HorizontalAxisDigital != 0)
			{
				if (character.Input.HorizontalAxisDigital == 1)
				{
					character.Translate(TimeManager.FrameTime * horizontalSpeed, 0, true);
					// Make sure we don't go off side of ladder
					float ladderRight = (character.CurrentLadder.transform.position.x + ((BoxCollider2D)character.CurrentLadder).Offset().x) +
						(character.CurrentLadder.transform.lossyScale.x * (((BoxCollider2D)character.CurrentLadder).size.x / 2.0f));
					if (ladderRight < character.RightExtent)
					{
						character.Translate(ladderRight - character.RightExtent, 0, true);
						// Dismount - climbed off side of ladder
						if (((int)LadderDismountType.LEFT_RIGHT & availableDismounts) == (int)LadderDismountType.LEFT_RIGHT) 
						{
							dismount = true;
							character.ApplyBaseCollisionsForRaycastType(RaycastType.FOOT);
						}
					}
					
				}
				else if (character.Input.HorizontalAxisDigital == -1)
				{
					character.Translate(TimeManager.FrameTime * -horizontalSpeed, 0, true);
					// Make sure we don't go off side of ladder
					float ladderLeft = (character.CurrentLadder.transform.position.x + ((BoxCollider2D)character.CurrentLadder).Offset().x) -
						(character.CurrentLadder.transform.lossyScale.x * (((BoxCollider2D)character.CurrentLadder).size.x / 2.0f));
					if (ladderLeft > character.LeftExtent)
					{
						character.Translate(ladderLeft - character.LeftExtent, 0, true);
						// Dismount - climbed off side of ladder
						if (((int)LadderDismountType.LEFT_RIGHT & availableDismounts) == (int)LadderDismountType.LEFT_RIGHT) 
						{
							dismount = true;
							character.ApplyBaseCollisionsForRaycastType(RaycastType.FOOT);
						}
					}
				}
			}
			// We have no residual velocity if on a ladder
			character.SetVelocityX(0);
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
				horizontalSpeed = movementData[HorizontalSpeedIndex].FloatValue;
				availableDismounts = movementData[AvailableDismountsIndex].IntValue;
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

			    // Mathf.Abs (transform.position.x - character.CurrentLadder.transform.position.x) < leeway  &&

			    character.Input.VerticalAxisDigital != 0.0f &&
			    // Make sure character is not at top of ladder or if they are they are pressing down
			    (!atTop || character.Input.VerticalAxisDigital == -1) &&
			    // Make sure character is not at bottom of ladder or if they are they are pressing up
			    (!atBottom || character.Input.VerticalAxisDigital == 1) &&
			    (!jumpDismount || character.Velocity.y <= 0.0f)
			    )
			{
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
				else if (character.Input.VerticalAxisDigital == 1)
				{
					return AnimationState.CLIMB_RIGHT;
				}
				else if (character.Input.VerticalAxisDigital == -1)
				{
					return AnimationState.CLIMB_LEFT;
				}
				else
				{
					return AnimationState.CLIMB_HOLD;
				}
			}
		}

		#endregion

		#region static methods
	
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
			movementData[ClimbSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Climb Speed", "How fast the character climbs up/down."), movementData[ClimbSpeedIndex].FloatValue);
			if (movementData[ClimbSpeedIndex].FloatValue < 0) movementData[ClimbSpeedIndex].FloatValue = 0.0f;
			if (movementData[ClimbSpeedIndex].FloatValue == 0.0f) EditorGUILayout.HelpBox("Climb speed should be larger than 0.", MessageType.Warning, true);

			// Horizontal Climb speed
			if (movementData[HorizontalSpeedIndex] == null) movementData[HorizontalSpeedIndex] = new MovementVariable(DefaultClimbSpeed);
			movementData[HorizontalSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Horizontal Climb Speed", "How fast the character climbs left/right."), movementData[HorizontalSpeedIndex].FloatValue);
			if (movementData[HorizontalSpeedIndex].FloatValue < 0) movementData[ClimbSpeedIndex].FloatValue = 0.0f;
			if (movementData[HorizontalSpeedIndex].FloatValue == 0.0f) EditorGUILayout.HelpBox("Climb speed should be larger than 0.", MessageType.Warning, true);


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
				return character.Input.HorizontalAxisDigital;
			}
		}

		#endregion

#endif


	}

}

