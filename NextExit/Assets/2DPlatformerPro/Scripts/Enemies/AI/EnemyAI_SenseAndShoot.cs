using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PlatformerPro
{

	/// <summary>
	/// Extension to see and shoot which also has a proximity like sense (for example could be hearing).
	/// </summary>
	public class EnemyAI_SenseAndShoot : EnemyAI_SeeAndShoot
	{

		/// <summary>
		/// Range of the proximity sense.
		/// </summary>
		[Tooltip ("Range of the proximity sense.")]
		public float senseRadius = 1.0f;

		/// <summary>
		/// Centre point of the proximity sense.
		/// </summary>
		[Tooltip ("Centre point of the proximity sense.")]
		public Vector2 senseOffset;

		/// <summary>
		/// Layers to check for characters with proximity sense.
		/// </summary>
		[Tooltip ("Layers to check for character via proximity senses.")]
		public LayerMask senseLayers;

		/// <summary>
		/// Used to store results of overlap call.
		/// </summary>
		protected Collider2D[] proximityColliders;

		/// <summary>
		/// The max colliders for statically allocated colliders.
		/// </summary>
		private const int MaxColliders = 8;

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
			proximityColliders = new Collider2D[MaxColliders];
			if (sightDistance <= 0.0f && senseRadius <= 0.0f) Debug.LogError("Both sense ranges are zero or smaller, enemy will not be able to sense anything.");
		}

		/// <summary>
		/// The sense routine used to detect when something changes. In this
		/// case cast a ray from the transform in the facing direction to look for the player. And cast a 
		/// circle from the circle offset to "sense" the player.
		/// </summary>
		override public bool Sense()
		{
			Character character = null;
			ICharacterReference characterRef = null;
			if (sightDistance > 0.0f)
			{
				RaycastHit2D hit = Physics2D.Raycast(myTransform.position + offset, new Vector3(enemy.LastFacedDirection, 0, 0), sightDistance, sightLayers);
				if (hit.collider != null)
				{
					characterRef = (ICharacterReference) hit.collider.gameObject.GetComponent(typeof(ICharacterReference));
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
			if (senseRadius > 0.0f)
			{
				Vector2 offset = (Vector2)transform.position + (enemy.FacingDirection == -1 ? new Vector2(-senseOffset.x, senseOffset.y) : senseOffset);
				int hits = Physics2D.OverlapCircleNonAlloc (offset, senseRadius, proximityColliders, senseLayers);
				if (hits > 0)
				{
					// Always pick first target in list
					characterRef = (ICharacterReference) proximityColliders[0].gameObject.GetComponent(typeof(ICharacterReference));
					if (characterRef == null)
					{
						character = proximityColliders[0].gameObject.GetComponent<Character>();
					} 
					else
					{
						character = characterRef.Character;
					}
					CurrentTarget = character;
					shootStateTimer = shootStateTime;
					return true;
				}
			}
			CurrentTarget = null;
			return false;
		}

#if UNITY_EDITOR

		/// <summary>
		/// Draw gizmos for showing sight range.
		/// </summary>
		override public void OnDrawGizmos()
		{
			offset = new Vector3 (0, yOffset, 0);
			float arrowOffset = 0.25f;
			float actualSightDistance = sightDistance;
			int offsetModifier = 1;
			if (gameObject.GetComponentInParent<Enemy>().FacingDirection < 0)
			{
				arrowOffset  *= -1;
				actualSightDistance *= -1;
				offsetModifier = -1;
			}
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position + offset,  transform.position + offset + new Vector3(actualSightDistance, 0, 0));
			Gizmos.DrawLine(transform.position + offset + new Vector3(actualSightDistance, 0.0f, 0), transform.position + offset + new Vector3(actualSightDistance - arrowOffset, 0.25f, 0));
			Gizmos.DrawLine(transform.position + offset + new Vector3(actualSightDistance, 0.0f, 0), transform.position + offset + new Vector3(actualSightDistance  - arrowOffset, -0.25f, 0));
			Handles.color = new Color (1, 0, 0, 0.25f);
			Handles.DrawSolidDisc (transform.position + new Vector3 (senseOffset.x * offsetModifier, senseOffset.y, 0), new Vector3 (0, 0, 1), senseRadius);

	      	Gizmos.color = Color.white;
	     	Handles.color = Color.white;
		}

#endif

	}
}

