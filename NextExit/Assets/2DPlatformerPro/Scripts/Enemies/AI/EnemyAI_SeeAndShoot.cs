using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PlatformerPro
{

	/// <summary>
	/// Enemy AI which ocassionally runs back and forward within a designated area, and if it sees the player shoots at it.
	/// </summary>
	public class EnemyAI_SeeAndShoot : EnemyAI
	{

		/// <summary>
		/// How far can the enemy see?
		/// </summary>
		[Tooltip ("How far can the enemy see.")]
		public float sightDistance = 5.0f;

		/// <summary>
		/// Y position of the characters 'eyes'.
		/// </summary>
		[Tooltip ("Y position of the characters 'eyes'.")]
		public float yOffset;

		/// <summary>
		/// Layers to check for obstacle and characters.
		/// </summary>
		[Tooltip ("Layers to check for obstacle and characters. The enemy will be be able to 'see through' anything not in this layer mask.")]
		public LayerMask sightLayers;

		/// <summary>
		/// How long to keep shooting at player before going back to the default state.
		/// </summary>
		[Tooltip ("How long after seeing the player will the character keep shooting.")]
		public float shootStateTime;

		/// <summary>
		/// How often will the character walk about from 0 - never, to 1 - always.
		/// </summary>
		[Range (0, 100)]
		[Tooltip ("How often will the character walk about from 0 - never, to 100 - always.")]
		public int activityRate;

		/// <summary>
		/// Cached transform reference.
		/// </summary>
		protected Transform myTransform;

		/// <summary>
		/// Counts down from shootStateTime when the player is seen. When non zero the character will be in the shooting state.
		/// </summary>
		protected float shootStateTimer;

		/// <summary>
		/// Store the offset to apply to the sight raycast.
		/// </summary>
		protected Vector3 offset;

		/// <summary>
		/// Unity update hook.
		/// </summary>
		void Update()
		{
			if (shootStateTimer > 0.0f) shootStateTimer -= TimeManager.FrameTime;
		}

		/// <summary>
		/// Init this enemy AI.
		/// </summary>
		override public void Init(Enemy enemy)
		{
			base.Init (enemy);
			offset = new Vector3 (0, yOffset, 0);
			myTransform = transform;
		}

		/// <summary>
		/// The sense routine used to detect when something changes. In this
		/// case cast a ray from the transform in the facing direction to look for the player.
		/// </summary>
		override public bool Sense()
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
			CurrentTarget = null;
			return false;
		}

		/// <summary>
		/// Decide the next move
		/// </summary>
		override public EnemyState Decide()
		{
			int range = Random.Range (0, 100);
			if (shootStateTimer > 0.0f) return EnemyState.SHOOTING;
			if ( range < activityRate) return EnemyState.PATROL;
			return EnemyState.DEFAULT;
		}

#if UNITY_EDITOR

		/// <summary>
		/// Draw gizmos for showing sight range.
		/// </summary>
		virtual public void OnDrawGizmos()
		{
			offset = new Vector3 (0, yOffset, 0);
			float arrowOffset = 0.25f;
			float actualSightDistance = sightDistance;
			if (gameObject.GetComponentInParent<Enemy>().FacingDirection < 0)
			{
				arrowOffset  *= -1;
				actualSightDistance *= -1;
			}
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position + offset,  transform.position + offset + new Vector3(actualSightDistance, 0, 0));
			Gizmos.DrawLine(transform.position + offset + new Vector3(actualSightDistance, 0.0f, 0), transform.position + offset + new Vector3(actualSightDistance - arrowOffset, 0.25f, 0));
			Gizmos.DrawLine(transform.position + offset + new Vector3(actualSightDistance, 0.0f, 0), transform.position + offset + new Vector3(actualSightDistance  - arrowOffset, -0.25f, 0));
		}

		/// <summary>
		/// Static info used by the editor.
		/// </summary>
		override public EnemyState[] Info
		{
			get
			{
				return new EnemyState[]{EnemyState.DEFAULT, EnemyState.PATROL, EnemyState.SHOOTING};
			}
		}

#endif

	}
}

