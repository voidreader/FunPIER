using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class TutorialObj_Physic : MonoBehaviour
{
	private enum ePhysicEventType
	{
		Exit,
		Movement,
	}

	//---------------------------------------
	[SerializeField]
	private Field_Tutorial _field = null;
	[SerializeField]
	private ePhysicEventType _eventType = ePhysicEventType.Exit;

	//---------------------------------------
	private void OnTriggerEnter(Collider other)
	{
		if (_field._playerActor.gameObject == other.gameObject)
		{
			switch(_eventType)
			{
				case ePhysicEventType.Exit:
					_field.OnSkipButton();
					break;

				case ePhysicEventType.Movement:
					_field.OnEnterTrigger_Movement();
					break;
			}
		}
	}
}


/////////////////////////////////////////
//---------------------------------------