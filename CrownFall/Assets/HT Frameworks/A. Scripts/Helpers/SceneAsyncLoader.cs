using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneAsyncLoader : MonoBehaviour {
	/////////////////////////////////////////
	//---------------------------------------
	public float fDelayTime = 1.0f;

	public GameObject m_LoadingGage;
	public GameObject m_LoadingText;

	public string m_szNextLevelName;
	
	AsyncOperation m_Async;
	float m_fLogInRetryMinTime;

	
	/////////////////////////////////////////
	//---------------------------------------
	// Use this for initialization
	void Start ()
	{
		Text procText = m_LoadingText.GetComponent<Text>();
		string szLoadText = HT.HTLocaleTable.GetLocalstring("ui_loading");
		procText.text = string.Format(szLoadText, 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (fDelayTime > 0.0f) {
			fDelayTime -= HT.TimeUtils.GameTime;
			
		} else {
			if (m_Async == null) {
				StartCoroutine (ASyncLoadLevelStart ());
			}
		}
	}

	
	/////////////////////////////////////////
	//---------------------------------------
	IEnumerator ASyncLoadLevelStart () {
		//-----
		Text procText = m_LoadingText.GetComponent<Text>();
		Slider procSlide = m_LoadingGage.GetComponent<Slider> ();
		
		//-----
		m_Async = SceneManager.LoadSceneAsync (m_szNextLevelName);

		string szLoadText = HT.HTLocaleTable.GetLocalstring("ui_loading");
		while (!m_Async.isDone) {
			procText.text = string.Format(szLoadText, (int)(m_Async.progress * 100.0f));
			procSlide.value = m_Async.progress;

			yield return true;
		}
	}
}
