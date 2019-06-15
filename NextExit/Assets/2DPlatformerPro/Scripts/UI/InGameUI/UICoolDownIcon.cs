using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// A simple UI for showing a cool down.
	/// </summary>
	public class UICoolDownIcon : MonoBehaviour
	{
		/// <summary>
		/// Shows the radial fill progress.
		/// </summary>
		public Image fillImage;

		/// <summary>
		/// Shows an icon when ability ready.
		/// </summary>
		public Image mainImage;

		/// <summary>
		/// The attack movement.
		/// </summary>
		public BasicAttacks attackMovement;

		/// <summary>
		/// Index of attack in attack movement.
		/// </summary>
		public int attackIndex;

		/// <summary>
		/// Color to use when ability not available.
		/// </summary>
		protected static Color clearColor = new Color (1, 1, 1, 0.5f);

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start()
		{
			UpdateView ();
		}

		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update()
		{
			UpdateView ();
		}

		/// <summary>
		/// updates the view to show current state.
		/// </summary>
		virtual protected void UpdateView()
		{
			float remaining = attackMovement.GetAttackCoolDownRemaining (attackIndex);
			float total = attackMovement.GetAttackCoolDown (attackIndex);
			if (remaining == 0)
			{
				mainImage.color = Color.white;
				fillImage.fillAmount = 1.0f;
			}
			else
			{
				float percentage = (total - remaining) / total;
				mainImage.color = clearColor;
				fillImage.fillAmount = percentage;
			}
		}

	}
}
