using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Attacks which can only be triggered after another attack (optionally only after another attack hits).
	public class ComboAttacks : BasicAttacks
	{
		/// <summary>
		/// The attack data.
		/// </summary>
		public List<ComboAttackData> comboAttacks;

		/// <summary>
		/// The queued attack.
		/// </summary>
		protected int queuedAttack = -1;

		/// <summary>
		/// Init this instance.
		/// </summary>
		override protected void PostInit()
		{
			foreach (ComboAttackData attack in comboAttacks)
			{
				attacks.Add(attack);
			}
			base.PostInit();
		}


		/// <summary>
		/// Gets a value indicating whether this movement wants to initiate an attack.
		/// </summary>
		/// <value><c>true</c> if this instance should attack; otherwise, <c>false</c>.</value>
		override public bool WantsAttack()
		{
			// Can't attack if disabled
			if (!enabled) return false;
			// Play the queued attack as soon as other attack finishes
			if (queuedAttack > -1 && character.AttackName == null) return true;
			// Check each attack
			for (int i = 0; i < comboAttacks.Count; i ++)
			{
				// Not cooled down
				if (cooldownTimers != null && cooldownTimers.Length > 0 && cooldownTimers[i] > 0) break;
				if (CheckLocation(comboAttacks[i]) && CheckInput(comboAttacks[i]) && CheckInput(comboAttacks[i]) && CheckComboConditions(comboAttacks[i], i))
				{
					return true;
				}
				
			}
			return false;
		}

		/// <summary>
		/// Do the next attack.
		/// </summary>
		/// <returns>true if this movement wants to main control of movement, false otherwise</returns>
		override public bool Attack()
		{
			if (queuedAttack == -1) return false;
			StartAttack(queuedAttack);
			queuedAttack = -1;
			return attackSystemWantsMovementControl;
		}

		/// <summary>
		/// Checks if the combo conditions are met.
		/// </summary>
		/// <returns><c>true</c>, if combo conditions were met, <c>false</c> otherwise.</returns>
		/// <param name="attack">Attack data.</param>
		virtual public bool CheckComboConditions(ComboAttackData attack, int index)
		{
			// Queue empty
			if (queuedAttack != -1) return false;
			// Check initial attack
			if (!(attack.initialAttack == null || attack.initialAttack == "" || character.AttackName == attack.initialAttack )) return false;
			// Check timing window
			if (character.AttackNormalisedTime < attack.minWindowTime || character.AttackNormalisedTime > attack.maxWindowTime) return false;
			switch (attack.comboType)
			{
				case ComboType.QUEUED:
					queuedAttack = index;
					break;
				case ComboType.CANCEL:
					if (character.ActiveMovement is BasicAttacks && character.ActiveMovement != this) {
						((BasicAttacks)character.ActiveMovement).InterruptAttack ();
					} else {
						InterruptAttack ();
					}
					queuedAttack = index;
					return true;				
				case ComboType.POST_HIT:
					if (!character.AttackHasHit) return false;
					queuedAttack = index;
					break;
			}
			return false;
		}
	}
}
