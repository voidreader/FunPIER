// To use first import and install Rewired then uncomment the line below

// #define REWIRED_AVAILABLE

#if REWIRED_AVAILABLE

using UnityEngine;
using System.Collections;
using Rewired;

namespace PlatformerPro.ThirdPartyWrappers
{

	/// <summary>
	/// Wraps a rewired input.
	/// </summary>
	public class RewiredInputWrapper : Input
	{
		/// <summary>
		/// Id of the player this input is for.
		/// </summary>
		public int playerId = 0;

		/// <summary>
		/// Name of the horizontal axis action.
		/// </summary>
		public string horizontalAxisName = "MoveHorizontal";

		/// <summary>
		/// Name of the secondary horizontal axis action.
		/// </summary>
		public string horizontalAxisAltName = "MoveHorizontalAlt";

		/// <summary>
		/// Name of the vertical axis action.
		/// </summary>
		public string verticalAxisName = "MoveVertical";

		
		/// <summary>
		/// Name of the secondary vertical axis action.
		/// </summary>
		public string verticalAxisAltName = "MoveVerticalAlt";

		/// <summary>
		/// The name of the jump button action.
		/// </summary>
		public string jumpButtonName = "Jump";

		/// <summary>
		/// The name of the jump button action.
		/// </summary>
		public string runButtonName = "Run";

		/// <summary>
		/// The name of the jump button action.
		/// </summary>
		public string pauseButtonName = "Pause";

		/// <summary>
		/// The name of the action button actions.
		/// </summary>
		public string[] actionButtonNames = new string[]{"Shoot"};

		/// <summary>
		/// Cached player reference.
		/// </summary>
		protected Player player;

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
			player = ReInput.players.GetPlayer(playerId);
		}

		/// <summary>
		/// HorizontalAxis
		/// </summary>
		override public float HorizontalAxis
		{
			get
			{
				// Need to check if axis is defined
				return player.GetAxis(horizontalAxisName);
			}
		}
		
		/// <summary>
		/// HorizontalAxisDigital
		/// </summary>
		override public int HorizontalAxisDigital
		{
			get
			{
				// Need to check if axis is defined
				// Get horizontal axis value
				float result = HorizontalAxis;
				// Right
				if (result > 0) return 1;
				// Left
				if (result < 0) return -1;
				// None
				return 0;
			}
		}
		
		/// <summary>
		/// Return ButtonState.DOWN if the axis state went from <= 0 to 1 or >= 0 to -1.
		/// Return ButtonState.HELD if the axis stayed at the same value and wasn't 0;
		/// Return ButtonState.UP if the axis state went from != 0 to 0.
		/// </summary>
		override public ButtonState HorizontalAxisState
		{
			get
			{
				Debug.LogWarning ("RewiredInputWrapper does not support button states on an axis");
				return ButtonState.NONE;
			}
		}
		
		/// <summary>
		/// VerticalAxis
		/// </summary>
		override public float VerticalAxis
		{
			get
			{
				// Need to check if axis is defined
				return player.GetAxis(verticalAxisName);
			}
		}
		
		/// <summary>
		/// VerticalAxisDigital
		/// </summary>
		override public int VerticalAxisDigital
		{
			get
			{
				// Need to check if axis is defined
				// Get horizontal axis value
				float result = VerticalAxis;
				// Up
				if (result > 0) return 1;
				// Down
				if (result < 0) return -1;
				// None
				return 0;
			}
		}
		
		
		/// <summary>
		/// Return ButtonState.DOWN if the axis state went from <= 0 to 1 or >= 0 to -1.
		/// Return ButtonState.HELD if the axis stayed at the same value and wasn't 0;
		/// Return ButtonState.UP if the axis state went from != 0 to 0.
		/// </summary>
		override public ButtonState VerticalAxisState
		{
			get
			{
				Debug.LogWarning ("RewiredInputWrapper does not support button states on an axis");
				return ButtonState.NONE;
			}
		}
		
		/// <summary>
		/// JumpButton
		/// </summary>
		override public ButtonState JumpButton
		{
			get
			{
				return GetButtonState(jumpButtonName);
			}
		}
		
		/// <summary>
		/// RunButton
		/// </summary>
		override public ButtonState RunButton
		{
			get
			{
				return GetButtonState(runButtonName);
			}
		}
		
		/// <summary>
		/// PauseButton
		/// </summary>
		override public ButtonState PauseButton
		{
			get
			{
				return GetButtonState(pauseButtonName);
			}
		}
		
		/// <summary>
		/// ActionButton
		/// </summary>
		override public ButtonState ActionButton
		{
			get
			{
				if (actionButtonNames.Length == 0) return ButtonState.NONE;
				return GetActionButtonState(0);
			}
		}
		
		/// <summary>
		/// GetActionButtonState
		/// </summary>
		/// <param name="buttonIndex"></param>
		/// <returns></returns>
		override public ButtonState GetActionButtonState(int buttonIndex)
		{
			ButtonState result = ButtonState.NONE;
			
			if (actionButtonNames.Length > 0 && buttonIndex >= 0 && actionButtonNames.Length > buttonIndex) result = GetButtonState(actionButtonNames[buttonIndex]);
			
			return result;
		}
		
		/// <summary>
		/// Returns the button state (DOWN, UP, HELD, or NONE), for the button defined in the Unity Input Manager
		/// </summary>
		/// <param name="button">Name of the button defined in the Unity Input Manager</param>
		/// <returns>ButtonState</returns>
		virtual protected ButtonState GetButtonState(string button)
		{
			if (player.GetButtonUp(button)) return  ButtonState.UP;
			if (player.GetButtonDown(button)) return  ButtonState.DOWN;
			if (player.GetButton(button)) return  ButtonState.HELD;
			return  ButtonState.NONE;
		}
		
		/// <summary>
		/// Returns true if any button or action key is being pressed.
		/// </summary>
		/// <value><c>true</c> if any key; otherwise, <c>false</c>.</value>
		override public bool AnyKey
		{
			get { return player.GetAnyButton(); }
		}
		
		/// <summary>
		/// Sets the keyboayrd key that corresponds to a Platform PRO input key.
		/// </summary>
		/// <returns><c>true</c>, if key was set, <c>false</c> otherwise.</returns>
		/// <param name="type">Type of key being set.</param>
		/// <param name="keyCode">Unity key code.</param>
		override public bool SetKey(KeyType type, KeyCode keyCode) 
		{
			Debug.LogError ("RewiredInputWrapper does not allow for in game control configuration. Use Rewireds system.");
			return false;
		}
		
		/// <summary>
		/// Sets the keyboayrd key that corresponds to a Platform PRO input key.
		/// </summary>
		/// <returns><c>true</c>, if key was set, <c>false</c> otherwise.</returns>
		/// <param name="type">Type of key being set.</param>
		/// <param name="keyNumber">The action key number or ignored if not setting an action key</param>
		override public bool SetKey(KeyType type, KeyCode keyCode, int keyNumber)
		{
			Debug.LogError ("RewiredInputWrapper does not allow for in game control configuration. Use Rewireds system.");
			return false;
		}
		
		
		/// <summary>
		/// Gets the key code for the given type.
		/// </summary>
		/// <returns>The mathcing KeyCode or keyCode.None if there is no match.</returns>
		/// <param name="type">Key type.</param>
		/// <param name="keyNumber">Key number if this is an action key (ignored otherwise).</param>
		override public KeyCode GetKeyForType(KeyType type, int keyNumber) 
		{
			Debug.LogError ("RewiredInputWrapper does not allow for in game control configuration. Use Rewireds system.");
			return KeyCode.None;
		}
		
		/// <summary>
		/// Gets the name of the axis for the given key type.
		/// </summary>
		/// <returns>The axis name.</returns>
		/// <param name="type">Type.</param>
		override public string GetAxisForType (KeyType type)
		{
			Debug.LogError ("RewiredInputWrapper does not allow for in game control configuration. Use Rewireds system.");
			return "";
		}
		
		/// <summary>
		/// Sets the joystick axis that corresponds to a Platform PRO input axis.
		/// </summary>
		/// <returns><c>true</c>, if key was set, <c>false</c> otherwise.</returns>
		/// <param name="type">Type of key being set.</param>
		/// <param name="axis">Unity axis name.</param>
		override public bool SetAxis(KeyType type, string axis, bool reverseAxis)
		{
			Debug.LogError ("RewiredInputWrapper does not allow for in game control configuration. Use Rewireds system.");
			return false;
		}
		
		/// <summary>
		/// Saves the input data.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		override public bool SaveInputData()
		{
			Debug.LogError ("RewiredInputWrapper does not allow for in game control configuration. Use Rewireds system.");
			return false;
		}
		
		/// <summary>
		/// Loads the input data.
		/// </summary>
		/// <returns><c>true</c>, if input data was loaded, <c>false</c> otherwise.</returns>
		/// <param name="dataName">Data name.</param>
		override public bool LoadInputData(string dataName)
		{
			Debug.LogError ("RewiredInputWrapper does not allow for in game control configuration. Use Rewireds system.");
			return false;
		}
		
		/// <summary>
		/// Loads the provided input data.
		/// </summary>
		/// <param name="data">Data to load.</param>
		override public void LoadInputData(StandardInputData data)
		{
			Debug.LogError ("RewiredInputWrapper does not allow for in game control configuration. Use Rewireds system.");
		}
		
		/// <summary>
		/// A float value clamped between -1 for completely left and 1 for compeletely right.
		/// 0.5f would mean "half right". The exact interpretation of the values is up to the
		/// movement behaviours.
		/// </summary>
		override public float AltHorizontalAxis
		{
			get
			{
				// Need to check if axis is defined
				return player.GetAxis(horizontalAxisAltName);
			}
		}
		
		/// <summary>
		/// A "digital" version of the alternate horizontal axis in which the only valid values are -1 for LEFT, 
		/// 0 for NONE, and 1 for RIGHT.
		/// </summary>
		override public int AltHorizontalAxisDigital
		{
			get
			{
				// Need to check if axis is defined
				// Get horizontal axis value
				float result = AltHorizontalAxis;
				// Right
				if (result > 0) return 1;
				// Left
				if (result < 0) return -1;
				// None
				return 0;
			}
		}
		
		/// <summary>
		/// Return ButtonState.DOWN if the axis state went from <= 0 to 1 or >= 0 to -1.
		/// Return ButtonState.HELD if the axis stayed at the same value.
		/// Return ButtonState.UP if the axis state went from != 0 to 0.
		/// </summary>
		override public ButtonState AltHorizontalAxisState
		{
			get
			{
				Debug.LogError ("RewiredInputWrapper does not support the alternate axis.");
				return ButtonState.NONE;
			}
		}
		
		/// <summary>
		/// A float value clamped between -1 for completely DOWN and 1 for completely UP.
		/// 0.5f would mean "half up". The exact interpretation of the values is up to the
		/// movement behaviours.
		/// </summary>
		override public float AltVerticalAxis
		{
			get
			{
				// Need to check if axis is defined
				return player.GetAxis(verticalAxisAltName);
			}
		}
		
		/// <summary>
		/// A "digital" version of the vertical axis in which the only valid values are -1 for DOWN, 
		/// 0 for NONE, and 1 for UP.
		/// </summary>
		override public int AltVerticalAxisDigital
		{
			get
			{
				// Need to check if axis is defined
				// Get horizontal axis value
				float result = AltVerticalAxis;
				// Right
				if (result > 0) return 1;
				// Left
				if (result < 0) return -1;
				// None
				return 0;
			}
		}
		
		/// <summary>
		/// Return ButtonState.DOWN if the axis state went from <= 0 to 1 or >= 0 to -1.
		/// Return ButtonState.HELD if the axis stayed at the same value.
		/// Return ButtonState.UP if the axis state went from != 0 to 0.
		/// </summary>
		override public ButtonState AltVerticalAxisState
		{
			get
			{
				Debug.LogError ("RewiredInputWrapper does not support the alternate axis.");
				return ButtonState.NONE;
			}
		}

	}
}
#endif