using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Scales an image bar based on the percentage of health (current/max).
	/// </summary>
	public class UICharge_PercentageBar : MonoBehaviour
	{
		// MULTIPLAYER TODO

		/// <summary>
		/// Name of the attack to check.
		/// </summary>
		public string attackType;

		/// <summary>
		/// Link to attack movement to use. If empty one will be searched for.
		/// </summary>
		public BasicAttacks attackMovement;

		/// <summary>
		/// Should we use image fill?
		/// </summary>
		public bool useFill = true;

		public float barMax = 1.0f;

		/// <summary>
		/// Disable visible content when not charging.
		/// </summary>
		public bool disableWhenNotCharging;

		/// <summary>
		/// Cached index of the attack we are listening to.
		/// </summary>
		protected int attackIndex = -1;

		/// <summary>
		/// The bar image.
		/// </summary>
		public Image barImage;

		/// <summary>
		/// Visible content.
		/// </summary>
		public GameObject visibleContent;

		/// <summary>
		/// Reference to the character loader.
		/// </summary>
		protected PlatformerProGameManager characterLoader;

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start()
		{
			Init ();
		}

		/// <summary>
		/// Unity update hook.
		/// </summary>
		void Update()
		{
			if (attackMovement != null && attackIndex != -1) UpdateImage ();
		}

		/// <summary>
		/// Do the destroy actions.
		/// </summary>
		void OnDestroy()
		{
			if (characterLoader != null)
			{
				characterLoader.CharacterLoaded -= HandleCharacterLoaded;
			}
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void Init()
		{
			if (attackMovement == null)
			{
				// No health assigned try to find one
				characterLoader = PlatformerProGameManager.Instance;
				characterLoader.CharacterLoaded += HandleCharacterLoaded;
				return;
			}
			attackIndex = attackMovement.GetIndexForName (attackType);
			if (attackIndex == -1)
			{
				Debug.LogWarning ("Charge UI couldn't find an attack with name: " + name);
				Destroy (this);
			}
		}


		/// <summary>
		/// Handles the character being loaded.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event args.</param>
		virtual protected void HandleCharacterLoaded (object sender, CharacterEventArgs e)
		{
			attackMovement = e.Character.GetComponentInChildren<BasicAttacks>();
			if (attackMovement == null)
			{
				Debug.LogWarning ("Charge UI couldn't find an attack movement.");
				Destroy (this);
				return;
			}
			attackIndex = attackMovement.GetIndexForName (attackType);
			if (attackIndex == -1)
			{
				Debug.LogWarning ("Charge UI couldn't find an attack with name: " + name);
				Destroy (this);
			}
		}

		/// <summary>
		/// Updates the bar image.
		/// </summary>
		virtual protected void UpdateImage()
		{
			float charge = attackMovement.GetRawChargeForAttack (attackIndex);
			if (charge <= 0 && disableWhenNotCharging)
			{
				visibleContent.SetActive (false);
				return;
			}
			visibleContent.SetActive (true);
			float fill = charge / barMax;
			if (fill < 0) fill = 0;
			if (fill > 1.0f) fill = 1.0f;
			if (useFill)
				barImage.fillAmount = fill;
			else
				barImage.rectTransform.sizeDelta = new Vector2(100.0f * fill, barImage.rectTransform.sizeDelta.y);
		}
	}
}