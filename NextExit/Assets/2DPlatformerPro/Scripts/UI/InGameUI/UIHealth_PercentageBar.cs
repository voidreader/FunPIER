using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Scales an image bar based on the percentage of health (current/max).
	/// </summary>
	[RequireComponent (typeof(Image))]
	public class UIHealth_PercentageBar : MonoBehaviour
	{

		/// <summary>
		/// The item manager.
		/// </summary>
		public CharacterHealth characterHealth;

		/// <summary>
		/// The bar image.
		/// </summary>
		protected Image barImage;
		
		/// <summary>
		/// Reference to the character loader.
		/// </summary>
		protected CharacterLoader characterLoader;

		void Start()
		{
			Init ();
		}

		void Update()
		{
			if (characterHealth != null) UpdateImage ();
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

		virtual protected void Init()
		{
			barImage = GetComponent<Image> ();
			if (characterHealth == null) 
			{
				// No health assigned try to find one
				if (characterLoader == null) characterLoader = CharacterLoader.GetCharacterLoader();
				if (characterLoader != null)
				{
					characterLoader.CharacterLoaded += HandleCharacterLoaded;
				}
				else 
				{
					characterHealth = GameObject.FindObjectOfType<CharacterHealth> ();
					if (characterHealth == null) Debug.LogWarning ("Couldn't find a character health!");
				}
			}
		}

		/// <summary>
		/// Handles the character being loaded.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event args.</param>
		virtual protected void HandleCharacterLoaded (object sender, CharacterEventArgs e)
		{
			characterHealth = e.Character.GetComponent<CharacterHealth>();
			if (characterHealth == null) Debug.LogWarning ("The loaded character doesn't have a character health!");
		}

		virtual protected void UpdateImage()
		{
			barImage.rectTransform.sizeDelta = new Vector2(100.0f * characterHealth.CurrentHealthAsPercentage, barImage.rectTransform.sizeDelta.y);
		}
	}
}