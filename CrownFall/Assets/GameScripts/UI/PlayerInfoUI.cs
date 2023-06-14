using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/////////////////////////////////////////
//---------------------------------------
public class PlayerInfoUI : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private string _dashSkillName = null;
	[SerializeField]
	private Image _img_DashCoolTime = null;
	[SerializeField]
	private IActorBase _playerActor = null;

	//---------------------------------------
	private ActorSkill _playerDashSkill = null;

	//---------------------------------------
	void Update()
	{
		if (_playerDashSkill == null)
		{
			if (_playerActor == null && BattleFramework._Instance != null)
				_playerActor = BattleFramework._Instance.m_pPlayerActor;

			if (_playerActor != null)
				_playerDashSkill = _playerActor.FindSkillInfo(_dashSkillName);
		}

		if (_playerDashSkill != null)
		{
			float fRatio = _playerDashSkill.GetSkillCooling() / _playerDashSkill.m_fSkillCoolTime;
			_img_DashCoolTime.fillAmount = fRatio;
		}
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------