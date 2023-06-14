using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using HT;


/////////////////////////////////////////
//---------------------------------------
public class Field_Tutorial : Field
{
	//---------------------------------------
	[Header("TUTORIAL FIELD")]
	public ControllableActor _playerActor = null;
	[SerializeField]
	private AudioClip _bgmSound = null;
	[SerializeField]
	private ParticleSystem _damaged_Effect = null;
	[SerializeField]
	private Animation _damaged_UIAnim = null;
	[SerializeField]
	private AudioClip _damaged_Sound = null;

	[Header("TUTORIAL FIELD - UI")]
	[SerializeField]
	private Button _skipButton = null;
	[SerializeField]
	private string _nextSceneName = null;
	[SerializeField]
	private CanvasGroup _blackMask_Canvas = null;

	//---------------------------------------
	[Header("TUTORIAL - MOVEMENT")]
	[SerializeField]
	private GameObject[] _tut_move_activateObjects = null;
	[SerializeField]
	private GameObject[] _tut_move_deactivateObjects = null;
	[SerializeField]
	private GameObject _tut_move_PC = null;
	[SerializeField]
	private GameObject _tut_move_Joystick = null;
	[SerializeField]
	private GameObject _tut_move_Mobile = null;
	[SerializeField]
	private CanvasGroup _tut_move_Canvas = null;
	[SerializeField]
	private Button _tut_move_NextBtn = null;
	[SerializeField]
	private GameObject _tut_move_EffectPos = null;

	private bool _tut_move_WaitEventer = false;

	//---------------------------------------
	[Header("TUTORIAL - ATTACK")]
	[SerializeField]
	private GameObject[] _tut_atk_activateObjects = null;
	[SerializeField]
	private GameObject[] _tut_atk_deactivateObjects = null;
	[SerializeField]
	private GameObject _tut_atk_PC = null;
	[SerializeField]
	private GameObject _tut_atk_Joystick = null;
	[SerializeField]
	private GameObject _tut_atk_Mobile = null;
	[SerializeField]
	private CanvasGroup _tut_atk_Canvas = null;
	[SerializeField]
	private Button _tut_atk_NextBtn = null;
	[SerializeField]
	private CanvasGroup _tut_atk_Canvas_Aim = null;
	[SerializeField]
	private Button _tut_atk_NextBtn_Aim = null;
	[SerializeField]
	private GameObject _tut_atk_EffectPos = null;

	private bool _tut_atk_WaitEventer = false;

	//---------------------------------------
	[Header("TUTORIAL - DASH")]
	[SerializeField]
	private GameObject[] _tut_dash_activateObjects = null;
	[SerializeField]
	private GameObject[] _tut_dash_deactivateObjects = null;
	[SerializeField]
	private GameObject _tut_dash_PC = null;
	[SerializeField]
	private GameObject _tut_dash_Joystick = null;
	[SerializeField]
	private GameObject _tut_dash_Mobile = null;
	[SerializeField]
	private CanvasGroup _tut_dash_Canvas = null;
	[SerializeField]
	private Button _tut_dash_NextBtn = null;
	[SerializeField]
	private CanvasGroup _tut_dash_Canvas_Ignore = null;
	[SerializeField]
	private Button _tut_dash_NextBtn_Ignore = null;
	[SerializeField]
	private GameObject _tut_dash_EffectPos = null;

	private bool _tut_dash_WaitEventer = false;

	//---------------------------------------
	[Header("TUTORIAL - DAMAGE IGNORE")]
	[SerializeField]
	private GameObject[] _tut_dmgIgnore_activateObjects = null;
	[SerializeField]
	private GameObject[] _tut_dmgIgnore_deactivateObjects = null;
	[SerializeField]
	private CanvasGroup _tut_dmgIgnore_Canvas = null;
	[SerializeField]
	private Button _tut_dmgIgnore_NextBtn = null;
	[SerializeField]
	private GameObject _tut_dmgIgnore_EffectPos = null;
	[SerializeField]
	private Animation[] _tut_dmgIgnore_Throns = null;
	[SerializeField]
	private AudioClip _tut_dmgIgnore_Thorn_Sound = null;
	[SerializeField]
	private Object_AreaAlert _tut_dmgIgnore_Alert = null;
	[SerializeField]
	private Object_AreaAlert _tut_dmgIgnore_Alert_Round = null;

	private bool _tut_dmgIgnore_ThornActiveAll = false;
	private bool _tut_dmgIgnore_WaitEventer = false;

	//---------------------------------------
	[Header("TUTORIAL - SPCATK")]
	[SerializeField]
	private GameObject[] _tut_spcAtk_activateObjects = null;
	[SerializeField]
	private GameObject[] _tut_spcAtk_deactivateObjects = null;
	[SerializeField]
	private GameObject _tut_spcAtk_PC = null;
	[SerializeField]
	private GameObject _tut_spcAtk_Joystick = null;
	[SerializeField]
	private GameObject _tut_spcAtk_Mobile = null;
	[SerializeField]
	private CanvasGroup _tut_spcAtk_Canvas = null;
	[SerializeField]
	private Button _tut_spcAtk_NextBtn = null;
	[SerializeField]
	private CanvasGroup _tut_spcAtk_Canvas_Ignore = null;
	[SerializeField]
	private Button _tut_spcAtk_NextBtn_Ignore = null;
	[SerializeField]
	private GameObject _tut_spcAtk_EffectPos = null;

	private bool _tut_spcAtk_WaitEventer = false;

	//---------------------------------------
	[Header("TUTORIAL - COMMON")]
	[SerializeField]
	private AudioClip _tutorialCommon_Sound_Complete = null;
	[SerializeField]
	private ParticleSystem _tutorialCommon_Effect_Complete = null;

	//---------------------------------------
	private HT.UIPopup_MessageBoxParam _param_Skip = null;

	//---------------------------------------
	protected override void OnAwake()
	{
		m_bIsLobbyField = false;

		_skipButton.onClick.AddListener(OnSkipButton);

		//-----
		HT.HTSoundManager.PlayMusic(_bgmSound);

		//-----
		Init();
		_playerActor.Init();
		_playerActor.onCastDash += OnCastDash;
		_playerActor.onCastSpcAtk += OnCastSpcAtk;
		_playerActor.onDamageIgnored += OnDamageIgnored;

		CameraManager._Instance.m_pMainCamera = m_pGameCamera;
		CameraManager._Instance.m_pTargetEntity = _playerActor.gameObject;

		//-----
		StartCoroutine(Tutorial_Internal());
	}

	//---------------------------------------
	protected override void Frame()
	{
		_playerActor.Frame();

		if (_playerActor.m_pActorInfo.m_cnNowHP.val < _playerActor.m_pActorInfo.m_cnMaxHP.val)
			_playerActor.m_pActorInfo.m_cnNowHP.val = _playerActor.m_pActorInfo.m_cnMaxHP.val;
	}

	//---------------------------------------
	private IEnumerator Tutorial_Internal()
	{
		_blackMask_Canvas.gameObject.SetActive(true);
		_blackMask_Canvas.alpha = 1.0f;

		float fFadeIn = 2.5f;
		float fFadeInLeast = 2.5f;
		while(fFadeInLeast > 0.0f)
		{
			fFadeInLeast -= TimeUtils.GameTime;
			_blackMask_Canvas.alpha = fFadeInLeast / fFadeIn;
			yield return new WaitForEndOfFrame();
		}

		_blackMask_Canvas.gameObject.SetActive(false);

		// Tutorial - Movement
		ObjectActivations(_tut_move_activateObjects, _tut_move_deactivateObjects, _tut_move_PC, _tut_move_Joystick, _tut_move_Mobile);
		yield return TutorialCanvas_Internal(_tut_move_Canvas, _tut_move_NextBtn);
		
		_tut_move_WaitEventer = true;
		while(_tut_move_WaitEventer)
			yield return new WaitForEndOfFrame();

		for (int nInd = 0; nInd < _tut_move_activateObjects.Length; ++nInd)
			_tut_move_activateObjects[nInd].SetActive(false);
		
		ShowTutorialEffect(_tut_move_EffectPos.transform.position);
		yield return new WaitForSeconds(2.0f);

		// Tutorial - Attack
		ObjectActivations(_tut_atk_activateObjects, _tut_atk_deactivateObjects, _tut_atk_PC, _tut_atk_Joystick, _tut_atk_Mobile);
		yield return TutorialCanvas_Internal(_tut_atk_Canvas, _tut_atk_NextBtn);
		yield return TutorialCanvas_Internal(_tut_atk_Canvas_Aim, _tut_atk_NextBtn_Aim);
		
		_tut_atk_WaitEventer = true;
		while (_tut_atk_WaitEventer)
			yield return new WaitForEndOfFrame();

		for (int nInd = 0; nInd < _tut_atk_activateObjects.Length; ++nInd)
			_tut_atk_activateObjects[nInd].SetActive(false);

		ShowTutorialEffect(_tut_atk_EffectPos.transform.position);
		yield return new WaitForSeconds(2.0f);

		// Tutorial - Dash
		ObjectActivations(_tut_dash_activateObjects, _tut_dash_deactivateObjects, _tut_dash_PC, _tut_dash_Joystick, _tut_dash_Mobile);
		yield return TutorialCanvas_Internal(_tut_dash_Canvas, _tut_dash_NextBtn);

		_tut_dash_WaitEventer = true;
		while (_tut_dash_WaitEventer)
			yield return new WaitForEndOfFrame();

		for (int nInd = 0; nInd < _tut_dash_activateObjects.Length; ++nInd)
			_tut_dash_activateObjects[nInd].SetActive(false);
		
		ShowTutorialEffect(_tut_dash_EffectPos.transform.position);
		yield return new WaitForSeconds(2.0f);

		// Tutorial - Damage Ignore
		ObjectActivations(_tut_dmgIgnore_activateObjects, _tut_dmgIgnore_deactivateObjects, null, null, null);
		yield return TutorialCanvas_Internal(_tut_dash_Canvas_Ignore, _tut_dash_NextBtn_Ignore);

		_tut_dmgIgnore_ThornActiveAll = true;
		Coroutine pThronProc = StartCoroutine(Tutorial_Thorns_Internal());

		_tut_dmgIgnore_WaitEventer = true;
		while (_tut_dmgIgnore_WaitEventer)
			yield return new WaitForEndOfFrame();

		for (int nInd = 0; nInd < _tut_dmgIgnore_activateObjects.Length; ++nInd)
			_tut_dmgIgnore_activateObjects[nInd].SetActive(false);

		_tut_dmgIgnore_ThornActiveAll = false;
		StopCoroutine(pThronProc);

		ShowTutorialEffect(_tut_dmgIgnore_EffectPos.transform.position);
		yield return new WaitForSeconds(2.0f);

		// Tutorial - Special Attack
		ObjectActivations(_tut_spcAtk_activateObjects, _tut_spcAtk_deactivateObjects, _tut_spcAtk_PC, _tut_spcAtk_Joystick, _tut_spcAtk_Mobile);
		yield return TutorialCanvas_Internal(_tut_spcAtk_Canvas, _tut_spcAtk_NextBtn);
		yield return TutorialCanvas_Internal(_tut_spcAtk_Canvas_Ignore, _tut_spcAtk_NextBtn_Ignore);

		pThronProc = StartCoroutine(Tutorial_Thorns_Internal());

		_playerActor.SpcAtk_CurCharged = _playerActor.SpcAtk_ChargeMax;

		_tut_atk_WaitEventer = true;
		_tut_spcAtk_WaitEventer = true;
		while (_tut_spcAtk_WaitEventer || _tut_spcAtk_WaitEventer)
		{
			if (_tut_atk_WaitEventer || _tut_spcAtk_WaitEventer)
			{
				yield return new WaitForSeconds(1.0f);

				if (_tut_atk_WaitEventer == false && _tut_spcAtk_WaitEventer == false)
					break;

				else
				{
					_tut_atk_WaitEventer = true;
					_tut_spcAtk_WaitEventer = true;
					_playerActor.SpcAtk_CurCharged = _playerActor.SpcAtk_ChargeMax;
				}
			}

			yield return new WaitForEndOfFrame();
		}

		for (int nInd = 0; nInd < _tut_spcAtk_activateObjects.Length; ++nInd)
			_tut_spcAtk_activateObjects[nInd].SetActive(false);

		StopCoroutine(pThronProc);

		ShowTutorialEffect(_tut_spcAtk_EffectPos.transform.position);
		yield return new WaitForSeconds(2.0f);

		// End Tutorial
		UIPopup_MessageBoxParam pParam = new UIPopup_MessageBoxParam();
		pParam.Init(eMessageBoxType.Exclamation, "msgwin_subj_notice", "tutorialmap_endtutorial", "msgwin_btn_close");

		HTUIManager.OpenMessageBox(pParam);

		//-----
		GameFramework._Instance._tutorialInfo_Game = true;
		if (HTAfxPref.IsMobilePlatform)
			GameFramework._Instance._tutorialInfo_Mobile = true;
		else
		{
			if (HTInputManager.Instance.JoystickConnected)
				GameFramework._Instance._tutorialInfo_Joystick = true;
			else
				GameFramework._Instance._tutorialInfo_Keyboard = true;
		}
		
		//-----
		pThronProc = StartCoroutine(Tutorial_Thorns_Internal());
		//while (true)
		//{
		//	_playerActor.SpcAtk_CurCharged = _playerActor.SpcAtk_ChargeMax;
		//	yield return new WaitForSeconds(1.0f);
		//}
	}

	//---------------------------------------
	private void ObjectActivations(GameObject[] vActivates, GameObject[] vDeactivates, GameObject pPC, GameObject pJoystick, GameObject pMobile)
	{
		for (int nInd = 0; nInd < vActivates.Length; ++nInd)
			vActivates[nInd].SetActive(true);

		for (int nInd = 0; nInd < vDeactivates.Length; ++nInd)
			vDeactivates[nInd].SetActive(false);

		//-----
		if (pPC != null)
			pPC.SetActive(false);

		if (pJoystick != null)
			pJoystick.SetActive(false);

		if (pMobile != null)
			pMobile.SetActive(false);

		//-----
		if (HT.HTAfxPref.IsMobilePlatform)
		{
			if (pMobile != null)
				pMobile.SetActive(true);
		}
		else
		{
			if (HT.HTInputManager.Instance.JoystickConnected)
			{
				if (pJoystick != null)
					pJoystick.SetActive(true);
			}
			else
			{
				if (pPC != null)
					pPC.SetActive(true);
			}
		}
	}

	private void ShowTutorialEffect(Vector3 vPos)
	{
		HT.HTSoundManager.PlaySound(_tutorialCommon_Sound_Complete);

		ParticleSystem pParticle = HT.Utils.Instantiate(_tutorialCommon_Effect_Complete);
		pParticle.transform.position = vPos;
		HT.Utils.SafeDestroy(pParticle.gameObject, pParticle.TotalSimulationTime());
	}

	private IEnumerator TutorialCanvas_Internal(CanvasGroup pCanvas, Button pButton)
	{
		pCanvas.gameObject.SetActive(true);
		pCanvas.alpha = 0.0f;

		float fAlpha = 0.0f;
		while (fAlpha < 1.0f)
		{
			fAlpha += HT.TimeUtils.GameTime * 2.0f;
			if (fAlpha > 1.0f)
				fAlpha = 1.0f;

			pCanvas.alpha = fAlpha;
			yield return new WaitForEndOfFrame();
		}

		//-----
		yield return new WaitForSeconds(1.0f);

		//-----
		bool bWaitForClick = true;
		pButton.onClick.AddListener(() => {
			bWaitForClick = false;
		});

		EventSystem.current.SetSelectedGameObject(pButton.gameObject);

		while (bWaitForClick)
			yield return new WaitForEndOfFrame();
		
		EventSystem.current.SetSelectedGameObject(null);
		pButton.onClick.RemoveAllListeners();

		//-----
		while (fAlpha > 0.0f)
		{
			fAlpha -= HT.TimeUtils.GameTime * 2.0f;
			if (fAlpha < 0.0f)
				fAlpha = 0.0f;

			pCanvas.alpha = fAlpha;
			yield return new WaitForEndOfFrame();
		}
		pCanvas.gameObject.SetActive(false);
	}

	private IEnumerator Tutorial_Thorns_Internal()
	{
		float fRange = 5.0f;
		Vector3 vPos = new Vector3(0.0f, 0.1f, 0.0f);

		while (true)
		{
			if (_tut_dmgIgnore_ThornActiveAll)
			{
				Object_AreaAlert pAlert = HT.Utils.InstantiateFromPool(_tut_dmgIgnore_Alert_Round);
				GameFramework._Instance.SetObjectPositionAndRotateByPhysic(pAlert.gameObject, Vector3.zero);

				Vector3 vScale = pAlert._defaultScale;
				vScale *= fRange;
				pAlert.m_pRoot.transform.localScale = vScale;
				pAlert.transform.position = vPos;

				pAlert.Init(1.0f);
				pAlert.m_eAlertType = Object_AreaAlert.eAreaAlertType.Warnning;
				yield return new WaitForSeconds(1.0f);

				//-----
				for (int nInd = 0; nInd < _tut_dmgIgnore_Throns.Length; ++nInd)
					_tut_dmgIgnore_Throns[nInd].Play();

				if (LogicUtils.IsInRange(vPos, _playerActor.gameObject.transform.position, fRange))
				{
					_playerActor.OnDamaged(1);

					if (_playerActor.SpcAtk_Counter_LeastInvTime <= 0.0f)
					{
						ParticleSystem pEffect = HT.Utils.Instantiate(_damaged_Effect);
						pEffect.transform.position = pEffect.transform.position + _playerActor.transform.position;
						HT.Utils.SafeDestroy(pEffect.gameObject, pEffect.TotalSimulationTime());

						//-----
						_damaged_UIAnim.Stop();
						_damaged_UIAnim.Play("DamageAlert_UI");

						//-----
						HT.HTSoundManager.PlaySound(_damaged_Sound);
					}
				}
			}
			else
			{
				int nIndex = HT.RandomUtils.Range(0, 8);
				int nRotate = nIndex * 45;

				//-----
				Object_AreaAlert pAlert = HT.Utils.InstantiateFromPool(_tut_dmgIgnore_Alert);
				GameFramework._Instance.SetObjectPositionAndRotateByPhysic(pAlert.gameObject, Vector3.zero);

				Vector3 vScale = pAlert._defaultScale;
				vScale *= fRange;
				pAlert.m_pRoot.transform.localScale = vScale;
				pAlert.transform.rotation = Quaternion.Euler(0.0f, (float)nRotate, 0.0f);
				pAlert.transform.position = vPos;

				pAlert.Init(1.0f);
				pAlert.m_eAlertType = Object_AreaAlert.eAreaAlertType.Warnning;
				yield return new WaitForSeconds(1.0f);

				//-----
				HT.HTSoundManager.PlaySound(_tut_dmgIgnore_Thorn_Sound);
				_tut_dmgIgnore_Throns[nIndex].Play();

				int nPrevIndex = (nIndex > 0) ? nIndex - 1 : _tut_dmgIgnore_Throns.Length - 1;
				_tut_dmgIgnore_Throns[nPrevIndex].Play();

				if (LogicUtils.CheckSight(pAlert.gameObject, _playerActor.gameObject, fRange, 90.0f))
				{
					_playerActor.OnDamaged(1);

					//-----
					if (_playerActor.SpcAtk_Counter_LeastInvTime <= 0.0f)
					{
						ParticleSystem pEffect = HT.Utils.Instantiate(_damaged_Effect);
						pEffect.transform.position = pEffect.transform.position + _playerActor.transform.position;
						HT.Utils.SafeDestroy(pEffect.gameObject, pEffect.TotalSimulationTime());

						//-----
						_damaged_UIAnim.Stop();
						_damaged_UIAnim.Play("DamageAlert_UI");

						//-----
						HT.HTSoundManager.PlaySound(_damaged_Sound);
					}
				}
			}

			yield return new WaitForSeconds(1.0f);
		}
	}

	//---------------------------------------
	public void OnSkipButton()
	{
		if (_param_Skip == null)
		{
			_param_Skip = new HT.UIPopup_MessageBoxParam();
			_param_Skip.Init(HT.eMessageBoxType.Question, "msgwin_subj_notice", "tutorialmap_quittutorial", "msgwin_btn_ok", "msgwin_btn_cancel", ToNextScene);
		}

		HTUIManager.OpenMessageBox(_param_Skip);
	}

	private void ToNextScene()
	{
		HT.HTFramework.Instance.SceneChange(_nextSceneName);
	}

	//---------------------------------------
	public void OnEnterTrigger_Movement()
	{
		_tut_move_WaitEventer = false;
	}

	public void OnHitListener()
	{
		_tut_atk_WaitEventer = false;
	}

	public void OnCastDash()
	{
		_tut_dash_WaitEventer = false;
	}

	public void OnDamageIgnored()
	{
		_tut_dmgIgnore_WaitEventer = false;
	}

	public void OnCastSpcAtk()
	{
		_tut_spcAtk_WaitEventer = false;
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------