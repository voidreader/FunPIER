using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Scales an image bar based on the percentage of breath (current/max).
	/// </summary>
	[RequireComponent (typeof(Image))]
	public class UIBreath_PercentageBar : MonoBehaviour
	{

        /// <summary>
        /// Id of the player this breath is for.
        /// </summary>
        public int playerId = -1;

        /// <summary>
        /// The item manager.
        /// </summary>
        public Breath breath;

		/// <summary>
		/// Should we use image fill?
		/// </summary>
		public bool useFill = true;

        /// <summary>
        /// If true then fill vaue/size value 1 == empty.
        /// </summary>
        public bool invert;

		/// <summary>
		/// The bar image.
		/// </summary>
		protected Image barImage;
		
		/// <summary>
		/// Reference to the character loader.
		/// </summary>
		protected PlatformerProGameManager characterLoader;

		void Start()
		{
			Init ();
		}

		void Update()
		{
			if (breath != null) UpdateImage ();
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
			if (breath == null) 
			{
				characterLoader = PlatformerProGameManager.Instance;
				characterLoader.CharacterLoaded += HandleCharacterLoaded;
			}
		}

		/// <summary>
		/// Handles the character being loaded.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event args.</param>
		virtual protected void HandleCharacterLoaded (object sender, CharacterEventArgs e)
		{
            if (playerId == -1 || e.PlayerId == playerId)
            {
                breath = e.Character.GetComponentInChildren<Breath>();
                if (breath == null) Debug.LogWarning("The loaded character doesn't have a Breath component.");
            }
		}

		virtual protected void UpdateImage()
		{
			if (useFill)
				barImage.fillAmount = invert ? (1.0f - breath.CurrentBreathAsPercentage) : (breath.CurrentBreathAsPercentage);
			else
				barImage.rectTransform.sizeDelta = new Vector2(100.0f * breath.CurrentBreathAsPercentage, barImage.rectTransform.sizeDelta.y);
		}
	}
}