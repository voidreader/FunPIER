using UnityEngine;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public sealed class FieldEvent_CamTarget : FieldEvent {
	/////////////////////////////////////////
	//---------------------------------------
	public enum eTargetActorType {
		ePlayerActor = 0,
		eEnemyActor,
	}

	public eTargetActorType m_eTargetActor;


	/////////////////////////////////////////
	//---------------------------------------
	public override void Init_Child () {
		switch (m_eTargetActor) {
		case eTargetActorType.eEnemyActor:
			CameraManager._Instance.m_pTargetEntity = BattleFramework._Instance.m_pEnemyActor.gameObject;
			break;

		case eTargetActorType.ePlayerActor:
			CameraManager._Instance.m_pTargetEntity = BattleFramework._Instance.m_pPlayerActor.gameObject;
			break;
		}
	}


	public override void Frame_Child () {
	}


	public override void Release_Child () {
	}

	/////////////////////////////////////////
	//---------------------------------------
}
