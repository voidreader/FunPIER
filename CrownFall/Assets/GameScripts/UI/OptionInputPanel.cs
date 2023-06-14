using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/////////////////////////////////////////
//---------------------------------------
public class OptionInputPanel : MonoBehaviour
{
	//---------------------------------------
	[Header("Keyboard")]
	[SerializeField]
	private Image _kb_targetImg = null;
	[SerializeField]
	private Toggle _kb_dash_keyboard = null;
	[SerializeField]
	private Sprite _kb_dash_keyboard_Img = null;
	[SerializeField]
	private Toggle _kb_dash_mouse = null;
	[SerializeField]
	private Sprite _kb_dash_mouse_Img = null;

	//---------------------------------------
	[Header("Xbox360 pad")]
	[SerializeField]
	private Image _pad_targetImg = null;
	[SerializeField]
	private Toggle _pad_aim_right = null;
	[SerializeField]
	private Sprite _pad_aim_right_Img = null;
	[SerializeField]
	private Toggle _pad_aim_left = null;
	[SerializeField]
	private Sprite _pad_aim_left_Img = null;

	//---------------------------------------
	private void Awake()
	{
		_kb_dash_keyboard.onValueChanged.AddListener(OnClicked_Kb_Keyboard);
		_kb_dash_mouse.onValueChanged.AddListener(OnClicked_Kb_Mouse);

		_pad_aim_right.onValueChanged.AddListener(OnClicked_Pad_Right);
		_pad_aim_left.onValueChanged.AddListener(OnClicked_Pad_Left);

		//-----
		if (GameFramework._Instance._option_dash_useMoveDir)
		{
			_kb_dash_keyboard.isOn = true;
			_kb_dash_mouse.isOn = false;
			OnClicked_Kb_Keyboard(true);
		}
		else
		{
			_kb_dash_keyboard.isOn = false;
			_kb_dash_mouse.isOn = true;
			OnClicked_Kb_Mouse(true);
		}

		//-----
		if (GameFramework._Instance._option_aim_useRightAnalog)
		{
			_pad_aim_right.isOn = true;
			_pad_aim_left.isOn = false;
			OnClicked_Pad_Right(true);
		}
		else
		{
			_pad_aim_right.isOn = false;
			_pad_aim_left.isOn = true;
			OnClicked_Pad_Left(true);
		}
	}

	//---------------------------------------
	private void OnClicked_Kb_Keyboard(bool bOn)
	{
		if (bOn == false)
			return;

		GameFramework._Instance._option_dash_useMoveDir = true;
		_kb_targetImg.sprite = _kb_dash_keyboard_Img;
	}

	private void OnClicked_Kb_Mouse(bool bOn)
	{
		if (bOn == false)
			return;
		GameFramework._Instance._option_dash_useMoveDir = false;
		_kb_targetImg.sprite = _kb_dash_mouse_Img;
	}

	//---------------------------------------
	private void OnClicked_Pad_Right(bool bOn)
	{
		if (bOn == false)
			return;

		GameFramework._Instance._option_aim_useRightAnalog = true;
		_pad_targetImg.sprite = _pad_aim_right_Img;
	}

	private void OnClicked_Pad_Left(bool bOn)
	{
		if (bOn == false)
			return;

		GameFramework._Instance._option_aim_useRightAnalog = false;
		_pad_targetImg.sprite = _pad_aim_left_Img;
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------