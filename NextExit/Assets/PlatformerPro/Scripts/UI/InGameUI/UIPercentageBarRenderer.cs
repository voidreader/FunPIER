using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Renders a UI value provider as a percentage amount using the fill amount or sizeDelta(in x) of an image.
	/// </summary>
	[RequireComponent (typeof(Image))]
	public class UIPercentageBarRenderer : PlatformerProMonoBehaviour, IValueRenderer 
	{
		/// <summary>
		/// Format string used to display the value.
		/// </summary>
		[Tooltip ("If true we will fill the image. If false we will change its sizeDelta in x. If using size delta you " +
				  "can rotate the component to get a vertical bar.")]
		public bool useFillAmount = true;

		/// <summary>
		/// Should we use percentage value or float value?
		/// </summary>
		[Tooltip ("Should we use percentage value or the raw float value?")]
		public bool usePercentageValue = true;

		/// <summary>
		/// If we are not using fill amount, this multiplier is applied as follows to get the final value: finalvalue = value * originalSizeDelta * scaleMultiplier")]
		/// </summary>
		[Tooltip ("If we are not using fill amount, this multiplier is applied as follows to get the final" +
				  " value: finalvalue = value * originalSizeDelta * scaleMultiplier")]
		public float scaleMultiplier = 1.0f;

		/// <summary>
		/// Stores original x sizeDelta.
		/// </summary>
		protected float originalDelta;

		/// <summary>
		/// Gets the header string used to describe the component.
		/// </summary>
		/// <value>The header.</value>
		override public string Header
		{
			get
			{
				return "Renders the parent UIValueProvider in to an image by changing the images fillAmount or x sizeDelta.";
			}
		}


		/// <summary>
		/// The text comopnent to update..
		/// </summary>
		protected Image myImage;

		/// <summary>
		/// Unity Start() hook.
		/// </summary>
		void Start()
		{
			myImage = GetComponent<Image> ();
			if (!useFillAmount)
			{
				originalDelta = myImage.rectTransform.sizeDelta.x;
			}
		}

		/// <summary>
		/// Render the specified value.
		/// </summary>
		/// <param name="value">Value.</param>
		public void Render(UIValueProvider provider)
		{
			float value = usePercentageValue ? provider.PercentageValue : provider.FloatValue;
			if (useFillAmount)
			{
				myImage.fillAmount = value;
			}
			else
			{
				myImage.rectTransform.sizeDelta = new Vector2 (originalDelta * value * scaleMultiplier, myImage.rectTransform.sizeDelta.y);
			}
		}
	}
}