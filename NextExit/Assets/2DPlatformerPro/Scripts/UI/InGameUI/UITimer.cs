using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Shows score in a text field.
	/// </summary>
	[RequireComponent (typeof(Text))]
	public class UITimer : MonoBehaviour
	{
		[Tooltip ("Format string to use for the time. The variable {0} is the current time. Note if you use D formatting you need to convert time to int.")]
		[SerializeField]
		protected string formatString = "{0:00}";

		[Tooltip ("Should we pass time as an integer to the string formater. Required if youw ant to use D style formating.")]
		[SerializeField]
		protected bool convertTimeToInt = true;

		/// <summary>
		/// The text field to show timer on.
		/// </summary>
		protected Text counterText;

		/// <summary>
		/// Cached reference to the timer.
		/// </summary>
		protected TimeManagerWithTimer timer;

		/// <summary>
		/// Start this instance.
		/// </summary>
		void Start()
		{
			counterText = GetComponent<Text> ();
			if (!(TimeManager.Instance is TimeManagerWithTimer))
		    {
				Debug.LogWarning("UITimer cannot work unless the Manager is a TimeManagerWithTimer. Destroying.");
				Destroy(gameObject);
			}
			else
			{
				timer = (TimeManagerWithTimer) TimeManager.Instance;
			}
		}

		
		void Update()
		{
			counterText.text = string.Format (formatString, (convertTimeToInt ? Mathf.CeilToInt(timer.CurrentTime) : timer.CurrentTime));
		}

	}
}
