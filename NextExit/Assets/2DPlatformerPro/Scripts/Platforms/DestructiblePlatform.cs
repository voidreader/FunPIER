using UnityEngine;
using System.Collections;

namespace PlatformerPro
{

	/// <summary>
	/// A platform that can be destoryed via damage.
	/// </summary>
	[RequireComponent (typeof (Collider2D))]
	public class DestructiblePlatform : Platform, IHurtable
	{
		/// <summary>
		/// How much damage before platform is destroyed.
		/// </summary>
		[Range (1,99)]
		public int health = 1;

		/// <summary>
		/// How long to wait before destroying object completely.
		/// </summary>
		[Tooltip ("How long to wait before destroying object completely.")]
		public float destroyDelay;

		/// <summary>
		/// Should we also send a damage event when destroyed.
		/// </summary>
		[Tooltip ("Should we also send a damage event when destroyed.")]
		public bool sendDamageOnDestroy;

		/// <summary>
		/// Cached copy of damage event args to save on allocations.
		/// </summary>
		protected DamageInfoEventArgs damageEventArgs;

		#region events

		/// <summary>
		/// Event for damage.
		/// </summary>
		public event System.EventHandler <DamageInfoEventArgs> Damaged;
		
		/// <summary>
		/// Event for destory.
		/// </summary>
		public event System.EventHandler <DamageInfoEventArgs> Destroyed;

		/// <summary>
		/// Raises the damaged event.
		/// </summary>
		/// <param name="info">Info.</param>
		virtual protected void OnDamaged(DamageInfo info)
		{
			if (Damaged != null)
			{
				damageEventArgs.UpdateDamageInfoEventArgs(info);
				Damaged(this, damageEventArgs);
			}
		}


		/// <summary>
		/// Raises the Destroyed event.
		/// </summary>
		/// <param name="info">Info.</param>
		virtual protected void OnDestroyed(DamageInfo info)
		{
			if (Destroyed != null)
			{
				damageEventArgs.UpdateDamageInfoEventArgs(info);
				Destroyed(this, damageEventArgs);
			}
		}
		#endregion

		#region Unity hoooks

		/// <summary>
		/// Unity start hook.
		/// </summary>
		void Start()
		{
			Init ();
		}

		#endregion

		/// <summary>
		/// Deal damage to the hurtable.
		/// </summary>
		/// <param name="info">Info.</param>
		virtual public void Damage(DamageInfo info)
		{
			health -= info.Amount;
			if (health <= 0)
			{
				DoDestroy (info);
			}
			else
			{
				DoDamage (info);
			}
		}
		
		/// <summary>
		/// Get the mobile (charater or enemy) that this hurt box belongs too. In this case null.
		/// </summary>
		virtual public IMob Mob
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Set up this instance.
		/// </summary>
		override protected void Init()
		{
			base.Init ();
			damageEventArgs = new DamageInfoEventArgs();
		}

		/// <summary>
		/// A simple destroy implementation.
		/// </summary>
		virtual protected void DoDestroy(DamageInfo info)
		{
			GetComponent<Collider2D>().enabled = false;
			if (sendDamageOnDestroy) OnDamaged(info);
			OnDestroyed (info);
			StartCoroutine (DestroyAfterDelay (info));
		}

		/// <summary>
		/// A simple damage implementation.
		/// </summary>
		virtual protected void DoDamage(DamageInfo info)
		{
			OnDamaged(info);
		}

		/// <summary>
		/// Coroutine to destory this object. You could override if you didn't want to destory
		/// for example if you have a pooling system.
		/// </summary>
		/// <param name="info">Damage info.</param>
		virtual protected IEnumerator DestroyAfterDelay(DamageInfo info)
		{
			yield return new WaitForSeconds (destroyDelay);
			GameObject.Destroy (gameObject);
		}
	}

}
