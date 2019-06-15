using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PopupGameClear : RPGLayer
{
    public static PopupGameClear show()
    {
        return RPGSceneManager.Instance.pushPopup<PopupGameClear>("Prefabs/Popup/PopupGameClear");
    }

    float defaultTimeScale = 1.0f;
    GameObject m_LayerRate;
    tk2dUIScrollbar m_ScrollBar;

	public RPGTextMesh textTime;
	public RPGTextMesh textDeath;

    RPGTextMesh m_TextTime;
    RPGTextMesh m_TextDeath;
	public tk2dUIItem buttonExit;
	public tk2dUIItem buttonAgain;
	public tk2dUIItem buttonNext;

	public tk2dSprite sprite_1;
	public tk2dSprite sprite_2;
	public tk2dSprite sprite_3;

	public tk2dBaseSprite bg_black;
	public List<tk2dUIItem> buttomList = new List<tk2dUIItem>();

	public tk2dSprite sptiteImgTitle;

	private bool m_action = false;

    public override void init()
    {
		RPGSoundManager.Instance.StopBgm();

        base.init();
        Transform layerRate = getTransform().Find("LAYER_Rate");
        m_LayerRate = layerRate.gameObject;
        m_TextTime = getTransform().Find("TEXT/Content/text_time").GetComponent<RPGTextMesh>();
        m_TextDeath = getTransform().Find("TEXT/Content/text_death").GetComponent<RPGTextMesh>();
        m_ScrollBar = layerRate.Find("BTN/Slider").GetComponent<tk2dUIScrollbar>();
        m_ScrollBar.Value = 1.0f;

		//*
		int StageNumber =  GameManager.Instance.StageNumber + 1;

		if ( GameManager.Instance.STAGE_LIST.Count < StageNumber )
		{
			buttonExit.transform.localPosition = new Vector3( -200f, -96f, 0f ) / 20f;
			buttonAgain.transform.localPosition = new Vector3( 200f, -96f, 0f ) / 20f;
			buttonNext.gameObject.SetActive( false );
		}
		//		
		defaultTimeScale = Time.timeScale;
		Time.timeScale = 0;

		m_TextDeath.Text = "";
		m_TextTime.Text = "";

		DataSaveManager.Instance.m_questData.DEATH_COUNT = DataSaveManager.Instance.m_questData.DEATH_COUNT + GameManager.Instance.CountDeath;

		DataSaveManager.Instance.SetStageClearInfo( GameManager.Instance.StageNumber, GameManager.Instance.PlayTime, GameManager.Instance.CountDeath );

		DataSaveManager.Instance.CheckQuestListComplete();

		if ( DataSaveManager.Instance.gameRetension == 0  )
		{
			DataSaveManager.Instance.SetGameRetension( RPGDefine.getNowTimeStamp() );
		}
		else if ( RPGDefine.getNowTimeStamp() - DataSaveManager.Instance.gameRetension >= 420 )
		{
			// UnityAnalyticsManager.Instance.CustomEvent( "retension" );
			DataSaveManager.Instance.SetGameRetension( RPGDefine.getNowTimeStamp() );
		}
		else
		{
			DataSaveManager.Instance.SetGameRetension( RPGDefine.getNowTimeStamp() );
		}
		if ( GameManager.Instance.StageNumber - 2 > 0 )
		{
			int score = ( ( GameManager.Instance.StageNumber - 2 ) * 1000 ) - (int)( GameManager.Instance.PlayTime ) - ( GameManager.Instance.CountDeath * 10 );
			if ( score > 0 && score > DataSaveManager.Instance.m_bestScore )
			{
				DataSaveManager.Instance.SetBestScore( score );
			}
		}
		
		StartCoroutine( C_StartAction() );
    }



	private IEnumerator C_StartAction()
	{
		m_action = true;
		m_TextTime.gameObject.SetActive( false );
		m_TextDeath.gameObject.SetActive( false );
		float time = 0.2f;
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
		startColor = new Color( 1f, 1f, 1f, 0f );
		endColor = new Color( 1f, 1f, 1f, 1f );
		sptiteImgTitle.gameObject.SetActive( true );
		while ( true )
		{
			timer += Time.unscaledDeltaTime;
			sptiteImgTitle.color = Color.Lerp( startColor, endColor, timer / time );
			sptiteImgTitle.transform.localScale = Vector3.Lerp( Vector3.zero, Vector3.one, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}
		RPGSoundManager.Instance.PlayUISound( 2 );
		textTime.gameObject.SetActive( true );
		m_TextTime.gameObject.SetActive( true );
		startColor = new Color( 37f / 255f, 230f / 255f, 26f / 255f, 0f );
		endColor = new Color( 37f / 255f, 230f / 255f, 26f / 255f, 1f );
		timer = 0;
		while ( true )
		{
			timer += Time.unscaledDeltaTime;
			textTime.SetColor( Color.Lerp( startColor, endColor, timer / time ) );
			m_TextTime.SetColor( Color.Lerp( startColor, endColor, timer / time ) );
			if ( timer >= time )
				break;
			yield return null;
		}

		textDeath.gameObject.SetActive( true );
		m_TextDeath.gameObject.SetActive( true );
		startColor = new Color( 250f / 255f, 79f / 255f, 137f / 255f, 0f );
		endColor = new Color( 250f / 255f, 79f / 255f, 137f / 255f, 1f );
		timer = 0;
		while ( true )
		{
			timer += Time.unscaledDeltaTime;
			textDeath.SetColor( Color.Lerp( startColor, endColor, timer / time ) );
			m_TextDeath.SetColor( Color.Lerp( startColor, endColor, timer / time ) );
			if ( timer >= time )
				break;
			yield return null;
		}

		timer = 0;
		while ( true )
		{
			timer += Time.unscaledDeltaTime;
			PrintTime( ( int )( ( float )GameManager.Instance.PlayTime * ( timer / time ) ) );
			m_TextDeath.Text = ( ( float )GameManager.Instance.CountDeath * ( timer / time ) ).ToString( "N0" );
			if ( timer >= time )
				break;
			yield return null;
		}
		PrintTime( GameManager.Instance.PlayTime );
		m_TextDeath.Text = GameManager.Instance.CountDeath.ToString();

		startColor = new Color( 1f, 1f, 1f, 0f );
		endColor = new Color( 1f, 1f, 1f, 1f );
		timer = 0;
		while ( true )
		{
			timer += Time.unscaledDeltaTime;
			sprite_1.color = Color.Lerp( startColor, endColor, timer / time );
			sprite_2.color = Color.Lerp( startColor, endColor, timer / time );
			sprite_3.color = Color.Lerp( startColor, endColor, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}

		for ( int index = 0; index < buttomList.Count; ++index )
			buttomList[index].enabled = true;
		m_action = false;

		if ( !GameManager.Instance.Is_review )
		{
			if ( ( GameManager.Instance.StageNumber - 2 ) == 30 )
			{
				PopupReview.show();
			}
			else if ( ( GameManager.Instance.StageNumber - 2 ) > 30 )
			{
				if ( Random.Range( 0, 10 ) % 20 == 0 )
				{
					PopupReview.show();
				}

			}
		}
	}

    void PrintTime( int _playTime )
    {
		int PlayTime = _playTime;

        int hour = 0;
        int min = 0;
        int sec = PlayTime % 60;
        if (PlayTime >= 60)
            min = PlayTime / 60;
        if (PlayTime >= 60 * 60)
            hour = PlayTime / 60 / 60;
        m_TextTime.Text = string.Format("{0}:{1}:{2}", hour.ToString("D2"), min.ToString("D2"), sec.ToString("D2"));
    }

    void OnBtnAgain()
    {
		RPGSoundManager.Instance.StopPlaySound();
		RPGSoundManager.Instance.PlayUISound( 3 );
		m_action = true;
        if (GameManager.Instance.PlayMode == GameManager.ePlayMode.Stage)
        {
			removeFromParent();
			//Time.timeScale = defaultTimeScale;
            // 스테이지 모드 에서는 1스테이지 부터 다시 시작해야 한다.
			GameManager.Instance.StartRestartGameAction( defaultTimeScale );
        }
        else
        {
            removeFromParent();
            Time.timeScale = defaultTimeScale;
			// UnityAnalyticsManager.Instance.CustomEvent( "Again" );
            GameManager.Instance.ReStart(false);
        }
		
		
    }

    void OnBtnReview()
    {
		RPGSoundManager.Instance.StopPlaySound();
		RPGSoundManager.Instance.PlayUISound( 3 );
		m_action = true;
		if ( GameManager.Instance.PlayMode == GameManager.ePlayMode.Stage )
		{
			removeFromParent();
			Time.timeScale = defaultTimeScale;
			// 스테이지 모드 에서는 1스테이지 부터 다시 시작해야 한다.
			// UnityAnalyticsManager.Instance.CustomEvent( "Next" );
			GameManager.Instance.ResetStageInfo();
			GameManager.Instance.StartGameAction();
		}
		else
		{
			removeFromParent();
			Time.timeScale = defaultTimeScale;
			GameManager.Instance.ReStart( false );
		}
		
    }

    void OnBtnExit()
    {
		// UnityAnalyticsManager.Instance.CustomEvent( "GameEnd" );
		RPGSoundManager.Instance.StopPlaySound();
		RPGSoundManager.Instance.PlayUISound( 3 );
		OnPopupClose();
    }

	public override void OnPopupClose()
	{
		if ( m_action )
			return;
		m_action = true;
		Time.timeScale = defaultTimeScale;
		GameManager.Instance.ExitInGameAction();
		
		removeFromParent();
		
	}
}
