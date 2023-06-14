using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	public class HTConsoleOrder
	{
		public string _console = null;
		public string _descript = null;

		public bool InvokeCallback(string szValue)
		{
			if (szValue == null || szValue == string.Empty)
				return false;

			InvokeCallback_Element(szValue);
			return true;
		}

		public virtual void InvokeCallback_Element(string szValue)
		{

		}
	}

	public class HTConsoleOrder_Int : HTConsoleOrder
	{
		public override void InvokeCallback_Element(string szValue)
		{
			int nValue = int.Parse(szValue, CultureInfo.InvariantCulture);
			Utils.SafeInvoke<int>(_callback, nValue);
		}

		public Action<int> _callback;
	}

	public class HTConsoleOrder_Float : HTConsoleOrder
	{
		public override void InvokeCallback_Element(string szValue)
		{
			float fValue = float.Parse(szValue, CultureInfo.InvariantCulture);
			Utils.SafeInvoke<float>(_callback, fValue);
		}

		public Action<float> _callback;
	}

	public class HTConsoleOrder_String : HTConsoleOrder
	{
		public override void InvokeCallback_Element(string szValue)
		{
			Utils.SafeInvoke<string>(_callback, szValue);
		}

		public Action<string> _callback;
	}


	/////////////////////////////////////////
	//---------------------------------------
	public class HTConsoleOrderContainer
	{
		//---------------------------------------
		static List<HTConsoleOrder> _registedOrders = new List<HTConsoleOrder>();

		//---------------------------------------
		enum eConsoleVarType
		{
			Int,
			Float,
			String,
		}
		
		[System.Diagnostics.Conditional("ENABLE_DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
		public static void RegistConsoleOrder(string szConsole, string szDesc, Action<int> pCallback)
		{
			HTConsoleOrder_Int pNewConsole = CreateConsoleOrder<int>(szConsole, szDesc, pCallback, eConsoleVarType.Int) as HTConsoleOrder_Int;
			if (pNewConsole == null)
				return;
			
			pNewConsole._callback = pCallback;
		}

		[System.Diagnostics.Conditional("ENABLE_DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
		public static void RegistConsoleOrder(string szConsole, string szDesc, Action<float> pCallback)
		{
			HTConsoleOrder_Float pNewConsole = CreateConsoleOrder<float>(szConsole, szDesc, pCallback, eConsoleVarType.Float) as HTConsoleOrder_Float;
			if (pNewConsole == null)
				return;
			
			pNewConsole._callback = pCallback;
		}

		[System.Diagnostics.Conditional("ENABLE_DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
		public static void RegistConsoleOrder(string szConsole, string szDesc, Action<string> pCallback)
		{
			HTConsoleOrder_String pNewConsole = CreateConsoleOrder<string>(szConsole, szDesc, pCallback, eConsoleVarType.String) as HTConsoleOrder_String;
			if (pNewConsole == null)
				return;
			
			pNewConsole._callback = pCallback;
		}

		private static HTConsoleOrder CreateConsoleOrder<T>(string szConsole, string szDesc, Action<T> pCallback, eConsoleVarType eVarType)
		{
			if (pCallback == null)
				return null;

			if (FindConsoleOrder(szConsole) != null)
				return null;

			HTConsoleOrder pNewVar = null;
			switch (eVarType)
			{
				case eConsoleVarType.Int:
					pNewVar = new HTConsoleOrder_Int();
					break;

				case eConsoleVarType.Float:
					pNewVar = new HTConsoleOrder_Float();
					break;

				case eConsoleVarType.String:
					pNewVar = new HTConsoleOrder_String();
					break;
			}

			pNewVar._console = szConsole;
			pNewVar._descript = szDesc;

			_registedOrders.Add(pNewVar);
			return pNewVar;
		}

		//---------------------------------------
		public static HTConsoleOrder FindConsoleOrder(string szConsole)
		{
			for (int nInd = 0; nInd < _registedOrders.Count; ++nInd)
			{
				if (_registedOrders[nInd]._console == szConsole)
					return _registedOrders[nInd];
			}

			return null;
		}

		public static List<HTConsoleOrder> FindAllSimilarOrders(string szConsole)
		{
			List<HTConsoleOrder> vList = new List<HTConsoleOrder>();

			szConsole = szConsole.ToLower();
			for(int nInd = 0; nInd < _registedOrders.Count; ++nInd)
			{
				string lowerConsole = _registedOrders[nInd]._console.ToLower();
				if (lowerConsole.IndexOf(szConsole) >= 0)
					vList.Add(_registedOrders[nInd]);
			}

			if (vList.Count > 1)
			{
				vList.Sort(delegate (HTConsoleOrder x, HTConsoleOrder y) 
				{
					if (x == null || y == null)
						return 0;

					if (string.IsNullOrEmpty(x._console))
						return -1;

					if (string.IsNullOrEmpty(y._console))
						return 1;

					return x._console.CompareTo(y._console);
				});
			}

			return vList;
		}

		//---------------------------------------
		public static bool ChangeConsoleVar(string szConsole, string szVal)
		{
			HTConsoleOrder pOrder = FindConsoleOrder(szConsole);
			if (pOrder == null)
				return false;

			pOrder.InvokeCallback(szVal);
			return true;
		}

		public static bool ChangeConsoleVar(string szConsole, int nVal)
		{
			HTConsoleOrder pOrder = FindConsoleOrder(szConsole);
			if (pOrder == null)
				return false;

			pOrder.InvokeCallback(string.Format("{0}", nVal));
			return true;
		}

		public static bool ChangeConsoleVar(string szConsole, float fVal)
		{
			HTConsoleOrder pOrder = FindConsoleOrder(szConsole);
			if (pOrder == null)
				return false;

			pOrder.InvokeCallback(string.Format("{0:F3}", fVal));
			return true;
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
	public class HTConsoleWindow : MonoBehaviour
	{
		//---------------------------------------
		private string _lastOrder = null;

		//---------------------------------------
		[Header("BASE ELEMENTS")]
		[SerializeField]
		private Image _mask = null;
		private RectTransform _maskRecttransform = null;
		[SerializeField]
		private Text _consoleLine = null;

		[Header("EVENTER")]
		[SerializeField]
		private InputField _inputField = null;
		[SerializeField]
		private Button _closeButton = null;

		//---------------------------------------
		private static bool _initialized = false;

		private static bool _isOpened = false;
		public static bool IsOpened { get { return _isOpened; } }

		//---------------------------------------
		#region CONSOLE DEFAULT STRINGS
		const string s_Token = " ";
		const string s_Error_Input = "[Input] {0} value change to {1}";
		const string s_Error_CantFoundConsole = "[Error] Failed to found Console order [{0}]";
		#endregion

		private int _defaultFontSize = 0;
		private string _lastConsoleString = null;

		//---------------------------------------
		static HTConsoleWindow _instance = null;
		public static HTConsoleWindow Instance
		{
			get
			{
#if ENABLE_DEBUG || UNITY_EDITOR
				return _instance;
#else // ENABLE_DEBUG || UNITY_EDITOR
				return null;
#endif // ENABLE_DEBUG || UNITY_EDITOR
			}
		}
		
		//---------------------------------------
		private void Awake()
		{
#if ENABLE_DEBUG || UNITY_EDITOR
			InitDefaultConsoleOrder();

			_inputField.onEndEdit.AddListener((string s) => { CompleteInputConsoleKey(); });
			_closeButton.onClick.AddListener(CloseWindow);

			_defaultFontSize = _consoleLine.fontSize;

			_instance = this;
			DontDestroyOnLoad(_instance);
			
			OnEnable();
#else // ENABLE_DEBUG || UNITY_EDITOR
			_instance = null;
			Destroy(gameObject);
#endif // ENABLE_DEBUG || UNITY_EDITOR
		}

		private void Update()
		{
			if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == _inputField.gameObject)
			{
				if (string.IsNullOrEmpty(_inputField.text) == false && Input.GetKeyDown(KeyCode.Tab))
				{
					FindSimilarOrderAndAccept(_inputField.text);
				}
				else if (string.IsNullOrEmpty(_lastOrder) == false && Input.GetKeyDown(KeyCode.UpArrow))
				{
					InputFieldChange(string.Format("{0} ", _lastOrder));
				}
				else if (string.IsNullOrEmpty(_inputField.text) == false)
				{
					int nIndex = _inputField.text.IndexOf("?");
					if (nIndex > 0)
					{
						string szID = _inputField.text.Substring(0, nIndex);

						InputFieldChange(szID);
						FindSimilarOrderAndAccept(szID);
					}
				}
			}
		}

		private void OnDestroy()
		{
			_instance = null;
		}

		//---------------------------------------
		private void InitDefaultConsoleOrder()
		{
			if (_initialized)
				return;

			_initialized = true;
			
			HTConsoleOrderContainer.RegistConsoleOrder("Time_scale", "Change time scale to value (0.0 ~ )", (float fValue) => 
			{
				TimeUtils.SetTimeScale(fValue, GEnv.TIMELAYER_CONSOLE);
			});
		}

		//---------------------------------------
		void OnEnable()
		{
			if (_maskRecttransform == null)
				_maskRecttransform = _mask.GetComponent<RectTransform>();
			
			_maskRecttransform.sizeDelta = Vector2.zero;

			//-----
			gameObject.transform.SetAsLastSibling();
		}

		//---------------------------------------
		[System.Diagnostics.Conditional("ENABLE_DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
		public void CompleteInputConsoleKey()
		{
			string szConsoleOrder = _inputField.text;
			_inputField.text = string.Empty;

			DoConsoleOrder(szConsoleOrder);
		}

		[System.Diagnostics.Conditional("ENABLE_DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
		public static void DoConsoleOrder(string szConsoleOrder)
		{
			if (szConsoleOrder == null || szConsoleOrder == string.Empty || szConsoleOrder == "`")
				return;

			//-----
			int nTokenIndex = szConsoleOrder.IndexOf(s_Token);
			string szConsole = string.Empty;
			string szValue = string.Empty;
			if (nTokenIndex > 0)
			{
				szConsole = szConsoleOrder.Substring(0, nTokenIndex);

				if (nTokenIndex < szConsoleOrder.Length)
					szValue = szConsoleOrder.Substring(nTokenIndex + 1, szConsoleOrder.Length - nTokenIndex - 1);
			}
			else
			{
				szConsole = szConsoleOrder;
			} 

			//-----
			HTConsoleOrder pOrder = HTConsoleOrderContainer.FindConsoleOrder(szConsole);
			if (pOrder == null)
			{
				if (Instance != null)
					Instance.PrintText(eMessageType.Error, string.Format(s_Error_CantFoundConsole, szConsole));
			}
			else
			{
				if (Instance != null)
				{
					Instance._lastOrder = szConsole;
					Instance.PrintText(eMessageType.None, string.Format(s_Error_Input, szConsole, szValue));
				}

				pOrder.InvokeCallback(szValue);
			}
		}

		//---------------------------------------
		[System.Diagnostics.Conditional("ENABLE_DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
		public static void OpenWindow()
		{
			if (_instance == null)
			{
				string szPrefabAddr = GEnv.HTPrefabFolder + GEnv.Prefab_ConsoleWindow;
				GameObject pObject = Utils.Instantiate(szPrefabAddr);
				if (pObject != null)
					_instance = pObject.GetComponent<HTConsoleWindow>();
			}

			if (_instance != null)
			{
				_isOpened = true;
				Utils.SafeActive(_instance.gameObject, true);
				EventSystem.current.SetSelectedGameObject(_instance._inputField.gameObject);
			}
		}

		public static void CloseWindow()
		{
			if (_instance != null)
			{
				_isOpened = false;
				Utils.SafeActive(_instance.gameObject, false);
			}
		}

		//---------------------------------------
		[System.Diagnostics.Conditional("ENABLE_DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
		public static void PrintConsoleText(eMessageType eType, string szString)
		{
			if (_instance == null)
				return;

			_instance.PrintText(eType, szString);
		}

		[System.Diagnostics.Conditional("ENABLE_DEBUG"), System.Diagnostics.Conditional("UNITY_EDITOR")]
		public void PrintText(eMessageType eType, string szString)
		{
			if (_lastConsoleString == szString)
				return;
			
			Color pColor;
			switch (eType)
			{
				case eMessageType.Good:
					pColor = Color.green;
					break;

				case eMessageType.Warning:
					pColor = Color.yellow;
					break;

				case eMessageType.Error:
					pColor = Color.red;
					break;

				case eMessageType.None:
				default:
					pColor = Color.white;
					break;
			}

			//-----
			string szLines = _consoleLine.text;
			szLines += string.Format("{0}<color=#{2}>{1}</color>", System.Environment.NewLine, szString, ColorUtility.ToHtmlStringRGBA(pColor));
			_consoleLine.text = szLines;

			_consoleLine.fontSize = (int)(_defaultFontSize * (1.0f / gameObject.transform.localScale.x));

			int nPreventInfiniteLoop = 0;
			while (nPreventInfiniteLoop < 10)
			{
				++nPreventInfiniteLoop;

				if (_consoleLine.preferredHeight > _consoleLine.rectTransform.rect.height)
				{
					int nFirstIndexOfNewLine = szLines.IndexOf(System.Environment.NewLine) + 1;
					szLines = szLines.Substring(nFirstIndexOfNewLine, szLines.Length - nFirstIndexOfNewLine);
					_consoleLine.text = szLines;
				}
				else
					break;
			}

			_lastConsoleString = szString;
		}

		//---------------------------------------
		private void FindSimilarOrderAndAccept(string szConsole)
		{
			PrintConsoleText(eMessageType.None, string.Empty);

			//-----
			List<HTConsoleOrder> vList = HTConsoleOrderContainer.FindAllSimilarOrders(szConsole);
			if (vList.Count > 0)
			{
				InputFieldChange(string.Format("{0} ", vList[0]._console));

				for (int nInd = 0; nInd < vList.Count; ++nInd)
					PrintConsoleText(eMessageType.Good, string.Format("{0} : {1}", vList[nInd]._console, vList[nInd]._descript));
			}
			else
			{
				PrintConsoleText(eMessageType.Warning, "[Error] No console order found.");
			}
		}

		private void InputFieldChange(string szString)
		{
			_inputField.text = szString;
			_inputField.caretPosition = _inputField.text.Length;
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}