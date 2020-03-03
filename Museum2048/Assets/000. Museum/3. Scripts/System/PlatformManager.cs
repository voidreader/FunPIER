using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif


// using Facebook.Unity;

/// <summary>
/// 업적 종류
/// </summary>
public enum MIMAchievement {
    CompleteCar,
    CompleteWine,
    CompleteViking,
    

    Move100,
    Move500,
    Move1000,
    Make10,
    Make11,
    Make12,

    CompleteIce

}

public class PlatformManager : MonoBehaviour
{
    public static PlatformManager main = null;

    string leaderboardCarID = "CgkIgYf6gpcYEAIQCg";
    string leaderboardWineID = "CgkIgYf6gpcYEAIQCw";
    string leaderboardVikingID = "CgkIgYf6gpcYEAIQDA";
    string leaderboardIceID = "CgkIgYf6gpcYEAIQDg";

    // 업적리스트 
    string ah_completeCarID = "CgkIgYf6gpcYEAIQAQ";
    string ah_completeWineID = "CgkIgYf6gpcYEAIQAg";
    string ah_completeVikingID = "CgkIgYf6gpcYEAIQAw";
    string ah_completeIceID = "CgkIgYf6gpcYEAIQDQ";



    string ah_move100 = "CgkIgYf6gpcYEAIQBA";
    string ah_move500 = "CgkIgYf6gpcYEAIQBQ";
    string ah_move1000 = "CgkIgYf6gpcYEAIQBg";

    string ah_make10 = "CgkIgYf6gpcYEAIQBw";
    string ah_make11 = "CgkIgYf6gpcYEAIQCA";
    string ah_make12 = "CgkIgYf6gpcYEAIQCQ";





    private void Awake() {
        main = this;
    }

    // Start is called before the first frame update
    void Start()    {

        // InitFacebook(); // 페이스북 초기화

    }

#region 공통 메소드  iOS & Google Play

    /// <summary>
    /// 플랫폼별 서비스 처리 
    /// </summary>
    public void InitPlatformService() {

        Debug.Log("!!! InitPlatformService Start... ");

#if UNITY_ANDROID
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
#elif UNITY_IOS
        GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
#endif

        SignInGameService();
    }

    public void SignInGameService()
    {
        Debug.Log("!!! SignInGameService Start... ");

        if (!Social.localUser.authenticated)
        {
            Social.localUser.Authenticate((bool bSuccess) =>
            {
                if (bSuccess)
                {
                    Debug.Log("Success : " + Social.localUser.userName);


                    Social.LoadAchievements(achievements => {
                        if (achievements.Length > 0)
                        {
                            Debug.Log("Got " + achievements.Length + " achievement instances");
                            string myAchievements = "My achievements:\n";
                            foreach (IAchievement achievement in achievements)
                            {
                                myAchievements += "\t" +
                                    achievement.id + " " +
                                    achievement.percentCompleted + " " +
                                    achievement.completed + " " +
                                    achievement.lastReportedDate + "\n";
                            }
                            Debug.Log(myAchievements);
                        }
                        else
                            Debug.Log("No achievements returned");
                    });

                }
                else
                {
                    Debug.Log("Fall");
                    
                }
            });
        }
    }

    /// <summary>
    /// 스코어 제출 
    /// </summary>
    /// <param name="t"></param>
    /// <param name="score"></param>
    public void SubmitScore(Theme t, int score) {

        if (!Social.localUser.authenticated)
            return;

        string lbid = string.Empty;
        Debug.Log(">> Submit LB Score :: " + t.ToString() + " / " + score);
        

        switch(t) {
            case Theme.Car:
                lbid = leaderboardCarID;
                break;

            case Theme.Wine:
                lbid = leaderboardWineID;
                break;

            case Theme.Viking:
                lbid = leaderboardVikingID;
                break;

            case Theme.Ice:
                lbid = leaderboardIceID;
                Debug.Log("submit leaderboard is ICE!!");
                 break;

                // 추가!!
        }

        Social.ReportScore(score, lbid, (bool bSuccess) => { 
            if(bSuccess)
            {
                Debug.Log("ReportLeaderBoard Success :: " + lbid);
            }
            else
            {
                Debug.Log("ReportLeaderBoard Fail :: " + lbid);
            } 

        });
    }


    /// <summary>
    /// 리더보드 호출 
    /// </summary>
    public void OpenLeaderboards() {
        if (!Social.localUser.authenticated)
        {
            SignInGameService();
        }
         

        Social.ShowLeaderboardUI();
    }

    /// <summary>
    /// 업적 창 호출 
    /// </summary>
    public void OpenAchievements() {

        if (!Social.localUser.authenticated)
        {
            SignInGameService();
        }

        Social.ShowAchievementsUI();
    }

    /// <summary>
    /// 해제 
    /// </summary>
    /// <param name="a"></param>
    public void UnlockAchievements(MIMAchievement a) {

        if (!Social.localUser.authenticated)
            return;

        Debug.Log(">> UnlockAchievements called :: " + a.ToString());


        string targetID = string.Empty;

#region targetID 설정 
        switch (a) {
            case MIMAchievement.CompleteCar:
                targetID = ah_completeCarID;
                break;
            case MIMAchievement.CompleteWine:
                targetID = ah_completeWineID;
                break;
            case MIMAchievement.CompleteViking:
                targetID = ah_completeVikingID;
                break;

            case MIMAchievement.CompleteIce:
                targetID = ah_completeIceID;
                break;

            case MIMAchievement.Move100:
                targetID = ah_move100;
                break;
            case MIMAchievement.Move500:
                targetID = ah_move500;
                break;
            case MIMAchievement.Move1000:
                targetID = ah_move1000;
                break;

            case MIMAchievement.Make10:
                targetID = ah_make10;
                break;
            case MIMAchievement.Make11:
                targetID = ah_make11;
                break;
            case MIMAchievement.Make12:
                targetID = ah_make12;
                break;

        }

        Debug.Log(">> UnlockAchievements targetID :: " + targetID);

        #endregion

        Social.ReportProgress(targetID, 100f, (bool bSucc) => { 
            if(bSucc)
            {
                Debug.Log("Unlock Achievement OK : " + targetID);
            }
            else
                Debug.Log("Unlock Achievement Fail : " + targetID);
        });
    }

    // 공통메소드 끝 
#endregion

#region iOS GamePlay

#if UNITY_IOS
    public void InitGameCenter() {
        ISN_GKLocalPlayer.Authenticate((SA_Result result) => {
            if (result.IsSucceeded) {
                Debug.Log("Authenticate is succeeded!");
                GetGameCenterLeaderboard(); // 리더보드 조회 
            }
            else {
                Debug.Log("Authenticate is failed! Error with code: " + result.Error.Code + " and description: " + result.Error.Message);
            }
        });

    }

    bool IsGameCenterSignIn() {
        return ISN_GKLocalPlayer.LocalPlayer != null;
    }


    public void OpenGameCenterLeaderboards() {

        if(!IsGameCenterSignIn()) {
            InitGameCenter();
            return;
        }

        ISN_GKGameCenterViewController viewController = new ISN_GKGameCenterViewController();
        viewController.ViewState = ISN_GKGameCenterViewControllerState.Leaderboards;
        viewController.Show();
    }

    public void OpenGameCenterAchievements() {

        if (!IsGameCenterSignIn()) {
            InitGameCenter();
            return;
        }

        ISN_GKGameCenterViewController viewController = new ISN_GKGameCenterViewController();
        viewController.ViewState = ISN_GKGameCenterViewControllerState.Achievements;
        viewController.Show();
    }

    /// <summary>
    /// 게임센터 리더보드 스코어 조회 
    /// </summary>
    void GetGameCenterLeaderboard() {
        ISN_GKLeaderboard.LoadLeaderboards((result) => {
            if (result.IsSucceeded) {

                foreach (ISN_GKLeaderboard leaderboards in result.Leaderboards) {

                    if (leaderboards.Identifier == leaderboardCarID) {
                        leaderboards.LoadScores((callback) => {

                            if(callback.IsSucceeded) {
                                PierSystem.main.carHighScore = (int)leaderboards.LocalPlayerScore.Value;
                            }

                        });
                    }
                    else if (leaderboards.Identifier == leaderboardWineID) {
                        leaderboards.LoadScores((callback) => {

                            if (callback.IsSucceeded) {
                                PierSystem.main.wineHighScore = (int)leaderboards.LocalPlayerScore.Value;
                            }

                        });
                    }
                    else if (leaderboards.Identifier == leaderboardVikingID) {
                        leaderboards.LoadScores((callback) => {

                            if (callback.IsSucceeded) {
                                PierSystem.main.vikingHighScore = (int)leaderboards.LocalPlayerScore.Value;
                            }

                        });
                    }
                    else if (leaderboards.Identifier == leaderboardIceID) {
                        leaderboards.LoadScores((callback) => {

                            if (callback.IsSucceeded) {
                                PierSystem.main.iceHighScore = (int)leaderboards.LocalPlayerScore.Value;
                            }

                        });
                    }


                } // end of foreach 


            } // end of result isSucceeded
            else {
                Debug.Log("Load Leaderboards failed! Code: " + result.Error.Code + " Message: " + result.Error.Message);
            }
        });
    }



    /// <summary>
    /// 게임센터 리더보드 스코어 제출
    /// </summary>
    /// <param name="id"></param>
    /// <param name="score"></param>
    public void SubmitGameCenterLeaderboard(string id, int score) {
        ISN_GKScore scoreReporter = new ISN_GKScore(id);
        scoreReporter.Value = score;
        scoreReporter.Context = 1;

        scoreReporter.Report((result) => {
            if (result.IsSucceeded) {
                Debug.Log("Score Report Success");
            }
            else {
                Debug.Log("Score Report failed! Code: " + result.Error.Code + " Message: " + result.Error.Message);
            }
        });
    }

    /// <summary>
    /// 게임센터 업적 해제 
    /// </summary>
    /// <param name="targetID"></param>
    public void UnlockGameCenterAchievement(string targetID) {

        if(!IsGameCenterSignIn()) {
            InitGameCenter();
            return;
        }

        ISN_GKAchievement achievement = new ISN_GKAchievement("targetID");
        achievement.PercentComplete = 100.0f;
        achievement.Report((result) => {
            if (result.IsSucceeded) {
                Debug.Log("Achievement reported");
            }
            else {
                Debug.Log("Achievement report failed! Code: " + result.Error.Code + " Message: " + result.Error.Message);
            }
        });
    }

#endif
#endregion

#region Facebook 

    /*
    /// <summary>
    /// true :: 오늘 공유했음, false :: 오늘 공유 안했음
    /// </summary>
    /// <returns></returns>
    public bool CheckFacebookShareToday() {

        if (!ES2.Exists(ConstBox.KeySavedFacebookReward))
            return false;

        if (ES2.Load<int>(ConstBox.KeySavedFacebookReward) == System.DateTime.Now.DayOfYear)
            return true;

        return false;
    }

    void InitFacebook() {
        Debug.Log(">> Init Facebook Called");

        if (!FB.IsInitialized) {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    private void InitCallback() {
        if (FB.IsInitialized) {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...

            Debug.Log("FB is initialized");
        }
        else {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }


    private void OnHideUnity(bool isGameShown) {
        if (!isGameShown) {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    /// <summary>
    /// 페이스북 로그인 
    /// </summary>
    public void LoginFacebook() {

        if (!FB.IsInitialized)
            return;

        if (!FB.IsLoggedIn) {

            AdsControl.main.IsCoolingPauseAds = true;
            var perms = new List<string>() { "public_profile", "email" };
            FB.LogInWithReadPermissions(perms, AuthCallback);

            return;
        }

        // 
    }

    private void AuthCallback(ILoginResult result) {
        if (FB.IsLoggedIn) {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(aToken.UserId);
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions) {
                Debug.Log(perm);
            }
        }
        else {
            Debug.Log("User cancelled login :: " + result.Error);
            Debug.Log("Raw result :: " + result.RawResult);
        }

    }

    public void RequestShareLink() {
        PageManager.main.OpenDoubleButtonMessage(Message.ShareReward, CheckLogin, delegate { });
    }

    public void CheckLogin() {
        Debug.Log("CheckLogin ShareLink ");



        //로그인이 안되어있으면    
        if (!FB.IsLoggedIn) {
            Debug.Log("Need Facebook Login.. Go!");
            AdsControl.main.IsCoolingPauseAds = true;
            var perms = new List<string>() { "public_profile", "email" };
            FB.LogInWithReadPermissions(perms, ShareProcedure);

            return;
        }
        else {
            Debug.Log("Already login. Share!");
            ShareProcedure(null);
        }

    }

    void ShareProcedure(ILoginResult result) {


        string address = string.Empty;
        address = "http://invite.pier-showcase.com/invite/MiM2048.html";

        Debug.Log("ShareProcedure :: " + address);

        AdsControl.main.IsCoolingPauseAds = true;

        // Share Link
        FB.ShareLink(
            new System.Uri(address),
            callback: ShareCallback);
    }


    /// <summary>
    /// 레벨클리어 공유 콜백
    /// </summary>
    /// <param name="result"></param>
    void ShareCallback(IResult result) {

        Debug.Log("Share done.");
        Debug.Log(result.RawResult);

        if (result.Cancelled || !string.IsNullOrEmpty(result.Error)) {
            return;
        }

        Debug.Log("Share reward.");

        // 보상 부분 (레드문과 변수 공유)
        int itemRange = Random.Range(0, 100);
        int valueRange = Random.Range(0, 100);

        if (itemRange < 50)
            PierSystem.currentRedMoonItem = "back";
        else if (itemRange >= 50 && itemRange < 75)
            PierSystem.currentRedMoonItem = "upgrader";
        else
            PierSystem.currentRedMoonItem = "cleaner";


        PierSystem.currentRedMoonValue = 3;
        Debug.Log("Share reward :: " + PierSystem.currentRedMoonItem + "/" + PierSystem.currentRedMoonValue);

        if (PierSystem.currentRedMoonItem == "back") {
            PierSystem.main.itemBack += PierSystem.currentRedMoonValue;
            PageManager.main.OpenMessage(Message.ItemGet, delegate { }, PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT6), PierSystem.currentRedMoonValue.ToString());
        }
        else if (PierSystem.currentRedMoonItem == "upgrader") {
            PierSystem.main.itemUpgrade += PierSystem.currentRedMoonValue;
            PageManager.main.OpenMessage(Message.ItemGet, delegate { }, PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT7), PierSystem.currentRedMoonValue.ToString());
        }
        else if (PierSystem.currentRedMoonItem == "cleaner") {
            PierSystem.main.itemCleaner += PierSystem.currentRedMoonValue;
            PageManager.main.OpenMessage(Message.ItemGet, delegate { }, PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT8), PierSystem.currentRedMoonValue.ToString());
        }

        ItemCounter.RefreshItems();
        PierSystem.main.SaveProfile();

        ES2.Save<int>(System.DateTime.Now.DayOfYear, ConstBox.KeySavedFacebookReward);
        LobbyManager.main.RefreshFacebookShareButton();

    }
    */




#endregion
}
