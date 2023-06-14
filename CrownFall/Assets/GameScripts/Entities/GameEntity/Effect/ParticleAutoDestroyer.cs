using UnityEngine;
using System.Collections;
using HT;

/////////////////////////////////////////
//---------------------------------------
public class ParticleAutoDestroyer : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	public ParticleSystem[] m_vParticles;


	/////////////////////////////////////////
	//---------------------------------------
	private void OnEnable()
	{
		float fMaxLength = 0.0f;
		for (int nInd = 0; nInd < m_vParticles.Length; ++nInd)
		{
			float fTime = m_vParticles[nInd].TotalSimulationTime();
			fMaxLength = Mathf.Max(fMaxLength, fTime);
		}

		HT.Utils.SafeDestroy(gameObject, fMaxLength);
	}

	/////////////////////////////////////////
	//---------------------------------------
}
