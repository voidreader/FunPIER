using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HT;


/////////////////////////////////////////
//---------------------------------------
public class ArchivementManager : SingletonBehaviour<ArchivementManager>
{
	//---------------------------------------
	public Action<Archives> onArchiveComplete = null;

	//---------------------------------------
	[Header("SETTINGS")]
	[SerializeField]
	private Archivement[] _archivements = null;
	[SerializeField]
	private Archivement[] _archivements_DK = null;
	[SerializeField]
	private Archivement[] _archivements_FL = null;
	[SerializeField]
	private Archivement[] _archivements_PD = null;
	[SerializeField]
	private Archivement[] _archivements_GL = null;
	[SerializeField]
	private Archivement[] _archivements_HAL = null;
	[SerializeField]
	private Archivement[] _archivements_VT = null;
	[SerializeField]
	private Archivement[] _archivements_GT = null;
	[SerializeField]
	private Archivement[] _archivements_RG = null;
	[SerializeField]
	private Archivement[] _archivements_BC = null;

	//---------------------------------------
	[Header("OBJECST")]
	[SerializeField]
	private ArchivementShower _archiveShower = null;

	//---------------------------------------
	private Archives[] _archives = null;

	public Archives[] Archives { get { return _archives; } }


	//---------------------------------------
	public void Init()
	{
		onArchiveComplete += ShowArchivement;

		//-----
		int nTotalCount = _archivements.Length;
		nTotalCount += _archivements_DK.Length;
		nTotalCount += _archivements_FL.Length;
		nTotalCount += _archivements_PD.Length;
		nTotalCount += _archivements_GL.Length;
		nTotalCount += _archivements_HAL.Length;
		nTotalCount += _archivements_VT.Length;
		nTotalCount += _archivements_GT.Length;
		nTotalCount += _archivements_RG.Length;
		nTotalCount += _archivements_BC.Length;

		_archives = new Archives[nTotalCount];

		int nLastIndex = 0;
		nLastIndex = AddToArchives(_archivements, nLastIndex);
		nLastIndex = AddToArchives(_archivements_DK, nLastIndex);
		nLastIndex = AddToArchives(_archivements_FL, nLastIndex);
		nLastIndex = AddToArchives(_archivements_PD, nLastIndex);
		nLastIndex = AddToArchives(_archivements_GL, nLastIndex);
		nLastIndex = AddToArchives(_archivements_HAL, nLastIndex);
		nLastIndex = AddToArchives(_archivements_VT, nLastIndex);
		nLastIndex = AddToArchives(_archivements_GT, nLastIndex);
		nLastIndex = AddToArchives(_archivements_RG, nLastIndex);
		nLastIndex = AddToArchives(_archivements_BC, nLastIndex);
	}

	private int AddToArchives(Archivement[] vArchivement, int nLastIndex)
	{
		int nInd = 0;
		for (; nInd < vArchivement.Length; ++nInd)
		{
			_archives[nLastIndex + nInd] = new Archives();
			_archives[nLastIndex + nInd].Init(vArchivement[nInd]);
		}

		return nLastIndex + nInd;
	}

	//---------------------------------------
	public Archives FindArchive(string szID)
	{
		for (int nInd = 0; nInd < _archives.Length; ++nInd)
			if (_archives[nInd].Archive.ArchiveID == szID)
				return _archives[nInd];

		return null;
	}

	public Archivement[] FindArchivements(eBossType eType)
	{
		switch(eType)
		{
			case eBossType.eSoulBringer: return _archivements_DK;
			case eBossType.eMeltenMaw: return _archivements_FL;
			case eBossType.eSchenabel: return _archivements_PD;
			case eBossType.eStoneGolem: return _archivements_GL;
			case eBossType.eArchitecture: return _archivements_HAL;
			case eBossType.eVenusTrap: return _archivements_VT;
			case eBossType.eGladiator: return _archivements_GT;
			case eBossType.eRoyalGuard: return _archivements_RG;
			case eBossType.eBlack: return _archivements_BC;
		}

		return null;
	}

	//---------------------------------------
	private void ShowArchivement(Archives pArchives)
	{
		ArchivementShower pShower = HT.Utils.Instantiate(_archiveShower);

		Canvas pCanvas = HTGlobalUI.Instance.MainCanvas;
		pShower.transform.SetParent(pCanvas.transform);
		pShower.m_pRectTransform.anchoredPosition = _archiveShower.m_pRectTransform.anchoredPosition;

		pShower.Init(pArchives);
	}

	//---------------------------------------
	private const int VERSION = 1;

	//---------------------------------------
	public JSONNode ToJSON()
	{
		JSONClass pJSON = new JSONClass();

		JSONUtils.AddValue(pJSON, "VSN", VERSION);

		JSONUtils.AddValue(pJSON, "ACV_CNT", Archives.Length);

		for (int nInd = 0; nInd < Archives.Length; ++nInd)
		{
			Archives pArcv = Archives[nInd];
			JSONUtils.AddValue(pJSON, string.Format("ACV_NM_{0}", nInd), pArcv.Archive.ArchiveID);
			JSONUtils.AddValue(pJSON, string.Format("ACV_CL_{0}", nInd), pArcv.IsComplete());
		}

		return pJSON;
	}

	//---------------------------------------
	public void ParseJSON(JSONNode pNode)
	{
		if (pNode == null)
			return;

		int nVersion = 0;
		JSONUtils.GetValue(pNode, "VSN", out nVersion);

		int nCount = 0;
		JSONUtils.GetValue(pNode, "ACV_CNT", out nCount);

		for (int nInd = 0; nInd < nCount; ++nInd)
		{
			string szID = null;
			JSONUtils.GetValue(pNode, string.Format("ACV_NM_{0}", nInd), out szID);

			Archives pFound = FindArchive(szID);
			if (pFound != null)
			{
				bool bCompleted = false;
				JSONUtils.GetValue(pNode, string.Format("ACV_CL_{0}", nInd), out bCompleted);

				if (bCompleted)
					pFound.Counted = pFound.Archive.RequireCount;
				else
					pFound.Counted = 0;
			}
		}
	}

	//---------------------------------------
	public void RefreshAllByPlatformData()
	{
		HT.HTDebug.PrintLog(eMessageType.None, "[HTPlatform] Archive record reset by Platform Data");

		HTPlatform pPlatform = HTPlatform.Instance;
		for (int nInd = 0; nInd < _archives.Length; ++nInd)
		{
			string szID = _archives[nInd].Archive.CorrectID;

			int nValue = 0;
			if (pPlatform.GetAchievementCleared(szID))
			{
				HT.HTDebug.PrintLog(eMessageType.None, string.Format("[HTPlatform] {0} has cleared", szID));
				nValue = _archives[nInd].Archive.RequireCount;
			}

			_archives[nInd].Counted = nValue;
		}
	}
}


/////////////////////////////////////////
//---------------------------------------