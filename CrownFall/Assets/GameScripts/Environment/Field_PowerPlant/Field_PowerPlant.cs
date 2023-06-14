using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public sealed class Field_PowerPlant : Field
{
	//---------------------------------------
	[Header("WALL")]
	[SerializeField]
	private PowerPlant_Wall _wall_Left = null;
	[SerializeField]
	private PowerPlant_Wall _wall_Right = null;

	//---------------------------------------
	[Header("POWER FLOW EFFECT")]
	[SerializeField]
	private Animation[] _powerFlows = null;
	[SerializeField]
	private float _powerFlow_RepeatTime = 0.15f;
	[SerializeField]
	private string _powerFlow_AnimName = null;

	private float _powerFlowTime = 0.0f;

	//---------------------------------------
	[Header("LIGHTNING RODS")]
	[SerializeField]
	private PowerPlant_LightningRod_Group[] _lightningRods = null;

	[Header("LIGHTNING RODS - TIMES")]
	[SerializeField]
	private float _rod_RepeatTime_Lv1 = 1.25f;
	[SerializeField]
	private float _rod_RepeatTime_Lv2 = 1.0f;
	[SerializeField]
	private float _rod_RepeatTime_Lv3 = 0.75f;
	[SerializeField]
	private float _rod_DelayTime = 1.0f;

	//---------------------------------------
	[Header("LIGHTNING STRIKE")]
	[SerializeField]
	private PowerPlant_LightningStrike[] _lightningStrikes = null;

	[Header("LIGHTNING STRIKE - TIMES")]
	[SerializeField]
	private float _strike_RepeatTime = 10.0f;
	[SerializeField]
	private float _strike_Term = 0.25f;
	[SerializeField]
	private int _strike_Count_Lv1 = 3;
	[SerializeField]
	private int _strike_Count_Lv2 = 4;
	[SerializeField]
	private int _strike_Count_Lv3 = 5;
	[SerializeField]
	private float _strike_DelayTime = 1.25f;

	//---------------------------------------
	[Header("ELECTRIC WINDMILL")]
	[SerializeField]
	private BossHAL_ElectricWindmill[] _instance_windmill = null;
	[SerializeField]
	private float _windmill_RepeatTime_Lv1 = 18.0f;
	[SerializeField]
	private float _windmill_RepeatTime_Lv2 = 17.0f;
	[SerializeField]
	private float _windmill_RepeatTime_Lv3 = 16.0f;
	[SerializeField]
	private float _windmill_LifeTime_Lv1 = 30.0f;
	[SerializeField]
	private float _windmill_LifeTime_Lv2 = 35.0f;
	[SerializeField]
	private float _windmill_LifeTime_Lv3 = 40.0f;
	[SerializeField]
	Vector3 _windmill_RandomRange_Min = Vector3.zero;
	[SerializeField]
	Vector3 _windmill_RandomRange_Max = Vector3.zero;

	//---------------------------------------
	[Header("ESCAPE SEQUENCE")]
	[SerializeField]
	private Animation _escape_AlertAnim = null;
	[SerializeField]
	private AudioClip _escape_AlertSounds = null;
	[SerializeField]
	private HT.HTLightning _escape_lightningEffect = null;
	[SerializeField]
	private float _escape_AlertTime = 7.5f;
	[SerializeField]
	private float _escape_AlertScale = 13.7f;
	[SerializeField]
	private float _escape_DamageTerm = 3.0f;
	[SerializeField]
	private GameObject _escape_SparkStart = null;
	[SerializeField]
	private float _escape_SparkEffectRepeat = 0.5f;
	[SerializeField]
	private float _escape_Spark_Width = 0.2f;
	[SerializeField]
	private float _escape_Spark_Size = 1.0f;
	[SerializeField]
	private float _escape_Spark_LifeTime = 0.2f;
	[SerializeField]
	private float _escape_Spark_Delay = 0.2f;
	[SerializeField]
	private AudioClip _escape_SparkSounds = null;
	[SerializeField]
	private GameObject _escape_ExplosenEffect = null;
	[SerializeField]
	private float _escape_ExplosenCamShake = 2.0f;
	[SerializeField]
	private AudioClip _escape_ExplosenSounds = null;

	//---------------------------------------
	[Header("BEAM STRIKE")]
	[SerializeField]
	private GameObject _beam_Instance_Lv1 = null;
	[SerializeField]
	private GameObject _beam_Instance_Lv2 = null;
	[SerializeField]
	private GameObject _beam_Instance_Lv3 = null;
	[SerializeField]
	private float _beam_Repeat_Lv1 = 28.0f;
	[SerializeField]
	private float _beam_Repeat_Lv2 = 25.0f;
	[SerializeField]
	private float _beam_Repeat_Lv3 = 22.0f;

	//---------------------------------------
	[Header("ARCHIVEMENT - ESCAPE SEQUENCE")]
	[SerializeField]
	private string _acv_NoEscape = null;
	[SerializeField]
	private float _acv_NoEscape_RequireTime = 30.0f;

	private float _acv_NoEscape_LeastTime = 0.0f;
	public float Acv_NoEscape_LeastTime { get { return _acv_NoEscape_LeastTime; } }

	[SerializeField]
	private string _acv_NoBelt = null;
	[SerializeField]
	private float _acv_NoBelt_RequireTime = 25.0f;

	private float _acv_NoBelt_LeastTime = 0.0f;
	public float Acv_NoBelt_LeastTime { get { return _acv_NoBelt_LeastTime; } }

	//---------------------------------------
	private Coroutine _lightningRod_Proc = null;
	private Coroutine _escapeSeq_Proc = null;
	private Coroutine _escapeSeqSpark_Proc = null;
	private Coroutine _beam_Proc = null;

	private bool _playerIsOnField = false;

	private bool _escapeSeq_InvincibleState = false;

	//---------------------------------------
	public override void Init()
	{
		base.Init();

		_acv_NoEscape_LeastTime = _acv_NoEscape_RequireTime;
		_acv_NoBelt_LeastTime = _acv_NoBelt_RequireTime;
	}

	protected override void Frame()
	{
		base.Frame();

		//-----
		_powerFlowTime -= HT.TimeUtils.GameTime;
		if (_powerFlowTime < 0.0f)
		{
			_powerFlowTime = _powerFlow_RepeatTime;
			int nIndex = Random.Range(0, _powerFlows.Length);

			Animation pAnim = _powerFlows[nIndex];
			if (pAnim.isPlaying == false)
				pAnim.Play(string.Format(_powerFlow_AnimName, Random.Range(0, pAnim.GetClipCount())));
		}

		//-----
		if (m_bIsLobbyField)
			return;

	}

	//---------------------------------------
	public override void OnBattleStart()
	{
		base.OnBattleStart();
		
		_lightningRod_Proc = StartCoroutine(LightningRod_Internal());
		_beam_Proc = StartCoroutine(Beam_Internal());

		StartCoroutine(LightningStrike_Internal());
		StartCoroutine(Windmill_Internal());
	}

	public override void OnBattleEnd(bool bPlayerWin)
	{
		base.OnBattleEnd(bPlayerWin);

		if (bPlayerWin)
		{
			if (_acv_NoBelt_LeastTime > 0.0f)
			{
				Archives pArchives = ArchivementManager.Instance.FindArchive(_acv_NoBelt);
				pArchives.Archive.OnArchiveCount(1);
			}

			if (_acv_NoEscape_LeastTime <= 0.0f)
			{
				Archives pArchives = ArchivementManager.Instance.FindArchive(_acv_NoEscape);
				pArchives.Archive.OnArchiveCount(1);
			}
		}

		StopAllCoroutines();
	}

	//---------------------------------------
	private void OnTriggerEnter(Collider other)
	{
		if (BattleFramework._Instance == null)
			return;

		if (other.gameObject == BattleFramework._Instance.m_pPlayerActor.gameObject)
			_playerIsOnField = true;
	}

	private void OnTriggerExit(Collider other)
	{
		if (BattleFramework._Instance == null)
			return;

		if (other.gameObject == BattleFramework._Instance.m_pPlayerActor.gameObject)
			_playerIsOnField = false;
	}

	//---------------------------------------
	private IEnumerator LightningRod_Internal()
	{
		eGameDifficulty eDiff = GameFramework._Instance.m_pPlayerData.m_eDifficulty;

		float fDelay = _rod_RepeatTime_Lv1;
		switch(eDiff)
		{
			case eGameDifficulty.eNormal:
				fDelay = _rod_RepeatTime_Lv2;
				break;
			case eGameDifficulty.eHard:
				fDelay = _rod_RepeatTime_Lv3;
				break;
		}

		while(true)
		{
			yield return new WaitForSeconds(fDelay);

			int nIndex = Random.Range(0, _lightningRods.Length);
			_lightningRods[nIndex].ActivateRod(_rod_DelayTime);
		}
	}

	//---------------------------------------
	private IEnumerator LightningStrike_Internal()
	{
		eGameDifficulty eDiff = GameFramework._Instance.m_pPlayerData.m_eDifficulty;

		int nStrikeCount = _strike_Count_Lv1;
		switch (eDiff)
		{
			case eGameDifficulty.eNormal:
				nStrikeCount = _strike_Count_Lv2;
				break;
			case eGameDifficulty.eHard:
				nStrikeCount = _strike_Count_Lv3;
				break;
		}

		while (true)
		{
			yield return new WaitForSeconds(_strike_RepeatTime);

			for (int nInd = 0; nInd < nStrikeCount; ++nInd)
			{
				Vector3 vPos = BattleFramework._Instance.m_pPlayerActor.transform.position;
				_lightningStrikes[nInd].ActivateStike(vPos, _strike_DelayTime);

				yield return new WaitForSeconds(_strike_Term);
			}
		}
	}

	//---------------------------------------
	private IEnumerator Windmill_Internal()
	{
		eGameDifficulty eDiff = GameFramework._Instance.m_pPlayerData.m_eDifficulty;

		float fRepeatTime = _windmill_RepeatTime_Lv1;
		float fLifeTime = _windmill_LifeTime_Lv1;
        BossHAL_ElectricWindmill pWindmill_Instance = _instance_windmill[(int)eDiff];

        switch (eDiff)
		{
			case eGameDifficulty.eNormal:
				fRepeatTime = _windmill_RepeatTime_Lv2;
				fLifeTime = _windmill_LifeTime_Lv2;

                break;
			case eGameDifficulty.eHard:
				fRepeatTime = _windmill_RepeatTime_Lv3;
				fLifeTime = _windmill_LifeTime_Lv3;
				break;
		}

		while (true)
		{
			yield return new WaitForSeconds(fRepeatTime);
            
			BossHAL_ElectricWindmill pWindmill = HT.Utils.Instantiate(pWindmill_Instance);

			Vector3 vPos = new Vector3();
			vPos.x = Random.Range(_windmill_RandomRange_Min.x, _windmill_RandomRange_Max.x);
			vPos.y = Random.Range(_windmill_RandomRange_Min.y, _windmill_RandomRange_Max.y);
			vPos.z = Random.Range(_windmill_RandomRange_Min.z, _windmill_RandomRange_Max.z);

			pWindmill.Init(vPos, fLifeTime);
		}
	}

	//---------------------------------------
	private IEnumerator Beam_Internal()
	{
		eGameDifficulty eDiff = GameFramework._Instance.m_pPlayerData.m_eDifficulty;

		float fRepeat = _beam_Repeat_Lv1;
		GameObject pBeam = _beam_Instance_Lv1;
		switch (eDiff)
		{
			case eGameDifficulty.eNormal:
				fRepeat = _beam_Repeat_Lv2;
				pBeam = _beam_Instance_Lv2;
				break;

			case eGameDifficulty.eHard:
				fRepeat = _beam_Repeat_Lv3;
				pBeam = _beam_Instance_Lv3;
				break;
		}

		while (true)
		{
			yield return new WaitForSeconds(fRepeat);

			GameObject pNewBeam = HT.Utils.Instantiate(pBeam);
			Vector3 vPos = BattleFramework._Instance.m_pPlayerActor.transform.position;
			pNewBeam.transform.position = GameFramework._Instance.GetPositionByPhysic(vPos);
		}
	}

	//---------------------------------------
	public void OnCastEscapeSequence()
	{
		if (_lightningRod_Proc != null)
			StopCoroutine(_lightningRod_Proc);
		_lightningRod_Proc = null;

		if (_beam_Proc != null)
			StopCoroutine(_beam_Proc);
		_beam_Proc = null;

		if (_escapeSeq_Proc == null)
			_escapeSeq_Proc = StartCoroutine(EscapeSequence_Internal());

		if (_escapeSeqSpark_Proc == null)
			_escapeSeqSpark_Proc = StartCoroutine(EscapeSequence_Sparks_Internal());
	}

	public void StopCastEscapeSequence()
	{
		if (_escapeSeqSpark_Proc != null)
			StopCoroutine(_escapeSeqSpark_Proc);
		_escapeSeqSpark_Proc = null;

		if (_escapeSeq_Proc != null)
			StopCoroutine(_escapeSeq_Proc);
		_escapeSeq_Proc = null;

		if (_lightningRod_Proc == null)
			_lightningRod_Proc = StartCoroutine(LightningRod_Internal());

		if (_beam_Proc == null)
			_beam_Proc = StartCoroutine(Beam_Internal());
	}

	private IEnumerator EscapeSequence_Internal()
	{
		//-----
		bool bLeftBridge = (Random.Range(0.0f, 1.0f) > 0.5f) ? true : false;
		PowerPlant_Wall pWall = (bLeftBridge) ? _wall_Left : _wall_Right;
		pWall.DoorOpen(true, true);

		Vector3 vPos = pWall.GetDoorPosition(true);
		Object_AreaAlert pAlertObj = BattleFramework._Instance.CreateAreaSafty(vPos, 0.0f, 999.0f, true);

		//-----
		IActorBase pPlayer = BattleFramework._Instance.m_pPlayerActor;

		bool bJustAlert = true;
		float fTimeRepeat = _escape_AlertTime;
		BattleFramework._Instance.CreateAreaAlert(Vector3.zero, _escape_AlertScale, _escape_AlertScale, fTimeRepeat - 0.1f);
		while (true)
		{
			if (pWall.IsOnBelt(pPlayer.gameObject))
				break;

			float fDeltaTime = HT.TimeUtils.GameTime;
			_acv_NoEscape_LeastTime -= fDeltaTime;

			//-----
			fTimeRepeat -= fDeltaTime;
			if (fTimeRepeat <= 0.0f)
			{
				GameObject pNewEffect = HT.Utils.Instantiate(_escape_ExplosenEffect);
				pNewEffect.transform.position = Vector3.zero;

				CameraManager._Instance.SetCameraShake(_escape_ExplosenCamShake);

				HT.HTSoundManager.PlaySound(_escape_ExplosenSounds);

				//-----
				fTimeRepeat = _escape_DamageTerm;
				BattleFramework._Instance.CreateAreaAlert(Vector3.zero, _escape_AlertScale, _escape_AlertScale, fTimeRepeat - 0.1f);

				if (bJustAlert)
					bJustAlert = false;

				pPlayer.OnDamaged(1);
			}

			yield return new WaitForEndOfFrame();
		}

		HT.Utils.SafeDestroy(pAlertObj.gameObject);

		//-----
		_escapeSeq_InvincibleState = true;
		pWall.DoorOpen(true, false);
		pWall.DoorOpen(false, true);

		while(true)
		{
			if (_playerIsOnField)
				break;

			_acv_NoBelt_LeastTime -= HT.TimeUtils.GameTime;

			yield return new WaitForEndOfFrame();
		}

		//-----
		pWall.DoorOpen(false, false);
		_escapeSeq_InvincibleState = false;

		StopCastEscapeSequence();
		yield break;
	}

	private IEnumerator EscapeSequence_Sparks_Internal()
	{
		_escape_AlertAnim.Play();
		HT.HTSoundManager.PlaySound(_escape_AlertSounds);

		_escape_lightningEffect.SetLightningInfo(_escape_Spark_Width, _escape_Spark_Size, 10);

		//float fLightningTime = _escape_SparkEffectRepeat;
		while(true)
		{
			yield return new WaitForSeconds(_escape_SparkEffectRepeat);

			GameObject pSparkTarget = null;
			PowerPlant_LightningRod_Group pRod = _lightningRods[Random.Range(0, _lightningRods.Length)];
			pSparkTarget = pRod.GetRod((Random.Range(0, 1.0f) > 0.5f)? true : false).Rod_Light.gameObject;
			_escape_lightningEffect.Init(_escape_SparkStart, pSparkTarget, _escape_Spark_LifeTime, _escape_Spark_Delay);

			HT.HTSoundManager.PlaySound(_escape_SparkSounds);
		}
	}

	public bool IsProcessing_EscapeSeq()
	{
		return (_escapeSeq_Proc != null) ? true : false;
	}

	public bool IsEscapeSeq_InvincibleState()
	{
		return _escapeSeq_InvincibleState;
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------