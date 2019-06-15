using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Shows score in a text field.
	/// </summary>
	[RequireComponent (typeof(Text))]
	public class UIScore : MonoBehaviour
	{
		/// <summary>
		/// String for the score type to show.
		/// </summary>
		[Tooltip ("String for the score type to show.")]
		public string scoreType;

		[Tooltip ("How long to wait between each counter update, or 0 not to wait at all.")]
		public float delay = 0.1f;

		/// <summary>
		/// The bar image.
		/// </summary>
		protected Text counterText;

		/// <summary>
		/// The value currently displayed.
		/// </summary>
		protected int currentValue;

		/// <summary>
		/// The value that will be displayed.
		/// </summary>
		protected int targetValue;

		/// <summary>
		/// Unity Awake hook.
		/// </summary>
		void Awake ()
		{
			ScoreManager.GetInstanceForType (scoreType).ScoreChanged += HandleScoreChanged;
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		void Start()
		{
			counterText = GetComponent<Text> ();
			if (counterText == null) Debug.LogWarning("UIScore needs to be on the same GameObject as a Text coponent");
			currentValue = ScoreManager.GetInstanceForType (scoreType).Score;
			counterText.text = currentValue.ToString();

		}

		/// <summary>
		/// Unity destroy hook.
		/// </summary>
		void OnDestroy()
		{
			ScoreManager.GetInstanceForType (scoreType).ScoreChanged -= HandleScoreChanged;
		}

		/// <summary>
		/// Handles the score changed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		virtual protected void HandleScoreChanged (object sender, ScoreEventArgs e)
		{
			SetTarget(e.Current);
		}

		/// <summary>
		/// Sets the target and restarts coroutine.
		/// </summary>
		virtual protected void SetTarget(int t)
		{
			StopCoroutine(CountToTarget());
			targetValue = t;
			// Assume zero is hard reset
			if (delay == 0 || targetValue == 0) 
			{
				currentValue = t;
				counterText.text = currentValue.ToString();
			}
			else
			{
				StartCoroutine(CountToTarget());
			}
		}

		/// <summary>
		/// Coroutine which counts score from current to target.
		/// </summary>
		virtual protected IEnumerator CountToTarget()
		{
			int diff = targetValue - currentValue;
			while (diff != 0)
			{
				if (diff > 2000) currentValue += 1000;
				else if (diff > 200) currentValue += 100;
				else if (diff > 20) currentValue += 100;
				else if (diff > 0) currentValue += 1;
				else if (diff < -2000) currentValue -= 1000;
				else if (diff < -200) currentValue -= 100;
				else if (diff < -20) currentValue -= 10;
				else if (diff < 0) currentValue -= 1;
				if (counterText != null) counterText.text = currentValue.ToString();
				diff = targetValue - currentValue;
				yield return new WaitForSeconds(delay);
			}
		}
	}
}
