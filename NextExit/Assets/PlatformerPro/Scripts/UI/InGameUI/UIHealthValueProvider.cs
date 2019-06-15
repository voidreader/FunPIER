using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Provides the health value for rendering by IValueRenderer components.
	/// </summary>
	public class UIHealthValueProvider : UIValueProvider 
	{

		/// <summary>
		/// Gets the header string used to describe the component.
		/// </summary>
		/// <value>The header.</value>
		override public string Header
		{
			get
			{
				return "Provides the characters health as a value to be rendered by one or more IValueRenderers.";
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
				return characterHealth.CurrentHealth;
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
				return characterHealth.CurrentHealth;
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
				return characterHealth.MaxHealth;
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
				return characterHealth.CurrentHealthAsPercentage;
			}
		}

	}
}