using System.Collections;
using System.Collections.Generic;
using SA.Android.App;
using SA.Android.GMS.Auth;
using SA.Android.GMS.Common;
using SA.Android.GMS.Games;

using UnityEngine.SocialPlatforms.GameCenter;


using UnityEngine;



public class PlatformManager : MonoBehaviour {
    public static PlatformManager main = null;

    public string LB_ID = "CgkIrt-r_L4HEAIQAg";
    public bool PlatformAvailable = false;
    public System.Action OnSignIn = delegate { };

    private void Awake() {
        main = this;
    }


    // Start is called before the first frame update
    void Start() {
        InitPlatformService();
    }



    /// <summary>
    /// 플랫폼 서비스 초기화
    /// </summary>
    void InitPlatformService() {

        Debug.Log("InitPlatformService..... ");

#if UNITY_ANDROID

        int responce = AN_GoogleApiAvailability.IsGooglePlayServicesAvailable();
        if (responce == AN_ConnectionResult.SUCCESS) {
            // All good you can use GMS API

            Debug.Log("InitPlatformService..... IsGooglePlayServicesAvailable");

            PlatformAvailable = true;
            GPGS_SignIn(true);
        }
        else {
            Debug.Log("Google Api not avaliable on current device, trying to resolve");
            AN_GoogleApiAvailability.MakeGooglePlayServicesAvailable((result) => {
                if (result.IsSucceeded) {
                    Debug.Log("InitPlatformService..... MakeGooglePlayServicesAvailable");

                    // Issue resolved you can use GMS API now
                    PlatformAvailable = true;
                    GPGS_SignIn(true);
                }
                else {
                    Debug.Log("InitPlatformService..... Fail GPGS");
                    PlatformAvailable = false;
                }
            });
        }

#elif UNITY_IOS
        PlatformAvailable = true;
        GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
        GameCenter_IsSignedIn();    
#endif





    }


    /// <summary>
    /// 리더보드 보여주기 
    /// </summary>
    public void ShowLeaderBoardUI() {



#if UNITY_ANDROID
        Debug.Log(">> ShowLeaderBoardUI  :: " + PlatformAvailable + "/" + GPGS_IsSignedIn());


        if (PlatformAvailable && GPGS_IsSignedIn()) {
            var leaderboards = AN_Games.GetLeaderboardsClient();
            leaderboards.GetAllLeaderboardsIntent((result) => {
                if (result.IsSucceeded) {
                    var intent = result.Intent;
                    AN_ProxyActivity proxy = new AN_ProxyActivity();
                    proxy.StartActivityForResult(intent, (intentResult) => {
                        proxy.Finish();
                        //TODO you might want to check is user had sigend out with that UI
                    });

                }
                else {
                    Debug.Log("Failed to Get leaderboards Intent " + result.Error.FullMessage);
                }
            });
        }
        else {
            if (!PlatformAvailable)
                return;

            if(!GPGS_IsSignedIn()) {
                OnSignIn = ShowLeaderBoardUI;
                GPGS_SignIn(false); // 로그인 후 리더보드 재 호출

            }

        }

#elif UNITY_IOS
        if (!GameCenter_IsSignedIn()) {
            OnSignIn = ShowLeaderBoardUI;
            GameCenter_Signin();
        }
        else {
            Debug.Log("Show Gamecenter Leaderboard");
            Social.ShowLeaderboardUI();
        }


#endif




    }

#if UNITY_IOS

    public void GameCenter_Signin() {

        Social.localUser.Authenticate((bool success) => {
            if(success) {
                Debug.Log("Game Center Sign in Sucess");
                OnSignIn();
            }
            else {
                Debug.Log("Game Center Sign in Fail..!!");
            }
        });
    }


    private bool GameCenter_IsSignedIn() {
        return Social.localUser.authenticated;
    }


    void CallbackReportScore(bool result) {
        Debug.Log("Report Score callback :: " + result);
    }

#endif


    /// <summary>
    /// 스코어 제출 
    /// </summary>
    /// <param name="score"></param>
    public void ReportScore(long score) {


#if UNITY_ANDROID
        if (PlatformAvailable && GPGS_IsSignedIn()) 
            AN_Games.GetLeaderboardsClient().SubmitScore(LB_ID, score);

#elif UNITY_IOS
        
        if(GameCenter_IsSignedIn()) {
            Social.ReportScore(score, LB_ID,  CallbackReportScore);
        }

#endif




    }





#region GPGS
#if UNITY_ANDROID

    private bool GPGS_IsSignedIn() {
        return AN_GoogleSignIn.GetLastSignedInAccount() != null;
    }

    public void GPGS_SignIn(bool isSilent) {
        AN_GoogleSignInOptions.Builder builder = new AN_GoogleSignInOptions.Builder(AN_GoogleSignInOptions.DEFAULT_GAMES_SIGN_IN);
        builder.RequestId();
        builder.RequestEmail();
        builder.RequestProfile();

        AN_GoogleSignInOptions gso = builder.Build();
        AN_GoogleSignInClient client = AN_GoogleSignIn.GetClient(gso);
        Debug.Log("SignInNoSilent Start ");

        if (isSilent) {

            client.SilentSignIn((signInResult) => {
                Debug.Log("Sign In StatusCode: " + signInResult.StatusCode);
                if (signInResult.IsSucceeded) {
                    Debug.Log("SignIn Succeeded");
                    // UpdateUIWithAccount(signInResult.Account);
                    OnSignIn();
                }
                else {
                    Debug.Log("SignIn filed: " + signInResult.Error.FullMessage);
                }
            });
        }
        else {
            client.SignIn((signInResult) => {
                Debug.Log("Sign In StatusCode: " + signInResult.StatusCode);
                if (signInResult.IsSucceeded) {
                    Debug.Log("SignIn Succeeded");
                    // UpdateUIWithAccount(signInResult.Account);
                    OnSignIn();
                }
                else {
                    Debug.Log("SignIn filed: " + signInResult.Error.FullMessage);
                }
            });
        }



    }
#endif


#endregion
}
