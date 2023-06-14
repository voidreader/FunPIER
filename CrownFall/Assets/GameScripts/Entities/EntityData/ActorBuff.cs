using UnityEngine;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public class ActorBuff : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	[Header("BUFF INFO")]
	public string m_szBuffName;

	//---------------------------------------
	public enum eBuffType
	{
		eDummyBuff = 0,

		eSpeed_Percentage,

		eDamage_TimeEnd,
		eDamage_TimeMax,

		eDamage_Increase,

		eStun,

		eSpawnObject,

		eFrenzy,
	}

	[Header("BUFF EFFECTIVE INFO")]
	public eBuffType m_eBuffType;
	public float fBuffValue;

	//---------------------------------------
	public float m_fBuffTime;
	public bool _buffRemoveWhenTimeEnd = true;
	public bool _refreshTimeWhenAddSameBuff = true;

	//---------------------------------------
	public ActorBuffEffect m_pBuffEffect;

	//---------------------------------------
	public ActorBuff _endRelativeBuff = null;
	public enum eEndRelativeType
	{
		TimeEnd,
		EndEffectable,
	}
	public eEndRelativeType _endRelativeType = eEndRelativeType.TimeEnd;

	//---------------------------------------
	[Header("GUAGE DISPLAY")]
	public bool m_bShowMiniGage;
	public Color m_pMiniGageColor;

	//---------------------------------------
	ActorBuffEffect m_pCreatedEffect;

	//---------------------------------------
	[Header("BUFF STACK INFO")]
	public bool _isStackable = false;
	public int _maxStackCount = 0;

	//---------------------------------------
	[Header("ARCHIVEMENT")]
	public string m_SzEffectiveArchiveName = null;
	public enum eBuffEffectiveArchive
	{
		eNotEffective = 0,
		eWhenGetBuff,
		eWhenDamage,
		eStackCount,
	}
	public eBuffEffectiveArchive m_eEffectArchiveType = eBuffEffectiveArchive.eNotEffective;
	public int _eEffectArchiveStackCount = 1;

	//---------------------------------------
	[Header("SPAWN OBJECT")]
	public GameObject _spawn_GameObject = null;
	public enum eSpawnObjectType
	{
		eWhenEnd,
		eWhenStart,
		eWhenTime,
	}
	public eSpawnObjectType _spawnObjectType = eSpawnObjectType.eWhenEnd;
	public float _spawnObjectTime = 0.0f;
	public bool _spawnObjectAlignToGround = true;


	/////////////////////////////////////////
	//---------------------------------------
	public void Enable(IActorBase pOwner, float fTime)
	{
		if (m_pBuffEffect != null && m_pCreatedEffect == null)
		{
			m_pCreatedEffect = Instantiate(m_pBuffEffect);
			m_pCreatedEffect.m_pOwner = pOwner;
		}

		if (m_bShowMiniGage)
			BattleFramework._Instance.m_pPlayer_MiniBuffGage.SetEnabled(m_fBuffTime, fTime, m_pMiniGageColor);
	}

	public void Disabled()
	{
		if (m_pBuffEffect != null)
		{
			HT.Utils.SafeDestroy(m_pCreatedEffect.gameObject);
			m_pCreatedEffect = null;
		}

		if (m_bShowMiniGage)
			BattleFramework._Instance.m_pPlayer_MiniBuffGage.SetDisabled();
	}

	//---------------------------------------
	public void SpawnObject(IActorBase pOwner)
	{
		Vector3 vPos = pOwner.transform.position;
		GameObject pNewObj = HT.Utils.Instantiate(_spawn_GameObject);

		if (_spawnObjectAlignToGround)
			GameFramework._Instance.SetObjectPositionAndRotateByPhysic(pNewObj, vPos);
	}
}
