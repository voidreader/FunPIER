using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A very simple axis that just wraps a negative and positive button and provides values of -1, 0 and 1.
	/// </summary>
	public class TwoButtonTouchAxis : MonoBehaviour, ITouchAxis
	{
		/// <summary>
		/// The negative buttons Game Obejct.
		/// </summary>
		public GameObject negativeButton;

		/// <summary>
		/// The negative button.
		/// </summary>
		protected ITouchButton actualNegativeButton;

		/// <summary>
		/// The positive buttons Game Obejct.
		/// </summary>
		public GameObject positiveButton;
		
		/// <summary>
		/// The positive button.
		/// </summary>
		protected ITouchButton actualPositiveButton;

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start()
		{
			Init ();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void Init()
		{
			actualNegativeButton = (ITouchButton)negativeButton.GetComponent (typeof(ITouchButton));
			actualPositiveButton = (ITouchButton)positiveButton.GetComponent (typeof(ITouchButton));

			if (actualNegativeButton == null) Debug.LogWarning ("Couldn't find the negative button for the touch axis");
			if (actualPositiveButton == null) Debug.LogWarning ("Couldn't find the positive button for the touch axis");
		}

		/// <summary>
		/// Gets the axis value.
		/// </summary>
		/// <value>The value.</value>
		public float Value
		{
			get
			{
				if (actualNegativeButton.ButtonState == ButtonState.DOWN || actualNegativeButton.ButtonState == ButtonState.HELD)
				{
					if (actualPositiveButton.ButtonState == ButtonState.DOWN || actualPositiveButton.ButtonState == ButtonState.HELD)
					{
						// Both held
						return 0;
					}
					return -1.0f;
				}
				else if (actualPositiveButton.ButtonState == ButtonState.DOWN || actualPositiveButton.ButtonState == ButtonState.HELD)
				{
					return 1.0f;
				}
				return 0;
			}
		}
	}
}
