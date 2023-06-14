using UnityEngine;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public class AIActor_Extend : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	public static AIActor_Extend _Instance;

	//---------------------------------------
	[Header("EXTEND SETTINGS")]
	public AIActor m_pActorBase;

	[Header("EXTEND SETTINGS - ADDITIVE ARCHIVE ON PLAYER")]
	public string _playerAcv_OnDash = null;
	public string _playerAcv_OnIgnoreDamage = null;


	/////////////////////////////////////////
	//---------------------------------------
	[Header("LINKED ARCHIVES")]
	public Archivement[] _linkedArchives = null;


	/////////////////////////////////////////
	//---------------------------------------
	public virtual void Init()
	{
		if (BattleFramework._Instance != null)
		{
			ControllableActor pPlayer = BattleFramework._Instance.m_pPlayerActor as ControllableActor;
			pPlayer.ArchiveWhenDash = _playerAcv_OnDash;
			pPlayer.ArchiveWhenDash = _playerAcv_OnIgnoreDamage;
		}
	}

	public virtual void Release()
	{
	}

	/////////////////////////////////////////
	//---------------------------------------
	public virtual void Extended_Init()
	{
	}

	public virtual void Extended_PostInit()
	{
	}

	public virtual void Extended_Frame()
	{
	}

	public virtual void Extended_Release()
	{
	}


	/////////////////////////////////////////
	//---------------------------------------
	public virtual bool Extended_IsDamageEnable()
	{
		return true;
	}

	public virtual void Extended_EventCallback(AIActor.eActorEventCallback eEvent, GameObject pParam)
	{
	}


	/////////////////////////////////////////
	//---------------------------------------
	public virtual void OnBattleStart()
	{
		for (int nInd = 0; nInd < _linkedArchives.Length; ++nInd)
			_linkedArchives[nInd].OnBattleStart();
	}

	public virtual void OnBattleEnd(bool bPlayerWin)
	{
		for (int nInd = 0; nInd < _linkedArchives.Length; ++nInd)
			_linkedArchives[nInd].OnBattleEnd(bPlayerWin);
	}
}
