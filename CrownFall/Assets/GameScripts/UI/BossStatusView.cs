using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/////////////////////////////////////////
//---------------------------------------
public class BossStatusView : MonoBehaviour
{
	//---------------------------------------
	[Header("BOSS STAT VIEW")]
	[SerializeField]
	private Text _bossName = null;
	[SerializeField]
	private Slider _bossStat_Vitality = null;
	[SerializeField]
	private Slider _bossStat_Speed = null;
	[SerializeField]
	private Slider _bossStat_Complexity = null;


	//---------------------------------------
	[Header("BOSS SKILL VIEW")]
	[SerializeField]
	private Text _analysticPoint = null;
	//[SerializeField]
	//private Button _btn_SkillView = null;


	//---------------------------------------
	[Header("ARCHIVEMENT VIEW")]
	[SerializeField]
	private ArchivementViewer _archiveViewer = null;


	//---------------------------------------
	const string _analysticPointDesc = "boss_stat_view_score";
	private string _actorName = null;

	//---------------------------------------
	private void OnEnable()
	{
		HT.HTLocaleTable.onLanguageChanged += UpdateLocals;
	}

	private void OnDisable()
	{
		HT.HTLocaleTable.onLanguageChanged -= UpdateLocals;
	}

	//---------------------------------------
	public void UpdateAllData(LevelSettings pLevel, AIActor pBossActor)
	{
		_actorName = pLevel.m_pEnemyActor.m_pActorInfo.m_szActorName;

		//-----
		UpdateLocals();
		_bossName.color = pLevel.m_pLevelColor;

		_bossStat_Vitality.value = pLevel._skillInfo_Vitality;
		_bossStat_Speed.value = pLevel._skillInfo_Speed;
		_bossStat_Complexity.value = pLevel._skillInfo_Complexity;

		//-----
		string szPointDesc = HT.HTLocaleTable.GetLocalstring(_analysticPointDesc);
		_analysticPoint.text = string.Format(szPointDesc, 0);

		//-----
		if (_archiveViewer != null)
			_archiveViewer.OnChangeViewType(pLevel._bossInfo_BossType);
	}

	private void UpdateLocals()
	{
		_bossName.text = HT.HTLocaleTable.GetLocalstring(_actorName);
	}
}


/////////////////////////////////////////
//---------------------------------------