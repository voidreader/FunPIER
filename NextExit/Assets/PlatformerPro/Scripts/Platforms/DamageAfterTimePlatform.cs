using UnityEngine;
using System.Collections;

namespace PlatformerPro
{

	/// <summary>
	/// A platform which applies damage after a time.
	/// </summary>
	[ExecuteInEditMode]
	public class DamageAfterTimePlatform : ParentOnStandPlatform
	{
		/// <summary>
		/// Amount of damage to apply.
		/// </summary>
		public int damageAmount = 1;

		/// <summary>
		/// Type of damage to apply.
		/// </summary>
		public DamageType damageType;

		/// <summary>
		/// Time delay before damage is caused.
		/// </summary>
		public float damageTime = 1.0f;

		/// <summary>
		/// Unity enable hook.
		/// </summary>
		void OnEnable()
		{
			automaticActivation = PlatformActivationType.ACTIVATE_ON_STAND;
			automaticDeactivation = PlatformDeactivationType.DEACTIVATE_ON_LEAVE;
		}

		/// <summary>
		/// Do the moving. Required by parent on stand.
		/// </summary>
		protected override void DoMove (){}

		/// <summary>
		/// Called when the character is parented to this platform.
		/// </summary>
		override public void Parent(IMob character)
		{
			base.Parent (character);
			StartCoroutine (DamageCharacter (character));
		}

		/// <summary>
		/// Called when the character is unparented from this platform.
		/// </summary>
		override public void UnParent(IMob character)
		{
			base.UnParent (character);
			StopAllCoroutines ();
		}

		virtual protected IEnumerator DamageCharacter(IMob character)
		{
			float timer = damageTime;
			CharacterHealth health = null;
			if (character is Character)
			{
				health = ((Character)character).GetComponentInChildren<CharacterHealth> ();
			}
			if (health == null && (!(character is Enemy))) 
			{
				Debug.LogWarning("DamageAfterTimePlatform couldn't find an Enemy or CharacterHealth and thus cannot damage Character");
			}
			else
			{
				while (timer > 0)
				{
					yield return true;
					timer -= TimeManager.FrameTime;
				}
				if (character is Enemy)
				{
					((Enemy)character).Damage(new DamageInfo(damageAmount, damageType, new Vector2(0, -1)));
				}
				else
				{
					health.Damage(new DamageInfo(damageAmount, damageType, new Vector2(0, -1)));
				}
			}
		}
	}
}
