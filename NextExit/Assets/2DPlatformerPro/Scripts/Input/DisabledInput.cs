using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// An Input that is always off/disabled. Returns 0/NONE to everything except AnyKey and PauseButton.
	/// </summary>
	public class DisabledInput : Input
	{
		Input mainInput;

		public void Init(Input mainInput) {
			this.mainInput = mainInput;
		}

		public override ButtonState GetActionButtonState (int buttonIndex)
		{
			return ButtonState.NONE;
		}

		public override bool SetAxis (KeyType type, string axis, bool reverseAxis)
		{
			Debug.LogWarning ("You can't configure a disabled input");
			return false;
		}

		public override bool SetKey (KeyType type, KeyCode keyCode)
		{
			Debug.LogWarning ("You can't configure a disabled input");
			return false;
		}

		public override bool SetKey (KeyType type, KeyCode keyCode, int keyNumber)
		{
			Debug.LogWarning ("You can't configure a disabled input");
			return false;
		}

		public override KeyCode GetKeyForType (KeyType type, int keyNumber)
		{
			Debug.LogWarning ("You can't configure a disabled input");
			return KeyCode.None;
		}

		public override string GetAxisForType (KeyType type)
		{
			Debug.LogWarning ("You can't configure a disabled input");
			return null;
		}

		public override bool SaveInputData ()
		{
			Debug.LogWarning ("You can't configure a disabled input");
			return false;
		}

		public override bool LoadInputData (string dataName)
		{
			Debug.LogWarning ("You can't configure a disabled input");
			return false;
		}

		public override void LoadInputData (StandardInputData data)
		{
			Debug.LogWarning ("You can't configure a disabled input");
		}

		public override float HorizontalAxis {
			get {
				return 0.0f;
			}
		}

		public override int HorizontalAxisDigital {
			get {
				return 0;
			}
		}

		public override ButtonState HorizontalAxisState {
			get {
				return ButtonState.NONE;
			}
		}

		public override float VerticalAxis {
			get {
				return 0.0f;
			}
		}

		public override int VerticalAxisDigital {
			get {
				return 0;
			}
		}

		public override ButtonState VerticalAxisState {
			get {
				return ButtonState.NONE;
			}
		}

		public override float AltHorizontalAxis {
			get {
				return 0.0f;
			}
		}

		public override int AltHorizontalAxisDigital {
			get {
				return 0;
			}
		}

		public override ButtonState AltHorizontalAxisState {
			get {
				return ButtonState.NONE;
			}
		}

		public override float AltVerticalAxis {
			get {
				return 0.0f;
			}
		}

		public override int AltVerticalAxisDigital {
			get {
				return 0;
			}
		}

		public override ButtonState AltVerticalAxisState {
			get {
				return ButtonState.NONE;
			}
		}

		public override ButtonState JumpButton {
			get {
				return ButtonState.NONE;
			}
		}

		public override ButtonState RunButton {
			get {
				return ButtonState.NONE;
			}
		}

		public override ButtonState PauseButton {
			get {
				return mainInput.PauseButton;
			}
		}

		public override ButtonState ActionButton {
			get {
				return ButtonState.NONE;
			}
		}

		public override bool AnyKey {
			get {
				return mainInput.AnyKey;
			}
		}
	}
}
