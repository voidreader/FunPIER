using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Projectile attacks that luanch more than one projectile.
	/// </summary>
	public class MultiProjectileAttacks : BasicAttacks
	{
	
		/// <summary>
		/// The projectile data
		/// </summary>
		public List<MultiProjectileAttackData> projectileData;

		#region movement info constants and properties
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Multi Projectile Attacks";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Projectile attacks that luanch more than one projectile.";
		
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

		#region public methods

		/// <summary>
		/// Init this instance.
		/// </summary>
		override protected void PostInit()
		{
			foreach (MultiProjectileAttackData attack in projectileData)
			{
				attacks.Add(attack);
			}
			base.PostInit ();
		}

		#endregion

		#region protected methods

		
		/// <summary>
		/// Starts the given attack.
		/// </summary>
		/// <param name="attackIndex">Attack index.</param>
		override protected void StartAttack(int attackIndex)
		{
			StopAllCoroutines();
			currentAttack = attackIndex;
			currentAttackTimer = 0.0f;

			StartCoroutine(DoProjectileAttack(attackIndex));

		}

		/// <summary>
		/// Do a projectile attack.
		/// </summary>
		/// <param name="attackIndex">Attack index.</param>
		override protected IEnumerator DoProjectileAttack(int attackIndex)
		{
			foreach (MultiProjectileData data in projectileData[attackIndex].projectileData)
			{
				StartCoroutine(FireSingleProjectile(projectileData[attackIndex], data));
			}
			while(currentAttackTimer < attacks[currentAttack].attackTime)
			{
				currentAttackTimer += TimeManager.FrameTime;
				yield return true;
			}
			// Reset any animation overrides before currentAttack is cleared
			character.RemoveAnimationOverride (OverrideState);

			// Attack finished
			string attackName = attacks[attackIndex].name;
			currentAttack = -1;
			currentAttackTimer = 0.0f;
			character.OnChangeAnimationState ();
			character.FinishedAttack(attackName);

			// Set cooldown
			if (cooldownTimers != null) cooldownTimers [attackIndex] = attacks [attackIndex].coolDown;
		}
	
		/// <summary>
		/// Fires a single projectile.
		/// </summary>
		/// <param name="data">Data about projectile.</param>
		virtual protected IEnumerator FireSingleProjectile(MultiProjectileAttackData attack, MultiProjectileData data)
		{
			int currentAttackTmp = currentAttack;
			yield return new WaitForSeconds (data.delay);
			GameObject go = (GameObject) GameObject.Instantiate(data.projectilePrefab);
			Projectile projectile = go.GetComponent<Projectile>();
			if (projectileAimer != null) 
			{
				go.transform.position = character.transform.position + (Vector3)projectileAimer.GetAimOffset(character) + new Vector3(data.positionOffset.x * character.LastFacedDirection, data.positionOffset.y, 0);
			}
			else
			{
				go.transform.position = character.transform.position +  new Vector3(data.positionOffset.x * character.LastFacedDirection, data.positionOffset.y, 0);
			}
			
			if (projectile != null) {
				// Fire projectile if the projectile is of type projectile
				Vector2 direction = new Vector2(character.LastFacedDirection != 0 ? character.LastFacedDirection : 1, 0);
				// Use aimer to get direction fo fire if the aimer is configured
				if (projectileAimer != null) direction = projectileAimer.GetAimDirection(character);
				direction = Quaternion.Euler(0,0, data.angleOffset * character.LastFacedDirection)  * direction;

				if (data.flipX) direction.x *= -1;
				if (data.flipY) direction.y *= -1;
				projectile.Fire (attack.damageAmount, attack.damageType, direction, character, Charge);
			}

			// If the projectile is found and the go is still alive call finish
			if (projectile != null && go != null) projectile.Finish ();
			ConsumeAmmo (attacks [currentAttackTmp]);
		}

			/// <summary>
			/// Instatiates a projectile.
			/// </summary>
			/// <param name="attackIndex">Index of the projectile to instantiate.</param>
		override public void InstantiateProjectile(int attackIndex)
		{
				Debug.Log ("Multi-projectile attack does not support InstantiateProjectile() being called directly");
		}

		#endregion
	}
}