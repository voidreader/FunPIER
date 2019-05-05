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
    Make12

}

public class PlatformManager : MonoBehaviour
{
    public static PlatformManager main = null;

    string leaderboardCarID = "CgkIgYf6gpcYEAIQCg";
    string leaderboardWineID = "CgkIgYf6gpcYEAIQCw";
    string leaderboardVikingID = "CgkIgYf6gpcYEAIQDA";

    // 업적리스트 
    string ah_completeCarID = "CgkIgYf6gpcYEAIQAQ";
    string ah_completeWineID = "CgkIgYf6gpcYEAIQAg";
    string ah_completeVikingID = "CgkIgYf6gpcYEAIQAw";

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

        // 초기화
#if UNITY_ANDROID
        leaderboardCarID = "CgkIgYf6gpcYEAIQCg";
        leaderboardWineID = "CgkIgYf6gpcYEAIQCw";
        leaderboardVikingID = "CgkIgYf6gpcYEAIQDA";
#else

#endif

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
        ISN_GKGameCenterViewController viewController = new ISN_GKGameCenterViewController();
        viewController.ViewState = ISN_GKGameCenterViewControllerState.Leaderboards;
        viewController.Show();
    }

    public void OpenGameCenterAchievements() {
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
}
