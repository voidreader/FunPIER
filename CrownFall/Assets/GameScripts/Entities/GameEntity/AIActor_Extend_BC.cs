using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HT;


/////////////////////////////////////////
//---------------------------------------
[Serializable]
public struct EndSequenceBossSpawnInfo
{
	public bool bUseOnlyOneSkill;
	public float fPostDelayTime;
}


/////////////////////////////////////////
//---------------------------------------
public sealed class AIActor_Extend_BC : AIActor_Extend
{
	//---------------------------------------
	[Header("COPIED ACTORS")]
	[SerializeField]
	private BossBC_CopiedActor[] _copiedActors = null;
	[SerializeField]
	private float _copiedBoss_Cooltime = 25.0f;
	[SerializeField]
	private string _copiedBoss_CastAnim = null;
	[SerializeField]
	private string _copiedBoss_SpawnAnim = null;
	[SerializeField]
	private string _copiedBoss_EndAnim = null;
	[SerializeField]
	private string _copiedBoss_RespawnAnim = null;
	[SerializeField]
	private GameObject _copiedBoss_FloorObj = null;
	[SerializeField]
	private float _copiedBoss_FloowObj_Speed = 5.0f;
	[SerializeField]
	private ParticleSystem _copiedBoss_Effect = null;

	private float _copiedBoss_LeastTime = 0.0f;
	private Coroutine _copyBossProc = null;

	[Header("COPIED ACTORS - ARCHIVE")]
	[SerializeField]
	private string _copiedActor_Acv_Damage = null;

	//---------------------------------------
	[Header("FRAGMENTS")]
	[SerializeField]
	private Projectile_Parabola _fragments_Projectile = null;
	[SerializeField]
	private int _fragments_Count = 5;
	[SerializeField]
	private float _fragments_Speed = 4.0f;
	[SerializeField]
	private float _fragments_Distance_Min = 3.0f;
	[SerializeField]
	private float _fragments_Distance_Max = 5.0f;
	[SerializeField]
	private float _fragments_Height_Min = 3.0f;
	[SerializeField]
	private float _fragments_Height_Max = 5.0f;
	[SerializeField]
	private GameObject _fragments_Mass = null;
	[SerializeField]
	private Collider _fragments_Mass_Root = null;

	[Header("FRAGMENTS - ARCHIVEMENT")]
	[SerializeField]
	private string _fragments_Acv_Kill = null;

	private bool _fragmentsState_Proc = false;

	//---------------------------------------
	[Header("LURK")]
	[SerializeField]
	private float[] _lurk_RepeatTime = null;
	[SerializeField]
	private BossBC_Lurk[] _lurk_LurkObject = null;
	[SerializeField]
	private int _lurk_Count = 5;
	[SerializeField]
	private float _lurk_PostDelay = 1.5f;
	[SerializeField]
	private AudioClip _lurk_Sound = null;

	private float _lurk_LeastTime = 20.0f;
	private Coroutine _lurkProc = null;

	//---------------------------------------
	[Header("HELL OF THORN")]
	[SerializeField]
	private float _hot_RepeatTime = 15.0f;
	[SerializeField]
	private float _hot_During = 7.0f;
	[SerializeField]
	private int _hot_Thorn_Count = 2;
	[SerializeField]
	private float _hot_Thorn_Time = 0.1f;
	[SerializeField]
	private ChasingAreaDamage[] _hot_Projectile = null;
	[SerializeField]
	private AudioClip _hot_Sound = null;

	private float _hot_LeastTime = 20.0f;
	private Coroutine _hotProc = null;

	//---------------------------------------
	[Header("THORN RING")]
	[SerializeField]
	private float[] _ring_RepeatTime = null;
	[SerializeField]
	private float _ring_CastRange_Min = 3.0f;
	[SerializeField]
	private float _ring_CastRange_Max = 8.0f;
	[SerializeField]
	private ParticleSystem _ring_CastEffect = null;
	[SerializeField]
	private float _ring_CastTime = 1.0f;
	[SerializeField]
	private BossBC_ThornRing[] _ring_Instances = null;
	[SerializeField]
	private float[] _ring_StartRange = null;
	[SerializeField]
	private float[] _ring_EndRange = null;
	[SerializeField]
	private float[] _ring_BetweenRange = null;
	[SerializeField]
	private float[] _ring_ExpandRangeWhenBack = null;
	[SerializeField]
	private float[] _ring_ExpandTerm = null;
	[SerializeField]
	private float _ring_EndDelay = 1.0f;

	private float _ring_LeastTime = 0.0f;
	private Coroutine _ringProc = null;

	//---------------------------------------
	[Header("BLACK ARROW")]
	[SerializeField]
	private float[] _blackArrow_RepeatTime = null;
	[SerializeField]
	private Transform _blackArrow_Parent = null;
	[SerializeField]
	private BossBC_BlackArrow[] _blackArrow_Instance = null;
	[SerializeField]
	private float _blackArrow_FloatingDistance = 2.0f;
	[SerializeField]
	private float _blackArrow_FloatingHeight = 1.0f;
	[SerializeField]
	private int[] _blackArrow_ProjectileCount = null;
	[SerializeField]
	private float _blackArrow_FirstMoveTime = 2.0f;
	[SerializeField]
	private float[] _blackArrow_FirstDelay = null;
	[SerializeField]
	private float[] _blackArrow_ShootTerm = null;

	private float _blackArrow_LeastTime = 0.0f;
	private Coroutine _blackArrowProc = null;

	private List<BossBC_BlackArrow> _createdArrows = new List<BossBC_BlackArrow>();

	//---------------------------------------
	[Header("END GAME SEQUENCE")]
	[SerializeField]
	private float _endGameSeq_HPRatio = 0.4f;
	[SerializeField]
	private EndSequenceBossSpawnInfo[] _endGameSeq_SpawnInfo = null;
	[SerializeField]
	private AudioClip _endGameSeq_EndGameBgm = null;

	[Header("END GAME SEQUENCE - SEQ 1")]
	[SerializeField]
	private string _endGameSeq_CamSeq_1 = null;
	[SerializeField]
	private float _endGameSeq_CamSeq_1_Time = 2.0f;
	[SerializeField]
	private float _endGameSeq_CamSeq_1_Wait = 2.0f;

	[Header("END GAME SEQUENCE - SEQ 2")]
	[SerializeField]
	private string _endGameSeq_FieldAnimName = null;

	[Header("END GAME SEQUENCE - SEQ 3")]
	[SerializeField]
	private float _endGameSeq_HealthLeastTime = 40.0f;

	private Coroutine _endGameSeq = null;

	//---------------------------------------
	private Vector3 _lastCopiedBossPosition = Vector3.zero;

	//---------------------------------------
	public override void Extended_Init()
	{
		_copiedBoss_LeastTime = _copiedBoss_Cooltime;

		int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;
		_hot_LeastTime = _hot_RepeatTime;
		_lurk_LeastTime = _lurk_RepeatTime[nDiff];
		_ring_LeastTime = _ring_RepeatTime[nDiff];
		_blackArrow_LeastTime = _blackArrow_RepeatTime[nDiff];

		for (int nInd = 0; nInd < _copiedActors.Length; ++nInd)
			_copiedActors[nInd].gameObject.SetActive(false);
	}

	public override void Extended_Frame()
	{
		float fDeltaTime = HT.TimeUtils.GameTime;
		int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;

		//-----
		_copiedBoss_LeastTime -= fDeltaTime;
		do
		{
			if (CheckAnyProcessing(_copiedBoss_LeastTime))
				break;

			if (_blackArrowProc != null)
				break;

			_copyBossProc = StartCoroutine(CopyBoss_Internal());
		}
		while (false);

		//-----
		if (_copyBossProc == null)
			_lurk_LeastTime -= fDeltaTime;

		do
		{
			if (CheckAnyProcessing(_lurk_LeastTime))
				break;

			_lurk_LeastTime = _lurk_RepeatTime[nDiff];
			_lurkProc = StartCoroutine(Lurk_Internal());
		}
		while (false);

		//-----
		if (_copyBossProc == null)
			_hot_LeastTime -= fDeltaTime;

		do
		{
			if (CheckAnyProcessing(_hot_LeastTime))
				break;

			_hot_LeastTime = _hot_RepeatTime;
			_hotProc = StartCoroutine(HellOfThorn_Internal());
		}
		while (false);

		//-----
		if (_copyBossProc == null)
			_ring_LeastTime -= fDeltaTime;

		do
		{
			if (CheckAnyProcessing(_ring_LeastTime))
				break;

			float fDistance = Vector3.Distance(gameObject.transform.position, BattleFramework._Instance.m_pPlayerActor.transform.position);
			if (fDistance < _ring_CastRange_Min || fDistance > _ring_CastRange_Max)
				break;

			_ring_LeastTime = _ring_RepeatTime[nDiff];
			_ringProc = StartCoroutine(ThornRing_Internal());
		}
		while (false);

		//------
		if (_copyBossProc == null && _blackArrowProc == null)
			_blackArrow_LeastTime -= fDeltaTime;

		do
		{
			//if (CheckAnyProcessing(_blackArrow_LeastTime))
			//	break;

			if (_blackArrow_LeastTime >= 0.0f)
				break;

			if (_copyBossProc != null || _blackArrowProc != null)
				break;

			_blackArrow_LeastTime = _blackArrow_RepeatTime[nDiff];
			_blackArrowProc = StartCoroutine(BlackArrow_Internal());
		}
		while (false);

		////-----
		//float nCurHP = m_pActorBase.GetCurrHP();
		//float nMaxHP = m_pActorBase.GetMaxHP();
		//if (_endGameSeq == null && (/*(float)*/nCurHP / nMaxHP <= _endGameSeq_HPRatio))
		//	_endGameSeq = StartCoroutine(EndGameSeq_Internal());
	}

	public override void Extended_Release()
	{
		StopAllCoroutines();
	}

	//---------------------------------------
	private bool CheckAnyProcessing(float fSkillCoolTime)
	{
		if (_endGameSeq != null || _copyBossProc != null || _lurkProc != null || _hotProc != null || _ringProc != null)
			return true;

		if (m_pActorBase.GetActorState() == IActorBase.eActorState.eAction)
			return true;

		if (fSkillCoolTime > 0.0f)
			return true;

		return false;
	}

	//---------------------------------------
	private IEnumerator CopyBoss_Internal()
	{
		int nAcvRecord_PrevHP = m_pActorBase.GetCurrHP();

		//-----
		_copiedBoss_FloorObj.transform.position = gameObject.transform.position;

		yield return new WaitForEndOfFrame();

		float fAnimTime = m_pActorBase.SetAction(_copiedBoss_CastAnim);
		m_pActorBase.SetActionReadyTime(float.PositiveInfinity);
		
		yield return new WaitForSeconds(fAnimTime);

		//-----
		//Vector3 vPlayerPos = BattleFramework._Instance.m_pPlayerActor.transform.position;
		//Vector3 vTeleportPosition = vPlayerPos - gameObject.transform.position;
		//vTeleportPosition.Normalize();
		//
		//float fDistance = Vector3.Distance(vPlayerPos, gameObject.transform.position);
		//vTeleportPosition = gameObject.transform.position + (vTeleportPosition * (fDistance - 2.0f));

		//-----
		IActorBase pPlayer = BattleFramework._Instance.m_pPlayerActor;

		Vector3 vTeleportPosition = gameObject.transform.position;
		while(true)
		{
			Vector3 vPlayerPos = pPlayer.transform.position;
			Vector3 vCurFloorPos = _copiedBoss_FloorObj.transform.position;
			if (Vector3.Distance(vPlayerPos, vCurFloorPos) < 2.0f)
			{
				vTeleportPosition = vCurFloorPos;
				break;
			}

			Vector3 vDir = (vPlayerPos - vCurFloorPos).normalized;
			vCurFloorPos += vDir * (_copiedBoss_FloowObj_Speed * HT.TimeUtils.GameTime);

			_copiedBoss_FloorObj.transform.position = vCurFloorPos;

			yield return new WaitForEndOfFrame();
		}

		//-----
		BossBC_CopiedActor pCurBoss = HT.RandomUtils.Array(_copiedActors);

		pCurBoss.gameObject.SetActive(true);
		pCurBoss.CopyActor_Init(false);
		
		gameObject.transform.position = vTeleportPosition;
		_copiedBoss_FloorObj.transform.position = vTeleportPosition;
		pCurBoss.transform.position = vTeleportPosition;

		//-----
		m_pActorBase.SetAction(_copiedBoss_SpawnAnim);
		m_pActorBase.SetActionReadyTime(float.PositiveInfinity);

		yield return new WaitForSeconds(fAnimTime);

		//-----
		ParticleSystem pParticle = HT.Utils.InstantiateFromPool(_copiedBoss_Effect);
		pParticle.transform.position = gameObject.transform.position;
		
		HT.Utils.SafeDestroy(pParticle.gameObject, pParticle.TotalSimulationTime());

		//-----
		_lastCopiedBossPosition = pCurBoss.transform.position;
		while (true)
		{
			if (pCurBoss.CopyActor_Frame() == false)
				break;

			_lastCopiedBossPosition = pCurBoss.transform.position;

			yield return new WaitForEndOfFrame();
		}

		_lastCopiedBossPosition = pCurBoss.transform.position;

		if (pCurBoss.PostDelay > 0.0f)
			yield return new WaitForSeconds(pCurBoss.PostDelay);

		yield return new WaitForSeconds(1.0f);

		pCurBoss.CopyActor_Release();

		//-----
		Vector3 vCurBossPos = pCurBoss.transform.position;
		gameObject.transform.position = vCurBossPos;
		pCurBoss.transform.position = vCurBossPos;

		pParticle = HT.Utils.InstantiateFromPool(_copiedBoss_Effect);
		pParticle.transform.position = vCurBossPos;

		HT.Utils.SafeDestroy(pParticle.gameObject, pParticle.TotalSimulationTime());

		//-----
		m_pActorBase.SetAction(_copiedBoss_EndAnim);
		m_pActorBase.SetActionReadyTime(float.PositiveInfinity);

		//yield return new WaitForEndOfFrame();
		yield return new WaitForSeconds(0.1f);

		//-----
		_copiedBoss_FloorObj.transform.position = gameObject.transform.position;
		_fragmentsState_Proc = true;

		Ray pRay = new Ray();
		pRay.origin = pCurBoss.transform.position;

		for (int nInd = 0; nInd < _fragments_Count; ++nInd)
		{
			Projectile_Parabola pProj = HT.Utils.Instantiate(_fragments_Projectile);
			pProj.m_pSkill_Proj = null;
			pProj.m_pParent = m_pActorBase;

			//-----
			pProj._damage = 0;
			pProj._speed = _fragments_Speed;
			pProj._flyHeight = HT.RandomUtils.Range(_fragments_Height_Min, _fragments_Height_Max);

			Vector3 vDestPos = Vector3.zero;
			vDestPos.x = HT.RandomUtils.Range(-1.0f, 1.0f);
			vDestPos.z = HT.RandomUtils.Range(-1.0f, 1.0f);
			vDestPos.Normalize();

			float fFragDistance = HT.RandomUtils.Range(_fragments_Distance_Min, _fragments_Distance_Max);

			pRay.direction = vDestPos;
			RaycastHit pHitInfo;
			if (Physics.Raycast(pRay, out pHitInfo, fFragDistance))
				fFragDistance = pHitInfo.distance - 0.5f;

			vDestPos = pCurBoss.transform.position + (vDestPos * fFragDistance);

			//-----
			pProj.Init(pCurBoss.transform.position, vDestPos);
		}

		Vector3 vFragMassPos = pCurBoss.transform.position;
		SpawnActor_Extend_BlackFrags.FragCongregatePos = vFragMassPos;

		//----
		yield return new WaitForSeconds(0.5f);

		//----
		Vector3 vLerpStartScale = new Vector3(1.0f, 1.0f, 0.5f);
		Vector3 vTargetMassScale = vLerpStartScale;

		Vector3 vMassPos = vFragMassPos;
		vMassPos.y = -0.01f;

		_fragments_Mass_Root.gameObject.SetActive(true);
		//_fragments_Mass_Root.enabled = false;

		_fragments_Mass_Root.transform.position = vMassPos;

		_fragments_Mass.transform.localScale = vLerpStartScale;

		float fWaitMax = 5.0f;
		while (SpawnActor_Extend_BlackFrags.CreatedFragments.Count < _fragments_Count)
		{
			fWaitMax -= HT.TimeUtils.GameTime;
			if (fWaitMax < 0.0f)
				break;
			
			yield return new WaitForEndOfFrame();
		}

		int nLastCompleteCount = 0;

		const float fMassLerpTime = 0.5f;
		float fMassLerpLeastTime = 0.0f;

        float fWaitMaxTime = 3.0f;
		List<SpawnActor_Extend_BlackFrags> vList = SpawnActor_Extend_BlackFrags.CreatedFragments;
		while (true)
		{
            int nCompleteCount = 0;
			bool bAllCompleted = true;
			for (int nInd = 0; nInd < vList.Count; ++nInd)
			{
				if (vList[nInd].MoveCompleted == false)
					bAllCompleted = false;
				else
					++nCompleteCount;
			}

			//if (nCompleteCount > 0)
			//	_fragments_Mass_Root.enabled = true;

			if (nLastCompleteCount != nCompleteCount)
			{
				nLastCompleteCount = nCompleteCount;
				fMassLerpLeastTime = fMassLerpTime;

				vLerpStartScale = _fragments_Mass.transform.localScale;

				//float fXYScale = Mathf.Min(1.0f, nCompleteCount / (vList.Count * 0.5f));
				//vTargetMassScale = new Vector3(fXYScale, fXYScale, (1.0f / vList.Count) * nCompleteCount);

				vTargetMassScale = new Vector3(1.0f, 1.0f, 0.25f + (((1.0f / vList.Count) * nCompleteCount) * 0.75f));
			}

			if (fMassLerpLeastTime > 0.0f)
			{
				fMassLerpLeastTime -= HT.TimeUtils.GameTime;
				if (fMassLerpLeastTime < 0.0f)
					fMassLerpLeastTime = 0.0f;
				
				_fragments_Mass.transform.localScale = Vector3.Lerp(vLerpStartScale, vTargetMassScale, 1.0f - (fMassLerpLeastTime * 2.0f));
			}

			if (bAllCompleted && fMassLerpLeastTime <= 0.0f)
				break;
			
			_fragments_Mass_Root.transform.position = vMassPos;

            //-----
            fWaitMaxTime -= HT.TimeUtils.GameTime;
            if (fWaitMaxTime <= 0.0f)
                break;

            yield return new WaitForEndOfFrame();
		}

		for (int nInd = vList.Count - 1; nInd >= 0; --nInd)
		{
			if (nInd < 0)
				break;

            if (vList[nInd] == null)
            {
                HT.HTDebug.PrintLog(eMessageType.Warning, string.Format("[ActorExtend_BC] Frag {0} was null!", nInd));
                continue;
            }

			HT.Utils.SafeDestroy(vList[nInd].gameObject);
		}

		vList.Clear ();

		//-----
		yield return new WaitForEndOfFrame();

		//-----
		gameObject.transform.position = vFragMassPos;

		vMassPos = vFragMassPos;
		vMassPos.y = -0.01f;
		_fragments_Mass_Root.transform.position = vMassPos;

		fAnimTime = m_pActorBase.SetAction(_copiedBoss_RespawnAnim);
		m_pActorBase.SetActionReadyTime(float.PositiveInfinity);

		pCurBoss.gameObject.SetActive(false);

		yield return new WaitForSeconds(fAnimTime);
		
		//-----
		_copiedBoss_LeastTime = _copiedBoss_Cooltime;
		
		//-----
		m_pActorBase.SetAction(m_pActorBase.m_szIDLEAnimName);
		m_pActorBase.SetActionReadyTime(0.0f);

		m_pActorBase.SetActorState(IActorBase.eActorState.eIdle);

		//-----
		_fragmentsState_Proc = false;

		int nAcvRecord_CurHP = m_pActorBase.GetCurrHP();
		if (nAcvRecord_PrevHP > nAcvRecord_CurHP)
		{
			Archives pArchive = ArchivementManager.Instance.FindArchive(_copiedActor_Acv_Damage);
			pArchive.Archive.OnArchiveCount(nAcvRecord_PrevHP - nAcvRecord_CurHP);
		}

		//-----
		_copyBossProc = null;
	}


	/////////////////////////////////////////
	//---------------------------------------
	private IEnumerator Lurk_Internal()
	{
		m_pActorBase.SetActorState(IActorBase.eActorState.eAction);
		m_pActorBase.SetActionReadyTime(float.PositiveInfinity);
		int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;

		//-----
		BossBC_Lurk pLastObj = null;
		//float fLeastTime = _lurk_During;
		//while(fLeastTime > 0.0f)

		float fWaitForAlert = 0.5f;
		Vector3[] vCreatedDirections = new Vector3[_lurk_Count];

		for(int nInd = 0; nInd < _lurk_Count; ++nInd)
		{
			Vector3 vDir = HT.RandomUtils.GetVector3(true, false, true);
			vDir.y = 0.0f;
			vDir.Normalize();

			Object_AreaAlert pAlert = BattleFramework._Instance.CreateAreaAlert_Simple(gameObject.transform.position, 0.5f, 100.0f, fWaitForAlert);
			pAlert.transform.forward = vDir;

			vCreatedDirections[nInd] = vDir;
		}

		yield return new WaitForSeconds(fWaitForAlert);

		if (_lurk_Sound != null)
			HT.HTSoundManager.PlaySound(_lurk_Sound);

		for (int nInd = 0; nInd < _lurk_Count; ++nInd)
		{
			//Vector3 vDir = HT.RandomUtils.Vector3(true, false, true);
			//vDir.y = 0.0f;
			//vDir.Normalize();
			//
			//float fWaitForAlert = 0.5f;
			//Object_AreaAlert pAlert = BattleFramework._Instance.CreateAreaAlert_Simple(gameObject.transform.position, 0.5f, 100.0f, fWaitForAlert);
			//pAlert.transform.forward = vDir;

			//yield return new WaitForSeconds(fWaitForAlert);
			//fLeastTime -= fWaitForAlert;

			//-----
			pLastObj = HT.Utils.InstantiateFromPool(_lurk_LurkObject[nDiff]);
			
			pLastObj.transform.forward = vCreatedDirections[nInd];
			pLastObj.transform.position = gameObject.transform.position;

			pLastObj.Init();

			////-----
			//float fReadyForNextCreate = _lurk_ThornCreateTime;
			//while (fReadyForNextCreate > 0.0f)
			//{
			//	fReadyForNextCreate -= HT.TimeUtils.GameTime;
			//
			//	fLeastTime -= HT.TimeUtils.GameTime;
			//	if (fLeastTime < 0.0f)
			//		break;
			//
			//	yield return new WaitForEndOfFrame();
			//}
		}
		
		//-----
		yield return new WaitForSeconds(_lurk_PostDelay);
		//pLastObj.SetComplete(true);

		m_pActorBase.SetActorState(IActorBase.eActorState.eIdle);
		m_pActorBase.SetActionReadyTime(0.0f);
		
		//-----
		_lurkProc = null;
		//_lurk_LeastTime = _lurk_RepeatTime;
	}


	/////////////////////////////////////////
	//---------------------------------------
	private IEnumerator HellOfThorn_Internal()
	{
		m_pActorBase.SetActorState(IActorBase.eActorState.eAction);
		m_pActorBase.SetActionReadyTime(float.PositiveInfinity);
		int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;

		//-----
		float fSoundDelay = -1.0f;
		if (_hot_Sound != null)
		{
			fSoundDelay = _hot_Sound.length;
			HT.HTSoundManager.PlaySound(_hot_Sound);
		}

		float fDuring = _hot_During;
		float fThornTime = 0.0f;
		while(fDuring > 0.0f)
		{
			float fDelta = HT.TimeUtils.GameTime;

			//-----
			fThornTime -= fDelta;
			if (fThornTime < 0.0f)
			{
				fThornTime = _hot_Thorn_Time;

				for(int nInd = 0; nInd < _hot_Thorn_Count; ++nInd)
				{
					ChasingAreaDamage pObject = HT.Utils.InstantiateFromPool(_hot_Projectile[nDiff]);
					Vector3 vDir = new Vector3(HT.RandomUtils.Range(-1.0f, 1.0f), 0.0f, HT.RandomUtils.Range(-1.0f, 1.0f)).normalized;
					float fDistance = HT.RandomUtils.Range(0.0f, 20.0f);

					pObject.transform.position = vDir * fDistance;
					pObject.Init();
				}
			}

			//-----
			if (fSoundDelay >= 0.0f)
			{
				fSoundDelay -= fDelta;

				if (fSoundDelay <= 0.0f)
				{
					fSoundDelay = _hot_Sound.length;
					HT.HTSoundManager.PlaySound(_hot_Sound);
				}
			}

			//-----
			fDuring -= fDelta;
			yield return new WaitForEndOfFrame();
		}

		//-----
		m_pActorBase.SetActorState(IActorBase.eActorState.eIdle);
		m_pActorBase.SetActionReadyTime(0.0f);

		//-----
		_hotProc = null;
	}


	/////////////////////////////////////////
	//---------------------------------------
	private IEnumerator ThornRing_Internal()
	{
		m_pActorBase.SetActorState(IActorBase.eActorState.eAction);
		m_pActorBase.SetActionReadyTime(float.PositiveInfinity);
		int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;

		//-----
		ParticleSystem pCastEffect = HT.Utils.InstantiateFromPool (_ring_CastEffect);
		pCastEffect.transform.position = gameObject.transform.position;

		HT.Utils.SafeDestroy (pCastEffect.gameObject, pCastEffect.TotalSimulationTime ());

		yield return new WaitForSeconds (_ring_CastTime);

		//-----
		bool bExpand = true;
		float fCurRange = _ring_StartRange[nDiff];

		while(true)
		{
			BossBC_ThornRing pRing = HT.Utils.InstantiateFromPool(_ring_Instances[nDiff]);
			pRing.transform.position = gameObject.transform.position;
			pRing.Init(fCurRange);

			//-----
			if (bExpand)
			{
				fCurRange += _ring_BetweenRange[nDiff];
				if (_ring_EndRange[nDiff] <= fCurRange)
				{
					bExpand = false;
					fCurRange += _ring_ExpandRangeWhenBack[nDiff];
				}
			}
			else
			{
				fCurRange -= _ring_BetweenRange[nDiff];
				if (_ring_StartRange[nDiff] > fCurRange)
					break;
			}

			//-----
			yield return new WaitForSeconds(_ring_ExpandTerm[nDiff]);
		}

		//-----
		yield return new WaitForSeconds(_ring_EndDelay);

		//-----
		m_pActorBase.SetActorState(IActorBase.eActorState.eIdle);
		m_pActorBase.SetActionReadyTime(0.0f);

		//-----
		_ringProc = null;
	}


	/////////////////////////////////////////
	//---------------------------------------
	private IEnumerator BlackArrow_Internal()
	{
		int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;
		
		//-----
		BossBC_BlackArrow pInstance = _blackArrow_Instance[nDiff];

		int nProjCount = _blackArrow_ProjectileCount[nDiff];
		for(int nInd = 0; nInd < nProjCount; ++nInd)
		{
			BossBC_BlackArrow pArrow = HT.Utils.Instantiate(pInstance);
			pArrow.transform.position = gameObject.transform.position + (Vector3.up * _blackArrow_FloatingHeight);

			pArrow.transform.SetParent(_blackArrow_Parent);
			_createdArrows.Add(pArrow);
		}

		//-----
		float fTime = _blackArrow_FirstMoveTime;
		while (fTime > 0.0f)
		{
			fTime -= HT.TimeUtils.GameTime;

			float fDistance = (1.0f - (fTime / _blackArrow_FirstMoveTime)) * _blackArrow_FloatingDistance;
			float fRotTerm = 360.0f / _createdArrows.Count;
			for(int nInd = 0; nInd < _createdArrows.Count; ++nInd)
			{
				Vector3 vDir = Quaternion.Euler(0.0f, fRotTerm * nInd, 0.0f) * Vector3.forward;
				Vector3 vLocalPos = new Vector3(0.0f, _blackArrow_FloatingHeight, 0.0f) + (vDir * fDistance);

				_createdArrows[nInd].transform.localPosition = vLocalPos;
			}

			yield return new WaitForEndOfFrame();
		}

		//-----
		yield return new WaitForSeconds(_blackArrow_FirstDelay[nDiff]);

		//-----
		while(true)
		{
			if (_createdArrows.Count == 0)
				break;

			BossBC_BlackArrow pCurArrow = _createdArrows[_createdArrows.Count - 1];
			pCurArrow.transform.SetParent(null);

			pCurArrow.CastSkill();

			_createdArrows.Remove(pCurArrow);

			yield return new WaitForSeconds(_blackArrow_ShootTerm[nDiff]);
		}

		//-----
		_blackArrowProc = null;
	}


	/////////////////////////////////////////
	//---------------------------------------
	private IEnumerator EndGameSeq_Internal()
	{
		IActorBase pPlayer = BattleFramework._Instance.m_pPlayerActor;
		Field pField = BattleFramework._Instance.m_pField;

		HT.HTGameAudioMisc pBGM = HT.HTSoundManager.PlayMusic(_endGameSeq_EndGameBgm);
		pBGM.AudioSource.loop = false;
		
		yield return new WaitForEndOfFrame();
		
		//-----
		float fAnimTime = m_pActorBase.SetAction(_copiedBoss_CastAnim);
		m_pActorBase.SetActionReadyTime(float.PositiveInfinity);
		
		yield return new WaitForSeconds(fAnimTime);
		
		//-----
		for (int nInd = 0; nInd < _copiedActors.Length; ++nInd)
		{
			EndSequenceBossSpawnInfo pInfo = _endGameSeq_SpawnInfo[nInd];
		
			BossBC_CopiedActor pCurBoss = _copiedActors[nInd];
		
			pCurBoss.gameObject.SetActive(true);
			pCurBoss.CopyActor_Init(pInfo.bUseOnlyOneSkill);
		
			//-----
			Vector3 vPlayerPos = pPlayer.transform.position;
			Vector3 vTeleportPosition = vPlayerPos - gameObject.transform.position;
			vTeleportPosition.Normalize();
		
			float fDistance = Vector3.Distance(vPlayerPos, gameObject.transform.position);
			vTeleportPosition = gameObject.transform.position + (vTeleportPosition * (fDistance - 3.0f));
		
			gameObject.transform.position = vTeleportPosition;
			pCurBoss.transform.position = vTeleportPosition;
		
			//-----
			m_pActorBase.SetAction(_copiedBoss_SpawnAnim);
			m_pActorBase.SetActionReadyTime(float.PositiveInfinity);
		
			yield return new WaitForSeconds(fAnimTime);
		
			//-----
			ParticleSystem pParticle = HT.Utils.InstantiateFromPool(_copiedBoss_Effect);
			pParticle.transform.position = gameObject.transform.position;
		
			HT.Utils.SafeDestroy(pParticle.gameObject, pParticle.TotalSimulationTime());
		
			//-----
			while (true)
			{
				if (pCurBoss.CopyActor_Frame() == false)
					break;
		
				yield return new WaitForEndOfFrame();
			}
		
			//----
			if (pInfo.fPostDelayTime > 0.0f)
				yield return new WaitForSeconds(pInfo.fPostDelayTime);
		
			//-----
			pParticle = HT.Utils.InstantiateFromPool(_copiedBoss_Effect);
			pParticle.transform.position = pCurBoss.gameObject.transform.position;
		
			HT.Utils.SafeDestroy(pParticle.gameObject, pParticle.TotalSimulationTime());
		
			//-----
			float fEndAnimTime = m_pActorBase.SetAction(_copiedBoss_EndAnim);
			m_pActorBase.SetActionReadyTime(float.PositiveInfinity);
		
			yield return new WaitForSeconds(fEndAnimTime);
		
			pCurBoss.gameObject.SetActive(false);
		}
		
		//-----
		fAnimTime = m_pActorBase.SetAction(_copiedBoss_RespawnAnim);
		m_pActorBase.SetActionReadyTime(float.PositiveInfinity);
		
		yield return new WaitForSeconds(fAnimTime);

		//-----
		{
			BattleFramework._Instance.GamePaused = true;
			pPlayer.SetAction(pPlayer.m_szIDLEAnimName);

			GameObject pFocusTarget = CameraManager._Instance.m_pFocusedEntity;
			CameraManager._Instance.m_pFocusedEntity = null;

			Camera pCurCam = CameraManager._Instance.m_pMainCamera;
			float fFOV = pCurCam.fieldOfView;

			CameraManager._Instance.m_pMainCamera = null;

			//-----
			//CanvasGroup pGameUI = BattleFramework._Instance._gameOnlyObjects;

			Transform pCamSeqTrans = pField.FindDummyPivot(_endGameSeq_CamSeq_1).transform;
			Vector3 vStartPos = pCurCam.transform.position;
			Quaternion vStartQut = pCurCam.transform.rotation;
			//float fStartFov = fFOV;

			Vector3 vOriginalPos = vStartPos;
			Quaternion vOriginalQut = vStartQut;

			float fLeastTime = 0.0f;
			while (fLeastTime < _endGameSeq_CamSeq_1_Time)
			{
				float fLerpRatio = fLeastTime / _endGameSeq_CamSeq_1_Time;

				//pGameUI.alpha = 1.0f - fLerpRatio;

				pCurCam.transform.position = Vector3.Lerp(vStartPos, pCamSeqTrans.position, fLerpRatio);
				pCurCam.transform.rotation = Quaternion.Lerp(vStartQut, pCamSeqTrans.rotation, fLerpRatio);
				//pCurCam.fieldOfView = Mathf.Lerp(fStartFov, 45.0f, fLerpRatio);

				fLeastTime += HT.TimeUtils.GameTime;
				
				yield return new WaitForEndOfFrame();
			}

			yield return new WaitForSeconds(_endGameSeq_CamSeq_1_Wait);

			//-----
			pField._fieldAnimator.Play(_endGameSeq_FieldAnimName);
			AnimationClip pFieldAnimClip = pField._fieldAnimator.GetClip(_endGameSeq_FieldAnimName);

			yield return new WaitForSeconds(pFieldAnimClip.length);

			//-----
			BossBC_EffectCanvas._instance.OnWhiteMask(0.5f, 0.5f);

			yield return new WaitForSeconds(0.5f);

			pCurCam.transform.position = vOriginalPos;
			pCurCam.transform.rotation = vOriginalQut;

			CameraManager._Instance.m_pMainCamera = pCurCam;
			CameraManager._Instance.m_pFocusedEntity = pFocusTarget;
			pCurCam.fieldOfView = fFOV;

			//-----
			BattleFramework._Instance.GamePaused = false;
			m_pActorBase.SetActionReadyTime(1.0f);

			//-----
			float fDefaultHP = m_pActorBase.GetMaxHP() * _endGameSeq_HPRatio;
			float fTime = 0.0f;
			while(fTime < _endGameSeq_HealthLeastTime)
			{
				fTime += HT.TimeUtils.GameTime;

				float fRatio = fTime / _endGameSeq_HealthLeastTime;
				float fCurHP = Mathf.Lerp(fDefaultHP, 0, fRatio);
				BossBC_InvincibleHP._instance._hpSlider.value = 1.0f - fRatio;

				m_pActorBase.m_pActorInfo.m_cnNowHP.val = (int)fCurHP;

				yield return new WaitForEndOfFrame();
			}
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	public override bool Extended_IsDamageEnable()
	{
        //return (_copyBossProc == null && _endGameSeq == null) ? true : false;
        return true;
	}

	public override void Extended_EventCallback(AIActor.eActorEventCallback eEvent, GameObject pParam)
	{
		if (eEvent == AIActor.eActorEventCallback.eDamaged)
		{
			if (m_pActorBase.GetCurrHP() <= 0)
			{
				if (_fragmentsState_Proc)
				{
					Archives pArchives = ArchivementManager.Instance.FindArchive(_fragments_Acv_Kill);
					pArchives.Archive.OnArchiveCount(1);
				}

				if (_copyBossProc != null)
					gameObject.transform.position = _lastCopiedBossPosition;

				StopAllCoroutines();
			}
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	public override void OnBattleStart()
	{
		base.OnBattleStart();

		//----
		for (int nInd = 0; nInd < _copiedActors.Length; ++nInd)
			_copiedActors[nInd].OnBattleStart();
	}

	public override void OnBattleEnd(bool bPlayerWin)
	{
		base.OnBattleEnd(bPlayerWin);

		//----
		for (int nInd = 0; nInd < _copiedActors.Length; ++nInd)
			_copiedActors[nInd].OnBattleEnd(bPlayerWin);
	}
}


/////////////////////////////////////////
//---------------------------------------