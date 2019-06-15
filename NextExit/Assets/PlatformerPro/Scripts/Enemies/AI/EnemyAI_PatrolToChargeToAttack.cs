using UnityEngine;
using System.Collections;

namespace PlatformerPro
{

	/// <summary>
	/// Anemy AI which moves the character from whatever default is set to charge to an attack state if the enemy comes within range.
	/// </summary>
	public class EnemyAI_PatrolToChargeToAttack : EnemyAI
	{

		/// <summary>
		/// Distance to check for enemy.
		/// </summary>
		public float chargeDistance;

		/// <summary>
		/// Distance to start attacking.
		/// </summary>
		public float attackDistance;

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
			RaycastHit2D hit = Physics2D.Raycast(myTransform.position, new Vector3(enemy.LastFacedDirection, 0, 0), chargeDistance, characterLayer);
			if (hit.collider != null)
			{
				CharacterHurtBox hurtBox = hit.collider.gameObject.GetComponent<CharacterHurtBox>();
				if (hurtBox != null)
				{
					// Work around becasue cahrge movement isn't great (to be updated soon)
					GetComponent<EnemyMovement_Charge>().SetDirection(new Vector2(-Mathf.Sign(hurtBox.transform.position.x - myTransform.position.x), 0));
					if (hit.distance <= attackDistance) return EnemyState.ATTACKING;
					return EnemyState.CHARGING;
				}
			}
			return EnemyState.PATROL;
		}

		
#if UNITY_EDITOR
		
		/// <summary>
		/// Static info used by the editor.
		/// </summary>
		override public EnemyState[] Info
		{
			get
			{
				return new EnemyState[]{EnemyState.PATROL, EnemyState.CHARGING, EnemyState.ATTACKING};
			}
		}
		
#endif
	}
}

