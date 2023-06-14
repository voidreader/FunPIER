//#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR_OSX)
//#define ENABLE_HTINPUT
//#endif // UNITY_STANDALONE_WIN && !UNITY_EDITOR_OSX

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

#if ENABLE_HTINPUT
using System.Runtime.InteropServices;
#endif // ENABLE_HTINPUT

namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	/// <summary>
	/// Key의 State를 보관하고 있는 Class입니다.
	/// InputKey만 갖고있으면 자동 갱신되므로 HTInputManager에 매번 접근하지 않아도 됩니다.
	/// 직접 멤버 변수를 수정해서 다음 갱신 때 영향을 줄 수 있지만 권장되진 않습니다.
	/// </summary>
	[Serializable]
	public abstract class HTKey
	{
		//---------------------------------------
		[Header("KEY SETTING")]
		[SerializeField]
		private string _keyID = null;
		public string KeyID { get { return _keyID; } }

		[SerializeField]
		private string _keyNameLocale = null;
		public string KeyNameLocale { get { return _keyNameLocale; } }

		[SerializeField]
		private bool _updateOnMobile = false;
		public bool UpdateOnMobile { get { return _updateOnMobile; } }

		//---------------------------------------
		private eButtonState _buttonState = eButtonState.Free;
		public eButtonState ButtonState
		{
			get { return _buttonState; }
			set { _buttonState = value; }
		}

		public bool IsFree { get { return (_buttonState == eButtonState.Free) ? true : false; } }
		public bool IsDown { get { return (_buttonState == eButtonState.Down) ? true : false; } }
		public bool IsHold { get { return (_buttonState == eButtonState.Hold) ? true : false; } }
		public bool IsUp { get { return (_buttonState == eButtonState.Up) ? true : false; } }

		//---------------------------------------
		public void Update()
		{
			if (HTAfxPref.IsMobilePlatform && _updateOnMobile == false)
				return;

			if (UpdateKeyState())
			{
				if (ButtonState == eButtonState.Down || ButtonState == eButtonState.Hold)
					ButtonState = eButtonState.Hold;
				else
					ButtonState = eButtonState.Down;
			}
			else
			{
				if (ButtonState == eButtonState.Down || ButtonState == eButtonState.Hold)
					ButtonState = eButtonState.Up;
				else
					ButtonState = eButtonState.Free;
			}
		}

		protected abstract bool UpdateKeyState();

		//---------------------------------------
	}

	//---------------------------------------
	/// <summary>
	/// Key의 State를 보관하고 있는 Class입니다.
	/// InputKey만 갖고있으면 자동 갱신되므로 HTInputManager에 매번 접근하지 않아도 됩니다.
	/// 직접 멤버 변수를 수정해서 다음 갱신 때 영향을 줄 수 있지만 권장되진 않습니다.
	/// </summary>
	[Serializable]
	public class HTInputKey : HTKey
	{
		//---------------------------------------
		[Header("INPUT KEY SETTING")]
		[SerializeField]
		private eAxisButtonType _axisType = eAxisButtonType.None;
		public eAxisButtonType AxisType { get { return _axisType; } }

		//---------------------------------------
		private float _axisRatio = 0.0f;
		public float AxisRatio
		{
			get { return _axisRatio; }
			set { _axisRatio = value; }
		}

		//---------------------------------------
		protected override bool UpdateKeyState()
		{
			bool bIsDown = false;
			if (HTAfxPref.IsMobilePlatform)
				bIsDown = Input.GetButton(KeyID);

			else
			{
				if (_axisType != eAxisButtonType.None)
				{
					switch (_axisType)
					{
						case eAxisButtonType.AddAxis:
							_axisRatio = Input.GetAxis(KeyID + "_Axis");
							break;

						case eAxisButtonType.IsAxis:
							_axisRatio = Input.GetAxis(KeyID);
							break;
					}

					if (Mathf.Abs(_axisRatio) > 0.0f)
						bIsDown = true;
				}

				if (bIsDown == false)
					bIsDown = Input.GetButton(KeyID);
			}

			return bIsDown;
		}
	}

	//---------------------------------------
	[Serializable]
	public class HTMappedKey : HTKey
	{
		//---------------------------------------
		[Header("MAPPED KEY SETTING")]
		[SerializeField]
		private KeyCode _defaultKeyCode;
		public KeyCode DefaultKeyCode { get { return _defaultKeyCode; } }

		//---------------------------------------
		private bool _keyRemapped = false;
		public bool KeyRemapped { get { return _keyRemapped; } }

		private KeyCode _mappedKeyCode;
		public KeyCode MappedKeyCode { get { return _mappedKeyCode; } }

		public KeyCode CurrentKey { get { return (_keyRemapped) ? _mappedKeyCode : _defaultKeyCode; } }
		
		//---------------------------------------
		protected override bool UpdateKeyState()
		{
			if (CurrentKey == KeyCode.None)
				return false;

			return HTVirtualInput.GetKey(CurrentKey);
		}

		public void SetKeyMap(bool bSet, Action<HTMappedKey> onResetAnother, KeyCode eCode = KeyCode.Escape)
		{
			if (bSet && eCode == KeyCode.Escape)
				return;

			if (eCode == _defaultKeyCode || eCode == KeyCode.Escape)
				_keyRemapped = false;
			else
				_keyRemapped = bSet;

			//-----
			if (_keyRemapped)
				_mappedKeyCode = eCode;

			if (CurrentKey != KeyCode.None)
			{
				HTMappedKey[] vMappedKey = GEnv.Get()._mapped_Buttons;
				for (int nInd = 0; nInd < vMappedKey.Length; ++nInd)
				{
					if (vMappedKey[nInd] == this)
						continue;

					if (vMappedKey[nInd].CurrentKey == eCode)
					{
						vMappedKey[nInd].SetKeyMap(true, null, KeyCode.None);
						Utils.SafeInvoke(onResetAnother, vMappedKey[nInd]);
					}
				}
			}
		}
	}

	//---------------------------------------
	public class HTVirtualInput
	{
#if ENABLE_HTINPUT
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
		protected static extern short GetAsyncKeyState(int keyCode);

		// Mapper for Unity KeyCode to Virtual KeyCode with full key set
		private static int KeyCodeToVkeyFullSet(KeyCode key)
		{
			int VK = 0;
			switch (key)
			{
				case KeyCode.Backspace: VK = 0x08; break;
				case KeyCode.Tab: VK = 0x09; break;
				case KeyCode.Clear: VK = 0x0C; break;
				case KeyCode.Return: VK = 0x0D; break;
				case KeyCode.Pause: VK = 0x13; break;
				case KeyCode.Escape: VK = 0x1B; break;
				case KeyCode.Space: VK = 0x20; break;
				case KeyCode.Exclaim: VK = 0x31; break;
				case KeyCode.DoubleQuote: VK = 0xDE; break;
				case KeyCode.Hash: VK = 0x33; break;
				case KeyCode.Dollar: VK = 0x34; break;
				case KeyCode.Ampersand: VK = 0x37; break;
				case KeyCode.Quote: VK = 0xDE; break;
				case KeyCode.LeftParen: VK = 0x39; break;
				case KeyCode.RightParen: VK = 0x30; break;
				case KeyCode.Asterisk: VK = 0x13; break;
				case KeyCode.Equals:
				case KeyCode.Plus: VK = 0xBB; break;
				case KeyCode.Less:
				case KeyCode.Comma: VK = 0xBC; break;
				case KeyCode.Underscore:
				case KeyCode.Minus: VK = 0xBD; break;
				case KeyCode.Greater:
				case KeyCode.Period: VK = 0xBE; break;
				case KeyCode.Question:
				case KeyCode.Slash: VK = 0xBF; break;

				case KeyCode.Alpha0:
				case KeyCode.Alpha1:
				case KeyCode.Alpha2:
				case KeyCode.Alpha3:
				case KeyCode.Alpha4:
				case KeyCode.Alpha5:
				case KeyCode.Alpha6:
				case KeyCode.Alpha7:
				case KeyCode.Alpha8:
				case KeyCode.Alpha9:
					VK = 0x30 + ((int)key - (int)KeyCode.Alpha0); break;

				case KeyCode.Colon:
				case KeyCode.Semicolon: VK = 0xBA; break;

				case KeyCode.At: VK = 0x32; break;
				case KeyCode.LeftBracket: VK = 0xDB; break;
				case KeyCode.Backslash: VK = 0xDC; break;
				case KeyCode.RightBracket: VK = 0xDD; break;
				case KeyCode.Caret: VK = 0x36; break;
				case KeyCode.BackQuote: VK = 0xC0; break;

				case KeyCode.A:
				case KeyCode.B:
				case KeyCode.C:
				case KeyCode.D:
				case KeyCode.E:
				case KeyCode.F:
				case KeyCode.G:
				case KeyCode.H:
				case KeyCode.I:
				case KeyCode.J:
				case KeyCode.K:
				case KeyCode.L:
				case KeyCode.M:
				case KeyCode.N:
				case KeyCode.O:
				case KeyCode.P:
				case KeyCode.Q:
				case KeyCode.R:
				case KeyCode.S:
				case KeyCode.T:
				case KeyCode.U:
				case KeyCode.V:
				case KeyCode.W:
				case KeyCode.X:
				case KeyCode.Y:
				case KeyCode.Z:
					VK = 0x41 + ((int)key - (int)KeyCode.A); break;

				case KeyCode.Delete: VK = 0x2E; break;
				case KeyCode.Keypad0:
				case KeyCode.Keypad1:
				case KeyCode.Keypad2:
				case KeyCode.Keypad3:
				case KeyCode.Keypad4:
				case KeyCode.Keypad5:
				case KeyCode.Keypad6:
				case KeyCode.Keypad7:
				case KeyCode.Keypad8:
				case KeyCode.Keypad9:
					VK = 0x60 + ((int)key - (int)KeyCode.Keypad0); break;

				case KeyCode.KeypadPeriod: VK = 0x6E; break;
				case KeyCode.KeypadDivide: VK = 0x6F; break;
				case KeyCode.KeypadMultiply: VK = 0x6A; break;
				case KeyCode.KeypadMinus: VK = 0x6D; break;
				case KeyCode.KeypadPlus: VK = 0x6B; break;
				case KeyCode.KeypadEnter: VK = 0x6C; break;
				//case KeyCode.KeypadEquals: VK = 0x00; break;
				case KeyCode.UpArrow: VK = 0x26; break;
				case KeyCode.DownArrow: VK = 0x28; break;
				case KeyCode.RightArrow: VK = 0x27; break;
				case KeyCode.LeftArrow: VK = 0x25; break;
				case KeyCode.Insert: VK = 0x2D; break;
				case KeyCode.Home: VK = 0x24; break;
				case KeyCode.End: VK = 0x23; break;
				case KeyCode.PageUp: VK = 0x21; break;
				case KeyCode.PageDown: VK = 0x22; break;

				case KeyCode.F1:
				case KeyCode.F2:
				case KeyCode.F3:
				case KeyCode.F4:
				case KeyCode.F5:
				case KeyCode.F6:
				case KeyCode.F7:
				case KeyCode.F8:
				case KeyCode.F9:
				case KeyCode.F10:
				case KeyCode.F11:
				case KeyCode.F12:
				case KeyCode.F13:
				case KeyCode.F14:
				case KeyCode.F15:
					VK = 0x70 + ((int)key - (int)KeyCode.F1); break;

				case KeyCode.Numlock: VK = 0x90; break;
				case KeyCode.CapsLock: VK = 0x14; break;
				case KeyCode.ScrollLock: VK = 0x91; break;
				case KeyCode.RightShift: VK = 0xA1; break;
				case KeyCode.LeftShift: VK = 0xA0; break;
				case KeyCode.RightControl: VK = 0xA3; break;
				case KeyCode.LeftControl: VK = 0xA2; break;
				case KeyCode.RightAlt: VK = 0xA5; break;
				case KeyCode.LeftAlt: VK = 0xA4; break;
				//case KeyCode.RightCommand: VK = 0x22; break;
				//case KeyCode.RightApple: VK = 0x22; break;
				//case KeyCode.LeftCommand: VK = 0x22; break;
				//case KeyCode.LeftApple: VK = 0x22; break;
				//case KeyCode.LeftWindows: VK = 0x22; break;
				//case KeyCode.RightWindows: VK = 0x22; break;
				case KeyCode.Help: VK = 0xE3; break;
				case KeyCode.Print: VK = 0x2A; break;
				case KeyCode.SysReq: VK = 0x2C; break;
				case KeyCode.Break: VK = 0x03; break;
			}
			return VK;
		}

		// Simplified Mapper for Unity KeyCode to Virtual KeyCode with Alphabet key set
		private static int KeyCodeToVkey(KeyCode key)
		{
			int VK = 0;
			switch (key)
			{
				case KeyCode.A:
				case KeyCode.B:
				case KeyCode.C:
				case KeyCode.D:
				case KeyCode.E:
				case KeyCode.F:
				case KeyCode.G:
				case KeyCode.H:
				case KeyCode.I:
				case KeyCode.J:
				case KeyCode.K:
				case KeyCode.L:
				case KeyCode.M:
				case KeyCode.N:
				case KeyCode.O:
				case KeyCode.P:
				case KeyCode.Q:
				case KeyCode.R:
				case KeyCode.S:
				case KeyCode.T:
				case KeyCode.U:
				case KeyCode.V:
				case KeyCode.W:
				case KeyCode.X:
				case KeyCode.Y:
				case KeyCode.Z:
					VK = 0x41 + ((int)key - (int)KeyCode.A); break;
			}
			return VK;
		}
#endif // ENABLE_HTINPUT

		// GetKey with GetAsyncKeyState for IME Alphabets
		public static bool GetKey(KeyCode key)
		{
#if ENABLE_HTINPUT
			int VK = KeyCodeToVkey(key);
			if (VK != 0)
				return GetKeyVK(VK);
			else
				return Input.GetKey(key);
#else // ENABLE_HTINPUT
            return Input.GetKey(key);
#endif // ENABLE_HTINPUT
		}

		// GetKeyDown with GetAsyncKeyState for IME Alphabets
		public static bool GetKeyDown(KeyCode key)
		{
#if ENABLE_HTINPUT
			return (GetAsyncKeyState(KeyCodeToVkey(key)) == -32767);
#else // ENABLE_HTINPUT
            return Input.GetKeyDown(key);
#endif // ENABLE_HTINPUT
		}

		// GetKey with GetAsyncKeyState for full keycode
		public static bool GetKeyFullCover(KeyCode key)
		{
#if ENABLE_HTINPUT
			int VK = KeyCodeToVkeyFullSet(key);
			if (VK != 0)
				return GetKeyVK(VK);
			else
				return Input.GetKey(key);
#else // ENABLE_HTINPUT
            return Input.GetKey(key);
#endif // ENABLE_HTINPUT
		}

		// Key Check with Virtual Keycode
		public static bool GetKeyVK(int VKey)
		{
#if ENABLE_HTINPUT
			return (GetAsyncKeyState(VKey) != 0);
#else // ENABLE_HTINPUT
			return false;
#endif // ENABLE_HTINPUT
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	/// <summary>
	/// 키 입력을 관리해주는 Manager입니다.
	/// [현재 관리되고 있는 입력 목록]
	/// - Keyboard, Mouse, XBox360 Pad
	/// </summary>
	public sealed class HTInputManager : HTComponent
	{
		//---------------------------------------
		private static HTInputManager _instance = null;
		public static HTInputManager Instance { get { return _instance; } }

		//---------------------------------------
		private bool _joystickConnected = false;
		public bool JoystickConnected { get { return _joystickConnected; } }
		
		//---------------------------------------
		private float _horizontal = 0.0f;
		public float Horizontal
		{
			get { return _horizontal; }
			set { _horizontal = value; }
		}
		private float _vertical = 0.0f;
		public float Vertical
		{
			get { return _vertical; }
			set { _vertical = value; }
		}

		//---------------------------------------
		private Vector3 _mousePickingPos = Vector3.zero;
		public Vector3 MousePickingPos
		{
			get { return _mousePickingPos; }
			set { _mousePickingPos = value; }
		}
		
		//---------------------------------------
		private GEnv _gEnv = null;
		private Ray _lastMousePickedRay = new Ray();

		//---------------------------------------
		private GameObject _firstSelectObject = null;
		public GameObject FirstSelectObject
		{
			get { return _firstSelectObject; }
			set { _firstSelectObject = value; }
		}
		
		//---------------------------------------
		public override void Initialize()
		{
			_instance = this;
			_gEnv = HTFramework.Instance.RegistGEnv;

			HTFramework.onSceneChange += OnSceneChange;
		}

		public override void Tick(float fDelta)
		{
			if (_gEnv == null)
				_gEnv = GEnv.Get();

			//-----
			for (int nInd = 0; nInd < _gEnv._input_Buttons.Length; ++nInd)
				_gEnv._input_Buttons[nInd].Update();

			for (int nInd = 0; nInd < _gEnv._mapped_Buttons.Length; ++nInd)
				_gEnv._mapped_Buttons[nInd].Update();

			//-----
			if (HTAfxPref.IsMobilePlatform == false)
			{
				_joystickConnected = false;
				if (_gEnv._input_Use_Joystick)
				{
					string[] szJoystickNames = Input.GetJoystickNames();
					do
					{
						if (szJoystickNames == null || szJoystickNames.Length == 0)
							break;

						for (int nInd = 0; nInd < szJoystickNames.Length; ++nInd)
						{
							if (string.IsNullOrEmpty(szJoystickNames[nInd]) == false)
							{
								_joystickConnected = true;
								break;
							}
						}
					}
					while (false);
				}

				if (JoystickConnected)
				{
					SystemUtils.CursorShow(false);

					//-----
					Vector2 vMovement = Vector2.zero;
					vMovement.x = Input.GetAxis(_gEnv._input_Axis_Horizontal);
					vMovement.y = Input.GetAxis(_gEnv._input_Axis_Vertical);

					_horizontal = vMovement.x;
					_vertical = vMovement.y;

					//-----
					if (EventSystem.current != null)
					{
						GameObject pCurSelectObj = EventSystem.current.currentSelectedGameObject;
						if (pCurSelectObj == null || pCurSelectObj.activeInHierarchy == false)
						{
							GameObject pSelectObject = null;
							List<UIPopup> vList = HTUIManager.OpenedPopupList;
							for (int nInd = vList.Count - 1; nInd >= 0; --nInd)
							{
								if (nInd < 0)
									break;

								if (vList[nInd].FirstSelectObject != null)
								{
									pSelectObject = vList[nInd].FirstSelectObject;
									break;
								}
							}

							if (pSelectObject == null)
								pSelectObject = _firstSelectObject;

							EventSystem.current.SetSelectedGameObject(pSelectObject);
						}
					}
				}
				else
				{
					SystemUtils.CursorShow(true);

					_horizontal = Input.GetAxis(_gEnv._input_Axis_Horizontal);
					_vertical = Input.GetAxis(_gEnv._input_Axis_Vertical);
				}
			}
		}

		public override void FixedTick(float fDelta)
		{
			bool bIsMobilePlatform = HTAfxPref.IsMobilePlatform;
			bool bUsePicking = (bIsMobilePlatform) ? _gEnv._input_UseMousePicking_Mobile : _gEnv._input_UseMousePicking_PC;

			if (bUsePicking && _joystickConnected == false)
			{
				Camera pCurCamera = Camera.main;
				if (pCurCamera != null)
				{
					Vector3 vMousePos = Input.mousePosition;
					vMousePos.z = 1.0f;

					vMousePos = pCurCamera.ScreenToWorldPoint(vMousePos);

					Vector3 vMouseDir = vMousePos - pCurCamera.transform.position;
					vMouseDir.Normalize();

					_lastMousePickedRay.origin = vMousePos;
					_lastMousePickedRay.direction = vMouseDir;

					float fNearistance = float.MaxValue;
					RaycastHit[] vRayHits = Physics.RaycastAll(_lastMousePickedRay);
					for (int nInd = 0; nInd < vRayHits.Length; ++nInd)
					{
						if (vRayHits[nInd].collider == null)
							continue;

						float fCurDistance = vRayHits[nInd].distance;
						if (fNearistance < fCurDistance)
							continue;

						MousePickableObject pPickable = vRayHits[nInd].collider.gameObject.GetComponent<HT.MousePickableObject>();
						if (pPickable == null)
							continue;

						fNearistance = fCurDistance;
						_mousePickingPos = vRayHits[nInd].point;
					}
				}
			}
		}

		public override void OnDestroy()
		{
		}

		//---------------------------------------
		private void OnSceneChange()
		{
			_firstSelectObject = null;
		}

		//---------------------------------------
		public HTKey GetKey(string szKey)
		{
			HTKey pRetVal = null;
			do
			{
				pRetVal = _gEnv.FindInputKey(szKey);
				if (pRetVal != null)
					break;

				pRetVal = _gEnv.FindMappedKey(szKey);
			}
			while (false);

			return pRetVal;
		}

		public eButtonState GetKeyState(string szKey)
		{
			HTKey pKey = GetKey(szKey);
			if (pKey != null)
				return pKey.ButtonState;

			return eButtonState.Free;
		}

		public bool GetKeyState(string szKey, eButtonState eState)
		{
			HTKey pKey = GetKey(szKey);
			if (pKey != null)
				return (pKey.ButtonState == eState) ? true : false;

			return false;
		}

		//---------------------------------------
	}
}

/////////////////////////////////////////
//---------------------------------------
