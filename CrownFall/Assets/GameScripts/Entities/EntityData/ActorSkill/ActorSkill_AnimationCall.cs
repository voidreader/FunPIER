using UnityEngine;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public sealed class ActorSkill_AnimationCall : ActorSkill {
	/////////////////////////////////////////
	//---------------------------------------
	public string m_szAnimationName;

	public enum eAnimCallTarget {
		eTarget = 0,

		eLevel_LightManager,
	}
	public eAnimCallTarget m_eAnimCallTarget;


	/////////////////////////////////////////
	//---------------------------------------
	public override bool SkillCastReady_Child () {
		return true;
	}

	public override void SkillThrow_Child () {
		switch (m_eAnimCallTarget) {
		case eAnimCallTarget.eTarget:
			m_pTarget.SetAction (m_szAnimationName);
			break;

		case eAnimCallTarget.eLevel_LightManager:
			{
				Field pField = BattleFramework._Instance.m_pField;
				LightManager [] vManageres = pField.m_pControllableDyanmicLights;
				for (int nInd = 0; nInd < vManageres.Length; ++nInd) {
					Animation pAnim = vManageres [nInd].GetComponent<Animation> ();

					if (pAnim != null) {
						pAnim.Play (m_szAnimationName);
					}
				}
			}
			break;
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
}
