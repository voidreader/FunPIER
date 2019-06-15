using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Input which accepts input form multiple sources and provides an interface that makes them look like one source.
	/// </summary>
	public class MultiInput : Input
	{
		/// <summary>
		/// An ordered list of inputs. Highest in the list is given priority.
		/// </summary>
		[Tooltip ("An ordered list of inputs. Highest in the list is given priority.")]
		public Input[] inputs;

		void Start()
		{
			if (inputs == null) Debug.LogWarning ("No inputs supplied to MultiInput, you should define some!"); 
		}

		/// <summary>
		/// Get the state of an action button.
		/// </summary>
		/// <returns>The buttons state.</returns>
		/// <param name="buttonIndex">The index of the button.</param>
		public override ButtonState GetActionButtonState (int buttonIndex)
		{
			for (int i = 0; i < inputs.Length; i++)
			{
				if (inputs[i].GetActionButtonState(buttonIndex) != ButtonState.NONE) return inputs[i].GetActionButtonState(buttonIndex);
			}
			return ButtonState.NONE;
		}

		/// <summary>
		/// A float value clamped between -1 for completely left and 1 for compeletely right.
		/// 0.5f would mean "half right". The exact interpretation of the values is up to the
		/// movement behaviours.
		/// </summary>
		/// <value>The horizontal axis.</value>
		public override float HorizontalAxis
		{
			get
			{
				for (int i = 0; i < inputs.Length; i++)
				{
					if (inputs[i].HorizontalAxis != 0) return inputs[i].HorizontalAxis;
				}
				return 0;
			}
		}

		/// <summary>
		/// A "digital" version of the horizontal axis in which the only valid values are -1 for LEFT, 
		/// 0 for NONE, and 1 for RIGHT.
		/// </summary>
		/// <value>The horizontal axis digital.</value>
		public override int HorizontalAxisDigital
		{
			get
			{
				for (int i = 0; i < inputs.Length; i++)
				{
					if (inputs[i].HorizontalAxisDigital != 0) return inputs[i].HorizontalAxisDigital;
				}
				return 0;
			}
		}

		/// <summary>
		/// Gets the state of the horizontal axis.
		/// </summary>
		/// <value>The state of the horizontal axis.</value>
		public override ButtonState HorizontalAxisState
		{
			get
			{
				for (int i = 0; i < inputs.Length; i++)
				{
					if (inputs[i].HorizontalAxisState != ButtonState.NONE) return inputs[i].HorizontalAxisState;
				}
				return ButtonState.NONE;
			}
		}

		/// <summary>
		/// A float value clamped between -1 for completely DOWN and 1 for completely UP.
		/// 0.5f would mean "half up". The exact interpretation of the values is up to the
		/// movement behaviours.
		/// </summary>
		/// <value>The vertical axis.</value>
		public override float VerticalAxis
		{
			get
			{
				for (int i = 0; i < inputs.Length; i++)
				{
					if (inputs[i].VerticalAxis != 0) return inputs[i].VerticalAxis;
				}
				return 0;
			}
		}

		/// <summary>
		/// A "digital" version of the alternate vertical axis in which the only valid values are -1 for DOWN, 
		/// 0 for NONE, and 1 for UP.
		/// </summary>
		/// <value>The vertical axis digital.</value>
		public override int VerticalAxisDigital
		{
			get
			{
				for (int i = 0; i < inputs.Length; i++)
				{
					if (inputs[i].VerticalAxisDigital != 0) return inputs[i].VerticalAxisDigital;
				}
				return 0;
			}
		}

		/// <summary>
		/// Gets the state of the vertical axis.
		/// </summary>
		/// <value>The state of the vertical axis.</value>
		public override ButtonState VerticalAxisState
		{
			get
			{
				for (int i = 0; i < inputs.Length; i++)
				{
					if (inputs[i].VerticalAxisState != ButtonState.NONE) return inputs[i].VerticalAxisState;
				}
				return ButtonState.NONE;
			}
		}

		/// <summary>
		/// A float value clamped between -1 for completely left and 1 for compeletely right.
		/// 0.5f would mean "half right". The exact interpretation of the values is up to the
		/// movement behaviours.
		/// </summary>
		/// <value>The alternate horizontal axis.</value>
		public override float AltHorizontalAxis
		{
			get
			{
				for (int i = 0; i < inputs.Length; i++)
				{
					if (inputs[i].AltHorizontalAxis != 0) return inputs[i].AltHorizontalAxis;
				}
				return 0;
			}
		}

		/// <summary>
		/// A "digital" version of the alternate horizontal axis in which the only valid values are -1 for LEFT, 
		/// 0 for NONE, and 1 for RIGHT.
		/// </summary>
		/// <value>The alternate horizontal axis digital.</value>
		public override int AltHorizontalAxisDigital
		{
			get
			{
				for (int i = 0; i < inputs.Length; i++)
				{
					if (inputs[i].AltHorizontalAxisDigital != 0) return inputs[i].AltHorizontalAxisDigital;
				}
				return 0;
			}
		}

		/// <summary>
		/// Gets the state of the alternate horizontal axis.
		/// </summary>
		/// <value>The state of the alternate horizontal axis.</value>
		public override ButtonState AltHorizontalAxisState
		{
			get
			{
				for (int i = 0; i < inputs.Length; i++)
				{
					if (inputs[i].AltHorizontalAxisState != ButtonState.NONE) return inputs[i].AltHorizontalAxisState;
				}
				return ButtonState.NONE;
			}
		}

		/// <summary>
		/// A float value clamped between -1 for completely DOWN and 1 for completely UP.
		/// 0.5f would mean "half up". The exact interpretation of the values is up to the
		/// movement behaviours.
		/// </summary>
		/// <value>The alternate vertical axis.</value>
		public override float AltVerticalAxis
		{
			get
			{
				for (int i = 0; i < inputs.Length; i++)
				{
					if (inputs[i].AltVerticalAxis != 0) return inputs[i].AltVerticalAxis;
				}
				return 0;
			}
		}

		/// <summary>
		/// A "digital" version of the vertical axis in which the only valid values are -1 for DOWN, 
		/// 0 for NONE, and 1 for UP.
		/// </summary>
		/// <value>The alternate vertical axis digital.</value>
		public override int AltVerticalAxisDigital
		{
			get
			{
				for (int i = 0; i < inputs.Length; i++)
				{
					if (inputs[i].AltVerticalAxisDigital != 0) return inputs[i].AltVerticalAxisDigital;
				}
				return 0;
			}
		}

		/// <summary>
		/// Gets the state of the alternate vertical axis.
		/// </summary>
		/// <value>The state of the alternate vertical axis.</value>
		public override ButtonState AltVerticalAxisState
		{
			get
			{
				for (int i = 0; i < inputs.Length; i++)
				{
					if (inputs[i].AltVerticalAxisState != ButtonState.NONE) return inputs[i].AltVerticalAxisState;
				}
				return ButtonState.NONE;
			}
		}

		/// <summary>
		/// State of the jump button.
		/// </summary>
		/// <value>The jump button.</value>
		public override ButtonState JumpButton
		{
			get
			{
				for (int i = 0; i < inputs.Length; i++)
				{
					if (inputs[i].JumpButton != ButtonState.NONE) return inputs[i].JumpButton;
				}
				return ButtonState.NONE;
			}
		}

		/// <summary>
		/// State of the run button.
		/// </summary>
		/// <value>The run button.</value>
		public override ButtonState RunButton
		{
			get
			{
				for (int i = 0; i < inputs.Length; i++)
				{
					if (inputs[i].RunButton != ButtonState.NONE) return inputs[i].RunButton;
				}
				return ButtonState.NONE;
			}
		}

		/// <summary>
		/// State of the pause button.
		/// </summary>
		/// <value>The pause button.</value>
		public override ButtonState PauseButton
		{
			get
			{
				for (int i = 0; i < inputs.Length; i++)
				{
					if (inputs[i].PauseButton != ButtonState.NONE) return inputs[i].PauseButton;
				}
				return ButtonState.NONE;
			}
		}

		/// <summary>
		/// Gets the action button.
		/// </summary>
		/// <value>The action button.</value>
		public override ButtonState ActionButton
		{
			get
			{
				for (int i = 0; i < inputs.Length; i++)
				{
					if (inputs[i].ActionButton != ButtonState.NONE) return inputs[i].ActionButton;
				}
				return ButtonState.NONE;
			}
		}

		/// <summary>
		/// Returns true if any button or action key is being pressed.
		/// </summary>
		/// <value>true</value>
		/// <c>false</c>
		public override bool AnyKey 
		{
			get
			{
				for (int i = 0; i < inputs.Length; i++)
				{
					if (inputs[i].AnyKey) return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Sets the joystick axis that corresponds to a Platform PRO input axis. NOT AVAILABLE in MultiInput.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="type">Type of key being set.</param>
		/// <param name="axis">Unity axis name.</param>
		/// <param name="reverseAxis">Should axis values be reversed.</param>
		public override bool SetAxis (KeyType type, string axis, bool reverseAxis)
		{
			Debug.LogWarning ("You cannot configure a MultiInput directly. Call this method on the wrapped inputs.");
			return false;
		}

		/// <summary>
		/// Sets the keyboayrd key that corresponds to a Platform PRO input key.  NOT AVAILABLE in MultiInput.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="type">Type of key being set.</param>
		/// <param name="keyCode">Unity key code.</param>
		public override bool SetKey (KeyType type, KeyCode keyCode)
		{
			Debug.LogWarning ("You cannot configure a MultiInput directly. Call this method on the wrapped inputs.");
			return false;
		}

		/// <summary>
		/// Sets the keyboayrd key that corresponds to a Platform PRO input key. NOT AVAILABLE in MultiInput.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="type">Type of key being set.</param>
		/// <param name="keyNumber">The action key number or ignored if not setting an action key</param>
		/// <param name="keyCode">Key code.</param>
		public override bool SetKey (KeyType type, KeyCode keyCode, int keyNumber)
		{
			Debug.LogWarning ("You cannot configure a MultiInput directly. Call this method on the wrapped inputs.");
			return false;
		}

		/// <summary>
		/// Gets the key code for the given type. NOT AVAILABLE in MultiInput.
		/// </summary>
		/// <returns>The mathcing KeyCode or keyCode.None if there is no match.</returns>
		/// <param name="type">Key type.</param>
		/// <param name="keyNumber">Key number if this is an action key (ignored otherwise).</param>
		public override KeyCode GetKeyForType (KeyType type, int keyNumber)
		{
			Debug.LogWarning ("You cannot configure a MultiInput directly. Call this method on the wrapped inputs.");
			return KeyCode.None;
		}

		/// <summary>
		/// Gets the name of the axis for the given key type. NOT AVAILABLE in MultiInput.
		/// </summary>
		/// <returns>The axis name.</returns>
		/// <param name="type">Type.</param>
		public override string GetAxisForType (KeyType type)
		{
			Debug.LogWarning ("You cannot configure a MultiInput directly. Call this method on the wrapped inputs.");
			return "NONE";
		}

		/// <summary>
		/// Saves the input data. NOT AVAILABLE in MultiInput.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		public override bool SaveInputData ()
		{
			Debug.LogWarning ("You cannot configure a MultiInput directly. Call this method on the wrapped inputs.");
			return false;
		}

		/// <summary>
		/// Loads the input data for the given data name. NOT AVAILABLE in MultiInput.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="dataName">Data to load.</param>
		public override bool LoadInputData (string dataName)
		{
			Debug.LogWarning ("You cannot configure a MultiInput directly. Call this method on the wrapped inputs.");
			return false;
		}

		/// <summary>
		/// Loads the input data for the given data name. NOT AVAILABLE in MultiInput.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="dataName">Data to load.</param>
		/// <param name="data">Data.</param>
		public override void LoadInputData (StandardInputData data)
		{
			Debug.LogWarning ("You cannot configure a MultiInput directly. Call this method on the wrapped inputs.");
		}
	}
}
