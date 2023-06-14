using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public enum eParentObject
{
	Player,
	Boss,

	Max,
}

/////////////////////////////////////////
//---------------------------------------
public class GameObj_DamageReciver : GameObj_HitListener
{
	//---------------------------------------
	public Action<int> onDamaged = null;

	//---------------------------------------
	[Header("RECIVER SETTING")]
	[SerializeField]
	private eParentObject _parentType = eParentObject.Max;
	[SerializeField]
	private IActorBase _parentActor = null;
	[SerializeField]
	private float _reciveRatio = 1.0f;

	//---------------------------------------
	[Header("ARCHIVEMENTS")]
	[SerializeField]
	private string _archiveName = null;

	//---------------------------------------
	public override void OnHitByActor(IActorBase pActor, int nDamage)
	{
		if (_parentActor == null)
		{
			switch (_parentType)
			{
				case eParentObject.Boss:
					_parentActor = BattleFramework._Instance.m_pEnemyActor;
					break;

				case eParentObject.Player:
					_parentActor = BattleFramework._Instance.m_pPlayerActor;
					break;
			}

			if (_parentActor == null)
				return;
		}

		int nCalDamage = (int)(nDamage * _reciveRatio);
		if (_parentActor != pActor)
			_parentActor.OnDamaged(nCalDamage);

		HT.Utils.SafeInvoke(onDamaged, nCalDamage);

		if (string.IsNullOrEmpty(_archiveName) == false)
		{
			Archives pArchives = ArchivementManager.Instance.FindArchive(_archiveName);
			pArchives.Archive.OnArchiveCount(1);
		}
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------