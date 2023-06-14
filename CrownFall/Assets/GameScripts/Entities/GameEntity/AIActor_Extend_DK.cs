using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/////////////////////////////////////////
//---------------------------------------
public sealed class AIActor_Extend_DK : AIActor_Extend
{
	/////////////////////////////////////////
	//---------------------------------------
	[Header("MAIN SETTINGS")]
	public GameObject m_pSwordBindDummy;
	BossDK_SwordScript m_pSword;

	//---------------------------------------
	[Header("LIGHT OFF")]
	public ActorBuff[] m_pLightOffDmgBuff = null;
	public float[] _lightOffDmgRange = null;
	static readonly string m_szLightOffSkillName = "LIGHT_OFF";
	static readonly string m_szLightOffDmgBuffName = "LIGHT_OFF_DMG";
	static readonly string m_szLightOnAnimName = "LightOnAnim";
	Object_AreaAlert m_pLightOffAreaAlert = null;

	public ParticleSystem m_pLightOff_DrainEffect;
	public float m_fLightOff_DrainEffect_Speed;
	public float m_fLightOffDmgBuffRatio = 1.0f;
	float m_fLightOffDmgBuffTime = 0.0f;
	ParticleSystem[] m_vCreatedDrainEffect;
	bool m_bPrevLightOnState = true;

	//---------------------------------------
	[Header("SOUL RESIDUE")]
	public BossDK_SoulResidue m_pInst_SoulResidue;
	public float[] m_fCreateSoulResidueTime = null;
	public float m_fSoulResidueLifeTime;
	public float m_fSoulResidueLifeTime_PerLv;
	public int m_nSoulResidueDamage;
	public int m_nSoulDrainHealing;
	public int m_nSoulDrainHealing_PerLv;
	float m_fSoulResidueCoolTime;

	//---------------------------------------
	[Header("SOUL WALKER")]
	public BossDK_SoulWalker _soulWalker_Horz = null;
	public BossDK_SoulWalker _soulWalker_Vert = null;
	public eGameDifficulty _soulWalker_Diff = eGameDifficulty.eHard;
	public float _soulWalker_RepeatTime = 20.0f;
	public bool _soulWalker_LastIsHorz = false;

	private float _soulWalker_LeastTime = 0.0f;

	//---------------------------------------
	[Header("PHYSIC EXIT")]
	public ActorBuff _phyicExit_Buff = null;
	public float _physicExit_RequireTime = 5.0f;

	private List<Collider> _physicExit_CheckPhys = null;
	private bool _physicExit_IsInPhys = false;
	private float _physicExit_LeastTime = 0.0f;


	/////////////////////////////////////////
	//---------------------------------------
	public override void Extended_Init()
	{
		//-----
		m_vCreatedDrainEffect = new ParticleSystem[20];

		SetSoulResidueCoolTime();

		//-----
		_soulWalker_LastIsHorz = (Random.Range(0.0f, 1.0f) < 0.5f) ? true : false;
		_soulWalker_LeastTime = _soulWalker_RepeatTime;

		//-----
		_physicExit_IsInPhys = false;
		_physicExit_LeastTime = _physicExit_RequireTime;

		_physicExit_CheckPhys = new List<Collider>();
		DummyPivot[] vPivots = BattleFramework._Instance.m_pField.GetDummyPivots();
		for(int nInd = 0; nInd < vPivots.Length; ++nInd)
		{
			Collider pCollider = vPivots[nInd].GetComponent<Collider>();
			if (pCollider != null)
				_physicExit_CheckPhys.Add(pCollider);
		}
	}

	public override void Extended_Frame()
	{
		//-----
		if (m_pSword == null)
			m_pSword = BossDK_SwordScript._Instance;

		//-----
		IActorBase pPlayer = BattleFramework._Instance.m_pPlayerActor;

		//-----
		IActorBase.ActorBuffEnable pBuff_LightOff = m_pActorBase.FindEnabledActorBuff(m_szLightOffSkillName);
		IActorBase.ActorBuffEnable pBuff_LightOffDmg = pPlayer.FindEnabledActorBuff(m_szLightOffDmgBuffName);

		if (pBuff_LightOff != null)
		{
			if (m_bPrevLightOnState)
			{
				m_bPrevLightOnState = false;

				ParticleSystem.EmissionModule pEmission = m_pSword.m_pLantern.m_pSoulTrapEffect.emission;
				pEmission.enabled = true;
			}

			Vector3 vAreaPos = m_pSword.m_pLantern.gameObject.transform.position;
			vAreaPos.y = gameObject.transform.position.y;

			int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;
			float fLanternRangeHalf = _lightOffDmgRange[nDiff];

			if (m_pLightOffAreaAlert == null)
				m_pLightOffAreaAlert = BattleFramework._Instance.CreateAreaSafty(vAreaPos, fLanternRangeHalf, float.PositiveInfinity);

			else
				m_pLightOffAreaAlert.transform.position = vAreaPos;

			//-----
			if (Vector3.Distance(pPlayer.transform.position, vAreaPos) > (fLanternRangeHalf))
			{
				if (pBuff_LightOffDmg == null)
					pPlayer.AddActorBuff(m_pLightOffDmgBuff[nDiff], 0.0f);

			}
			else
			{
				pPlayer.RemoveActorBuff(m_szLightOffDmgBuffName, false);
			}

		}
		else
		{
			if (m_bPrevLightOnState == false)
			{
				m_bPrevLightOnState = true;

				ParticleSystem.EmissionModule pEmission = m_pSword.m_pLantern.m_pSoulTrapEffect.emission;
				pEmission.enabled = false;

				pPlayer.RemoveActorBuff(m_szLightOffDmgBuffName, false);

				//-----
				Field pField = BattleFramework._Instance.m_pField;
				LightManager[] vManageres = pField.m_pControllableDyanmicLights;
				for (int nInd = 0; nInd < vManageres.Length; ++nInd)
				{
					Animation pAnim = vManageres[nInd].GetComponent<Animation>();

					if (pAnim != null)
					{
						pAnim.Play(m_szLightOnAnimName);
					}
				}
			}

			//-----
			if (m_pLightOffAreaAlert != null)
			{
				HT.Utils.SafeDestroy(m_pLightOffAreaAlert.gameObject);
				m_pLightOffAreaAlert = null;
			}
		}

		if (pBuff_LightOffDmg == null)
			m_fLightOffDmgBuffTime = 0.0f;

		else
		{
			m_fLightOffDmgBuffTime -= HT.TimeUtils.GameTime;
			if (m_fLightOffDmgBuffTime <= 0.0f)
			{
				m_fLightOffDmgBuffTime = m_fLightOffDmgBuffRatio;

				for (int nInd = 0; nInd < m_vCreatedDrainEffect.Length; ++nInd)
				{
					if (m_vCreatedDrainEffect[nInd] == null)
					{
						ParticleSystem pNewDrain = HT.Utils.Instantiate(m_pLightOff_DrainEffect);
						pNewDrain.transform.position = pPlayer.transform.position;

						m_vCreatedDrainEffect[nInd] = pNewDrain;
						break;
					}
				}
			}
		}

		//-----
		BossDK_LanternScript pLantern = BossDK_SwordScript._Instance.m_pLantern;
		Vector3 vLanternPos = pLantern.transform.position;

		float fDrainEffectSpeed = m_fLightOff_DrainEffect_Speed * HT.TimeUtils.GameTime;
		for (int nInd = 0; nInd < m_vCreatedDrainEffect.Length; ++nInd)
		{

			ParticleSystem pDrain = m_vCreatedDrainEffect[nInd];
			if (pDrain == null)
				continue;

			Vector3 vDrainPos = pDrain.transform.position;
			if (Vector3.Distance(vDrainPos, vLanternPos) < fDrainEffectSpeed)
			{
				ParticleSystem.EmissionModule pEmittor = pDrain.emission;
				pEmittor.enabled = false;
				HT.Utils.SafeDestroy(pDrain.gameObject, pDrain.main.startLifetimeMultiplier);

				m_vCreatedDrainEffect[nInd] = null;

			}
			else
			{
				Vector3 vDir = vLanternPos - vDrainPos;
				vDir.Normalize();

				vDrainPos += vDir * fDrainEffectSpeed;
				pDrain.transform.position = vDrainPos;
			}
		}

		//-----
		m_fSoulResidueCoolTime -= HT.TimeUtils.GameTime;
		if (m_pActorBase.GetActorState() != IActorBase.eActorState.eAction)
		{
			if (m_fSoulResidueCoolTime < 0.0f)
			{
				//if (vLanternPos.y < 1.0f && vLanternPos.y >= 0.0f) {
				//m_fSoulResidueCoolTime = m_fCreateSoulResidueTime;
				SetSoulResidueCoolTime();

				BossDK_SoulResidue pResidue = HT.Utils.Instantiate(m_pInst_SoulResidue);

				pResidue.m_pParent = m_pActorBase;

				int nDifficulty = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;
				pResidue.m_fLifeTime = m_fSoulResidueLifeTime + (m_fSoulResidueLifeTime_PerLv * nDifficulty);
				pResidue.m_nDamage = m_nSoulResidueDamage;

				pResidue.m_nDrainHealing = m_nSoulDrainHealing + (m_nSoulDrainHealing_PerLv * nDifficulty);

				Vector3 vResiduePos = pPlayer.transform.position;//vLanternPos;
				vResiduePos.y = 0.0f;

				pResidue.transform.position = vResiduePos;
				//}
			}
		}

		//-----
		if (GameFramework._Instance.m_pPlayerData.m_eDifficulty >= _soulWalker_Diff)
		{
			if (_soulWalker_LeastTime > 0.0f)
				_soulWalker_LeastTime -= HT.TimeUtils.GameTime;

			else
			{
				_soulWalker_LeastTime = _soulWalker_RepeatTime;

				BossDK_SoulWalker pWalker = null;
				if (_soulWalker_LastIsHorz)
                {
                    pWalker = HT.Utils.InstantiateFromPool(_soulWalker_Vert);
                    if (Random.Range(0.0f, 1.0f) < 0.5f)
                        pWalker.transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                    else
                        pWalker.transform.rotation = Quaternion.Euler(0.0f, 270.0f, 0.0f);
                }
				else
                {
                    pWalker = HT.Utils.InstantiateFromPool(_soulWalker_Horz);
                    if (Random.Range(0.0f, 1.0f) < 0.5f)
                        pWalker.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    else
                        pWalker.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
                }

				pWalker.transform.position = Vector3.zero;
				_soulWalker_LastIsHorz = !_soulWalker_LastIsHorz;
			}
		}

		//-----
		if (_physicExit_IsInPhys)
		{
			_physicExit_LeastTime -= HT.TimeUtils.GameTime;
			if (_physicExit_LeastTime < 0.0f)
			{
				m_pActorBase.AddActorBuff(_phyicExit_Buff);
				_physicExit_IsInPhys = false;
			}
		}
	}

	public override void Extended_Release()
	{
		if (m_vCreatedDrainEffect != null)
		{
			for (int nInd = 0; nInd < m_vCreatedDrainEffect.Length; ++nInd)
				if (m_vCreatedDrainEffect[nInd] != null)
					HT.Utils.SafeDestroy(m_vCreatedDrainEffect[nInd].gameObject);
		}

		if (m_pLightOffAreaAlert != null)
		{
			HT.Utils.SafeDestroy(m_pLightOffAreaAlert.gameObject);
			m_pLightOffAreaAlert = null;
		}

		int nSoulCount = BossDK_SoulWalker._createdList.Count;
		if (nSoulCount > 0)
		{
			for (;;)
			{
				if (BossDK_SoulWalker._createdList.Count == 0)
					break;
				
				HT.Utils.SafeDestroy (BossDK_SoulWalker._createdList[0].gameObject);
			}
			
			BossDK_SoulWalker._createdList.Clear ();
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	public override void Extended_EventCallback(AIActor.eActorEventCallback eEvent, GameObject pParam)
	{
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void SwordPickUp(float fLerpTime)
	{
		if (m_pSword == null)
			m_pSword = BossDK_SwordScript._Instance;

		//-----
		m_pSword.SetSwordPosInit(fLerpTime);
		m_pSword.m_pBindTarget = m_pSwordBindDummy;
	}

	void SetSoulResidueCoolTime()
	{
		int nDifficulty = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;

		float fResidueTime = m_fCreateSoulResidueTime[nDifficulty];
		m_fSoulResidueCoolTime = fResidueTime; //m_fCreateSoulResidueTime + (m_fCreateSoulResidueTime_PerLv * nDifficulty);
	}


	/////////////////////////////////////////
	//---------------------------------------
	private void OnCollisionEnter(Collision collision)
	{
		if (_physicExit_CheckPhys == null)
			return;

		for(int nInd = 0; nInd < _physicExit_CheckPhys.Count; ++nInd)
		{
			if (_physicExit_CheckPhys[nInd] == collision.collider)
			{
				_physicExit_IsInPhys = true;
				_physicExit_LeastTime = _physicExit_RequireTime;
				break;
			}
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (_physicExit_IsInPhys == false)
			return;

		for (int nInd = 0; nInd < _physicExit_CheckPhys.Count; ++nInd)
		{
			if (_physicExit_CheckPhys[nInd] == collision.collider)
			{
				_physicExit_IsInPhys = false;
				break;
			}
		}
	}
}
