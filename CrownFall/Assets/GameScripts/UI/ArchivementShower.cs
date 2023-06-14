using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using HT;

public class ArchivementShower : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	public RectTransform m_pRectTransform;


	/////////////////////////////////////////
	//---------------------------------------
	public Image m_pIcon;

	public Text m_pPoint;
	public Text m_pSubject;
	public Text m_pDescript;

	//---------------------------------------
	public ParticleSystem m_pCreateEffect;
	public float m_fCreateEffectFromCamDist = 0.005f;

	//---------------------------------------
	public static int s_nActivatedShowerCount = 0;


	/////////////////////////////////////////
	//---------------------------------------
	public void Init(Archives pArchive)
	{
		++ArchivementShower.s_nActivatedShowerCount;
		
		//-----
		m_pIcon.sprite = pArchive.Archive.ArchiveIcon;
		
		m_pSubject.text = HT.HTLocaleTable.GetLocalstring(pArchive.Archive.ArchiveName);
		m_pDescript.text = HT.HTLocaleTable.GetLocalstring(pArchive.Archive.ArchiveDesc);
		
		m_pPoint.text = string.Format ("+{0}", pArchive.Archive.ArchivePoint);
		
		//-----
		Camera pMainCam = CameraManager._Instance.m_pMainCamera;
		
		ParticleSystem pNewParticle = HT.Utils.InstantiateFromPool(m_pCreateEffect);
		Vector3 vPos = m_pRectTransform.transform.position;
		vPos.z = m_fCreateEffectFromCamDist;
		
		vPos = pMainCam.ScreenToWorldPoint (vPos);
		pNewParticle.transform.position = vPos;

		pNewParticle.transform.SetParent(pMainCam.transform);

		//-----
		vPos = m_pRectTransform.anchoredPosition;
		vPos.y += ((ArchivementShower.s_nActivatedShowerCount - 1) * 125.0f);

		m_pRectTransform.anchoredPosition = vPos;

		//-----
		HT.Utils.SafeDestroy(pNewParticle.gameObject, pNewParticle.TotalSimulationTime());
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void CallDestroy()
	{
		--ArchivementShower.s_nActivatedShowerCount;
		HT.Utils.SafeDestroy(gameObject);
	}
}
