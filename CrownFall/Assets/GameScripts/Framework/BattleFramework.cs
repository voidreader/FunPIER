using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using HT;


/////////////////////////////////////////
//---------------------------------------
public sealed class BattleFramework : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	static public BattleFramework _Instance;
	public Action<Projectile> onPlayerAttack = null;
	public Action<int> onPlayerDamaged = null;
	public Action<bool> onBattleEnd = null;


	/////////////////////////////////////////
	//---------------------------------------
	[Header("MAIN ENTITIES")]
	public IActorBase m_pPlayerActor;
	public IActorBase m_pEnemyActor;

	public Field m_pField;

	//---------------------------------------
	public ActorBuff[] m_vAddPatternBuffs;

	//---------------------------------------
	[Header("LOBBY OBJECT")]
	public GameObject _lobbyOnlyObjects = null;
	public GameObject _lobby_FirstSelectObj = null;
	public Button _lobby_ToStateSelectBtn = null;

	[Header("GAME OBJECT")]
	public CanvasGroup _gameOnlyObjects = null;
	public GameObject _game_FirstSelectObj = null;

	[Header("END GAME OBJECTS")]
	public CanvasGroup _endGameCanvas = null;
	public EndGameInfoViewer _endGameViewer = null;
	public AudioClip _endGame_Sound_Victory = null;
	public AudioClip _endGame_Sound_Defeat = null;

	//---------------------------------------
	[Header("UI OBJECTS")]
	public CanvasGroup _blackMask = null;
	public MiniBuffGage m_pPlayer_MiniBuffGage;
	public InteractionBtnView _ui_InteractBtnView = null;
	public GameObject _ui_padArrowShake = null;

	public BossStatusView _ui_bossStatusView = null;

	[Header("GAME OBJECTS")]
	public Button _endGameButton = null;
	public GameObject m_pPlayer_DamageAlert;
	public GameObject m_pPlayer_UI_DamageAlert;
	public AudioClip m_pPlayer_DamageSounds;

	//---------------------------------------
	[Header("GLOBAL INSTANCE")]
	public Object_AreaAlert m_pGlobalObj_AreaAlert;
	public Object_AreaAlert m_pGlobalObj_AreaSafty;
	public Object_AreaAlert m_pGlobalObj_Interaction;

	public Object_AreaAlert m_pGlobalObj_AreaAlert_Rect;
	public Object_AreaAlert m_pGlobalObj_AreaAlert_Rect_Simple;

	public Object_AreaAlert m_pGlobalObj_AreaAlert_90 = null;
	public Object_AreaAlert m_pGlobalObj_AreaAlert_130 = null;
	public Object_AreaAlert m_pGlobalObj_AreaAlert_180 = null;
	public Object_AreaAlert m_pGlobalObj_AreaAlert_360 = null;

	//---------------------------------------
	[Header("RETRY")]
	public Button _retryButton = null;

	//---------------------------------------
	public enum eBattleState
	{
		eLobby,
		eReady,
		eBattle,
		eGameWin,
		eGameDefeat,
		eEnd,
		eEnd_Wait,
	}
	[Header("ETC SETTINGS")]
	public eBattleState m_eBattleState = eBattleState.eLobby;

	//---------------------------------------
	private HT.UIPopup_MessageBoxParam _backMsgBoxParam = null;

	//---------------------------------------
	//private HT.UIPopup_MessageBoxParam _closeMsgBoxParam = null;

	//---------------------------------------
	private bool m_bFieldEventProcessing = false;
	private int m_nFieldEventIndex = 0;
	private bool m_bWaitEventProcessing = true;

	//---------------------------------------
	private bool _gamePaused = false;
	private bool m_bGameEnd = false;
	private float m_fGameEndWaitTime = 0.0f;
	private string m_szNextSceneName = null;

	public bool GameEnd { get { return m_bGameEnd; } }

	public bool GamePaused
	{
		get { return _gamePaused; }
		set { _gamePaused = value; }
	}

	//---------------------------------------
	private bool _isInLobby = true;
	private bool _endGameButtonClicked = false;

	//---------------------------------------
	private static bool _waitForRetry = false;


	/////////////////////////////////////////
	//---------------------------------------
	private List<GameObject> _autoTargetList = new List<GameObject>();
	public List<GameObject> AutoTargetList { get { return _autoTargetList; } }


	/////////////////////////////////////////
	//---------------------------------------
	public List<Object_AreaAlert> m_vAreaAlertMessage;


	/////////////////////////////////////////
	//---------------------------------------
	void Awake()
	{
		_Instance = this;
		_isInLobby = true;

		//-----
		_lobby_ToStateSelectBtn.onClick.AddListener(() => {
			HTUIManager.OpenMessageBox(_backMsgBoxParam);
		});

		_backMsgBoxParam = new UIPopup_MessageBoxParam()
		{
			eType = eMessageBoxType.Question,
			szSubject = "msgwin_subj_notice",
			szDescription = "ui_lobby_tostageselect_msg",
			szLBtnLocale = "msgwin_btn_ok",
			szRBtnLocale = "msgwin_btn_cancel",
			onLBtnClick = () =>
			{
				HTFramework.Instance.SceneChange(GameDefine.szSelectFieldSceneName);
			},
		};

		//-----
		HTUIManager.RegistOnCloseBtnClicked(()=>
		{
			if (_isInLobby == false)
				return;
			
			HTUIManager.OpenMessageBox(_backMsgBoxParam);
		});

		//-----
		GameFramework pGame = GameFramework._Instance;
		pGame.m_pPlayerData.Initialized = true;

		LevelSettings pLevel = pGame.m_vLevelSettings[pGame.m_pPlayerData.m_nActivatedLevel];
		PlayerData pData = pGame.m_pPlayerData;

		//----- Instantiate
		if (m_pField == null)
			m_pField = HT.Utils.Instantiate(pLevel.m_pField);

		m_pField.m_bIsLobbyField = true;
		m_pField.Init();

		AudioClip pClip = pGame.m_vLevelSettings[pGame.m_pPlayerData.m_nActivatedLevel]._lobbyBGM;
		HT.HTSoundManager.PlayMusic(pClip, pGame.m_vLevelSettings[pGame.m_pPlayerData.m_nActivatedLevel]._lobbyBGM_Volume);

		_lobbyOnlyObjects.SetActive(true);
		_gameOnlyObjects.gameObject.SetActive(false);
		_endGameCanvas.gameObject.SetActive(false);

		m_pPlayerActor = HT.Utils.Instantiate(pLevel.m_pPlayerActor);
		m_pEnemyActor = HT.Utils.Instantiate(pLevel.m_pEnemyActor);

		//----- Player Status Update
		m_pPlayerActor.m_pActorInfo.m_nCalculatedHP = GameDefine.nPlayer_Base_HP;
		m_pPlayerActor.m_pActorInfo.m_nCalculatedHP += pData.GetUpgrades(PlayerData.ePlayerUpgrades.eHealth);

		int nSpeedUpgrade = pData.GetUpgrades(PlayerData.ePlayerUpgrades.eSpeed);
		float fSpeedRatio = 1.0f + (nSpeedUpgrade * GameDefine.fMoveSpeedIncreaseRatio);
		m_pPlayerActor.m_pActorInfo.m_fCalculatedMoveSpeed = m_pPlayerActor.m_pActorInfo.m_fBaseMoveSpeed * fSpeedRatio;

		//----- Boss Status Update
		int nDifficulty = (int)pData.m_eDifficulty;
		float fBossBaseHealth = (float)m_pEnemyActor.m_pActorInfo.m_nBaseHP;

		int nClearedStage = 0;
		for (int nInd = 0; nInd < pData.m_vBossKillRecord.Length; ++nInd)
		{
			if (pData.m_vBossKillRecord[nInd].bSetInfomationComplete)
				++nClearedStage;
			else
				break;
		}

		//-----
		float fHPIncRatio = 1.0f;

		if (GameDefine.fBoss_Base_HP_IncreaseRatio > 0.0f)
			fHPIncRatio += GameDefine.fBoss_Base_HP_IncreaseRatio * nClearedStage;

		fHPIncRatio += GameDefine.fBoss_Base_HP_IncreaseRatio_Difficult * nDifficulty;

		m_pEnemyActor.m_pActorInfo.m_nCalculatedHP = (int)(fBossBaseHealth * fHPIncRatio);
		m_pEnemyActor.m_pActorInfo.m_fCalculatedMoveSpeed = m_pEnemyActor.m_pActorInfo.m_fBaseMoveSpeed;

		//----- Boss Kill Record Select
		int nRecordIndex = 0;
		for (; nRecordIndex < pData.m_vBossKillRecord.Length; ++nRecordIndex)
		{
			if (pData.m_vBossKillRecord[nRecordIndex].bSetInfomationComplete == false)
				break;
		}

		if (nRecordIndex >= pData.m_vBossKillRecord.Length)
		{
			HT.HTDebug.PrintLog(HT.eMessageType.Warning, "[BattleFramework] Boss kill record index fixed!");

			nRecordIndex = pData.m_vBossKillRecord.Length - 1;
			pData.m_vBossKillRecord[nRecordIndex].bSetInfomationComplete = false;
		}

		pData.m_nKillRecordWriteIndex = nRecordIndex;
        pData.m_vBossKillRecord[nRecordIndex].Reset();
        pData.m_vBossKillRecord[nRecordIndex].nBossIndex = pData.m_nActivatedLevel;

		if (pData._isHardCoreMode)
			pData.m_vBossKillRecord[nRecordIndex].bSetInfomationComplete = true;

		//----- Init
		m_pPlayerActor.Init();
		m_pEnemyActor.Init();

		pLevel._inGameBGM.LoadAudioData();

		//-----
#if UNITY_EDITOR
		nClearedStage = (GameFramework._Instance._enableAllAddonPattern) ? m_vAddPatternBuffs.Length : (nClearedStage + 1) / GameDefine.nBoss_AddPattern_ByStage;
#else // UNITY_EDITOR
		nClearedStage = (nClearedStage + 1) / GameDefine.nBoss_AddPattern_ByStage;
#endif // UNITY_EDITOR

		for (int nInd = 0; nInd < nClearedStage; ++nInd)
		{
			if (nClearedStage >= m_vAddPatternBuffs.Length)
				break;

			m_pEnemyActor.AddActorBuff(m_vAddPatternBuffs[nInd]);
		}

		HT.HTDebug.PrintLog(HT.eMessageType.None, string.Format("[BattleFramework] Boss has add pattern {0}", nClearedStage));

		//-----
		m_pPlayerActor.transform.position = m_pField.m_pPlayerLobbyPos.transform.position;
		m_pPlayerActor.transform.rotation = m_pField.m_pPlayerLobbyPos.transform.rotation;
		m_pPlayerActor.m_vViewVector = m_pField.m_pPlayerLobbyPos.transform.right;

		if (string.IsNullOrEmpty(pLevel._playerLobbyAnimName) == false)
		{
			m_pPlayerActor.SetAction(pLevel._playerLobbyAnimName);
			m_pPlayerActor.SetActionReadyTime(float.PositiveInfinity);
		}

		m_pEnemyActor.transform.position = m_pField.m_pBossStartPos.transform.position;
		m_pEnemyActor.transform.rotation = m_pField.m_pBossStartPos.transform.rotation;
		m_pEnemyActor.m_vViewVector = m_pField.m_pBossStartPos.transform.right;

		//-----
		m_pEnemyActor.m_pAnimations.Play("EVENT_LOBBY");

		//-----
		CameraManager._Instance.m_pTargetEntity = m_pPlayerActor.gameObject;
		CameraManager._Instance.ClearCameraShake();

		//-----
		HT.HTInputManager.Instance.FirstSelectObject = _lobby_FirstSelectObj;

		_blackMask.alpha = 0.0f;
		_blackMask.gameObject.SetActive(false);

		//-----
		_ui_bossStatusView.UpdateAllData(pLevel, m_pEnemyActor as AIActor);

		//-----
		HT.HTPlatform_LeaderBoardViewer.LeaderboardID = pLevel.LeaderBoardID;
		HT.HTThirdParty_Adbrix.Instance.OnSubmitFirstTimeEvent(string.Format("Lobby_{0}", pLevel._fieldName));

		//-----
		_retryButton.onClick.AddListener(OnClickedRetry);

		//-----
		if (_waitForRetry)
		{
			_waitForRetry = false;
			StartRetry();
		}
	}

	//---------------------------------------
	public void StartBattle()
	{
		StartCoroutine(StartBattle_Internal(4.0f));
	}

	public void StartRetry()
	{
		StartCoroutine(StartBattle_Initialize_Internal());
	}

	private IEnumerator StartBattle_Internal(float fTime)
	{
		HT.HTInputManager.Instance.FirstSelectObject = _game_FirstSelectObj;
		HT.HTSoundManager.StopMusic(false);

		_isInLobby = false;

		//-----
		_blackMask.gameObject.SetActive(true);
		float fLeastTime = fTime;
		while (fLeastTime > 0.0f)
		{
			fLeastTime -= HT.TimeUtils.GameTime;
			_blackMask.alpha = 1.0f - (fLeastTime / fTime);
			yield return new WaitForEndOfFrame();
		}

		//-----
		yield return StartCoroutine(StartBattle_Initialize_Internal());
	}

	private IEnumerator StartBattle_Initialize_Internal()
	{
		HTUIManager.RegistOnCloseBtnClicked(null);

		m_pField.m_bIsLobbyField = false;
		m_pField.Init();

		PlayerData pData = GameFramework._Instance.m_pPlayerData;
		pData.m_vTryCount[pData.m_nActivatedLevel] += 1;

		_lobbyOnlyObjects.SetActive(false);
		_endGameCanvas.gameObject.SetActive(false);
		_gameOnlyObjects.gameObject.SetActive(true);
		_gameOnlyObjects.alpha = 1.0f;

		m_pPlayerActor.transform.position = m_pField.m_pPlayerStartPos.transform.position;
		m_pPlayerActor.transform.rotation = m_pField.m_pPlayerStartPos.transform.rotation;
		m_pPlayerActor.m_vViewVector = m_pField.m_pPlayerStartPos.transform.right;

		if (m_pField.m_vFieldEvents.Length > 0)
		{
			m_bFieldEventProcessing = true;

			m_nFieldEventIndex = 0;
			m_bWaitEventProcessing = true;

			m_pField.m_vFieldEvents[0].Init();
		}

		m_eBattleState = eBattleState.eReady;

		//-----
		float fLeastTime = GEnv.GLOBALUI_DEFAULT_FADETIME;
		while (fLeastTime > 0.0f)
		{
			fLeastTime -= HT.TimeUtils.GameTime;
			_blackMask.alpha = fLeastTime / GEnv.GLOBALUI_DEFAULT_FADETIME;
			yield return new WaitForEndOfFrame();
		}

		_blackMask.alpha = 0.0f;
		_blackMask.gameObject.SetActive(false);

		//-----
		GameFramework pGame = GameFramework._Instance;
		if (pGame.m_pPlayerData._isHardCoreMode)
			pGame.m_pPlayerData.Initialized = false;

		//-----
		m_pPlayerActor.SetAction(m_pPlayerActor.m_szIDLEAnimName);
		m_pPlayerActor.SetActorState(IActorBase.eActorState.eIdle);
		m_pPlayerActor.SetActionReadyTime(0.0f);

		//-----
		HT.HTThirdParty_Adbrix.Instance.OnSubmitEvent(CompareEventID("START"));

		m_pPlayerActor.OnBattleStart();
		m_pEnemyActor.OnBattleStart();
	}

	//---------------------------------------
	void Update()
	{
		GameFramework pGame = GameFramework._Instance;
		HT.HTInputManager pInputMan = HT.HTInputManager.Instance;

		//-----
		switch (m_eBattleState)
		{
			case eBattleState.eLobby:
				{
				}
				return;

			case eBattleState.eReady:
				{
					if (m_bFieldEventProcessing)
					{
						if (m_bWaitEventProcessing == false)
						{
							//-----
							if (m_nFieldEventIndex >= 0 && m_pField.m_vFieldEvents.Length > m_nFieldEventIndex)
								m_pField.m_vFieldEvents[m_nFieldEventIndex].Release();

							++m_nFieldEventIndex;

							if (m_pField.m_vFieldEvents.Length > m_nFieldEventIndex)
							{
								m_bWaitEventProcessing = true;

								m_pField.m_vFieldEvents[m_nFieldEventIndex].Init();

							}
							else
								m_bFieldEventProcessing = false;

							//-----

						}
						else
						{
							if (m_pField.m_vFieldEvents[m_nFieldEventIndex].Frame() == false)
								m_bWaitEventProcessing = false;
						}

					}
					else
                    {
						Action onTutorialEnd = () =>
                        {
                            CameraManager._Instance.m_pMainCamera = m_pField.m_pGameCamera;

                            CameraManager._Instance.m_bCamFollowing = true;
                            CameraManager._Instance.m_pTargetEntity = m_pPlayerActor.gameObject;

                            //-----
                            AudioClip pClip = pGame.m_vLevelSettings[pGame.m_pPlayerData.m_nActivatedLevel]._inGameBGM;
                            HT.HTSoundManager.PlayMusic(pClip, pGame.m_vLevelSettings[pGame.m_pPlayerData.m_nActivatedLevel]._inGameBGM_Volume);

                            //-----
                            m_eBattleState = eBattleState.eBattle;

                            m_pField.OnBattleStart();
						};

                        //-----
                        if (UIPopup_Tutorial.TutorialOpened == false)
                        {
                            bool bNeedShowTutorial = false;
                            if (HT.HTAfxPref.IsMobilePlatform == false)
                            {
                                if (pInputMan.JoystickConnected == false && pGame._tutorialInfo_Keyboard == false)
                                {
                                    bNeedShowTutorial = true;
									pGame._tutorialInfo_Keyboard = true;
                                }
                                else if (pInputMan.JoystickConnected && pGame._tutorialInfo_Joystick == false)
                                {
                                    bNeedShowTutorial = true;
									pGame._tutorialInfo_Joystick = true;
                                }
                            }
                            else
                            {
                                if (pGame._tutorialInfo_Mobile == false)
                                {
                                    bNeedShowTutorial = true;
									pGame._tutorialInfo_Mobile = true;
                                }
                            }

                            if (bNeedShowTutorial)
                            {
                                UIPopup_Tutorial pTutorialUI = HTUIManager.OpenPopup("Popup_Tutorial") as UIPopup_Tutorial;

								bool bShowGameTutorial = (pGame._tutorialInfo_Game)? false : true;
								pGame._tutorialInfo_Game = true;

								pTutorialUI.InitTutorial(bShowGameTutorial, onTutorialEnd);
                            }
                            else
                                HT.Utils.SafeInvoke(onTutorialEnd);
                        }

						//-----
					}
				}
				break;

			case eBattleState.eBattle:
				{
					BossKillRecord pRecord = pGame.m_pPlayerData.m_vBossKillRecord[pGame.m_pPlayerData.m_nKillRecordWriteIndex];
					if (GamePaused == false)
					{
						m_pPlayerActor.Frame();
						m_pEnemyActor.Frame();

						pRecord.fTime += HT.TimeUtils.GameTime;
					}

					//-----
					bool bGameEnd = false;

					//----- Game Win
					if (m_pEnemyActor.m_pActorInfo.m_cnNowHP.val <= 0 && m_pPlayerActor.m_pActorInfo.m_cnNowHP.val > 0)
					{
						bGameEnd = true;
						m_eBattleState = eBattleState.eGameWin;
						pRecord.bVictory = true;

						if (m_pPlayerActor.GetActorState() == IActorBase.eActorState.eMove)
							m_pPlayerActor.SetAction(m_pPlayerActor.m_szIDLEAnimName);

						//-----
						m_pEnemyActor.SetAction(m_pEnemyActor.m_szDEATHAnimName);
						++pGame.m_pPlayerData.m_nUpgradePoint;

						//-----
						HT.Utils.SafeInvoke(onBattleEnd, true);
						m_pField.OnBattleEnd(true);

						m_pPlayerActor.OnBattleEnd(true);
						m_pEnemyActor.OnBattleEnd(true);

						m_pPlayerActor.SetAction(m_pPlayerActor.m_szIDLEAnimName);

						//-----
						float fTime = pRecord.fTime;
#if UNITY_ANDROID
						fTime *= 1000.0f;
#endif // UNITY_ANDROID
						
						HT.HTThirdParty_Adbrix.Instance.OnSubmitEvent(CompareEventID("WIN"));

						LevelSettings pLevel = pGame.m_vLevelSettings[pGame.m_pPlayerData.m_nActivatedLevel];
						HT.HTPlatform.Instance.SubmitLeaderBoardScore(pLevel.LeaderBoardID, (int)fTime);
					}

					//----- Game Defeat
					if (m_pPlayerActor.m_pActorInfo.m_cnNowHP.val <= 0)
					{
						bGameEnd = true;
						m_eBattleState = eBattleState.eGameDefeat;
						pRecord.bVictory = false;

						if (m_pEnemyActor.GetActorState() == IActorBase.eActorState.eMove)
							m_pEnemyActor.SetAction(m_pEnemyActor.m_szIDLEAnimName);

						//-----
						m_pPlayerActor.SetAction(m_pPlayerActor.m_szDEATHAnimName);

						//-----
						//m_pEnemyActor.SetAction(m_pEnemyActor.m_szIDLEAnimName);
						//m_pEnemyActor.transform.position = GameFramework._Instance.GetPositionByPhysic(m_pEnemyActor.transform.position);

						//-----
						m_pEnemyActor.m_pRigidBody.velocity = Vector3.zero;
						m_pEnemyActor.m_pRigidBody.angularVelocity = Vector3.zero;

						//-----
						HT.Utils.SafeInvoke(onBattleEnd, false);
						m_pField.OnBattleEnd(false);

						m_pPlayerActor.OnBattleEnd(false);
						m_pEnemyActor.OnBattleEnd(false);

						//-----
						HTThirdParty_Adbrix.Instance.OnSubmitEvent(CompareEventID("DEFEAT"));
					}

					if (bGameEnd)
					{
						m_pEnemyActor.m_vMoveVector = Vector3.zero;
						if (m_pEnemyActor.m_pRigidBody != null)
						{
							m_pEnemyActor.m_pRigidBody.velocity = Vector3.zero;
							m_pEnemyActor.m_pRigidBody.useGravity = true;
						}

						m_pPlayerActor.m_pRigidBody.velocity = Vector3.zero;
						m_pPlayerActor.m_pRigidBody.angularVelocity = Vector3.zero;

						if (m_pPlayerActor.GetCurrHP() > 0)
							m_pPlayerActor.SetAction(m_pPlayerActor.m_szIDLEAnimName);

						HTSoundManager.StopMusic(false);
						m_fGameEndWaitTime = 5.0f;
					}
				}
				break;

			case eBattleState.eGameWin:
				{
					if (m_bGameEnd == false)
					{

					}
					else
					{
#if DEMO_VERSION
						ShowDemoMessageBox();
#endif // DEMO_VERSION

						m_pPlayerActor.SetAction(m_pPlayerActor.m_szIDLEAnimName);

						PlayerData pData = GameFramework._Instance.m_pPlayerData;
						pData.m_vLevelActivated[pData.m_nActivatedLevel] = true;

						int nActiveLevel = pGame.SelectRandomLevel();

						//-----
						if (nActiveLevel < 0)
						{
							m_szNextSceneName = GameDefine.szEndGameSceneName;
							GameFramework._Instance._hardMode_Opened = true;
							EndGameFramework._isEnableEndingCutscene = true;

							pData.Initialized = false;
							pData._battleIsEnd = true;
						}
						else
                            m_szNextSceneName = GameDefine.szSelectFieldSceneName;

						//-----
						_retryButton.interactable = false;

						pData.m_nActivatedLevel = -1;
                        m_eBattleState = eBattleState.eEnd;

						//-----
						if (pGame.m_pPlayerData._isHardCoreMode == false)
						{
							BossKillRecord pRecord = pGame.m_pPlayerData.GetLastKillRecord();
							pRecord.bSetInfomationComplete = true;
						}
					}
				}
				break;

			case eBattleState.eGameDefeat:
				{
					if (m_bGameEnd == false)
					{

					}
					else
					{
#if DEMO_VERSION
						ShowDemoMessageBox();
#endif // DEMO_VERSION

						if (pGame.m_pPlayerData._isHardCoreMode)
						{
							pGame.m_pPlayerData.Initialized = false;

							m_szNextSceneName = GameDefine.szEndGameSceneName;
							EndGameFramework._isEnableEndingCutscene = false;
						}
						else
						{
							pGame.m_pPlayerData.Initialized = true;
							m_szNextSceneName = pGame.m_vLevelSettings[pGame.m_pPlayerData.m_nActivatedLevel].GetLevelName();
						}

						//-----
						if (pGame.m_pPlayerData._isHardCoreMode)
							_retryButton.interactable = false;
						else
							_retryButton.interactable = true;

						//-----
						m_eBattleState = eBattleState.eEnd;
					}
				}
				break;

			case eBattleState.eEnd:
				{
					//-----
					if (m_pPlayerActor.GetCurrHP() <= 0)
					{
						HT.HTSoundManager.PlaySound(_endGame_Sound_Defeat);
					}
					else
					{
						HT.HTGameAudioMisc pAudio = HT.HTSoundManager.PlayMusic(_endGame_Sound_Victory);
						pAudio.AudioSource.loop = false;
					}

					//-----
					pGame.SaveGameData();

					m_pPlayerActor.Release();
					m_pEnemyActor.Release();

					//-----
					CameraManager._Instance.m_pMainCamera = null;

					CameraManager._Instance.m_bCamFollowing = false;
					CameraManager._Instance.m_pTargetEntity = null;

					//-----
					if (pGame.m_pPlayerData.Initialized)
						StartCoroutine(EndGame_Internal());

					else
						HT.HTFramework.Instance.SceneChange(m_szNextSceneName, 3.0f);

					//-----
					m_eBattleState = eBattleState.eEnd_Wait;
				}
				break;

			case eBattleState.eEnd_Wait:
				break;
		}

		//-----
		if (m_eBattleState == eBattleState.eGameWin || m_eBattleState == eBattleState.eGameDefeat)
		{
			m_fGameEndWaitTime -= HT.TimeUtils.GameTime;

			if (m_fGameEndWaitTime <= 0.0f)
				m_bGameEnd = true;
		}
	}

	//---------------------------------------
	void OnDestroy()
	{
		//-----
		_Instance = null;

		//-----
		if (CameraManager._Instance != null)
		{
			CameraManager._Instance.m_pMainCamera = null;
			CameraManager._Instance.m_pTargetEntity = null;
		}
	}

	/////////////////////////////////////////
	//---------------------------------------
	public Object_AreaAlert CreateAreaAlert(Vector3 vPos, float fRadius, float fTime, bool bAddedArrow = false)
	{
		Object_AreaAlert pNewAlert = CreateAreaMessage(m_pGlobalObj_AreaAlert, vPos, fRadius, fTime);
		pNewAlert.m_eAlertType = Object_AreaAlert.eAreaAlertType.Warnning;

		if (bAddedArrow)
			m_vAreaAlertMessage.Add(pNewAlert);

		return pNewAlert;
	}

	public Object_AreaAlert CreateAreaSafty(Vector3 vPos, float fRadius, float fTime, bool bAddedArrow = true)
	{
		Object_AreaAlert pNewAlert = CreateAreaMessage(m_pGlobalObj_AreaSafty, vPos, fRadius, fTime);
		pNewAlert.m_eAlertType = Object_AreaAlert.eAreaAlertType.Safety;

		if (bAddedArrow)
			m_vAreaAlertMessage.Add(pNewAlert);

		return pNewAlert;
	}

	//---------------------------------------
	Object_AreaAlert CreateAreaMessage(Object_AreaAlert pInstance, Vector3 vPos, float fRadius, float fTime)
	{
		Object_AreaAlert pAlert = HT.Utils.InstantiateFromPool(pInstance);
		GameFramework._Instance.SetObjectPositionAndRotateByPhysic(pAlert.gameObject, vPos);

		Vector3 vScale = pAlert._defaultScale;
		vScale *= fRadius;
		pAlert.m_pRoot.transform.localScale = vScale;

		pAlert.Init(fTime);
		return pAlert;
	}


	/////////////////////////////////////////
	//---------------------------------------
	public Object_AreaAlert CreateAreaAlert(eAlertRingType eType, Vector3 vPos, float fRadius, float fTime, bool bAddedArrow = false)
	{
		Object_AreaAlert pAlertObj = null;
		switch (eType)
		{
			case eAlertRingType.Angle360_Simple:
				pAlertObj = m_pGlobalObj_AreaAlert_360;
				break;

			case eAlertRingType.Angle360:
				pAlertObj = m_pGlobalObj_AreaAlert;
				break;

			case eAlertRingType.Angle180:
				pAlertObj = m_pGlobalObj_AreaAlert_180;
				break;

			case eAlertRingType.Angle130:
				pAlertObj = m_pGlobalObj_AreaAlert_130;
				break;

			case eAlertRingType.Angle90:
				pAlertObj = m_pGlobalObj_AreaAlert_90;
				break;
		}

		Object_AreaAlert pNewAlert = CreateAreaMessage(pAlertObj, vPos, fRadius, fTime);
		pNewAlert.m_eAlertType = Object_AreaAlert.eAreaAlertType.Warnning;

		if (bAddedArrow)
			m_vAreaAlertMessage.Add(pNewAlert);

		return pNewAlert;
	}


	/////////////////////////////////////////
	//---------------------------------------
	public Object_AreaAlert CreateAreaAlert(Vector3 vPos, float fX, float fY, float fTime, bool bAddedArrow = false)
	{
		Object_AreaAlert pNewAlert = CreateAreaMessage(m_pGlobalObj_AreaAlert_Rect, vPos, fX, fY, fTime);
		pNewAlert.m_eAlertType = Object_AreaAlert.eAreaAlertType.Warnning;

		if (bAddedArrow)
			m_vAreaAlertMessage.Add(pNewAlert);

		return pNewAlert;
	}

	public Object_AreaAlert CreateAreaAlert_Simple(Vector3 vPos, float fX, float fY, float fTime, bool bAddedArrow = false)
	{
		Object_AreaAlert pNewAlert = CreateAreaMessage(m_pGlobalObj_AreaAlert_Rect_Simple, vPos, fX, fY, fTime);
		pNewAlert.m_eAlertType = Object_AreaAlert.eAreaAlertType.Warnning;

		if (bAddedArrow)
			m_vAreaAlertMessage.Add(pNewAlert);

		return pNewAlert;
	}

	Object_AreaAlert CreateAreaMessage(Object_AreaAlert pInstance, Vector3 vPos, float fX, float fY, float fTime)
	{
		Object_AreaAlert pAlert = HT.Utils.InstantiateFromPool(pInstance);
		GameFramework._Instance.SetObjectPositionAndRotateByPhysic(pAlert.gameObject, vPos);

		Vector3 vScale = pAlert._defaultScale;
		vScale.x *= fX;
		vScale.y *= fY;
		pAlert.m_pRoot.transform.localScale = vScale;

		pAlert.Init(fTime);
		return pAlert;
	}

	//---------------------------------------
	public Object_AreaAlert CreateInteractionNotice(Vector3 vPos)
	{
		Object_AreaAlert pNewAlert = CreateAreaMessage(m_pGlobalObj_Interaction, vPos, 0.0f, float.PositiveInfinity);
		pNewAlert.m_eAlertType = Object_AreaAlert.eAreaAlertType.Interaction;

		m_vAreaAlertMessage.Add(pNewAlert);

		return pNewAlert;
	}

	public void RemoveInteractionNotice(Object_AreaAlert pObj)
	{
		RemoveAreaAlertMessage(pObj);
	}

	//---------------------------------------
	public void RemoveAreaAlertMessage(Object_AreaAlert pObj)
	{
		for (int nInd = 0; nInd < m_vAreaAlertMessage.Count; ++nInd)
		{
			if (m_vAreaAlertMessage[nInd] == pObj)
				m_vAreaAlertMessage.RemoveAt(nInd);
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
#if DEMO_VERSION
	public void ShowDemoMessageBox()
	{
#if UNITY_ANDROID
		HT.UIPopup_MessageBoxParam pParam = new HT.UIPopup_MessageBoxParam();
		pParam.Init(HT.eMessageBoxType.Exclamation, "msgwin_subj_notice", "ui_demo_messagedesc", "msgwin_btn_ok", "msgwin_btn_cancel", () =>
		{
			Application.OpenURL("market://details?q=pname:com.pier.CrownFall/");
		});
		
		HT.HTUIHelper.OpenMessageBox(pParam);
#endif // UNITY_ANDROID
	}
#endif // DEMO_VERSION


	/////////////////////////////////////////
	//---------------------------------------
	public void AddAutoTargetObject(GameObject pObject)
	{
		if (_autoTargetList.Contains(pObject) == false)
			_autoTargetList.Add(pObject);
	}

	public void RemoveAutoTargetObject(GameObject pObject)
	{
		_autoTargetList.Remove(pObject);
	}


	/////////////////////////////////////////
	//---------------------------------------
	private IEnumerator EndGame_Internal()
	{
		_endGameViewer.Init(GameFramework._Instance.m_pPlayerData.m_nKillRecordWriteIndex);

		//-----
		float fGameUIAlpha = _gameOnlyObjects.alpha;

		_endGameCanvas.gameObject.SetActive(true);
		_endGameCanvas.alpha = 0.0f;

		float fTime = 1.0f;
		while(fTime > 0.0f)
		{
			fTime -= HT.TimeUtils.GameTime;
			_endGameCanvas.alpha = 1.0f - fTime;
			_gameOnlyObjects.alpha = fTime * fGameUIAlpha;

			yield return new WaitForEndOfFrame();
		}

		_endGameCanvas.alpha = 1.0f;
		_gameOnlyObjects.alpha = 0.0f;
		_gameOnlyObjects.gameObject.SetActive(false);
		
		yield return new WaitForSeconds(1.0f);

		//-----
		_endGameButtonClicked = false;
		while(_endGameButtonClicked == false)
		{
			if (HT.HTInputManager.Instance.JoystickConnected && UnityEngine.EventSystems.EventSystem.current != _endGameButton.gameObject)
				UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(_endGameButton.gameObject);

			yield return new WaitForEndOfFrame();
		}

		//-----
		HT.HTSoundManager.StopMusic(false);
		HT.HTFramework.Instance.SceneChange(m_szNextSceneName, 3.0f);
	}

	public void SetEndGame_OnClicked()
	{
		_endGameButtonClicked = true;
	}

	//---------------------------------------
	public Vector3 FindNearestAutoTarget(Vector3 vPos, Vector3 vDir, float fMaxDot)
	{
		Vector3 vRetVal = vDir;

		//-----
		float fDotNearest = float.PositiveInfinity;
		GameObject pNearObject = null;
		Vector3 vNearObjectVector = Vector3.zero;

		List<GameObject> vTargetList = BattleFramework._Instance.AutoTargetList;
		for (int nTarget = 0; nTarget < vTargetList.Count; ++nTarget)
		{
			Vector3 vTargetVector = (vTargetList[nTarget].transform.position - vPos).normalized;
			float fDot = Vector3.Dot(vTargetVector, vDir);
			if (fDot < (1.0f - fMaxDot))
				continue;

			if (fDot > fDotNearest)
				continue;

			fDotNearest = fDot;
			pNearObject = vTargetList[nTarget];
			vNearObjectVector = vTargetVector;
		}

		if (pNearObject != null)
		{
			vRetVal = vNearObjectVector;
			vRetVal.y = 0.0f;

			vRetVal.Normalize();
		}

		//-----
		return vRetVal;
	}

	//---------------------------------------
	public string CompareEventID(string szKey)
	{
		GameFramework pGame = GameFramework._Instance;
		LevelSettings pLevel = pGame.m_vLevelSettings[pGame.m_pPlayerData.m_nActivatedLevel];

		string szID = pLevel._fieldName;

		string szDiff = null;
		switch(pGame.m_pPlayerData.m_eDifficulty)
		{
			case eGameDifficulty.eEasy:
				szDiff = "EASY";
				break;

			case eGameDifficulty.eNormal:
				szDiff = "NORMAL";
				break;

			case eGameDifficulty.eHard:
				szDiff = "HARD";
				break;
		}

		return string.Format("{0}_{1}_{2}", szID, szDiff, szKey);
	}

	//---------------------------------------
	public void OnClickedRetry()
	{
		_waitForRetry = true;

		//-----
		m_pPlayerActor.Release();
		m_pEnemyActor.Release();

		m_pField.OnBattleEnd(false);

		//-----
		GameFramework pGame = GameFramework._Instance;
		HT.HTFramework.Instance.SceneChange(pGame.m_vLevelSettings[pGame.m_pPlayerData.m_nActivatedLevel].GetLevelName());
	}

	//---------------------------------------
}
