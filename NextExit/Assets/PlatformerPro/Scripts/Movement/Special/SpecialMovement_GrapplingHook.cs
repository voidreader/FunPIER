#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A grappling hook with various options.
	/// </summary>
	public class SpecialMovement_GrapplingHook : SpecialMovement
	{
		#region members

		/// <summary>
		/// Distance between characters transform and their hand.
		/// </summary>
		public Vector2 handOffset;

		/// <summary>
		/// The max distance the rope can travel.
		/// </summary>
		public float maxDistance;

		/// <summary>
		/// How we control the grapple.
		/// </summary>
		public GrapplingHookControlType controlType;

		/// <summary>
		/// Can we launch the grapple when gorunded?
		/// </summary>
		public bool canLaunchWhenGrounded;

		/// <summary>
		/// Speed at which the hook retracts (i.e speed which played moves up/down).
		/// </summary>
		public float retractSpeed;

		/// <summary>
		/// The additional boost we add to the swing velocity when player leaves grapple.
		/// </summary>
		public Vector2 launchVelocityBoost;

		/// <summary>
		/// The additional multiplier applied swing velocity when player leaves grapple.
		/// </summary>
		public Vector2 launchVelocityMultiplier;

		/// <summary>
		/// Speed at which the hook retracts (i.e speed which played moves up/down).
		/// </summary>
		public float slingshotRetractSpeed;

		/// <summary>
		/// Speed at which we slingshot.
		/// </summary>
		public Vector2 slingshotLaunchVelocity;

		/// <summary>
		/// When retracting by how much do we speed up the swing?
		/// </summary>
		public float drag;

		/// <summary>
		/// Gravity to use when swinging.
		/// </summary>
		public float gravity;

		/// <summary>
		/// Prefab to use for the grapple.
		/// </summary>
		public GameObject grapplePrefab;

		/// <summary>
		/// Prefab to use for the rope.
		/// </summary>
		public GameObject ropePrefab;

		/// <summary>
		/// How do we slow down?
		/// </summary>
		public SlowDownType slowDownType;

		/// <summary>
		/// Cached reference to the projectile aimer.
		/// </summary>
		protected ProjectileAimer aimer;

		/// <summary>
		/// Actual grappling hook.
		/// </summary>
		protected GrapplingHookProjectile grapple;

		/// <summary>
		/// The state.
		/// </summary>
		protected GrapplingHookState state;

		/// <summary>
		/// Tracks if we have left the ground.
		/// </summary>
		protected bool hasLeftGround;

		/// <summary>
		/// Current distance between player and rope.
		/// </summary>
		protected float ropeDistance;

		/// <summary>
		/// Slingshot after this timer reaches threshold.
		/// </summary>
		protected float buttonHeldTimer;

		/// <summary>
		/// Checks if the button is being held.
		/// </summary>
		protected bool buttonReleased;

		/// <summary>
		/// Have we started to hold button.
		/// </summary>
		protected bool buttonHoldStarted;

		/// <summary>
		/// Gameobject for the rope.
		/// </summary>
		protected GameObject ropeGo;

		/// <summary>
		/// Direction from character to grapple.
		/// </summary>
		protected Vector2 direction;

		/// <summary>
		/// Tracks how far we have travelled up the rope since slingshot started.
		/// </summary>
		protected float travelledDistance;

		/// <summary>
		/// Tracks the distance we have to travel before we slingshot.
		/// </summary>
		protected float slingshotLaunchDistance;

		/// <summary>
		/// The max rope distance.
		/// </summary>
		protected float maxRopeDistance;

		/// <summary>
		/// Time we have been swinging for.
		/// </summary>
		protected float swingTime;


		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Grappling Hook";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "A grappling hook with various options.";
		
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
		/// The index of the hand offset in the movement data.
		/// </summary>
		protected const int HandOffsetIndex = 0;

		/// <summary>
		/// The index of the max distance in the movement data.
		/// </summary>
		protected const int MaxDistanceIndex = 1;

		/// <summary>
		/// The index of the control type in the movement data.
		/// </summary>
		protected const int ControlTypeIndex = 2;

		/// <summary>
		/// The index of the retract speed in the movement data.
		/// </summary>
		protected const int RetractSpeedIndex = 3;

		/// <summary>
		/// The index of the gravity in the movement data.
		/// </summary>
		protected const int GravityIndex = 4;

		/// <summary>
		/// The index of the grapple prefab in the movement data.
		/// </summary>
		protected const int GrapplePrefabIndex = 5;

		/// <summary>
		/// The index of the rope prefab in the movement data.
		/// </summary>
		protected const int RopePrefabIndex = 6;

		/// <summary>
		/// The index of the slow down type in the movement data.
		/// </summary>
		protected const int SlowDownTypeIndex = 7;

		/// <summary>
		/// The index of the drag in the movement data.
		/// </summary>
		protected const int DragIndex = 8;

		/// <summary>
		/// The index of the slingshot retract speed value in the movement data.
		/// </summary>
		protected const int SlingshotRetractSpeedIndex = 9;

		/// <summary>
		/// The index of the slingshot launch velocity.
		/// </summary>
		protected const int SlingshotLaunchVelocityIndex = 10;

		/// <summary>
		/// The index of can launch when grounded.
		/// </summary>
		protected const int CanLaunchWhenGroundedIndex = 11;

		/// <summary>
		/// The index of launch velocity boost in the movement data.
		/// </summary>
		protected const int LaunchVelocityBoostIndex = 12;

		/// <summary>
		/// The index of launch velocity multiplier in the movement data.
		/// </summary>
		protected const int LaunchVelocityMultiplierIndex = 13;

		/// <summary>
		/// The size of the movement variable array in the movement data.
		/// </summary>
		protected const int MovementVariableCount = 14;

		/// <summary>
		/// The default max distance.
		/// </summary>
		protected const float DefaultMaxDistance = 5.0f;

		/// <summary>
		/// The default type of the control.
		/// </summary>
		protected const GrapplingHookControlType DefaultControlType = GrapplingHookControlType.AUTO_RETRACT;

		/// <summary>
		/// The default retract speed.
		/// </summary>
		protected const float DefaultRetractSpeed = 10.0f;

		/// <summary>
		/// The default gravity.
		/// </summary>
		protected const float DefaultGravity = -50f;

		/// <summary>
		/// The button held time.
		/// </summary>ns
		protected const float buttonHeldTime = 0.25f;

		/// <summary>
		/// The default launch velocity boost.
		/// </summary>
		protected static Vector2 DefaultLaunchVelocityBoost = new Vector2(2.0f, 2.0f);

		/// <summary>
		/// Maximum distance after which you will always slingshot.
		/// </summary>
		protected const float MaxRopeSlingshotDistance = 2.5f;

		#endregion

		/// <summary>
		/// Unity update hook.
		/// </summary>
		void Update()
		{
			if (buttonHoldStarted)
			{
				buttonHeldTimer += TimeManager.FrameTime;
			}
			if (state != GrapplingHookState.NONE && hasLeftGround) swingTime += TimeManager.FrameTime;
		}

		/// <summary>
		/// Initialise the movement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			if (movementData != null && movementData.Length >= MovementVariableCount)
			{
				handOffset = movementData[HandOffsetIndex].Vector2Value;
				maxDistance = movementData[MaxDistanceIndex].FloatValue;
				controlType = (GrapplingHookControlType) movementData[ControlTypeIndex].IntValue;
				retractSpeed = movementData[RetractSpeedIndex].FloatValue;
				drag = movementData[DragIndex].FloatValue;
				slingshotRetractSpeed = movementData[SlingshotRetractSpeedIndex].FloatValue;
				slingshotLaunchVelocity = movementData[SlingshotLaunchVelocityIndex].Vector2Value;
				gravity = movementData[GravityIndex].FloatValue;
				grapplePrefab = movementData[GrapplePrefabIndex].GameObjectValue;
				ropePrefab = movementData[RopePrefabIndex].GameObjectValue;
				canLaunchWhenGrounded = movementData[CanLaunchWhenGroundedIndex].BoolValue;
				launchVelocityBoost = movementData[LaunchVelocityBoostIndex].Vector2Value;
				launchVelocityMultiplier = movementData[LaunchVelocityMultiplierIndex].Vector2Value;
				slowDownType = (SlowDownType) movementData[SlowDownTypeIndex].IntValue;
			}
			else
			{
				Debug.LogError("Invalid movement data.");
			}
			
			AssignReferences (character);
			// Prefer an aimer close to this object
			aimer = GetComponentInChildren<ProjectileAimer> ();
			if (aimer == null) aimer = character.GetComponentInChildren<ProjectileAimer> ();
			return this;
		}

		/// <summary>
		/// Gets a value indicating whether this movement wants to do a special move.
		/// </summary>
		override public bool WantsSpecialMove()
		{
			// Check for jump input
			if ((character.Input.JumpButton == ButtonState.DOWN))
			{
				return false;
			}

			if (state != GrapplingHookState.NONE) return true;

			// TODO Set action button
			if (GetGrappleButtonState() == ButtonState.DOWN && (canLaunchWhenGrounded || !character.Grounded))
			{
				hasLeftGround = !character.Grounded;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Determines state of grapple button.
		/// </summary>
		virtual protected ButtonState GetGrappleButtonState()
		{
			if (character.Input.GetActionButtonState (0) == ButtonState.DOWN) return ButtonState.DOWN;
			if (character.Input.GetActionButtonState (0) == ButtonState.HELD) return ButtonState.HELD;
			if (character.Input.GetActionButtonState (0) == ButtonState.UP) return ButtonState.UP;
			return ButtonState.NONE;
		}

		/// <summary>
		/// Start the special mvoe
		/// </summary>
		override public void DoSpecialMove()
		{

			if (state == GrapplingHookState.NONE)
			{
				swingTime = 0.0f;
				buttonReleased = false;
				buttonHoldStarted = false;
				state = GrapplingHookState.LAUNCHING;
				FireGrapple ();
			}
		}

		/// <summary>
		/// This class will handle gravity internally.
		/// </summary>
		override public bool ShouldApplyGravity
		{
			get
			{
				// Handle gravity internally
				return false;
			}
		}

		/// <summary>
		/// Called by the hook when it latches on to something.
		/// </summary>
		virtual public void Latch()
		{
			ropeDistance = Vector2.Distance(((Vector2)grapple.transform.position - HandOffset), (Vector2)character.Transform.position);
			if (maxRopeDistance < ropeDistance) maxRopeDistance = Vector2.Distance(((Vector2)grapple.transform.position - HandOffset), (Vector2)character.Transform.position);
			// TODO Make a constant
			if (ropeDistance < 0.25f)
			{
				CancelGrapple();
			}
			else
			{
				state = GrapplingHookState.CLINGING;
			}
		}

		/// <summary>
		/// Cancels the grapple.
		/// </summary>
		virtual public void CancelGrapple()
		{
			state = GrapplingHookState.NONE;
			if (ropeGo != null) ropeGo.SetActive (false);
			if (grapple != null) grapple.Hide ();
			base.ConditionsActivated ();
		}

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			if (grapple == null)
			{
				state = GrapplingHookState.NONE;
				return;
			}

			// Check for hitting head
			if (character.WouldHitHeadThisFrame || WouldHitSides())
			{
				CancelGrapple();
				return;
			}

			// Cancel swing if we hit ground again
			if (hasLeftGround && character.Grounded)
			{
				CancelGrapple ();
				return;
			}

			// have we left the ground?
			if (!hasLeftGround && !character.Grounded) hasLeftGround = true;

			// Hnadle input
			HandleInputs ();

			switch (state)
			{
			case GrapplingHookState.LAUNCHING:
				if (!character.Grounded)
				{
					// Apply acceleration
					character.AddVelocity(0, TimeManager.FrameTime * character.Gravity, false);
					// Limit to terminal velocity
					if (character.Velocity.y < character.terminalVelocity) character.SetVelocityY(character.terminalVelocity);
					// Keep moving according to residual velocity
					character.Translate(character.Velocity.x * TimeManager.FrameTime, character.Velocity.y * TimeManager.FrameTime, true);
				}
				// Update rope distance
				ropeDistance = Vector2.Distance(((Vector2)grapple.transform.position - HandOffset), (Vector2)character.Transform.position);
				if (maxDistance < ropeDistance) maxDistance = ropeDistance;
				break;
			case GrapplingHookState.CLINGING: 
				if (controlType == GrapplingHookControlType.AUTO_RETRACT) state = GrapplingHookState.AUTO_RETRACTING;
				DoSwing();
				if (controlType != GrapplingHookControlType.AUTO_RETRACT && character.Input.VerticalAxisDigital == 1) DoRetract(1);
				if (controlType == GrapplingHookControlType.UP_AND_DOWN && character.Input.VerticalAxisDigital == -1 && !character.Grounded) DoRetract(-1);
				break;
			case GrapplingHookState.AUTO_RETRACTING:
				DoSwing();
				DoRetract(1);
				break;
			
			case GrapplingHookState.SLINGSHOT: 
				// DoSwing();
				DoRetract(1);
				break;
			}
			// Draw Rope
			DrawRope ();
		}

		/// <summary>
		/// Returns the direction the character is facing. 0 for none, 1 for right, -1 for left.
		/// This overriden version always returns the velocity direction.
		/// </summary>
		override public int FacingDirection
		{
			get 
			{
				if (state == GrapplingHookState.LAUNCHING && grapple != null) 
				{
					return 0;
//					// TODO should save this instead of calculating twice
//					Vector2 direction = ((Vector2)grapple.transform.position) - (Vector2)character.Transform.position;
//					direction.Normalize();
//					if (direction.x > 0.2f) return 1;
//					if (direction.x < -0.2f) return -1;
//					return 0;
				}
				if (Mathf.Abs (character.Velocity.x) < 0.4f) return 0;
				if (character.Velocity.x > 0) return 1;
				return -1;
			}
		}

		/// <summary>
		/// Called when the movement loses control.
		/// </summary>
		override public void LosingControl()
		{
			buttonReleased = false;
			buttonHoldStarted = false;
			state = GrapplingHookState.NONE;
			if (grapple != null) grapple.Hide ();
			if (ropeGo != null) ropeGo.SetActive (false);
			CancelGrapple ();
		}

		/// <summary>
		/// Handle inputs during the grapple
		/// </summary>
		virtual protected void HandleInputs()
		{

			// Early out if no state
			if (state == GrapplingHookState.NONE) return;

			// Early out if no slingshot (further input has no effect)
			if (state == GrapplingHookState.SLINGSHOT) return;

			// Check if button released
			if (GetGrappleButtonState() == ButtonState.NONE || GetGrappleButtonState() == ButtonState.UP) 
			{
				buttonReleased = true;
			}

			// Check if button pressed again
			if (buttonReleased && GetGrappleButtonState() == ButtonState.DOWN) 
			{
				buttonHoldStarted = true;
				buttonHeldTimer = 0;
			}

			// Launching we can't do anything more
			if (state == GrapplingHookState.LAUNCHING) return;

			// Check for button release
			if (buttonHoldStarted)
			{
				// Check if button held
				if (buttonHeldTimer > buttonHeldTime)
				{
					travelledDistance = 0;
					slingshotLaunchDistance = Mathf.Min (MaxRopeSlingshotDistance, ropeDistance / 2.0f);
					state = GrapplingHookState.SLINGSHOT;
				}
				// Else Cut hook
				else if (GetGrappleButtonState() == ButtonState.UP)
				{
					// Apply velovity boosts
					character.SetVelocityX(launchVelocityMultiplier.x * character.Velocity.x);
					character.SetVelocityY(launchVelocityMultiplier.y * character.Velocity.y);
					character.AddVelocity(launchVelocityBoost.x * character.FacingDirection, launchVelocityBoost.y, true);
					CancelGrapple();
				}
			}
		}

		/// <summary>
		/// Fires the grapple.
		/// </summary>
		virtual protected void FireGrapple()
		{
			if (ropeGo == null)
			{
				ropeGo = (GameObject)GameObject.Instantiate (ropePrefab);
				ropeGo.SetActive (false);
			}
			// Instantiate grapple first time
			if (grapple == null)
			{
				GameObject go = (GameObject)GameObject.Instantiate (grapplePrefab);
				grapple = go.GetComponent<GrapplingHookProjectile> ();

			}

			// Fire grapple
			grapple.transform.position = character.Transform.position + (Vector3)(aimer == null ? Vector2.zero : aimer.GetAimOffset (character));
			grapple.Fire (1, DamageType.NONE, aimer == null ? new Vector2 (0, 1) : aimer.GetAimDirection(character), character);
			maxRopeDistance = 0;
		}
	
		/// <summary>
		/// Does the retracting
		/// </summary>
		/// <param name="retractDirection">Retract direction, 1 for up -1 for down.</param>
		virtual protected void DoRetract(int retractDirection)
		{
			// Grapple cancelled
			if (state == GrapplingHookState.NONE) return;

			// TODO Add ability to move down the hook

			float actualRetractSpeed = (state == GrapplingHookState.SLINGSHOT) ? slingshotRetractSpeed : retractSpeed * retractDirection;

			// Move towards latch point
			Vector2 direction = ((Vector2)grapple.transform.position - HandOffset) - (Vector2)character.Transform.position;
			direction.Normalize();

			// NOTE: The retract velocity does not count towards the characters velocity
			character.Translate (direction.x * actualRetractSpeed * TimeManager.FrameTime, direction.y * actualRetractSpeed * TimeManager.FrameTime, true);

//			Vector2 updatedDirection = ((Vector2)grapple.transform.position - HandOffset) - (Vector2)character.Transform.position;
//			// Check if we have gone too far
//			if (direction.x > 0 && updatedDirection.x < 0 ||
//			    direction.x < 0 && updatedDirection.x > 0 ||
//			    direction.y > 0 && updatedDirection.y < 0 ||
//			    direction.y < 0 && updatedDirection.y > 0)
//			{
//				character.Transform.position = ((Vector2)grapple.transform.position - HandOffset);
//				state = GrapplingHookState.HANGING;
//			}
			ropeDistance = Vector2.Distance(((Vector2)grapple.transform.position - HandOffset), (Vector2)character.Transform.position);
			maxRopeDistance -= (actualRetractSpeed * Time.deltaTime);
			if (maxDistance < ropeDistance) maxRopeDistance = ropeDistance;

			// Speed up/Slow down
			character.SetVelocityX(character.Velocity.x * ( 1 + (TimeManager.FrameTime * drag * retractDirection)));
			character.SetVelocityY(character.Velocity.y * ( 1 + (TimeManager.FrameTime * drag * retractDirection)));

			if (state == GrapplingHookState.SLINGSHOT)
			{
				travelledDistance += actualRetractSpeed * Time.deltaTime;
				if (travelledDistance >= slingshotLaunchDistance) DoSlingShot();
			}
		}

		/// <summary>
		/// Launches the player with slingshot velocity
		/// </summary>
		virtual protected void DoSlingShot()
		{
			// Launch (slingshot)
			direction = ((Vector2)grapple.transform.position - HandOffset) - (Vector2)character.Transform.position;
			direction.Normalize();
			// Overwrite velocity
			character.SetVelocityX(direction.x * slingshotLaunchVelocity.x);
			character.SetVelocityY(direction.y * slingshotLaunchVelocity.y);
			state = GrapplingHookState.NONE;
		}

		/// <summary>
		/// Draws the rope.
		/// </summary>
		virtual protected void DrawRope()
		{
			// Hide the rope if grapple isn't visible
			if (state == GrapplingHookState.NONE || grapple == null || !grapple.IsVisible) 
			{
				if (ropeGo != null) ropeGo.SetActive (false);
				return;
			}


			float distance = Vector2.Distance((Vector2)grapple.transform.position, (Vector2)character.Transform.position + HandOffset);
			Vector2 direction = ((Vector2)grapple.transform.position - ((Vector2)character.Transform.position + HandOffset));
			direction.Normalize();
			float angle = Vector2.Angle(Vector2.right, direction);
			if ((character.Transform.position.y + HandOffset.y) > grapple.transform.position.y) angle *= -1;
			ropeGo.transform.rotation = Quaternion.Euler(0,0,angle);
			ropeGo.transform.position = (Vector2)character.Transform.position + HandOffset;
			ropeGo.transform.localScale = new Vector3(distance / 1.5f, ropeGo.transform.localScale.y, ropeGo.transform.localScale.z);
			ropeGo.SetActive (true);

            // TODO A rope type that doesn't stretch // + (direction * (distance / 2.0f)
        }

        /// <summary>
        /// Gets the hand offset offset for facing direction.
        /// </summary>
        /// <value>The hand offset.</value>
        protected Vector2 HandOffset
		{
			get
			{
				// if (character.LastFacedDirection == -1) return new Vector2 (-handOffset.x, handOffset.y);
				return new Vector2 (handOffset.x, handOffset.y);
			}
		}

		/// <summary>
		/// Are we hitting our sides? If so cancel grapple.
		/// </summary>
		virtual protected bool WouldHitSides()
		{
			int hitCount = 0;
			for (int i = 0; i < character.Colliders.Length; i++)
			{
				if (character.Colliders[i].RaycastType == RaycastType.SIDE_RIGHT || character.Colliders[i].RaycastType == RaycastType.SIDE_LEFT)
				{
					RaycastHit2D hit = character.GetClosestCollision(i);
					if (hit.collider != null)
					{
						hitCount++;
					}
				}
			}
			// Two sides hit, reset velocity (we could make this more or less)
			if (hitCount > 1) return true;
			return false;
		}


		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				if (!hasLeftGround && character.Grounded) return AnimationState.GRAPPLE_THROW;
				if (state == GrapplingHookState.LAUNCHING)
				{
					return AnimationState.GRAPPLE_THROW;
				}
				return AnimationState.GRAPPLE_SWING;
			}
		}

		/// <summary>
		/// Does the swinging movement
		/// </summary>
		virtual protected void DoSwing()
		{

			// Early out in case we reached out hang point or grapple cancelled
			if (state == GrapplingHookState.NONE) return;

			// Early out grounded
			if (character.Grounded) return;

			// Early out if rope is too short to swing TODO make a constant
			if (ropeDistance < 0.2f) return;

			Vector2 originalPosition =  character.Transform.position;

			// Calculate gravity
			float newYVelocity = character.Velocity.y + (gravity * TimeManager.FrameTime);

			// Move to new position
			character.Translate (character.Velocity.x * TimeManager.FrameTime, newYVelocity * TimeManager.FrameTime , true);

			// Calculate new direction
			Vector2 direction = ((Vector2)grapple.transform.position - HandOffset) - (Vector2)character.Transform.position;
			direction.Normalize ();

			// Update position to match rope length
			float distance = Vector2.Distance(((Vector2)grapple.transform.position - HandOffset), (Vector2)character.Transform.position);
			if (distance > maxRopeDistance) 
			{
				Vector2 displacement = direction * -(maxRopeDistance - distance);
				character.Translate (displacement.x, displacement.y , true);
			}

			// Retroactively update velocity
			Vector2 updatedVelocity = ((Vector2)character.Transform.position - originalPosition) / TimeManager.FrameTime;
			character.SetVelocityX (updatedVelocity.x);
			character.SetVelocityY(updatedVelocity.y);


			// Speed up/Drag
			if (slowDownType == SlowDownType.DRAG_FACTOR)
			{
				character.SetVelocityX(character.Velocity.x * ( 1 + (TimeManager.FrameTime * drag)));
				character.SetVelocityY(character.Velocity.y * ( 1 + (TimeManager.FrameTime * drag)));
			}
			else if (slowDownType == SlowDownType.EXPONENTIAL)
			{
				character.SetVelocityX(character.Velocity.x * ( 1 + (TimeManager.FrameTime * drag * swingTime)));
				character.SetVelocityY(character.Velocity.y * ( 1 + (TimeManager.FrameTime * drag * swingTime)));
			}

		}

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

			GUILayout.Label ("Controls", EditorStyles.boldLabel);

			// Control Type
			if (movementData [ControlTypeIndex] == null) movementData [ControlTypeIndex] = new MovementVariable ((int)DefaultControlType);
			movementData [ControlTypeIndex].IntValue = (int)(GrapplingHookControlType)EditorGUILayout.EnumPopup (new GUIContent ("Control Type", "What type of control system to use."), (GrapplingHookControlType)movementData [ControlTypeIndex].IntValue);
		 	EditorGUILayout.HelpBox (((GrapplingHookControlType)(movementData [ControlTypeIndex].IntValue)).GetDescription (), MessageType.Info);

			if (movementData [CanLaunchWhenGroundedIndex] == null) movementData [CanLaunchWhenGroundedIndex] = new MovementVariable ();
			movementData [CanLaunchWhenGroundedIndex].BoolValue = EditorGUILayout.Toggle (new GUIContent ("Fire When Grounded", "Can we shoot a grapple while on the ground?"), movementData [CanLaunchWhenGroundedIndex].BoolValue);

			if (movementData [SlingshotRetractSpeedIndex] == null) movementData [SlingshotRetractSpeedIndex] = new MovementVariable ();
			bool enableSlingShot = EditorGUILayout.Toggle (new GUIContent ("Enable Slingshot", "Can the user slingshot back up the grapple?"), movementData [SlingshotRetractSpeedIndex].FloatValue > 0);
			if (movementData [SlingshotRetractSpeedIndex].FloatValue <= 0 && enableSlingShot) movementData [SlingshotRetractSpeedIndex].FloatValue = 1.0f;
			else if (!enableSlingShot) movementData [SlingshotRetractSpeedIndex].FloatValue = 0;

			GUILayout.Label ("Behaviour", EditorStyles.boldLabel);

			// Max Distance
			if (movementData [MaxDistanceIndex] == null) movementData [MaxDistanceIndex] = new MovementVariable (DefaultMaxDistance);
			movementData [MaxDistanceIndex].FloatValue = EditorGUILayout.FloatField (new GUIContent ("Max Distance", "Maximum distance the rope can travel."), movementData [MaxDistanceIndex].FloatValue);
			if (movementData [MaxDistanceIndex].FloatValue <= 0) movementData [MaxDistanceIndex].FloatValue = DefaultMaxDistance;

			// Retract Speed
			if (movementData [RetractSpeedIndex] == null) movementData [RetractSpeedIndex] = new MovementVariable (DefaultRetractSpeed);
			movementData [RetractSpeedIndex].FloatValue = EditorGUILayout.FloatField (new GUIContent ("Retract Speed", "Speed at which the hook retracts (character moves)."), movementData [RetractSpeedIndex].FloatValue);
			if (movementData [RetractSpeedIndex].FloatValue <= 0) movementData [RetractSpeedIndex].FloatValue = DefaultRetractSpeed;


			// Acceleration
			if (movementData [GravityIndex] == null) movementData [GravityIndex] = new MovementVariable (DefaultGravity);
			movementData [GravityIndex].FloatValue = EditorGUILayout.FloatField (new GUIContent ("Acceleration", "Downwards force to apply."), movementData [GravityIndex].FloatValue);
			if (movementData [GravityIndex].FloatValue >= 0) movementData [GravityIndex].FloatValue = DefaultGravity;

			// Slow down Type
			if (movementData [SlowDownTypeIndex] == null) movementData [SlowDownTypeIndex] = new MovementVariable ();
			movementData [SlowDownTypeIndex].IntValue = (int)(SlowDownType)EditorGUILayout.EnumPopup (new GUIContent ("Slow Down Type", "How do we slow down when swinging?"), (SlowDownType)movementData [SlowDownTypeIndex].IntValue);
			EditorGUILayout.HelpBox (((SlowDownType)(movementData [SlowDownTypeIndex].IntValue)).GetDescription (), MessageType.Info);

			if (movementData [SlowDownTypeIndex].IntValue != (int)SlowDownType.NATURAL)
			{
				// Slow down Type supporting value
				if (movementData [DragIndex] == null) movementData [DragIndex] = new MovementVariable ();
				movementData [DragIndex].FloatValue = EditorGUILayout.FloatField (new GUIContent ("Drag Factor", "How much extra do we slow down."), movementData [DragIndex].FloatValue);
			}


			GUILayout.Label ("Behaviour (Launch)", EditorStyles.boldLabel);

			// Velocity Multiplier
			if (movementData [LaunchVelocityMultiplierIndex] == null) movementData [LaunchVelocityMultiplierIndex] = new MovementVariable (Vector2.one);
			movementData [LaunchVelocityMultiplierIndex].Vector2Value = EditorGUILayout.Vector2Field (new GUIContent ("Launch Velocity Multiplier", "Multiply velocities by these values when launching off the grapple."), movementData [LaunchVelocityMultiplierIndex].Vector2Value);

			// Velocity Boost
			if (movementData [LaunchVelocityBoostIndex] == null) movementData [LaunchVelocityBoostIndex] = new MovementVariable (DefaultLaunchVelocityBoost);
			movementData [LaunchVelocityBoostIndex].Vector2Value = EditorGUILayout.Vector2Field (new GUIContent ("Launch Velocity Boost", "Additional velocity to add when launching off the grapple."), movementData [LaunchVelocityBoostIndex].Vector2Value);

			if (movementData [SlingshotRetractSpeedIndex].FloatValue > 0)
			{
				GUILayout.Label ("Behaviour (Slingshot)", EditorStyles.boldLabel);

				// Slingshot retract speed
				if (movementData [SlingshotRetractSpeedIndex] == null)
					movementData [SlingshotRetractSpeedIndex] = new MovementVariable ();
				movementData [SlingshotRetractSpeedIndex].FloatValue = EditorGUILayout.FloatField (new GUIContent ("Retract Speed", "Speed at which the hook retracts (character moves) when slingshotting."), movementData [SlingshotRetractSpeedIndex].FloatValue);

				// Slingshot
				if (movementData [SlingshotLaunchVelocityIndex] == null) movementData [SlingshotLaunchVelocityIndex] = new MovementVariable ();
				movementData [SlingshotLaunchVelocityIndex].Vector2Value = EditorGUILayout.Vector2Field (new GUIContent ("Launch Velocity", "Speed at which we slingshot."), movementData [SlingshotLaunchVelocityIndex].Vector2Value);
			}

			GUILayout.Label ("Visuals", EditorStyles.boldLabel);

			// Hand offset
			if (movementData [HandOffsetIndex] == null) movementData [HandOffsetIndex] = new MovementVariable ();
			movementData [HandOffsetIndex].Vector2Value = EditorGUILayout.Vector2Field (new GUIContent ("Hand Offset", "Distance between the characters hand and the characters transform."), movementData [HandOffsetIndex].Vector2Value);

			// Grapple Prefab
			if (movementData [GrapplePrefabIndex] == null) movementData [GrapplePrefabIndex] = new MovementVariable ();
			movementData [GrapplePrefabIndex].GameObjectValue = (GameObject) EditorGUILayout.ObjectField(new GUIContent ("Grapple Prefab", "Prefab to use for the grapple."), movementData [GrapplePrefabIndex].GameObjectValue, typeof(GameObject), false);
			if (movementData [GrapplePrefabIndex].GameObjectValue != null && movementData [GrapplePrefabIndex].GameObjectValue .GetComponent<GrapplingHookProjectile>() == null)
			{
				EditorGUILayout.HelpBox ("The grapple prefab should have a GrapplingHookProjectile component.", MessageType.Warning);
			}

			// Rope prefab
			if (movementData [RopePrefabIndex] == null) movementData [RopePrefabIndex] = new MovementVariable ();
			movementData [RopePrefabIndex].GameObjectValue = (GameObject) EditorGUILayout.ObjectField(new GUIContent ("Rope Prefab", "Prefab to use for the rope."), movementData [RopePrefabIndex].GameObjectValue, typeof(GameObject), false);



			return movementData;
		}


		#endregion

#endif

	}

	/// <summary>
	/// Grappling hook state.
	/// </summary>
	public enum GrapplingHookState
	{
		NONE,
		LAUNCHING,
		CLINGING,
		AUTO_RETRACTING,
		SLINGSHOT
	}

	/// <summary>
	/// Grappling hook control type.
	/// </summary>
	public enum GrapplingHookControlType
	{
		AUTO_RETRACT,
		UP_ONLY,
		UP_AND_DOWN
	}

	/// <summary>
	/// Different ways to slow the swing.
	/// </summary>
	public enum SlowDownType
	{
		NATURAL,
		DRAG_FACTOR,
		EXPONENTIAL
	}

	/// <summary>
	/// Grappling hook control type extensions.
	/// </summary>
	public static class GrapplingHookControlTypeExtensions
	{
		public static string GetDescription(this GrapplingHookControlType me)
		{
			switch(me)
			{
			case GrapplingHookControlType.AUTO_RETRACT: return "The character starts moving up the hook as soon as it latches on to something.";
			case GrapplingHookControlType.UP_ONLY: return "The character can press a button to move higher up the rope.";
			case GrapplingHookControlType.UP_AND_DOWN: return "The character can move both up and down the rope.";
			}
			return "No information available.";
		}
		
	}
	/// <summary>
	/// Grappling hook control type extensions.
	/// </summary>
	public static class GrapplingHookSlowDownTypeExtensions
	{
		public static string GetDescription(this SlowDownType me)
		{
			switch(me)
			{
			case SlowDownType.NATURAL: return "The swing will gradually slow down.";
			case SlowDownType.DRAG_FACTOR: return "The natural slow down will be multipled by drag, you can use a negative value for speed up.";
			case SlowDownType.EXPONENTIAL: return "The longer the swing goes the more rapidly we slow down.";
			}
			return "No information available.";
		}
		
	}

}
