using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HT;


/////////////////////////////////////////
//---------------------------------------
public sealed class AIActor_Extend_GT : AIActor_Extend
{
	//---------------------------------------
	[Header("BoomStick Mode")]
	[SerializeField]
	private ActorBuff _boomStick_Buff = null;
	[SerializeField]
	private string _boomStick_IDLE = null;
	[SerializeField]
	private string _boomStick_MOVE = null;
	[SerializeField]
	private string _boomStick_DASH = null;
	[SerializeField]
	private string _boomStick_OnAnim = null;
	[SerializeField]
	private string _boomStick_OffAnim = null;
	[SerializeField]
	private float _boomStick_Chase_RefreshTime = 3.0f;
	[SerializeField]
	private float _boomStick_Chase_Distance = 5.0f;
	[SerializeField]
	private float _boomStick_Chase_Min = 3.0f;
	[SerializeField]
	private float _boomStick_Chase_Max = 7.0f;

	private float _boomStick_PosRefreshLeastTime = 0.0f;
	private GameObject _boomStick_ChasingDummy = null;

	//---------------------------------------
	[Header("TwoHand Sword Mode")]
	[SerializeField]
	private ActorBuff _ths_Buff = null;
	[SerializeField]
	private string _ths_IDLE = null;
	[SerializeField]
	private string _ths_MOVE = null;
	[SerializeField]
	private string _ths_DASH = null;
	[SerializeField]
	private string _ths_OnAnim = null;
	[SerializeField]
	private string _ths_OffAnim = null;

	//---------------------------------------
	[Header("OneHand Sword Mode")]
	[SerializeField]
	private ActorBuff _ohs_Buff = null;
	[SerializeField]
	private string _ohs_IDLE = null;
	[SerializeField]
	private string _ohs_MOVE = null;
	[SerializeField]
	private string _ohs_DASH = null;
	[SerializeField]
	private string _ohs_OnAnim = null;
	[SerializeField]
	private string _ohs_OffAnim = null;

	//---------------------------------------
	[Header("ModeSwap Info")]
	[SerializeField]
	private float _modeSwap_Time_Min = 7.5f;
	[SerializeField]
	private float _modeSwap_Time_Max = 15.0f;

	private float _modeSwap_LeastTime = 0.0f;

	//---------------------------------------
	[Header("Dash Info")]
	[SerializeField]
	private float _dash_CoolTime = 5.0f;
	[SerializeField]
	private float _dash_CoolTime_Mobile = 5.0f;
	[SerializeField]
	private float _dash_Range = 1.5f;
	[SerializeField]
	private ActorBuff _dash_Buff = null;
	[SerializeField]
	private AudioClip _dash_Sound = null;
	[SerializeField]
	private ParticleSystem _dash_Effect = null;

	private string _dashAnimName = null;
	private float _dash_LeastTime = 0.0f;

	//---------------------------------------
	public enum eWeaponType
	{
		BoomStick,
		TwoHandSword,
		OneHandSword,

		Max,
	}

	[Header("STATUS")]
	[SerializeField]
	private eWeaponType _curWeaponType = eWeaponType.Max;

	//---------------------------------------
	private Coroutine _process = null;

	//---------------------------------------
	private void OnEnable()
	{
		BattleFramework._Instance.onPlayerAttack += OnPlayerAttacked;
	}

	private void OnDisable()
	{
		if (BattleFramework._Instance != null)
			BattleFramework._Instance.onPlayerAttack -= OnPlayerAttacked;
	}

	//---------------------------------------
	public override void Extended_Init()
	{
		_boomStick_ChasingDummy = HT.Utils.Instantiate();
		_boomStick_ChasingDummy.SetActive(false);
	}

	public override void Extended_PostInit()
	{
	}

	public override void Extended_Frame()
	{
		IActorBase pPlayer = BattleFramework._Instance.m_pPlayerActor;
		if (_process == null && m_pActorBase.GetActorState() != IActorBase.eActorState.eAction)
		{
			if (_curWeaponType == eWeaponType.Max || _modeSwap_LeastTime <= 0.0f)
			{
				eWeaponType wpnType = eWeaponType.Max;
				for (int nInd = 0; nInd < 1000; ++nInd)
				{
					eWeaponType eType = (eWeaponType)HT.RandomUtils.Range(0, (int)eWeaponType.Max);
					if (eType == _curWeaponType)
						continue;

					wpnType = eType;
				}
				
				SetWeapon(wpnType);
			}
			else
			{
				switch(_curWeaponType)
				{
					case eWeaponType.BoomStick:
						{
							bool bRefreshPos = false;
							if (_boomStick_PosRefreshLeastTime > 0.0f)
							{
								Vector3 vPlayerPos = pPlayer.transform.position;
								Vector3 vPrevDummyPos = _boomStick_ChasingDummy.transform.position;
								float fDistace = Vector3.Distance(vPrevDummyPos, vPlayerPos);

								if ((fDistace > _boomStick_Chase_Min && fDistace < _boomStick_Chase_Max) == false)
									bRefreshPos = true;
							}
							else
								bRefreshPos = true;

							if (bRefreshPos)
								RefreshChasingTarget();
						}
						break;
				}
			}
		}

		float fDeltaTime = HT.TimeUtils.GameTime;
		if (_dash_LeastTime > 0.0f)
			_dash_LeastTime -= fDeltaTime;

		if (_modeSwap_LeastTime > 0.0f)
			_modeSwap_LeastTime -= fDeltaTime;

		if (_boomStick_PosRefreshLeastTime > 0.0f)
			_boomStick_PosRefreshLeastTime -= fDeltaTime;
	}

	public override void Extended_Release()
	{
	}

	//---------------------------------------
	private void RefreshChasingTarget()
	{
		_boomStick_PosRefreshLeastTime = _boomStick_Chase_RefreshTime;

		Vector3 vPlayerPos = BattleFramework._Instance.m_pPlayerActor.transform.position;
		Vector3 vPrevDummyPos = _boomStick_ChasingDummy.transform.position;

		//-----
		Vector3 vChasingDummyPos = vPlayerPos;

		Vector3 vDir = (gameObject.transform.position - vPlayerPos).normalized;
		vDir = Quaternion.Euler(0.0f, HT.RandomUtils.Range(-30.0f, 30.0f), 0.0f) * vDir;

		vChasingDummyPos += vDir * _boomStick_Chase_Distance;
		_boomStick_ChasingDummy.transform.position = vChasingDummyPos;
	}

	//---------------------------------------
	public void SetWeapon(eWeaponType eType)
	{
		if (_process != null)
			return;
		
		_modeSwap_LeastTime = HT.RandomUtils.Range(_modeSwap_Time_Min, _modeSwap_Time_Max);
		_process = StartCoroutine(SetWeapon_Internal(eType));
	}

	private IEnumerator SetWeapon_Internal(eWeaponType eType)
	{
		if (_curWeaponType != eWeaponType.Max)
		{
			string szOffAnimName = null;
			switch(_curWeaponType)
			{
				case eWeaponType.OneHandSword:
					szOffAnimName = _ohs_OffAnim;
					break;

				case eWeaponType.TwoHandSword:
					szOffAnimName = _ths_OffAnim;
					break;

				case eWeaponType.BoomStick:
					szOffAnimName = _boomStick_OffAnim;
					break;
			}

			m_pActorBase.RemoveActorBuff(_boomStick_Buff, false);
			m_pActorBase.RemoveActorBuff(_ths_Buff, false);
			m_pActorBase.RemoveActorBuff(_ohs_Buff, false);

			m_pActorBase.SetAction(szOffAnimName);

			AnimationClip pOffClip = m_pActorBase.m_pAnimations.GetClip(szOffAnimName);
			yield return new WaitForSeconds(pOffClip.length);
		}

		//-----
		ActorBuff pBuff = null;
		string szOnAnimName = null;
		switch (eType)
		{
			case eWeaponType.OneHandSword:
				pBuff = _ohs_Buff;
				szOnAnimName = _ohs_OnAnim;
				m_pActorBase.m_szIDLEAnimName = _ohs_IDLE;
				m_pActorBase.m_szMOVEAnimName = _ohs_MOVE;
				_dashAnimName = _ohs_DASH;
				break;

			case eWeaponType.TwoHandSword:
				pBuff = _ths_Buff;
				szOnAnimName = _ths_OnAnim;
				m_pActorBase.m_szIDLEAnimName = _ths_IDLE;
				m_pActorBase.m_szMOVEAnimName = _ths_MOVE;
				_dashAnimName = _ths_DASH;
				break;

			case eWeaponType.BoomStick:
				pBuff = _boomStick_Buff;
				szOnAnimName = _boomStick_OnAnim;
				m_pActorBase.m_szIDLEAnimName = _boomStick_IDLE;
				m_pActorBase.m_szMOVEAnimName = _boomStick_MOVE;
				_dashAnimName = _boomStick_DASH;
				break;
		}

		m_pActorBase.AddActorBuff(pBuff);
		_curWeaponType = eType;

		m_pActorBase.SetAction(szOnAnimName);

		AnimationClip pOnClip = m_pActorBase.m_pAnimations.GetClip(szOnAnimName);
		yield return new WaitForSeconds(pOnClip.length);

		//-----
		switch (_curWeaponType)
		{
			case eWeaponType.BoomStick:
				m_pActorBase.ChasingTarget = _boomStick_ChasingDummy;
				_boomStick_PosRefreshLeastTime = 0.0f;
				break;

			case eWeaponType.OneHandSword:
			case eWeaponType.TwoHandSword:
				m_pActorBase.ChasingTarget = null;
				break;
		}

		_process = null;
	}

	//---------------------------------------
	private void OnPlayerAttacked(Projectile pProj)
	{
		if (_process != null)
			return;

		if (m_pActorBase.GetActorState() == IActorBase.eActorState.eAction)
			return;

		if (pProj == null)
			return;

		if (string.IsNullOrEmpty(_dashAnimName))
			return;

		if (_dash_LeastTime > 0.0f)
			return;

		Vector3 vDestination = pProj.transform.position;
		float fDistance = Vector3.Distance(gameObject.transform.position, vDestination);
		vDestination += pProj.m_vMoveVector * fDistance;

		fDistance = Vector3.Distance(gameObject.transform.position, vDestination);
		if (fDistance < _dash_Range)
			_process = StartCoroutine(Dash_Internal(pProj));
	}

	private IEnumerator Dash_Internal(Projectile pProj)
	{
		_dash_LeastTime = _dash_CoolTime;
		if (HTAfxPref.IsMobilePlatform || HTInputManager.Instance.JoystickConnected)
			_dash_LeastTime = _dash_CoolTime_Mobile;

		//-----
		Vector3 vDir = gameObject.transform.position - pProj.transform.position;
		vDir.Normalize();

		float fRotate = 0.0f;
		switch (_curWeaponType)
		{
			case eWeaponType.BoomStick:
				fRotate = (HT.RandomUtils.Range(0.0f, 1.0f) < 0.5f) ? 45.0f : -45.0f;
				break;

			case eWeaponType.OneHandSword:
			case eWeaponType.TwoHandSword:
				fRotate = (HT.RandomUtils.Range(0.0f, 1.0f) < 0.5f) ? 135.0f : -135.0f;
				break;
		}
		vDir = Quaternion.Euler(0.0f, fRotate, 0.0f) * vDir;

		//-----
		float fTime = 0.5f;
		m_pActorBase.SetAction(_dashAnimName);
		m_pActorBase.SetReadyTime(fTime);

		HT.HTSoundManager.PlaySound(_dash_Sound);

		ParticleSystem pEffect = HT.Utils.InstantiateFromPool(_dash_Effect);
		HT.Utils.SafeDestroy(pEffect.gameObject, pEffect.TotalSimulationTime());

		//-----
		m_pActorBase.AddActorBuff(_dash_Buff);
		m_pActorBase.m_vViewVector = vDir;
		m_pActorBase.m_vMoveVector = vDir;

		m_pActorBase.m_bActorIncapacitation = true;

		yield return new WaitForSeconds(fTime);

		m_pActorBase.m_bActorIncapacitation = false;
		m_pActorBase.SetActorState(IActorBase.eActorState.eIdle);

		//-----
		RefreshChasingTarget();
		
		_process = null;
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void OnExplosionForce()
	{
		Collider[] vColliders = GetComponentsInChildren<Collider>();
		for (int nInd = 0; nInd < vColliders.Length; ++nInd)
		{
			if (vColliders[nInd].enabled == false)
				continue;

			Rigidbody pBody = vColliders[nInd].GetComponent<Rigidbody>();
			if (pBody == null)
				continue;

			pBody.constraints = RigidbodyConstraints.None;
			pBody.AddExplosionForce(1000.0f, gameObject.transform.position, 10.0f);
		}
	}
}


/////////////////////////////////////////
//---------------------------------------