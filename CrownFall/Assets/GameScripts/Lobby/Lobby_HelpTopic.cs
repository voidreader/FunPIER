using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public class Lobby_HelpTopic : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	public HelpTopics[] m_vHelptopics;
	int m_nActivateTopic = -1;

	//---------------------------------------
	public Image m_pTopicImage;
	public Text m_pSubject;
	public Text m_pDescript;

	//---------------------------------------
	public Image m_pPrevButtonImage;
	public Image m_pNextButtonImage;

	public Color m_pEnableColor;
	public Color m_pDisableColor;


	/////////////////////////////////////////
	//---------------------------------------
	void Awake()
	{
		UpdateTopic(0);
		HT.HTLocaleTable.onLanguageChanged += UpdateTopic;
	}
	
	private void OnDestroy()
	{
		HT.HTLocaleTable.onLanguageChanged -= UpdateTopic;
	}

	/////////////////////////////////////////
	//---------------------------------------
	private void UpdateTopic()
	{
		UpdateTopic(m_nActivateTopic);
	}

	public void UpdateTopic(int nIndex)
	{
		m_nActivateTopic = nIndex;
		m_pTopicImage.sprite = m_vHelptopics[m_nActivateTopic].GetTexture();

		m_pSubject.text = HT.HTLocaleTable.GetLocalstring(m_vHelptopics[m_nActivateTopic].m_szSubject);
		m_pDescript.text = HT.HTLocaleTable.GetLocalstring(m_vHelptopics[m_nActivateTopic].m_szDescript);

		//-----
		if (m_nActivateTopic <= 0)
			m_pPrevButtonImage.color = m_pDisableColor;
		else
			m_pPrevButtonImage.color = m_pEnableColor;

		if (m_nActivateTopic >= (m_vHelptopics.Length - 1))
			m_pNextButtonImage.color = m_pDisableColor;
		else
			m_pNextButtonImage.color = m_pEnableColor;
	}

	//---------------------------------------
	public void UpdateTopic_Prev()
	{
		if (m_nActivateTopic == 0)
			return;

		UpdateTopic(m_nActivateTopic - 1);
	}

	public void UpdateTopic_Next()
	{
		if (m_nActivateTopic >= (m_vHelptopics.Length - 1))
			return;

		UpdateTopic(m_nActivateTopic + 1);
	}
}
