using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Main_Prisoner : RPGLayer {

	public static UI_Main_Prisoner show()
    {
		return RPGSceneManager.Instance.pushScene<UI_Main_Prisoner>( "Prefabs/UI/UI_Main_Prisoner" );
    }

    GameObject m_btn_CustomEditor;

	public tk2dBaseSprite sptieLock;

	public Transform tranStage;

	public Item_StageInfo_Main stageInfo;

	public GameObject objPlayEffect;

	public Transform tranInfo;


	public GameObject buttonNoads;
    [Tooltip("YSY::쿠폰입력 버튼")]
    public GameObject buttonCoupon;

	private int m_stageNum = -1;

	private int m_stageIndex = 0;

	public List<Item_StageInfo_Main> m_stageInfoList = new List<Item_StageInfo_Main>();

	private Item_StageInfo_Main selectStageInfo = null;

	private bool m_adsOn;

    public override void init()
    {
        //YSY::테스트용 임시.
        //AdvertisingManager.Instance.SetAdsTime(10);

		RPGSoundManager.Instance.PlayBgm( RPGSoundManager.eSoundBGM.MainBGM );
        base.init();
		objPlayEffect.gameObject.SetActive( false );
		sptieLock.gameObject.SetActive( false );
        m_btn_CustomEditor = getTransform().Find("BTN/btn_CustomEditor").gameObject;
#if UNITY_EDITOR
        // 에디터 모드에서만 노출됩니다.
        m_btn_CustomEditor.SetActive(true);
#else
        m_btn_CustomEditor.SetActive(false);
#endif

#if UNITY_IOS
        //YSY::IOS에서는 쿠폰 버튼이 보이지 않도록 한다.
        buttonCoupon.SetActive(false);
		buttonNoads.transform.localPosition = buttonCoupon.transform.localPosition;
#endif

        OnAdsButton();
		SetStageInfo();
    }

	private int m_maxStage = 0;

	private Vector2 m_downPos;
	private Vector2 m_upPos;
	private bool m_down = false;

	void Update()
	{
		InputControl();

		
	}
	
	void InputControl()
	{
		if ( m_moveAction )
			return;
		for ( int i = 0; i < RPGTouchManager.Instance.Touches.Length; i++ )
		{
			RPGTouch touch = RPGTouchManager.Instance.Touches[i];
			if ( touch.IsEnable )
			{
				if ( touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase != TouchPhase.Ended )
				{
					if ( !m_down && touch.phase == TouchPhase.Began )
					{
						Transform rayTransform = RPGTouchManager.Instance.RaycastForTransform( touch.position );
						
						if ( rayTransform != null && rayTransform.tag.Equals( "Main_StageInfo" ) )
						{
							m_downPos = touch.position;
							m_upPos = m_downPos;
							m_down = true;
						}
					}
					else if( m_down )
					{
						float distance = Vector3.Distance( touch.position, m_upPos );
						if ( distance > 5f )
						{
							//tranStage.
							Vector3 pos = Camera.main.ScreenToWorldPoint( m_upPos );
							//pos.x = pos.x;//-tranStage.position.x;
							pos.y = tranStage.position.y;
							pos.z = tranStage.position.z;
							tranInfo.position = pos;
							Vector3 stagePos = tranInfo.localPosition;
							distance = distance * 0.03f;
							stagePos.z = tranStage.localPosition.z;
							if ( m_upPos.x > touch.position.x )
							{
								//if ( ( m_stageIndex + 1 ) >= m_maxStage )
								//	return;

								float poxX = ( m_stageIndex + 1 == 0 ? -364f : -364f - ( ( 364f * 3 ) * ( m_stageIndex + 1 ) ) ) / 20f;
								if ( tranStage.localPosition.x - distance <= poxX )
									return;
								stagePos.x = tranStage.localPosition.x - distance;//tranInfo.localPosition.x;
							}
							else
							{
								//if ( m_stageIndex <= 0 )
								//	return;
								float poxX = ( m_stageIndex - 1 == 0 ? -364f : -364f - ( ( 364f * 3 ) * ( m_stageIndex - 1 ) ) ) / 20f;
								if ( tranStage.localPosition.x + distance >= poxX )
									return;

								stagePos.x = tranStage.localPosition.x + distance;//tranInfo.localPosition.x;
							}
							tranStage.localPosition = stagePos;
						}
						m_upPos = touch.position;
					}
				}
				else if( m_down )
				{
					m_down = false;
					float distance = Vector3.Distance( m_downPos, m_upPos );
					if( distance > 40f )
					{
						if ( m_downPos.x > m_upPos.x )
							OnRClick();
						else
							OnLClick();
					}
				}
			}
		}
	}

	public void OnStageClick( tk2dUIItem _item )
	{
		Item_StageInfo_Main stageInfo = _item.GetComponent<Item_StageInfo_Main>();
		stageInfo.OnClick();
	}

	private void SetStageInfo()
	{
		float posx = 364f;
		int stageInfoIndex = 0;
		/*
		for ( int index = 0; index < GameManager.Instance.STAGE_LIST.Count; ++index )
		{
			GameObject obj = GameObject.Instantiate( stageInfo.gameObject ) as GameObject;
			obj.transform.parent = tranStage;
			obj.transform.localScale = Vector3.one;
			obj.transform.localPosition = new Vector3( ( posx * index ) / 20f, 0f, 0f );
			Item_StageInfo_Main info = obj.GetComponent<Item_StageInfo_Main>();
			info.init( GameManager.Instance.STAGE_LIST[index], index );

			if( DataSaveManager.Instance.GetStageClearInfo( ( index  ) ) != null )
				stageInfoIndex = index;

			m_stageInfoList.Add( info );
			m_stageInfoList[index].gameObject.SetActive( false );
		}
		*/

		for ( int index = 0; index < GameManager.Instance.STAGE_LIST.Count; ++index )
		{
			if ( DataSaveManager.Instance.GetStageClearInfo( ( index ) ) != null )
				stageInfoIndex = index;
		}
		//stageInfoIndex = 0;
		m_maxStage = GameManager.Instance.STAGE_LIST.Count / 3;
		if ( ( float )GameManager.Instance.STAGE_LIST.Count % 3f != 0 )
			m_maxStage = GameManager.Instance.STAGE_LIST.Count / 3 + 1;
		else
			m_maxStage = GameManager.Instance.STAGE_LIST.Count / 3;

		m_stageNum = stageInfoIndex;
		objPlayEffect.gameObject.SetActive( true );

		m_stageIndex = stageInfoIndex / 3;

		for ( int index = 0; index < 9; ++index )
		{
			int stageIndex = ( ( ( m_stageIndex - 1 ) * 3 ) + index );
			GameObject obj = GameObject.Instantiate( stageInfo.gameObject ) as GameObject;
			obj.transform.parent = tranStage;
			obj.transform.localScale = Vector3.one;
			obj.transform.localPosition = new Vector3( ( posx * stageIndex ) / 20f, 0f, 0f );
			Item_StageInfo_Main info = obj.GetComponent<Item_StageInfo_Main>();
			m_stageInfoList.Add( info );
			info.init( stageIndex );
		}

		Vector3 endPos = tranStage.transform.localPosition;

		endPos.x -= ( ( 364f * 3 ) * m_stageIndex ) / 20f;
		tranStage.transform.localPosition = endPos;

		stageInfo.gameObject.SetActive( false );

	}

    /// <summary>
    /// 스테이지 미션 시작.
    /// </summary>
    void OnBtnStart()
    {
		RPGSoundManager.Instance.PlayUISound( 1 );
		if ( m_stageNum >= 0 )
			StartGame();
		//	StartCoroutine( C_Action() );
    }

	private IEnumerator C_Action()
	{
		float time = 0.5f;
		float timer = 0f;
		sptieLock.gameObject.SetActive( true );
		Color startColor = new Color( 0f, 0f, 0f, 0f );
		Color endColor = new Color( 0f, 0f, 0f, 1f );
		while( true )
		{
			timer += Time.deltaTime;

			sptieLock.color = Color.Lerp( startColor, endColor, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}
		GameManager.Instance.StartStage( m_stageNum );
	}

    void OnBtnOption()
    {
		
		RPGSoundManager.Instance.PlayUISound( 3 );
		Popup_Option_Prisoner.show();
    }

    void OnBtnCustomEditor()
    {
        UIEditorStage.show();
    }

	public void OnBtnWork()
	{
		
#if UNITY_EDITOR
#elif UNITY_ANDROID
		AndroidManager.GetInstance.ShowAchievementsUI();
#elif UNITY_IOS
        //YSY::추후에는 소셜클래스를 생성해서 처리 할 것.
        if (Social.localUser.authenticated == false)
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    // Sign In 성공
                    // 바로 리더보드 UI 표시 요청
                    Social.ShowAchievementsUI();
                    return;
                }
            });
        }
        else
            Social.ShowAchievementsUI();

#endif

    }

	public void OnBtnNoads()
	{

		
	}

	public void OnAdsButton() {


	}

	public void OnStageClick( Item_StageInfo_Main _stageNum )
	{
		if ( m_moveAction )
			return;
		if ( m_stageNum == _stageNum.stageNum )
		{
			RPGSoundManager.Instance.PlayUISound( 1 );
			StartGame();
			return;
		}
		RPGSoundManager.Instance.PlayUISound( 3 );
		if ( selectStageInfo != null )
		{
			selectStageInfo.StageSelect( false );


		}
		m_stageNum = _stageNum.stageNum;
		selectStageInfo = _stageNum;
		_stageNum.StageSelect( true );
		objPlayEffect.gameObject.SetActive( true );
	}

    /// <summary>
    /// 게임 시작 
    /// </summary>
	private void StartGame()
	{
		

		if ( m_stageNum == 11 && !DataSaveManager.Instance.CheckAnalytics( "StartStage10" ) )
		{
			DataSaveManager.Instance.SetAnalytics( "StartStage10" );
		
		}
		else if ( m_stageNum == 21 && !DataSaveManager.Instance.CheckAnalytics( "StartStage20" ) )
		{
			DataSaveManager.Instance.SetAnalytics( "StartStage20" );
			
		}
		else if ( m_stageNum == 31 && !DataSaveManager.Instance.CheckAnalytics( "StartStage30" ) )
		{
			DataSaveManager.Instance.SetAnalytics( "StartStage30" );
			
		}
		else if ( m_stageNum == 41 && !DataSaveManager.Instance.CheckAnalytics( "StartStage40" ) )
		{
			DataSaveManager.Instance.SetAnalytics( "StartStage40" );
			
		}
		else if ( m_stageNum == 51 && !DataSaveManager.Instance.CheckAnalytics( "StartStage50" ) )
		{
			DataSaveManager.Instance.SetAnalytics( "StartStage50" );
			
		}
		else if ( m_stageNum == 61 && !DataSaveManager.Instance.CheckAnalytics( "StartStage60" ) )
		{
			DataSaveManager.Instance.SetAnalytics( "StartStage60" );
			
		}
		else if ( m_stageNum == 71 && !DataSaveManager.Instance.CheckAnalytics( "StartStage70" ) )
		{
			DataSaveManager.Instance.SetAnalytics( "StartStage70" );
			
		}
		else if ( m_stageNum == 81 && !DataSaveManager.Instance.CheckAnalytics( "StartStage80" ) )
		{
			DataSaveManager.Instance.SetAnalytics( "StartStage80" );
			
		}
		else if ( m_stageNum == 91 && !DataSaveManager.Instance.CheckAnalytics( "StartStage90" ) )
		{
			DataSaveManager.Instance.SetAnalytics( "StartStage90" );
			
		}
		else if ( m_stageNum == 101 && !DataSaveManager.Instance.CheckAnalytics( "StartStage100" ) )
		{
			DataSaveManager.Instance.SetAnalytics( "StartStage100" );
			
		}

		GameManager.Instance.StartStage( m_stageNum );
	}

	private bool m_moveAction = false;

	public void OnRClick()
	{
		/*
		if ( ( m_stageIndex + 1 ) >= m_maxStage )
			return;
		if ( tranStage.transform.localPosition.x <= -( ( 364f * ( GameManager.Instance.STAGE_LIST.Count - 2 ) ) / 20f ) )
			return;
		*/
		if ( m_moveAction )
			return;
		m_moveAction = true;
		StartCoroutine( C_StageMoveAction( true ) );
	}

	public void OnLClick()
	{
		/*
		if ( m_stageIndex <= 0 )
			return;
		if ( tranStage.transform.localPosition.x >= -364f / 20f )
			return;
		*/
		if ( m_moveAction )
			return;
		m_moveAction = true;
		StartCoroutine( C_StageMoveAction( false ) );
	}

	public void OnRankCliock()
	{
		

	}

	private IEnumerator C_StageMoveAction( bool _r )
	{
		if ( selectStageInfo != null )
		{
			objPlayEffect.gameObject.SetActive( false );
			selectStageInfo.StageSelect( false );
			m_stageNum = -1;
			selectStageInfo = null;
		}
		float time = 0.3f;
		float timer = 0f;

		Vector3 startPos = tranStage.transform.localPosition;
		//startPos.x = ( m_stageIndex == 0 ? -364f : -364f - ( ( 364f * 3 ) * m_stageIndex ) )/ 20f;
		Vector3 endPos = startPos;
		endPos.x = ( m_stageIndex == 0 ? -364f : -364f - ( ( 364f * 3 ) * m_stageIndex ) ) / 20f;
		
		Vector3 pos = startPos;
		pos.x = ( m_stageIndex == 0 ? -364f : -364f - ( ( 364f * 3 ) * m_stageIndex ) ) / 20f;

		if ( !_r )
		{
			m_stageIndex--;
			endPos.x += ( 364f * 3 ) / 20f;
		}
		else
		{
			m_stageIndex++;
			endPos.x -= ( 364f * 3 ) / 20f;
		}
		time = 0.3f * ( Vector3.Distance( startPos, endPos ) /  Vector3.Distance( pos, endPos ) );
		while( true )
		{
			timer += Time.deltaTime;
			float t = timer / time;
			tranStage.transform.localPosition = Vector3.Lerp( startPos, endPos, t );
			if ( timer >= time )
				break;
			yield return null;
		}

		float posx = 364f;
		if ( m_stageIndex >= GameManager.Instance.STAGE_LIST.Count / 3 )
		{
			m_stageIndex = 0;
			tranStage.transform.localPosition = new Vector3( -364f / 20f, 0f, -2f / 20f );
			for ( int index = 0; index < m_stageInfoList.Count; ++index )
			{
				int stageIndex = ( ( ( m_stageIndex - 1 ) * 3 ) + index );
				m_stageInfoList[index].transform.localPosition = new Vector3( ( posx * stageIndex ) / 20f, 0f, 0f );
				m_stageInfoList[index].init( stageIndex );
			}
		}
		else if ( m_stageIndex < 0 )
		{
			m_stageIndex = ( GameManager.Instance.STAGE_LIST.Count - 1 ) / 3;
			tranStage.transform.localPosition = new Vector3( -364f / 20f, 0f, -2f / 20f );
			Vector3 newendPos = tranStage.transform.localPosition;

			newendPos.x -= ( ( 364f * 3 ) * m_stageIndex ) / 20f;
			tranStage.transform.localPosition = newendPos;

			for ( int index = 0; index < m_stageInfoList.Count; ++index )
			{
				int stageIndex = ( ( ( m_stageIndex - 1 ) * 3 ) + index );
				m_stageInfoList[index].transform.localPosition = new Vector3( ( posx * stageIndex ) / 20f, 0f, 0f );
				m_stageInfoList[index].init( stageIndex );
			}
		}
		else
		{
			if ( _r )
			{
				for ( int index = 0; index < 3; ++index )
				{
					int stageIndex = ( ( ( m_stageIndex + 1 ) * 3 ) + index );
					m_stageInfoList[index].transform.localPosition = new Vector3( ( posx * stageIndex ) / 20f, 0f, 0f );
					m_stageInfoList[index].init( stageIndex );
				}
				m_stageInfoList.Sort( StageInfoSort );
			}
			else
			{
				for ( int index = 0; index < 3; ++index )
				{
					int stageIndex = ( ( ( m_stageIndex - 1 ) * 3 ) + index );
					m_stageInfoList[( m_stageInfoList.Count - 1 ) - index].transform.localPosition = new Vector3( ( posx * stageIndex ) / 20f, 0f, 0f );
					m_stageInfoList[( m_stageInfoList.Count - 1 ) - index].init( stageIndex );
				}
				m_stageInfoList.Sort( StageInfoSort );
			}
		}
		m_moveAction = false;
	}

	private int StageInfoSort( Item_StageInfo_Main _stageInfo_1, Item_StageInfo_Main _stageInfo_2 )
	{
		return _stageInfo_1.m_index.CompareTo( _stageInfo_2.m_index );
	}

	public void OnCouponClick()
	{
		PopupCoupon.show();
	}

    public void CheckNoAds() {
        
    }
}