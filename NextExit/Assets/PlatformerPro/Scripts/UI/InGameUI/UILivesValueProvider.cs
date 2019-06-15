using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Provides the lives value for rendering by IValueRenderer components.
	/// </summary>
	public class UILivesValueProvider : UIValueProvider
	{

		public bool showLivesAsLivesMinusOne = true;
		
		/// <summary>
		/// Gets the header string used to describe the component.
		/// </summary>
		/// <value>The header.</value>
		override public string Header
		{
			get
			{
				return "Provides the characters lives as a value to be rendered by one or more IValueRenderers.";
			}
		}


		/// <summary>
		/// Gets the raw value.
		/// </summary>
		/// <value>The value.</value>
		override public object RawValue
		{
			get
			{
				if (characterHealth == null) return "";
				if (characterHealth.CurrentLives == 0) return 0;
				return characterHealth.CurrentLives - (showLivesAsLivesMinusOne ? 1 : 0);
			}
		}

		/// <summary>
		/// Gets the int value.
		/// </summary>
		/// <value>The int value.</value>
		override public int IntValue
		{
			get
			{
				if (characterHealth == null) return 0;
				if (characterHealth.CurrentLives == 0) return 0;
				return characterHealth.CurrentLives - (showLivesAsLivesMinusOne ? 1 : 0);
			}
		}

		/// <summary>
		/// Gets the int value.
		/// </summary>
		/// <value>The int value.</value>
		override public int IntMaxValue
		{
			get
			{
				if (characterHealth == null) return 0;
				return characterHealth.maxLives;
			}
		}

		/// <summary>
		/// Gets the value as percentage between 0 (0%) and 1 (100%).
		/// </summary>
		/// <value>The value.</value>
		override public float PercentageValue
		{
			get
			{
				if (characterHealth == null) return 0;
				// Exact value matches to avoid float errors on key values
				if (characterHealth.CurrentLives == 0) return 0.0f;
				if (characterHealth.CurrentLives == characterHealth.MaxLives) return 1.0f;
				// Other values
				return (float) characterHealth.CurrentLives / (float) characterHealth.MaxLives;
			}
		}

	}
}