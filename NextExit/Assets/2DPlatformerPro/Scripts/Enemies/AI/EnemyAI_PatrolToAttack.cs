using UnityEngine;
using System.Collections;

namespace PlatformerPro
{

	/// <summary>
	/// Anemy AI which moves the character from whatever default is set to an attack state if the enemy comes within range.
	/// </summary>
	public class EnemyAI_PatrolToAttack : EnemyAI
	{

		/// <summary>
		/// Distance to check for enemy.
		/// </summary>
		public float enemyDistance;

		/// <summary>
		/// Layer to check for characters on.
		/// </summary>
		public LayerMask characterLayer;

		/// <summary>
		/// Cached transform reference.
		/// </summary>
		protected Transform myTransform;

		override public void Init(Enemy enemy)
		{
			base.Init (enemy);
			myTransform = transform;
		}

		/// <summary>
		/// Decide the next move
		/// </summary>
		override public EnemyState Decide()
		{
			RaycastHit2D hit = Physics2D.Raycast(myTransform.position, new Vector3(enemy.LastFacedDirection, 0, 0), enemyDistance, characterLayer);
			if (hit.collider != null)
			{
				CharacterHurtBox hurtBox = hit.collider.gameObject.GetComponent<CharacterHurtBox>();
				if (hurtBox != null)
				{
					return EnemyState.ATTACKING;
				}
			}
			return EnemyState.PATROL;
		}

	}
}

