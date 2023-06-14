using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HT;


/////////////////////////////////////////
//---------------------------------------
public class InteractionBtnView : MonoBehaviour
{
	//---------------------------------------
	[Header("OBJECTS")]
	[SerializeField]
	private GameObject _root = null;
	[SerializeField]
	private CanvasGroup _canvasGroup = null;
	[SerializeField]
	private Image _btnIcon = null;
	[SerializeField]
	private Text _btnDesc = null;
	[SerializeField]
	private Image _btnMaskImg = null;
	[SerializeField]
	private Image _filledCooltime = null;
	[SerializeField]
	private Image _btnPadButtonImg = null;

	[Header("INPUTS")]
	[SerializeField]
	private Sprite[] _padButtons = null;
	[SerializeField]
	private ePadButton _interactPadBtn = ePadButton.X;
	[SerializeField]
	private string _interactKeyboard = "E";
	[SerializeField]
	private string _interactKeyName = "Interact";
	//[SerializeField]
	//private Button _clickBtn = null;
	[SerializeField]
	private HT.UIEventer_ClickHandler _clickHandler = null;

	//---------------------------------------
	private HT.HTKey _interactKey = null;
	private Coroutine _coolTimeProc = null;

	//---------------------------------------
	private void Awake()
	{
		ShowInteract(false);
		SetEnabled(false);
		
		if (HT.HTInputManager.Instance.JoystickConnected)
		{
			_btnDesc.text = string.Empty;
			_btnPadButtonImg.sprite = _padButtons[(int)_interactPadBtn];
		}
		else
		{
			_btnDesc.text = _interactKeyboard;
			_btnPadButtonImg.sprite = _padButtons[(int)ePadButton.Max];
		}

		_interactKey = HT.HTInputManager.Instance.GetKey(_interactKeyName);
		_clickHandler.Init(OnClickDownEvent, OnClickUpEvent);
	}

	private void OnClickDownEvent()
	{
		_interactKey.ButtonState = HT.eButtonState.Down;
	}

	private void OnClickUpEvent(float fTime)
	{
		_interactKey.ButtonState = HT.eButtonState.Free;
	}

	//---------------------------------------
	public void ShowInteract(bool bShow, Sprite pSpr = null)
	{
		_btnIcon.sprite = pSpr;
		_btnMaskImg.sprite = pSpr;

		_root.SetActive(bShow);

		_filledCooltime.fillAmount = 0.0f;
	}

	public void SetEnabled(bool bEnable)
	{
		_canvasGroup.alpha = (bEnable) ? 1.0f : 0.5f;
	}

	//---------------------------------------
	public void SetCoolTime(float fTime)
	{
		_coolTimeProc = StartCoroutine(CoolTime_Internal(fTime));
	}

	IEnumerator CoolTime_Internal(float fTime)
	{
		_filledCooltime.fillAmount = 1.0f;

		float fCurTime = fTime;
		while(fCurTime >= 0.0f)
		{
			fCurTime -= HT.TimeUtils.GameTime;
			_filledCooltime.fillAmount = fCurTime / fTime;

			yield return new WaitForEndOfFrame();
		}

		_filledCooltime.fillAmount = 0.0f;
		_coolTimeProc = null;
	}

	public void StopCoolTime()
	{
		if (_coolTimeProc != null)
		{
			StopCoroutine(_coolTimeProc);
			_coolTimeProc = null;
		}

		_filledCooltime.fillAmount = 0.0f;
	}
}


/////////////////////////////////////////
//---------------------------------------