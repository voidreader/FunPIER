using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Wraps a standard input and allows you to define triggers as action buttons
	/// </summary>
	public class TriggerAsButtonInput : StandardInput
	{
		void Update()
		{
		}

		/// <summary>
		/// Gets the state of the default action button.
		/// </summary>
		override public ButtonState ActionButton
		{
			get
			{
				return base.ActionButton;
			}
		}

		/// <summary>
		/// Gets the state of the given action button.
		/// </summary>
		/// <returns>The action button.</returns>
		/// <param name="index">Index.</param>
		override public ButtonState GetActionButtonState(int index)
		{
			return base.GetActionButtonState (index);
		}
	}
}
