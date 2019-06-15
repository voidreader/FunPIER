using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Renders a UI value provider as text.
	/// </summary>
	[RequireComponent (typeof(Text))]
	public class UITextRenderer : PlatformerProMonoBehaviour, IValueRenderer 
	{
		/// <summary>
		/// Format string used to display the value.
		/// </summary>
		[Tooltip ("Format string used to display the value. If empty the ToString() method will be used.")]
		public string formatString = "";

		/// <summary>
		/// Gets the header string used to describe the component.
		/// </summary>
		/// <value>The header.</value>
		override public string Header
		{
			get
			{
				return "Renders the parent UIValueProvider in to a text field as per the provided format string.";
			}
		}


		/// <summary>
		/// The text comopnent to update..
		/// </summary>
		protected Text myText;

		/// <summary>
		/// Unity Start() hook.
		/// </summary>
		void Start()
		{
			myText = GetComponent<Text> ();
		}

		/// <summary>
		/// Render the specified value.
		/// </summary>
		/// <param name="value">Value.</param>
		public void Render(UIValueProvider provider)
		{
			if (provider == null || provider.RawValue == null) return;
			if (formatString != null && formatString != "")
			{
				myText.text = string.Format (formatString, provider.RawValue);
			}
			else
			{
				myText.text = provider.RawValue.ToString ();
			}
		}
	}
}