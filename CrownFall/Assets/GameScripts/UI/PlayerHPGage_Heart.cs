using UnityEngine;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public class PlayerHPGage_Heart : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	public GameObject m_pHeart_Full;
	public GameObject m_pHeart_Half;

	public GameObject m_pHeartBack_Full;
	public GameObject m_pHeartBack_Half;


	/////////////////////////////////////////
	//---------------------------------------
	public enum eHeartState
	{
		eFull = 0,
		eHalf,
		eBlank,

		eMAX,
	}
	public eHeartState m_eHeartState;

	public enum eEdgeState
	{
		eFull = 0,
		eHalf,

		eMAX,
	}
	public eEdgeState m_eEdgeState;


	/////////////////////////////////////////
	//---------------------------------------
	void Start()
	{
		FixedUpdate();
	}
	
	void FixedUpdate()
	{
		switch (m_eEdgeState)
		{
			case eEdgeState.eFull:
				m_pHeartBack_Full.SetActive(true);
				m_pHeartBack_Half.SetActive(false);
				break;

			case eEdgeState.eHalf:
				m_pHeartBack_Full.SetActive(false);
				m_pHeartBack_Half.SetActive(true);
				break;

			case eEdgeState.eMAX:
				m_pHeartBack_Full.SetActive(false);
				m_pHeartBack_Half.SetActive(false);
				break;
		}

		switch (m_eHeartState)
		{
			case eHeartState.eFull:
				m_pHeart_Full.SetActive(true);
				m_pHeart_Half.SetActive(false);
				break;

			case eHeartState.eHalf:
				m_pHeart_Full.SetActive(false);
				m_pHeart_Half.SetActive(true);
				break;

			case eHeartState.eBlank:
				m_pHeart_Full.SetActive(false);
				m_pHeart_Half.SetActive(false);
				break;
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
	public void SetState(eHeartState eHeart, eEdgeState eEdge)
	{
		if (eHeart != eHeartState.eMAX)
			m_eHeartState = eHeart;

		if (eEdge != eEdgeState.eMAX)
			m_eEdgeState = eEdge;
	}
}
