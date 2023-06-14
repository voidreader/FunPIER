using UnityEngine;
using System;
using System.Collections;

namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	public class GEnv : MonoBehaviour
	{
		/////////////////////////////////////////
		//---------------------------------------
		[Header("ENGINE SETTINGS")]
		/// <summary>
		/// 다중 Client 실행을 방지합니다.
		/// PC에만 적용됩니다.
		/// </summary>
		public bool _preventMultiClient = true;

		/// <summary>
		/// 3D Sound의 Falloff 거리를 설정합니다.
		/// SoundManager에 의해 호출 된 Sound만 적용됩니다.
		/// </summary>
		public float _3DSoundFallOffDistance = 100.0f;
		public float _3DSoundBlendRatio = 0.9f;
		public AudioRolloffMode _3DSoundRolloffMode = AudioRolloffMode.Linear;

		/// <summary>
		/// TimeUtils가 서버 시간을 이용해서 모든 시간을 출력하도록 합니다.
		/// </summary>
		public bool _timeUseOnlyServerTime = false;

		//---------------------------------------
		[Header("ENGINE SETTINGS - PC")]
		public int _frameLimit_PC = 60;
		public eScreenResolution _resolutionFix_PC = eScreenResolution.Unfixed;
		public Vector2 _resolutionPC = new Vector2(1920, 1080);
		public bool _resolutionPC_FullScreen = true;

		//---------------------------------------
		[Header("ENGINE SETTINGS - MOBILE")]
		public int _frameLimit_AOS = 30;
		public int _frameLimit_IOS = 30;
		public eScreenResolution _resolutionFix_Mobile = eScreenResolution.Fixed;
		public Vector2 _resolutionMobile = new Vector2(1280, 720);
		public bool _resolusionMobile_FixRatio = false;
		public bool _mobileIsNeverSleep = true;

		//---------------------------------------
		[Header("ENGINE SETTINGS - PLATFORM")]
		/// <summary>
		/// HTPlatform을 사용할지 결정합니다.
		/// </summary>
		public bool _platform_GooglePlay = true;
		/// <summary>
		/// HTPlatform을 사용할지 결정합니다.
		/// </summary>
		public bool _platform_iOSGameCenter = true;

		//---------------------------------------
		[Header("ENGINE SETTINGS - COMMON")]
		/// <summary>
		/// 자동으로 GC가 최소 몇 Frame마다 호출 될 지 결정합니다.
		/// </summary>
		public float _autoGCWhenFrame_Min = 30;
		/// <summary>
		/// 자동으로 GC가 최대 몇 Frame 동안 호출되지 않을 경우 강제 호출 할지 결정합니다.
		/// </summary>
		public float _autoGCWhenFrame_Max = 60;
		/// <summary>
		/// 최소 Frame 조건이 달성 된 이후 현재 Frame 처리에 소요 된 시간이 이보다 낮을 경우 GC가 호출됩니다.
		/// </summary>
		public float _autoGCWhenDeltaTime = 0.05f;

		//---------------------------------------
		[Space(20)]
		[Header("TABLE - ADRESS")]
		public string _tableFileFolder = "Tables";

		[Header("TABLE - NAMES [Key/Address]")]
		/// <summary>
		/// HTTableManager에서 자동으로 Load 할 Table을 등록합니다.
		/// </summary>
		public string[] _userTableSettings = null;

		[Header("TABLE - HT FRAMEWORK DEFAULT TABLES")]
		public string _table_LocalstringKey;

		//---------------------------------------
		[Space(20)]
		[Header("UI - ADDRESS")]
		public string _uiPrefabFolder = "Prefabs/UI";

		[Header("UI - SETTINGS")]
		public string _uiDefaultMessageBox = "Popup_MessageBox";
		public Sprite _ui_BlankSprite = null;
		public float _ui_dragThresholdDist = 0.5f;

		public Vector2 _ui_ScaleWithSizeResolution = new Vector2(1920, 1080);
		[Range(0.0f, 1.0f)]
		public float _ui_ScaleWithSizeMatch = 0.0f;
		public Vector2 ConvertUIPosByScreenRect(Vector2 vScreenRect)
		{
			Vector2 vRetVal = vScreenRect;
			vRetVal.x = UIResolutionScaled_Width * vScreenRect.x;
			vRetVal.y = UIResolutionScaled_Height * vScreenRect.y;

			return vRetVal;
		}

		public float UIResolutionScaled_Width { get { return _ui_ScaleWithSizeResolution.x / Screen.width; } }
		public float UIResolutionScaled_Height { get { return _ui_ScaleWithSizeResolution.y / Screen.height; } }

		[Header("UI - FONT")]
		/// <summary>
		/// 현재 언어에 따라 자동으로 사용 할 Font를 설정합니다.
		/// </summary>
		public Font _ui_DefaultFont = null;
		public HTFont[] _ui_RegistedFont = null;

		//---------------------------------------
		[Space(20)]
		[Header("NETWORK")]
		public float _network_EntityPosLerpTime = 0.25f;

		//---------------------------------------
		[Space(20)]
		[Header("INPUT - AXIS - KEYBOARD")]
		public string _input_Axis_Horizontal = "Horizontal";
		public string _input_Axis_Vertical = "Vertical";

		[Header("INPUT - AXIS - JOYSTICK")]
		public bool _input_Use_Joystick = true;
		public string _input_Axis_Trigger = "Trigger";

		[Header("INPUT - BUTTONS")]
		public HTInputKey[] _input_Buttons = null;
		public HTMappedKey[] _mapped_Buttons = null;
		public bool _input_UseMousePicking_PC = false;
		public bool _input_UseMousePicking_Mobile = false;

		[Header("INPUT - SYSTEM INTERACT")]
		public string _input_Interact_Close = "Cancel";

		//---------------------------------------
		[Space(20)]
		[Header("SYSTEM - GRAPHIC COMPONENTS")]
		public float _downSamplingRate = 0.5f;

		[Space(20)]
		[Header("SYSTEM - PROTECTION CLIENT")]
		public string _dataPurityInfomationSourceFile = "Scripts/HTDataMD5Keys.cs";
		

		/////////////////////////////////////////
		//---------------------------------------
		public const string HTPrefabFolder = "HTPrefabs/";
		public const string Prefab_HTFramework = "HTFramework";
		public const string Prefab_GlobalUI = "HTGlobalUI";
		public const string Prefab_ConsoleWindow = "HTConsoleWindow";
		public const string Prefab_DebugFeatureWindow = "HTDebugFeatureWindow";
		public const string Prefab_GPUPrecache = "HTGPUPrecache";
		public const string Prefab_UIToolTip = "HTUIToolTip";

		//-----
		public const string HTPrefab_ObjectPoolName = "_HTObjectPool_";
		public const string HTPrefab_UIHelperName = "_HTUIManager_";
		public const string HTPrefab_SceneCanvasName = "_SceneRenderCanvas_";

		public const int TIMELAYER_ENGINE = 0;
		public const int TIMELAYER_CONSOLE = 1;
		public const int TIMELAYER_INDICATOR = 2;
		public const int TIMELAYER_GAME = 3;

		public const float TIME_MAXDELTA = 1.0f;
		public const int NETWORK_TIMEOUT_MILLISEC = 5000;
		public const float NETWORK_ENTITY_TELEPORT_DIST = 10.0f;

		public const float GLOBALUI_DEFAULT_FADETIME = 0.5f;

		//-----
		public const float DELAY_FOR_ANIMATOR_STATE_CHANGE = 0.25f;

		//-----
		public const string FILE_TEXTASSET_EXIST = ".bytes";
		public const string NET_LOCALHOST = "127.0.0.1";

		//-----
		public const int CRYPTKEY_INT = 0x71347692;
		public const byte CRYPTKEY_BYTE = 0xA5;
		public const float CRYPTKEY_FLOAT_A = 2.1284f;
		public const float CRYPTKEY_FLOAT_B = 56.9942f;

		//-----
		public const int INVALID_TID = -1;
		public const string LOCALTABLE_SUBLOCALKEY = "|";

		//-----
		public const float UNITY_ALLOWSCENEACTIVATION_WAITPROG = 0.9f;

		//---------------------------------------
		private static GEnv _gEnv = null;
		static public GEnv Get()
		{
			if (_gEnv == null)
			{
				HTFramework pFramework = HTFramework.Instance;
				if (pFramework == null)
					return null;

				_gEnv = pFramework.RegistGEnv;
			}

			return _gEnv;
		}

		//---------------------------------------
		public HTInputKey FindInputKey(string szID)
		{
			for (int nInd = 0; nInd < _input_Buttons.Length; ++nInd)
				if (_input_Buttons[nInd].KeyID == szID)
					return _input_Buttons[nInd];

			return null;
		}

		public HTMappedKey FindMappedKey(string szID)
		{
			for (int nInd = 0; nInd < _mapped_Buttons.Length; ++nInd)
				if (_mapped_Buttons[nInd].KeyID == szID)
					return _mapped_Buttons[nInd];

			return null;
		}

		//---------------------------------------
#if UNITY_EDITOR
		public void DoRefreshAllMD5Keys()
		{
			string szSourceFileAddr = string.Format("{0}/{1}", Application.dataPath, _dataPurityInfomationSourceFile);
			string szSource;
			FileUtils.LoadBufferFromFile(szSourceFileAddr, out szSource, false);

			if (string.IsNullOrEmpty(szSource))
				return;

			szSource = szSource.Replace(HTDataMD5Keys_Token.FEATURE_DISABLED, HTDataMD5Keys_Token.FEATURE_ENABLED);
			if (_userTableSettings.Length > 0)
			{
				int nIndexOfInitPos = szSource.IndexOf(HTDataMD5Keys_Token.FEATURE_TABLE_INIT, 0);
				int nIndexOfInitEnd = szSource.IndexOf(HTDataMD5Keys_Token.KEYWORD_END, nIndexOfInitPos);

				string szNewInit = string.Format("{0}{1}", HTDataMD5Keys_Token.FEATURE_TABLE_INIT, string.Format(HTDataMD5Keys_Token.KEYWORD_TABLE_INIT, _userTableSettings.Length));
				szSource = szSource.Replace(szSource.Substring(nIndexOfInitPos, nIndexOfInitEnd - nIndexOfInitPos), szNewInit);

				//-----
				int nIndexOfArrayPos = szSource.IndexOf(HTDataMD5Keys_Token.FEATURE_TABLE_ARRAY, 0);
				int nIndexOfArrayEnd = szSource.IndexOf(HTDataMD5Keys_Token.KEYWORD_END, nIndexOfArrayPos);

				string szNewArray = string.Format("{0}{1}", HTDataMD5Keys_Token.FEATURE_TABLE_ARRAY, System.Environment.NewLine);
				for (int nInd = 0; nInd < _userTableSettings.Length; ++nInd)
				{
					int nKeyIndex = _userTableSettings[nInd].IndexOf("/");
					string szTableFileName = _userTableSettings[nInd].Substring(nKeyIndex + 1);
					string szCurMD5 = CryptUtils.EncryptString(CryptUtils.GetResourceHashMD5(string.Format("{0}/{1}", _tableFileFolder, szTableFileName)));
					szNewArray += string.Format(HTDataMD5Keys_Token.KEYWORD_TABLE_ARRAY, nInd, szTableFileName, szCurMD5) + System.Environment.NewLine;
				}

				szSource = szSource.Replace(szSource.Substring(nIndexOfArrayPos, nIndexOfArrayEnd - nIndexOfArrayPos), szNewArray);
			}

			//-----
			FileUtils.CreateFileFromBuffer(szSourceFileAddr, szSource, false);
			UnityEditor.AssetDatabase.Refresh();
		}
#endif // UNITY_EDITOR

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
	public enum eStartType
	{
		None,
		OnAwake,
		OnStart,
		OnEnable,
	}

	public enum eRepeatType
	{
		Once,
		Loop,
		PingPong_Once,
		PingPong_Loop,
	}

	public enum eTimeType
	{
		RealTime,
		GameTime,
		SmoothGameTime,
	}

	public enum eCreateWhen
	{
		None,

		OnAwake,
		OnStart,
		OnEnable,
		OnDisable,
		OnDestroy,
	}


	/////////////////////////////////////////
	//---------------------------------------
	public enum eTweenType
	{
		Back,
		Bounce,
		Circ,
		Cubic,
		Elastic,
		Expo,
		Quad,
		Quart,
		Quint,
		Sine,
		Linear,
		Septieme,
		Desimo,

		Max,
	}

	public enum eTweenInOut
	{
		In,
		Out,
		InOut,
		OutIn,

		Max,
	}


	/////////////////////////////////////////
	//---------------------------------------
	public enum eMessageType
	{
		None,
		Good,
		Warning,
		Error,
	}

	public enum eCreateType
	{
		None,
		Create,
		CreateToPool,
	}

	public enum eDestroyType
	{
		None,
		Disable,
		Destroy,
	}


	/////////////////////////////////////////
	//---------------------------------------
	public enum eAudioVolumeType
	{
		System,
		BGM,
		SFX,

		Max,
	}

	public enum eAudioMiscType
	{
		SFX,
		BGM,

		Max,
	}


	/////////////////////////////////////////
	//---------------------------------------
	public enum eButtonState
	{
		Down,
		Hold,
		Up,
		Free,
	}

	public enum eAxisButtonType
	{
		None,
		IsAxis,
		AddAxis,
	}

	public enum eMessageBoxType
	{
		Question,
		Exclamation,
	}

	public enum eMessageBoxButton
	{
		TwoButton,
		OneButton,
	}

	public enum eToolTipEventType
	{
		OnClickHold,
		OnCursorEnter,
	}

	public enum eToolTipElementType
	{
		Text,
		Image,
		Blank,
		Object,

		Max,
	}


	/////////////////////////////////////////
	//---------------------------------------
	public enum eOS
	{
		Unknown = 0,
		PC,
		Android,
		IOS,
	}

	public enum ePlatform
	{
		Unknown = 0,
		PC,
		Mobile,
	}

	public enum ePlatformType
	{
		None,
		Google,
		iOS,
		Steam,

		Max,
	}

	public enum ePlatformObject
	{
		EnableAll,
		EnableOnlyPC,
		EnableOnlyMobile,
	}

	public enum eScreenResolution
	{
		Unfixed,
		Fixed,
	}

	public enum eImageExist
	{
		PNG,
		JPG
	}


	/////////////////////////////////////////
	//---------------------------------------
	public enum eEntityMoveType
	{
		SourceControl,
		RigidBody,
		Navigation,
		Transform,
	}

	public enum eEntityRotateType
	{
		SourceControl,
		MoveDirection,
		MousePosition,
	}


	/////////////////////////////////////////
	//---------------------------------------
	public struct eCursorLockState
	{
		public enum e
		{
			Free,
			LockToCenter,
			LockToWindow,
		}

		public static CursorLockMode ToLockMode(e eType)
		{
			switch (eType)
			{
				case e.Free:
					return CursorLockMode.None;

				case e.LockToCenter:
					return CursorLockMode.Locked;

				case e.LockToWindow:
					return CursorLockMode.Confined;
			}

			return CursorLockMode.None;
		}

		public static e ToLockState(CursorLockMode eType)
		{
			switch (eType)
			{
				case CursorLockMode.None:
					return e.Free;

				case CursorLockMode.Locked:
					return e.LockToCenter;

				case CursorLockMode.Confined:
					return e.LockToWindow;
			}

			return e.Free;
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
}