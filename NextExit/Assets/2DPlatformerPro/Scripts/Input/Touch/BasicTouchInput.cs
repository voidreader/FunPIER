using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A simple but effective touch input.
	/// </summary>
	public class BasicTouchInput : Input
	{

		/// <summary>
		/// GameObject that holds an ITouchAxis to be used as the horizontal axis.
		/// </summary>
		[Tooltip ("GameObject that holds an ITouchAxis to be used as the horizontal axis.")]
		public GameObject horizontalAxisGo;

		/// <summary>
		/// GameObject that holds an ITouchAxis to be used as the vertical axis.
		/// </summary>
		[Tooltip ("GameObject that holds an ITouchAxis to be used as the vertical axis.")]
		public GameObject verticalAxisGo;

		/// <summary>
		/// GameObject that holds an ITouchButton to be used as the jump button.
		/// </summary>
		public GameObject jumpButtonGo;

		/// <summary>
		/// GameObject that holds an ITouchButton to be used as the run button.
		/// </summary>
		public GameObject runButtonGo;

		/// <summary>
		/// GameObjects that hold ITouchButtons to be used as the actions buttons.
		/// </summary>
		public GameObject[] actionButtonGos;

		/// <summary>
		/// GameObject that holds an ITouchButton to be used as the pause button.
		/// </summary>
		public GameObject pauseButtonGo;


		/// <summary>
		/// Actual IAxis to use for horizontal axis.
		/// </summary>
		protected ITouchAxis actualHorizontalAxis;

		/// <summary>
		/// Actual IAxis to use for vertical axis.
		/// </summary>
		protected ITouchAxis actualVerticalAxis;

		/// <summary>
		/// Actual ITouchButton to use for jump.
		/// </summary>
		protected ITouchButton actualJumpButton;

		/// <summary>
		/// Actual ITouchButton to use for run.
		/// </summary>
		protected ITouchButton actualRunButton;

		/// <summary>
		/// Actual ITouchButton to use for pause.
		/// </summary>
		protected ITouchButton actualPauseButton;

		/// <summary>
		/// Array of the actual ITouchButtons to use for actions.
		/// </summary>
		protected ITouchButton[] actualActionButtons;

		/// <summary>
		/// Stores the last state of the vertical axis (digital).
		/// </summary>
		protected int lastDigitalVerticalAxisState;
		
		/// <summary>
		/// Stores the last state of the horizontal axis (digital).
		/// </summary>
		protected int lastDigitalHorizontalAxisState;
		
		/// <summary>
		/// Stores the last state of the alternate vertical axis (digital).
		/// </summary>
		//protected int lastDigitalAltVerticalAxisState;
		
		/// <summary>
		/// Stores the last state of the alternate horizontal axis (digital).
		/// </summary>
		//protected int lastDigitalAltHorizontalAxisState;


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
			// Get all the axis and button refs
			actualHorizontalAxis = (ITouchAxis)horizontalAxisGo.GetComponent (typeof(ITouchAxis));
			if (actualHorizontalAxis == null) Debug.LogWarning ("Couldn't find an ITouchAxis on the horizontal axis GameObject");
			if (verticalAxisGo != null ) 
			{
				actualVerticalAxis = (ITouchAxis)verticalAxisGo.GetComponent (typeof(ITouchAxis));
				if (actualVerticalAxis == null) Debug.LogWarning ("Couldn't find an ITouchAxis on the vertical axis GameObject");
			}


			actualJumpButton = (ITouchButton)jumpButtonGo.GetComponent (typeof(ITouchButton));
			if (runButtonGo != null )
			{
				actualRunButton = (ITouchButton)runButtonGo.GetComponent (typeof(ITouchButton));
				if (actualRunButton == null) Debug.LogWarning ("Couldn't find a ITouchButton on the run button GameObject");
			}
			if (pauseButtonGo != null )
			{
				actualPauseButton = (ITouchButton)pauseButtonGo.GetComponent (typeof(ITouchButton));
				if (actualPauseButton == null) Debug.LogWarning ("Couldn't find a ITouchButton on the pause button GameObject");
			}

			actualActionButtons = new ITouchButton[actionButtonGos.Length];
			for (int i = 0; i < actionButtonGos.Length; i++)
			{
				if (actionButtonGos[i] != null)
				{
					actualActionButtons[i] = (ITouchButton)actionButtonGos[i].GetComponent (typeof(ITouchButton));
					if (actualActionButtons[i] == null) Debug.LogWarning ("Couldn't find a ITouchButton on the action button GameObject for button " + i);
				}
			}
		}

		/// <summary>
		/// Unity LateUpdate() hook.
		/// </summary>
		void LateUpdate()
		{
			if (PauseButton == ButtonState.DOWN) 
			{
				TimeManager.Instance.TogglePause(false);
			}
			lastDigitalHorizontalAxisState = HorizontalAxisDigital;
			lastDigitalVerticalAxisState = VerticalAxisDigital;
			//lastDigitalAltHorizontalAxisState = AltHorizontalAxisDigital;
			//lastDigitalAltVerticalAxisState = AltVerticalAxisDigital;
		}

		/// <summary>
		/// HorizontalAxis
		/// </summary>
		override public float HorizontalAxis
		{
			get
			{
				return actualHorizontalAxis.Value;
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
				float result = actualHorizontalAxis.Value;
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
				if (lastDigitalHorizontalAxisState <= 0 && HorizontalAxisDigital == 1) return ButtonState.DOWN;
				if (lastDigitalHorizontalAxisState >= 0 && HorizontalAxisDigital == -1) return ButtonState.DOWN;
				if (lastDigitalHorizontalAxisState != 0 && lastDigitalHorizontalAxisState == HorizontalAxisDigital) return ButtonState.HELD;
				if (lastDigitalHorizontalAxisState != 0 && HorizontalAxisDigital == 0) return ButtonState.UP;
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
				if (actualVerticalAxis == null ) return 0;
				return actualVerticalAxis.Value;
			}
		}
		
		/// <summary>
		/// VerticalAxisDigital
		/// </summary>
		override public int VerticalAxisDigital
		{
			get
			{
				if (actualVerticalAxis == null ) return 0;
				// Get horizontal axis value
				float result = actualVerticalAxis.Value;
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
				if (lastDigitalVerticalAxisState <= 0 && VerticalAxisDigital == 1) return ButtonState.DOWN;
				if (lastDigitalVerticalAxisState >= 0 && VerticalAxisDigital == -1) return ButtonState.DOWN;
				if (lastDigitalVerticalAxisState != 0 && lastDigitalVerticalAxisState == VerticalAxisDigital) return ButtonState.HELD;
				if (lastDigitalVerticalAxisState != 0 && VerticalAxisDigital == 0) return ButtonState.UP;
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
				return actualJumpButton.ButtonState;
			}
		}
		
		/// <summary>
		/// RunButton
		/// </summary>
		override public ButtonState RunButton
		{
			get
			{
				return (actualRunButton != null ? actualRunButton.ButtonState : ButtonState.NONE );
			}
		}
		
		/// <summary>
		/// PauseButton
		/// </summary>
		override public ButtonState PauseButton
		{
			get
			{
				return (actualPauseButton != null ? actualPauseButton.ButtonState : ButtonState.NONE );
			}
		}
		
		/// <summary>
		/// ActionButton
		/// </summary>
		override public ButtonState ActionButton
		{
			get
			{
				if (actualActionButtons.Length == 0) return ButtonState.NONE;
				return actualActionButtons[0].ButtonState;
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
			if (actualActionButtons.Length > 0 && buttonIndex >= 0 && actualActionButtons.Length > buttonIndex) result = actualActionButtons[buttonIndex].ButtonState;
			
			return result;
		}

		/// <summary>
		/// Returns true if any button or action key is being pressed.
		/// </summary>
		/// <value><c>true</c> if any key; otherwise, <c>false</c>.</value>
		override public bool AnyKey
		{
			get
			{ 
				// Just cycle all touches
				int touchCount = UnityEngine.Input.touchCount;
				for (int i = 0; i < touchCount; i++)
				{
					Touch currentTouch = UnityEngine.Input.touches[i];
					if (currentTouch.phase == TouchPhase.Began) return true;
				}
				return false;
			}
		}
		
		/// <summary>
		/// Sets the keyboayrd key that corresponds to a Platform PRO input key.
		/// </summary>
		/// <returns><c>true</c>, if key was set, <c>false</c> otherwise.</returns>
		/// <param name="type">Type of key being set.</param>
		/// <param name="keyCode">Unity key code.</param>
		override public bool SetKey(KeyType type, KeyCode keyCode) 
		{
			Debug.LogError ("BasicTouchInput does not allow for in game control configuration.");
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
			Debug.LogError ("BasicTouchInput does not allow for in game control configuration.");
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
			Debug.LogError ("BasicTouchInput does not allow for in game control configuration.");
			return KeyCode.None;
		}
		
		/// <summary>
		/// Gets the name of the axis for the given key type.
		/// </summary>
		/// <returns>The axis name.</returns>
		/// <param name="type">Type.</param>
		override public string GetAxisForType (KeyType type)
		{
			Debug.LogError ("BasicTouchInput does not allow for in game control configuration.");
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
			Debug.LogError ("BasicTouchInput does not allow for in game control configuration.");
			return false;
		}
		
		/// <summary>
		/// Saves the input data.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		override public bool SaveInputData()
		{
			Debug.LogError ("BasicTouchInput does not allow for in game control configuration.");
			return false;
		}
		
		/// <summary>
		/// Loads the input data.
		/// </summary>
		/// <returns><c>true</c>, if input data was loaded, <c>false</c> otherwise.</returns>
		/// <param name="dataName">Data name.</param>
		override public bool LoadInputData(string dataName)
		{
			Debug.LogError ("BasicTouchInput does not allow for in game control configuration.");
			return false;
		}
		
		/// <summary>
		/// Loads the provided input data.
		/// </summary>
		/// <param name="data">Data to load.</param>
		override public void LoadInputData(StandardInputData data)
		{
			Debug.LogError ("BasicTouchInput does not allow for in game control configuration.");
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
				Debug.LogError ("BasicTouchInput does not support the alternate axis.");
				return 0.0f;
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
				Debug.LogError ("BasicTouchInput does not support the alternate axis.");
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
				Debug.LogError ("BasicTouchInput does not support the alternate axis.");
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
				Debug.LogError ("BasicTouchInput does not support the alternate axis.");
				return 0.0f;
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
				Debug.LogError ("BasicTouchInput does not support the alternate axis.");
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
				Debug.LogError ("BasicTouchInput does not support the alternate axis.");
				return ButtonState.NONE;
			}
		}
		
	}
}
