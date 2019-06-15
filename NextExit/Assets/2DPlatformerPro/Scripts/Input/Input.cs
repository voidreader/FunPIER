using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	// TODO Why isn't this an interface

	/// <summary>
	/// Abstract defintion of what is required in an input class.
	/// </summary>
	public abstract class Input : MonoBehaviour
	{
		/// <summary>
		/// A float value clamped between -1 for completely left and 1 for compeletely right.
		/// 0.5f would mean "half right". The exact interpretation of the values is up to the
		/// movement behaviours.
		/// </summary>
		abstract public float HorizontalAxis
		{
			get;
		}

		/// <summary>
		/// A "digital" version of the horizontal axis in which the only valid values are -1 for LEFT, 
		/// 0 for NONE, and 1 for RIGHT.
		/// </summary>
		abstract public int HorizontalAxisDigital
		{
			get;
		}

		/// <summary>
		/// Return ButtonState.DOWN if the axis state went from <= 0 to 1 or >= 0 to -1.
		/// Return ButtonState.HELD if the axis stayed at the same value.
		/// Return ButtonState.UP if the axis state went from != 0 to 0.
		/// </summary>
		abstract public ButtonState HorizontalAxisState
		{
			get;
		}

		/// <summary>
		/// A float value clamped between -1 for completely DOWN and 1 for completely UP.
		/// 0.5f would mean "half up". The exact interpretation of the values is up to the
		/// movement behaviours.
		/// </summary>
		abstract public float VerticalAxis
		{
			get;
		}

		/// <summary>
		/// A "digital" version of the alternate vertical axis in which the only valid values are -1 for DOWN, 
		/// 0 for NONE, and 1 for UP.
		/// </summary>
		abstract public int VerticalAxisDigital
		{
			get;
		}

		/// <summary>
		/// Return ButtonState.DOWN if the axis state went from <= 0 to 1 or >= 0 to -1.
		/// Return ButtonState.HELD if the axis stayed at the same value.
		/// Return ButtonState.UP if the axis state went from != 0 to 0.
		/// </summary>
		abstract public ButtonState VerticalAxisState
		{
			get;
		}

		/// <summary>
		/// A float value clamped between -1 for completely left and 1 for compeletely right.
		/// 0.5f would mean "half right". The exact interpretation of the values is up to the
		/// movement behaviours.
		/// </summary>
		abstract public float AltHorizontalAxis
		{
			get;
		}
		
		/// <summary>
		/// A "digital" version of the alternate horizontal axis in which the only valid values are -1 for LEFT, 
		/// 0 for NONE, and 1 for RIGHT.
		/// </summary>
		abstract public int AltHorizontalAxisDigital
		{
			get;
		}
		
		/// <summary>
		/// Return ButtonState.DOWN if the axis state went from <= 0 to 1 or >= 0 to -1.
		/// Return ButtonState.HELD if the axis stayed at the same value.
		/// Return ButtonState.UP if the axis state went from != 0 to 0.
		/// </summary>
		abstract public ButtonState AltHorizontalAxisState
		{
			get;
		}
		
		/// <summary>
		/// A float value clamped between -1 for completely DOWN and 1 for completely UP.
		/// 0.5f would mean "half up". The exact interpretation of the values is up to the
		/// movement behaviours.
		/// </summary>
		abstract public float AltVerticalAxis
		{
			get;
		}
		
		/// <summary>
		/// A "digital" version of the vertical axis in which the only valid values are -1 for DOWN, 
		/// 0 for NONE, and 1 for UP.
		/// </summary>
		abstract public int AltVerticalAxisDigital
		{
			get;
		}
		
		/// <summary>
		/// Return ButtonState.DOWN if the axis state went from <= 0 to 1 or >= 0 to -1.
		/// Return ButtonState.HELD if the axis stayed at the same value.
		/// Return ButtonState.UP if the axis state went from != 0 to 0.
		/// </summary>
		abstract public ButtonState AltVerticalAxisState
		{
			get;
		}

		/// <summary>
		/// State of the jump button.
		/// </summary>
		/// <value>The jump button.</value>
		abstract public ButtonState JumpButton
		{
			get;
		}

		/// <summary>
		/// State of the run button.
		/// </summary>
		/// <value>The run button.</value>
		abstract public ButtonState RunButton
		{
			get;
		}

		/// <summary>
		/// State of the pause button.
		/// </summary>
		/// <value>The pause button.</value>
		abstract public ButtonState PauseButton
		{
			get;
		}

		/// <summary>
		/// State of the default action button. This could be pickup, attack, etc. If you need
		/// more buttons use the additional action use 
		/// <see cref="Input.GetActionButtonState()"/>
		/// </summary>
		/// <value>The crouch button.</value>
		abstract public ButtonState ActionButton
		{
			get;
		}

		/// <summary>
		/// Returns true if any button or action key is being pressed.
		/// </summary>
		/// <value><c>true</c> if any key; otherwise, <c>false</c>.</value>
		abstract public bool AnyKey
		{
			get;
		}

		/// <summary>
		/// Get the state of an action button.
		/// </summary>
		/// <returns>The buttons state.</returns>
		/// <param name="buttonIndex">The index of the button.</param>
		abstract public ButtonState GetActionButtonState(int buttonIndex);

		/// <summary>
		/// Sets the joystick axis that corresponds to a Platform PRO input axis.
		/// </summary>
		/// <returns><c>true</c>, if key was set, <c>false</c> otherwise.</returns>
		/// <param name="type">Type of key being set.</param>
		/// <param name="axis">Unity axis name.</param>
		/// <param name="reverseAxis">Should axis values be reversed.</param>
		abstract public bool SetAxis(KeyType type, string axis, bool reverseAxis);


		/// <summary>
		/// Sets the keyboayrd key that corresponds to a Platform PRO input key.
		/// </summary>
		/// <returns><c>true</c>, if key was set, <c>false</c> otherwise.</returns>
		/// <param name="type">Type of key being set.</param>
		/// <param name="keyCode">Unity key code.</param>
		abstract public bool SetKey(KeyType type, KeyCode keyCode);

		/// <summary>
		/// Sets the keyboayrd key that corresponds to a Platform PRO input key.
		/// </summary>
		/// <returns><c>true</c>, if key was set, <c>false</c> otherwise.</returns>
		/// <param name="type">Type of key being set.</param>
		/// <param name="keyNumber">The action key number or ignored if not setting an action key</param>
		abstract public bool SetKey(KeyType type, KeyCode keyCode, int keyNumber);

		/// <summary>
		/// Gets the key code for the given type.
		/// </summary>
		/// <returns>The mathcing KeyCode or keyCode.None if there is no match.</returns>
		/// <param name="type">Key type.</param>
		/// <param name="keyNumber">Key number if this is an action key (ignored otherwise).</param>
		abstract public KeyCode GetKeyForType(KeyType type, int keyNumber);

		/// <summary>
		/// Gets the name of the axis for the given key type.
		/// </summary>
		/// <returns>The axis name.</returns>
		/// <param name="type">Type.</param>
		abstract public string GetAxisForType (KeyType type);

		/// <summary>
		/// Saves the input data.
		/// </summary>
		/// <returns><c>true</c>, if input data was saved, <c>false</c> otherwise.</returns>
		abstract public bool SaveInputData();

		/// <summary>
		/// Loads the input data for the given data name.
		/// </summary>
		/// <returns><c>true</c>, if input data was loaded, <c>false</c> otherwise.</returns>
		/// <param name="dataName">Data to load.</param>
		abstract public bool LoadInputData(string dataName);

		/// <summary>
		/// Loads the provided input data.
		/// </summary>
		/// <param name="data">Data to load.</param>
		abstract public void LoadInputData(StandardInputData data);

	}

}