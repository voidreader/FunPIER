using UnityEngine;
using System.Collections;

public class SceneAutoSwapper : MonoBehaviour
{
	/////////////////////////////////////////
	//---------------------------------------
	public float m_fSceneSwapTime = 1.0f;
	public string m_szSceneName;

	//---------------------------------------
	private float _leastSwapTime = 0.0f;
	bool m_bCalledSceneChange = false;

	[SerializeField]
	private bool _skipOnEditor = true;


	/////////////////////////////////////////
	//---------------------------------------
	void Start()
	{
		_leastSwapTime = m_fSceneSwapTime;
		if (Application.isEditor && _skipOnEditor)
			_leastSwapTime = 0.0f;
	}

	void FixedUpdate()
	{
		if (m_bCalledSceneChange)
			return;

		_leastSwapTime -= Time.fixedDeltaTime;
		if (_leastSwapTime <= 0.0f)
		{
			m_bCalledSceneChange = true;
			HT.HTFramework.Instance.SceneChange(m_szSceneName);
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
}
