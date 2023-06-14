using UnityEngine;
using System.Collections;

public class LevelLighting : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	public Color m_GIAmbient_Color;
	public float m_GIAmbient_Intensity;

	//---------------------------------------
	public UnityEngine.Rendering.DefaultReflectionMode _reflectMode = UnityEngine.Rendering.DefaultReflectionMode.Skybox;
	public Cubemap _customReflectMap = null;

	//---------------------------------------
	public bool _lobby_Fog_Enabled = true;
	public Color m_Lobby_Fog_Color;
	public float m_Lobby_Fog_Start;
	public float m_Lobby_Fog_End;

	//---------------------------------------
	public bool _fog_Enabled = true;
	public Color m_Fog_Color;
	public float m_Fog_Start = 99.0f;
	public float m_Fog_End = 999.0f;

	//---------------------------------------
	public float _globalBloomThreshold = -1.0f;


	/////////////////////////////////////////
	//---------------------------------------
	public void EnableLighting(bool bLobby)
	{
		RenderSettings.ambientSkyColor = m_GIAmbient_Color;
		RenderSettings.ambientIntensity = m_GIAmbient_Intensity;

		//-----
		RenderSettings.defaultReflectionMode = _reflectMode;
		if (_reflectMode == UnityEngine.Rendering.DefaultReflectionMode.Custom)
			RenderSettings.customReflection = _customReflectMap;

		//-----
		bool bFogEnabled = true;
		Color pFogColor = Color.white;
		float fFogStartDist = 0.0f;
		float fFogEndDist = 0.0f;

		if (bLobby)
		{
			bFogEnabled = _lobby_Fog_Enabled;
			pFogColor = m_Lobby_Fog_Color;
			fFogStartDist =  m_Lobby_Fog_Start;
			fFogEndDist =  m_Lobby_Fog_End;
		}
		else
		{
			bFogEnabled = _fog_Enabled;
			pFogColor = m_Fog_Color;
			fFogStartDist = m_Fog_Start;
			fFogEndDist = m_Fog_End;
		}

		RenderSettings.fog = bFogEnabled;
		RenderSettings.fogColor = pFogColor;
		RenderSettings.fogStartDistance = fFogStartDist;
		RenderSettings.fogEndDistance = fFogEndDist;

		//-----
		HT.PostProcessStackHelper.GlobalOption_Bloom_Threshold = _globalBloomThreshold;
	}


	/////////////////////////////////////////
	//---------------------------------------
}
