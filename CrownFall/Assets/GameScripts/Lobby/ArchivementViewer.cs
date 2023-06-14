using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/////////////////////////////////////////
//---------------------------------------
public class ArchivementViewer : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	public ArchivementElement m_pInst_ArchiveElem;
	public RectTransform m_pContentRect;

	//---------------------------------------
	public bool _isAutoUpdate = true;

	//---------------------------------------
	public Toggle[] _toggleButtons = null;
	public Text m_pTotalPoint_Desc;
	public Text m_pArchiveCount_Desc;
	public Slider m_pArchiveCount_Slider;

	//---------------------------------------
	public eBossType _archiveViewType = eBossType.eSoulBringer;

	//---------------------------------------
	private List<ArchivementElement> _createdList = new List<ArchivementElement>();


	/////////////////////////////////////////
	//---------------------------------------
	void Awake()
	{
		if (_isAutoUpdate)
		{
			_archiveViewType = eBossType.eSoulBringer;
			UpdateArchivements();
		}
	}

	//---------------------------------------
	private void OnEnable()
	{
		int nNowPoint = 0;
		int nClearCount = 0;

		//-----
		ArchivementManager pManager = ArchivementManager.Instance;
		Archives[] vArchives = pManager.Archives;
		for (int nInd = 0; nInd < vArchives.Length; ++nInd)
		{
			if (vArchives[nInd].IsComplete())
			{
				++nClearCount;
				nNowPoint += vArchives[nInd].Archive.ArchivePoint;
			}
		}

		//-----
		if (m_pTotalPoint_Desc != null)
			m_pTotalPoint_Desc.text = string.Format("{0}", nNowPoint);

		if (m_pArchiveCount_Desc != null)
			m_pArchiveCount_Desc.text = string.Format("{0}/{1}", nClearCount, vArchives.Length);
		
		if (m_pArchiveCount_Slider != null)
		{
			float fRatio = ((float)nClearCount) / vArchives.Length;
			m_pArchiveCount_Slider.value = fRatio;
		}
	}

	//---------------------------------------
	public void OnChangeViewType(eBossType eType)
	{
		_archiveViewType = eType;
		UpdateArchivements();
	}

	public void UpdateArchivements()
	{
        float fLastY = 0.0f;
		float fTotalHeight = 0;

		//-----
		for (int nInd = 0; nInd < _createdList.Count; ++nInd)
			HT.Utils.SafeDestroy(_createdList[nInd].gameObject);

		_createdList.Clear();

		//-----
		ArchivementManager pManager = ArchivementManager.Instance;
		Archives[] vArchives = pManager.Archives;
		
		for (int nInd = 0; nInd < vArchives.Length; ++nInd)
		{
			if (vArchives[nInd].Archive.BossType != _archiveViewType)
				continue;
		
			ArchivementElement pElem = HT.Utils.Instantiate(m_pInst_ArchiveElem);
			_createdList.Add(pElem);
		
			RectTransform pElemTrans = pElem.GetComponent<RectTransform>();
            pElemTrans.transform.SetParent(m_pContentRect.transform);

            Vector2 vPos = pElemTrans.transform.position;
			vPos.y += fLastY;
		
			fLastY -= pElemTrans.rect.height;
			fTotalHeight += pElemTrans.rect.height;

			pElemTrans.anchoredPosition = vPos;
		
			pElem.Init(vArchives[nInd].Archive.ArchiveID);
		}

		//-----
		Vector2 vSize = m_pContentRect.sizeDelta;
		vSize.y = fTotalHeight;
		vSize.y -= (m_pInst_ArchiveElem.transform.position.y * 2.0f);

		m_pContentRect.sizeDelta = vSize;
	}

	/////////////////////////////////////////
	//---------------------------------------
}
