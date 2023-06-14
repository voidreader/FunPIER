using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public class UpgradePointViewer : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	public Color m_pEnableColor;
	public Color m_pDisableColor;


	/////////////////////////////////////////
	//---------------------------------------
	Text m_pText;

	int m_nPrevPoint;


	/////////////////////////////////////////
	//---------------------------------------
	void Start()
	{
		m_pText = GetComponent<Text>();

		m_nPrevPoint = GameFramework._Instance.m_pPlayerData.m_nUpgradePoint;
		UpdateTexts();

		HT.HTLocaleTable.onLanguageChanged += UpdateTexts;
	}

	private void OnDestroy()
	{
		HT.HTLocaleTable.onLanguageChanged -= UpdateTexts;
	}

	void Update()
	{
		bool bNeedUpdate = false;

		if (m_nPrevPoint != GameFramework._Instance.m_pPlayerData.m_nUpgradePoint)
			bNeedUpdate = true;

		if (bNeedUpdate)
			UpdateTexts();
	}


	/////////////////////////////////////////
	//---------------------------------------
	void UpdateTexts()
	{
		m_nPrevPoint = GameFramework._Instance.m_pPlayerData.m_nUpgradePoint;

		string szLocalString = HT.HTLocaleTable.GetLocalstring("charinfo_remainpoint");

		m_pText.text = string.Format("{0} : {1}", szLocalString, m_nPrevPoint);
		m_pText.color = (m_nPrevPoint > 0) ? m_pEnableColor : m_pDisableColor;
	}
}
