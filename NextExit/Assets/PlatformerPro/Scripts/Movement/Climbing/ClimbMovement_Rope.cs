using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A climb movement for swinging climbables (i.e ropes!) It does not handle collisions so make sure there is room around your ropes.
	/// </summary>
	public class ClimbMovement_Rope : ClimbMovement
	{

		#region members

		/// <summary>
		/// The speed the character climbs at.
		/// </summary>
		public float climbSpeed;

		/// <summary>
		/// The character can only climb the rope if the distance between
		/// their transform and the ladders transform is smaller than this.
		/// </summary>
		public float leeway;

		/// <summary>
		/// Enum mask for the ways in which the chracter can dismount the rope.
		/// </summary>
		public int availableDismounts;

		/// <summary>
		/// Should this movement only be used for climbing ropes. If false it will also be used for ladders and other climbables. 
  		/// </summary>
  		public bool onlyClimbRopes;

		/// <summary>
		/// How much force does the character apply downwards on the rope section. Makes the rope behave like the character has weight.
		/// </summary>
		public float downforce;

		/// <summary>
		/// How much force does the character impart to the rope when they latch on to it.
		/// </summary>
		public float impartForceOnLatch;

		/// <summary>
		/// How high does the character jump when the press jump, before any rope force is added?
		/// </summary>
		public float baseJumpHeight;

		/// <summary>
		/// How much force does the rope impart to the character when they jump off.
		/// </summary>
		public Vector2 impartForceOnJump;

		/// <summary>
		/// If true we automatically grab this rope as we move past it.
		/// </summary>
		public bool autograb;

		/// <summary>
		/// How much force, if any, to impart when the user presses an arrow key.
		/// </summary>
		public float swingForce;

		/// <summary>
		/// Should we calculate rope angles and rotate chracter to match.
		/// </summary>
		public bool calculateAngles;

		/// <summary>
		/// If calculating angles we can average the angle across multiple rope sections (i.e. between hands and feet).
		/// This value is the distance between the hand contact pooint and feet contact point.
		/// </summary>
		public float handOffset;

  		/// <summary>
		/// If the user is dismounting we will relinquish control.
		/// </summary>
		protected bool dismount;

		/// <summary>
		/// When a character jumps while holding up or down you don't want them to automatically stick
		/// back on the rope. We track if the way they dismounted was a jump, and if it was we only stick back 
		/// to the rope when the y velocity is smaller than 0.
		/// </summary>
		protected bool jumpDismount;

		/// <summary>
		/// The rope character is currently on.
		/// </summary>
		protected RopeSection currentRopeSection;

		/// <summary>
		/// Position on the current rope section from 0 bottom to 1 top).
		/// </summary>
		protected float positionOnSection;

		/// <summary>
		/// Cached section length of current rope. Assumes ropes always have same sized sections.
		/// </summary>
		protected float currentRopeSectionLength;

		/// <summary>
		/// Ensure we have time to dismount if we dismount off of bottom.
		/// </summary>
		protected float dismountTimer;


		/// <summary>
		/// Stores the direction we were moving when we last did a swing. We wont be able to swing again until this changes.
		/// </summary>
		protected float swingTimer;

		/// <summary>
		/// If true we swung this frame and should play swing animation.
		/// </summary>
		protected bool showSwingAnimation;

		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Climb/Rope";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Climb movement which allows climbing of ropes and moving things.";
		
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
		/// Time between swings.
		/// </summary>
		protected const float swingTime = 0.5f;
		
		/// <summary>
		/// How long should we not be able to mount rope after dismounting from the bottom of a rope?
		/// </summary>
		protected const float DismountTime = 0.33f;

		/// <summary>
		/// The index for the speed value in the movement data.
		/// </summary>
		protected const int ClimbSpeedIndex = 0;

		/// <summary>
		/// The index for the leeway value in the movement data.
		/// </summary>
		protected const int LeewayIndex = 1;

		/// <summary>
		/// The index of the available dismounts mask.
		/// </summary>
		protected const int AvailableDismountsIndex = 2;

		/// <summary>
		/// The index of the only climb ropes flag.
		/// </summary>
		protected const int OnlyClimbRopesIndex = 3;

		/// <summary>
		/// The index of the down force.
		/// </summary>
		protected const int DownForceIndex = 4;

		/// <summary>
		/// The index of impart force on latch.
		/// </summary>
		protected const int ImpartForceOnLatchIndex = 5;

		/// <summary>
		/// The index of the impart force on jump.
		/// </summary>
		protected const int ImpartForceOnJumpIndex = 6;

		/// <summary>
		/// The index of the auto grab value.
		/// </summary>
		protected const int AutoGrabIndex = 7;
		
		/// <summary>
		/// The index of the swing velocity.
		/// </summary>
		protected const int SwingVelocityIndex = 9;

		/// <summary>
		/// The index of the calculate angles value.
		/// </summary>
		protected const int CalculateAnglesIndex = 9;

		/// <summary>
		/// The index of the hand offset value.
		/// </summary>
		protected const int HandOffsetIndex = 10;

		/// <summary>
		/// The index of the base jump height.
		/// </summary>
		protected const int BaseJumpHeightIndex = 11;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		protected const int MovementVariableCount = 12;

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
		protected const int DefaultAvailableDismounts = (int)LadderDismountType.TOP_BOTTOM_AND_JUMP;

		/// <summary>
		/// The default downforce.
		/// </summary>
		protected const float DefaultDownforce = 5.0f;

		/// <summary>
		/// The default impart force on latch.
		/// </summary>
		protected const float DefaultImpartForceOnLatch = 2.0f;

		/// <summary>
		/// The default height of the base jump.
		/// </summary>
		protected const float DefaultBaseJumpHeight = 1.5f;

		/// <summary>
		/// The default impart force on jump.
		/// </summary>
		protected static Vector2 DefaultImpartForceOnJump = new Vector2 (1.5f, 1.5f);

		/// <summary>
		/// The default autograb.
		/// </summary>
		protected const bool DefaultAutograb = true;

		/// <summary>
		/// The default swing force.
		/// </summary>
		protected const float DefaultSwingForce = 2.0f;

		/// <summary>
		/// The default calculate angles.
		/// </summary>
		protected const bool DefaultCalculateAngles = false;

		/// <summary>
		/// The default hand offset.
		/// </summary>
		protected const float DefaultHandOffset = 0.0f;
		

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

		/// <summary>
		/// How does this movement use Velocity.
		/// </summary>
		override public VelocityType VelocityType
		{
			get
			{
				return VelocityType.WORLD;
			}
		}

		#endregion

		#region Unity hooks

		/// <summary>
		/// Unity update() hook.
		/// </summary>
		void Update()
		{
			if (dismountTimer > 0.0f) dismountTimer -= TimeManager.FrameTime;
			if (swingTimer > 0.0f) swingTimer -= TimeManager.FrameTime;
		}

		#endregion

		#region public methods

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			if (swingTimer <= 0.0f) showSwingAnimation = false;
			// On a ladder/vine
			if (currentRopeSection == null)
			{
				float offsetFromCentre = transform.position.x - character.CurrentLadder.transform.position.x;
				character.Translate(-offsetFromCentre, 0, true);

				// Determine if we are at bottom of ladder (ie. the bottom of the feet colliders are below or at the ladders end)
				// Climbing
				if (character.Input.VerticalAxisDigital != 0)
				{
					if (character.Input.VerticalAxisDigital == 1)
					{
						character.Translate(0, TimeManager.FrameTime * climbSpeed, true);
						// Make sure we don't go above ladder
						float ladderTop = (character.CurrentLadder.transform.position.y + 
											((BoxCollider2D)character.CurrentLadder).Offset().y) +
										  	(character.CurrentLadder.transform.lossyScale.y * (((BoxCollider2D)character.CurrentLadder).size.y / 2));
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
						float ladderBottom = (character.CurrentLadder.transform.position.y + 
											  ((BoxCollider2D)character.CurrentLadder).Offset().y) -
											  (character.CurrentLadder.transform.lossyScale.y * (((BoxCollider2D)character.CurrentLadder).size.y / 2));
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
			}
			// On a rope
			else
			{
				// Handle non-climbable rope
				if (currentRopeSection.usedFixedPosition)
				{
					// Dismount on down
					if (character.Input.VerticalAxisDigital == -1)
					{
						if (((int)LadderDismountType.TOP_BOTTOM & availableDismounts) == (int)LadderDismountType.TOP_BOTTOM)
						{
							dismount = true;
							dismountTimer = DismountTime;
							character.ApplyBaseCollisionsForRaycastType(RaycastType.FOOT);
						}
					}
				}
				else
				{
					// Climbing
					if (character.Input.VerticalAxisDigital != 0)
					{
						if (character.Input.VerticalAxisDigital == 1)
						{
							// Make sure we can climb to the next rope piece if it exists
							// Move along length
							float newPosition = positionOnSection + (TimeManager.FrameTime * (climbSpeed / currentRopeSectionLength));
							if (newPosition >= 2.0f)
							{
								Debug.LogWarning("Rope climbing speed is too large for rope section size");
								newPosition = 1.9999f;
							}
							if (newPosition > 1.0f)
							{
								// If position is bigger than one move up to next rope section
								RopeSection nextSection = currentRopeSection.SectionAbove;
								// Don't allow climbing to the RopeAnchor
								if (nextSection is RopeAnchor) nextSection = null;
								// Or if no rope section left cap position at 1.
								if (nextSection == null)
								{
									positionOnSection = 1.0f;
								}
								else
								{
									positionOnSection = newPosition - 1.0f;
									currentRopeSection = nextSection;
								}
							}
							else positionOnSection = newPosition;
						}
						else if (character.Input.VerticalAxisDigital == -1)
						{
							// Make sure we can climb to the next rope piece if it exists
							// Move along length
							float newPosition = positionOnSection - (TimeManager.FrameTime * (climbSpeed / currentRopeSectionLength));
							if (newPosition <= -1.0f)
							{
								Debug.LogWarning("Rope climbing speed is too large for rope section size");
								newPosition = -0.9999f;
							}
							if (newPosition < 0)
							{
								// If position is smaller than zero move down to next rope section
								RopeSection nextSection = currentRopeSection.SectionBelow;
								// Or if no rope section left cap position at 0
								if (nextSection == null)
								{
									positionOnSection = 0;
									if (((int)LadderDismountType.TOP_BOTTOM & availableDismounts) == (int)LadderDismountType.TOP_BOTTOM && character.Input.VerticalAxisDigital == -1)
									{
										dismount = true;
										dismountTimer = DismountTime;
										character.ApplyBaseCollisionsForRaycastType(RaycastType.FOOT);
									}
								}
								else
								{
									positionOnSection = newPosition + 1.0f;
									currentRopeSection = nextSection;
								}
							}
							else
							{
								positionOnSection = newPosition;
							}
						}
					}
				}

				// Rotate
				if (calculateAngles)
				{
					// TODO This rotation interacts directly with the chracter transform (and other variables) - it would be nicer to do this through the character interface
					float deg = currentRopeSection.GetCharacterRotationForPosition(positionOnSection, handOffset);
					float deg2 = currentRopeSection.GetCharacterRotationForPosition(positionOnSection, 0);
					deg = (deg + deg2) / 2.0f;
					float difference  = deg - character.transform.eulerAngles.z;

					// Shouldn't really happen but just in case
					if (difference > 180) difference = difference - 360;
					if (difference < -180) difference = difference + 360;
					// character.transform.Rotate =  Quaternion.Euler(0,0,deg);
					//character.transform.RotateAround(transform.position + new Vector3(0, handOffset, 0), new Vector3(0,0,1), difference);
					if (difference > character.rotationSpeed * TimeManager.FrameTime) difference = character.rotationSpeed * TimeManager.FrameTime;
					if (difference < -character.rotationSpeed * TimeManager.FrameTime) difference = -character.rotationSpeed * TimeManager.FrameTime;
					character.transform.RotateAround(character.transform.position, new Vector3(0,0,1), difference);
				}


				// Snap to centre
				Vector2 diff = (Vector2)character.transform.position + (Vector2)(character.transform.localRotation * new Vector2(0, handOffset)) - currentRopeSection.GetCharacterPositionForPosition(positionOnSection, handOffset);
				character.Translate(-diff.x, -diff.y, true);

				// Apply down force
				if (downforce > 0)
				{
					currentRopeSection.GetComponent<Rigidbody2D>().AddForceAtPosition(new Vector2(0, -downforce), character.Transform.position, ForceMode2D.Force);
				}

				// Swing (but only if not climbing)
				if (swingForce > 0 && swingTimer <= 0.0f && character.Input.HorizontalAxisDigital != 0 && character.Input.VerticalAxisDigital == 0)
				{
					currentRopeSection.GetComponent<Rigidbody2D>().AddForceAtPosition(currentRopeSection.transform.rotation * new Vector2(swingForce * character.Input.HorizontalAxisDigital, 0), character.Transform.position, ForceMode2D.Impulse);
					swingTimer = swingTime;
					showSwingAnimation = true;
				}
			}

			// We have no residual velocity if on a rope
			if (!jumpDismount) {
				character.SetVelocityX (0);
				character.SetVelocityY (0);
			}
		}

		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions.
		/// </summary>
		override public void LosingControl()
		{
			positionOnSection = 0.0f;
			dismount = false;
			jumpDismount = false;
			currentRopeSection = null;
			swingTimer = 0.0f;
		}

		/// <summary>
		/// Initialise the mvoement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			AssignReferences (character);

			if (movementData != null && movementData.Length == MovementVariableCount)
			{
				climbSpeed = movementData[ClimbSpeedIndex].FloatValue;
				leeway = movementData[LeewayIndex].FloatValue;
				availableDismounts = movementData[AvailableDismountsIndex].IntValue;
				autograb = movementData[AutoGrabIndex].BoolValue;
				onlyClimbRopes = movementData[OnlyClimbRopesIndex].BoolValue;
				downforce = movementData[DownForceIndex].FloatValue;
				baseJumpHeight = movementData[BaseJumpHeightIndex].FloatValue;
				impartForceOnLatch = movementData[ImpartForceOnLatchIndex].FloatValue;
				impartForceOnJump = movementData[ImpartForceOnJumpIndex].Vector2Value;
				swingForce = movementData[SwingVelocityIndex].FloatValue;
				calculateAngles = movementData[CalculateAnglesIndex].BoolValue;
				handOffset = movementData[HandOffsetIndex].FloatValue;
			}
			else
			{
				Debug.LogError("Invalid movement data, not enough values.");
			}
			return this;
		}

		/// <summary>
		/// The rope controller can only work with 2D Box Colliders.
		/// </summary>
		/// <value><c>true</c> if the character should climb; otherwise, <c>false</c>.</value>
		override public bool WantsClimb()
		{
			if (dismountTimer > 0.0f) return false;

			if (character.CurrentLadder != null && character.CurrentLadder is BoxCollider2D &&
			    // Within leeway
			    Mathf.Abs (transform.position.x - character.CurrentLadder.transform.position.x) < leeway && 
			    // Only snap if holding up or down or auto grab is enabled - and don't autograb if grounded unless pressing up
			    ((character.Input.VerticalAxisDigital == 1 ) || (character.Input.VerticalAxisDigital == -1 && !character.Grounded) || (autograb && !character.Grounded)) &&
				// If character isn't jump dismounting or they are moving downwards, 
			    (!jumpDismount || character.Velocity.y <= 0.0f)
			    )
			{
				dismount = false;
				currentRopeSection = character.CurrentLadder.GetComponent<RopeSection>();
				if (currentRopeSection is RopeAnchor) 
				{
					currentRopeSection = null;
					return false;
				}
				// Auto grab 
				if (currentRopeSection != null) 
				{
					positionOnSection = currentRopeSection.GetPositionForCharacterPosition(character.transform.position);
					currentRopeSectionLength = currentRopeSection.Length;
					// Apply latch force
					if (impartForceOnLatch > 0)
					{
						currentRopeSection.GetComponent<Rigidbody2D>().AddForceAtPosition(new Vector2(impartForceOnLatch * character.Velocity.x, 0), character.Transform.position, ForceMode2D.Impulse);
					}
					return true;
				}
				if (!onlyClimbRopes) return true;
			}

			return false;
		}

		/// <summary>
		/// Keep control until dismount.
		/// </summary>
		override public bool ForceMaintainControl()
		{
			// Keep control for the frame we dismounted (so a jump dismount wont trigger jump), but otherwise not if dismount timer is active
			if (dismountTimer > 0.0f) return false;

			// Manual dismounts - left and right
			if (((int)LadderDismountType.LEFT_RIGHT & availableDismounts) == (int)LadderDismountType.LEFT_RIGHT)
			{
				if (character.Input.HorizontalAxisDigital != 0)
				{
					dismount = true;
					dismountTimer = DismountTime;
				}
			}
			// Manual dismounts - jump
			if (((int)LadderDismountType.JUMP & availableDismounts) == (int)LadderDismountType.JUMP)
			{
				if (character.Input.JumpButton == ButtonState.DOWN)
				{
					if (currentRopeSection != null)
					{
						Vector2 velocity = currentRopeSection.GetComponent<Rigidbody2D>().GetPointVelocity(character.transform.position);
						character.SetVelocityX(velocity.x * impartForceOnJump.x);
						character.SetVelocityY(velocity.y * impartForceOnJump.y);
						// For non-physics movements we need to trigger an overriden jump (we cant use 'is' becuase AirMovement is never an IPhysics)
						if (!typeof(IPhysicsMovement).IsAssignableFrom(character.DefaultAirMovement.GetType()))
						{
							// This is not physically accurate but when cobined with impartForceOnJump seems to feel about right
							character.DefaultAirMovement.DoOverridenJump(baseJumpHeight + (impartForceOnJump.y * velocity.y), 2);
						}
						else
						{
							// Don't allow ropes to push you downwards
							float initialVelocity = Mathf.Sqrt(-2.0f * character.DefaultGravity  * baseJumpHeight);
							character.AddVelocity(0, initialVelocity, false);
							if (character.Velocity.y < 0) character.SetVelocityY(0);
						}
					}
					dismount = true;
					jumpDismount = true;
				}
			}
		

			if (dismount) 
			{
				character.ApplyBaseCollisionsForRaycastType(RaycastType.FOOT);
				dismountTimer = DismountTime;
				// Jump dismount needs to keep control for one more frame so the custom jump overrides the WantsJump()
				if (jumpDismount) return true;
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
				if (currentRopeSection != null)
				{

					if (showSwingAnimation) 
					{
						return AnimationState.ROPE_SWING;
					}
					else if (character.Input.VerticalAxisDigital == 1)
					{
						if (positionOnSection >= 1.0f && (currentRopeSection.SectionAbove == null ||  currentRopeSection.SectionAbove.SectionAbove == null))
						{
							return AnimationState.ROPE_CLIMB_HOLD;
						}
						return AnimationState.ROPE_CLIMB_UP;
					}
					else if (character.Input.VerticalAxisDigital == -1)
					{
						return AnimationState.ROPE_CLIMB_DOWN;
					}
					else
					{
						return AnimationState.ROPE_CLIMB_HOLD;
					}
				}
				else
				{
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
		}


		#endregion

		#region static methods
	
		#endregion

#if UNITY_EDITOR
		#region draw inspector

		/// <summary>
		/// Editor only, should we show dismount options.
		/// </summary>
		protected static bool showDismounts;

		/// <summary>
		/// Editor only, should we show physics options.
		/// </summary>
		protected static bool showPhysicsOptions;

		/// <summary>
		/// Draws the inspector.
		/// </summary>
		public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{
			if (movementData == null || movementData.Length < MovementVariableCount)
			{
				movementData = new MovementVariable[MovementVariableCount];
			}

			// Auto grab
			if (movementData[AutoGrabIndex] == null) movementData[AutoGrabIndex] = new MovementVariable(DefaultAutograb);
			movementData[AutoGrabIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Auto Grab", "If true we automatically grab this rope as we move past it."), movementData[AutoGrabIndex].BoolValue);

			// Climb speed
			if (movementData[ClimbSpeedIndex] == null) movementData[ClimbSpeedIndex] = new MovementVariable(DefaultClimbSpeed);
			movementData[ClimbSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Climb Speed", "How fast the character climbs."), movementData[ClimbSpeedIndex].FloatValue);
			if (movementData[ClimbSpeedIndex].FloatValue < 0) movementData[ClimbSpeedIndex].FloatValue = 0.0f;
			if (movementData[ClimbSpeedIndex].FloatValue == 0.0f) EditorGUILayout.HelpBox("Climb speed should be larger than 0.", MessageType.Warning, true);

			// Dismounts
			if (movementData[AvailableDismountsIndex] == null) movementData[AvailableDismountsIndex] = new MovementVariable(DefaultAvailableDismounts);

			showDismounts = EditorGUILayout.Foldout(showDismounts, new GUIContent("Dismounts", "Options for defining how the character gets off a rope."));
			if (showDismounts)
			{
				bool jump = EditorGUILayout.Toggle(new GUIContent("Jump", "Can the character jump off of the rope."), (movementData[AvailableDismountsIndex].IntValue & (int)LadderDismountType.JUMP) == (int)LadderDismountType.JUMP);
				if (jump) movementData[AvailableDismountsIndex].IntValue |= (int) LadderDismountType.JUMP;
				else  movementData[AvailableDismountsIndex].IntValue &= ~((int) LadderDismountType.JUMP);

				bool leftRight = EditorGUILayout.Toggle(new GUIContent("Left/Right", "Can the character move left and right off of the rope."), (movementData[AvailableDismountsIndex].IntValue & (int)LadderDismountType.LEFT_RIGHT) == (int)LadderDismountType.LEFT_RIGHT);
				if (leftRight) movementData[AvailableDismountsIndex].IntValue |= (int) LadderDismountType.LEFT_RIGHT;
				else  movementData[AvailableDismountsIndex].IntValue &= ~((int) LadderDismountType.LEFT_RIGHT);

				bool topBottom = EditorGUILayout.Toggle(new GUIContent("Bottom/Top", "Can the character climb off the bottom of the rope."), (movementData[AvailableDismountsIndex].IntValue & (int)LadderDismountType.TOP_BOTTOM) == (int)LadderDismountType.TOP_BOTTOM);
				if (topBottom) movementData[AvailableDismountsIndex].IntValue |= (int) LadderDismountType.TOP_BOTTOM;
				else  movementData[AvailableDismountsIndex].IntValue &= ~((int) LadderDismountType.TOP_BOTTOM);
			}
			// Reset outside of shown physics
			if (movementData[DownForceIndex] == null) movementData[DownForceIndex] = new MovementVariable(DefaultDownforce);
			if (movementData[ImpartForceOnLatchIndex] == null) movementData[ImpartForceOnLatchIndex] = new MovementVariable(DefaultImpartForceOnLatch);
			if (movementData[BaseJumpHeightIndex] == null) movementData[BaseJumpHeightIndex] = new MovementVariable(DefaultBaseJumpHeight);
			if (movementData[ImpartForceOnJumpIndex] == null) movementData[ImpartForceOnJumpIndex] = new MovementVariable(DefaultImpartForceOnJump);
			if (movementData[SwingVelocityIndex] == null) movementData[SwingVelocityIndex] = new MovementVariable(DefaultSwingForce);

			showPhysicsOptions = EditorGUILayout.Foldout(showPhysicsOptions, new GUIContent("Physics", "Options for defining the physics like behaviours of a rope."));
			if (showPhysicsOptions)
			{
				// Downforce
				movementData[DownForceIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Down Force", "How much force does the character apply downwards on the rope section. Makes the rope behave like the character has weight."), movementData[DownForceIndex].FloatValue);
				if (movementData[DownForceIndex].FloatValue < 0) movementData[DownForceIndex].FloatValue = 0.0f;

				// Impart velocity on latch
				movementData[ImpartForceOnLatchIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Latch Force", "How much force does the character impart to the rope when they latch on to it. This value is multiplied by character Velocity X so keep it low (e.g. 1.0f)."), movementData[ImpartForceOnLatchIndex].FloatValue);
				if (movementData[ImpartForceOnLatchIndex].FloatValue < 0) movementData[ImpartForceOnLatchIndex].FloatValue = 0.0f;


				// Base Jump Height
				if ((movementData[AvailableDismountsIndex].IntValue & (int) LadderDismountType.JUMP) == (int) LadderDismountType.JUMP)
				{
					movementData[BaseJumpHeightIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Base Jump height", "How high does the character jump when the press jump, before any rope force is added?"), movementData[BaseJumpHeightIndex].FloatValue);
				
					// Impart velocity on jump
					movementData[ImpartForceOnJumpIndex].Vector2Value = EditorGUILayout.Vector2Field(new GUIContent("Jump Force", "How much force does the rope impart to the character when they jump off, this is multiplied by x and y speed."), movementData[ImpartForceOnJumpIndex].Vector2Value);
				}

				//} Swing velocity 
				movementData[SwingVelocityIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Swing Velocity", "How much force does swinging impart. Or zero for no swinging."), movementData[SwingVelocityIndex].FloatValue);
				if (movementData[SwingVelocityIndex].FloatValue < 0) movementData[SwingVelocityIndex].FloatValue = 0.0f;

			}

			// Reset outside of show details
			if (movementData[CalculateAnglesIndex] == null) movementData[CalculateAnglesIndex] = new MovementVariable(DefaultCalculateAngles);
			if (movementData[LeewayIndex] == null) movementData[LeewayIndex] = new MovementVariable(DefaultLeeway);
			if (movementData[OnlyClimbRopesIndex] == null) movementData[OnlyClimbRopesIndex] = new MovementVariable(false); 
			if (movementData[HandOffsetIndex] == null) movementData[HandOffsetIndex] = new MovementVariable(DefaultHandOffset);

			showDetails = EditorGUILayout.Foldout(showDetails, "Details");
			if (showDetails)
			{
				// Rotation
				movementData[CalculateAnglesIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Rotate to Match Rope", "Should the character rotate to match rope sections angle?"), movementData[CalculateAnglesIndex].BoolValue);

				// Leeway
				movementData[LeewayIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Leeway", "How close to the rope the character needs to be before they snap to the middle."), movementData[LeewayIndex].FloatValue);
				if (movementData[LeewayIndex].FloatValue < 0) movementData[LeewayIndex].FloatValue = 0.0f;

				// Only Climb Ropes
				movementData[OnlyClimbRopesIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Only Climb Ropes","Should this movement only be used for climbing ropes. If false it will also be used for ladders and other climbables. "), movementData[OnlyClimbRopesIndex].BoolValue);

				// Hand offset
				movementData[HandOffsetIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Hand Offset", "The difference in y between the rope grabbing position (i.e. usually the hand) and the characters pivot."), movementData[HandOffsetIndex].FloatValue);
			}
	
			// Help Box if a weird combination is found
			string warning = CheckForUnexpectedSettings(movementData, target);
			if (warning != null) EditorGUILayout.HelpBox(warning, MessageType.Warning, true);

			return movementData;
		}

		/// <summary>
		/// Checks for unexpected settings combinations and returns a warning if they are found.
		/// </summary>
		/// <returns>The string describing unexpected settings, or null if none found..</returns>
		protected static string CheckForUnexpectedSettings(MovementVariable[] movementData, Character target)
		{
			if (movementData[AvailableDismountsIndex].IntValue == 0) return "No method for dismounting rope has been selected. This may mean you cannot get off from a rope.";

			if (movementData[CalculateAnglesIndex].BoolValue && !target.rotateToSlopes ) return "You have set the character to rotate to ropes but your character doesn't allow rotation. This may cause unexpected problems.";

			return null;
		}

		#endregion

#endif


	}

}

