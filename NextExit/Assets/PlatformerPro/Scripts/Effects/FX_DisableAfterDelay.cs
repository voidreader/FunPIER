using UnityEngine;
using System.Collections;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// FX class which disables or deactivates things after a delay.
	/// </summary>

	public class  FX_DisableAfterDelay: FX_Base
	{
		/// <summary>
		/// How long to delay.
		/// </summary>
		[Tooltip ("How long to delay.")]
		public float delay;

		/// <summary>
		/// If true disable and deactive, if false enable and activate.
		/// </summary>
		[Tooltip ("If true disable and deactive, if false enable and activate.")]
		public bool disable;

		/// <summary>
		/// Array of GameObejcts to deactivate.
		/// </summary>
		[Tooltip ("GameObjects to activate/deactivate.")]
		public GameObject[] gameObjects;

		/// <summary>
		/// Components to disable.
		/// </summary>
		[Tooltip ("Components to enable/disable.")]
		public Behaviour[] components;


		/// <summary>
		/// Unity start hook.
		/// </summary>
		void Awake()
		{
			if (playOnAwake) DoEffect();
		}

		/// <summary>
		/// Coroutine which does delay.
		/// </summary>
		/// <returns>The coroutine.</returns>
		virtual protected IEnumerator EffectCoroutine()
		{
			yield return new WaitForSeconds (delay);
			for (int i = 0; i < gameObjects.Length; i++)
			{
				gameObjects[i].SetActive(!disable);
			}
			for (int i = 0; i < components.Length; i++)
			{
				components[i].enabled = !disable;
			}
		}

		/// <summary>
		/// Do the effect.
		/// </summary>
		override protected void DoEffect()
		{
			StartCoroutine (EffectCoroutine());
		}

	}
}