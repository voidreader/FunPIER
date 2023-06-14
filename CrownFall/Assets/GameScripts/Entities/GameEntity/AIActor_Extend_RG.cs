using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HT;


/////////////////////////////////////////
//---------------------------------------
public sealed class AIActor_Extend_RG : AIActor_Extend
{
    //---------------------------------------
    [Header("Extended Actor Info")]
    [SerializeField]
    private ActorBuff _floorBuff_1F = null;
    [SerializeField]
    private ActorBuff _floorBuff_2F = null;

    //---------------------------------------
    [Header("Skill - Play Organ")]
    [SerializeField]
    private float _organ_DelayTime = 30.0f;
    [SerializeField]
    private float _organ_PlayTime = 15.0f;
    [SerializeField]
    private ActorBuff _organ_PlayBuff = null;
    [SerializeField]
    private string _organ_DisappearAnimName = null;
    [SerializeField]
    private string _organ_AppearAnimName = null;
    [SerializeField]
    private AudioClip _organ_PlaySound = null;
    [SerializeField]
    private string _organ_PlayAnimName = null;
    [SerializeField]
    private string _organ_EndAnimName = null;
    [SerializeField]
    private string _organ_PosDummyName = null;
    [SerializeField]
    private SpawnActor[] _organ_SpawnActor = null;
    [SerializeField]
    private float[] _organ_SpawnActor_Repeat = null;
    [SerializeField]
    private int _organ_SpawnActor_MaxCount = 5;
	[SerializeField]
	private ParticleSystem _organ_SpawnActor_Effect = null;

	private float _organPlay_LeastTime = -1.0f;
	private HT.HTGameAudioMisc _organ_PlayingSoundMisc = null;

	//---------------------------------------
	[Header("Skill - Into Picture")]
	[SerializeField]
	private string _intoPic_IN_From1F = null;
	[SerializeField]
	private string _intoPic_IN_From2F = null;
	[SerializeField]
	private string _intoPic_OUT = null;
	[SerializeField]
	private string[] _intoPic_Dummy_StairView_1F = null;
	[SerializeField]
	private string[] _intoPic_Dummy_StairView_2F = null;
	[SerializeField]
	private float[] _intoPic_RequireDamage = null;
	[SerializeField]
	private string _intoPic_DestPivot_1F = null;
	[SerializeField]
	private string _intoPic_DestPivot_2F = null;
    [SerializeField]
    private string _intoPic_PoltergeistSkill = null;
	[SerializeField]
	private ActorBuff _intoPic_MoveDownBuff = null;

	//---------------------------------------
	[Header("Archives - Ghost Spawn")]
	[SerializeField]
	private string _archive_GhostSpawn_Name = null;
	[SerializeField]
	private int _archive_GhostSpawn_RequireCount = 10;
	[SerializeField]
	private float _archive_GhostSpawn_RequireTime = 10.0f;

	private float _archive_GhostSpawn_ProcTime = 0.0f;

	[Header("Archives - Ghost Kill")]
	[SerializeField]
	private string _archive_GhostKill_Name = null;
	[SerializeField]
	private int _archive_GhostKill_RequireCount = 5;
	[SerializeField]
	private float _archive_GhostKill_RequireTime = 0.2f;

	private float _archive_GhostKill_LeastTime = 0.0f;

	[Header("Archives - Get Out")]
	[SerializeField]
	private string _archive_GetOut_Name = null;
	[SerializeField]
	private float _archive_GetOut_RequireTime = 13.0f;

	[Header("Archives - No Disturbance")]
	[SerializeField]
	private string _archive_NoDisturbance_Name = null;

	[Header("Archives - Disturbance")]
	[SerializeField]
	private string _archive_Disturbance_Name = null;
	[SerializeField]
	private int _archive_Disturbance_RequireDamage = 400;

	private int _archive_Disturbance_TotalDamaged = 0;

	//---------------------------------------
	private bool _isIn1F = false;
	private Coroutine _stairProc = null;

    //---------------------------------------
    public override void Extended_PostInit()
    {
		AddFloorBuff_1F();

		_archive_GhostSpawn_ProcTime = 0.0f;
		_archive_GhostKill_LeastTime = 0.0f;
		_archive_Disturbance_TotalDamaged = 0;
	}

    //---------------------------------------
    public override void Extended_Frame()
    {
        if (_isIn1F)
        {
            if (_organPlay_LeastTime > 0.0f)
            {
                _organPlay_LeastTime -= HT.TimeUtils.GameTime;
                if (_organPlay_LeastTime <= 0.0f)
				{
					_organPlay_LeastTime = 0.1f;

					do
					{
						if (m_pActorBase.GetActorState() == IActorBase.eActorState.eAction)
							break;

						_organPlay_LeastTime = -1.0f;
						StartCoroutine(PlayOrgan_Internal());
					}
					while (false);
				}
            }
        }
        else
        {
			if (m_pActorBase.FindEnabledActorBuff(_intoPic_MoveDownBuff) != null)
			{
				m_pActorBase.RemoveActorBuff(_intoPic_MoveDownBuff, false);
				StartCoroutine(IntoPicture_Internal());
			}
        }

		//-----
		if (_archive_GhostSpawn_RequireCount <= SpawnActor_Extend_Ghost.SpawnedGhostCount)
		{
			_archive_GhostSpawn_ProcTime += HT.TimeUtils.GameTime;
			if (_archive_GhostSpawn_ProcTime >= _archive_GhostSpawn_RequireTime)
			{
				Archives pArchive = ArchivementManager.Instance.FindArchive(_archive_GhostSpawn_Name);
				pArchive.Archive.OnArchiveCount(1);
			}
		}
		else
			_archive_GhostSpawn_ProcTime = 0.0f;

		//-----
		if (SpawnActor_Extend_Ghost.KilledGhostCount > 0)
		{
			if (_archive_GhostKill_LeastTime <= 0.0f)
				_archive_GhostKill_LeastTime = _archive_GhostKill_RequireTime;

			else
			{
				_archive_GhostKill_LeastTime -= HT.TimeUtils.GameTime;
				if (SpawnActor_Extend_Ghost.KilledGhostCount >= _archive_GhostKill_RequireCount)
				{
					Archives pArchive = ArchivementManager.Instance.FindArchive(_archive_GhostKill_Name);
					if (pArchive.IsComplete() == false)
						pArchive.Archive.OnArchiveCount(1);
				}

				if (_archive_GhostKill_LeastTime <= 0.0f)
				{
					SpawnActor_Extend_Ghost.KilledGhostCount = 0;
					_archive_GhostKill_LeastTime = 0.0f;
				}
			}
		}
		else
			_archive_GhostKill_LeastTime = 0.0f;
	}

	public override void Extended_Release()
	{
		if (_stairProc != null)
		{
			Field_Hall pHall = BattleFramework._Instance.m_pField as Field_Hall;
			pHall.StopPictureProc();
			pHall.SetPictureCanAttack(false);

			_stairProc = null;
		}

		if (_organ_PlayingSoundMisc != null)
			_organ_PlayingSoundMisc.FadeOut();

		StopAllCoroutines();
	}

	//---------------------------------------
	public override void OnBattleStart()
	{
		base.OnBattleStart();

		BattleFramework._Instance.onPlayerDamaged += OnPlayerDamaged;
	}

	public override void OnBattleEnd(bool bPlayerWin)
	{
		base.OnBattleEnd(bPlayerWin);

		BattleFramework._Instance.onPlayerDamaged -= OnPlayerDamaged;
	}

	private void OnPlayerDamaged(int nValue)
	{
		_archive_GhostKill_LeastTime = 0.0f;
	}

	//---------------------------------------
	private IEnumerator PlayOrgan_Internal()
    {
        m_pActorBase.AddActorBuff(_organ_PlayBuff);

		int nCurHP = m_pActorBase.GetCurrHP();
		int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;

		//-----
		m_pActorBase.m_pRigidBody.velocity = Vector3.zero;
		m_pActorBase.m_pRigidBody.angularVelocity = Vector3.zero;
		m_pActorBase.m_vMoveVector = Vector3.zero;

		float fAnimWaitTime = m_pActorBase.SetAction(_organ_DisappearAnimName);
		m_pActorBase.SetActionReadyTime(float.PositiveInfinity);
		yield return new WaitForSeconds(fAnimWaitTime);

		//-----
		GameObject pPivot = BattleFramework._Instance.m_pField.FindDummyPivot(_organ_PosDummyName);
        m_pActorBase.transform.position = pPivot.transform.position;
        m_pActorBase.transform.rotation = pPivot.transform.rotation;
		m_pActorBase.m_vViewVector = Vector3.forward;

		m_pActorBase.m_pRigidBody.velocity = Vector3.zero;
		m_pActorBase.m_pRigidBody.angularVelocity = Vector3.zero;

		//-----
		fAnimWaitTime = m_pActorBase.SetAction(_organ_AppearAnimName);
		m_pActorBase.SetActionReadyTime(float.PositiveInfinity);
		yield return new WaitForSeconds(fAnimWaitTime);

		//-----
		HT.HTSoundManager.FadeOutMusic();
		_organ_PlayingSoundMisc = HT.HTSoundManager.PlaySound(_organ_PlaySound);

		m_pActorBase.SetAction(_organ_PlayAnimName);
		m_pActorBase.SetActionReadyTime(float.PositiveInfinity);
		
		float fSpawnDelay = _organ_SpawnActor_Repeat[nDiff];
        float fLeastTime = _organ_PlayTime;
        
        List<SpawnActor> vSpawnList = new List<SpawnActor>();

		//-----
        Field_Hall pHall = BattleFramework._Instance.m_pField as Field_Hall;
		int nCurPipeIndex = HT.RandomUtils.Range(0, pHall.Organ_PipeDummy.Length);
		DummyPivot pCurPipePivot = null;

		Vector3 vCurPos = m_pActorBase.transform.position;
		Quaternion vCurRot = m_pActorBase.transform.rotation;
		int nCurDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;
		while (fLeastTime > 0.0f)
		{
			m_pActorBase.transform.position = vCurPos;
			m_pActorBase.transform.rotation = vCurRot;

			m_pActorBase.m_pRigidBody.velocity = Vector3.zero;
			m_pActorBase.m_pRigidBody.angularVelocity = Vector3.zero;

			//-----
			float fDeltaTime = HT.TimeUtils.GameTime;
            fLeastTime -= fDeltaTime;

			fSpawnDelay -= fDeltaTime;
            if (fSpawnDelay <= 0.0f && vSpawnList.Count < _organ_SpawnActor_MaxCount)
            {
                fSpawnDelay = _organ_SpawnActor_Repeat[nDiff];

                SpawnActor pActor = HT.Utils.Instantiate(_organ_SpawnActor[nCurDiff]);
				pActor._parentActor = m_pActorBase;

				vSpawnList.Add(pActor);

                pActor.onDestroy = (SpawnActor pDestroyActor) => { vSpawnList.Remove(pDestroyActor); };
				
                pActor.transform.position = pCurPipePivot.transform.position;
                pActor.transform.forward = pCurPipePivot.transform.forward;

                pActor.Init();

				//-----
				nCurPipeIndex = HT.RandomUtils.Range(0, pHall.Organ_PipeDummy.Length);
				pCurPipePivot = null;
			}

			//-----
			if (pCurPipePivot == null)
			{
				pCurPipePivot = pHall.Organ_PipeDummy[nCurPipeIndex];

				ParticleSystem pParticle = _organ_SpawnActor_Effect.CreateInstanceFromPool();
				pParticle.transform.position = pCurPipePivot.transform.position;
				pParticle.transform.forward = pCurPipePivot.transform.forward;
			}

			//-----
			yield return new WaitForEndOfFrame();
		}

		//-----
		yield return new WaitForSeconds(2.5f);
		if (vSpawnList.Count > 0)
		{
			for (int nInd = 0; nInd < vSpawnList.Count; ++nInd)
			{
				vSpawnList[nInd].onDestroy = null;
				HT.Utils.SafeDestroy(vSpawnList[nInd].gameObject);
			}
		}

		vSpawnList = null;
		
		//-----
		//pOrganSound.FadeOut();
        HT.HTSoundManager.FadeInMusic();

		//-----
		_organ_PlayingSoundMisc = null;

		float fReadyTime = m_pActorBase.SetAction(_organ_EndAnimName);
        m_pActorBase.SetActionReadyTime(float.PositiveInfinity);

		m_pActorBase.RemoveActorBuff(_organ_PlayBuff, false);
		
		yield return new WaitForSeconds(fReadyTime + 2.0f);

		//-----
		int nPostHP = m_pActorBase.GetCurrHP();

		_archive_Disturbance_TotalDamaged += nCurHP - nPostHP;
		if (_archive_Disturbance_TotalDamaged >= _archive_Disturbance_RequireDamage)
		{
			_archive_Disturbance_TotalDamaged = 0;

			Archives pArchives = ArchivementManager.Instance.FindArchive(_archive_Disturbance_Name);
			pArchives.Archive.OnArchiveCount(1);
		}

		//-----
		_stairProc = StartCoroutine(IntoPicture_Internal());
    }

    //---------------------------------------
    private void AddFloorBuff_1F()
    {
        m_pActorBase.AddActorBuff(_floorBuff_1F);
        m_pActorBase.RemoveActorBuff(_floorBuff_2F, false);

        _organPlay_LeastTime = _organ_DelayTime;
        _isIn1F = true;
    }

    private void AddFloorBuff_2F()
    {
        m_pActorBase.RemoveActorBuff(_floorBuff_1F, false);
        m_pActorBase.AddActorBuff(_floorBuff_2F);

        _organPlay_LeastTime = -1.0f;
        _isIn1F = false;
	}

	//---------------------------------------
	private IEnumerator IntoPicture_Internal()
	{
		Field_Hall pHall = BattleFramework._Instance.m_pField as Field_Hall;
		int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;

		//-----
		bool bIsNowFirstFloor = _isIn1F;

		string szIntoPictureAnim = (bIsNowFirstFloor) ? _intoPic_IN_From1F : _intoPic_IN_From2F;
		AnimationClip pIntoClip = m_pActorBase.m_pAnimations.GetClip(szIntoPictureAnim);

		m_pActorBase.SetAction(szIntoPictureAnim);
		m_pActorBase.SetActionReadyTime(float.PositiveInfinity);

		yield return new WaitForSeconds(pIntoClip.length);

		//-----
		pHall.InitPictureProc((bIsNowFirstFloor) ? false : true);

		//yield return new WaitForSeconds(_intoPic_DelayForAttack);
		string[] vArrowDummies = (bIsNowFirstFloor) ? _intoPic_Dummy_StairView_1F : _intoPic_Dummy_StairView_2F;
		Object_AreaAlert[] vArrowDummyObjs = new Object_AreaAlert[vArrowDummies.Length];

		bool bArrowDummyAdded = false;
		Action pAddArrow = () =>
		{
			if (bArrowDummyAdded == false)
			{
				bArrowDummyAdded = true;

				Field pField = BattleFramework._Instance.m_pField;
				for (int nInd = 0; nInd < vArrowDummies.Length; ++nInd)
				{
					GameObject pObj = pField.FindDummyPivot (vArrowDummies [nInd]);
					vArrowDummyObjs [nInd] = BattleFramework._Instance.CreateInteractionNotice (pObj.transform.position);
				}
			}
		};

		Action pRemoveArrow = () =>
		{
			if (bArrowDummyAdded)
			{
				bArrowDummyAdded = false;
				for(int nInd = 0; nInd < vArrowDummies.Length; ++nInd)
				{
					BattleFramework._Instance.RemoveInteractionNotice (vArrowDummyObjs[nInd]);
					vArrowDummyObjs [nInd] = null;
				}
			}
		};

		pAddArrow ();

		float intoPictureTime = 0.0f;
		while (true)
		{
			//intoPictureTime += HT.TimeUtils.GameTime;
			if (bIsNowFirstFloor == BossRG_StairActivator.SecondFloorActivated)
				break;

			//if (BossRG_StairActivator.ThirdCamWallActivated)
			//	pRemoveArrow ();
			//else
			//	pAddArrow ();

			yield return new WaitForEndOfFrame();
		}

		pRemoveArrow ();

		pHall.SetIntoPictureProc((bIsNowFirstFloor) ? false : true);
		pHall.SetPictureCanAttack(true);

		//-----
		int nPrevHP = m_pActorBase.GetCurrHP();
		while(true)
		{
			int nCurHP = m_pActorBase.GetCurrHP();
			if ((nPrevHP - nCurHP) >= _intoPic_RequireDamage[nDiff])
				break;

			intoPictureTime += HT.TimeUtils.GameTime;
			yield return new WaitForEndOfFrame();
		}

		if (intoPictureTime <= _archive_GetOut_RequireTime)
		{
			Archives pArchives = ArchivementManager.Instance.FindArchive(_archive_GetOut_Name);
			pArchives.Archive.OnArchiveCount(1);
		}

        //-----
        BossRG_Picture pCurPicture = pHall.GetCurPicture();

        pHall.StopPictureProc();
        pHall.SetPictureCanAttack(false);

        //-----
        AnimationClip pOutClip = m_pActorBase.m_pAnimations.GetClip(_intoPic_OUT);
		m_pActorBase.SetAction(_intoPic_OUT);

		//-----
		if (bIsNowFirstFloor)
			AddFloorBuff_2F();
		else
			AddFloorBuff_1F();

        //-----
        gameObject.transform.position = pCurPicture.transform.position;
        gameObject.transform.forward = Quaternion.Euler(Vector3.up * 180.0f) * pCurPicture.transform.right;

        GameObject pDestPivot = pHall.FindDummyPivot((_isIn1F)? _intoPic_DestPivot_1F : _intoPic_DestPivot_2F);
		float fLeastTime = 1.0f; //pOutClip.length;

		Vector3 vCurPos = gameObject.transform.position;
        Vector3 vCurForward = gameObject.transform.forward;
		while(fLeastTime > 0.0f)
		{
			fLeastTime -= HT.TimeUtils.GameTime;
			float fRatio = 1.0f - (fLeastTime / pOutClip.length);

			gameObject.transform.position = Vector3.Lerp(vCurPos, pDestPivot.transform.position, fRatio);
            m_pActorBase.m_vViewVector = Vector3.Lerp(vCurForward, pDestPivot.transform.right, fRatio);

            m_pActorBase.m_pRigidBody.velocity = Vector3.zero;
            m_pActorBase.m_pRigidBody.angularVelocity = Vector3.zero;

            yield return new WaitForEndOfFrame();
        }

        //-----
        ActorSkill pPoltergeist = m_pActorBase.FindSkillInfo(_intoPic_PoltergeistSkill);
        pPoltergeist.SetSkillCooling(pPoltergeist.m_fSkillCoolTime);

		_stairProc = null;
	}

	//---------------------------------------
	public override void Extended_EventCallback(AIActor.eActorEventCallback eEvent, GameObject pParam)
	{
		if (eEvent == AIActor.eActorEventCallback.eDamaged)
		{
			if (m_pActorBase.FindEnabledActorBuff(_organ_PlayBuff) != null)
			{
				Archives pArchives = ArchivementManager.Instance.FindArchive(_archive_NoDisturbance_Name);
				pArchives.Archive.OnArchiveCount(1);
			}
		}
	}
}


/////////////////////////////////////////
//---------------------------------------