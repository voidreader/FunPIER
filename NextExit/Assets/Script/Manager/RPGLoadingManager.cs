using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RPGLoadingManager : RPGSingleton<RPGLoadingManager>
{
    /*
    public class BundleObject
    {
        public string bundleID;
        public string id;
        public System.Type type;
        BundleObject(string bundleID, string id, System.Type type)
        {
            this.bundleID = bundleID;
            this.id = id;
            this.type = type;
        }
    }
    */
	public GameObject m_LoadingObject;

	private int m_retainCount = 0;
    public int RetainCount { get { return m_retainCount; } }

    /// <summary>
    /// 로딩 시간 계산 용도.
    /// </summary>
    private float m_loadingTime;
    public float LoadingTime { get { return m_loadingTime; } }

    public void startLoading()
	{
		m_retainCount++;
        Debug.Log("startLoading retainCount = " + m_retainCount);
		if (m_retainCount == 1) 
		{
            RPGSceneManager.Instance.LoadingScene.gameObject.SetActive(true);
            m_loadingTime = Time.time;
		}
	}

    /// <summary>
    /// Ends the loading.
    /// </summary>
	public void endLoading()
	{
		m_retainCount--;
        Debug.Log("endLoading retainCount = " + m_retainCount);
        if (m_retainCount == 0)
        {
            RPGSceneManager.Instance.LoadingScene.gameObject.SetActive(false);
            Debug.Log("Total loading Time = " + (Time.time - m_loadingTime));
        }
		if (m_retainCount < 0)
            Debug.LogError("SYLoadingManager retaincount Error : " + m_retainCount);
	}
}

