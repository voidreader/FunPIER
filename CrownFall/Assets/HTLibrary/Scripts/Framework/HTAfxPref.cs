using System;
using System.Collections.Generic;
using UnityEngine;

namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	/// <summary>
	/// HTFramework의 Global Value들을 관리합니다.
	/// 자동으로 Value들을 Unity Pref에 저장합니다.
	/// </summary>
	public class HTAfxPref
	{
		/////////////////////////////////////////
		//---------------------------------------
		private const int HTAfxPrefVersion = 5;
		private const int HTAfxPrefVer_BaseLoadAssured = 4;
		private const int HTAfxPrefVer_MappedKeySavable = 5;

		private const float _defaultSystemVolme = 1.0f;
		private const float _defaultSFXVolme = 0.8f;
		private const float _defaultBGMVolme = 0.8f;

		//---------------------------------------
		public static eOS Platform
		{
			get
			{
#if UNITY_STANDALONE_WIN
				return eOS.PC;
#elif UNITY_ANDROID
				return eOS.Android;
#elif UNITY_IOS
				return eOS.IOS;
#else // Unknown
				return eOS.Unknown;
#endif // Unknown
			}
		}

		public static bool IsMobilePlatform
		{
			get
			{
				switch(Platform)
				{
					case eOS.Android:
					case eOS.IOS:
						return true;
				}

				return false;
			}
		}

		public static bool CheckPlatform(ePlatformObject eObjType)
		{
			switch(eObjType)
			{
				case ePlatformObject.EnableOnlyPC:
					{
						if (IsMobilePlatform)
							return false;
					}
					break;

				case ePlatformObject.EnableOnlyMobile:
					{
						if (IsMobilePlatform == false)
							return false;
					}
					break;
			}

			return true;
		}

		//---------------------------------------
		public static Action onChangeVolume_All = null;
		public static Action onChangeVolume_BGM = null;
		public static Action onChangeVolume_SFX = null;

		public static Action onChangeEnable_All = null;
		public static Action onChangeEnable_BGM = null;
		public static Action onChangeEnable_SFX = null;

		//---------------------------------------
		private static bool _sound_EnableAll = true;
		private static float _sound_VolumeAll = _defaultSystemVolme;

		private static bool _sound_EnableBGM = true;
		private static float _sound_VolumeBGM = _defaultBGMVolme;

		private static bool _sound_EnableSFX = true;
		private static float _sound_VolumeSFX = _defaultSFXVolme;

		//---------------------------------------
		public static bool Sound_EnableAll
		{
			get { return _sound_EnableAll; }
			set
			{
				_sound_EnableAll = value;
				Utils.SafeInvoke(onChangeEnable_All);
			}
		}

		public static float Sound_VolumeAll
		{
			get { return _sound_VolumeAll; }
			set
			{
				_sound_VolumeAll = value;
				Utils.SafeInvoke(onChangeVolume_All);
			}
		}

		//---------------------------------------
		public static bool Sound_EnableBGM
		{
			get { return _sound_EnableBGM; }
			set
			{
				_sound_EnableBGM = value;
				Utils.SafeInvoke(onChangeEnable_BGM);
			}
		}

		public static float Sound_VolumeBGM
		{
			get { return _sound_VolumeBGM; }
			set
			{
				_sound_VolumeBGM = value;
				Utils.SafeInvoke(onChangeVolume_BGM);
			}
		}

		//---------------------------------------
		public static bool Sound_EnableSFX
		{
			get { return _sound_EnableSFX; }
			set
			{
				_sound_EnableSFX = value;
				Utils.SafeInvoke(onChangeEnable_SFX);
			}
		}

		public static float Sound_VolumeSFX
		{
			get { return _sound_VolumeSFX; }
			set
			{
				_sound_VolumeSFX = value;
				Utils.SafeInvoke(onChangeVolume_SFX);
			}
		}


		/////////////////////////////////////////
		//---------------------------------------
		public static bool FullScreen
		{
#if UNITY_ANDROID || UNITY_IOS
			get { return true; }
			set { }
#else // UNITY_ANDROID || UNITY_IOS
			get { return Screen.fullScreen; }
			set { Screen.fullScreen = value; }
#endif // UNITY_ANDROID || UNITY_IOS
		}

		//---------------------------------------
		public static Action<int> onChangeQuality = null;
		
		private static int _quality = -1;
		public static int Quality
		{
			get
			{
				if (_quality < 0)
					_quality = QualitySettings.GetQualityLevel();

				return _quality;
			}
			set
			{
				_quality = value;
				RefreshQualityLevel();
			}
		}

		public static void RefreshQualityLevel()
		{
			QualitySettings.SetQualityLevel(_quality);
			Utils.SafeInvoke(onChangeQuality, _quality);
		}

		//---------------------------------------
		private static int _resolutionWidth = 0;
		private static int _resolutionHeight = 0;

		public static void SetResolution(int nWidth, int nHeight)
		{
			_resolutionWidth = nWidth;
			_resolutionHeight = nHeight;
			Screen.SetResolution(nWidth, nHeight, FullScreen);

			if (SystemUtils.GetCursorState() == eCursorLockState.e.LockToWindow)
			{
				SystemUtils.CursorLock(eCursorLockState.e.Free);
				SystemUtils.CursorLock(eCursorLockState.e.LockToWindow);
			}
		}

		/////////////////////////////////////////
		//---------------------------------------
		private static bool _waitingForSceneChange = false;
		public static bool WaitingForSceneChange
		{
			get { return _waitingForSceneChange; }
			set { _waitingForSceneChange = value; }
		}



		/////////////////////////////////////////
		//---------------------------------------
		private static bool _afxPrefLoaded = false;

		//---------------------------------------
		public static void SaveAfxPref()
		{
			if (_afxPrefLoaded == false)
				return;

			SetPref("VSN", HTAfxPrefVersion);

			//-----
			SetPref("SEA", _sound_EnableAll);
			SetPref("SEB", _sound_EnableBGM);
			SetPref("SES", _sound_EnableSFX);

			SetPref("SVA", _sound_VolumeAll);
			SetPref("SVB", _sound_VolumeBGM);
			SetPref("SVS", _sound_VolumeSFX);

			//-----
			SetPref("FSC", FullScreen);
			SetPref("QLT", Quality);

			SetPref("RW", _resolutionWidth);
			SetPref("RH", _resolutionHeight);

			//-----
			SetPref("SLG", Application.systemLanguage);
			SetPref("CLG", HTLocaleTable.CurrentLanguage);

			//-----
			GEnv pGEnv = GEnv.Get();
			if (pGEnv != null)
			{
				HTMappedKey[] vMappedKeys = pGEnv._mapped_Buttons;
				for (int nInd = 0; nInd < vMappedKeys.Length; ++nInd)
				{
					HTMappedKey pMappedKey = vMappedKeys[nInd];
					if (pMappedKey.KeyRemapped)
						SetPref(pMappedKey.KeyID, pMappedKey.MappedKeyCode);
				}
			}

			//-----
			PlayerPrefs.Save();
		}

		public static void LoadAfxPref()
		{
			int nCurVersion;
			GetPref("VSN", out nCurVersion, -1);
			if (nCurVersion >= HTAfxPrefVer_BaseLoadAssured)
			{
				GetPref("SEA", out _sound_EnableAll, true);
				GetPref("SEB", out _sound_EnableBGM, true);
				GetPref("SES", out _sound_EnableSFX, true);

				GetPref("SVA", out _sound_VolumeAll, _defaultSystemVolme);
				GetPref("SVB", out _sound_VolumeBGM, _defaultBGMVolme);
				GetPref("SVS", out _sound_VolumeSFX, _defaultSFXVolme);

				//-----
				bool bFullScreen = false;
				GetPref("FSC", out bFullScreen, true);
				FullScreen = bFullScreen;
				
				GetPref("QLT", out _quality, -1);
				if (_quality == -1)
					_quality = ((IsMobilePlatform) ? 0 : QualitySettings.names.Length - 1);

				//-----
				GetPref("RW", out _resolutionWidth, 0);
				GetPref("RH", out _resolutionHeight, 0);
				if (_resolutionWidth > 0 && _resolutionHeight > 0)
					SetResolution(_resolutionWidth, _resolutionHeight);

				//-----
				SystemLanguage eLang;
				GetPref("SLG", out eLang, SystemLanguage.Unknown);
				if (eLang == Application.systemLanguage)
					GetPref("CLG", out eLang, SystemLanguage.Unknown);
				
				if (eLang == SystemLanguage.Unknown)
				{
					eLang = Application.systemLanguage;
					HTDebug.PrintLog(eMessageType.Warning, string.Format("[HTAfx] System language change to {0}", eLang.ToString()));
				}

#if EANBLE_STEAM
				if (eLang == SystemLanguage.Korean)
					eLang = SystemLanguage.English;

				HTDebug.PrintLog(eMessageType.Warning, "[HTAfx] Korean can not be supported");
#endif // EANBLE_STEAM

				HTLocaleTable.ChangeLanguage(eLang, true);
			}

			//-----
			if (nCurVersion >= HTAfxPrefVer_MappedKeySavable)
			{
				GEnv pGEnv = GEnv.Get();
				if (pGEnv != null)
				{
					HTMappedKey[] vMappedKeys = pGEnv._mapped_Buttons;
					for (int nInd = 0; nInd < vMappedKeys.Length; ++nInd)
					{
						HTMappedKey pMappedKey = vMappedKeys[nInd];
						if (pMappedKey.KeyRemapped)
						{
							KeyCode eMappedKey;
							GetPref(pMappedKey.KeyID, out eMappedKey, pMappedKey.DefaultKeyCode);
							if (pMappedKey.DefaultKeyCode != eMappedKey)
								pMappedKey.SetKeyMap(true, null, eMappedKey);
						}
					}
				}
			}

			//-----
			_afxPrefLoaded = true;
		}


		/////////////////////////////////////////
		//---------------------------------------
		private static void SetPref(string key, bool val)
		{
			SetPref(key, (val) ? 1 : 0);
		}

		private static void SetPref(string key, int val)
		{
			SetPref(key, val.ToString());
		}

		private static void SetPref(string key, float val)
		{
			SetPref(key, val.ToString());
		}

		private static void SetPref<T>(string key, T val) where T : struct
		{
			SetPref(key, val.ToString());
		}

		private static void SetPref(string key, string val)
		{
			PlayerPrefs.SetString(key, CryptUtils.EncryptString(val));
		}

		//---------------------------------------
		private static void GetPref(string key, out bool val, bool defaultVal)
		{
			string szVal = null;
			GetPref(key, out szVal);

			if (string.IsNullOrEmpty(szVal))
			{
				val = defaultVal;
				return;
			}

			//-----
			val = false;

			try
			{
				int nVal = int.Parse(szVal);
				val = (nVal == 0) ? false : true;
			}
			catch (Exception ex)
			{
				HTDebug.PrintLog(eMessageType.Error, ex.Message);
			}
		}

		private static void GetPref(string key, out int val, int defaultVal)
		{
			string szVal = null;
			GetPref(key, out szVal);

			if (string.IsNullOrEmpty(szVal))
			{
				val = defaultVal;
				return;
			}

			//-----
			val = 0;

			try
			{
				val = int.Parse(szVal);
			}
			catch (Exception ex)
			{
				HTDebug.PrintLog(eMessageType.Error, ex.Message);
			}
		}

		private static void GetPref(string key, out float val, float defaultVal)
		{
			string szVal = null;
			GetPref(key, out szVal);

			if (string.IsNullOrEmpty(szVal))
			{
				val = defaultVal;
				return;
			}

			//-----
			val = 0.0f;

			try
			{
				val = float.Parse(szVal);
			}
			catch (Exception ex)
			{
				HTDebug.PrintLog(eMessageType.Error, ex.Message);
			}
		}

		private static void GetPref<T>(string key, out T val, T defaultVal) where T : struct
		{
			string szVal = null;
			GetPref(key, out szVal);

			if (string.IsNullOrEmpty(szVal))
			{
				val = defaultVal;
				return;
			}

			//-----
			val = default(T);

			try
			{
				val = (T)Enum.Parse(typeof(T), szVal);
			}
			catch (Exception ex)
			{
				HTDebug.PrintLog(eMessageType.Error, ex.Message);
			}
		}

		private static void GetPref(string key, out string val)
		{
			string szRetVal = PlayerPrefs.GetString(key, string.Empty);
			if (string.IsNullOrEmpty(szRetVal))
			{
				val = string.Empty;
				return;
			}

			val = CryptUtils.DecryptString(szRetVal);
		}


		/////////////////////////////////////////
		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}
