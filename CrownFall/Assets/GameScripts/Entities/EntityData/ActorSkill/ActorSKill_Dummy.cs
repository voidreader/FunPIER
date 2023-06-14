using UnityEngine;
using System.Collections;


/////////////////////////////////////////
//---------------------------------------
public sealed class ActorSKill_Dummy : ActorSkill
{
	/////////////////////////////////////////
	//---------------------------------------


	/////////////////////////////////////////
	//---------------------------------------
	public override bool SkillCastReady_Child()
	{
		return true;
	}

	public override void SkillThrow_Child()
	{
		CallSkillObject_Throw();
	}

	/////////////////////////////////////////
	//---------------------------------------
}
