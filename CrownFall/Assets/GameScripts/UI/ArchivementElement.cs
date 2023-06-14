using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public class ArchivementElement : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	public Image m_pIcon;

	public Text m_pPoint;
	public Text m_pSubject;
	public Text m_pDescript;

	public Text m_pRecord;
	public Slider m_pRecordSlider;


	/////////////////////////////////////////
	//---------------------------------------
	public Image m_pWindowPanel;
	public Color m_pDisableColor;
	public Color m_pEnableColor;

	//---------------------------------------
	public string _curArchiveID = null;


	/////////////////////////////////////////
	//---------------------------------------
	public void Init(string szID)
	{
		_curArchiveID = szID;

		//-----
		ResetElement();
	}

	private void OnEnable()
	{
		RefreshInfos();
	}

	private void Awake()
	{
		HT.HTLocaleTable.onLanguageChanged += ResetElement;
	}

	private void OnDestroy()
	{
		HT.HTLocaleTable.onLanguageChanged -= ResetElement;
	}


	/////////////////////////////////////////
	//---------------------------------------
	void ResetElement()
	{
		Archives pArchive = ArchivementManager.Instance.FindArchive(_curArchiveID);
		if (pArchive != null)
		{
			m_pIcon.sprite = pArchive.Archive.ArchiveIcon;
			m_pSubject.text = HT.HTLocaleTable.GetLocalstring(pArchive.Archive.ArchiveName);
			m_pDescript.text = HT.HTLocaleTable.GetLocalstring(pArchive.Archive.ArchiveDesc);

			m_pPoint.text = string.Format("+{0}", pArchive.Archive.ArchivePoint);
		}

		RefreshInfos();
	}

	void RefreshInfos()
	{
		Archives pArchive = ArchivementManager.Instance.FindArchive(_curArchiveID);
		if (pArchive != null)
		{
			bool bEnabledArchive = pArchive.IsComplete();
			m_pWindowPanel.color = ((bEnabledArchive) ? m_pEnableColor : m_pDisableColor);

			if (m_pRecordSlider != null)
			{
				m_pRecordSlider.gameObject.SetActive(!bEnabledArchive);

				m_pRecord.text = string.Format("{0}/{1}", pArchive.Counted, pArchive.Archive.RequireCount);
				m_pRecordSlider.value = ((pArchive.Counted * 1.0f) / pArchive.Archive.RequireCount);
			}
		}
	}
}
