using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

public class PlatformManager : MonoBehaviour {
    public static PlatformManager main = null;

    public string LB_ID = "CgkIrt-r_L4HEAIQAg";

    private void Awake() {
        main = this;
    }


    // Start is called before the first frame update
    void Start() {
        InitPlatformService();
    }


    void InitPlatformService() {

#if UNITY_ANDROID

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

#elif UNITY_IOS
            GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
#endif

        SignIn();

    }

    public void SignIn() {
        if (Social.localUser.authenticated)
            return;

        Debug.Log("Sign in Game Platform Service.... ");
        Social.localUser.Authenticate(AuthenticateCallback);
    }

    void AuthenticateCallback(bool result) {
        Debug.Log(">> AuthenticateCallback :: " + result);
    }

    void AuthenticateCallbackLB(bool result) {
        Debug.Log(">> AuthenticateCallbackLB :: " + result);

        if(result) {
            ShowLeaderBoardUI();
        }
    }

    /// <summary>
    /// 리더보드 보여주기 
    /// </summary>
    public void ShowLeaderBoardUI() {
        if (!Social.localUser.authenticated) {
            Social.localUser.Authenticate(AuthenticateCallbackLB);
        }

        Social.ShowLeaderboardUI();
            
    }

    /// <summary>
    /// 스코어 제출 
    /// </summary>
    /// <param name="score"></param>
    public void ReportScore(long score) {

        if (!Social.localUser.authenticated)
            return;

        Social.ReportScore(score, LB_ID, ReportScoreCallback);
    }

    void ReportScoreCallback(bool result) {
        Debug.Log(">> ReportScoreCallback :: " + result);
    }
}
