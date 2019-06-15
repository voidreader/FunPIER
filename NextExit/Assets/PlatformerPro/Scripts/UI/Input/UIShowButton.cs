using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Shows the name of an assigned button or axis in the inspector.
	/// </summary>
	[RequireComponent (typeof(Text))]
	public class UIShowButton : MonoBehaviour 
	{
		/// <summary>
		/// The input to show.
		/// </summary>
		[Tooltip ("The input to show configuration for.")]
		public Input input;

		/// <summary>
		/// The type of key to show.
		/// </summary>
		[Tooltip ("The type of key to show.")]
		public KeyType keyType;

		/// <summary>
		/// If KeyType is an action button then this is the action button number. Otherwise ignored.
		/// </summary>
		[Tooltip ("If KeyType is an action button then this is the action button number. Otherwise ignored.")]
		public int actionButton;

		/// <summary>
		/// Cached reference to text field.
		/// </summary>
		protected Text textField;

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start()
		{
			UpdateText ();
		}

		/// <summary>
		/// Updates the text field with the name of the assigned button or axis.
		/// </summary>
		public void UpdateText()
		{
			if (textField == null) textField = GetComponent<Text>();
			if (input == null) GetInput ();
			if (input == null)
			{
				Debug.LogWarning ("UIShowButton was unable to find an input");
				return;
			}
			switch (keyType)
			{
			case KeyType.VERTICAL_AXIS:
				textField.text = input.GetAxisForType (keyType);
				if (input is StandardInput && !((StandardInput)input).enableController)
				{
					textField.text = "";
				}
				break;
			case KeyType.HORIZONTAL_AXIS:
				textField.text = input.GetAxisForType (keyType);
				if (input is StandardInput && !((StandardInput)input).enableController)
				{
					textField.text = "";
				}
				break;
			default:
				textField.text = input.GetKeyForType (keyType, actionButton).ToString();
				break;
			}
		}

		/// <summary>
		/// Finds an input in the scene.
		/// </summary>
		protected void GetInput()
		{
			Input[] inputs = FindObjectsOfType<Input>();
			if (inputs.Length == 1) 
			{
				if (inputs[0] is DisabledInput)
				{
					return;
				}
				else 
				{
					input = inputs[0];
					return;
				}
			}
			else if (inputs.Length == 0)
			{
				return;
			}
			else 
			{
				// Check for a standard input, these are preferred
				foreach (Input potentialInput in inputs)
				{
					if (potentialInput is StandardInput)
					{
						input = potentialInput;
						break;
					}
				}
				// Otherwise do the best we can
				if (input == null)
				{
					// Okay so if they added two disabled inputs this would fail, but really?
					if (inputs[0] is DisabledInput) input = inputs[1];
					else input = inputs[0];
				}
			}
		}
	}
}