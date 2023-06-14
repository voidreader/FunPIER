using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HT;


/////////////////////////////////////////
//---------------------------------------
public class MobileController : MonoBehaviour
{
	//---------------------------------------
	[Header("Handlers")]
	[SerializeField]
	private UIEventer_DragHandler _moveHandler = null;
	[SerializeField]
	private UIEventer_DragHandler _attackHandler = null;
	[SerializeField]
	private UIEventer_ClickHandler _dashHandler = null;
	[SerializeField]
	private IActorBase _playerActor = null;

	[Header("Handlers - Special Attacks")]
	[SerializeField]
	private Image _spcAtk_handlerImage = null;
	[SerializeField]
	private Sprite _spcAtk_handlerImage_Enable = null;
	[SerializeField]
	private Sprite _spcAtk_handlerImage_Disable = null;
	[SerializeField]
	private UIEventer_ClickHandler _spcAtkHandler = null;

	[Header("Key Name")]
	[SerializeField]
	private string _attackKeyname = null;
	[SerializeField]
	private string _dashKeyname = null;
	[SerializeField]
	private string _spcAtkKeyname = null;

	[Header("Dash Control")]
	[SerializeField]
	private float _moveLerpRatio = 2.5f;
	[SerializeField]
	private Image _dash_handlerImage = null;
	[SerializeField]
	private Sprite _dash_handlerImage_Enable = null;
	[SerializeField]
	private Sprite _dash_handlerImage_Disable = null;

	//---------------------------------------
	private Vector2 _lastMoveInput = Vector2.zero;
	private Vector2 _lerpMoveInput = Vector2.zero;
	private Vector2 _lastAtkInput = Vector2.zero;

	//---------------------------------------
	private HTKey _attackKey = null;
	private HTKey _dashKey = null;
	private HTKey _spcAtkKey = null;

	private HTInputManager _inputMan = null;

	//---------------------------------------
	bool bIsMoveUpdated = false;
	bool bAttackKeyDowned = false;

	//---------------------------------------
	private void Start()
	{
		_inputMan = HTInputManager.Instance;

		_attackKey = _inputMan.GetKey(_attackKeyname);
		_dashKey = _inputMan.GetKey(_dashKeyname);
		_spcAtkKey = _inputMan.GetKey(_spcAtkKeyname);

		//-----
		_moveHandler.Init(OnMoveHandler, OnMoveHandler, OnMoveEnd);

		_attackHandler.Init(OnAttackHandler_Down, OnAttackHandler_Down, OnAttackHandler_Up);
		_dashHandler.Init(OnDashHandler_Down, OnDashHandler_Up);
		_spcAtkHandler.Init(OnSpcAtkHandler_Down, OnSpcAtkHandler_Up);
	}

	private void Update()
	{
		//-----
		if (bIsMoveUpdated == false)
			_lastMoveInput = Vector2.zero;

		else
		{
			_lastMoveInput.Normalize();

			_lastMoveInput = Vector2.Lerp(_lerpMoveInput, _lastMoveInput, _moveLerpRatio * TimeUtils.GameTime).normalized;
			_lastMoveInput.x = Mathf.Clamp(_lastMoveInput.x * 2.0f, -1.0f, 1.0f);
			_lastMoveInput.y = Mathf.Clamp(_lastMoveInput.y * 2.0f, -1.0f, 1.0f);

			_lerpMoveInput = _lastMoveInput;
		}

		_inputMan.Horizontal = _lastMoveInput.x;
		_inputMan.Vertical = _lastMoveInput.y;

		//-----
		Vector3 vAtkInput = (bAttackKeyDowned) ? _lastAtkInput : _lastMoveInput;
		if (vAtkInput.sqrMagnitude > float.Epsilon)
		{
			Vector3 vPlayerPos = Vector3.zero;

			if (BattleFramework._Instance != null)
				vPlayerPos = BattleFramework._Instance.m_pPlayerActor.transform.position;
			else if (_playerActor != null)
				vPlayerPos = _playerActor.transform.position;

			Vector3 vMoveInput = new Vector3(vAtkInput.x, 0.0f, vAtkInput.y);
			_inputMan.MousePickingPos = vPlayerPos + (vMoveInput * 10.0f);
		}

		//-----
		_attackKey.ButtonState = (bAttackKeyDowned) ? eButtonState.Down : eButtonState.Free;
	}

	private void FixedUpdate()
	{
		ControllableActor pPlayer = null;
		if (_playerActor == null && BattleFramework._Instance != null)
			pPlayer = BattleFramework._Instance.m_pPlayerActor as ControllableActor;
		else
			pPlayer = _playerActor as ControllableActor;

		//-----
		if (pPlayer != null)
		{
			bool bSpcAtkEnabled = false;
			if (pPlayer.SpcAtk_CurCharged >= pPlayer.SpcAtk_ChargeMax)
				bSpcAtkEnabled = true;

			_spcAtk_handlerImage.sprite = (bSpcAtkEnabled) ? _spcAtk_handlerImage_Enable : _spcAtk_handlerImage_Disable;

			//-----
			bool bDashEnabled = false;
			if (pPlayer.DashSkillBook.GetSkillCooling() <= 0.0f)
				bDashEnabled = true;

			_dash_handlerImage.sprite = (bDashEnabled) ? _dash_handlerImage_Enable : _dash_handlerImage_Disable;
		}
	}

	//---------------------------------------
	private void OnMoveHandler(Vector2 vMove)
	{
		bIsMoveUpdated = true;
		_lastMoveInput = vMove;
		_lerpMoveInput = vMove.normalized;
	}

	private void OnMoveEnd()
	{
		bIsMoveUpdated = false;
		_lastMoveInput = Vector2.zero;
	}

	//---------------------------------------
	private void OnAttackHandler_Down(Vector2 vMove)
	{
		bAttackKeyDowned = true;
		_lastAtkInput = vMove;
	}

	private void OnAttackHandler_Up()
	{
		bAttackKeyDowned = false;
		_lastAtkInput = Vector2.zero;
	}

	//---------------------------------------
	private void OnSpcAtkHandler_Down()
	{
		_spcAtkKey.ButtonState = eButtonState.Down;
	}

	private void OnSpcAtkHandler_Up(float fTime)
	{
		_spcAtkKey.ButtonState = eButtonState.Free;
	}

	//---------------------------------------
	private void OnDashHandler_Down()
	{
		_dashKey.ButtonState = eButtonState.Down;
	}

	private void OnDashHandler_Up(float fTime)
	{
		_dashKey.ButtonState = eButtonState.Free;
	}
}

/////////////////////////////////////////
//---------------------------------------