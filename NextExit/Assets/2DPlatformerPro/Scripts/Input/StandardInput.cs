#define RPGSTYLEINPUT

using UnityEngine;
#if !UNITY_4_6 && !UNITY_5_1 && !UNITY_5_2
using UnityEngine.SceneManagement;
#endif
using System.Collections;
using System.IO;

namespace PlatformerPro
{
	/// <summary>
	/// The standard input with reassignable buttons and a set of default key assignments (arrows + z, x, and c).
	/// Also handles controllers and user configurable inputs.
	/// </summary>
	public class StandardInput : Input
	{

		/// <summary>
		/// Name of the input data file to load by default.
		/// </summary>
		[HideInInspector]
		public string dataToLoad = "Player1";

		#region Member Variables - Standard Input Data

		[Header ("General")]
		/// <summary>
		/// Should the controller input be enabled.
		/// </summary>
		public bool enableController = true;
		
		/// <summary>
		/// Should the keyboard input be enabled.
		/// </summary>
		public bool enableKeyboard = true;

		[Header ("Controllers")]
		/// <summary>
		/// The horizontal controller axis.
		/// </summary>
		public string horizontalAxisName = "Joystick1Axis1";

		/// <summary>
		/// Should we reverse the values of the horizontal axis.
		/// </summary>
		public bool reverseHorizontalAxis;

		/// <summary>
		/// Threshold for digital input to be considered non-zero.
		/// </summary>
		public float digitalHorizontalThreshold = 0.25f;

		/// <summary>
		/// The vertical controller axis.
		/// </summary>
		public string verticalAxisName = "Joystick1Axis2";

		/// <summary>
		/// Should we reverse the values of the vertival axis.
		/// </summary>
		public bool reverseVerticalAxis;

		/// <summary>
		/// Threshold for digital input to be considered non-zero.
		/// </summary>
		public float digitalVerticalThreshold = 0.25f;

		/// <summary>
		/// The horizontal controller axis.
		/// </summary>
		public string altHorizontalAxisName = "Joystick1Axis7";
		
		/// <summary>
		/// Should we reverse the values of the horizontal axis.
		/// </summary>
		public bool reverseAltHorizontalAxis;
		
		/// <summary>
		/// Threshold for digital input to be considered non-zero.
		/// </summary>
		public float digitalAltHorizontalThreshold = 0.25f;
		
		/// <summary>
		/// The alternateVertical controller axis.
		/// </summary>
		public string altVerticalAxisName = "Joystick1Axis8";
		
		/// <summary>
		/// Should we reverse the values of the alternate vertival axis.
		/// </summary>
		public bool reverseAltVerticalAxis;

		/// <summary>
		/// Threshold for digital input to be considered non-zero.
		/// </summary>
		public float digitalAltVerticalThreshold = 0.25f;

		[Header ("Keyboard and Buttons")]
		/// <summary>
		/// The right key.
		/// </summary>
		public KeyCode right = KeyCode.RightArrow;

		/// <summary>
		/// The left key.
		/// </summary>
		public KeyCode left = KeyCode.LeftArrow;

		/// <summary>
		/// The up key.
		/// </summary>
		public KeyCode up = KeyCode.UpArrow;

		/// <summary>
		/// The down key.
		/// </summary>
		public KeyCode down = KeyCode.DownArrow;

		/// <summary>
		/// The jump key.
		/// </summary>
		public KeyCode jump = KeyCode.Z;

		/// <summary>
		/// The run key.
		/// </summary>
		public KeyCode run = KeyCode.X;
	
		/// <summary>
		/// The pause key.
		/// </summary>
		public KeyCode pause = KeyCode.P;

		/// <summary>
		///  The action buttons with the first value in the array being the default.
		/// </summary>
		public KeyCode[] actionButtons;

		#endregion

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
		protected int lastDigitalAltVerticalAxisState;
		
		/// <summary>
		/// Stores the last state of the alternate horizontal axis (digital).
		/// </summary>
		protected int lastDigitalAltHorizontalAxisState;


		/// <summary>
		/// Constant used as a prefix for saving and loading input configuration from player prefs.
		/// </summary>
		public const string SavedPreferencePrefix = "PP.Input.";

		#region Unity Hooks

		void Awake()
		{
			PreInit ();
		}
		
		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void LateUpdate() 
		{
#if UNITY_EDITOR
			// Press R (reset) to reload current scene in editor.
			if (UnityEngine.Input.GetKeyDown(KeyCode.R)) 
			#if !UNITY_4_6 && !UNITY_5_1 && !UNITY_5_2
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			#else
			Application.LoadLevel(Application.loadedLevel);
			#endif

#endif
			if (PauseButton == ButtonState.DOWN) 
			{
				TimeManager.Instance.TogglePause(false);
			}

			lastDigitalHorizontalAxisState = HorizontalAxisDigital;
			lastDigitalVerticalAxisState = VerticalAxisDigital;
			lastDigitalAltHorizontalAxisState = AltHorizontalAxisDigital;
			lastDigitalAltVerticalAxisState = AltVerticalAxisDigital;
		}

		#endregion

		#region methods

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void PreInit()
		{
			if (dataToLoad != null && dataToLoad != "")
			{
				LoadInputData(dataToLoad);
			}
		}

		/// <summary>
		/// Loads the input data from the given player prefs.
		/// </summary>
		/// <returns><c>true</c>, if input data was loaded, <c>false</c> otherwise.</returns>
		/// <param name="dataName">Player prefs name (constant prefix will be added too).</param>
		override public bool LoadInputData(string dataName)
		{
			string prefsName = SavedPreferencePrefix + dataName;
			string data = PlayerPrefs.GetString (prefsName, "");
			StandardInputData result;
			if (data.Length > 0)
			{
				try 
				{
					System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(StandardInputData));
					using (StringReader stream = new StringReader(data)) {
						result = (StandardInputData)serializer.Deserialize(stream);
						SetInputData(result);
						return true;
					}
				}
				catch (System.IO.IOException e)
				{
					Debug.LogError("Error loading input data:" + e.Message);
				}
			}
			return false;
		}

		/// <summary>
		/// Loads the provided input data.
		/// </summary>
		/// <param name="data">Data to load.</param>
		override public void LoadInputData(StandardInputData data)
		{
			SetInputData (data);
		}

		/// <summary>
		/// Saves the input data.
		/// </summary>
		/// <returns><c>true</c>, if input data was saved, <c>false</c> otherwise.</returns>
		/// <param name="dataName">Data name.</param>
		virtual public bool SaveInputData(string dataName)
		{
			string prefsName = SavedPreferencePrefix + dataName;
			System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(StandardInputData));
			StandardInputData dataToSave = GetInputData();
			try 
			{
				using (var stream = new StringWriter()) {
					serializer.Serialize(stream, dataToSave);
					PlayerPrefs.SetString(prefsName, stream.ToString());
				}
			}
			catch (System.IO.IOException e)
			{
				Debug.LogError("Error saving input data:" + e.Message);
				return false;
			}	
			return true;
		}
		
		/// <summary>
		/// Sets this input to reflect the provided data object.
		/// </summary>
		/// <param name="data">Data.</param>
		virtual protected void SetInputData(StandardInputData data)
		{
			enableController = data.enableController;
			enableKeyboard = data.enableKeyboard;
			horizontalAxisName = data.horizontalAxisName;
			reverseHorizontalAxis = data.reverseHorizontalAxis;
			digitalHorizontalThreshold = data.digitalHorizontalThreshold;
			verticalAxisName = data.verticalAxisName;
			reverseVerticalAxis = data.reverseVerticalAxis;
			digitalVerticalThreshold = data.digitalVerticalThreshold;

			altHorizontalAxisName = data.altHorizontalAxisName;
			reverseAltHorizontalAxis = data.reverseAltHorizontalAxis;
			digitalAltHorizontalThreshold = data.digitalAltHorizontalThreshold;
			altVerticalAxisName = data.altVerticalAxisName;
			reverseAltVerticalAxis = data.reverseAltVerticalAxis;
			digitalAltVerticalThreshold = data.digitalAltVerticalThreshold;

			right = data.right;
			left = data.left;
			up = data.up;
			down = data.down;
			jump = data.jump;
			run = data.run;
			pause = data.pause;
			actionButtons = new KeyCode[data.actionButtons.Length];
			for (int i = 0; i < data.actionButtons.Length; i++) actionButtons[i] = data.actionButtons[i];
		}
		
		
		/// <summary>
		/// Sets this input to reflect the provided data object.
		/// </summary>
		/// <param name="data">Data.</param>
		virtual public StandardInputData GetInputData()
		{
			StandardInputData data = new StandardInputData ();
			data.enableController = enableController;
			data.enableKeyboard = enableKeyboard;
			data.horizontalAxisName = horizontalAxisName;
			data.reverseHorizontalAxis = reverseHorizontalAxis;
			data.digitalHorizontalThreshold = digitalHorizontalThreshold;
			data.verticalAxisName = verticalAxisName;
			data.reverseVerticalAxis = reverseVerticalAxis;
			data.digitalVerticalThreshold = digitalVerticalThreshold;

			data.altHorizontalAxisName = altHorizontalAxisName;
			data.reverseAltHorizontalAxis = reverseAltHorizontalAxis;
			data.digitalAltHorizontalThreshold = digitalAltHorizontalThreshold;
			data.altVerticalAxisName = altVerticalAxisName;
			data.reverseAltVerticalAxis = reverseAltVerticalAxis;
			data.digitalAltVerticalThreshold = digitalAltVerticalThreshold;

			data.right = right;
			data.left = left;
			data.up = up;
			data.down = down;
			data.jump = jump;
			data.run = run;
			data.pause = pause;
			// Assume there are always 8 action buttons (no harm if they aren't used). If we don't do this
			// saving data with less buttons can cause issues which aren't immediately obvious.
			data.actionButtons = new KeyCode[8];
			for (int i = 0; i < data.actionButtons.Length; i++) 
			{
				if (i < actionButtons.Length) data.actionButtons[i] = actionButtons[i];
				else data.actionButtons[i] = KeyCode.None;
			}
			return data;
		}

		#endregion

		#region Methods for Input Interface

		/// <summary>
		/// A float value clamped between -1 for completely left and 1 for compeletely right.
		/// 0.5f would mean "half right". This implementation only returns -1.0f, 0.0f or 1.0f.
		/// </summary>
		override public float HorizontalAxis
		{
			get
			{
				// Controller
				if (enableController)
				{
					if (horizontalAxisName != null && horizontalAxisName != "" && horizontalAxisName != "none")
					{
						float axisValue = UnityEngine.Input.GetAxis(horizontalAxisName);
						if (axisValue > 0 || axisValue < 0) return reverseHorizontalAxis ? -axisValue : axisValue;
					}
				}
				if (enableKeyboard)
				{
					// Both down return 0
					if (UnityEngine.Input.GetKey(right) && UnityEngine.Input.GetKey(left)) return 0.0f;
					// Right
					if (UnityEngine.Input.GetKey(right)) return 1.0f;
					// Left
					if (UnityEngine.Input.GetKey(left)) return -1.0f;
				}
				// Otherwise 0
				return 0.0f;
			}
		}
		
		/// <summary>
		/// A "digital" version of the horizontal axis in which the only valid values are -1 for LEFT, 
		/// 0 for NONE, and 1 for RIGHT.
		/// </summary>
		override public int HorizontalAxisDigital
		{
			get
			{
#if RPGSTYLEINPUT
                return PlatformerInputManager.Instance.HorizontalAxisDigital;
#else
				// Controller
				if (enableController)
				{
					if (horizontalAxisName != null && horizontalAxisName != "" && horizontalAxisName != "none")
					{
						float axisValue = UnityEngine.Input.GetAxis(horizontalAxisName);
						if (axisValue > digitalHorizontalThreshold) return reverseHorizontalAxis ? -1 : 1;
						if (axisValue < -digitalHorizontalThreshold) return reverseHorizontalAxis ? 1 : -1;
					}
				}
				if (enableKeyboard)
				{
					// Both down return 0
					if (UnityEngine.Input.GetKey(right) && UnityEngine.Input.GetKey(left)) return 0;
					// Right
					if (UnityEngine.Input.GetKey(right)) return 1;
					// Left
					if (UnityEngine.Input.GetKey(left)) return -1;
				}
				// Otherwise 0
				return 0;
#endif
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
		/// A float value clamped between -1 for completely DOWN and 1 for completely UP.
		/// 0.5f would mean "half up". The exact interpretation of the values is up to the
		/// movement behaviours.
		/// </summary>
		override public float VerticalAxis
		{
			get
			{
				// Controller
				if (enableController)
				{
					if (verticalAxisName != null && verticalAxisName != "" && verticalAxisName != "none")
					{
						float axisValue = UnityEngine.Input.GetAxis(verticalAxisName);
						if (axisValue > 0 || axisValue < 0) return reverseVerticalAxis ? -axisValue :  axisValue;
					}
				}
				if (enableKeyboard)
				{
					// Both down return 0
					if (UnityEngine.Input.GetKey(up) && UnityEngine.Input.GetKey(down)) return 0.0f;
					// Right
					if (UnityEngine.Input.GetKey(up)) return 1.0f;
					// Left
					if (UnityEngine.Input.GetKey(down)) return -1.0f;
				}
				// Otherwise 0
				return 0.0f;
			}
		}
		
		/// <summary>
		/// A "digital" version of the vertical axis in which the only valid values are -1 for DOWN, 
		/// 0 for NONE, and 1 for UP.
		/// </summary>
		override public int VerticalAxisDigital
		{
			get
			{
				// Controller
				if (enableController)
				{
					if (verticalAxisName != null && verticalAxisName != "" && verticalAxisName != "none")
					{
						float axisValue = UnityEngine.Input.GetAxis(verticalAxisName);
						if (axisValue > digitalVerticalThreshold) return reverseVerticalAxis ? -1 : 1;
						if (axisValue < -digitalVerticalThreshold) return reverseVerticalAxis ? 1 : -1;
					}
				}
				if (enableKeyboard)
				{
					// Both down return 0
					if (UnityEngine.Input.GetKey(up) && UnityEngine.Input.GetKey(down)) return 0;
					// Right
					if (UnityEngine.Input.GetKey(up)) return 1;
					// Left
					if (UnityEngine.Input.GetKey(down)) return -1;
				}
				// Otherwise 0
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
		/// A float value clamped between -1 for completely left and 1 for compeletely right.
		/// 0.5f would mean "half right". This implementation only returns -1.0f, 0.0f or 1.0f.
		/// </summary>
		override public float AltHorizontalAxis
		{
			get
			{
				// Controller
				if (enableController)
				{
					if (altHorizontalAxisName != null && altHorizontalAxisName != "" && altHorizontalAxisName != "none")
					{
						float axisValue = UnityEngine.Input.GetAxis(altHorizontalAxisName);
						if (axisValue > 0 || axisValue < 0) return reverseAltHorizontalAxis ? -axisValue : axisValue;
					}
				}

				// Otherwise 0
				return 0.0f;
			}
		}
		
		/// <summary>
		/// A "digital" version of the horizontal axis in which the only valid values are -1 for LEFT, 
		/// 0 for NONE, and 1 for RIGHT.
		/// </summary>
		override public int AltHorizontalAxisDigital
		{
			get
			{
				// Controller
				if (enableController)
				{
					if (altHorizontalAxisName != null && altHorizontalAxisName != "" && altHorizontalAxisName != "none")
					{
						float axisValue = UnityEngine.Input.GetAxis(altHorizontalAxisName);
						if (axisValue > digitalAltHorizontalThreshold) return reverseAltHorizontalAxis ? -1 : 1;
						if (axisValue < -digitalAltHorizontalThreshold) return reverseAltHorizontalAxis ? 1 : -1;
					}
				}
				// Otherwise 0
				return 0;
			}
		}
		
		/// <summary>
		/// Return ButtonState.DOWN if the axis state went from <= 0 to 1 or >= 0 to -1.
		/// Return ButtonState.HELD if the axis stayed at the same value and wasn't 0;
		/// Return ButtonState.UP if the axis state went from != 0 to 0.
		/// </summary>
		override public ButtonState AltHorizontalAxisState
		{
			get
			{
				if (lastDigitalAltHorizontalAxisState <= 0 && AltHorizontalAxisDigital == 1) return ButtonState.DOWN;
				if (lastDigitalAltHorizontalAxisState >= 0 && AltHorizontalAxisDigital == -1) return ButtonState.DOWN;
				if (lastDigitalAltHorizontalAxisState != 0 && lastDigitalAltHorizontalAxisState == AltHorizontalAxisDigital) return ButtonState.HELD;
				if (lastDigitalAltHorizontalAxisState != 0 && AltHorizontalAxisDigital == 0) return ButtonState.UP;
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
				// Controller
				if (enableController)
				{
					if (altVerticalAxisName != null && altVerticalAxisName != "" && altVerticalAxisName != "none")
					{
						float axisValue = UnityEngine.Input.GetAxis(altVerticalAxisName);
						if (axisValue > 0 || axisValue < 0) return reverseAltVerticalAxis ? -axisValue :  axisValue;
					}
				}
				// Otherwise 0
				return 0.0f;
			}
		}
		
		/// <summary>
		/// A "digital" version of the alternate vertical axis in which the only valid values are -1 for DOWN, 
		/// 0 for NONE, and 1 for UP.
		/// </summary>
		override public int AltVerticalAxisDigital
		{
			get
			{
				// Controller
				if (enableController)
				{
					if (altVerticalAxisName != null && altVerticalAxisName != "" && altVerticalAxisName != "none")
					{
						float axisValue = UnityEngine.Input.GetAxis(altVerticalAxisName);
						if (axisValue > digitalAltVerticalThreshold) return reverseAltVerticalAxis ? -1 : 1;
						if (axisValue < -digitalAltVerticalThreshold) return reverseAltVerticalAxis ? 1 : -1;
					}
				}
				// Otherwise 0
				return 0;
			}
		}
		
		/// <summary>
		/// Return ButtonState.DOWN if the axis state went from <= 0 to 1 or >= 0 to -1.
		/// Return ButtonState.HELD if the axis stayed at the same value and wasn't 0;
		/// Return ButtonState.UP if the axis state went from != 0 to 0.
		/// </summary>
		override public ButtonState AltVerticalAxisState
		{
			get
			{
				if (lastDigitalAltVerticalAxisState <= 0 && AltVerticalAxisDigital == 1) return ButtonState.DOWN;
				if (lastDigitalAltVerticalAxisState >= 0 && AltVerticalAxisDigital == -1) return ButtonState.DOWN;
				if (lastDigitalAltVerticalAxisState != 0 && lastDigitalAltVerticalAxisState == AltVerticalAxisDigital) return ButtonState.HELD;
				if (lastDigitalAltVerticalAxisState != 0 && AltVerticalAxisDigital == 0) return ButtonState.UP;
				return ButtonState.NONE;
			}
		}

		/// <summary>
		/// State of the jump button.
		/// </summary>
		override public ButtonState JumpButton
		{
			get
			{
#if RPGSTYLEINPUT
                return PlatformerInputManager.Instance.JumpButton;
#else
				return GetStateForKey(jump);
#endif
			}
		}
		
		/// <summary>
		/// State of the run button.
		/// </summary>
		override public ButtonState RunButton
		{
			get
			{
				return GetStateForKey(run);
			}
		}
		
		/// <summary>
		/// State of the pause button.
		/// </summary>
		override public ButtonState PauseButton
		{
			get
			{
				return GetStateForKey(pause);
			}
		}

		/// <summary>
		/// State of the default action button. This could be pickup, attack, etc. If you need
		/// more buttons use the additional action use 
		/// <see cref="Input.GetActionButtonState()"/>
		/// </summary>
		override public ButtonState ActionButton
		{
			get
			{
				if (actionButtons.Length == 0) return ButtonState.NONE;
				return GetStateForKey(actionButtons[0]);
			}
		}
		
		/// <summary>
		/// Get the state of an action button.
		/// </summary>
		/// <returns>The buttons state.</returns>
		/// <param name="buttonIndex">The index of the button.</param>
		override public ButtonState GetActionButtonState(int buttonIndex)
		{
			if (buttonIndex == 0) return ActionButton;
			if (buttonIndex < actionButtons.Length) return GetStateForKey(actionButtons[buttonIndex]);
			return ButtonState.NONE;
		}

		/// <summary>
		/// Returns true if any button or action key is being held.
		/// </summary>
		/// <value><c>true</c> if any key; otherwise, <c>false</c>.</value>
		override public bool AnyKey
		{
			get 
			{
				return UnityEngine.Input.anyKeyDown;
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
			return SetKey (type, keyCode, 0);
		}
		
		/// <summary>
		/// Sets the keyboayrd key that corresponds to a Platform PRO input key.
		/// </summary>
		/// <returns><c>true</c>, if key was set, <c>false</c> otherwise.</returns>
		/// <param name="type">Type of key being set.</param>
		/// <param name="keyNumber">The action key number, ignored if not setting an action key.</param>
		override public bool SetKey(KeyType type, KeyCode keyCode, int keyNumber)
		{
			ClearKeyCode (keyCode);
			switch(type)
			{
			case KeyType.UP:
				up = keyCode;
				break;
			case KeyType.DOWN:
				down = keyCode;
				break;
			case KeyType.LEFT:
				left = keyCode;
				break;
			case KeyType.RIGHT:
				right = keyCode;
				break;
			case KeyType.JUMP:
				jump = keyCode;
				break;
			case KeyType.RUN:
				run = keyCode;
				break;
			case KeyType.PAUSE:
				pause = keyCode;
				break;
			case KeyType.ACTION:
				if (keyNumber >= 0 && actionButtons.Length > keyNumber) actionButtons[keyNumber] = keyCode;
				else
				{
					Debug.LogError ("Invalid action key index: " + keyNumber);
					return false;

				}
				break;
			}
			return true;
		}

		/// <summary>
		/// Sets any KeyType that has a matching KeyCode to KeyCode.NONE
		/// </summary>
		/// <param name="keyCode">The key code to match.</param>
		virtual protected void ClearKeyCode(KeyCode keyCode)
		{
			if (up == keyCode) up = KeyCode.None;
			if (down == keyCode) down = KeyCode.None;
			if (left == keyCode) left = KeyCode.None;
			if (right == keyCode) right = KeyCode.None;
			if (jump == keyCode) jump = KeyCode.None;
			if (run == keyCode) run = KeyCode.None;
			if (pause == keyCode) pause = KeyCode.None;
			for(int i = 0; i < actionButtons.Length; i++)
			{
				if (actionButtons[i] == keyCode) actionButtons[i] = KeyCode.None;
			}
		}

		/// <summary>
		/// Gets the button state for given keyboard key.
		/// </summary>
		/// <returns>The state for key.</returns>
		/// <param name="key">Key to check.</param>
		virtual protected ButtonState GetStateForKey(KeyCode key)
		{
			if (UnityEngine.Input.GetKeyDown(key)) return ButtonState.DOWN;
			if (UnityEngine.Input.GetKeyUp(key)) return ButtonState.UP;
			if (UnityEngine.Input.GetKey(key)) return ButtonState.HELD;
			return ButtonState.NONE;
		}

		/// <summary>
		/// Gets the key code for the given type.
		/// </summary>
		/// <returns>The mathcing KeyCode or keyCode.None if there is no match.</returns>
		/// <param name="type">Key type.</param>
		/// <param name="keyNumber">Key number if this is an action key (ignored otherwise).</param>
		override public KeyCode GetKeyForType(KeyType type, int keyNumber) 
		{
			if (type == KeyType.UP) return up;
			if (type == KeyType.DOWN) return down;
			if (type == KeyType.LEFT) return left;
			if (type == KeyType.RIGHT) return right;
			if (type == KeyType.JUMP) return jump;
			if (type == KeyType.RUN) return run;
			if (type == KeyType.PAUSE) return pause;
			if (type == KeyType.ACTION && keyNumber >= 0 && keyNumber < actionButtons.Length)return actionButtons[keyNumber]; 
			return KeyCode.None;
		}

		/// <summary>
		/// Gets the name of the axis for the given key type.
		/// </summary>
		/// <returns>The axis name.</returns>
		/// <param name="type">Type.</param>
		override public string GetAxisForType (KeyType type)
		{
			if (type == KeyType.VERTICAL_AXIS) return verticalAxisName;
			if (type == KeyType.HORIZONTAL_AXIS) return horizontalAxisName;
			return "None";
		}

		/// <summary>
		/// Sets the joystick axis that corresponds to a Platform PRO input axis.
		/// </summary>
		/// <returns><c>true</c>, if key was set, <c>false</c> otherwise.</returns>
		/// <param name="type">Type of key being set.</param>
		/// <param name="axis">Unity axis name.</param>
		/// <param name="reverseAxis">Should we flip the axis?</param>
		override public bool SetAxis(KeyType type, string axis, bool reverseAxis)
		{
			if (type == KeyType.HORIZONTAL_AXIS)
			{
				horizontalAxisName = axis;
				reverseHorizontalAxis = reverseAxis;
			}
			else if (type == KeyType.VERTICAL_AXIS) 
			{
				verticalAxisName = axis;
				reverseAltVerticalAxis = reverseAxis;
			}
			else if (type == KeyType.ALT_HORIZONTAL_AXIS)
			{
				altHorizontalAxisName = axis;
				reverseAltHorizontalAxis = reverseAxis;
			}
			else if (type == KeyType.ALT_VERTICAL_AXIS) 
			{
				altVerticalAxisName = axis;
				reverseVerticalAxis = reverseAxis;
			}
			else return false;
			return true;
		}

		/// <summary>
		/// Saves the input data usinf the default data name.
		/// </summary>
		/// <returns><c>true</c>, if input data was saved, <c>false</c> otherwise.</returns>
		override public bool SaveInputData()
		{
			return SaveInputData(dataToLoad);
		}

		#endregion

	}

}
