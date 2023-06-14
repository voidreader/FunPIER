using UnityEngine;
using System.Collections;


/////////////////////////////////////////
//---------------------------------------
public sealed class AIActor_Extend_FL : AIActor_Extend
{
	/////////////////////////////////////////
	//---------------------------------------
	[Header("MAIN SETTINGS")]
	public BossFL_Parts[] m_vParts;
	public Material m_pMagmaMaterial;
	public ActorBuff _atkDelayBuff = null;
	static readonly string s_szMagmaMaterialName = "Material #25_Magma";
	Material m_pNewMagmaMaterials;

	//---------------------------------------
	[Header("MAGMA")]
	public float m_fMagmaDecreaseRatio = 0.01f;
	public float m_fMagmaEmissionRatio = 200.0f;
	float m_fMagmaRatio = 1.0f;
	float m_fMagmaRatio_Prev;
	float m_fMagmaRatio_Lerp;
	static readonly string s_szDissolveRateName = "_Cutoff";

	float m_fMagmaEmission = 0.0f;
	float m_fMagmaEmission_Prev;

	public float m_fMagmaDecreaseWhenDamaged = 0.015f;
	public float m_fMagmaDecreaseWhenSkillCast = 0.01f;

	public float m_fMagmaIncreaseRatio = 0.05f;
	public ParticleSystem m_pMagmaDropParticle;

	//---------------------------------------
	[Header("FLOOR DOWN")]
	public float m_fFloorDownRepeat_Lv1 = 2.5f;
	public float m_fFloorDownRepeat_Lv2 = 2.0f;
	public float m_fFloorDownRepeat_Lv3 = 1.5f;
	float m_fFloorDownRepeat;
	float m_fFloorDownReady;
	public float m_fFloorDiveTimeMin = 10.0f;
	public float m_fFloorDiveTimeMax = 20.0f;

	//---------------------------------------
	[Header("TIDES OF FLOOR")]
	public float _floorTides_TideSpeed = 0.5f;
	public float _floorTides_RepeatTime = 5.0f;
	public float _floorTides_DiveTime = 2.0f;

	private bool _floorTide_Enabled = false;
	private bool _floorTide_Progressing = false;
	private bool _floorTide_ToHorizon = false;
	private bool _floorTide_ToPositive = false;

	private float _floorTide_waitForSpeed = 0.0f;
	private float _floorTide_waitForRepeat = 0.0f;
	private int _floorTide_ProgX = 0;
	private int _floorTide_ProgY = 0;


	/////////////////////////////////////////
	//---------------------------------------
	[Header("ARCHIVEMENT - JUMP")]
	public ActorBuff _acv_Jump_Buff = null;
	public string _acv_Jump_ArchiveName = null;

	public string _acv_FinHit_ArchiveName = null;


	/////////////////////////////////////////
	//---------------------------------------
	[Header("ETC")]
	public ActorBuff m_pBuff_HasMagma;
	public ActorBuff m_pBuff_DiveInMagma;
	public ActorBuff m_pBuff_MagmaLostAll;
	public ActorBuff m_pBuff_MagmaChargingFull;

	//---------------------------------------
	static readonly string s_szIDLE_Stance_Ground = "IDLE";
	static readonly string s_szIDLE_Stance_UnderWater = "IDLE_UNDERWATER";

	static readonly string s_szDEATH_Stance_Ground = "DEATH";
	static readonly string s_szDEATH_Stance_UnderWater = "DEATH_UNDERWATER";

	//---------------------------------------
	//static string s_szSkillName_Bite = "BITE";
	static string s_szSkillName_DiveToMagma = "DIVE_TO_MAGMA";
	static string s_szSkillName_DiveToGround = "JUMP_TO_GROUND";


	/////////////////////////////////////////
	//---------------------------------------
	public override void Extended_Init()
	{
		//-----
		m_vParts = GetComponentsInChildren<BossFL_Parts>();

		m_pNewMagmaMaterials = Instantiate(m_pMagmaMaterial);
		m_pNewMagmaMaterials.EnableKeyword(s_szDissolveRateName);

		for (int nInd = 0; nInd < m_vParts.Length; ++nInd)
		{
			MeshRenderer pRenderer = m_vParts[nInd].m_pMeshRenderer;
			if (pRenderer == null)
				continue;

			Material[] pMaterials = pRenderer.materials;
			for (int nMatInd = 0; nMatInd < pMaterials.Length; ++nMatInd)
			{
				if (pMaterials[nMatInd].name.IndexOf(s_szMagmaMaterialName) >= 0)
				{
					pMaterials[nMatInd] = m_pNewMagmaMaterials;
				}
			}

			pRenderer.materials = pMaterials;
		}

		//-----
		m_fMagmaRatio_Prev = m_fMagmaRatio;
		m_fMagmaRatio_Lerp = m_fMagmaRatio;
		m_pNewMagmaMaterials.SetFloat(s_szDissolveRateName, 1.0f - m_fMagmaRatio_Lerp);

		//-----
		bool bFloorTideEnabled = false;
		switch (GameFramework._Instance.m_pPlayerData.m_eDifficulty)
		{
			case eGameDifficulty.eEasy:
				m_fFloorDownRepeat = m_fFloorDownRepeat_Lv1;
				break;

			case eGameDifficulty.eNormal:
				m_fFloorDownRepeat = m_fFloorDownRepeat_Lv2;
				break;

			case eGameDifficulty.eHard:
				m_fFloorDownRepeat = m_fFloorDownRepeat_Lv3;
				bFloorTideEnabled = true;
				break;
		}

		m_fFloorDownReady = m_fFloorDownRepeat;
		_floorTide_Enabled = bFloorTideEnabled;
	}

	public override void Extended_PostInit()
	{
		m_pActorBase.AddActorBuff(m_pBuff_HasMagma);
		m_pActorBase.AddActorBuff(_atkDelayBuff);
	}

	public override void Extended_Frame()
	{
		if (m_fMagmaRatio > 0.0f)
		{
			m_fMagmaRatio -= (HT.TimeUtils.GameTime * m_fMagmaDecreaseRatio);
			m_fMagmaRatio = Mathf.Clamp(m_fMagmaRatio, 0.0f, 1.0f);
		}

		m_fMagmaRatio_Lerp = Mathf.Lerp(m_fMagmaRatio_Lerp, m_fMagmaRatio, HT.TimeUtils.GameTime * 2.0f);
		m_pNewMagmaMaterials.SetFloat(s_szDissolveRateName, 1.0f - m_fMagmaRatio_Lerp);

		//-----
		bool bMagmaEnabled = false;
		if (m_fMagmaRatio > 0.0f)
			bMagmaEnabled = true;

		if (m_pActorBase.m_pActorInfo.m_cnNowHP.val <= 0)
			bMagmaEnabled = false;

		m_fMagmaEmission = 0.0f;
		if ((m_fMagmaRatio_Prev - m_fMagmaRatio) > 0.0f)
		{
			m_fMagmaEmission = m_fMagmaRatio_Prev - m_fMagmaRatio;
			m_fMagmaEmission *= m_fMagmaEmissionRatio;

		}
		else
		{
			m_fMagmaEmission = 0.0f;
		}

		float fLerpRatio = HT.TimeUtils.GameTime * 5.0f;
		m_fMagmaEmission_Prev = Mathf.Lerp(m_fMagmaEmission_Prev, m_fMagmaEmission, fLerpRatio);

		for (int nInd = 0; nInd < m_vParts.Length; ++nInd)
		{
			if (m_vParts[nInd].m_pMagmaParticle == null)
				continue;

			ParticleSystem.EmissionModule pEmittor = m_vParts[nInd].m_pMagmaParticle.emission;
			pEmittor.enabled = bMagmaEnabled;

			ParticleSystem.MinMaxCurve pCurve = pEmittor.rateOverTime;
			pCurve.constantMax = m_fMagmaEmission;
			pCurve.constantMin = m_fMagmaEmission;

			pEmittor.rateOverTime = pCurve;
		}

		//-----
		IActorBase.ActorBuffEnable pBuff_HasMagma = m_pActorBase.FindEnabledActorBuff(m_pBuff_HasMagma);
		if (pBuff_HasMagma != null && m_fMagmaRatio <= 0.0f)
		{
			m_pActorBase.RemoveActorBuff(m_pBuff_HasMagma, true);
			m_pActorBase.AddActorBuff(m_pBuff_MagmaLostAll);
		}

		IActorBase.ActorBuffEnable pBuff_Dive = m_pActorBase.FindEnabledActorBuff(m_pBuff_DiveInMagma);
		if (pBuff_Dive != null)
		{
			m_fMagmaRatio += (HT.TimeUtils.GameTime * (m_fMagmaIncreaseRatio + m_fMagmaDecreaseRatio));

			if (m_fMagmaRatio >= 1.0f && m_pActorBase.GetActorState() != IActorBase.eActorState.eAction)
			{
				m_fMagmaRatio = Mathf.Clamp(m_fMagmaRatio, 0.0f, 1.0f);

				m_pActorBase.RemoveActorBuff(m_pBuff_DiveInMagma, true);
				m_pActorBase.AddActorBuff(m_pBuff_MagmaChargingFull);
			}
		}

		//-----
		m_fMagmaEmission_Prev = m_fMagmaEmission;
		m_fMagmaRatio_Prev = m_fMagmaRatio;

		//-----
		FloorCreator pFloorManager = FloorCreator._Instance;

		m_fFloorDownReady -= HT.TimeUtils.GameTime;
		if (m_fFloorDownReady <= 0.0f)
		{
			m_fFloorDownReady = m_fFloorDownRepeat;

			int nX = Random.Range(0, pFloorManager.m_nCountX);
			int nY = Random.Range(0, pFloorManager.m_nCountY);

			VolcanicCanyon_Floor pFloor = pFloorManager.GetFloor(nX, nY);
			if (pFloor.GetDive() == false)
			{
				//BattleFramework._Instance.CreateAreaAlert (pFloor.m_pFloorTagPoint.transform.position, 2.0f, 1.0f);
				pFloor.SetDive(Random.Range(m_fFloorDiveTimeMin, m_fFloorDiveTimeMax), false);
			}
		}

		//-----
		if (_floorTide_Enabled)
		{
			if (_floorTide_waitForRepeat > 0.0f)
				_floorTide_waitForRepeat -= HT.TimeUtils.GameTime;

			else
			{
				if (_floorTide_Progressing)
				{
					if (_floorTide_waitForSpeed > 0.0f)
						_floorTide_waitForSpeed -= HT.TimeUtils.GameTime;

					else
					{
						VolcanicCanyon_Floor pFloor = pFloorManager.GetFloor(_floorTide_ProgX, _floorTide_ProgY);
						if (pFloor.GetDive() == false)
							pFloor.SetDive(_floorTides_DiveTime, false);

						if (_floorTide_ToHorizon)
							_floorTide_ProgX += (_floorTide_ToPositive) ? 1 : -1;

						else
							_floorTide_ProgY += (_floorTide_ToPositive) ? 1 : -1;

						_floorTide_waitForSpeed = _floorTides_TideSpeed;

						if (_floorTide_ProgX < 0 || _floorTide_ProgX >= pFloorManager.m_nCountX ||
							_floorTide_ProgY < 0 || _floorTide_ProgY >= pFloorManager.m_nCountY)
						{
							_floorTide_Progressing = false;

							_floorTide_waitForSpeed = 0.0f;
							_floorTide_waitForRepeat = _floorTides_RepeatTime;
						}
					}
				}
				else
				{
					_floorTide_ToHorizon = (UnityEngine.Random.Range(0.0f, 1.0f) > 0.5f) ? true : false;
					_floorTide_ToPositive = (UnityEngine.Random.Range(0.0f, 1.0f) > 0.5f) ? true : false;
					if (_floorTide_ToHorizon)
					{
						_floorTide_ProgX = (_floorTide_ToPositive) ? 0 : pFloorManager.m_nCountX - 1;
						_floorTide_ProgY = UnityEngine.Random.Range(0, pFloorManager.m_nCountY);
					}
					else
					{
						_floorTide_ProgX = UnityEngine.Random.Range(0, pFloorManager.m_nCountX);
						_floorTide_ProgY = (_floorTide_ToPositive) ? 0 : pFloorManager.m_nCountY - 1;
					}

					_floorTide_Progressing = true;
				}
			}
		}
	}

	public override void Extended_Release()
	{
	}


	/////////////////////////////////////////
	//---------------------------------------
	public override void Extended_EventCallback(AIActor.eActorEventCallback eEvent, GameObject pParam)
	{
		IActorBase.ActorBuffEnable pBuff_Dive = m_pActorBase.FindEnabledActorBuff(m_pBuff_DiveInMagma);

		//-----
		switch (eEvent)
		{
			case AIActor.eActorEventCallback.eDamaged:
				{
					if (pBuff_Dive == null)
						m_fMagmaRatio -= m_fMagmaDecreaseWhenDamaged;

					if (m_pActorBase.FindEnabledActorBuff(_acv_Jump_Buff) != null)
					{
						Archives pArchives = ArchivementManager.Instance.FindArchive(_acv_Jump_ArchiveName);
						pArchives.Archive.OnArchiveCount(1);
					}
					else if (m_pActorBase.FindEnabledActorBuff(m_pBuff_DiveInMagma) != null)
					{
						Archives pArchives = ArchivementManager.Instance.FindArchive(_acv_FinHit_ArchiveName);
						pArchives.Archive.OnArchiveCount(1);
					}
				}
				break;

			case AIActor.eActorEventCallback.eSkillCast:
				{
					m_fMagmaRatio -= m_fMagmaDecreaseWhenSkillCast;

					ActorSkill pSkill = pParam.GetComponent<ActorSkill>();
					if (pSkill != null)
					{
						if (pSkill.m_szSkillName == s_szSkillName_DiveToMagma)
						{
							m_pActorBase.RemoveActorBuff(m_pBuff_MagmaLostAll, true);

							m_pActorBase.m_szIDLEAnimName = s_szIDLE_Stance_UnderWater;
							m_pActorBase.m_szDEATHAnimName = s_szDEATH_Stance_UnderWater;
						}
						else if (pSkill.m_szSkillName == s_szSkillName_DiveToGround)
						{
							m_pActorBase.RemoveActorBuff(m_pBuff_MagmaChargingFull, true);

							m_pActorBase.m_szIDLEAnimName = s_szIDLE_Stance_Ground;
							m_pActorBase.m_szDEATHAnimName = s_szDEATH_Stance_Ground;
						}
					}
				}
				break;
		}

		//-----
		m_fMagmaRatio = Mathf.Clamp(m_fMagmaRatio, 0.0f, 1.0f);
	}


	/////////////////////////////////////////
	//---------------------------------------
	void OnMagmaDropParticle()
	{
		ParticleSystem.EmissionModule pEmittor = m_pMagmaDropParticle.emission;
		pEmittor.enabled = true;

		ParticleSystem.MinMaxCurve pCurve = pEmittor.rateOverTime;
		pCurve.constantMax = 50.0f;
		pCurve.constantMin = 50.0f;
		pEmittor.rateOverTime = pCurve;
	}

	void OffMagmaDropParticle()
	{
		ParticleSystem.EmissionModule pEmittor = m_pMagmaDropParticle.emission;
		pEmittor.enabled = false;
	}
}
