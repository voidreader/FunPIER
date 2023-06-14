using UnityEngine;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public sealed class FieldEvent_ActorAnim : FieldEvent {
	/////////////////////////////////////////
	//---------------------------------------
	public enum eTargetActorType {
		ePlayerActor = 0,
		eEnemyActor,
	}

	public eTargetActorType m_eTargetActor;
	public string m_szAnimName;

	public bool m_bWaitAnimTime;


	/////////////////////////////////////////
	//---------------------------------------
	public override void Init_Child () {
		IActorBase pActor = null;

		switch (m_eTargetActor) {
		case eTargetActorType.eEnemyActor:
			pActor = BattleFramework._Instance.m_pEnemyActor;
			break;

		case eTargetActorType.ePlayerActor:
			pActor = BattleFramework._Instance.m_pPlayerActor;
			break;
		}

		pActor.m_pAnimations.Play (m_szAnimName);

		if (m_bWaitAnimTime) {
			AnimationClip pAnimClip = pActor.m_pAnimations.GetClip (m_szAnimName);
			m_fTimeLeast = pAnimClip.length;
		}
	}


	public override void Frame_Child () {
	}


	public override void Release_Child () {
	}


	/////////////////////////////////////////
	//---------------------------------------
}
