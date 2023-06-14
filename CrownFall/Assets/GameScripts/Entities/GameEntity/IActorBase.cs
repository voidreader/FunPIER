using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HT;

/////////////////////////////////////////
//---------------------------------------
public class IActorBase : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	[Header("ACTOR BASE")]
	public ActorInfo m_pActorInfo;
	//public ActorSkill[] m_vActorSkill;
	public ActorSkillContainer[] m_vActorSkillCont;

	//---------------------------------------
	public enum eActorType
	{
		eBaseActor = 0,
		eControllable,
		eAI,
		eSpawn,
	}

	public virtual eActorType GetActorType()
	{
		return eActorType.eBaseActor;
	}

	//---------------------------------------
	public float m_fActorHeight;

	//---------------------------------------
	public class ActorBuffEnable
	{
		public ActorBuff pBuff;
		public float fLeastTime;
		public int nStackCount;
	}
	public List<ActorBuffEnable> m_vActorBuffList = new List<ActorBuffEnable>();
	private List<ActorBuff> _buffTimeAdded = new List<ActorBuff>();

	//---------------------------------------
	public bool m_bManagedBySystem;
	public bool m_bActorIncapacitation = false;

	//---------------------------------------
	bool m_bIsEssential = false;
	float m_fEssentialTime = 0.0f;

	//---------------------------------------
	public string m_szIDLEAnimName = "IDLE";
	public string m_szMOVEAnimName = "MOVE";
	public string m_szDEATHAnimName = "DEATH";

	[Header("ACTOR BASE - HELPERS")]
	[SerializeField]
	private DummyPivot[] _dummyPivots = null;


	/////////////////////////////////////////
	//---------------------------------------
	[Header("ACTOR BASE - INFO")]
	public Rigidbody m_pRigidBody;
	public Animation m_pAnimations;

	public enum eActorState
	{
		eIdle = 0,
		eMove,
		eAction,
	}
	protected eActorState m_eActorState = eActorState.eIdle;
    public bool _actorRotateEnable = true;

	//---------------------------------------
	public Vector3 m_vMoveVector;
	public Vector3 m_vViewVector;

	protected float m_fActionReadyTime;

	//---------------------------------------
	Quaternion _baseQuarternion;
	Vector3 _baseRotation;

	public Quaternion BaseQuarternion { get { return _baseQuarternion; } }
	public Vector3 BaseRotation { get { return _baseRotation; } }


	/////////////////////////////////////////
	//---------------------------------------
	private void Awake()
	{
	}

	public virtual void Init()
	{
		if (m_pActorInfo != null)
		{
			m_pActorInfo.m_cnMaxHP.val = m_pActorInfo.m_nCalculatedHP;
			m_pActorInfo.m_cnNowHP.val = m_pActorInfo.m_nCalculatedHP;

			if (m_pActorInfo.m_fCalculatedMoveSpeed <= 0.0f)
				m_pActorInfo.m_fCalculatedMoveSpeed = m_pActorInfo.m_fBaseMoveSpeed;
		}

		//-----
		CapsuleCollider pCapsule = GetComponent<CapsuleCollider>();
		if (pCapsule != null)
			m_fActorHeight = pCapsule.height;

		//-----
		m_bManagedBySystem = false;
		m_bActorIncapacitation = false;

		//-----
		if (m_pRigidBody == null)
			m_pRigidBody = GetComponent<Rigidbody>();

		if (m_pAnimations == null)
			m_pAnimations = GetComponent<Animation>();

		_baseQuarternion = transform.rotation;
		_baseRotation = _baseQuarternion.eulerAngles;

		//-----
		m_vViewVector = transform.forward;

		//-----
		m_vActorBuffList.Clear();

		for (int nInd = 0; nInd < m_vActorSkillCont.Length; ++nInd)
		{
			//m_vActorSkillCont[nInd] = Instantiate(m_vActorSkillCont[nInd]);
			m_vActorSkillCont[nInd].Init();
		}
	}

	//---------------------------------------
	public virtual void Frame()
	{
		//----- Death State
		if (GetCurrHP() <= 0)
		{
			m_bActorIncapacitation = true;
			m_vMoveVector = Vector3.zero;

			m_eActorState = eActorState.eAction;
			m_pAnimations.Play(m_szDEATHAnimName);
		}

		//----- Add force for move
		if (m_vMoveVector.sqrMagnitude > float.Epsilon)
		{
			m_vViewVector = m_vMoveVector;
			m_vViewVector.Normalize();

			//-----
			if (m_bManagedBySystem == false)
			{
				Vector3 vMoveVec_Norm = m_vMoveVector;
				vMoveVec_Norm.Normalize();

				float fTotalMoveSpeed = CalculateActorBuffEffect(ActorBuff.eBuffType.eSpeed_Percentage);
				fTotalMoveSpeed = GetMoveSpeed() * fTotalMoveSpeed;

				vMoveVec_Norm *= fTotalMoveSpeed;

				vMoveVec_Norm.x = Mathf.Abs(m_vMoveVector.x) * vMoveVec_Norm.x;
				vMoveVec_Norm.z = Mathf.Abs(m_vMoveVector.z) * vMoveVec_Norm.z;
				//vMoveVec_Norm.x = m_vMoveVector.magnitude * vMoveVec_Norm.x;
				//vMoveVec_Norm.z = m_vMoveVector.magnitude * vMoveVec_Norm.z;
				//vMoveVec_Norm.x = Mathf.Lerp (Mathf.Abs(m_vMoveVector.x), m_vMoveVector.magnitude, 0.5f) * vMoveVec_Norm.x;
				//vMoveVec_Norm.z = Mathf.Lerp (Mathf.Abs(m_vMoveVector.z), m_vMoveVector.magnitude, 0.5f) * vMoveVec_Norm.z;

				// Velocity가 이미 프레임 단위로 이동 속도를 제공하기 때문에,
				// 프레임이 떨어지면 Velocity가 60프레임 일 때 보다 높아져 비정상적으로 빠르게 움직이게 된다.
				// 그러므로 GameTime을 곱하는게 아닌, 60프레임에서 맞춰놓은 속도로 움직이도록 1/60을 곱해준다.
				vMoveVec_Norm *= HT.TimeUtils.frame60DeltaTime;
				vMoveVec_Norm.y = m_pRigidBody.velocity.y;

				//-----
				m_pRigidBody.velocity = vMoveVec_Norm;
			}
		}

		//----- Look at view vector
		UpdateViewVector();

		//----- Actor state update
		if (m_bActorIncapacitation)
			return;

		//-----
		if (m_fActionReadyTime >= 0.0f)
			m_fActionReadyTime -= HT.TimeUtils.GameTime;

		//-----
		if (m_eActorState == eActorState.eIdle && m_vMoveVector.magnitude > 0.0f)
			m_eActorState = eActorState.eMove;

		else if (m_eActorState == eActorState.eMove && m_vMoveVector.sqrMagnitude < float.Epsilon)
			m_eActorState = eActorState.eIdle;

		else if (m_eActorState == eActorState.eAction && m_fActionReadyTime <= 0.0f)
			m_eActorState = eActorState.eIdle;

		//----- Play base animation
		switch (m_eActorState)
		{
			case eActorState.eIdle:
				m_pAnimations.Play(m_szIDLEAnimName);
				break;

			case eActorState.eMove:
				m_pAnimations.Play(m_szMOVEAnimName);
				break;

			case eActorState.eAction:
				break;
		}

		//-----
		for (int nInd = 0; nInd < m_vActorSkillCont.Length; ++nInd)
			m_vActorSkillCont[nInd].GetSkillByLevel().Frame();

		for (int nInd = m_vActorBuffList.Count - 1; nInd >= 0; --nInd)
		{
			ActorBuffEnable pCurBuff = m_vActorBuffList[nInd];

			float fPrevTime = pCurBuff.fLeastTime;
			if (fPrevTime >= pCurBuff.pBuff.m_fBuffTime && pCurBuff.pBuff.m_eBuffType == ActorBuff.eBuffType.eDamage_TimeMax)
			{
				OnBuffDamaged(pCurBuff.pBuff);
				
				RemoveActorBuff(nInd, false);
				if (pCurBuff.pBuff._buffRemoveWhenTimeEnd == false)
					AddActorBuff(pCurBuff.pBuff);
			}

			float fNextTime = pCurBuff.fLeastTime - HT.TimeUtils.GameTime;
			if (pCurBuff.pBuff.m_eBuffType == ActorBuff.eBuffType.eSpawnObject)
			{
				if (pCurBuff.pBuff._spawnObjectType == ActorBuff.eSpawnObjectType.eWhenTime)
				{
					int nPrevRate = (int)((pCurBuff.pBuff.m_fBuffTime - fPrevTime) / pCurBuff.pBuff._spawnObjectTime);
					int nNextRate = (int)((pCurBuff.pBuff.m_fBuffTime - fNextTime) / pCurBuff.pBuff._spawnObjectTime);
					if (nNextRate > nPrevRate)
						pCurBuff.pBuff.SpawnObject(this);
				}
			}

			pCurBuff.fLeastTime = fNextTime;
			if (pCurBuff.fLeastTime < 0.0f)
			{
				switch (m_vActorBuffList[nInd].pBuff.m_eBuffType)
				{
					case ActorBuff.eBuffType.eSpawnObject:
						if (pCurBuff.pBuff._spawnObjectType == ActorBuff.eSpawnObjectType.eWhenEnd)
							pCurBuff.pBuff.SpawnObject(this);

						break;

					case ActorBuff.eBuffType.eDamage_TimeEnd:
						OnBuffDamaged(pCurBuff.pBuff);
						break;
				}

				//-----
				if (pCurBuff.pBuff._endRelativeBuff != null)
				{
					if (pCurBuff.pBuff._endRelativeType == ActorBuff.eEndRelativeType.TimeEnd)
						AddActorBuff(pCurBuff.pBuff._endRelativeBuff);
				}

				//-----
				RemoveActorBuff(nInd, false);
				if (pCurBuff.pBuff._buffRemoveWhenTimeEnd == false)
					AddActorBuff(pCurBuff.pBuff);
			}
		}

		//-----
		if (m_fEssentialTime > 0.0f)
			m_fEssentialTime -= HT.TimeUtils.GameTime;

		m_bIsEssential = ((m_fEssentialTime > 0.0f) ? true : false);

		//-----
		_buffTimeAdded.Clear();
	}

	//---------------------------------------
	public virtual void Release()
	{
		m_vActorBuffList.Clear();
	}

	//---------------------------------------
	public virtual void OnBattleStart()
	{
	}

	public virtual void OnBattleEnd(bool bPlayerWin)
	{

	}


	/////////////////////////////////////////
	//---------------------------------------
	public virtual int GetCurrHP()
	{
		return m_pActorInfo.m_cnNowHP.val;
	}

	public virtual int GetMaxHP()
	{
		return m_pActorInfo.m_cnMaxHP.val;
	}

	public virtual float GetMoveSpeed()
	{
		return m_pActorInfo.m_fCalculatedMoveSpeed;
	}

	//---------------------------------------
	public void UpdateViewVector()
	{
        if (_actorRotateEnable == false)
            return;

		if (m_vViewVector.sqrMagnitude > float.Epsilon)
		{
			Quaternion qQuat = transform.rotation;
			qQuat.SetLookRotation(m_vViewVector);

			Vector3 vEulerAngle = qQuat.eulerAngles;
			vEulerAngle -= _baseRotation;

			qQuat.eulerAngles = vEulerAngle;
			transform.rotation = qQuat;
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	public float SetAction(string szAniName)
	{
		m_eActorState = eActorState.eAction;

		m_pAnimations.Play(szAniName);

		AnimationClip pClip = m_pAnimations.GetClip(szAniName);
		m_fActionReadyTime = pClip.length;

		OnCallSetAction(pClip);

		return m_fActionReadyTime;
	}

	public void SetActionReadyTime(float fTime)
	{
		m_fActionReadyTime = fTime;
	}

	public virtual void OnCallSetAction(AnimationClip pAnimClip)
	{
		return;
	}

	public void SetActorState(eActorState eState)
	{
		m_eActorState = eState;
	}

	public eActorState GetActorState()
	{
		return m_eActorState;
	}

	//---------------------------------------
	public ActorSkill FindSkillInfo(string szName)
	{
		for (int nInd = 0; nInd < m_vActorSkillCont.Length; ++nInd)
		{
			if (m_vActorSkillCont[nInd].GetSkillByLevel().m_szSkillName == szName)
				return m_vActorSkillCont[nInd].GetSkillByLevel();
		}

		return null;
	}

	//---------------------------------------
	public void AddActorBuff(ActorBuff pBuff, float fBuffTime = 0.0f)
	{
        if (pBuff == null || m_vActorBuffList == null || GetCurrHP() <= 0)
            return;

        //-----
		float fBuffLeastTime = ((fBuffTime > 0.0f) ? fBuffTime : pBuff.m_fBuffTime);

		if (pBuff.m_eBuffType == ActorBuff.eBuffType.eSpawnObject)
		{
			if (pBuff._spawnObjectType == ActorBuff.eSpawnObjectType.eWhenStart)
			{
				pBuff.SpawnObject(this);
			}
		}

		ActorBuffEnable pBuffFind = FindEnabledActorBuff(pBuff.m_szBuffName);
		if (pBuffFind != null)
		{
			if (pBuff._isStackable && pBuffFind.nStackCount < pBuff._maxStackCount)
				++pBuffFind.nStackCount;

			if (pBuffFind.pBuff._refreshTimeWhenAddSameBuff && pBuffFind.fLeastTime < fBuffLeastTime)
				pBuffFind.fLeastTime = fBuffLeastTime;
		}
		else
		{
			ActorBuffEnable pNewBuff = new ActorBuffEnable();

			pNewBuff.pBuff = pBuff;
			pNewBuff.fLeastTime = fBuffLeastTime;
			pNewBuff.nStackCount = 1;

			pNewBuff.pBuff.Enable(this, fBuffLeastTime);
			m_vActorBuffList.Add(pNewBuff);

			pBuffFind = pNewBuff;
		}

		if (pBuff.m_eBuffType == ActorBuff.eBuffType.eStun)
		{
			m_pAnimations.Stop();
			SetControlEnable(false);
		}

		if (string.IsNullOrEmpty(pBuff.m_SzEffectiveArchiveName) != false)
		{
			bool bAddArchiveCount = false;
			if (pBuff.m_eEffectArchiveType == ActorBuff.eBuffEffectiveArchive.eWhenGetBuff)
				bAddArchiveCount = true;

			if (pBuff.m_eEffectArchiveType == ActorBuff.eBuffEffectiveArchive.eStackCount && pBuffFind.nStackCount == pBuff._eEffectArchiveStackCount)
				bAddArchiveCount = true;

			if (bAddArchiveCount)
			{
				Archives pArchive = ArchivementManager.Instance.FindArchive(pBuff.m_SzEffectiveArchiveName);
				pArchive.Archive.OnArchiveCount(1);
			}
		}
	}

	//---------------------------------------
	public bool AddEnableActorBuffTime(ActorBuff pBuff, float fTime, bool bButOnceOnFrame)
	{
		if (bButOnceOnFrame && _buffTimeAdded.Contains(pBuff))
			return false;

		ActorBuffEnable pEnableBuff = FindEnabledActorBuff(pBuff);
		if (pEnableBuff == null)
			return false;

		_buffTimeAdded.Add(pBuff);

		pEnableBuff.fLeastTime += fTime;
		pEnableBuff.pBuff.Enable(this, pEnableBuff.fLeastTime);

		return true;
	}

	//---------------------------------------
	public ActorBuffEnable FindEnabledActorBuff(string szName)
	{
        if (m_vActorBuffList == null)
            return null;

		for (int nInd = 0; nInd < m_vActorBuffList.Count; ++nInd)
		{
			if (m_vActorBuffList[nInd].pBuff.m_szBuffName == szName)
				return m_vActorBuffList[nInd];
		}

		return null;
	}

	public ActorBuffEnable FindEnabledActorBuff(ActorBuff pBuff)
	{
		if (m_vActorBuffList == null)
			return null;

		for (int nInd = 0; nInd < m_vActorBuffList.Count; ++nInd)
		{
			if (m_vActorBuffList[nInd].pBuff == pBuff)
				return m_vActorBuffList[nInd];
		}

		return null;
	}

	public bool FindEnabledActorBuff(ActorBuff.eBuffType eType)
	{
		if (m_vActorBuffList == null)
			return false;

		for (int nInd = 0; nInd < m_vActorBuffList.Count; ++nInd)
			if (m_vActorBuffList[nInd].pBuff.m_eBuffType == eType)
				return true;

		return false;
	}

	//---------------------------------------
	public void RemoveActorBuff(string szName, bool bEndEffectEnable)
    {
        if (m_vActorBuffList == null)
            return;

        for (int nInd = 0; nInd < m_vActorBuffList.Count; ++nInd)
		{
			if (m_vActorBuffList[nInd].pBuff.m_szBuffName == szName)
			{
				RemoveActorBuff(nInd, bEndEffectEnable);
				return;
			}
		}
	}

	public void RemoveActorBuff(ActorBuff pBuff, bool bEndEffectEnable)
    {
        if (m_vActorBuffList == null)
            return;

        for (int nInd = 0; nInd < m_vActorBuffList.Count; ++nInd)
		{
			if (m_vActorBuffList[nInd].pBuff == pBuff)
			{
				RemoveActorBuff(nInd, bEndEffectEnable);
				return;
			}
		}
	}

	public void RemoveActorBuff(int nInd, bool bEndEffectEnable)
	{
		ActorBuffEnable pCurBuff = m_vActorBuffList[nInd];
		if (bEndEffectEnable)
		{
			switch (pCurBuff.pBuff.m_eBuffType)
			{
				case ActorBuff.eBuffType.eDamage_TimeEnd:
					ActorBuff pBuff = pCurBuff.pBuff;
					OnBuffDamaged(pBuff);
					break;
			}
		}

		if (pCurBuff.pBuff._endRelativeBuff != null)
		{
			bool bOnEnable = false;
			switch (pCurBuff.pBuff._endRelativeType)
			{
				case ActorBuff.eEndRelativeType.EndEffectable:
					if (bEndEffectEnable)
						bOnEnable = true;

					break;
			}

			if (bOnEnable)
				AddActorBuff(pCurBuff.pBuff._endRelativeBuff);
		}

		pCurBuff.pBuff.Disabled();
		m_vActorBuffList.RemoveAt(nInd);
		
		if (pCurBuff.pBuff.m_eBuffType == ActorBuff.eBuffType.eStun)
		{
			if (FindEnabledActorBuff(ActorBuff.eBuffType.eStun) == false)
				SetControlEnable(true);
		}
	}

	private void OnBuffDamaged(ActorBuff pBuff)
	{
		int nDamage = (int)pBuff.fBuffValue;
		if (nDamage > 0)
			OnDamaged(nDamage);

		if (string.IsNullOrEmpty(pBuff.m_SzEffectiveArchiveName) == false && pBuff.m_eEffectArchiveType == ActorBuff.eBuffEffectiveArchive.eWhenDamage)
		{
			Archives pArchive = ArchivementManager.Instance.FindArchive(pBuff.m_SzEffectiveArchiveName);
			pArchive.Archive.OnArchiveCount(1);
		}
	}

	//---------------------------------------
	public float CalculateActorBuffEffect(ActorBuff.eBuffType eType)
	{
		float fRetVal = ((eType == ActorBuff.eBuffType.eSpeed_Percentage) ? 1.0f : 0.0f);

		for (int nInd = 0; nInd < m_vActorBuffList.Count; ++nInd)
		{
			if (m_vActorBuffList[nInd].pBuff.m_eBuffType == eType)
			{
				float fBuffValue = m_vActorBuffList[nInd].pBuff.fBuffValue;

				if (m_vActorBuffList[nInd].pBuff._isStackable)
					fBuffValue *= m_vActorBuffList[nInd].nStackCount;

				fRetVal += fBuffValue;
			}

			if (m_vActorBuffList[nInd].pBuff.m_eBuffType == ActorBuff.eBuffType.eFrenzy)
			{
				if (eType == ActorBuff.eBuffType.eDamage_Increase)
					fRetVal += m_vActorBuffList[nInd].pBuff.fBuffValue;
			}
		}

		return fRetVal;
	}


	/////////////////////////////////////////
	//---------------------------------------
	public bool CastSkill(string szSkillName)
	{
		ActorSkill pSkill = FindSkillInfo(szSkillName);

		if (pSkill != null)
			return CastSkill(pSkill);

		return false;
	}

	public virtual bool CastSkill(ActorSkill pSkill)
	{
		return false;
	}

	//---------------------------------------
	public void CreateActorBasedParticle(ParticleSystem pParticle)
	{
		ParticleSystem pEffect = HT.Utils.InstantiateFromPool(pParticle);
		pEffect.gameObject.transform.position = gameObject.transform.position;
		pEffect.gameObject.transform.rotation = gameObject.transform.rotation * pEffect.gameObject.transform.rotation;

		HT.Utils.SafeDestroy(pEffect.gameObject, pEffect.TotalSimulationTime());
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void SetEssentialTime(float fSet)
	{
		m_fEssentialTime = fSet;

		if (m_fEssentialTime > 0.0f)
			m_bIsEssential = true;
	}

	public virtual bool IsEssential()
	{
		return m_bIsEssential;
	}

	//---------------------------------------
	public void OnDamaged(int nDamage)
	{
		int nCalDamage = nDamage;
		if (FindEnabledActorBuff(ActorBuff.eBuffType.eFrenzy))
			nCalDamage *= 2;

		OnDamage_Calculated(nCalDamage);
	}

	protected virtual void OnDamage_Calculated(int nDamage)
	{

	}

	public virtual void SetControlEnable(bool bSet)
	{

	}


	/////////////////////////////////////////
	//---------------------------------------
	public void Animation_PlayMisc(AudioClip pClip)
	{
		if (pClip != null)
			HTSoundManager.PlaySound(pClip);
	}

	public void Animation_CreateParticle(GameObject pParticleObj)
	{
		if (pParticleObj != null)
		{
			ParticleSystem pParticle = pParticleObj.GetComponent<ParticleSystem>();
			CreateActorBasedParticle(pParticle);
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	public GameObject FindDummyPivot(string szName)
	{
		if (_dummyPivots == null)
			return null;

		for (int nInd = 0; nInd < _dummyPivots.Length; ++nInd)
			if (_dummyPivots[nInd].name == szName)
				return _dummyPivots[nInd].gameObject;

		return null;
	}
}
