using UnityEngine;
using UnityEngine.SocialPlatforms;
using System;
using System.Collections;
using System.Collections.Generic;


public class AndroidManager  
{
    //Gpgs 사용을 원할시에는  항상 이닛을 먼저 한후 로그인을 진행해야 한다
    //로그인을 하지 않을시에 모든기능 사용 불가 (업적, 클라우드 저장, 랭킹)
    //Googld Admob Api https://developers.google.com/admob/android/games#unity&_adc=ap-ko-ha-bk

    private static AndroidManager MyInstance;
    public static  AndroidManager GetInstance
    {
        get
        {
            if(MyInstance == null)
            {
                MyInstance = new AndroidManager();
            }
            return MyInstance;
        }
    }


    bool isBillInit = false;
    string LEADERBOARD_ID = "CgkIj42RhZQEEAIQIw";
    

    /// <summary>
    /// GPGS 이닛 및 안드로이드 매니져 기본 셋팅
    /// </summary>
    public void Init()
    {
        Debug.Log("Android Manager Init");

        // billing
        InitBilling();

        GPGSInit();

        // 유저 검색. 
        //Social.LoadUsers()



    }

    #region 빌링 
    void InitBilling() {


        Debug.Log("InitBilling!");

        //listening for Purchase and consume events
        AndroidInAppPurchaseManager.ActionProductPurchased += OnProductPurchased;

        AndroidInAppPurchaseManager.ActionProductConsumed += AndroidInAppPurchaseManager_ActionProductConsumed; 

        //listening for store initialising finish
        AndroidInAppPurchaseManager.ActionBillingSetupFinished += OnBillingConnected;

        //you may use loadStore function without parameter if you have filled base64EncodedPublicKey in plugin settings
        
        AndroidInAppPurchaseManager.Client.AddProduct("nextexit_001");

        // AndroidInAppPurchaseManager.Client.Connect();
        AndroidInAppPurchaseManager.Client.Connect("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA2I3hKCmqcU5f5DgLRNbFTAncUXw4TcKWjEro1syD2al2qSieM4L95tqI/W7byfhgwSEAaFCU3pP4hbgGVui+HSmLM+5aQ519DkTPNjzYS4eQUrVK5FoPxm625aMHXU/P8QT3R7YcMJAxd8puqAy86Y8fLV/owQhDDCmTJWocOsebjgqdWVnObeJdg7oloxcduqioAzX3NflbywFpWMnnGyxrY5xMuro8HVB0vwNkKZBGfxWuX+56CCdm85QPPjseLIdOfQQkZubPAbzz5iCXwaiC/F2/PM41VYZWw9M4t9/hv4RDYqsDUIoBSNDOHGHARIoxkKdg9j/QwqpzycVICQIDAQAB");


    }



    private void OnBillingConnected(BillingResult result) {
        AndroidInAppPurchaseManager.ActionBillingSetupFinished -= OnBillingConnected;
        Debug.Log("Connection Response: " + result.Response.ToString() + " " + result.Message);

        if (result.IsSuccess) {
            AndroidInAppPurchaseManager.ActionRetrieveProducsFinished += OnRetrieveProductsFinised;

            //Store connection is Successful. Next we loading product and customer purchasing details
            AndroidInAppPurchaseManager.Client.RetrieveProducDetails();
        }

        // AndroidMessage.Create("Response", result.Response.ToString() + " " + result.Message);
        Debug.Log("Response: " + result.Response.ToString() + " " + result.Message);

        
    }


    /// <summary>
    /// 인벤토리 받아오기 완료
    /// </summary>
    /// <param name="result"></param>
    private void OnRetrieveProductsFinised(BillingResult result) {

        AndroidInAppPurchaseManager.ActionRetrieveProducsFinished -= OnRetrieveProductsFinised;

        if (result.IsSuccess) {

            isBillInit = true;
            // AndroidMessage.Create("Success", "Billing init complete inventory contains: " + AndroidInAppPurchaseManager.Client.Inventory.Purchases.Count + " products");
            Debug.Log("Billing init complete inventory contains :: " + AndroidInAppPurchaseManager.Client.Inventory.Purchases.Count);

            foreach (GoogleProductTemplate tpl in AndroidInAppPurchaseManager.Client.Inventory.Products) {
                Debug.Log(tpl.Title);
                Debug.Log(tpl.OriginalJson);
            }

            //구매실패 체크 
            if(AndroidInAppPurchaseManager.Client.Inventory.IsProductPurchased("nextexit_001")) {
                AndroidInAppPurchaseManager.Client.Consume("nextexit_001");
            }

        }
        else {
            // AndroidMessage.Create("Connection Response", result.Response.ToString() + " " + result.Message);
        }

        Debug.Log("Connection Response: " + result.Response.ToString() + " " + result.Message);

    }


    /// <summary>
    /// Purchased 
    /// </summary>
    /// <param name="result"></param>
    private void OnProductPurchased(BillingResult result) {
        if (result.IsSuccess) {
            // AndroidMessage.Create("Product Purchased", result.Purchase.SKU + "\n Full Response: " + result.Purchase.OriginalJson);
            AndroidMessage.Create("Product Purchased", DefineMessage.getMsg(40002));
            OnProcessingPurchasedProduct(result.Purchase);
        }
        else {
            AndroidMessage.Create("Product Purchase Failed", result.Response.ToString() + " " + result.Message);
        }

        Debug.Log("Purchased Response: " + result.Response.ToString() + " " + result.Message);
    }


    //실제 구매 처리 후 소비
    public void OnProcessingPurchasedProduct(GooglePurchaseTemplate purchase) {

        Debug.Log(">>> OnProcessingPurchasedProduct :: " + purchase.SKU);

        switch (purchase.SKU) {
            case "nextexit_001":
                Debug.Log(">>> Call Consume :: " + purchase.SKU);

                AndroidInAppPurchaseManager.Client.Consume(purchase.SKU);
                break;
        }
    }


    private void AndroidInAppPurchaseManager_ActionProductConsumed(BillingResult obj) {

        Debug.Log(">>> AndroidInAppPurchaseManager_ActionProductConsumed :: " + obj.Purchase.SKU);

        OnProcessingConsumeProduct(obj.Purchase);
    }

    // 소비 처리 
    public void OnProcessingConsumeProduct(GooglePurchaseTemplate purchase) {

        Debug.Log(">>> OnProcessingConsumeProduct :: " + purchase.SKU);

        AdvertisingManager.Instance.SetAdsOn(false);
        UI_Main_Prisoner t = GameObject.FindObjectOfType<UI_Main_Prisoner>();
        t.CheckNoAds();

    }


    public void OnConsumeEditor() {
        AdvertisingManager.Instance.SetAdsOn(false);
        UI_Main_Prisoner t = GameObject.FindObjectOfType<UI_Main_Prisoner>();
        t.CheckNoAds();
    }



    #endregion

    /// <summary>
    /// 업적 달성 관련 메소드 
    /// 업적 달성단계가 없는 업적들은 무조건 100f로 해놓아야 업적 달성이 됨
    /// 업적 달성단계가 있는 업적들은 자신이 설정해놓은 달성단계에 따라 Progress값을 입력해 주어야 함. __ 이전 수치에 증가 되는 방식이 아니라 자신이 입력한 값으로 덮어 씌워짐 __ 이전수치보다 낮은값 입력시 적용 안됨
    /// </summary>
    /// <param name="achievementID_">적용할 업적 ID</param>
    /// <param name="progress_">업적 달성도 수치</param>
    /// <param name="TestCallBack_">콜백</param>
    public void AchievementStepUp(string achievementID_) {
        // PlayGamesPlatform.Instance.ReportProgress(achievementID_, progress_, TestCallBack_);
        GooglePlayManager.Instance.UnlockAchievementById(achievementID_);
    }
    /// <summary>
    /// GooglePlayGameService Init
    /// </summary>
    private void GPGSInit()  {

        


        GooglePlayConnection.ActionConnectionResultReceived += ActionConnectionResultReceived;
        GooglePlayConnection.Instance.Connect(); 
    }

    private void ActionConnectionResultReceived(GooglePlayConnectionResult result) {

        GooglePlayConnection.ActionConnectionResultReceived -= ActionConnectionResultReceived;

        if (result.IsSuccess) {
            Debug.Log("Connected!");
            GooglePlayManager.ActionAchievementsLoaded += OnAchivmentsLoaded;
            GooglePlayManager.Instance.LoadAchievements();

            GooglePlayManager.ActionLeaderboardsLoaded += OnLeaderBoardsLoaded;
            GooglePlayManager.Instance.LoadLeaderBoards();

        }
        else {
            Debug.Log("Cnnection failed with code: " + result.code.ToString());
        }
    }

    private void OnAchivmentsLoaded(GooglePlayResult result) {
        GooglePlayManager.ActionAchievementsLoaded -= OnAchivmentsLoaded;
        if (result.IsSucceeded) {
            /*
            foreach (string achievementId in GooglePlayManager.Instance.Achievements) {
                GPAchievement achievement = GooglePlayManager.Instance.GetAchievement(achievementId);
                Debug.Log(achievement.Id);
                Debug.Log(achievement.Name);
                Debug.Log(achievement.Description);
                Debug.Log(achievement.Type);
                Debug.Log(achievement.State);
                Debug.Log(achievement.CurrentSteps);
                Debug.Log(achievement.TotalSteps);
            }
            */

            Debug.Log("Total Achievements:" + GooglePlayManager.Instance.Achievements.Count.ToString());

        }
        else {
            // AndroidNative.showMessage("Achievements Loaded error: ", result.Message);
        }
    }


    private void OnLeaderBoardsLoaded(GooglePlayResult result) {
        GooglePlayManager.ActionLeaderboardsLoaded -= OnLeaderBoardsLoaded;
        if (result.IsSucceeded) {
            if (GooglePlayManager.Instance.GetLeaderBoard(LEADERBOARD_ID) == null) {
                Debug.Log("Leader boards loaded not found in leader boards list");
                return;
            }

            GPLeaderBoard leaderboard = GooglePlayManager.Instance.GetLeaderBoard(LEADERBOARD_ID);
            long score = leaderboard.GetCurrentPlayerScore(GPBoardTimeSpan.ALL_TIME, GPCollectionType.GLOBAL).LongScore;

            Debug.Log(LEADERBOARD_ID + "  score" + score.ToString());
        }
        else {
            AndroidMessage.Create("Leader-Boards Loaded error: ", result.Message);
        }
    }


    /// <summary>
    /// 업적 UI 띄우기
    /// </summary>
    public void ShowAchievementsUI()
    {
        // 로그인 되어있지 않은경우.
        if (GooglePlayConnection.State != GPConnectionState.STATE_CONNECTED) {
            GPGSInit();
            return;
        }

        GooglePlayManager.Instance.ShowAchievementsUI();
    }
    /// <summary>
    /// 리더보드 UI 띄우기
    /// </summary>
    public void ShowLeaderboardUI()
    {
        // 로그인 되어있지 않은경우.
        if (GooglePlayConnection.State != GPConnectionState.STATE_CONNECTED) {
            GPGSInit();
            return;
        }


        GooglePlayManager.Instance.ShowLeaderBoardsUI();
    }
	//#########################################################################################################################

	public void LoadScore()
	{
		//PlayGamesPlatform.Instance.HandleLoadingScores
		//m_User.
		//Social.LoadScores( "CgkInNXfh_kLEAIQHw", Loads );

	}
	public void Loads( IScore[] _score )
	{
		for ( int index = 0; index < _score.Length; ++index )
		{
			Debug.Log( "_score[" + index + "].value : " + _score[index].value );
		}
	}

	public void OpenSavedGame( bool bSave )
	{
        /*
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		if ( bSave )
			savedGameClient.OpenWithAutomaticConflictResolution( "userData", DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedToSave ); //저장루틴진행
		else
			savedGameClient.OpenWithAutomaticConflictResolution( "userData", DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedToRead ); //로딩루틴 진행
        */
	}

    void OnSavedGameOpenedToSave() {
        //void OnSavedGameOpenedToSave( SavedGameRequestStatus status, ISavedGameMetadata game )	{

        /*
		if ( status == SavedGameRequestStatus.Success )
		{
			// handle reading or writing of saved game.
			//파일이 준비되었습니다. 실제 게임 저장을 수행합니다.
			//저장할데이터바이트배열에 저장하실 데이터의 바이트 배열을 지정합니다.
			string data = AdvertisingManager.Instance.AdsOpen.ToString();
			byte[] savedata = new System.Text.UTF8Encoding().GetBytes( data );
			//SaveGame( game, savedata, DateTime.Now.TimeOfDay );
		}
		else
		{
			//파일열기에 실패 했습니다. 오류메시지를 출력하든지 합니다.
		}
        */
    }


    void SaveGame() {
        // void SaveGame( ISavedGameMetadata game, byte[] savedData, TimeSpan totalPlaytime )	{
        /*
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

		SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
		builder = builder
			.WithUpdatedPlayedTime( totalPlaytime )
			.WithUpdatedDescription( "Saved game at " + DateTime.Now );

		SavedGameMetadataUpdate updatedMetadata = builder.Build();
		savedGameClient.CommitUpdate( game, updatedMetadata, savedData, OnSavedGameWritten );
        */
    }

    void OnSavedGameWritten()
    // void OnSavedGameWritten( SavedGameRequestStatus status, ISavedGameMetadata game )
    {


	}
	//############################################################################################################
	public void LoadFromCloud()
	{
		OpenSavedGame( false );
	}


    void OnSavedGameOpenedToRead()
    // void OnSavedGameOpenedToRead(SavedGameRequestStatus status, ISavedGameMetadata game)
    // void OnSavedGameOpenedToRead(  )
    {
        /*
		Debug.Log( "OnSavedGameOpenedToRead : " + status );
		if ( status == SavedGameRequestStatus.Success )
		{
			LoadGameData( game );
		}
		else
		{
			OpenSavedGame( true );
		}
        */

	}

    /*
	void LoadGameData( ISavedGameMetadata game )
	{
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.ReadBinaryData( game, OnSavedGameDataRead );
	}

	void OnSavedGameDataRead( SavedGameRequestStatus status, byte[] data )
	{
		if ( status == SavedGameRequestStatus.Success )
		{
			string savedata = new System.Text.UTF8Encoding().GetString( data );
			Debug.Log( "savedata : " + savedata );
			Debug.Log( "savedata.Length : " + savedata.Length );
			if ( savedata == null || savedata.Length <= 0 )
				OpenSavedGame( true );
			else
				AdvertisingManager.Instance.SetAdsOn( bool.Parse( savedata ) );
		}
		else
		{
			//읽기에 실패 했습니다. 오류메시지를 출력하던지 합니다.
		}
	}
    */

	/// <summary>
    /// 로그아웃
    /// </summary>
    public void LogOut()
    {
        // PlayGamesPlatform.Instance.SignOut();
        GooglePlayConnection.Instance.Disconnect();

    }

    /// <summary>
    /// 랭크UI 오픈 _ 자신이 보여주고싶은 보드ID를 입력하면 된다. 
    /// </summary>
    /// <param name="boardID_">구글개발자콘솔 리더보드 아이디</param>
    public void ShowRank(string boardID_) {


        // 로그인 되어있지 않은경우.
        if (GooglePlayConnection.State != GPConnectionState.STATE_CONNECTED) {
            GPGSInit();
            return;
        }

        GooglePlayManager.Instance.ShowLeaderBoardsUI();

    }
    /// <summary>
    /// 리더보드에 스코어 갱신 __ 이전에 갱신했던 점수보다 낮은 score 값을 입력할시 리더보드에 적용 안됨
    /// </summary>
    /// <param name="score_">갱신할 점수</param>
    /// <param name="boardid_">갱신할 리더보드 아이디</param>
    /// <param name="setscorecallback_">갱신후 콜백 </param>
    public void SetScore(int score_, string boardid_)
    {

        if(GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED)
            GooglePlayManager.Instance.SubmitScoreById(LEADERBOARD_ID, score_);

        //PlayGamesPlatform.Instance.ReportScore( score_, GPGSIds.leaderboard_rank, setscorecallback_ );
        //Social.ReportScore( score_, "CgkInNXfh_kLEAIQHw", setscorecallback_ );
    }
}
