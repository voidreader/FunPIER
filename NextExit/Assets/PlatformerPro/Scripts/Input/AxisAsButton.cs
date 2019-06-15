using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Treats an axis like a button.
	/// </summary>
	public class AxisAsButton : MonoBehaviour
	{
		/// <summary>
		/// The input that we will set button state on.
		/// </summary>
		public Input input;

		/// <summary>
		/// Axis to treat like a button.
		/// </summary>
		public string axisName = "Joystick1Axis1";

		/// <summary>
		/// The index of the action button.
		/// </summary>
		public int actionButtonNumber;

		/// <summary>
		/// How far the button has to be pushed down to count as pushed.
		/// </summary>
		public float pushThreshold = 0.7f;
		
		/// <summary>
		/// How far the button has to be released to count as released.
		/// </summary>
		public float releaseThreshold = 0.5f;

		/// <summary>
		/// Is the button currently pushed down.
		/// </summary>
		protected bool isPushed;


		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start ()
		{
			Init ();
		}
		
		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void LateUpdate ()
		{
			if (enabled) CheckAxis ();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void Init()
		{
			if (input == null) input = FindObjectOfType<StandardInput> ();
			if (input == null)
			{
				Debug.LogWarning ("No input found");
				enabled = false;
				return;
			}
			if (axisName == null || axisName == "" && axisName == "none")
			{
				Debug.LogWarning("Invalid axis name. Disabling AxisAsButton.");
				enabled = false;
				return;
			}


		}

		/// <summary>
		/// Checks the axis.
		/// </summary>
		virtual protected void CheckAxis()
		{
			float axisValue = UnityEngine.Input.GetAxis(axisName);
			if (isPushed)
			{
				if ((releaseThreshold > 0 && axisValue <= releaseThreshold) ||  (releaseThreshold < 0 && axisValue >= releaseThreshold))
				{
					isPushed = false;
					// We don't force state NONE as some other button might set it
				}
				else
				{
					input.ForceButtonState(actionButtonNumber, ButtonState.HELD);
				}
			}
			else
			{
				if ((releaseThreshold > 0 && axisValue >= pushThreshold) || (releaseThreshold < 0 && axisValue <= pushThreshold))
				{
					isPushed = true;
					input.ForceButtonState(actionButtonNumber, ButtonState.DOWN);
				}
				else
				{
					// We don't force state NONE as some other button might set it
				}
			}
		}

	}
}