using UnityEngine;
using System.Collections;

/////////////////////////////////////////
//---------------------------------------
public class LightManager : MonoBehaviour {
	/////////////////////////////////////////
	//---------------------------------------
	public bool m_bEnableLight = true;

	//---------------------------------------
	public Light m_pLight;

	public Color m_pLightColor;
	public float m_fRange;
	public float m_fIntensity;


	/////////////////////////////////////////
	//---------------------------------------
	public void Init () {
		m_bEnableLight = m_pLight.enabled;

		m_pLightColor = m_pLight.color;
		m_fRange = m_pLight.range;
		m_fIntensity = m_pLight.intensity;
	}


	//---------------------------------------
	public void AcceptOriginalData () {
		m_pLight.enabled = m_bEnableLight;

		m_pLight.color = m_pLightColor;
		m_pLight.range = m_fRange;
		m_pLight.intensity = m_fIntensity;
	}


	/////////////////////////////////////////
	//---------------------------------------
}
