using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region UNITY_IOS
using SA.iOS.GameKit;
using SA.Foundation.Templates;
#endregion

#if UNITY_ANDROID
using SA.Android.GMS.Common;
using SA.Android.GMS.Auth;
using SA.Android.GMS.Games;
using SA.Android.App;
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
    string leaderboardIceID = " CgkIgYf6gpcYEAIQDg";

    // 업적리스트 
    string ah_completeCarID = "CgkIgYf6gpcYEAIQAQ";
    string ah_completeWineID = "CgkIgYf6gpcYEAIQAg";
    string ah_completeVikingID = "CgkIgYf6gpcYEAIQAw";
    string ah_completeIceID = " CgkIgYf6gpcYEAIQDQ";

    string ah_move100 = "CgkIgYf6gpcYEAIQBA";
    string ah_move500 = "CgkIgYf6gpcYEAIQBQ";
    string ah_move1000 = "CgkIgYf6gpcYEAIQBg";

    string ah_make10 = "CgkIgYf6gpcYEAIQBw";
    string ah_make11 = "CgkIgYf6gpcYEAIQCA";
    string ah_make12 = "CgkIgYf6gpcYEAIQCQ";


#if UNITY_ANDROID
    AN_AchievementsClient _googlePlayAchievementClient;
    List<AN_Achievement> _listGooglePlayAchivements;
#endif

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
        InitGooglePlay();
#else
        InitGameCenter();
#endif

    }

    /// <summary>
    /// 스코어 제출 
    /// </summary>
    /// <param name="t"></param>
    /// <param name="score"></param>
    public void SubmitScore(Theme t, int score) {

        Debug.Log(">> Submit LB Score :: " + t.ToString() + " / " + score);

#if UNITY_ANDROID
        switch(t) {
            case Theme.Car:
                SubmitGooglePlayLeaderboard(leaderboardCarID, score);
                break;

            case Theme.Wine:
                SubmitGooglePlayLeaderboard(leaderboardWineID, score);
                break;

            case Theme.Viking:
                SubmitGooglePlayLeaderboard(leaderboardVikingID, score);
                break;

            case Theme.Ice:
                SubmitGooglePlayLeaderboard(leaderboardIceID, score);
                break;

                // 추가!!

        }

#else

        switch (t) {
            case Theme.Car:
                SubmitGameCenterLeaderboard(leaderboardCarID, score);
                break;

            case Theme.Wine:
                SubmitGameCenterLeaderboard(leaderboardWineID, score);
                break;

            case Theme.Viking:
                SubmitGameCenterLeaderboard(leaderboardVikingID, score);
                break;

            case Theme.Ice:
                SubmitGameCenterLeaderboard(leaderboardIceID, score);
                break;

                // 추가!!

        }
#endif
    }


    /// <summary>
    /// 리더보드 호출 
    /// </summary>
    public void OpenLeaderboards() {

#if UNITY_ANDROID
        OpenGooglePlayLeaderboard();

#else
        OpenGameCenterLeaderboards();
#endif
    }

    /// <summary>
    /// 업적 창 호출 
    /// </summary>
    public void OpenAchievements() {
#if UNITY_ANDROID
        OpenGooglePlayAchievements();
#else
        OpenGameCenterAchievements();
#endif
    }

    /// <summary>
    /// 해제 
    /// </summary>
    /// <param name="a"></param>
    public void UnlockAchievements(MIMAchievement a) {

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

#if UNITY_ANDROID

        UnlockGooglePlayAchievements(targetID);
#else
        UnlockGameCenterAchievement(targetID);
#endif
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

    #region Google (Android)

#if UNITY_ANDROID
    /// <summary>
    /// 구글 플레이 로그인 
    /// </summary>
    public void InitGooglePlay() {

        Debug.Log(">>InitGooglePlay << ");
        if (Application.isEditor)
            return;

        int responce = AN_GoogleApiAvailability.IsGooglePlayServicesAvailable();
        if (responce == AN_ConnectionResult.SUCCESS) {
            // All good you can use GMS API
            SignInGooglePlay();
        }
        else {
            Debug.Log("Google Api not avaliable on current device, trying to resolve");
            AN_GoogleApiAvailability.MakeGooglePlayServicesAvailable((result) => {
                if (result.IsSucceeded) {
                    // Issue resolved you can use GMS API now
                    SignInGooglePlay();
                }
                else {
                    // Failed to resolve, all attempts to use GMS API on this device will fail
                }
            });
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    bool IsGooglePlaySignIn() {

        try {
            return AN_GoogleSignIn.GetLastSignedInAccount() != null;
        }
        catch {
            Debug.Log("Exception in GooglePlaySignIn");
            return false;
        }
    }

    /// <summary>
    /// 구글플레이 로그인
    /// </summary>
    void SignInGooglePlay() {

        Debug.Log(">>SignInGooglePlay called ");

        if (IsGooglePlaySignIn()) {
            UpdateGooglePlayAccount(AN_GoogleSignIn.GetLastSignedInAccount());
            return;
        }



        Debug.Log(">>SignInGooglePlay Start ");

        AN_GoogleSignInOptions.Builder builder = new AN_GoogleSignInOptions.Builder(AN_GoogleSignInOptions.DEFAULT_GAMES_SIGN_IN);
        builder.RequestId();
        builder.RequestEmail();
        builder.RequestProfile();

        AN_GoogleSignInOptions gso = builder.Build();
        AN_GoogleSignInClient client = AN_GoogleSignIn.GetClient(gso);
        Debug.Log("SignInNoSilent Start ");

        client.SignIn((signInResult) => {
            Debug.Log("Sign In StatusCode: " + signInResult.StatusCode);
            if (signInResult.IsSucceeded) {
                Debug.Log("SignIn Succeeded");
                UpdateGooglePlayAccount(signInResult.Account);
            }
            else {
                Debug.Log("SignIn filed: " + signInResult.Error.FullMessage);
            }
        });
    }

    /// <summary>
    /// 로그인 완료 후 리더보드 조회 
    /// </summary>
    /// <param name="account"></param>
    void UpdateGooglePlayAccount(AN_GoogleSignInAccount account) {

        Debug.Log("player.Id: " + account.GetId());
        Debug.Log("player.Title: " + account.GetEmail());

        LoadGooglePlayAchievements();

        GetGooglePlayLeaderScore(leaderboardCarID);
        GetGooglePlayLeaderScore(leaderboardWineID);
        GetGooglePlayLeaderScore(leaderboardVikingID);
        GetGooglePlayLeaderScore(leaderboardIceID);

        /*
        AN_PlayersClient client = AN_Games.GetPlayersClient();
        client.GetCurrentPlayer((result) => {
            if (result.IsSucceeded) {
                AN_Player player = result.Data;
                //Printing player info:
                Debug.Log("player.Id: " + player.Id);
                Debug.Log("player.Title: " + player.Title);
                Debug.Log("player.DisplayName: " + player.DisplayName);
                Debug.Log("player.HiResImageUri: " + player.HiResImageUri);
                Debug.Log("player.IconImageUri: " + player.IconImageUri);
                Debug.Log("player.HasIconImage: " + player.HasIconImage);
                Debug.Log("player.HasHiResImage: " + player.HasHiResImage);
            }
            else {
                AN_Logger.Log("Failed to load Current Player " + result.Error.FullMessage);
            }
        });
        */
    }

    /// <summary>
    /// 리더보드 현재 스코어 조회 
    /// </summary>
    /// <param name="id"></param>
    public void GetGooglePlayLeaderScore(string id) {
        AN_LeaderboardsClient leaderboard = AN_Games.GetLeaderboardsClient();

        

        leaderboard.LoadCurrentPlayerLeaderboardScore(id, (result) => {

            if(result.IsSucceeded) {

                // 하이 스코어에 대한 처리 
                if(id == leaderboardCarID) {
                    PierSystem.main.carHighScore = (int)result.Data.RawScore;
                    Debug.Log("Get Car leaderboard score :: " + PierSystem.main.carHighScore);
                }
                else if(id == leaderboardWineID) {
                    PierSystem.main.wineHighScore = (int)result.Data.RawScore;
                    Debug.Log("Get Wine leaderboard score :: " + PierSystem.main.wineHighScore);
                }
                else if (id == leaderboardVikingID) {
                    PierSystem.main.vikingHighScore = (int)result.Data.RawScore;
                    Debug.Log("Get Viking leaderboard score :: " + PierSystem.main.vikingHighScore);
                }
                else if (id == leaderboardIceID) {
                                PierSystem.main.iceHighScore = (int)result.Data.RawScore;
                                Debug.Log("Get Ice leaderboard score :: " + PierSystem.main.iceHighScore);
                }


                PierSystem.main.SaveProfile();

            }
        });
    }

    /// <summary>
    /// 구글 리더보드 스코어 제출 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="score"></param>
    public void SubmitGooglePlayLeaderboard(string id, int score) {

        if (!IsGooglePlaySignIn()) {
            return;
        }

        var leaderboards = AN_Games.GetLeaderboardsClient();
        leaderboards.SubmitScore(id, score);
    }

    /// <summary>
    /// 리더보드 UI 오픈 
    /// </summary>
    public void OpenGooglePlayLeaderboard() {

        // 로그인 안되어있으면 ... 
        if(!IsGooglePlaySignIn()) {
            InitGooglePlay();
            return;
        }


        var leaderboards = AN_Games.GetLeaderboardsClient();
        leaderboards.GetAllLeaderboardsIntent((result) => {
            if (result.IsSucceeded) {

                AdsControl.main.IsCoolingPauseAds = true;

                var intent = result.Intent;
                AN_ProxyActivity proxy = new AN_ProxyActivity();
                proxy.StartActivityForResult(intent, (intentResult) => {
                    proxy.Finish();
                    //Note: you might want to check is user had sigend out with that UI
                });

            }
            else {
                Debug.Log("Failed to Get leaderboards Intent " + result.Error.FullMessage);
            }
        });
    }


    /// <summary>
    /// 구글 플레이 업적 오픈
    /// </summary>
    public void OpenGooglePlayAchievements() {

        // 로그인 안되어있으면 ... 
        if (!IsGooglePlaySignIn()) {
            InitGooglePlay();
            return;
        }

        var client = AN_Games.GetAchievementsClient();
        client.GetAchievementsIntent((result) => {
            if (result.IsSucceeded) {

                AdsControl.main.IsCoolingPauseAds = true;

                var intent = result.Intent;
                AN_ProxyActivity proxy = new AN_ProxyActivity();
                proxy.StartActivityForResult(intent, (intentResult) => {
                    proxy.Finish();
                    //TODO you might want to check is user had sigend out with that UI
                });

            }
            else {
                Debug.Log("Failed to Get Achievements Intent " + result.Error.FullMessage);
            }
        });
    }

    /// <summary>
    /// 구글 플레이 업적 해제 
    /// </summary>
    /// <param name="a"></param>
    public void UnlockGooglePlayAchievements(string targetID) {

        if (!IsGooglePlaySignIn()) {
            InitGooglePlay();
            return;
        }


        foreach (AN_Achievement achieve in _listGooglePlayAchivements) {
            if(achieve.AchievementId == targetID && achieve.State != AN_Achievement.AchievementState.UNLOCKED) {
                _googlePlayAchievementClient.Unlock(achieve.AchievementId); // 해제!
                _googlePlayAchievementClient.UnlockImmediate(achieve.AchievementId, (result) => {
                    if(result.IsSucceeded) {
                        Debug.Log(">> Google Play Unlock Succeeded! <<");
                        LoadGooglePlayAchievements(); // 성공했으면 다시 업적 정보 갱신
                    }
                });
            }
        }

    }

    /// <summary>
    /// 구글 플레이 업적 정보 조회
    /// </summary>
    void LoadGooglePlayAchievements() {

        Debug.Log(">> LoadGooglePlayAchievements <<");


        _googlePlayAchievementClient = AN_Games.GetAchievementsClient();

        _googlePlayAchievementClient.Load(false, (result) => {

            if (result.IsSucceeded) {
                Debug.Log("Load Achievements Succeeded, count: " + result.Achievements.Count);
                foreach (var achievement in result.Achievements) {
                    // Debug.Log("------------------------------------------------");
                    // Debug.Log("achievement.AchievementId: " + achievement.AchievementId);
                    /*
                    Debug.Log("achievement.Description: " + achievement.Description);
                    Debug.Log("achievement.Name: " + achievement.Name);
                    Debug.Log("achievement.UnlockedImageUri: " + achievement.UnlockedImageUri);
                    Debug.Log("achievement.CurrentSteps: " + achievement.CurrentSteps);
                    Debug.Log("achievement.TotalSteps: " + achievement.TotalSteps);
                    Debug.Log("achievement.Type: " + achievement.Type);
                    Debug.Log("achievement.Sate: " + achievement.State);
                    */
                }

                // 리스트 따로 저장
                _listGooglePlayAchivements = result.Achievements;

                // Debug.Log("------------------------------------------------");
            }
            else {
                Debug.Log("Load Achievements Failed: " + result.Error.FullMessage);
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
