using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
// TagWindow
public class TagWindow : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	public Button[] m_pvTagButtons;
	public GameObject[] m_pvTagWindows;
	public Selectable[] m_pvTagButtonFirstSelectNavi = null;

	//---------------------------------------
	public Sprite m_pEnableTexture;
	public Sprite m_pDisableTexture;

	public Color m_EnableTextCol;
	public Color m_DisableTextCol;

	public Color m_EnableButtonCol;
	public Color m_DisableButtonCol;

	//---------------------------------------
	int m_nSelectedIndex = 0;

	//---------------------------------------
	Image[] m_vButtonImage;
	Text[] m_vButtonText;


	/////////////////////////////////////////
	//---------------------------------------
	// Use this for initialization
	void Start()
	{
		m_vButtonImage = new Image[m_pvTagButtons.Length];
		m_vButtonText = new Text[m_pvTagButtons.Length];

		for (int nInd = 0; nInd < m_pvTagButtons.Length; ++nInd)
		{
			m_vButtonImage[nInd] = m_pvTagButtons[nInd].GetComponent<Image>();
			m_vButtonText[nInd] = m_pvTagButtons[nInd].GetComponentInChildren<Text>();
		}

		UpdateTagWindowState();
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void UpdateTagWindowState()
	{
		for (int nInd = 0; nInd < m_pvTagButtons.Length; ++nInd)
		{
			bool bEnable = (nInd == m_nSelectedIndex) ? true : false;

			if (m_vButtonImage[nInd] != null)
			{
				m_vButtonImage[nInd].color = (bEnable) ? m_EnableButtonCol : m_DisableButtonCol;
				m_vButtonImage[nInd].sprite = (bEnable) ? m_pEnableTexture : m_pDisableTexture;
			}

			if (m_vButtonText[nInd] != null)
				m_vButtonText[nInd].color = (bEnable) ? m_EnableTextCol : m_DisableTextCol;

			m_pvTagWindows[nInd].gameObject.SetActive(bEnable);

			//-----
			Navigation pNavi = m_pvTagButtons[nInd].navigation;
			pNavi.selectOnDown = m_pvTagButtonFirstSelectNavi[m_nSelectedIndex];
			m_pvTagButtons[nInd].navigation = pNavi;
		}
	}


	//---------------------------------------
	public void ChangeTagWindow(Button pCaller)
	{
		for (int nInd = 0; nInd < m_pvTagButtons.Length; ++nInd)
		{
			if (m_pvTagButtons[nInd] == pCaller)
			{
				m_nSelectedIndex = nInd;
				UpdateTagWindowState();
				break;
			}
		}
	}


	//---------------------------------------
}
