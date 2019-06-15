using UnityEngine;
using System.Collections;


namespace PlatformerPro
{
	/// <summary>
	/// An input that buffers the jump, run and action buttons from another input so that the controls can be more forgiving to the user.
	/// </summary>
	public class BufferedInput : Input
	{
		/// <summary>
		/// The input to buffer.
		/// </summary>
		[Tooltip("The input to buffer.")]
		public Input input;

		/// <summary>
		/// Time for jump button buffering in seconds.
		/// </summary>
		[Tooltip ("Time for jump button buffering in seconds.")]
		public float jumpBufferTime = 0.15f;

		/// <summary>
		/// Time for run button buffering in seconds.
		/// </summary>
		[Tooltip ("Time for run button buffering in seconds.")]
		public float runBufferTime;

		/// <summary>
		/// Time for action button buffering in seconds.
		/// </summary>
		[Tooltip ("Time for action button buffering in seconds. Index are aligned with the action button array.")]
		public float[] actionButtonBufferTimes;

		/// <summary>
		/// Tracks time since jump button pressed.
		/// </summary>
		protected float jumpButtonTimer;

		/// <summary>
		/// Tracks time since run button pressed.
		/// </summary>
		protected float runButtonTimer;

		/// <summary>
		/// Tracks times since action buttons pressed.
		/// </summary>
		protected float[] actionButtonTimers;

		#region Unity hooks

		/// <summary>
		/// Unity Start() hook.
		/// </summary>
		void Start()
		{
			Init ();
		}

		/// <summary>
		/// Unity Update() hook.
		/// </summary>
		void Update()
		{
			CheckButtons ();
		}

		#endregion


		#region proteted methods

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void Init()
		{
			// Try to find an input we should buffer.
			if (input == null)
			{
				Input[] potentialInputs = GetComponentsInChildren<Input>();
				foreach (Input potentialInput in potentialInputs)
				{
					// Prefer multi input over all others
					if (potentialInput is MultiInput) 
					{
						input = potentialInput;
						break;
					}
				}
				// If we still haven't found an input use anything that isn't this
				if (input == null)
				{
					foreach (Input potentialInput in potentialInputs)
					{
						if (potentialInput != this) 
						{
							input = potentialInput;
							break;
						}
					}
				}
				if (input == null) Debug.LogWarning("A BufferedInput must have an input defined or an input as a child.");
			}
			actionButtonTimers = new float[actionButtonBufferTimes.Length];
		}

		/// <summary>
		/// Chec button states in order to update buffer.
		/// </summary>
		virtual protected void CheckButtons()
		{
			// Jump button
			if (jumpButtonTimer > 0) jumpButtonTimer -= TimeManager.FrameTime;
			if (jumpBufferTime > 0)
			{
				if (input.JumpButton == ButtonState.DOWN) jumpButtonTimer = jumpBufferTime;
			}
			// Run button
			if (runButtonTimer > 0) runButtonTimer -= TimeManager.FrameTime;
			if (runBufferTime > 0)
			{
				if (input.RunButton == ButtonState.DOWN) runButtonTimer = runBufferTime;
			}
			// Action buttons
			for (int i = 0; i < actionButtonBufferTimes.Length; i++)
			{
				if (actionButtonTimers[i] > 0) actionButtonTimers[i] -= TimeManager.FrameTime;
				if (actionButtonBufferTimes[i] > 0)
				{
					if (input.GetActionButtonState(i) == ButtonState.DOWN) actionButtonTimers[i] = actionButtonBufferTimes[i];
				}
			}
		}

		#endregion

		#region Input Methods

		/// <summary>
		/// Get the state of an action button.
		/// </summary>
		/// <returns>The buttons state.</returns>
		/// <param name="buttonIndex">The index of the button.</param>
		override public ButtonState GetActionButtonState (int buttonIndex)
		{
			if (actionButtonTimers.Length > buttonIndex && actionButtonTimers[buttonIndex] > 0) return ButtonState.DOWN;
			return input.GetActionButtonState (buttonIndex);
		}

		/// <summary>
		/// Sets the joystick axis that corresponds to a Platform PRO input axis.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="type">Type of key being set.</param>
		/// <param name="axis">Unity axis name.</param>
		/// <param name="reverseAxis">Should axis values be reversed.</param>
		override public bool SetAxis (KeyType type, string axis, bool reverseAxis)
		{
			Debug.LogWarning ("BufferedInput should not be updated directly, update the underlying input implementation.");
			return false;
		}

		/// <summary>
		/// Sets the keyboayrd key that corresponds to a Platform PRO input key.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="type">Type of key being set.</param>
		/// <param name="keyCode">Unity key code.</param>
		override public bool SetKey (KeyType type, KeyCode keyCode)
		{
			Debug.LogWarning ("BufferedInput should not be updated directly, update the underlying input implementation.");
			return false;
		}

		/// <summary>
		/// Sets the keyboayrd key that corresponds to a Platform PRO input key.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="type">Type of key being set.</param>
		/// <param name="keyNumber">The action key number or ignored if not setting an action key</param>
		/// <param name="keyCode">Key code.</param>
		override public bool SetKey (KeyType type, KeyCode keyCode, int keyNumber)
		{
			Debug.LogWarning ("BufferedInput should not be updated directly, update the underlying input implementation.");
			return false;
		}

		/// <summary>
		/// Gets the key code for the given type.
		/// </summary>
		/// <returns>The mathcing KeyCode or keyCode.None if there is no match.</returns>
		/// <param name="type">Key type.</param>
		/// <param name="keyNumber">Key number if this is an action key (ignored otherwise).</param>
		override public KeyCode GetKeyForType (KeyType type, int keyNumber)
		{
			return input.GetKeyForType (type, keyNumber);
		}

		/// <summary>
		/// Gets the name of the axis for the given key type.
		/// </summary>
		/// <returns>The axis name.</returns>
		/// <param name="type">Type.</param>
		override public string GetAxisForType (KeyType type)
		{
			return input.GetAxisForType (type);
		}

		/// <summary>
		/// Saves the input data.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		override public bool SaveInputData ()
		{
			Debug.LogWarning ("BufferedInput should not be updated directly, update the underlying input implementation.");
			return false;
		}

		/// <summary>
		/// Loads the input data for the given data name.
		/// </summary>
		/// <param name="dataName">Data to load.</param>
		override public bool LoadInputData (string dataName)
		{
			Debug.LogWarning ("BufferedInput should not be updated directly, update the underlying input implementation.");
			return false;
		}

		/// <summary>
		/// Loads the input data for the given data name.
		/// </summary>
		/// <param name="dataName">Data to load.</param>
		/// <param name="data">Data.</param>
		override public void LoadInputData (StandardInputData data)
		{
			Debug.LogWarning ("BufferedInput should not be updated directly, update the underlying input implementation.");
		}

		/// <summary>
		/// A float value clamped between -1 for completely left and 1 for compeletely right.
		/// 0.5f would mean "half right". The exact interpretation of the values is up to the
		/// movement behaviours.
		/// </summary>
		/// <value>The horizontal axis.</value>
		override public float HorizontalAxis
		{
			get
			{
				return input.HorizontalAxis;
			}
		}

		/// <summary>
		/// A "digital" version of the horizontal axis in which the only valid values are -1 for LEFT, 
		/// 0 for NONE, and 1 for RIGHT.
		/// </summary>
		/// <value>The horizontal axis digital.</value>
		override public int HorizontalAxisDigital
		{
			get
			{
				return input.HorizontalAxisDigital;
			}
		}

		/// <summary>
		/// Gets the state of the horizontal axis.
		/// </summary>
		/// <value>The state of the horizontal axis.</value>
		override public ButtonState HorizontalAxisState
		{
			get
			{
				return input.HorizontalAxisState;
			}
		}

		/// <summary>
		/// A float value clamped between -1 for completely DOWN and 1 for completely UP.
		/// 0.5f would mean "half up". The exact interpretation of the values is up to the
		/// movement behaviours.
		/// </summary>
		/// <value>The vertical axis.</value>
		override public float VerticalAxis
		{
			get
			{
				return input.VerticalAxis;
			}
		}

		/// <summary>
		/// A "digital" version of the alternate vertical axis in which the only valid values are -1 for DOWN, 
		/// 0 for NONE, and 1 for UP.
		/// </summary>
		/// <value>The vertical axis digital.</value>
		override public int VerticalAxisDigital
		{
			get
			{
				return input.VerticalAxisDigital;
			}
		}

		/// <summary>
		/// Gets the state of the vertical axis.
		/// </summary>
		/// <value>The state of the vertical axis.</value>
		override public ButtonState VerticalAxisState 
		{
			get
			{
				return input.VerticalAxisState;
			}
		}

		/// <summary>
		/// A float value clamped between -1 for completely left and 1 for compeletely right.
		/// 0.5f would mean "half right". The exact interpretation of the values is up to the
		/// movement behaviours.
		/// </summary>
		/// <value>The alternate horizontal axis.</value>
		override public float AltHorizontalAxis
		{
			get
			{
				return input.AltHorizontalAxis;
			}
		}

		/// <summary>
		/// A "digital" version of the alternate horizontal axis in which the only valid values are -1 for LEFT, 
		/// 0 for NONE, and 1 for RIGHT.
		/// </summary>
		/// <value>The alternate horizontal axis digital.</value>
		override public int AltHorizontalAxisDigital
		{
			get
			{
				return input.AltHorizontalAxisDigital;
			}
		}

		/// <summary>
		/// Gets the state of the alternate horizontal axis.
		/// </summary>
		/// <value>The state of the alternate horizontal axis.</value>
		override public ButtonState AltHorizontalAxisState
		{
			get
			{
				return input.AltHorizontalAxisState;
			}
		}

		/// <summary>
		/// A float value clamped between -1 for completely DOWN and 1 for completely UP.
		/// 0.5f would mean "half up". The exact interpretation of the values is up to the
		/// movement behaviours.
		/// </summary>
		/// <value>The alternate vertical axis.</value>
		override public float AltVerticalAxis
		{
			get
			{
				return input.AltVerticalAxis;
			}
		}

		/// <summary>
		/// A "digital" version of the vertical axis in which the only valid values are -1 for DOWN, 
		/// 0 for NONE, and 1 for UP.
		/// </summary>
		/// <value>The alternate vertical axis digital.</value>
		override public int AltVerticalAxisDigital
		{
			get
			{
				return input.AltVerticalAxisDigital;
			}
		}

		/// <summary>
		/// Gets the state of the alternate vertical axis.
		/// </summary>
		/// <value>The state of the alternate vertical axis.</value>
		override public ButtonState AltVerticalAxisState
		{
			get
			{
				return input.AltVerticalAxisState;
			}
		}

		/// <summary>
		/// State of the jump button.
		/// </summary>
		/// <value>The jump button.</value>
		override public ButtonState JumpButton
		{
			get
			{
				if (jumpButtonTimer > 0) return ButtonState.DOWN;
				return input.JumpButton;
			}
		}

		/// <summary>
		/// State of the run button.
		/// </summary>
		/// <value>The run button.</value>
		override public ButtonState RunButton
		{
			get
			{
				if (runButtonTimer > 0) return ButtonState.DOWN;
				return input.RunButton;
			}
		}

		/// <summary>
		/// State of the pause button.
		/// </summary>
		/// <value>The pause button.</value>
		override public ButtonState PauseButton
		{
			get 
			{
				return input.PauseButton;
			}
		}

		/// <summary>
		/// Gets the action button.
		/// </summary>
		/// <value>The action button.</value>
		override public ButtonState ActionButton {
			get
			{
				if (actionButtonTimers.Length > 0 && actionButtonTimers[0] > 0) return ButtonState.DOWN;
				return input.ActionButton;
			}
		}

		/// <summary>
		/// Returns true if any button or action key is being pressed.
		/// </summary>
		/// <value>true</value>
		/// <c>false</c>
		override public bool AnyKey
		{
			get
			{
				return input.AnyKey;
			}
		}

		#endregion

	}
}