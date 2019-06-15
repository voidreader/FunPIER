using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// An extension to BasicAttacks which does a power bomb attack.
	/// </summary>
	public class PowerBombAttack : BasicAttacks
	{
		/// <summary>
		/// How fast the character dashes.
		/// </summary>
		public float pauseTime = 0.5f;

		/// <summary>
		/// Gravity to apply as we fall
		/// </summary>
		public float dropGravity = -45f;

		/// <summary>
		/// Time the character stays in landing position causing damage.
		/// </summary>
		public float landingTime = 0.5f;

		/// <summary>
		/// Minimum speed, you must be going faster than this to trigger the attack. 
		/// </summary>
		public float  minVelocity;

		/// <summary>
		/// Maximum speed, you must be going slower than this to trigger the attack.
		/// </summary>
		public float  maxVelocity;

		/// <summary>
		/// If true user must hold down as well as pressing key to trigger power bomb.
		/// </summary>
		public bool requireDownKey;

		/// <summary>
		/// Landing damage hit box.
		/// </summary>
		public CharacterHitBox landingDamageHitHox;

		/// <summary>
		/// Timer for the time we spend paused.
		/// </summary>
		protected float isPausedTimer;

		/// <summary>
		/// Timer for the time we spend landed.
		/// </summary>
		protected float hasLandedTimer;

		protected bool hasLanded;

		/// <summary>
		/// Event raised when landing starts.
		/// </summary>
		public event System.EventHandler <AttackEventArgs> PowerBombLand;

		/// <summary>
		/// The cached attack event args.
		/// </summary>
		protected AttackEventArgs attackEventArgs;

		/// <summary>
		/// Raises the power bomb land event.
		/// </summary>
		virtual protected void OnPowerBombLand()
		{
			if (PowerBombLand != null)
			{
				attackEventArgs.UpdateAttackStartedArgs(attacks[0].name);
				PowerBombLand(this, attackEventArgs);
			}
		}

		/// <summary>
		/// Used by the inspector to determine if a given attack can have multiple attacks defined in it.
		/// </summary>
		override public bool CanHaveMultipleAttacks
		{
			get { return false; }
		}

		/// <summary>
		/// Used by the inspector to determine if a given attack allows user to change attack type.
		/// </summary>
		override public bool CanUserSetAttackType
		{
			get { return false; }
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
		/// Init this instance.
		/// </summary>
		override protected void PostInit()
		{
			base.PostInit ();
			if (landingDamageHitHox != null) landingDamageHitHox.Init(new DamageInfo(attacks[0].damageAmount, attacks[0].damageType, Vector2.zero, character));
			attackEventArgs = new AttackEventArgs (character, null);
		}


		/// <summary>
		/// Is the character in the right place for the given attack.
		/// </summary>
		/// <returns><c>true</c>, if location was checked, <c>false</c> otherwise.</returns>
		/// <param name="attackData">Attack data.</param>
		override protected bool CheckLocation(BasicAttackData attackData)
		{
			if (character.Grounded) return false;
			if (character.ActiveMovement is AirMovement && CheckVelocity())
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Checks the velocity is suitable for launching this attack.
		/// </summary>
		/// <returns><c>true</c>, if velocity is okay, <c>false</c> otherwise.</returns>
		virtual protected bool CheckVelocity() 
		{
			if (character.Velocity.y > minVelocity && character.Velocity.y < maxVelocity) return true;
			return false;
		}

		/// <summary>
		/// Is the input correct for the given attack. This implmenetation is simple a key press, but another could
		/// be more complex (queueable combo attacks, or complex key combinations).
		/// </summary>
		/// <returns><c>true</c>, if input was checked, <c>false</c> otherwise.</returns>
		/// <param name="attackData">Attack data.</param>
		override protected bool CheckInput(BasicAttackData attackData)
		{
			if ((character.Input.GetActionButtonState(attackData.actionButtonIndex) == ButtonState.DOWN) &&
				(!requireDownKey ||character.Input.VerticalAxisDigital == -1))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Do whichever attack is available.
		/// </summary>
		/// <returns>true if this movement wants to main control of movement, false otherwise</returns>
		override public bool Attack()
		{
			hasLanded = false;
			isPausedTimer = pauseTime;
			hasLandedTimer = 0.0f;
			bool result = base.Attack ();
			if (result) 
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Do a melee attack.
		/// </summary>
		/// <param name="attackIndex">Attack index.</param>
		override protected IEnumerator DoAttack(int attackIndex)
		{
			currentAttackTimer = 0.0f;

			// Air pause
			while (isPausedTimer > 0)
			{
				isPausedTimer -= TimeManager.FrameTime;
				yield return true;
			}
			if (attacks [attackIndex].hitBox != null)
			{
				attacks [attackIndex].hitBox.Enable ();
				attacks [attackIndex].hitBox.UpdateDamageInfo(attacks[currentAttack].damageAmount, attacks[currentAttack].damageType);
			}

			// Attack forever (until we land)
			while (!character.Grounded)
			{
				currentAttackTimer += TimeManager.FrameTime;
				yield return true;
			}

			// Landed
			hasLandedTimer = landingTime;
			if (attacks [attackIndex].hitBox != null)
			{
				attacks[attackIndex].hitBox.ForceStop();
			}
			if (landingDamageHitHox != null)
			{
				// Note you could use different damage for landing daamge and other damage
				landingDamageHitHox.Enable();
				landingDamageHitHox.UpdateDamageInfo(attacks[currentAttack].damageAmount, attacks[currentAttack].damageType);
			}
			hasLanded = true;
			OnPowerBombLand ();
			while (hasLandedTimer > 0)
			{
				hasLandedTimer -= TimeManager.FrameTime;
				currentAttackTimer += TimeManager.FrameTime;
				yield return true;
			}

			// Attack finished
			currentAttack = -1;
			currentAttackTimer = 0.0f;
			if (landingDamageHitHox != null) landingDamageHitHox.ForceStop();
			// character.OnChangeAnimationState ();
			character.FinishedAttack(attacks[attackIndex].name);

			// Set cooldown
			if (cooldownTimers != null) cooldownTimers [attackIndex] = attacks [attackIndex].coolDown;
		}

		
		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			if (isPausedTimer > 0)
			{
				character.SetVelocityX (0);
				character.SetVelocityY (0);
			}
			else if (hasLandedTimer > 0)
			{
				character.SetVelocityX (0);
				character.SetVelocityY (0);
			}
			else
			{
				// Apply gravity
				if (!character.Grounded || character.Velocity.y > 0)
				{
					character.AddVelocity(0, TimeManager.FrameTime * dropGravity, false);
				}
				// Translate
				character.Translate(0, character.Velocity.y * TimeManager.FrameTime, true);
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
				if (isPausedTimer > 0) return AnimationState.POWER_BOMB_CHARGE;
				if (hasLanded) return AnimationState.POWER_BOMB_LAND;
				if (currentAttack != -1) return attacks[currentAttack].animation;
				return AnimationState.POWER_BOMB;
			}
		}

	}

}