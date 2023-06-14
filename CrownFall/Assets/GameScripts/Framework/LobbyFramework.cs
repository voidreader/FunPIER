using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using HT;


/////////////////////////////////////////
//---------------------------------------
public class LobbyFramework : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	private bool m_bInitialized = false;

	private Field m_pField;
	private IActorBase m_pPlayer;
	private IActorBase m_pEnemy;

	//---------------------------------------
	private HT.HTKey _closeButton = null;
	private HT.UIPopup_MessageBoxParam _closeMsgBoxParam = null;


	/////////////////////////////////////////
	//---------------------------------------
	private void Start()
	{
		GameFramework pGame = GameFramework._Instance;

		//-----
		GEnv pGEnv = GEnv.Get();
		_closeButton = HT.HTInputManager.Instance.GetKey(pGEnv._input_Interact_Close);

		_closeMsgBoxParam = new HT.UIPopup_MessageBoxParam();
		_closeMsgBoxParam.eType = HT.eMessageBoxType.Question;
		_closeMsgBoxParam.szSubject = "msgwin_subj_notice";
		_closeMsgBoxParam.szDescription = "msgwin_desc_quitegame";
		_closeMsgBoxParam.szLBtnLocale = "msgwin_btn_ok";
		_closeMsgBoxParam.szRBtnLocale = "msgwin_btn_cancel";
		_closeMsgBoxParam.onLBtnClick = () =>
		{
			HT.HTFramework.Instance.GameShutdown();
		};

		//-----
		pGame.m_pPlayerData.Initialized = true;

		//-----
		if (pGame.m_pPlayerData.m_nActivatedLevel >= 0)
		{
			//int nActivateCurLevel = pGame.m_pPlayerData.m_nActivatedLevel;
			//pGame.m_pPlayerData.m_vLevelActivated[nActivateCurLevel] = true;

			LevelSettings pLevel = pGame.m_vLevelSettings[pGame.m_pPlayerData.m_nActivatedLevel];
			m_pField = HT.Utils.Instantiate(pLevel.m_pField);
			m_pField.m_bIsLobbyField = true;
			m_pField.Init();

			m_pPlayer = HT.Utils.Instantiate(pLevel.m_pPlayerActor);
			m_pEnemy = HT.Utils.Instantiate(pLevel.m_pEnemyActor);

			//-----
			HT.HTSoundManager.PlayMusic(pLevel._lobbyBGM);

			//-----
			m_pPlayer.Init();
			m_pEnemy.Init();
		}
		else
		{
		}
	}

	private void Update()
	{
		if (_closeButton != null && _closeButton.IsDown)
		{
			if (HT.HTAfxPref.WaitingForSceneChange == false)
			{
				if (HTUIManager.OpenedPopupCount == 0)
					HTUIManager.OpenMessageBox(_closeMsgBoxParam);
			}
		}
	}

	private void FixedUpdate()
	{
		if (m_bInitialized == false)
		{
			m_bInitialized = true;
			
			if (m_pField != null && m_pPlayer != null && m_pEnemy != null)
			{
				m_pPlayer.transform.position = m_pField.m_pPlayerLobbyPos.transform.position;
				m_pPlayer.transform.rotation = m_pField.m_pPlayerLobbyPos.transform.rotation;

				m_pEnemy.transform.position = m_pField.m_pBossStartPos.transform.position;
				m_pEnemy.transform.rotation = m_pField.m_pBossStartPos.transform.rotation;

				m_pEnemy.m_pAnimations.Play("EVENT_LOBBY");
			}
		}
	}

	//---------------------------------------
	private void OnDestroy()
	{
		HT.HTSoundManager.StopMusic(false);

		//-----
		m_pPlayer.Release();
		m_pEnemy.Release();
	}


	/////////////////////////////////////////
	//---------------------------------------
}
