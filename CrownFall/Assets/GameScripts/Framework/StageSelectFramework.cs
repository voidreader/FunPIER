using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HT;


/////////////////////////////////////////
//---------------------------------------
public class StageSelectFramework : MonoBehaviour
{
	//---------------------------------------
	[Header("INFOMATION")]
	[SerializeField]
	private Text _fieldName = null;
	[SerializeField]
	private Text _fieldDescription = null;
	[SerializeField]
	private Image _background = null;
	[SerializeField]
	private Image _fieldImg = null;
	[SerializeField]
	private string _tutorialFieldName = null;

	[Header("OBJECTS")]
	[SerializeField]
	private AudioClip _backgroundMusic = null;
	[SerializeField, Range(0.0f, 1.0f)]
	private float _backGroundMusic_Volume = 1.0f;
	[SerializeField]
	private Button _prevLevel = null;
	[SerializeField]
	private Button _nextLevel = null;
	[SerializeField]
	private Button _selectLevel = null;
	[SerializeField]
	private CanvasGroup _clearMarks = null;
    [SerializeField]
    private CanvasGroup _lockedMarks = null;
	[SerializeField]
	private Button _backButton = null;

	[Header("FIELD SWAP ANIM")]
	[SerializeField]
	private Animation _animation = null;
	[SerializeField]
	private string _anim_Prev = null;
	[SerializeField]
	private string _anim_Next = null;
	[SerializeField]
	private float _animTime = 0.4f;
	[SerializeField]
	private Image _animObj_PrevPrev = null;
	[SerializeField]
	private Image _animObj_Prev = null;
	[SerializeField]
	private Image _animObj_Current = null;
	[SerializeField]
	private Image _animObj_Next = null;
	[SerializeField]
	private Image _animObj_NextNext = null;

	//---------------------------------------
	//private HT.UIPopup_MessageBoxParam _closeMsgBoxParam = null;
	private HT.UIPopup_MessageBoxParam _toTitleMsgBoxParam = null;

	//---------------------------------------
	private int _curFieldIndex = 0;
	private float _clearMarksTargetAlpha = 0.0f;

	private float _waitForAnimation = 0.0f;

	//---------------------------------------
	private void Start()
	{
		if (GameFramework._Instance.m_pPlayerData.m_nActivatedLevel <= 0)
			_curFieldIndex = 0;
		else
		{
			_curFieldIndex = GameFramework._Instance.m_pPlayerData.m_nActivatedLevel;
			GameFramework._Instance.m_pPlayerData.m_nActivatedLevel = -1;
		}

		//-----
		RefreshLevel();
		RefreshAnimationObjects();

		_prevLevel.onClick.AddListener(SubtractCurrentIndex);
		_nextLevel.onClick.AddListener(AddCurrentIndex);
		_selectLevel.onClick.AddListener(SelectStage);

		//-----
		_toTitleMsgBoxParam = new HT.UIPopup_MessageBoxParam();
		_toTitleMsgBoxParam.eType = HT.eMessageBoxType.Question;
		_toTitleMsgBoxParam.szSubject = "msgwin_subj_notice";
		_toTitleMsgBoxParam.szDescription = "ui_stageselect_totitle_msg";
		_toTitleMsgBoxParam.szLBtnLocale = "msgwin_btn_ok";
		_toTitleMsgBoxParam.szRBtnLocale = "msgwin_btn_cancel";
		_toTitleMsgBoxParam.onLBtnClick = () =>
		{
			GameFramework._Instance.SaveGameData();

			HT.HTSoundManager.StopMusic(false);
			HT.HTFramework.Instance.SceneChange(GameDefine.szMainTitleSceneName);
		};

		//-----
		HTUIManager.RegistOnCloseBtnClicked(() => {
			ShowToTitleMessageBox();
		});

		_backButton.onClick.AddListener(ShowToTitleMessageBox);

		//-----
		if (GameFramework._Instance.m_pPlayerData.m_vLevelActivated[_curFieldIndex])
		{
			_clearMarksTargetAlpha = 1.0f;
			_clearMarks.alpha = 1.0f;
		}

		//-----
		HT.HTSoundManager.PlayMusic(_backgroundMusic, _backGroundMusic_Volume);

		//-----
		if (GameFramework._Instance._askedTutorialPlay == false)
		{
			HT.UIPopup_MessageBoxParam tutorialMsgBoxParam = new HT.UIPopup_MessageBoxParam();
			tutorialMsgBoxParam.Init(HT.eMessageBoxType.Question, "msgwin_subj_notice", "tutorialmap_asktutorial", "msgwin_btn_ok", "msgwin_btn_cancel", () => 
			{
				GameFramework._Instance._askedTutorialPlay = true;
				HT.HTFramework.Instance.SceneChange(_tutorialFieldName);
			}, ()=>
			{
				GameFramework._Instance._askedTutorialPlay = true;
			});

			HTUIManager.OpenMessageBox(tutorialMsgBoxParam);
		}
	}

	private void Update()
	{
		if (_waitForAnimation > 0.0f)
			_waitForAnimation -= HT.TimeUtils.GameTime;

		//-----
		float fHorizontal = HT.HTInputManager.Instance.Horizontal;
		if (fHorizontal > 0.5f)
			AddCurrentIndex();

		else if (fHorizontal < -0.5f)
			SubtractCurrentIndex();
	}

	private void FixedUpdate()
	{
		if (_clearMarks.alpha > _clearMarksTargetAlpha)
		{
			_clearMarks.alpha -= HT.TimeUtils.FixedTime * 4.0f;
			if (_clearMarks.alpha <= _clearMarksTargetAlpha)
				_clearMarks.alpha = _clearMarksTargetAlpha;
		}
		else if (_clearMarks.alpha < _clearMarksTargetAlpha)
		{
			_clearMarks.alpha += HT.TimeUtils.FixedTime * 4.0f;
			if (_clearMarks.alpha >= _clearMarksTargetAlpha)
				_clearMarks.alpha = _clearMarksTargetAlpha;
		}
	}

	//---------------------------------------
	private void ShowToTitleMessageBox()
	{
		HTUIManager.OpenMessageBox(_toTitleMsgBoxParam);
	}

	//---------------------------------------
	private void AddCurrentIndex()
	{
		if (_waitForAnimation > 0.0f)
			return;

		++_curFieldIndex;
		if (_curFieldIndex >= GameFramework._Instance.m_vLevelSettings.Length)
			_curFieldIndex = 0;

		_animation.Play(_anim_Next);
		_waitForAnimation = _animTime;
		
		RefreshAnimationObjects();
		//RefreshLevel();
	}

	private void SubtractCurrentIndex()
	{
		if (_waitForAnimation > 0.0f)
			return;

		--_curFieldIndex;
		if (_curFieldIndex < 0)
			_curFieldIndex = GameFramework._Instance.m_vLevelSettings.Length - 1;

		_animation.Play(_anim_Prev);
		_waitForAnimation = _animTime;
		
		RefreshAnimationObjects();
		//RefreshLevel();
	}

	public void RefreshLevel()
	{
		LevelSettings pLevel = GameFramework._Instance.m_vLevelSettings[_curFieldIndex];

		_fieldImg.sprite = pLevel.m_pFieldIllust;
		_background.sprite = pLevel.m_pLevelIllust;

		_fieldName.text = HT.HTLocaleTable.GetLocalstring(pLevel._levelName);
		_fieldDescription.text = HT.HTLocaleTable.GetLocalstring(pLevel._levelDescription);

        //-----
        bool[] vLevelActivates = GameFramework._Instance.m_pPlayerData.m_vLevelActivated;
		bool bIsCleared = vLevelActivates[_curFieldIndex];
		_selectLevel.interactable = (bIsCleared) ? false : true;
		_clearMarksTargetAlpha = (bIsCleared) ? 1.0f : 0.0f;

		//-----
		HT.HTPlatform_LeaderBoardViewer.LeaderboardID = pLevel.LeaderBoardID;

		//-----
#if DEMO_VERSION
		if (pLevel._isPackToDemo)
			_lockedMarks.alpha = 0.0f;
		else
			_lockedMarks.alpha = 1.0f;

		if (bIsCleared == false && pLevel._isPackToDemo)
			_selectLevel.interactable = true;
		else
			_selectLevel.interactable = false;

#else // DEMO_VERSION

#if ENABLE_DEBUG || UNITY_EDITOR
		_lockedMarks.alpha = 0.0f;
#else // ENABLE_DEBUG || UNITY_EDITOR
			if (pLevel._isLastBoss && bIsCleared == false)
	        {
	            bool bIsAllCleared = true;
	            for(int nInd = 0; nInd < vLevelActivates.Length; ++nInd)
	            {
	                if (nInd == _curFieldIndex)
	                    continue;
	
	                if (vLevelActivates[nInd] == false)
	                    bIsAllCleared = false;
	            }
	
	            if (bIsAllCleared == false)
	            {
	                _selectLevel.interactable = false;
	                _lockedMarks.alpha = 1.0f;
	            }
	            else
	                _lockedMarks.alpha = 0.0f;
	        }
	        else
	            _lockedMarks.alpha = 0.0f;
#endif // ENABLE_DEBUG || UNITY_EDITOR

#endif // DEMO_VERSION
	}

	//---------------------------------------
	private void SelectStage()
	{
		if (GameFramework._Instance.m_pPlayerData.m_vLevelActivated[_curFieldIndex])
			return;

		if (_curFieldIndex < 0 || _curFieldIndex >= GameFramework._Instance.m_vLevelSettings.Length)
			return;

		GameFramework._Instance.m_pPlayerData.m_nActivatedLevel = _curFieldIndex;
		HT.HTFramework.Instance.SceneChange(GameFramework._Instance.m_vLevelSettings[_curFieldIndex].GetLevelName());
	}

	//---------------------------------------
	private void RefreshAnimationObjects()
	{
		_animObj_PrevPrev.sprite = GetAnimObjImage(-2);
		_animObj_Prev.sprite = GetAnimObjImage(-1);
		_animObj_Current.sprite = GetAnimObjImage(0);
		_animObj_Next.sprite = GetAnimObjImage(1);
		_animObj_NextNext.sprite = GetAnimObjImage(2);

		_prevLevel.image.sprite = GetAnimObjImage(-1);
		_nextLevel.image.sprite = GetAnimObjImage(1);
	}

	private Sprite GetAnimObjImage(int nOffset)
	{
		LevelSettings[] vSettings = GameFramework._Instance.m_vLevelSettings;

		int nIndex = _curFieldIndex + nOffset;
		if (nIndex >= vSettings.Length)
			nIndex -= vSettings.Length;

		if (nIndex < 0)
			nIndex += vSettings.Length;

		return vSettings[nIndex].m_pFieldIllust;
	}
}


/////////////////////////////////////////
//---------------------------------------