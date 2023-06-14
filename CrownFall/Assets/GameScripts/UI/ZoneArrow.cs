using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


/////////////////////////////////////////
//---------------------------------------
public class ZoneArrow : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	[SerializeField]
	private Object_AreaAlert.eAreaAlertType _alertType = Object_AreaAlert.eAreaAlertType.Safety;

    //---------------------------------------
    public GameObject m_pArrow;
	public GameObject m_PText;

	//---------------------------------------
	RectTransform m_pMainTransform;
	RectTransform m_pArrowTransform;
	RectTransform m_pTextTransform;


	/////////////////////////////////////////
	//---------------------------------------
	// Use this for initialization
	void Start()
	{
		m_pMainTransform = GetComponent<RectTransform>();

		m_pArrowTransform = m_pArrow.GetComponent<RectTransform>();

		if (m_PText != null)
			m_pTextTransform = m_PText.GetComponent<RectTransform>();
	}

	// Update is called once per frame
	void Update()
	{
		if (BattleFramework._Instance == null)
			return;

		if (BattleFramework._Instance.m_pPlayerActor == null)
			return;

		if (BattleFramework._Instance.m_vAreaAlertMessage == null)
			return;
		
		if (CameraManager._Instance.m_pMainCamera == null)
			return;

		//-----
		Vector3 vAlertPos = Vector3.zero;
		Vector3 vPlayerPos = BattleFramework._Instance.m_pPlayerActor.transform.position;

		bool bEnabled = false;
		float fPrevDistance = 999.0f;
		List<Object_AreaAlert> vAreaAlerts = BattleFramework._Instance.m_vAreaAlertMessage;
		for (int nInd = 0; nInd < vAreaAlerts.Count; ++nInd)
		{
			Object_AreaAlert pAlert = vAreaAlerts[nInd];
			if (pAlert.m_eAlertType == _alertType)
			{
				bEnabled = true;

				float fDistance = Vector3.Distance(pAlert.gameObject.transform.position, vPlayerPos);
				if (fPrevDistance > fDistance)
				{
					fPrevDistance = fDistance;
					vAlertPos = pAlert.gameObject.transform.position;
				}
			}
		}

		//-----
		if (bEnabled)
		{
			Vector3 vDir = vPlayerPos - vAlertPos;
			vDir.x *= -1.0f;
			vDir.y = 0.0f;
			vDir.Normalize();

			Quaternion qRot = m_pArrowTransform.rotation;
			qRot.SetLookRotation(vDir);

			vDir.x = vDir.y = 0.0f;
			vDir.z = qRot.eulerAngles.y;
			qRot.eulerAngles = vDir;

			m_pArrowTransform.rotation = qRot;

			//-----
			float fMessageW = m_pMainTransform.rect.width;
			float fMessageH = m_pMainTransform.rect.height;

			Camera pMainCam = CameraManager._Instance.m_pMainCamera;
			Vector3 vScreenPos = pMainCam.WorldToScreenPoint(vAlertPos);

			if (HT.HTAfxPref.IsMobilePlatform)
				vScreenPos = vScreenPos * 2.0f;

			float fScreenLimitW = (Screen.width) - (fMessageW * 0.5f);
			float fScreenLimitH = (Screen.height) - (fMessageH * 0.5f) - 50.0f;

            float fYMinPos = (HT.HTAfxPref.IsMobilePlatform)? 360.0f : fMessageH * 1.25f;

			vScreenPos.x = Mathf.Clamp(vScreenPos.x, fMessageW * 0.5f, fScreenLimitW);
			vScreenPos.y = Mathf.Clamp(vScreenPos.y, fYMinPos, fScreenLimitH);

			Vector2 vAnchoredPos = m_pArrowTransform.anchoredPosition;
			vAnchoredPos.x = vScreenPos.x;
			vAnchoredPos.y = vScreenPos.y;

			//-----
			m_pArrowTransform.anchoredPosition = vAnchoredPos;

			if (m_PText != null)
				m_pTextTransform.anchoredPosition = vAnchoredPos;
		}

		//-----
		m_pArrow.gameObject.SetActive(bEnabled);

		if (m_PText != null)
			m_PText.gameObject.SetActive(bEnabled);
	}


	/////////////////////////////////////////
	//---------------------------------------
}
