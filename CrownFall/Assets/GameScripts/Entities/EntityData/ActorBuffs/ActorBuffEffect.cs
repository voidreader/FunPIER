using UnityEngine;
using System.Collections;
using HT;

/////////////////////////////////////////
//---------------------------------------
public class ActorBuffEffect : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	public IActorBase m_pOwner;

	/////////////////////////////////////////
	//---------------------------------------
	public ParticleSystem m_pStickyEffect;
	public bool m_bStickyEffect_InheritRotate;


	/////////////////////////////////////////
	//---------------------------------------
	// Use this for initialization
	void Start()
	{
		if (m_pStickyEffect != null)
		{
			m_pStickyEffect = HT.Utils.InstantiateFromPool(m_pStickyEffect);

			m_pStickyEffect.transform.position = m_pOwner.transform.position;

			if (m_bStickyEffect_InheritRotate)
				m_pStickyEffect.transform.rotation = m_pOwner.transform.rotation;
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (m_pStickyEffect != null)
		{
			m_pStickyEffect.transform.position = m_pOwner.transform.position;

			if (m_bStickyEffect_InheritRotate)
				m_pStickyEffect.transform.rotation = m_pOwner.transform.rotation;
		}
	}

	void OnDestroy()
	{
		if (m_pStickyEffect != null)
		{
			m_pStickyEffect.Stop();
			HT.Utils.SafeDestroy(m_pStickyEffect.gameObject, m_pStickyEffect.TotalSimulationTime());
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
}
