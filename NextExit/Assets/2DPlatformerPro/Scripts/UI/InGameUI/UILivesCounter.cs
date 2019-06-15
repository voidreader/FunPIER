using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Shows count of a stackable item.
	/// </summary>
	[RequireComponent (typeof(Text))]
	public class UILivesCounter : MonoBehaviour {

		/// <summary>
		/// The item manager.
		/// </summary>
		public CharacterHealth characterHealth;

		/// <summary>
		/// Reference to the character loader.
		/// </summary>
		protected CharacterLoader characterLoader;

		/// <summary>
		/// Format string used to display the value.
		/// </summary>
		[Tooltip ("Format string used to display the value.")]
		public string formatString = "";

		/// <summary>
		/// The text box.
		/// </summary>
		protected Text counterText;

		/// <summary>
		/// Unity Start() hook.
		/// </summary>
		void Start()
		{
			Init ();
			UpdateText ();
		}

		/// <summary>
		/// Unity On Destory hook
		/// </summary>
		void OnDestory()
		{
			if (characterLoader != null) characterLoader.CharacterLoaded += HandleCharacterLoaded;
			if (characterHealth != null) 
			{
				characterHealth.Died -= HandleChange;
				characterHealth.Loaded -= HandleChange;
				characterHealth.GainLives -= HandleChange;
			}
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void Init()
		{
			counterText = GetComponent<Text> ();
			if (characterHealth == null) 
			{
				characterHealth = FindObjectOfType<CharacterHealth>();
				if (characterHealth == null)
				{
					characterLoader = CharacterLoader.GetCharacterLoader();
					if (characterLoader != null) characterLoader.CharacterLoaded += HandleCharacterLoaded;
					else Debug.LogError("Couldn't find a character health or a character loader");
				}
			}
			if (characterHealth != null) 
			{
				characterHealth.Died += HandleChange;
				characterHealth.Loaded += HandleChange;
				characterHealth.GainLives += HandleChange;
			}
		}

		/// <summary>
		/// Get character reference when character loaded.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event args.</param>
		void HandleCharacterLoaded (object sender, CharacterEventArgs e)
		{
			characterHealth = e.Character.GetComponent<CharacterHealth> ();
			if (characterHealth != null) 
			{
				characterHealth.Died += HandleChange;
				characterHealth.Loaded += HandleChange;
				characterHealth.GainLives += HandleChange;
				UpdateText();
			} else {
				Debug.LogError("Loaded character didn't have a CharacterHealth");
			}
		}

		/// <summary>
		/// Handles any change event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void HandleChange (object sender, System.EventArgs e)
		{
			UpdateText ();
		}

		/// <summary>
		/// Updates the text field.
		/// </summary>
		virtual protected void UpdateText()
		{
			if (characterHealth != null)
			{
				// Usually lives displays the remaining lives (which excludes the current life).
				int lives = characterHealth.CurrentLives - 1;
				if (lives < 0) lives = 0;
				if (formatString != null && formatString != "") counterText.text = string.Format(formatString, lives);
				else counterText.text = lives.ToString();
			}
		}
	}
}