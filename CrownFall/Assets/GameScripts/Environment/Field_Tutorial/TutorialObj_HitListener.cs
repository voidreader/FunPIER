using System;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class TutorialObj_HitListener : GameObj_HitListener
{
	//---------------------------------------
	[SerializeField]
	private Field_Tutorial _field = null;

	//---------------------------------------
	public override void OnHitByActor(IActorBase pActor, int nDamage)
	{
		_field.OnHitListener();
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------