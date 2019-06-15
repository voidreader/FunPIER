using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Air movement which allows you teleport multiple times in any direction. Note that you still move through space when teleporting
	/// </summary>
	public class AirMovement_TeleportJump : AirMovement
	{
		/// <summary>
		/// How fast the character dashes.
		/// </summary>
		public float warpSpeed;

		/// <summary>
		/// How fast the character dashes.
		/// </summary>
		public float warpDistance;

		/// <summary>
		/// Are we invincible when teleporting?
		/// </summary>
		public bool isInvincible;

		/// <summary>
		/// How many times can we teleport.
		/// </summary>
		public int maxWarpCount;

		/// <summary>
		/// How quickly does the user have to trigger the next warp if they want to multiwarp. 0 means no time limit.
		/// </summary>
		public float timeToNextWarp;

		/// <summary>
		/// How do we control the jump.
		/// </summary>
		public TeleportJumpControlType controlType;

		/// <summary>
		/// Action button to use if control type is ACTION_BUTTON_AND_DIRECTION.
		/// </summary>
		public int actionButton;

		/// <summary>
		/// Vector 2 representing direction and speed of travel.
		/// </summary>
		protected Vector2 teleportVelocity;

		/// <summary>
		/// True when we are currently warping.
		/// </summary>
		protected bool isWarping;

		/// <summary>
		/// Current warp count, only reset when grounded or on to wall.
		/// </summary>
		protected int warpCount;

		/// <summary>
		/// How far have we travelled in the current warp.
		/// </summary>
		protected float distanceTravelled; 

		/// <summary>
		/// How long to next warp.
		/// </summary>
		protected float warpTimer;

		/// <summary>
		/// Cached character health.
		/// </summary>
		protected CharacterHealth characterHealth;

		/// <summary>
		/// Cached event args.
		/// </summary>
		protected CharacterEventArgs eventArgs;

		#region events


		/// <summary>
		/// Event for start teleport.
		/// </summary>
		public event System.EventHandler <CharacterEventArgs> StartTeleport;

		/// <summary>
		/// Event for stop teleport.
		/// </summary>
		public event System.EventHandler <CharacterEventArgs> EndTeleport;

		/// <summary>
		/// Event for interrupt teleport.
		/// </summary>
		public event System.EventHandler <CharacterEventArgs> InterruptTeleport;

		/// <summary>
		/// Raises the start teleport event.
		/// </summary>
		virtual protected void OnStartTeleport()
		{
			if (StartTeleport != null)
			{
				StartTeleport(this, eventArgs);
			}
		}

		/// <summary>
		/// Raises the end teleport event.
		/// </summary>
		virtual protected void OnEndTeleport()
		{
			if (EndTeleport != null)
			{
				EndTeleport(this, eventArgs);
			}
		}

		/// <summary>
		/// Raises the interrupt teleport event.
		/// </summary>
		virtual protected void OnInterruptTeleport()
		{
			if (InterruptTeleport != null)
			{
				InterruptTeleport(this, eventArgs);
			}
		}
			
		#endregion

		#region constants

		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Special/Teleport Jump";

		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Air movement which allows you teleport multiple times in any direction. Note that you still move through space when teleporting.";

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
		/// The default speed.
		/// </summary>
		protected static float DefaulWarpSpeed = 10.0f;

		/// <summary>
		/// The default distance.
		/// </summary>
		protected static float DefaultWarpDistance = 3.0f;

		/// <summary>
		/// The index of the speed in the movement data.
		/// </summary>
		protected const int WarpSpeedIndex = 0;

		/// <summary>
		/// The index of the distance in the movement data.
		/// </summary>
		protected const int WarpDistanceIndex = 1;

		/// <summary>
		/// The index of the is invincible setting in the movement data.
		/// </summary>
		protected const int IsInvincibleIndex = 2;

		/// <summary>
		/// The index of the max warp count in the movement Data
		/// </summary>
		protected const int MaxWarpsIndex = 3;

		/// <summary>
		/// The index of the control type in the movement Data
		/// </summary>
		protected const int ControlTypeIndex = 4;

		/// <summary>
		/// The index of the action button in the movement Data
		/// </summary>
		protected const int ActionButtonindex = 5;

		/// <summary>
		/// The index of the time to next warp in the movement data.
		/// </summary>
		protected const int TimeToNextWarpIndex = 6;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 7;

		#endregion

		/// <summary>
		/// Initialise the movement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			AssignReferences (character);

				// Set variables
			if (movementData != null && movementData.Length == MovementVariableCount)
			{
				warpSpeed = movementData[WarpSpeedIndex].FloatValue;
				warpDistance = movementData[WarpDistanceIndex].FloatValue;
				maxWarpCount = movementData[MaxWarpsIndex].IntValue;
				isInvincible = movementData[IsInvincibleIndex].BoolValue;
				controlType = (TeleportJumpControlType) movementData[ControlTypeIndex].IntValue;
				actionButton = movementData [ActionButtonindex].IntValue;
				timeToNextWarp = movementData [TimeToNextWarpIndex].FloatValue;
			}
			else
			{
				Debug.LogError("Invalid movement data.");
			}

			if (isInvincible)
			{
				characterHealth = character.GetComponentInChildren<CharacterHealth> ();
				if (characterHealth == null)
				{
					Debug.LogWarning ("Teleport Jump couldn't find a character health and thus can't set invincibility.");
				}
			}
			eventArgs = new CharacterEventArgs (character);
			return this;
		}

		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update()
		{
			// Warp timer
			if (warpTimer > 0) warpTimer -= TimeManager.FrameTime;
			// Reset warp count
			if (warpCount > 0)
			{
				if (character.Grounded) warpCount = 0;
				else if (character.ActiveMovement is WallMovement) warpCount = 0;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="PlatformerPro.Movement"/> expects
		/// gravity to be applied after its movement finishes.
		/// </summary>
		override public bool ShouldApplyGravity
		{
			get 
			{
				return false;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this movement wants to intitiate the jump.
		/// </summary>
		/// <value>true</value>
		/// <c>false</c>
		override public bool WantsJump()
		{
			// In air (should we add some tougher conditions?)
			if (character.Grounded) return false;
				
			// Warp count exceeded
			if (warpCount >= maxWarpCount) return false;

			// Currently warping
			if (isWarping) return false;

			// Warp timer
			if (warpCount > 0 && timeToNextWarp > 0 && warpTimer <= 0 ) return false;

			// Button pressed
			if (CheckInput()) return true;

			return false;
		}

		/// <summary>
		/// Gets a value indicating whether this movement wants to control the movement in the air.
		/// Default is false with movement falling back to default air. Override if you want control.
		/// </summary>
		/// <value>true</value>
		/// <c>false</c>
		override public bool WantsAirControl()
		{
			if (isWarping) return true;
			return false;
		}

		/// <summary>
		/// Do the jump.
		/// </summary>
		override public void DoJump()
		{
			teleportVelocity = GetAimDirection () * warpSpeed;
			distanceTravelled = 0;
			isWarping = true;
			warpCount++;
			OnStartTeleport ();
		}

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			if (CheckForCancel ())
			{
				isWarping = false;
				// Set max warp count, after we hit something we can no longer warp
				warpCount = maxWarpCount;
				OnInterruptTeleport ();
			}
			else
			{
				character.SetVelocityX(teleportVelocity.x);
				character.SetVelocityY(teleportVelocity.y);
				character.Translate(teleportVelocity.x * TimeManager.FrameTime, teleportVelocity.y * TimeManager.FrameTime, false);
				distanceTravelled += Mathf.Abs(teleportVelocity.x * TimeManager.FrameTime) + Mathf.Abs(teleportVelocity.y * TimeManager.FrameTime);
				// Note this means its possible to travel slightly faster than set distance (at most MaxFrameTime * speed), this should be fine
				if (distanceTravelled >= warpDistance)
				{
					isWarping = false;
					warpTimer = timeToNextWarp;
					OnEndTeleport ();
				}
			}
		}

		/// <summary>
		/// Checks if we hit something
		/// </summary>
		/// <returns><c>true</c> if we hit something and should cancel the attack, <c>false</c> otherwise.</returns>
		virtual protected bool CheckForCancel() 
		{
			// Check for grounded only when moving downwards (grounded often has a large look ahead if we don't do this it might cancel too early)
			if (teleportVelocity.y <= 0 && character.Grounded) return true;
			if (CheckSideCollisions(character, 1, (teleportVelocity.x > 0 ? RaycastType.SIDE_RIGHT : RaycastType.SIDE_LEFT))) return true;
			if (CheckSideCollisions(character, 1, RaycastType.HEAD)) return true;
			return false;
		}

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		/// <value>The state of the animation.</value>
		override public AnimationState AnimationState
		{
			get 
			{
				return AnimationState.TELEPORT_JUMP;
			}
		}

		/// <summary>
		/// Called when the movement gets control. Typically used to do initialisation of velocity and the like.
		/// </summary>
		/// <returns><c>true</c>, if control was gained, <c>false</c> otherwise.</returns>
		override public void GainControl() 
		{
			if (isInvincible) characterHealth.SetInvulnerable ();
			isWarping = true;
			distanceTravelled = 0;
		}

		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions.
		/// </summary>
		override public void LosingControl()
		{
			if (isInvincible) characterHealth.SetVulnerable ();
			isWarping = false;
			distanceTravelled = 0;
		}

		/// <summary>
		/// Check if user has entered jump input.
		/// </summary>
		/// <returns><c>true</c>, if input was checked, <c>false</c> otherwise.</returns>
		virtual protected bool CheckInput()
		{
			switch (controlType)
			{
			case TeleportJumpControlType.JUMP_AND_DIRECTION:
				if (character.Input.JumpButton == ButtonState.DOWN) return true;
				break;
			case TeleportJumpControlType.ACTION_BUTTON_AND_DIRECTION:
				if (character.Input.GetActionButtonState (actionButton) == ButtonState.DOWN) return true;
				break;
			case TeleportJumpControlType.ALT_AXIS:
				if (character.Input.AltVerticalAxisState == ButtonState.DOWN ||
				    character.Input.AltHorizontalAxisState == ButtonState.DOWN)
				{
					return true;
				}
				break;
			}
			return false;
		}

		virtual protected Vector2 GetAimDirection()
		{
			Vector2 result = Vector2.zero;
			switch (controlType)
			{
			case TeleportJumpControlType.JUMP_AND_DIRECTION:
				result = new Vector2 (character.Input.HorizontalAxis, character.Input.VerticalAxis).normalized;
				break;
			case TeleportJumpControlType.ACTION_BUTTON_AND_DIRECTION:
				result = new Vector2 (character.Input.HorizontalAxis, character.Input.VerticalAxis).normalized;
				break;
			case TeleportJumpControlType.ALT_AXIS:
				result = new Vector2 (character.Input.AltHorizontalAxis, character.Input.AltVerticalAxis).normalized;
				break;
			}
			if (result.sqrMagnitude < 0.9f) result = Vector2.up;
			return result;
		}

		/// <summary>
		/// Gets the remainig warp time.
		/// </summary>
		/// <returns>The warp time remaining.</returns>
		public float WarpTimeRemaining
		{
			get 
			{
				if (isWarping) return 0;
				if (timeToNextWarp <= 0) return 0;
				return Mathf.Max(0, timeToNextWarp - warpTimer); 
			}
		}

		/// <summary>
		/// Gets the remainig warp time as percentage bweten 0 and 1 (e.g. for fill bar).
		/// </summary>
		/// <returns>The warp time remaining.</returns>
		public float WarpTimeRemainingAsPercentage
		{
			get 
			{
				if (isWarping) return 1.0f;
				if (timeToNextWarp <= 0) return 1.0f;
				return Mathf.Max(0, ((timeToNextWarp - warpTimer) / timeToNextWarp)); 
			}
		}
	}

	public enum TeleportJumpControlType
	{
		JUMP_AND_DIRECTION,
		ACTION_BUTTON_AND_DIRECTION,
		ALT_AXIS
	}
}