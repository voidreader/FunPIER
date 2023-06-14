using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public class UpgradeButton : MonoBehaviour {
	/////////////////////////////////////////
	//---------------------------------------
	public PlayerData.ePlayerUpgrades m_eUpgradeType;


	/////////////////////////////////////////
	//---------------------------------------
	public GameObject m_pControlTarget;
	public Animation _upgradeEffectAnim = null;

	//---------------------------------------
	RectTransform m_pTargetRectTransform;
	float m_fTargetOriginalX;

	//---------------------------------------
	RectTransform m_pRectTransform;
	Image m_pImage;
	Button m_pButton;

	//---------------------------------------
	bool m_bEnabledYet = false;


	/////////////////////////////////////////
	//---------------------------------------
	void Start () {
		m_pRectTransform = GetComponent<RectTransform> ();

		//-----
		m_pImage = GetComponent<Image> ();
		m_pButton = GetComponent<Button> ();

		//-----
		m_pImage.enabled = false;
		m_pButton.enabled = false;

		//-----
		if (m_pControlTarget != null) {
			m_pTargetRectTransform = m_pControlTarget.GetComponent<RectTransform> ();
			m_fTargetOriginalX = m_pTargetRectTransform.anchoredPosition.x;
		}
	}

	void Update () {
		int nUpgradePoint = GameFramework._Instance.m_pPlayerData.m_nUpgradePoint;
		PlayerData pData = GameFramework._Instance.m_pPlayerData;

		if (nUpgradePoint >= pData.m_vUpgrades [(int)m_eUpgradeType] + 1) {
			if (m_bEnabledYet == false) {
				m_bEnabledYet = true;

				//-----
				m_pImage.enabled = true;
				m_pButton.enabled = true;

				//-----
				if (m_pControlTarget != null) {
					float fPosX = m_fTargetOriginalX + m_pRectTransform.rect.width;

					Vector2 vPos = m_pTargetRectTransform.anchoredPosition;
					vPos.x = fPosX;

					m_pTargetRectTransform.anchoredPosition = vPos;
				}
			}
			
		} else {
			if (m_bEnabledYet) {
				m_bEnabledYet = false;

				//-----
				m_pImage.enabled = false;
				m_pButton.enabled = false;

				//-----
				Vector2 vPos = m_pTargetRectTransform.anchoredPosition;
				vPos.x = m_fTargetOriginalX;

				m_pTargetRectTransform.anchoredPosition = vPos;
			}
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void OnUpgrade () {
		PlayerData pData = GameFramework._Instance.m_pPlayerData;

		if (pData.m_nUpgradePoint >= pData.m_vUpgrades [(int)m_eUpgradeType] + 1) {
			pData.m_nUpgradePoint -= pData.m_vUpgrades [(int)m_eUpgradeType] + 1;
			pData.m_vUpgrades [(int)m_eUpgradeType] += 1;
		}

		if (_upgradeEffectAnim != null)
			_upgradeEffectAnim.Play();

		EventSystem.current.SetSelectedGameObject(null);
	}
}
