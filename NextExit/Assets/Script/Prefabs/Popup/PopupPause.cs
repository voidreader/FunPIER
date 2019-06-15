using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PopupPause : RPGLayer
{
    public static PopupPause show()
    {
        return RPGSceneManager.Instance.pushPopup<PopupPause>("Prefabs/Popup/PopupPause");
    }

    float defaultTimeScale = 1.0f;

	public tk2dBaseSprite bg_black;
	public Transform tranInfo;
	public List<tk2dUIItem> buttomList = new List<tk2dUIItem>();

	private bool m_action = false;

    public override void init()
    {
        base.init();
        defaultTimeScale = Time.timeScale;
        Time.timeScale = 0;
		StartCoroutine( C_StartAction() );
    }


	private IEnumerator C_StartAction()
	{
		m_action = true;
		float time = 0.1f;
		float timer = 0f;
		for ( int index = 0; index < buttomList.Count; ++index )
			buttomList[index].enabled = false;
		Color startColor = new Color( 0f, 57f / 255f, 1f, 0f );
		Color endColor = new Color( 0f, 57f / 255f, 1f, 104f / 255f );

		while ( true )
		{
			timer += Time.unscaledDeltaTime;
			bg_black.color = Color.Lerp( startColor, endColor, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}
		timer = 0;
		while ( true )
		{
			timer += Time.unscaledDeltaTime;
			tranInfo.localScale = Vector3.Lerp( Vector3.zero, Vector3.one, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}
		for ( int index = 0; index < buttomList.Count; ++index )
			buttomList[index].enabled = true;
		m_action = false;
	}


	private IEnumerator C_EndAction()
	{
		float time = 0.1f;
		float timer = 0f;
		for ( int index = 0; index < buttomList.Count; ++index )
			buttomList[index].enabled = false;
		Color startColor = new Color( 0f, 57f / 255f, 1f, 0f );
		Color endColor = new Color( 0f, 57f / 255f, 1f, 104f / 255f );

		while ( true )
		{
			timer += Time.unscaledDeltaTime;
			tranInfo.localScale = Vector3.Lerp( Vector3.one, Vector3.zero, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}
		timer = 0;
		while ( true )
		{
			timer += Time.unscaledDeltaTime;
			bg_black.color = Color.Lerp( endColor, startColor, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}

		removeFromParent();
		Time.timeScale = defaultTimeScale;
	}

    void OnBtnContinue()
    {
		RPGSoundManager.Instance.PlayUISound( 3 );
		OnPopupClose();
    }

	public override void OnPopupClose()
	{
		if ( m_action )
			return;
		m_action = true;
		StartCoroutine( C_EndAction() );
	}

    void OnBtnExit()
    {
		RPGSoundManager.Instance.PlayUISound( 3 );
        removeFromParent();
		Time.timeScale = defaultTimeScale;
		DataSaveManager.Instance.m_questData.DEATH_COUNT = DataSaveManager.Instance.m_questData.DEATH_COUNT + GameManager.Instance.CountDeath;
		DataSaveManager.Instance.CheckQuestListComplete();
		GameManager.Instance.ExitInGameAction();
		
	}

}
