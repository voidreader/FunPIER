using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PlatformerPro
{

	/// <summary>
	/// Enemy AI which looks left and right until it sees a character at which point it charges at the character.
	/// </summary>
	public class EnemyAI_SeeAndCharge : EnemyAI
	{
		/// <summary>
		/// How far can the enemy see?
		/// </summary>
		[Tooltip ("How far can the enemy see.")]
		public float sightDistance;

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
		/// How frequently does the AI change the direciton it is looking in. 0 For not at all.
		/// </summary>
		[Tooltip ("How frequently does the AI change the direciton it is looking in. 0 For not at all.")]
		[Range (0,100)]
		public int changeSightDirection;

		/// <summary>
		/// How long to keep charging at player before going back to sight mode.
		/// </summary>
		[Tooltip ("How long to keep charging at player before going back to sight mode.")]
		public float chargeTime;

		/// <summary>
		/// Cached transform reference.
		/// </summary>
		protected Transform myTransform;

		/// <summary>
		/// Counts down from shootStateTime when the player is seen. When non zero the character will be in the shooting state.
		/// </summary>
		protected float chargeTimer;

		/// <summary>
		/// Store the offset to apply to the sight raycast.
		/// </summary>
		protected Vector3 offset;

		/// <summary>
		/// Unity Awake hook.
		/// </summary>
		void Awake()
		{
			offset = new Vector3 (0, yOffset, 0);
		}

		/// <summary>
		/// Unity update hook.
		/// </summary>
		void Update()
		{
			if (chargeTimer > 0.0f) chargeTimer -= TimeManager.FrameTime;
		}

		/// <summary>
		/// Init this enemy AI.
		/// </summary>
		/// <param name="enemy">Enemy we are the brain for.</param>
		override public void Init(Enemy enemy)
		{
			base.Init (enemy);
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
				if (hit.collider.gameObject.GetComponent(typeof(ICharacterReference)) != null || 
				    hit.collider.gameObject.GetComponent<Character>() != null)
				{
					chargeTimer = chargeTime;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Decide the next move
		/// </summary>
		override public EnemyState Decide()
		{
			int range = Random.Range (0, 100);
			if (chargeTimer > 0.0f) return EnemyState.CHARGING;
			if (range < changeSightDirection) enemy.SwitchDirection ();
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
				return new EnemyState[]{EnemyState.DEFAULT, EnemyState.CHARGING};
			}
		}
		
#endif

	}
}

