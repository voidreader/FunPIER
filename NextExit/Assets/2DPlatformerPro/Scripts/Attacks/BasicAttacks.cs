using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Handles the setup of basic single button attacks for air and ground. This is a "special" movement.
	/// It derives from movement but has a lot of additional functionality.
	/// </summary>
	public class BasicAttacks : Movement
	{
		/// <summary>
		/// The attack data.
		/// </summary>
		public List<BasicAttackData> attacks;

		/// <summary>
		/// Does the attack also override movement. If false then the attack system will not control
		/// movement and animations handling will depend on attackSystemWantsAnimationStateOverride.
		/// </summary>
		public bool attackSystemWantsMovementControl;

		/// <summary>
		/// Does the attack override animation. If false then the attack system will only set an animation override not an animation state.
		/// </summary>
		public bool attackSystemWantsAnimationStateOverride;

		/// <summary>
		/// Index of the current attack or -1 if no current attack.
		/// </summary>
		protected int currentAttack;

		/// <summary>
		/// The timer for the current attack.
		/// </summary>
		protected float currentAttackTimer;

		/// <summary>
		/// Cached reference to a projectile aimer, or null if there is no aimer.
		/// </summary>
		protected ProjectileAimer projectileAimer;

		/// <summary>
		/// The index of the deferred attack.
		/// </summary>
		protected int deferredAttackIndex;

		/// <summary>
		/// Cached reference to the item manager used for ammo.
		/// </summary>
		protected ItemManager itemManager;

		/// <summary>
		/// The cooldown timers.
		/// </summary>
		protected float[] cooldownTimers;

		#region movement info constants and properties
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Basic Attacks";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Basic Attack class.";
		
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
		/// Gets a value indicating whether this instance is attacking.
		/// </summary>
		/// <value><c>true</c> if this instance is attacking; otherwise, <c>false</c>.</value>
		virtual public bool IsAttacking
		{
			get
			{
				if (currentAttack != -1) return true;
				return false;
			}
		}

		/// <summary>
		/// Gets the name of the active attack or null if no attack is aactive.
		/// </summary>
		/// <value><c>true</c> if active attack name; otherwise, <c>false</c>.</value>
		virtual public string ActiveAttackName
		{
			get
			{
				if (currentAttack != -1) return attacks[currentAttack].name;
				return null;
			}
		}

		/// <summary>
		/// Gets the normalised time of the active attack or -1 if no attack is aactive.
		/// </summary>
		/// <value><c>true</c> if active attack name; otherwise, <c>false</c>.</value>
		virtual public float ActiveAttackNormalisedTime
		{
			get
			{
				if (currentAttack != -1) return currentAttackTimer / attacks[currentAttack].attackTime;
				return -1.0f;
			}
		}

		/// <summary>
		/// Gets the active attack location or ANY if no  activeattack.
		/// </summary>
		virtual public AttackLocation ActiveAttackLocation
		{
			get
			{
				if (currentAttack != -1) return attacks[currentAttack].attackLocation;
				return AttackLocation.ANY;
			}
		}

		/// <summary>
		/// Returns true if the current attacks hit box has hit an enemy.
		/// </summary>
		virtual public bool ActiveAttackHasHit
		{
			get
			{
				if (currentAttack != -1) return attacks[currentAttack].hitBox.HasHit;
				return false;
			}
		}

		/// <summary>
		/// Used by the inspector to determine if a given attack can have multiple attacks defined in it.
		/// </summary>
		virtual public bool CanHaveMultipleAttacks
		{
			get { return true; }
		}

		/// <summary>
		/// Used by the inspector to determine if a given attack can have multiple attacks defined in it.
		/// </summary>
		virtual public bool CanUserSetAttackType
		{
			get { return true; }
		}


		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update()
		{
			if (cooldownTimers != null)
			{
				for (int i = 0; i < cooldownTimers.Length; i++) 
				{
					if (cooldownTimers[i] > 0) cooldownTimers[i] -= TimeManager.FrameTime;
				}
			}
		}

		/// <summary>
		/// Initialise this movement and retyrn a reference to the ready to use movement.
		/// </summary>
		override public Movement Init(Character character)
		{
			base.Init (character);
			PostInit ();
			return this;
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void PostInit()
		{
			bool hasCoolDowns = false;
			for (int i = 0; i < attacks.Count; i++)
			{
				if (attacks[i].hitBox != null) attacks[i].hitBox.Init(new DamageInfo(attacks[i].damageAmount, attacks[i].damageType, Vector2.zero, character));
				if (attacks[i].attackType == AttackType.PROJECTILE && attacks[i].ammoType != null && attacks[i].ammoType != "")
				{
					itemManager = character.GetComponentInChildren<ItemManager>();
					if (itemManager == null) Debug.LogWarning("Attack requires ammo but item manager could not be found");
				}
				if (attacks[i].coolDown > 0) hasCoolDowns = true;
			}
			if (hasCoolDowns) cooldownTimers = new float[attacks.Count];
			projectileAimer = GetComponent<ProjectileAimer>();
			currentAttack = -1;

		}

		/// <summary>
		/// Gets a value indicating whether this movement wants to initiate an attack.
		/// </summary>
		/// <value><c>true</c> if this instance should attack; otherwise, <c>false</c>.</value>
		virtual public bool WantsAttack()
		{
			// Can't attack if disabled
			if (!enabled) return false;

			// Can't attack if timer isn't 0
			if (currentAttackTimer > 0.0f) return false;

			// Check each attack
			for (int i = 0; i < attacks.Count; i ++)
			{
				// Not cooled down
				if (cooldownTimers != null && cooldownTimers.Length > 0 && cooldownTimers[i] > 0) break;
				// Ready to go...
				if (CheckLocation(attacks[i]) && CheckInput(attacks[i]) && CheckAmmo(attacks[i]))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		///  Gets a value indicating whether this movement wants to set the animation state.
		/// </summary>
		/// <returns><c>true</c>, if wants to set animation state, <c>false</c> otherwise.</returns>
		virtual public bool WantsToSetAnimationState()
		{
			if (currentAttack != -1 && attackSystemWantsAnimationStateOverride) return true;
			return false;
		}

		/// <summary>
		/// Do whichever attack is available.
		/// </summary>
		/// <returns>true if this movement wants to main control of movement, false otherwise</returns>
		virtual public bool Attack()
		{
			// Check each attack
			for (int i = 0; i < attacks.Count; i ++)
			{
				if (CheckLocation(attacks[i]) && CheckInput(attacks[i]) && CheckAmmo(attacks[i]))
				{
					StartAttack(i);
					return attackSystemWantsMovementControl;
				}
			}
			// We couldn't find matching attack,this shouldn't happen
			Debug.LogWarning ("Called Attack() but no suitable attack was found");
			currentAttack = -1;
			currentAttackTimer = 0.0f;
			return false;
		}

		/// <summary>
		/// Forces an attack to stop
		/// </summary>
		virtual public void InterruptAttack()
		{
			StopAllCoroutines ();
			if (currentAttack != -1)
			{
				// Reset any animation overrides before currentAttack is cleared
				if (!attackSystemWantsAnimationStateOverride && !attackSystemWantsMovementControl) character.RemoveAnimationOverride (OverrideState);

				// Attack finished
				if (attacks [currentAttack].hitBox != null)
				{
					attacks[currentAttack].hitBox.ForceStop();
				}
				currentAttackTimer = 0.0f;
				currentAttack = -1;
				character.OnChangeAnimationState ();
			}
			character.AttackFinished();
		}

		/// <summary>
		/// Gets the  cool down time for an attack.
		/// </summary>
		/// <returns>The remaining cool down.</returns>
		/// <param name="attackIndex">Attack index.</param>
		virtual public float GetAttackCoolDown(int attackIndex)
		{
			if (cooldownTimers != null && attackIndex >= 0 && attackIndex < attacks.Count) return attacks[attackIndex].coolDown;
			return 0;
		}

		/// <summary>
		/// Gets the remaining cool down time for an attack.
		/// </summary>
		/// <returns>The cool down.</returns>
		/// <param name="attackIndex">Attack index.</param>
		virtual public float GetAttackCoolDownRemaining(int attackIndex)
		{
			float remaining = 0;
			if (cooldownTimers != null && attackIndex >= 0 && attackIndex < cooldownTimers.Length) remaining =  cooldownTimers[attackIndex];
			if (remaining < 0) remaining = 0;
			return remaining;
		}

		/// <summary>
		/// Is the character in the right place for the given attack.
		/// </summary>
		/// <returns><c>true</c>, if location was checked, <c>false</c> otherwise.</returns>
		/// <param name="attackData">Attack data.</param>
		virtual protected bool CheckLocation(BasicAttackData attackData)
		{
			if (attackData.attackLocation == AttackLocation.ANY) return true;
			if (attackData.attackLocation == AttackLocation.ANY_BUT_SPECIAL &&
			    !(character.ActiveMovement is SpecialMovement) && 
			    !(character.ActiveMovement is WallMovement))
			{
				return true;
			}
			if (attackData.attackLocation == AttackLocation.GROUNDED && 
			    character.Grounded && 
			    !character.OnLadder &&
			    (character.ActiveMovement is GroundMovement || 
			 		// If the attack has control we still want to be able to trigger combos
			     	(character.ActiveMovement is BasicAttacks && (character.AttackLocation != AttackLocation.AIRBORNE))) &&
			    // Don't allow when character is about to jump
			  	character.Input.JumpButton != ButtonState.DOWN) 
			{
				return true;
			}
			if (attackData.attackLocation == AttackLocation.AIRBORNE && 
			    (character.ActiveMovement is AirMovement || 
			 		// If the attack has control we still want to be able to trigger combos
			 	 	(character.ActiveMovement is BasicAttacks && (character.AttackLocation != AttackLocation.GROUNDED))) &&
			    !character.OnLadder)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Is the input correct for the given attack. This implmenetation is simple a key press, but another could
		/// be more complex (queueable combo attacks, or complex key combinations).
		/// </summary>
		/// <returns><c>true</c>, if input was checked, <c>false</c> otherwise.</returns>
		/// <param name="attackData">Attack data.</param>
		virtual protected bool CheckInput(BasicAttackData attackData)
		{
			if (character.Input.GetActionButtonState(attackData.actionButtonIndex) == ButtonState.DOWN) return true;
			return false;
		}

		/// <summary>
		/// Checks that the player has enough ammo to fire.
		/// </summary>
		/// <returns><c>true</c>, if ammo was checked, <c>false</c> otherwise.</returns>
		/// <param name="attackData">Attack data.</param>
		virtual protected bool CheckAmmo(BasicAttackData attackData)
		{
			if (attackData.attackType == AttackType.MELEE || attackData.ammoType == null || attackData.ammoType == "") return true;
			if (itemManager.ItemCount (attackData.ammoType) > 0)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Consumes ammo.
		/// </summary>
		/// <returns><c>true</c>, if ammo was checked, <c>false</c> otherwise.</returns>
		/// <param name="attackData">Attack data.</param>
		virtual protected void ConsumeAmmo(BasicAttackData attackData)
		{
			if (attackData.attackType == AttackType.MELEE || attackData.ammoType == null || attackData.ammoType == "") return; 
			itemManager.ConsumeItem (attackData.ammoType, 1);
		}

		/// <summary>
		/// Starts the given attack.
		/// </summary>
		/// <param name="attackIndex">Attack index.</param>
		virtual protected void StartAttack(int attackIndex)
		{
			StopAllCoroutines();
			currentAttack = attackIndex;
			currentAttackTimer = 0.0f;
			if (attacks[attackIndex].attackType == AttackType.PROJECTILE)
			{
				StartCoroutine(DoProjectileAttack(attackIndex));
			}
			else
			{
				StartCoroutine(DoAttack(attackIndex));
			}
		}
		
		/// <summary>
		/// Do a melee attack.
		/// </summary>
		/// <param name="attackIndex">Attack index.</param>
		virtual protected IEnumerator DoAttack(int attackIndex)
		{
			if (attacks[currentAttack].resetVelocityX) character.SetVelocityX(0);
			if (attacks[currentAttack].resetVelocityY) character.SetVelocityY(0);

			if (attacks [attackIndex].hitBox == null)
			{
				Debug.LogWarning ("Trying to attack but no hit hitbox set");
			}
			else
			{
				attacks [attackIndex].hitBox.Enable (attacks [currentAttack].attackTime * attacks [currentAttack].attackHitBoxStart,
	                                   				 attacks [currentAttack].attackTime * attacks [currentAttack].attackHitBoxEnd);
				attacks [attackIndex].hitBox.UpdateDamageInfo(attacks[currentAttack].damageAmount, attacks[currentAttack].damageType);
			}
			while(currentAttackTimer < attacks[currentAttack].attackTime)
			{
				currentAttackTimer += TimeManager.FrameTime;
				yield return true;
			}
			// Reset any animation overrides before currentAttack is cleared
			if (!attackSystemWantsAnimationStateOverride && !attackSystemWantsMovementControl) character.RemoveAnimationOverride (OverrideState);

			// Attack finished
			currentAttack = -1;
			if (attacks [attackIndex].hitBox != null)
			{
				attacks[attackIndex].hitBox.ForceStop();
			}
			currentAttackTimer = 0.0f;
			character.OnChangeAnimationState ();
			character.AttackFinished();
			// Set cooldown
			if (cooldownTimers != null) cooldownTimers [attackIndex] = attacks [attackIndex].coolDown;
		}

		/// <summary>
		/// Do a projectile attack.
		/// </summary>
		/// <param name="attackIndex">Attack index.</param>
		virtual protected IEnumerator DoProjectileAttack(int attackIndex)
		{
			bool hasFired = false;
			while(currentAttackTimer < attacks[currentAttack].attackTime)
			{
				currentAttackTimer += TimeManager.FrameTime;
				if (!hasFired && currentAttackTimer >= attacks[currentAttack].projectileDelay) 
				{
					InstantiateProjectile(attackIndex);
					ConsumeAmmo (attacks [currentAttack]);
					hasFired = true;
				}
				yield return true;
			}
			// Reset any animation overrides before currentAttack is cleared
			if (!attackSystemWantsAnimationStateOverride && !attackSystemWantsMovementControl) character.RemoveAnimationOverride (OverrideState);

			// Attack finished
			currentAttack = -1;
			currentAttackTimer = 0.0f;
			character.OnChangeAnimationState ();
			character.AttackFinished();
		}

		/// <summary>
		/// Instantiates the deferred projectile.
		/// </summary>
		virtual public void InstantiateProjectile()
		{
			InstantiateProjectile (-1);
		}

		/// <summary>
		/// Instatiates a projectile.
		/// </summary>
		/// <param name="attackIndex">Index of the projectile to instantiate.</param>
		virtual public void InstantiateProjectile(int attackIndex)
		{
			// If attack index == -1 then we should use the deferred attack.
			if (attackIndex == -1) attackIndex = deferredAttackIndex;
			// Instantiate prefab
			GameObject go = (GameObject) GameObject.Instantiate(attacks[attackIndex].projectilePrefab);
			Projectile projectile = go.GetComponent<Projectile>();
			if (projectileAimer != null) 
			{
				go.transform.position = character.transform.position + (Vector3)projectileAimer.GetAimOffset(character);
			}
			else
			{
				go.transform.position = character.transform.position;
			}
			
			if (projectile != null) {
				// Fire projectile if the projectile is of type projectile
				Vector2 direction = new Vector2(character.LastFacedDirection != 0 ? character.LastFacedDirection : 1, 0);
				// Use aimer to get direction fo fire if the aimer is configured
				if (projectileAimer != null) direction = projectileAimer.GetAimDirection(character);
				projectile.Fire (attacks[attackIndex].damageAmount, attacks[attackIndex].damageType, direction, character);
			}
			// If the projectile is found and the go is still alive call finish
			if (projectile != null && go != null) projectile.Finish ();
		}

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		/// <value>The state of the animation.</value>
		override public AnimationState AnimationState
		{
			get 
			{
				if (currentAttack != -1) return attacks[currentAttack].animation;
				return AnimationState.NONE;
			}
		}

		/// <summary>
		/// Gets the priority of the animation state that this movement wants to set.
		/// </summary>
		override public int AnimationPriority
		{
			get 
			{
				if (currentAttack != -1) return 10;
				return 0;
			}
		}

		/// <summary>
		/// Gets the animation override state that this movement wants to set.
		/// </summary>
		override public string OverrideState
		{
			get 
			{
				// If we dont want control and we dont want state then set an override state
				if (!attackSystemWantsMovementControl && !attackSystemWantsAnimationStateOverride && this.AnimationState != AnimationState.NONE) return this.AnimationState.AsString();
				return null;
			}
		}

		/// <summary>
		/// If the attack is in progress force control.
		/// </summary>
		override public bool WantsControl()
		{
			if (currentAttack != -1 && attackSystemWantsMovementControl) return true;
			return false;
		}
		
		/// <summary>
		/// Gets a value indicating whether this <see cref="PlatformerPro.Movement"/> expects
		/// gravity to be applied after its movement finishes.
		/// </summary>
		override public bool ShouldApplyGravity
		{
			get
			{
				if (attackSystemWantsMovementControl && currentAttack != -1 ) return attacks[currentAttack].applyGravity;
				return true;
			}
		}

		/// <summary>
		/// Should we block jumping movement?
		/// </summary>
		/// <returns><c>true</c>, if jump should be blocked, <c>false</c> otherwise.</returns>
		virtual public bool BlockJump()
		{
			if (currentAttack == -1) return false;
			return attacks [currentAttack].blockJump;
		}
		
		/// <summary>
		/// Should we block wall movement?
		/// </summary>
		/// <returns><c>true</c>, if wall cling should be blocked, <c>false</c> otherwise.</returns>
		virtual public bool BlockWall()
		{
			if (currentAttack == -1) return false;
			return attacks [currentAttack].blockWall;
		}
		
		/// <summary>
		/// Should we block climb movement?
		/// </summary>
		/// <returns><c>true</c>, if climb should be blocked, <c>false</c> otherwise.</returns>
		virtual public bool BlockClimb()
		{
			if (currentAttack == -1) return false;
			return attacks [currentAttack].blockClimb;
		}

		/// <summary>
		/// Should we block climb movement?
		/// </summary>
		/// <returns><c>true</c>, if jump should be blocked, <c>false</c> otherwise.</returns>
		virtual public bool BlockSpecial()
		{
			if (currentAttack == -1) return false;
			return attacks [currentAttack].blockSpecial;
		}
	}

}