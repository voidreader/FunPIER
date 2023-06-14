using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public class PlayerHPGage : MonoBehaviour {
	/////////////////////////////////////////
	//---------------------------------------
	public GameObject m_fInst_Heart;
	public bool m_bUsedBattleFramework = true;

	/////////////////////////////////////////
	//---------------------------------------
	PlayerHPGage_Heart [] m_vHearts;

	//---------------------------------------
	public enum eAlign {
		eLeft = 0,
		eMiddle,
		eRight,
	}

	public eAlign m_eAlign = eAlign.eMiddle;

	//---------------------------------------
	bool m_bInitialized = false;
	int m_nPrevHeartCounts;


	/////////////////////////////////////////
	//---------------------------------------
	// Use this for initialization
	void Start () {
		Initialize ();
	}
	
	// Update is called once per frame
	void Update () {
		if (m_bInitialized == false || m_nPrevHeartCounts != GetMaxHP ()) {
			Release ();
			Initialize ();
		}

		//-----
		bool bHasHalf = false;
		int nHeartCount = 0;

		int nNowHP = 0;
		if (m_bUsedBattleFramework) {
			IActorBase pPlayer = BattleFramework._Instance.m_pPlayerActor;
			nNowHP = pPlayer.m_pActorInfo.m_cnNowHP.val;

		} else {
			nNowHP = GameDefine.nPlayer_Base_HP;
			nNowHP += GameFramework._Instance.m_pPlayerData.GetUpgrades (PlayerData.ePlayerUpgrades.eHealth);
		}

		if (nNowHP > 0) {
			bHasHalf = ((nNowHP % 2) == 0)? false : true;
			nHeartCount = (nNowHP / 2) + ((bHasHalf) ? 1 : 0);
		}

		//-----
		for (int nInd = 0; nInd < m_vHearts.Length; ++nInd) {
			PlayerHPGage_Heart pHeart = m_vHearts [nInd];

			if (nInd <= nHeartCount - 1) {
				if ((nInd < nHeartCount - 1) || (bHasHalf == false))
					pHeart.SetState (PlayerHPGage_Heart.eHeartState.eFull, PlayerHPGage_Heart.eEdgeState.eMAX);

				else
					pHeart.SetState (PlayerHPGage_Heart.eHeartState.eHalf, PlayerHPGage_Heart.eEdgeState.eMAX);

			} else
				pHeart.SetState (PlayerHPGage_Heart.eHeartState.eBlank, PlayerHPGage_Heart.eEdgeState.eMAX);
		}
	}

	void Release () {
		m_bInitialized = false;

		if (m_vHearts != null) {
			for (int nInd = 0; nInd < m_vHearts.Length; ++nInd) {
				Destroy (m_vHearts [nInd].gameObject);
			}

			m_vHearts = null;
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	void Initialize () {
		if (m_bInitialized == true)
			return;
		
		int nMaxHP = GetMaxHP ();
		if (nMaxHP < 0)
			return;

		//-----
		Release ();

		m_bInitialized = true;
		m_nPrevHeartCounts = nMaxHP;

		bool bHasHalf = ((nMaxHP % 2) == 0)? false : true;
		int nHeartCount = (nMaxHP / 2) + ((bHasHalf) ? 1 : 0);

		m_vHearts = new PlayerHPGage_Heart [nHeartCount];

		//-----
		RectTransform pRect = GetComponent<RectTransform> ();
		float fBaseWidth = pRect.rect.width;
		pRect.rect.Set (pRect.rect.x, pRect.rect.y, fBaseWidth * nHeartCount, pRect.rect.height);

		float fHeartStartPos = 0.0f;
		switch (m_eAlign) {
		case eAlign.eLeft:
			fHeartStartPos = pRect.position.x;
			break;

		case eAlign.eMiddle:
			fHeartStartPos = pRect.position.x - (fBaseWidth * 0.5f);
			fHeartStartPos -= (nHeartCount * fBaseWidth) * 0.5f;
			break;

		case eAlign.eRight:
			fHeartStartPos = pRect.position.x - (nHeartCount * fBaseWidth);
			break;
		}

		//-----
		for (int nInd = 0; nInd < nHeartCount; ++nInd) {
			GameObject pNewHeart = (GameObject)Instantiate (m_fInst_Heart);
			pNewHeart.transform.SetParent (gameObject.transform);

			PlayerHPGage_Heart pHeart = pNewHeart.GetComponent<PlayerHPGage_Heart> ();
			pHeart.SetState (PlayerHPGage_Heart.eHeartState.eFull, PlayerHPGage_Heart.eEdgeState.eFull);
			m_vHearts [nInd] = pHeart;

			RectTransform pHeartRect = pNewHeart.GetComponent<RectTransform> ();
			Vector3 vHeartPos = pHeartRect.position;
			vHeartPos.x = fHeartStartPos + (fBaseWidth * nInd);
			vHeartPos.y = pRect.position.y;

			pHeartRect.position = vHeartPos;
		}

		//-----
		if (bHasHalf) {
			PlayerHPGage_Heart pLastHeart = m_vHearts [m_vHearts.Length - 1];
			pLastHeart.SetState (PlayerHPGage_Heart.eHeartState.eHalf, PlayerHPGage_Heart.eEdgeState.eHalf);
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	int GetMaxHP () {
		int nMaxHP = 0;

		if (m_bUsedBattleFramework) {
			if (BattleFramework._Instance == null)
				return -1;

			if (BattleFramework._Instance.m_pPlayerActor == null)
				return -1;

			//-----
			IActorBase pPlayer = BattleFramework._Instance.m_pPlayerActor;
			if (pPlayer == null)
				return -1;

			nMaxHP = pPlayer.m_pActorInfo.m_cnMaxHP.val;

		} else {
			nMaxHP = GameDefine.nPlayer_Base_HP;
			nMaxHP += GameFramework._Instance.m_pPlayerData.GetUpgrades (PlayerData.ePlayerUpgrades.eHealth);
		}

		return nMaxHP;
	}
}
