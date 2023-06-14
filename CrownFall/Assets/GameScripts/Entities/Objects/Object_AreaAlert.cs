using UnityEngine;
using System.Collections;
using LlockhamIndustries.Decals;
using HT;


/////////////////////////////////////////
//---------------------------------------
public class Object_AreaAlert : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	public enum eAreaAlertType
	{
		Warnning = 0,
		Safety,
		Interaction,

		Max,
	}
	public eAreaAlertType m_eAlertType;


	/////////////////////////////////////////
	//---------------------------------------
	[SerializeField]
	private ProjectionRenderer[] _alertObjects = null;
	private Color[] _alertDefColor = null;
	[SerializeField]
	private Transform _timeObject = null;
	[SerializeField]
	private bool _timeObject_ScaleAll = true;

	public GameObject m_pRoot;

	//---------------------------------------
	public Vector3 _defaultScale = Vector3.one;


	/////////////////////////////////////////
	//---------------------------------------
	private float m_fLifeTime;
	private float m_fRunningTime;

	private bool m_bInitialized = false;

	//---------------------------------------
	static readonly float s_fBlendingTime = 0.2f;


	/////////////////////////////////////////
	//---------------------------------------
	void Awake()
	{
		//-----
		_alertDefColor = new Color[_alertObjects.Length];
		for (int nInd = 0; nInd < _alertObjects.Length; ++nInd)
			_alertDefColor[nInd] = _alertObjects[nInd].Properties[0].color;

		if (_timeObject != null)
			_timeObject.localScale = Vector3.zero;

		//-----
		FixedUpdate();
	}

	//---------------------------------------
	void FixedUpdate()
	{
		//-----
		if (m_bInitialized)
			m_fRunningTime += Time.fixedDeltaTime;
		else
			m_bInitialized = true;

		//-----
		if (m_fRunningTime > m_fLifeTime)
		{
			Release();
			HT.Utils.SafeDestroy(gameObject);
		}
		else
		{
			if (_timeObject != null)
			{
				Vector3 vScale = Vector3.zero;
				if (_timeObject_ScaleAll)
					vScale = Vector3.one * (m_fRunningTime / m_fLifeTime);
				else
				{
					float fScale = m_fRunningTime / m_fLifeTime;
					vScale = new Vector3(fScale, 1.0f, 1.0f);
				}

				if (vScale.IsNaN())
					vScale = Vector3.zero;
				else
					vScale.z = 1.0f;

				_timeObject.localScale = vScale;
			}

			//-----
			if (_alertObjects != null && _alertObjects.Length > 0)
			{
				float fBlendRatio = 1.0f / s_fBlendingTime;
				float fRatio = 1.0f;

				if (m_fRunningTime <= s_fBlendingTime)
					fRatio = m_fRunningTime * fBlendRatio;

				else if (m_fRunningTime >= (m_fLifeTime - s_fBlendingTime))
					fRatio = Mathf.Max((m_fLifeTime - m_fRunningTime) * fBlendRatio, 0.0f);

				//-----
				for (int nInd = 0; nInd < _alertObjects.Length; ++nInd)
				{
					Color pColor = _alertDefColor[nInd];
					pColor.a = pColor.a * fRatio;

					_alertObjects[nInd].SetColor(0, pColor);
					_alertObjects[nInd].UpdateProperties();
				}
			}
		}
	}

    //---------------------------------------
    private void OnDisable()
    {
        Release();
    }

    /////////////////////////////////////////
    //---------------------------------------
    public void Init(float fLifeTime)
	{
		m_bInitialized = false;

		m_fRunningTime = 0.0f;
		m_fLifeTime = fLifeTime;

		FixedUpdate();
	}

	public void Release()
	{
		if (BattleFramework._Instance != null)
			BattleFramework._Instance.RemoveAreaAlertMessage(this);
	}
}
