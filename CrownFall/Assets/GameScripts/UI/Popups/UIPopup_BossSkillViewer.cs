using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/////////////////////////////////////////
//---------------------------------------
public sealed class UIPopup_BossSkillViewer : HT.UIPopup
{
	//---------------------------------------
	[Header("BOSS SKILL VIEWER")]
	[SerializeField]
	private BossSkillViewElement _skillView_Element = null;
	[SerializeField]
	private RectTransform _contents = null;

	//---------------------------------------
	private List<BossSkillViewElement> _createdElements = new List<BossSkillViewElement>();
	private IActorBase _lastBoss = null;

	//---------------------------------------
	protected override void OnOpen()
	{
		IActorBase pEnemyActor = BattleFramework._Instance.m_pEnemyActor;
		if (_lastBoss != pEnemyActor)
			RefreshAllSettings(pEnemyActor);
	}

	protected override void OnClose()
	{
	}

	//---------------------------------------
	protected override void OnTextSetting()
	{
		RefreshAllSettings(_lastBoss);
	}

	//---------------------------------------
	private void RefreshAllSettings(IActorBase pBossActor)
	{
		for (int nInd = 0; nInd < _createdElements.Count; ++nInd)
			HT.Utils.SafeDestroy(_createdElements[nInd].gameObject);

		_createdElements.Clear();

		//-----
		Vector3 vLastPos = new Vector3(0.0f, -10.0f, 0.0f);

		eGameDifficulty eGameDiff = GameFramework._Instance.m_pPlayerData.m_eDifficulty;

		ActorSkillContainer[] vSkills = pBossActor.m_vActorSkillCont;
		for(int nInd = 0; nInd < vSkills.Length; ++nInd)
		{
			if (vSkills[nInd]._showToSkillInfo == false)
				continue;

			if (vSkills[nInd]._skillInfo_AddSkillDiff > eGameDiff)
				continue;

			BossSkillViewElement pElement = HT.Utils.InstantiateFromPool(_skillView_Element);
			pElement.SetSkillInfo(vSkills[nInd]);

			//-----
			RectTransform pRectTrans = pElement.GetComponent<RectTransform>();
			pRectTrans.SetParent(_contents);

			pRectTrans.anchoredPosition = vLastPos;

			//-----
			vLastPos.y -= pRectTrans.sizeDelta.y + 5.0f;
		}

		//-----
		_contents.sizeDelta = new Vector2(_contents.sizeDelta.x, Mathf.Abs(vLastPos.y));
	}
}


/////////////////////////////////////////
//---------------------------------------