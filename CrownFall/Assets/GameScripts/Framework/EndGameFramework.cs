using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public class EndGameFramework : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	public static bool _isEnableEndingCutscene = false;


	/////////////////////////////////////////
	//---------------------------------------
	public GameObject m_pMask;
	public EndGameInfoViewer _endGameViewer = null;
	public Button _nextButton = null;
	public CanvasGroup _uiCanvas = null;

	//---------------------------------------
	[SerializeField]
	private Animation _endingAnimation = null;

	//---------------------------------------
	private Animation m_pMaskAnimation;
	private const string m_szMaskAnim_FadeIn = "FadeIn";
	private const string m_szMaskAnim_FadeOut = "FadeOut";

	private int m_nRecordIndex = 0;

	//---------------------------------------
	private bool _isEnableNext = false;


	/////////////////////////////////////////
	//---------------------------------------
	// Use this for initialization
	void Start()
	{
		_endGameViewer.gameObject.SetActive(true);
		_nextButton.gameObject.SetActive(true);

		m_pMask.SetActive(true);
		m_pMaskAnimation = m_pMask.GetComponent<Animation>();

		//-----
		_uiCanvas.alpha = 1.0f;

		//-----
		UpdateRecords();
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void CallNextLevelRecord()
	{
		if (_isEnableNext == false)
			return;

		++m_nRecordIndex;

		//-----
		GameFramework pGame = GameFramework._Instance;

		bool bReturnToLobby = false;
		if (m_nRecordIndex >= pGame.m_pPlayerData.m_vBossKillRecord.Length)
			bReturnToLobby = true;

		else
		{
			for (int nInd = 0; nInd < pGame.m_pPlayerData.m_vBossKillRecord.Length; ++nInd)
			{
				if (pGame.m_pPlayerData.m_vBossKillRecord[nInd].nBossIndex == -1)
				{
					bReturnToLobby = true;
					break;
				}
			}
		}

		//-----

		if (bReturnToLobby)
			StartCoroutine(CanvasGroupOff_Internal());
		else
			FadeOut();
	}

	//---------------------------------------
	void FadeIn()
	{
		m_pMaskAnimation.Play(m_szMaskAnim_FadeIn);
		Invoke("DisableMask", 2.0f);
	}

	void FadeOut()
	{
		EnableMask();

		m_pMaskAnimation.Play(m_szMaskAnim_FadeOut);
		Invoke("UpdateRecords", 2.0f);
	}

	private IEnumerator CanvasGroupOff_Internal()
	{
		EnableMask();
		m_pMaskAnimation.Play(m_szMaskAnim_FadeOut);

		yield return new WaitForSeconds(2.0f);

		_endGameViewer.gameObject.SetActive(false);
		_nextButton.gameObject.SetActive(false);

		if (_isEnableEndingCutscene)
		{
			HT.HTSoundManager.FadeOutMusic();
			_endingAnimation.Play();

			//-----
			float fTime = 2.0f;
			while (fTime > 0.0f)
			{
				fTime -= HT.TimeUtils.GameTime;

				_uiCanvas.alpha = fTime * 0.5f;

				yield return new WaitForEndOfFrame();
			}
			
			_uiCanvas.alpha = 0.0f;
		}

		if (_isEnableEndingCutscene == false)
			OnAnimationEnd();
	}

	//---------------------------------------
	void EnableMask()
	{
		m_pMask.SetActive(true);
		_isEnableNext = false;
	}

	void DisableMask()
	{
		m_pMask.SetActive(false);
		_isEnableNext = true;
	}

	//---------------------------------------
	void UpdateRecords()
	{
		_endGameViewer.Init(m_nRecordIndex);
		FadeIn();
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void OnAnimationEnd()
	{
		HT.HTFramework.Instance.SceneChange(GameDefine.szMainTitleSceneName, 0.5f);
	}
}
