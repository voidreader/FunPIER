using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PlatformerPro
{

	/// <summary>
	/// Enemy AI which ocassionally runs back and forward within a designated area, and if it sees the player shoots at it. 
	/// If the player shoots it it hides (potentially becoming invulnerable to damage).
	/// </summary>
	public class EnemyAI_SeeAndShootWithHide : EnemyAI_SeeAndShoot
	{

		/// <summary>
		/// How long after being damaged does the enemy hide for.
		/// </summary>
		[Tooltip ("How long after being damaged the enemy hides for.")]
		public float hideStateTime = 5.0f;

		/// <summary>
		/// Counts down from hideStateTime when the enemy hides.
		/// </summary>
		protected float hideStateTimer;

		/// <summary>
		/// Unity update hook.
		/// </summary>
		void Update()
		{
			if (hideStateTimer > 0.0f) hideStateTimer -= TimeManager.FrameTime;
			if (shootStateTimer > 0.0f) shootStateTimer -= TimeManager.FrameTime;
		}

		/// <summary>
		/// Unity disable hook.
		/// </summary>
		void OnDisable()
		{
			enemy.Damaged -= EnemyDamaged;
		}

		/// <summary>
		/// Init this enemy AI.
		/// </summary>
		override public void Init(Enemy enemy)
		{
			base.Init (enemy);
			// Listen to damage events and if we get them prolong our hiding
			enemy.Damaged += EnemyDamaged;
		}

		/// <summary>
		/// The sense routine used to detect when something changes. In this
		/// case cast a ray from the transform in the facing direction to look for the player.
		/// </summary>
		override public bool Sense()
		{
			if (hideStateTimer <= 0.0f)
			{
				RaycastHit2D hit = Physics2D.Raycast(myTransform.position + offset, new Vector3(enemy.LastFacedDirection, 0, 0), sightDistance, sightLayers);
				if (hit.collider != null)
				{
					Character character = null;
					ICharacterReference characterRef = (ICharacterReference) hit.collider.gameObject.GetComponent(typeof(ICharacterReference));
					if (characterRef == null)
					{
						character = hit.collider.gameObject.GetComponent<Character>();
					} 
					else
					{
						character = characterRef.Character;
					}
					if (character != null)
					{
						shootStateTimer = shootStateTime;
						CurrentTarget = character;
						return true;
					}
				}
			}
			CurrentTarget = null;
			return false;
		}

		/// <summary>
		/// Decide the next move.
		/// </summary>
		override public EnemyState Decide()
		{
			int range = Random.Range (0, 100);
			if (hideStateTimer > 0.0f) return EnemyState.HIDING;
			if (shootStateTimer > 0.0f) return EnemyState.SHOOTING;
			if (range < activityRate) return EnemyState.PATROL;
			return EnemyState.DEFAULT;
		}

		/// <summary>
		/// Used to inform the AI we were damaged so an action like FLEE may be triggered.
		/// </summary>
		override public void Damaged()
		{
			hideStateTimer = hideStateTime;
		}

		/// <summary>
		/// When enemy is damaged keep hiding!
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		virtual protected void EnemyDamaged( object sender, DamageInfoEventArgs args)
		{
			hideStateTimer = hideStateTime;
		}

#if UNITY_EDITOR

		/// <summary>
		/// Static info used by the editor.
		/// </summary>
		override public EnemyState[] Info
		{
			get
			{
				return new EnemyState[]{EnemyState.DEFAULT, EnemyState.PATROL, EnemyState.SHOOTING, EnemyState.HIDING};
			}
		}

#endif

	}
}

