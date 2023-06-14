using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class GameObj_HitListener : MonoBehaviour
{
	//---------------------------------------
	public virtual void OnHitByActor(IActorBase pActor, int nDamage)
	{

	}

	//---------------------------------------
	public virtual bool IsEnabled()
	{
		return true;
	}
}


/////////////////////////////////////////
//---------------------------------------