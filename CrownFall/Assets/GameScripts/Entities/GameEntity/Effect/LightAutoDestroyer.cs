using UnityEngine;
using System.Collections;
using HT;

/////////////////////////////////////////
//---------------------------------------
public class LightAutoDestroyer : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	[Header("QUALITY")]
	public ePlatformObject _enablePlatform = ePlatformObject.EnableAll;
	public int _requireQuality = 0;

	//---------------------------------------
	[Header("LIGHT SETTING")]
	public Light _targetLight = null;
	public bool m_bDecreaseLightRange = false;
	public bool m_bDecreaseLightIntensity = false;

	public float m_fLifeTime = 1.0f;


	/////////////////////////////////////////
	//---------------------------------------
	private float m_fOrig_Range;
	private float m_fOrig_Intensity;

	private float _leastTime = 0.0f;

	Light m_pLight = null;


	/////////////////////////////////////////
	//---------------------------------------
	private void Awake()
	{
		m_pLight = (_targetLight != null)? _targetLight : GetComponent<Light>();

		m_fOrig_Range = m_pLight.range;
		m_fOrig_Intensity = m_pLight.intensity;
	}

	//---------------------------------------
	private void OnEnable()
	{
		_leastTime = m_fLifeTime;
	}

	private void OnDisable()
	{
		_leastTime = m_fLifeTime;
	}

	private void Update()
	{
		_leastTime -= HT.TimeUtils.GameTime;

		if (m_bDecreaseLightRange || m_bDecreaseLightIntensity)
		{
			float fRatio = _leastTime / m_fLifeTime;

			if (m_bDecreaseLightRange)
				m_pLight.range = m_fOrig_Range * fRatio;

			if (m_bDecreaseLightIntensity)
				m_pLight.intensity = m_fOrig_Intensity * fRatio;
		}

		if (_leastTime <= 0.0f)
			HT.Utils.SafeDestroy(gameObject);
	}

	/////////////////////////////////////////
	//---------------------------------------
}
