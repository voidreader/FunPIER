using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using HT;

/////////////////////////////////////////
//---------------------------------------
public class GameFramework : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	[Header("DEFAULT ELEMENTS")]
	static public GameFramework _Instance;


	/////////////////////////////////////////
	//---------------------------------------
	public LevelSettings[] m_vLevelSettings;

	//---------------------------------------
	public PlayerData m_pPlayerData;

	[Header("TUTORIAL INFO")]
	public bool _tutorialInfo_Keyboard = false;
	public bool _tutorialInfo_Joystick = false;
	public bool _tutorialInfo_Mobile = false;
	public bool _tutorialInfo_Game = false;
	public bool _askedTutorialPlay = false;

	[Header("GAME INFO")]
	public bool _socialLogin_Called = false;
	public bool _socialLogin_Joined = false;
	public bool _agreePrivacyTerms = false;
	public bool _hardMode_Opened = false;

	public bool _option_dash_useMoveDir = true;
	public bool _option_aim_useRightAnalog = true;


	/////////////////////////////////////////
	//---------------------------------------
#if ENABLE_DEBUG || UNITY_EDITOR
	[Header("DEBUG INFO")]
	public bool _enableAllAddonPattern = false;
	public bool _characterInvincible = false;
	public bool _enemyOneKill = false;
#endif // ENABLE_DEBUG || UNITY_EDITOR

	//---------------------------------------
	private string _safeKeyTarget = null;
	private bool _lastStateIsLogIn = false;

	//---------------------------------------
	private bool _saveLoadedOrInitialized = false;


	/////////////////////////////////////////
	//---------------------------------------
	private void Awake()
	{
		DontDestroyOnLoad(this);
		_Instance = this;

		//-----
#if DEMO_VERSION
		int nLastAlignIndex = 0;
		LevelSettings[] vAlign = new LevelSettings[m_vLevelSettings.Length];
		for(int nInd = 0; nInd < m_vLevelSettings.Length; ++nInd)
		{
			if (m_vLevelSettings[nInd]._isPackToDemo)
				vAlign[nLastAlignIndex++] = m_vLevelSettings[nInd];
		}

		for (int nInd = 0; nInd < m_vLevelSettings.Length; ++nInd)
		{
			if (m_vLevelSettings[nInd]._isPackToDemo == false)
				vAlign[nLastAlignIndex++] = m_vLevelSettings[nInd];
		}

		m_vLevelSettings = vAlign;
#endif // DEMO_VERSION

			//-----
			ArchivementManager.Instance.Init();

		//-----
		_safeKeyTarget = Application.dataPath + "/Resources/unity default resources";

		//-----
		m_pPlayerData = new PlayerData();

		//-----
		ResetSaveDatas();
		LoadSystemData();

		//-----
		RegistContoleOrders();

		//-----
		HTThirdParty_Adbrix.CreateInstance();
	}

	private void Update()
	{
	}

	private void FixedUpdate()
	{
		if (HTPlatform.Instanced != null)
			_lastStateIsLogIn = HTPlatform.Instance.IsLogIn;
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			SaveGameData();
			SaveSystemData();
		}
	}

	private void OnDestroy()
	{
		SaveGameData();
		SaveSystemData();
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void ResetSaveDatas()
	{
		m_pPlayerData.ResetData();
	}

	public void SetSaveInitialized()
	{
		_saveLoadedOrInitialized = true;
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void SaveGameData()
	{
		if (_saveLoadedOrInitialized == false)
			return;

		//----- Game Data Save
		{
			JSONClass pJSon = new JSONClass();
			JSONUtils.AddValue(pJSon, "VSN", GameDefine.nSaveFile_Version_Game);
			
			// SafeKey
			//if (Application.isEditor == false && HT.HTAfxPref.IsMobilePlatform == false)
			//{
			//	System.DateTime pNowTime = System.DateTime.Now;
			//
			//	System.IO.File.SetLastWriteTime(_safeKeyTarget, pNowTime);
			//
			//	int nKey = GetSafeTimeKey(pNowTime);
			//	JSONUtils.AddValue(pJSon, "SF_KY", nKey);
			//}

			//-----
			JSONUtils.AddValueSerialize(pJSon, "PLD", ref m_pPlayerData);

			int nSettingCount = m_vLevelSettings.Length;
			JSONUtils.AddValue(pJSon, "LSC", nSettingCount);

			//-----
			FileUtils.CreateFileFromJSON(FileUtils.CombinePersistentPath(GameDefine.szSaveFile_Game), pJSon);
			HT.HTDebug.PrintLog(eMessageType.None, "[SAVE DATA] Data save successed!");
		}
	}

	public bool LoadGameData()
	{
		bool bRetVal = true;

		//----- Game Data Save
		{
			JSONNode pJSon = null;
			FileUtils.LoadJSONFromFile(FileUtils.CombinePersistentPath(GameDefine.szSaveFile_Game), out pJSon);

			bool bNeedDelete = true;
			do
			{
				if (pJSon == null)
				{
					HT.HTDebug.PrintLog(eMessageType.Warning, "[LOAD DATA] Can't not found file...");
					break;
				}

				int nVersion = 0;
				JSONUtils.GetValue(pJSon, "VSN", out nVersion);
				if (nVersion != GameDefine.nSaveFile_Version_Game)
				{
					HT.HTDebug.PrintLog(eMessageType.Warning, "[LOAD DATA] Save Version Difference...");
					break;
				}

				//if (Application.isEditor == false && HT.HTAfxPref.IsMobilePlatform == false)
				//{
				//	int nSafeKey = 0;
				//	JSONUtils.GetValue(pJSon, "SF_KY", out nSafeKey);
				//
				//	System.DateTime pSavedTime = System.IO.File.GetLastWriteTime(_safeKeyTarget);
				//	int nCurKey = GetSafeTimeKey(pSavedTime);
				//
				//	if (nSafeKey != nCurKey)
				//	{
				//		HT.HTDebug.PrintLog(eMessageType.Warning, string.Format("[LOAD DATA] SaveKey is diffence... ({0}/{1})", nSafeKey, nCurKey));
				//		break;
				//	}
				//}

				JSONUtils.GetValueSerialize(pJSon, "PLD", ref m_pPlayerData);
				if (m_pPlayerData.Initialized == false)
				{
					HT.HTDebug.PrintLog(eMessageType.Warning, "[LOAD DATA] Save state is not initialized...");
					break;
				}

				int nSettingCount = 0;
				JSONUtils.GetValue(pJSon, "LSC", out nSettingCount);

				bNeedDelete = false;
			}
			while (false);

			if (bNeedDelete)
			{
				FileUtils.DeleteFile(FileUtils.CombinePersistentPath(GameDefine.szSaveFile_Game));
				bRetVal = false;
			}

			if (bRetVal)
			{
				_saveLoadedOrInitialized = true;
				HT.HTDebug.PrintLog(eMessageType.None, "[LOAD DATA] Data load successed!");
			}
			else
				HT.HTDebug.PrintLog(eMessageType.Warning, "[LOAD DATA] Data load failed...");
		}

		return bRetVal;
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void SaveSystemData()
	{
		//----- System Data Save
		{
			JSONClass pJSon = new JSONClass();

			//-----
			JSONUtils.AddNode(pJSon, "ACV", ArchivementManager.Instance.ToJSON());

			//-----
			JSONUtils.AddValue(pJSon, "TTR_VSN", GameDefine.nSaveFile_Version_System);
			JSONUtils.AddValue(pJSon, "TTR_KB", _tutorialInfo_Keyboard);
			JSONUtils.AddValue(pJSon, "TTR_JS", _tutorialInfo_Joystick);
			JSONUtils.AddValue(pJSon, "TTR_MB", _tutorialInfo_Mobile);
			JSONUtils.AddValue(pJSon, "TTR_GM", _tutorialInfo_Game);

			JSONUtils.AddValue(pJSon, "TTR_ATP", _askedTutorialPlay);

			JSONUtils.AddValue(pJSon, "AGT", _agreePrivacyTerms);
			JSONUtils.AddValue(pJSon, "HMO", _hardMode_Opened);

			JSONUtils.AddValue(pJSon, "SLC", _socialLogin_Called);
			JSONUtils.AddValue(pJSon, "SLJ", _lastStateIsLogIn);

			JSONUtils.AddValue(pJSon, "ODM", _option_dash_useMoveDir);
			JSONUtils.AddValue(pJSon, "OAR", _option_aim_useRightAnalog);

			//-----
			FileUtils.CreateFileFromJSON(FileUtils.CombinePersistentPath(GameDefine.szSaveFile_System), pJSon);
			HT.HTDebug.PrintLog(eMessageType.None, "[SAVE SYSTEM DATA] Data save successed!");
		}
	}

	public void LoadSystemData()
	{
		//----- System Data Save
		JSONNode pJSon = null;
		FileUtils.LoadJSONFromFile(FileUtils.CombinePersistentPath(GameDefine.szSaveFile_System), out pJSon);

		//-----
		_tutorialInfo_Keyboard = false;
		_tutorialInfo_Joystick = false;
		_tutorialInfo_Mobile = false;
		_tutorialInfo_Game = false;
		_askedTutorialPlay = false;
			
		_socialLogin_Called = false;
		_socialLogin_Joined = false;
		_agreePrivacyTerms = false;
		_hardMode_Opened = false;

		//-----
		bool bSystemDataLoadSucc = false;
		do
		{
			if (pJSon == null)
			{
				HT.HTDebug.PrintLog(eMessageType.Warning, "[LOAD SYSTEM DATA] Can't not found file...");
				break;
			}

			//-----
			JSONNode pChildNode = null;
			JSONUtils.GetNode(pJSon, "ACV", out pChildNode);

			ArchivementManager.Instance.ParseJSON(pChildNode);

			//-----
			int nCurVersion = 0;
			JSONUtils.GetValue(pJSon, "TTR_VSN", out nCurVersion, 0);
			if (nCurVersion <= 0)
				break;

			if (nCurVersion == GameDefine.nSaveFile_Version_System)
			{
				JSONUtils.GetValue(pJSon, "TTR_KB", out _tutorialInfo_Keyboard, false);
				JSONUtils.GetValue(pJSon, "TTR_JS", out _tutorialInfo_Joystick, false);
				JSONUtils.GetValue(pJSon, "TTR_MB", out _tutorialInfo_Mobile, false);
				JSONUtils.GetValue(pJSon, "TTR_GM", out _tutorialInfo_Game, false);

				JSONUtils.GetValue(pJSon, "TTR_ATP", out _askedTutorialPlay, false);

				JSONUtils.GetValue(pJSon, "SLC", out _socialLogin_Called, false);
				JSONUtils.GetValue(pJSon, "SLJ", out _socialLogin_Joined, false);
			}

			JSONUtils.GetValue(pJSon, "AGT", out _agreePrivacyTerms, false);
			JSONUtils.GetValue(pJSon, "HMO", out _hardMode_Opened, false);

			JSONUtils.GetValue(pJSon, "ODM", out _option_dash_useMoveDir, true);
			JSONUtils.GetValue(pJSon, "OAR", out _option_aim_useRightAnalog, true);

			//-----
			bSystemDataLoadSucc = true;
		}
		while (false);

		if (bSystemDataLoadSucc)
			HT.HTDebug.PrintLog(eMessageType.None, "[LOAD SYSTEM DATA] Data load successed!");
		else
			HT.HTDebug.PrintLog(eMessageType.Warning, "[LOAD SYSTEM DATA] Data load failed...");
	}

	//---------------------------------------
	private int GetSafeTimeKey(System.DateTime pTime)
	{
		int nRetVal = pTime.Hour;
		nRetVal += pTime.Minute;
		nRetVal += pTime.Second;
		nRetVal += pTime.Millisecond;

		return nRetVal;
	}

	/////////////////////////////////////////
	//---------------------------------------
	private void RegistContoleOrders()
	{
#if ENABLE_DEBUG || UNITY_EDITOR
		HTConsoleOrderContainer.RegistConsoleOrder("game_ivcb", "", (int nValue) => 
		{
			GameFramework._Instance._characterInvincible = (nValue == 0)? false : true;
		});

		HTConsoleOrderContainer.RegistConsoleOrder("game_eok", "", (int nValue) => 
		{
			GameFramework._Instance._enemyOneKill = (nValue == 0) ? false : true;
		});
#endif // ENABLE_DEBUG || UNITY_EDITOR
	}

	/////////////////////////////////////////
	//---------------------------------------
	public int SelectRandomLevel()
	{
		List<int> vBlankedList = new List<int>();
		for (int nInd = 0; nInd < m_pPlayerData.m_vLevelActivated.Length; ++nInd)
		{
			if (m_pPlayerData.m_vLevelActivated[nInd] == false)
				vBlankedList.Add(nInd);
		}

		if (vBlankedList.Count > 0)
		{
			int nIndex = vBlankedList[RandomUtils.Range(0, vBlankedList.Count)];
			if (m_pPlayerData.m_vLevelActivated[nIndex])
				HTDebug.PrintLog(eMessageType.Error, "[GameFramework] The already cleared level has been selected again!");

			return nIndex;
		}

		return -1;
	}

	//---------------------------------------
	public void SetObjectPositionAndRotateByPhysic(GameObject pTarget, Vector3 vPos)
	{
		RaycastHit pHit;
		if (GetNearestRayCast(vPos, out pHit))
		{
			pTarget.transform.position = pHit.point + (Vector3.up * 0.1f);
			pTarget.transform.up = pHit.normal;
		}
		else
			pTarget.transform.position = vPos;
	}

	public Vector3 GetPositionByPhysic(Vector3 vPos)
	{
		RaycastHit pHit;
		if (GetNearestRayCast(vPos, out pHit))
			return pHit.point + (Vector3.up * 0.1f);

		return vPos;
	}

	private bool GetNearestRayCast(Vector3 vPos, out RaycastHit pHit)
	{
		Ray pCastRay = new Ray();
		pCastRay.origin = vPos + (Vector3.up * 2.0f);
		pCastRay.direction = Vector3.down;

		RaycastHit[] vCastResults = Physics.RaycastAll(pCastRay, 999.0f);

		bool bFound = false;
		float fMinDistance = float.MaxValue;
		RaycastHit pMinCast = new RaycastHit();

		for (int nInd = 0; nInd < vCastResults.Length; ++nInd)
		{
			if (vCastResults[nInd].collider.GetComponent<HT.MousePickableObject>() != null)
			{
				float fDist = Vector3.Distance(pCastRay.origin, vCastResults[nInd].point);
				if (fDist > fMinDistance)
					continue;

				bFound = true;
				fMinDistance = fDist;
				pMinCast = vCastResults[nInd];
			}
		}

		pHit = pMinCast;
		return bFound;
	}

	//---------------------------------------
}