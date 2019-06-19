using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RPG.AntiVariable;

public class GameManager : RPGSingleton<GameManager> {

    const string _Key_StageNumber = "StageNumber";
    const string _Key_CountDeath = "CountDeath";
    const string _Key_PlayTime = "PlayTime";
	const string _key_Review = "Is_review";
	const string _Key_DeathCountAds = "DeathCountAds";
	const string _Key_StageClearCount = "StageClearCount";

    public enum eGameMode
    {
        None,
        InGameReady, // 게임이 시작되었을때 플레이어가 버튼을 누르기전의 대기상태.
        InGame,
    }

    public enum ePlayMode
    {
        UI,             // UI 화면.
        Stage,          // 스테이지 게임 모드.
        Custom,         // 커스텀 게임 모드.
        CustomEditor    // 커스텀 에디터 모드.
    }

    public enum eCustomMode
    {
        Editor,
        Delete
    }

	public Transform PlayLayer;
    public tk2dCamera UICamera;

    tk2dSprite m_spriteBtnLeft;
    tk2dSprite m_spriteBtnRight;
    tk2dSprite m_spriteBtnJump;

    [SerializeField] Transform m_BtnLeft;
    [SerializeField] Transform m_BtnRight;
    [SerializeField] Transform m_BtnJump;

    public List<TextAsset> _listMaps;

    /// <summary>
    /// 케릭터. SceneManager에서 셋팅됨.
    /// </summary>
	public Player player;
    /// <summary>
    /// 왼쪽 이동 버튼. SceneManager에서 셋팅됨.
    /// </summary>
    public Transform BtnLeft 
    { 
        get 
        { 
            return m_BtnLeft; 
        } 
        set
        {
            m_BtnLeft = value;
            m_spriteBtnLeft = m_BtnLeft.GetComponent<tk2dSprite>();
        }
    }
    /// <summary>
    /// 오른쪽 이동 버튼. SceneManager에서 셋팅됨.
    /// </summary>
    public Transform BtnRight
    {
        get
        {
            return m_BtnRight;
        }
        set
        {
            m_BtnRight = value;
            m_spriteBtnRight = m_BtnRight.GetComponent<tk2dSprite>();
        }
    }
    /// <summary>
    /// 점프 버튼. SceneManager에서 셋팅됨.
    /// </summary>
    public Transform BtnJump
    {
        get
        {
            return m_BtnJump;
        }
        set
        {
            m_BtnJump = value;
            m_spriteBtnJump = m_BtnJump.GetComponent<tk2dSprite>();
        }
    }

    public RPGTextMesh TextTime { get; set; }
    public RPGTextMesh TextStage { get; set; }
    public RPGTextMesh TextDeath { get; set; }

    //public tk2dSpriteAnimator Effect_Die { get; set; }
    public RPGLayer EffectLayer { get; set; }


    [SerializeField] bool IsBtnLeft = false;
    [SerializeField] bool IsBtnRight = false;
    [SerializeField] bool IsBtnJump = false;

    //eGameMode m_GameMode = eGameMode.None;

    public eGameMode GameMode { get; set; }
    public eCustomMode CustomMode { get; set; }

    public ePlayMode PlayMode { get; set; }

    /// <summary>
    /// 관리자 모드인지 확인합니다.
    /// 관리자 모드에서의 커스텀맵 제작은 스테이지 제작으로 사용됩니다.
    /// 관리자 모드가 아닌 경우에는 데이터를 나의 슬롯에 추가합니다.
    /// </summary>
    public bool IsAdminMode { get; private set; }

    /// <summary>
    /// 현재 화면에 터치된 블럭의 위치.
    /// </summary>
    Vector2 m_blockPosition;

    /// <summary>
    /// 플레이중.
    /// </summary>
    public bool IsPlaying
    {
        get { return (GameMode == eGameMode.InGame); }
    }

    /// <summary>
    /// 플레이 준비 상태.
    /// </summary>
    public bool IsReady
    {
        get { return (GameMode == eGameMode.InGameReady); }
    }

    /// <summary>
    /// 현재 진행중인 스테이지 번호. 1부터 시작.
    /// </summary>
    public int StageNumber { get; private set; }

    /// <summary>
    /// 스테이지 리스트.
    /// </summary>
    List<ArrayList> StageList = new List<ArrayList>();

	public bool is_Test;

	public List<ArrayList>  STAGE_LIST
	{
		get
		{
			return StageList;
		}
	}

    public HInt32 PlayTime { get; private set; }
    public int TotalStage { get; private set; }
    public HInt32 CountDeath { get; private set; }

	public bool Is_review{ get; private set; }

	public tk2dBaseSprite sptieBlack;

    public override void Init()
    {

        Debug.Log("GameManager Init #1");

		is_Test = false;
        base.Init();
        if (UICamera == null)
            UICamera = Camera.main.GetComponent<tk2dCamera>();

        //GameMode = eGameMode.None;
        //PlayMode = ePlayMode.UI;
        //StageNumber = 0;
        StageNumber = HPlayerPrefs.GetInt(_Key_StageNumber, 0);
        PlayTime = HPlayerPrefs.GetInt(_Key_PlayTime, 0);
        CountDeath = HPlayerPrefs.GetInt(_Key_CountDeath, 0);
		m_deathCountAds = HPlayerPrefs.GetInt( _Key_DeathCountAds, 0 );
		m_gameClearCount = HPlayerPrefs.GetInt( _Key_StageClearCount, 0 );
			//
		Is_review = bool.Parse( HPlayerPrefs.GetString( _key_Review, "false" ) );

        exitInGame();

        Debug.Log("GameManager Init #2");

        // 스테이지 목록을 생성합니다.
        StageList.Clear();

        TextAsset stage = Resources.Load<TextAsset>(GameConfig._StageFilePath);
        // 데이터 복호화.
        string compressData = RPGAesCrypt.Decrypt(stage.text);
        // 데이터 압축해제.
        string stageData = Zipper.UnzipString(compressData);

        Debug.Log("GameManager Init #3");


        // json 변환.
        JSONObject json = new JSONObject(stageData);
        // dictionary 변환.
        ArrayList list = json.ToArray();

        Debug.Log("GameManager Init #4 :: " + list.Count);
        TextAsset map;
        foreach (string str in list) {

            map = null;
            // Debug.Log(str);
            for(int i =0; i<_listMaps.Count;i++) {
                if (_listMaps[i].name == str) {
                    map = _listMaps[i];
                    break;
                }
            }

            if(map != null)
                StageList.Add(BlockManager.MapDataToList(map.text));

            /*
            TextAsset obj = Resources.Load<TextAsset>(GameConfig._SaveFileResPath + str);

            if (obj)
                StageList.Add(BlockManager.MapDataToList(obj.text));
            */
        }



        TotalStage = StageList.Count;
        Debug.Log("GameManager Init #5 TotalStage :: " + TotalStage);
		DataSaveManager.Instance.DataInit();
		
		LoadChar( DataSaveManager.Instance.CharId );
		// AdvertisingManager.Instance.AdInit();
		RPGSceneManager.Instance.InitScene();

		UI_Main_Prisoner.show();

        StartCoroutine("cPlayerTimer");



        Debug.Log("GameManager Init #6");

    }
	public void LoadChar( string _charId )
	{
		if ( player != null )
		{
			GameObject.Destroy( player.gameObject );
			//Animated = null;
		}
		GameObject obj = GameObject.Instantiate( Resources.Load( "Prefabs/Character/" + _charId ) ) as GameObject;
		obj.transform.parent = PlayLayer;
		obj.transform.localScale = Vector3.one;
		obj.transform.localPosition = Vector3.zero;
		player = obj.GetComponent<Player>();
		player.init();
	}
	public void SetReview( bool _review )
	{
		Is_review = _review;
		HPlayerPrefs.SetString( _key_Review, _review.ToString() );
	}


	// Update is called once per frame
    void Update()
    {

        InputControl();
    }

    IEnumerator cPlayerTimer()
    {
        while(true)
        {
            if (GameMode == eGameMode.InGame)
            {
                PrintTime();
                yield return new WaitForSeconds(1.0f);
                PlayTime++;
            }
            else
                yield return null;
        }
    }

    void PrintTime()
    {
        /*
        int hour = 0;
        int min = 0;
        int sec = PlayTime % 60;
        if (PlayTime >= 60)
            min = PlayTime / 60;
        if (PlayTime >= 60 * 60)
            hour = PlayTime / 60 / 60;
        */
        TextTime.Text = RPGDefine.SecToString(PlayTime);
    }

    void InputControl()
    {
        if (PlayMode == ePlayMode.Stage || PlayMode == ePlayMode.Custom)
        {
            if (player == null)
                return;

            bool isMoveLeft = false;
            bool isMoveRight = false;
            bool isJump = false;

            for (int i = 0; i < RPGTouchManager.Instance.Touches.Length; i++)
            {
                RPGTouch touch = RPGTouchManager.Instance.Touches[i];
                if (touch.IsEnable)
                {
                    //Debug.Log(touch.phase);

                    if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase != TouchPhase.Ended)
                    {
                        Transform rayTransform = RPGTouchManager.Instance.RaycastForTransform(touch.position);
                        if (BtnLeft != null && rayTransform == BtnLeft)
                            isMoveLeft = true;
                        else if (BtnRight != null && rayTransform == BtnRight)
                            isMoveRight = true;
                        else if (BtnJump != null && rayTransform == BtnJump)
                            isJump = true;
                    }
                }
            }
            if (isMoveLeft && !IsBtnLeft)
            {
				//RPGSoundManager.Instance.PlayUISound( 3 );
                Debug.Log("OnDownLeft");
                IsBtnLeft = true;
				m_spriteBtnLeft.SetSprite( "ui_ingame_btn_arrow_touch" );
                player.KeyDown(KeyCode.LeftArrow);
            }
            else if (!isMoveLeft && IsBtnLeft)
            {
                Debug.Log("OnUpLeft");
                IsBtnLeft = false;
				m_spriteBtnLeft.SetSprite( "ui_ingame_btn_arrow_idle" );
                player.KeyUp(KeyCode.LeftArrow);
            }
            else if (isMoveRight && !IsBtnRight)
            {
				//RPGSoundManager.Instance.PlayUISound( 3 );
                Debug.Log("OnDownRight");
                IsBtnRight = true;
				m_spriteBtnRight.SetSprite( "ui_ingame_btn_arrow_touch" );
                player.KeyDown(KeyCode.RightArrow);
            }
            else if (!isMoveRight && IsBtnRight)
            {
                Debug.Log("OnUpRight");
                IsBtnRight = false;
				m_spriteBtnRight.SetSprite( "ui_ingame_btn_arrow_idle" );
                player.KeyUp(KeyCode.RightArrow);
            }

            if (isJump && !IsBtnJump)
            {
				//RPGSoundManager.Instance.PlayUISound( 3 );
                Debug.Log("OnDownJump");
                IsBtnJump = true;
				m_spriteBtnJump.SetSprite( "ui_ingame_btn_jump_touch" );
                player.KeyDown(KeyCode.UpArrow);
            }
            else if (!isJump && IsBtnJump)
            {
                Debug.Log("OnUpJump");
                IsBtnJump = false;
				m_spriteBtnJump.SetSprite( "ui_ingame_btn_jump_idle" );
                player.KeyUp(KeyCode.UpArrow);
            }
        }
        else if (PlayMode == ePlayMode.CustomEditor)
        {
            RPGTouch touch = RPGTouchManager.Instance.Touches[0];
            // raycast로 앞에 컬라이더가 있으면 터치가 안되도록 막는다.
            Transform rayTransform = RPGTouchManager.Instance.RaycastForTransform(touch.position);
            if (rayTransform == null || !rayTransform.name.Equals(BlockManager.Instance.CustomBlockLayer.name))
                return;
            //Debug.Log("rayTransform.name = " + rayTransform.name);

            if (touch.IsEnable)
            {
                Debug.Log(touch.ToString());
                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Ended)
                {
                    Vector3 touchPosition = UICamera.ScreenCamera.ScreenToWorldPoint(touch.position) * GameConfig.PixelsPerMeter;
                    //Debug.Log("touchPosition = " + touchPosition.ToString());

                    //Debug.Log(touchPosition.ToString());
                    Vector2 blockPosition = BlockBase.ConvertPositionScreenToBlock(touchPosition.x, touchPosition.y);
                    if (touch.phase == TouchPhase.Began)
                        m_blockPosition = blockPosition;
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        if (blockPosition.x >= 0 && blockPosition.y >= 0 && m_blockPosition.Equals(blockPosition))
                        {
                            if (GameManager.Instance.CustomMode == eCustomMode.Editor)
                            {
                                if (BlockManager.Instance.SelectBlockID.Length > 0)
                                    BlockManager.Instance.createBlock(BlockManager.Instance.SelectBlockID, (int)blockPosition.x, (int)blockPosition.y);
                            }
                            else
                                BlockManager.Instance.deleteBlock((int)blockPosition.x, (int)blockPosition.y);
                        }
                    }                                        
                }

            }
        }
    }

	/// <summary>
	/// 스테이지 모드를 시작합니다.
	/// </summary>
	public void StartStage( )
	{
		PlayMode = ePlayMode.Stage;

		if ( StageNumber > 1 && StageNumber < StageList.Count )
		{
			MessageBox input = MessageBox.show();
			//저장된 게임이 존재합니다.\n이어 하시겠습니까?
			input.setMessage( DefineMessage.getMsg( 30001 ) );
			input.addYesButton( ( m ) =>
			{
				m.removeFromParent();
				StageNumber = StageNumber - 1;
				NextStage();
			} );
			input.addNoButton( ( m ) =>
			{
				m.removeFromParent();
				StageNumber = 0;
				PlayTime = 0;
				CountDeath = 0;

				NextStage();
			} );
			input.addCloseButton();
		}
		else
		{
			StageNumber = 0;
			PlayTime = 0;
			CountDeath = 0;
			NextStage();
		}
	}

    /// <summary>
    /// 스테이지 모드를 시작합니다.
    /// </summary>
    public void StartStage( int _stageNum )
    {
        PlayMode = ePlayMode.Stage;
		/*
		if ( ( _stageNum + 1 ) == StageNumber )
		{
			if ( StageNumber > 1 && StageNumber < StageList.Count )
			{
				MessageBox input = MessageBox.show();
				//저장된 게임이 존재합니다.\n이어 하시겠습니까?
				input.setMessage( DefineMessage.getMsg( 30001 ) );
				input.addYesButton( ( m ) =>
				{
					m.removeFromParent();
					StageNumber = StageNumber - 1;
					StartGameAction();
				} );
				input.addNoButton( ( m ) =>
				{
					m.removeFromParent();
					StageNumber = 0;
					PlayTime = 0;
					CountDeath = 0;
					StartGameAction();
				} );
				input.addCloseButton();
			}
			else
			{
				StageNumber = _stageNum;
				PlayTime = 0;
				CountDeath = 0;
				StartGameAction();
			}
		}
		else
		{
			StageNumber = _stageNum;
			PlayTime = 0;
			CountDeath = 0;
			StartGameAction();
		}
		*/
		StageNumber = _stageNum;
		PlayTime = 0;
		CountDeath = 0;
		StartGameAction();
    }

	public void ResetStageInfo()
	{
		PlayTime = 0;
		CountDeath = 0;
	}

	public void StartGameAction()
	{
		TextDeath.Text = CountDeath.ToString();
		PrintTime();
		StartCoroutine( C_Action() );
	}

	private IEnumerator C_Action()
	{
		float time = 0.3f;
		float timer = 0f;
		sptieBlack.gameObject.SetActive( true );
		Color startColor = new Color( 0f, 0f, 0f, 0f );
		Color endColor = new Color( 0f, 0f, 0f, 1f );

		while ( true )
		{
			timer += Time.deltaTime;

			sptieBlack.color = Color.Lerp( startColor, endColor, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}
		m_gameClearCount++;

        //2018.02 게임클리어 5회마다 동영상 광고 플레이 삭제. 
        /*
		if ( m_gameClearCount >= 5 ) {
			m_gameClearCount = 0;
			AdvertisingManager.Instance.ShowRewardedAd();
		}
        */

		HPlayerPrefs.SetInt( _Key_StageClearCount, m_gameClearCount );
		NextStage();
		timer = 0;
		while ( true )
		{
			timer += Time.deltaTime;

			sptieBlack.color = Color.Lerp( endColor, startColor, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}
		RPGSoundManager.Instance.PlayEffectSound( 6 );
		RPGSoundManager.Instance.PlayBgm( RPGSoundManager.eSoundBGM.GameBGM );
		sptieBlack.gameObject.SetActive( false );
	}

	public void StartRestartGameAction( float _defaultTimeScale )
	{
		StartCoroutine( C_ActionRestart( _defaultTimeScale ) );
	}

	private IEnumerator C_ActionRestart( float _defaultTimeScale )
	{
		float time = 0.3f;
		float timer = 0f;
		sptieBlack.gameObject.SetActive( true );
		Color startColor = new Color( 0f, 0f, 0f, 0f );
		Color endColor = new Color( 0f, 0f, 0f, 1f );

		while ( true )
		{
			timer += Time.unscaledDeltaTime;

			sptieBlack.color = Color.Lerp( startColor, endColor, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}
		ReStart( false );
		timer = 0;
		while ( true )
		{
			timer += Time.unscaledDeltaTime;

			sptieBlack.color = Color.Lerp( endColor, startColor, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}

		Time.timeScale = _defaultTimeScale;
		RPGSoundManager.Instance.PlayEffectSound( 6 );
		RPGSoundManager.Instance.PlayBgm( RPGSoundManager.eSoundBGM.GameBGM );
		sptieBlack.gameObject.SetActive( false );
	}

	public void ShowGameClear()
	{
		PopupGameClear.show();
	}

    /// <summary>
    /// 미션 클리어. 다음스테이지로 전환합니다.
    /// 커스텀 모드에서는 결과화면을 표시합니다.
    /// </summary>
    public void NextStage()
    {
        if (PlayMode == ePlayMode.Stage)
        {
            StageNumber = StageNumber + 1;

			if ( StageNumber == 1 )
				TextStage.Text = "PROLOG";
			else if ( StageNumber == 2 )
				TextStage.Text = "0th EXIT";
			else 
				TextStage.Text = ( StageNumber - 2 ).ToString();

            if (StageList.Count < StageNumber)
            {
                // 마지막 스테이지임. 결과를 출력해야 할듯?
                // 임시로 메인화면으로 보낸다.
                GameManager.Instance.exitInGame();
                UIMain.show();
                return;
            }
            else
            {
                HPlayerPrefs.SetInt(_Key_StageNumber, StageNumber);
                HPlayerPrefs.SetInt(_Key_CountDeath, CountDeath);
                HPlayerPrefs.SetInt(_Key_PlayTime, PlayTime);
                HPlayerPrefs.Save();

                ArrayList mapData = StageList[StageNumber - 1];
                BlockManager.Instance.loadCustom(mapData);
                startInGameReady(PlayMode);
            }
        }
        else
        {
            // 커스텀 맵 플레이 이므로 결과를 출력해야함.
            //ReStart();
            //UICustomMain.show();
            // 클리어 상태로 전환합니다.
            //DataSaveManager.Instance.Clear();
            //GameManager.Instance.exitInGame();
            //UICustomMyPage.show(0, 2);
            // 클리어 팝업을 띄웁니다.
            PopupGameClear.show();
        }
    }

	/// <summary>
	/// 광고 죽음 횟수.
	/// </summary>
	private int m_deathCountAds = 0;
    int m_adsprobability = 0; // 광고 등장 확률

	private int m_gameClearCount = 0;

    /// <summary>
    /// 현재 게임을 재시작 합니다.
    /// </summary>
    public void ReStart(bool IsDeath)
    {
        // 죽어서 리셋되는 경우.
        if (IsDeath) {

            

            CountDeath++;
			m_deathCountAds++;

            Debug.Log("StageNumber/DeathCount :: " + StageNumber + "/" + CountDeath);
            CheckAdPossibility();


            HPlayerPrefs.SetInt( _Key_DeathCountAds, m_deathCountAds );
        }
        else {
            // 처음부터 재시작 되는 경우.
            CountDeath = 0;
            PlayTime = 0;
        }

        TextDeath.Text = CountDeath.ToString();
		PrintTime();
        startInGameReady(PlayMode);
    }

    void CheckAdPossibility() {

        int stagenum = StageNumber - 2;
        int death = CountDeath;

        Debug.Log("Check Ad Possibility!! :: " + stagenum + "/" + death);

        if(stagenum <= 35) {
            if(death % 5 == 0) {
                AdmobManager.main.ShowInterstitial();
            }
        }
        else if(stagenum >35 && stagenum <= 50) {
            if(death % 7 == 0) {
                AdmobManager.main.ShowInterstitial();
            }
        }
        else {
            if (death % 10 == 0) {
                AdmobManager.main.ShowInterstitial();
            }
        }
    }

	/**/
    /// <summary>
    /// 일단 임시로 제작. 게임을 준비중 상태로 바꿉니다.
    /// </summary>    
    public void startInGameReady(ePlayMode playMode)
    {
        RPGSceneManager.Instance.UIScene.removeAllChild();
        RPGSceneManager.Instance.GameScene.gameObject.SetActive(true);

        RPGSceneManager.Instance.PlayLayer.gameObject.SetActive(true);
        RPGSceneManager.Instance.CustomLayer.gameObject.SetActive(false);

        GameMode = eGameMode.InGameReady;
        PlayMode = ePlayMode.UI;

        // 모든 블럭 리셋.
        BlockManager.Instance.resetAllBlock();

        object_01_entry entry_obj = BlockManager.Instance.getEntry();
        Transform entry_trans = entry_obj.transform;
        // -25.8, -10.4
        // -24.55, -9.7
        // 1.25, 0.7
        player.stop();
        player.gameObject.SetActive(false);
        entry_obj.Base.Animated.Play("entry_open");
        entry_obj.Base.Animated.AnimationCompleted = (animator, clip) =>
            {
                player.transform.localPosition = entry_trans.localPosition + new Vector3(0.6f, 0.15f);
                player.IsFlip = false;
				player.myCharacter.TimePaused();
                player.gameObject.SetActive(true);
				player.Animated.gameObject.SetActive( true );
				player.playerBoxCollider.enabled = true;
                entry_obj.Base.Animated.AnimationCompleted = null;
                entry_obj.Base.Animated.Play("entry_close");
                PlayMode = playMode;
            };
    }

    /// <summary>
    /// 게임준비중에서 플레이 상태로 변경합니다.
    /// </summary>
    public void startInGame()
    {
        if (GameMode == eGameMode.InGameReady)
        {
            GameMode = eGameMode.InGame;
            // 모든 블럭 리셋.
            BlockManager.Instance.resetAllBlock();
        }
    }

    /// <summary>
    /// 커스텀 에디터 모드 시작.
    /// </summary>
    public void startCustom(bool IsAdmin)
    {
        RPGSceneManager.Instance.UIScene.removeAllChild();
        RPGSceneManager.Instance.PopupScene.removeAllChild();
        RPGSceneManager.Instance.GameScene.gameObject.SetActive(true);

        IsAdminMode = IsAdmin;
        PlayMode = ePlayMode.CustomEditor;
        CustomMode = eCustomMode.Editor;

        //BlockManager.Instance.clearAll();
        BlockManager.Instance.clearCustomBlock();
        RPGSceneManager.Instance.PlayLayer.gameObject.SetActive(false);
        RPGSceneManager.Instance.CustomLayer.gameObject.SetActive(true);

		//kjh:: ui 변경
        //RPGSceneManager.Instance.CustomButtonControl.ToggleCustomMode.IsOn = true;
    }

    /// <summary>
    /// 인게임이 종료 될때.
    /// </summary>
    public void exitInGame()
    {
        GameMode = eGameMode.None;
        PlayMode = ePlayMode.UI;
        //BlockManager.Instance.resetAllBlock();
        BlockManager.Instance.clearGameBlock();
        RPGSceneManager.Instance.GameScene.gameObject.SetActive(false);
    }
	public void ExitInGameAction()
	{
		StartCoroutine( C_ActionEnd() );
	}
	private IEnumerator C_ActionEnd()
	{
		float time = 0.3f;
		float timer = 0f;
		sptieBlack.gameObject.SetActive( true );
		Color startColor = new Color( 0f, 0f, 0f, 0f );
		Color endColor = new Color( 0f, 0f, 0f, 1f );

		while ( true )
		{
			timer += Time.deltaTime;

			sptieBlack.color = Color.Lerp( startColor, endColor, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}
		GameMode = eGameMode.None;
		PlayMode = ePlayMode.UI;
		//BlockManager.Instance.resetAllBlock();
		BlockManager.Instance.clearGameBlock();
		RPGSceneManager.Instance.GameScene.gameObject.SetActive( false );
		UI_Main_Prisoner.show();
		timer = 0;
		while ( true )
		{
			timer += Time.deltaTime;

			sptieBlack.color = Color.Lerp( endColor, startColor, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}
		sptieBlack.gameObject.SetActive( false );
	}
}
