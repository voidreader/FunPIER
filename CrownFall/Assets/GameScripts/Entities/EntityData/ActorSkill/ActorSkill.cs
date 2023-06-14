using UnityEngine;
using System.Collections;
using HT;


/////////////////////////////////////////
//---------------------------------------
public class ActorSkill : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	public enum eAreaAlertType
	{
		eNotAlert = 0,
		eCasterPos,
		eTargetPos,
		eDummyPos,
		eDirection,
	}
	[Header("BASE SKILL INFO")]
	public eAreaAlertType m_eAreaAlertType = eAreaAlertType.eCasterPos;
	public eAlertRingType _ringType = eAlertRingType.Angle360;

	public float _dirAlert_XScale = 0.0f;
	public float _dirAlert_YScale = 0.0f;
	public string _castDummyName = null;

	public bool m_bIsEnemySkill = true;
	public string m_szSkillName;
	public float m_fSkillCoolTime;
	public float m_fSkillCoolTime_Extend_Mobile = 0.0f;
	public bool m_bSkillPrewarmed = true;
	public float _skillPrewarmRatio = 1.0f;

	[Header("SKILL CAST CONDITION")]
	public ActorBuff[] m_vRequireBuff;
	public bool _requireBuffRemove = false;
	public ActorBuff[] m_vRestrictBuffs = null;

	public float m_fSkillMinRange = -1.0f;
	public float m_fSkillMaxRange = 999.0f;
	public float m_fSkillRealRange = 1.0f;

	//---------------------------------------
	[Header("CAST INFO")]
	public string m_szCastAnimation;
	public AudioClip _castSound = null;
	public bool m_bNoSkillCastDelay;
	public float m_fCastCamShake;

	public bool _updateDirectionInCast = false;

	//---------------------------------------
	[Header("THROW INFO")]
	public bool _skillThrowBySystem = false;
	public string m_szThrowAnimation;
	public AudioClip _throwSound = null;
	public float m_fThrowCamShake;
	public ActorBuff m_pBuffWhenThrow;
	public string _throwOffsetDummy = null;
	public Object_Follower _dummyFollowerWhenCast = null;
	public string _dummyFollowerRefDummyName = null;

	public float m_fBuffTimeOverwrap = 0.0f;

	//---------------------------------------
	[Header("THROW WHEN TARGET")]
	public ActorBuff[] _targetBuffWhenThrow = null;

	//---------------------------------------
	protected IActorBase m_pCaster;
	protected IActorBase m_pTarget;
	protected Vector3 m_vTargetPosition;

	//---------------------------------------
	protected float m_fSkillCooling;
	protected float m_fCastReadyTime;

	[Header("THROW EFFECT")]
	public ParticleSystem _throwEffect = null;
	public bool _throwEffect_OnDummy = false;

	//---------------------------------------
	[Header("ARCHIVE SETTINGS")]
	public string _effectiveAcv_whenCast = null;
	public string _effectiveAcv_whenHit = null;
	public string _effectiveAcv_whenHit2 = null;
	public string _effectiveAcv_whenPostProc = null;

	//---------------------------------------
	[Header("CHILD SKILL INFOS")]
	public bool m_bCastYet = false;

	//---------------------------------------
	public ISkillObject m_pPostSkillObject;
	public bool _postSkillObject_UsePool = false;


	/////////////////////////////////////////
	//---------------------------------------
	public virtual void ResetAll()
	{

	}


	/////////////////////////////////////////
	//---------------------------------------
	public bool SkillCast(IActorBase pCaster, IActorBase pTarget)
	{
		m_bCastYet = false;
		m_pCaster = pCaster;
		m_pTarget = pTarget;

		//-----
		if (IsCastable() == false)
			return false;

		//-----
		if (m_pTarget != null)
			m_vTargetPosition = m_pTarget.transform.position;

		if (SkillCastReady_Child())
		{
			pCaster.m_pRigidBody.velocity = Vector3.zero;
			pCaster.m_pRigidBody.angularVelocity = Vector3.zero;
			pCaster.m_vMoveVector = Vector3.zero;

			//-----
			if (_requireBuffRemove)
			{
				for (int nInd = 0; nInd < m_vRequireBuff.Length; ++nInd)
					m_pCaster.RemoveActorBuff(m_vRequireBuff[nInd], true);
			}

			m_fSkillCooling = m_fSkillCoolTime + m_fSkillCoolTime_Extend_Mobile;

			m_fCastReadyTime = 0.0f;
			CalculateCastingTime();

			//-----
			HT.HTSoundManager.PlaySound(_castSound);

			if (m_fCastCamShake > float.Epsilon)
				CameraManager._Instance.SetCameraShake(m_fCastCamShake);
			
			//-----
			if (_skillThrowBySystem == false)
			{
				float fCastReadyTime = m_fCastReadyTime - 0.01f;
				if (m_bNoSkillCastDelay == false && fCastReadyTime > float.Epsilon)
					Invoke("SkillThrow_Invoke", fCastReadyTime);
				else
					SkillThrow();
				
				//-----
				if (m_eAreaAlertType != eAreaAlertType.eNotAlert)
				{
					bool bCreateDefaultAlert = true;
					Vector3 vAlertPos = Vector3.zero;
					switch (m_eAreaAlertType)
					{
						case eAreaAlertType.eCasterPos:
							vAlertPos = m_pCaster.transform.position;
							break;

						case eAreaAlertType.eTargetPos:
							vAlertPos = m_vTargetPosition;
							break;

						case eAreaAlertType.eDummyPos:
							vAlertPos = m_pCaster.FindDummyPivot(_castDummyName).transform.position;
							break;

						case eAreaAlertType.eDirection:
							bCreateDefaultAlert = false;
							vAlertPos = m_pCaster.transform.position;
							break;
					}

					if (bCreateDefaultAlert)
					{
						Object_AreaAlert pAlert = BattleFramework._Instance.CreateAreaAlert(_ringType, vAlertPos, m_fSkillRealRange, fCastReadyTime + GetCastDelayTime());
						pAlert.transform.forward = m_pCaster.m_vViewVector;
					}
					else
					{
						Object_AreaAlert pAlert = BattleFramework._Instance.CreateAreaAlert_Simple(vAlertPos, _dirAlert_XScale, _dirAlert_YScale, fCastReadyTime + GetCastDelayTime());

						Object_Follower pFollower = pAlert.GetComponent<Object_Follower>();
						if (pFollower != null)
							pFollower._targetObject = m_pCaster.gameObject;
					}
				}
			}

			return true;
		}

		//-----
		return false;
	}

	public virtual bool SkillCastReady_Child()
	{
		return true;
	}

	//---------------------------------------
	void SkillThrow_Invoke()
	{
		SkillThrow(true);
	}

	public void SkillThrow(bool bCallByInvoke = false)
	{
		if (m_bCastYet)
			return;

		if (m_pCaster.m_bActorIncapacitation || m_pCaster.GetCurrHP() <= 0 || (BattleFramework._Instance != null && BattleFramework._Instance.m_eBattleState != BattleFramework.eBattleState.eBattle))
			return;

		m_bCastYet = true;
		if (bCallByInvoke == false)
			CancelInvoke("SkillThrow_Invoke");

		//-----
		m_fSkillCooling = m_fSkillCoolTime + m_fSkillCoolTime_Extend_Mobile;

		HT.HTSoundManager.PlaySound(_throwSound);

		if (m_szThrowAnimation != null && m_szThrowAnimation.Length > 0)
			m_pCaster.SetAction(m_szThrowAnimation);

		if (m_fThrowCamShake > float.Epsilon)
			CameraManager._Instance.SetCameraShake(m_fThrowCamShake);

		if (m_pBuffWhenThrow != null)
			m_pCaster.AddActorBuff(m_pBuffWhenThrow, m_fBuffTimeOverwrap);

		if (_dummyFollowerWhenCast != null)
		{
			Object_Follower pFollwer = HT.Utils.Instantiate(_dummyFollowerWhenCast);

			pFollwer._owner = m_pCaster;
			pFollwer._targetObject = m_pCaster.FindDummyPivot(_dummyFollowerRefDummyName);
		}

		if (_throwEffect != null)
		{
			ParticleSystem pEffect = HT.Utils.InstantiateFromPool(_throwEffect);
			Transform pTransform = m_pCaster.transform;

			if (_throwEffect_OnDummy)
				pTransform = m_pCaster.FindDummyPivot(_throwOffsetDummy).transform;

			pEffect.transform.position = pTransform.position;
			pEffect.transform.rotation = pTransform.rotation;

			HT.Utils.SafeDestroy(pEffect.gameObject, pEffect.TotalSimulationTime());
		}

		//-----
		if (string.IsNullOrEmpty(_effectiveAcv_whenCast) == false)
		{
			Archives pArchives = ArchivementManager.Instance.FindArchive(_effectiveAcv_whenCast);
			pArchives.Archive.OnArchiveCount(1);
		}

		SkillThrow_Child();
	}

	public virtual void SkillThrow_Child()
	{
	}

	//---------------------------------------
	public void CallSkillObject_Throw()
	{
		if (m_pPostSkillObject != null)
		{
			ISkillObject pSkillObj = null;
			if (_postSkillObject_UsePool)
				pSkillObj = HT.Utils.InstantiateFromPool(m_pPostSkillObject);
			else
				pSkillObj = HT.Utils.Instantiate(m_pPostSkillObject);

			//-----
			Vector3 vPosition = Vector3.zero;
			Quaternion qRotation = Quaternion.identity;
			if (m_eAreaAlertType != eAreaAlertType.eDummyPos)
			{
				vPosition = m_pCaster.transform.position;
				qRotation = m_pCaster.transform.rotation;
			}
			else if (string.IsNullOrEmpty(_castDummyName) == false)
			{
				GameObject pTargetObj = m_pCaster.FindDummyPivot(_castDummyName);
				vPosition = pTargetObj.transform.position;
				qRotation = pTargetObj.transform.rotation;
			}

			//-----
			pSkillObj.transform.position = vPosition;

			if (pSkillObj.m_bInheritRotation)
				pSkillObj.transform.rotation = qRotation;

			//-----
			pSkillObj.m_pCaster = m_pCaster;
		}

		if (_targetBuffWhenThrow != null && _targetBuffWhenThrow.Length > 0)
		{
			for (int nInd = 0; nInd < _targetBuffWhenThrow.Length; ++nInd)
				m_pTarget.AddActorBuff(_targetBuffWhenThrow[nInd]);
		}

		//-----
		if (string.IsNullOrEmpty(_effectiveAcv_whenPostProc) == false)
		{
			Archives pArchives = ArchivementManager.Instance.FindArchive(_effectiveAcv_whenPostProc);
			pArchives.Archive.OnArchiveCount(1);
		}
	}

	/////////////////////////////////////////
	//---------------------------------------
	public void Frame()
	{
		if (m_fSkillCooling > 0.0f)
			m_fSkillCooling -= HT.TimeUtils.GameTime;
		
		Frame_Child();
	}

	protected virtual void Frame_Child()
	{
	}

	//---------------------------------------
	public bool IsCastable(IActorBase pCaster = null)
	{
		if (m_fSkillCooling > 0.0f)
			return false;

		if (pCaster == null)
			pCaster = m_pCaster;

		for (int nInd = 0; nInd < m_vRequireBuff.Length; ++nInd)
		{
			IActorBase.ActorBuffEnable pBuff = pCaster.FindEnabledActorBuff(m_vRequireBuff[nInd]);
			if (pBuff == null)
				return false;
		}

		for (int nInd = 0; nInd < m_vRestrictBuffs.Length; ++nInd)
		{
			IActorBase.ActorBuffEnable pBuff = pCaster.FindEnabledActorBuff(m_vRestrictBuffs[nInd]);
			if (pBuff != null)
				return false;
		}

		return true;
	}


	/////////////////////////////////////////
	//---------------------------------------
	public float CalculateCastingTime()
	{
		if (m_fCastReadyTime <= 0.0f)
		{
			if (m_szCastAnimation != null && m_szCastAnimation.Length > 0)
			{
				m_fCastReadyTime = m_pCaster.SetAction(m_szCastAnimation);
			}
			else
			{
				m_fCastReadyTime = 0.0f;
			}
		}

		return m_fCastReadyTime;
	}

	public virtual float GetCastDelayTime()
	{
		return 0.0f;
	}


	/////////////////////////////////////////
	//---------------------------------------
	public float GetSkillCooling()
	{
		return m_fSkillCooling;
	}

	public void SetSkillCooling(float fSet)
	{
		m_fSkillCooling = fSet;
	}


	/////////////////////////////////////////
	//---------------------------------------
	public Vector3 GetAlertTargetPos()
	{
		switch(m_eAreaAlertType)
		{
			case eAreaAlertType.eCasterPos:
				return m_pCaster.transform.position;

			case eAreaAlertType.eDummyPos:
				return m_pCaster.FindDummyPivot(_castDummyName).transform.position;
		}

		return Vector3.zero;
	}
}
