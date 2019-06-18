using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Toolkit;
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#else
using UnityEngine.SocialPlatforms.GameCenter;
#endif
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoSingleton<GameManager>
{

   
    private static readonly int WatchVideoMoney = 25;

    private SkinType m_Type = SkinType.Wooden;

    private bool m_Resume;

    private bool m_ShowAd;

    private bool m_BuyAd;

    private bool m_OpenRank;

    private int m_BigADType;

    private int m_isCallback;

    private static Action __f__am_cache0;

    private static Action __f__am_cache1;

    private static Action __f__am_cache2;

    private static Action __f__am_cache3;

    private static Action __f__am_cache4;

    private static Action __f__am_cache5;

    private static Action __f__am_cache6;

    private static Action __f__am_cache7;

    private static Action __f__am_cache8;

    private static Action __f__am_cache9;

    private static Action __f__am_cacheA;

    private static Action __f__am_cacheB;

    private static Action __f__am_cacheC;

    private static Action __f__am_cacheD;

    private static Action __f__am_cacheE;

    private static Action __f__am_cacheF;

    private static Action __f__am_cache10;

    private static Action __f__am_cache11;

    private static Action __f__am_cache12;

    private static Action __f__am_cache13;

    private static Action __f__am_cache14;

    private static Action __f__am_cache15;

    private static Action __f__am_cache16;

    private static Action __f__am_cache17;

    private static Action __f__am_cache18;

    private static Action __f__am_cache19;

    private static Action __f__am_cache1A;

    private static Action __f__am_cache1B;

    private static Action __f__am_cache1C;


    public int IsCallBack
    {
        get
        {
            return this.m_isCallback;
        }
        set
        {
            this.m_isCallback = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        Input.multiTouchEnabled = false;
        Application.targetFrameRate = 30;
        MonoSingleton<ConfigeManager>.Instance.Init();
        MonoSingleton<GameDataManager>.Instance.Init();
        MonoSingleton<GameLevelManger>.Instance.Init();
        MonoSingleton<GamePlayManager>.Instance.Init();
        MonoSingleton<GameUIManager>.Instance.Init();
        if (MonoSingleton<GameDataManager>.Instance.ProcessOpen)
        {

            MonoSingleton<GameDataManager>.Instance.GAOpen();
        }
 
    }


    private void Start()
    {
        
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
        var dependencyStatus = task.Result;
        if (dependencyStatus == Firebase.DependencyStatus.Available) {
            // Create and hold a reference to your FirebaseApp,
            // where app is a Firebase.FirebaseApp property of your application class.
            //   app = Firebase.FirebaseApp.DefaultInstance;

            // Set a flag here to indicate whether Firebase is ready to use by your app.
        } else {
            UnityEngine.Debug.LogError(System.String.Format(
            "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            // Firebase Unity SDK is not safe to use here.
        }
        });
        

        // Log an event with no parameters.
        //Firebase.Analytics.FirebaseAnalytics
        //    .LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLogin);

#if UNITY_ANDROID
        // Create client configuration
        PlayGamesClientConfiguration config = new 
            PlayGamesClientConfiguration.Builder()
            .Build();

        // Enable debugging output (recommended)
        PlayGamesPlatform.DebugLogEnabled = true;
        
        // Initialize and activate the platform
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
#elif UNITY_IOS
        GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
 
        // bool deviceIsIpad = UnityEngine.iOS.Device.generation.ToString().Contains("iPad");
        
        // UnityEngine.Debug.Log("UnityEngine.iOS.Device.generation " + UnityEngine.iOS.Device.generation);

        // if (deviceIsIpad) {
        //     GetComponent<CanvasScaler>().matchWidthOrHeight = 1f;
        // }
#endif

        MonoSingleton<GamePlayManager>.Instance.RefreshInitInfo();
        MonoSingleton<GameUIManager>.Instance.ShowLoad(delegate
        {
            if (MonoSingleton<GameDataManager>.Instance.BrickCount > 0)
            {
                MonoSingleton<GamePlayManager>.Instance.Show();
            }
        }, delegate
        {
            this.m_Resume = true;
            if (MonoSingleton<GameDataManager>.Instance.BrickCount > 0)
            {
                MonoSingleton<GamePlayManager>.Instance.GamePause();
                MonoSingleton<GameUIManager>.Instance.OpenPopPanel(null, 0f);
            }
            else
            {
                MonoSingleton<GameUIManager>.Instance.OpenHomePanel(delegate
                {
                    MonoSingleton<GamePlayManager>.Instance.Show();
                    if (MonoSingleton<GameDataManager>.Instance.Course)
                    {
                        //==AdManager.Instance.ShowFull(null);

                    }
                }, 0f);
            }
        });
    }

    public void StopCourse()
    {
        MonoSingleton<GameUIManager>.Instance.StopCourse();
    }

    public void CompleteCourse()
    {
        if (MonoSingleton<GameDataManager>.Instance.ProcessFirstShoot)
        {
            MonoSingleton<GameDataManager>.Instance.GAFirstShoot();
        }
        MonoSingleton<GameDataManager>.Instance.CompleteCourse();
        MonoSingleton<GameUIManager>.Instance.HideCourse();
    }

    public void GameStart(int mode)
    {
     
        MonoSingleton<GameDataManager>.Instance.GameModeInit(0);
        MonoSingleton<GameDataManager>.Instance.PlayModeInit(mode);

        MonoSingleton<GamePlayManager>.Instance.Show();
        MonoSingleton<GamePlayManager>.Instance.RefreshInitInfo();
        MonoSingleton<GameUIManager>.Instance.CloseCurrentPanel(new Action(MonoSingleton<GamePlayManager>.Instance.GameStartForWait), 0f);
    }

    public void GamePause()
    {
        if (MonoSingleton<GameDataManager>.Instance.Course)
        {
            MonoSingleton<GameUIManager>.Instance.OpenPasuePanel(delegate
            {
                if (MonoSingleton<GameUIManager>.Instance.UIState())
                {
                    //==MonoSingleton<GamePlayManager>.Instance.Hide();
                }
                this.OpenPauseAd();
            }, 0f);
            MonoSingleton<GamePlayManager>.Instance.GamePause();
        }
    }

    public void GamePlay()
    {
        MonoSingleton<GamePlayManager>.Instance.Show();
        MonoSingleton<GameUIManager>.Instance.CloseCurrentPanel(delegate
        {
            MonoSingleton<GamePlayManager>.Instance.GamePlay();
        }, 0f);
    }

    public void GameRestart()
    {
        MonoSingleton<GameDataManager>.Instance.SaveDataForGameOver(0, 2, true);
        if (!MonoSingleton<GameDataManager>.Instance.Course)
        {
            MonoSingleton<GameDataManager>.Instance.CompleteCourse();
        }
      
        MonoSingleton<GamePlayManager>.Instance.Show();
        MonoSingleton<GamePlayManager>.Instance.RefreshInitInfo();
        MonoSingleton<GameUIManager>.Instance.CloseCurrentPanel(delegate
        {
            MonoSingleton<GamePlayManager>.Instance.GameStartForWait();
        }, 0f);
    }

    public void GameHome()
    {
        MonoSingleton<GameUIManager>.Instance.CloseCurrentPanel(null, 0f);
        MonoSingleton<GameUIManager>.Instance.OpenHomePanel(null, (MonoSingleton<GamePlayManager>.Instance.State != GamePlayManager.GameState.Pause) ? 0f : 0.4f);
       
        MonoSingleton<GamePlayManager>.Instance.GameHome();
    }

    
    public void PauseRank()
    {
     
        this.m_OpenRank = true;

        SignIn();
    }

    public void PauseContinue()
    {
        this.GamePlay();
    }

    public void PuaseHome()
    {
        MonoSingleton<GamePlayManager>.Instance.GameHome();
        MonoSingleton<GameDataManager>.Instance.SaveDataForGameOver(0, 2, true);
        MonoSingleton<GameUIManager>.Instance.CloseCurrentPanel(null, 0f);
        MonoSingleton<GameUIManager>.Instance.OpenHomePanel(delegate
        {
        }, 0.4f);
    }

    public void AdBlock(bool state = true)
    {
        if (state)
        {
            MonoSingleton<GameDataManager>.Instance.NoAD();
            MonoSingleton<GameUIManager>.Instance.RefreshCurrentPanel();
        }
        this.m_BuyAd = false;
    }

    public void ContinueGame()
    {
        MonoSingleton<GameUIManager>.Instance.TimerState(false);
        MonoSingleton<GameUIManager>.Instance.CloseCurrentPanel(delegate
        {
            MonoSingleton<GamePlayManager>.Instance.ContinueGame();
        }, 0f);
    }

    public void OpenContinuePanel()
    {
        MonoSingleton<GamePlayManager>.Instance.WaitGameContinue();
        MonoSingleton<ConfigeManager>.Instance.CaptureScreenshot(delegate
        {
            MonoSingleton<GameUIManager>.Instance.OpenContinuePanel(null, 0f);
        });
    }

    public void GameOver()
    {
        SendScore();

        MonoSingleton<GameDataManager>.Instance.SaveDataForGameOver(MonoSingleton<GamePlayManager>.Instance.Score, 2, MonoSingleton<GamePlayManager>.Instance.BoolContinueGame);
        MonoSingleton<GamePlayManager>.Instance.GameOver();
        MonoSingleton<ConfigeManager>.Instance.CaptureScreenshot(delegate
        {
            MonoSingleton<GameUIManager>.Instance.OpenGameOverPanel(delegate
            {
                if (MonoSingleton<GameUIManager>.Instance.UIState())
                {
                    MonoSingleton<GamePlayManager>.Instance.Hide();
                }
                // if (!this.Grade())
                // {
                //     AdManager.Instance.ShowFull(null);
                // }
            }, 0f);
        });
     
    }

    public void ContinueToGameOver()
    {
        MonoSingleton<GameUIManager>.Instance.TimerState(false);
        MonoSingleton<GameDataManager>.Instance.SaveDataForGameOver(MonoSingleton<GamePlayManager>.Instance.Score, 2, false);
        MonoSingleton<GameUIManager>.Instance.CloseCurrentPanel(delegate
        {
            MonoSingleton<GamePlayManager>.Instance.GameOver();
            MonoSingleton<GameUIManager>.Instance.OpenGameOverPanel(delegate
            {
                if (MonoSingleton<GameUIManager>.Instance.UIState())
                {
                    MonoSingleton<GamePlayManager>.Instance.Hide();
                }
                // if (!this.Grade())
                // {
                //     AdManager.Instance.ShowFull(null);
                // }
            }, 0f);
        }, 0f);
     
    }

    public void UseSkin(SkinType type)
    {
        if (this.m_Type != type)
        {
            this.m_Type = type;
            //if (MonoSingleton<GameDataManager>.Instance.UseSkin(type))
            {
                Image[] componentsInChildren = base.gameObject.GetComponentsInChildren<Image>(true);
                Image[] array = componentsInChildren;
                for (int i = 0; i < array.Length; i++)
                {
                    Image image = array[i];
                    if (image.sprite != null)
                    {
                        Sprite spriteByName = MonoSingleton<ConfigeManager>.Instance.GetSpriteByName(image.sprite.texture.name);
                        if (spriteByName != null)
                        {
                            image.sprite = spriteByName;
                        }
                    }
                }
                MonoSingleton<GameUIManager>.Instance.UseSkin(type);
            }
        }
        MonoSingleton<GameUIManager>.Instance.CloseStorePanel(null, 0f);
    }

    public void UnlockSkin(SkinType type, int price)
    {
        if (MonoSingleton<GameDataManager>.Instance.UnlockSkin(type, price))
        {
            MonoSingleton<GameUIManager>.Instance.UnlockSkin(type);
            this.UseSkin(type);
        }
        else
        {
            MonoSingleton<GameUIManager>.Instance.StoreHit();
        }
    }

    public void GradeGame(int starCount)
    {
        MonoSingleton<GameDataManager>.Instance.GradeIt();
        if (starCount > 3)
        {
            MonoSingleton<GameUIManager>.Instance.CloseGradlePanel(delegate
            {
                   Application.OpenURL("https://play.google.com/store/apps/details?id="+ Application.identifier);
            }, 0f);
        }
        else
        {
            MonoSingleton<GameUIManager>.Instance.CloseGradlePanel(null, 0f);
        }
    }



    private void OnApplicationQuit()
    {
        UnityEngine.Debug.Log("-----------------OnApplicationQuit");
        if (MonoSingleton<GamePlayManager>.Instance.State == GamePlayManager.GameState.GameOver || MonoSingleton<GamePlayManager>.Instance.State == GamePlayManager.GameState.ContinueWait || MonoSingleton<GamePlayManager>.Instance.State == GamePlayManager.GameState.None)
        {
            if (this.m_Resume)
            {
                MonoSingleton<GameDataManager>.Instance.SaveDataForGameOver(0, 2, true);
            }
        }
        else
        {
            MonoSingleton<GameDataManager>.Instance.SaveDataForAppPause(MonoSingleton<GamePlayManager>.Instance.Score, MonoSingleton<GamePlayManager>.Instance.BoolContinueGame);
        }
    }

    private void OnApplicationPause()
    {
        UnityEngine.Debug.Log("-----------------OnApplicationPause");
        if (MonoSingleton<GamePlayManager>.Instance.State == GamePlayManager.GameState.GameOver || MonoSingleton<GamePlayManager>.Instance.State == GamePlayManager.GameState.ContinueWait || MonoSingleton<GamePlayManager>.Instance.State == GamePlayManager.GameState.None)
        {
            if (this.m_Resume)
            {
                if (Application.systemLanguage == SystemLanguage.English)
                    Localization.Instance.ResetLanguage(LanguageEnum.English);
                else
                    Localization.Instance.ResetLanguage(LanguageEnum.Kor);
        
                MonoSingleton<GameDataManager>.Instance.SaveDataForGameOver(0, 2, true);
            }
        }
        else
        {
            MonoSingleton<GameDataManager>.Instance.SaveDataForAppPause(MonoSingleton<GamePlayManager>.Instance.Score, MonoSingleton<GamePlayManager>.Instance.BoolContinueGame);
        }
    }

 
    //revive
    public void RewardVideoEnd(bool state = true)
    {
        if (state)
        {
            this.ContinueGame();
            //  MonoSingleton<GAEvent>.Instance.Continue();
            // MonoSingleton<GAEvent>.Instance.Continueing_Video();
            //MonoSingleton<GAEvent>.Instance.Video_Continue(MonoSingleton<GameDataManager>.Instance.GameDay(), MonoSingleton<GameDataManager>.Instance.ContineuVideo);
        }
        else
        {
            this.ContinueToGameOver();
        }
    }

    public void RewardVideoGetMoney(bool state = true)
    {
        if (state)
        {

            MonoSingleton<GameDataManager>.Instance.AddMoney(GameManager.WatchVideoMoney);
            MonoSingleton<GameUIManager>.Instance.ShowGameOverMoneyAnim(false, GameManager.WatchVideoMoney);
        }
    }

    public void PopGameHome()
    {
        MonoSingleton<GameDataManager>.Instance.SaveDataForGameOver(0, 2, true);
        MonoSingleton<GamePlayManager>.Instance.GameHome();
        MonoSingleton<GameUIManager>.Instance.CloseCurrentPanel(null, 0f);
        MonoSingleton<GameUIManager>.Instance.OpenHomePanel(delegate
        {
            AdManager.Instance.ShowFull(null);
        }, 0.4f);
    }

    public void PopContinueGame()
    {
        MonoSingleton<GamePlayManager>.Instance.GameStartForWait();
        MonoSingleton<GameUIManager>.Instance.CloseCurrentPanel(delegate
        {
            AdManager.Instance.ShowFull(null);
        }, 0f);
    }

    public void PopNewGame()
    {
        MonoSingleton<GameDataManager>.Instance.SaveDataForGameOver(0, 2, true);
      
        MonoSingleton<GamePlayManager>.Instance.Show();
        MonoSingleton<GamePlayManager>.Instance.RefreshInitInfo();
        MonoSingleton<GamePlayManager>.Instance.GameStartForWait();
        MonoSingleton<GameUIManager>.Instance.CloseCurrentPanel(delegate
        {
            AdManager.Instance.ShowFull(null);
        }, 0f);
    }

    public void AchievementContinue()
    {
        MonoSingleton<GamePlayManager>.Instance.GameStartForWait();
        MonoSingleton<GameUIManager>.Instance.CloseCurrentPanel(delegate
        {
            AdManager.Instance.ShowFull(null);
        }, 0f);
    }

    public void ShowVideo()
    {
        MonoSingleton<GameUIManager>.Instance.ShowLoading(delegate
        {
            AdManager.Instance.ShowReward(null);
        }, 0f);
    }

    public void OnClickOpenWatch()
    {
        if (!AdManager.Instance.IsRewardLoaded())
        {
            MonoSingleton<GameUIManager>.Instance.OpenGameOverWatch(null, 0f);
            return;
        }
        AdManager.Instance.ShowReward((success) =>
        {
            if (success)
            {
                if(GameUIManager.Instance.IsStoreState())
                {
                    //GameUIManager.Instance.CloseStorePanel(null);
                }
                else
                    MonoSingleton<GameUIManager>.Instance.CloseWatchBtn();
                MonoSingleton<GameDataManager>.Instance.AddMoney(GameManager.WatchVideoMoney);
                //if(GameUIManager.Instance.IsStoreState() == false)
                //    MonoSingleton<GameUIManager>.Instance.ShowGameOverMoneyAnim(false, GameManager.WatchVideoMoney);

                if(GameUIManager.Instance.IsStoreState())
                {   
                    GameUIManager.Instance.CloseStorePanel(null);
                    GameUIManager.Instance.RefreshCurrentPanel();
                    GameUIManager.Instance.OpenNoticePanel("Coin 25 " + Localization.Instance.GetLable("GetSuccess"));
                }
            }
        });
    }

    public void OnClickTestWatch()
    {
        if(GameUIManager.Instance.IsStoreState())
        {
            GameUIManager.Instance.CloseStorePanel(null);
        }
        else
            MonoSingleton<GameUIManager>.Instance.CloseWatchBtn();
        MonoSingleton<GameDataManager>.Instance.AddMoney(GameManager.WatchVideoMoney);
        GameUIManager.Instance.RefreshCurrentPanel();
        //MonoSingleton<GameUIManager>.Instance.ShowGameOverMoneyAnim(false, GameManager.WatchVideoMoney);
    }

    public void GameStore()
    {
      
        MonoSingleton<GameUIManager>.Instance.OpenStorePanel(null, 0f);
    }

    public void CloseGameStore()
    {
        MonoSingleton<GameUIManager>.Instance.CloseStorePanel(null, 0f);
    }

    public void GameItemStore()
    {
      
        MonoSingleton<GameUIManager>.Instance.OpenItemStorePanel(null, 0f);
    }

    public void CloseGameItemStore()
    {
        MonoSingleton<GameUIManager>.Instance.CloseItemStorePanel(null, 0f);
    }


    //view ad to revive
    public void WatchAD()
    {

        if (!AdManager.Instance.IsRewardLoaded())
        {
            UnityEngine.Debug.Log("unity not ready");
            MonoSingleton<GameUIManager>.Instance.OpenGameOverWatch(null, 0f);
            this.ContinueToGameOver();
            return;
        }
        UnityEngine.Debug.Log("unity  ready");
        MonoSingleton<GamePlayManager>.Instance.GamePause();
        MonoSingleton<GameUIManager>.Instance.TimerState(false);

        AdManager.Instance.ShowReward(

         (success) =>
        {
            if (success)
            {
                this.ContinueGame();
            }
            else
            {
                this.ContinueToGameOver();
            }


        });


    }

    public void GameShare()
    {
        NativeShare.Share(
            Singleton<Localization>.instance.GetLable("ShareWords", 0) + " #ShootnMerge https://play.google.com/store/apps/details?id="+Application.identifier,Application.persistentDataPath + "/Shoot2048-GameOver-Screen.png", "", "", "image/png", true, "");
    }

   

   
    public void GamePhoneShake()
    {
        Handheld.Vibrate();

        MonoSingleton<GameDataManager>.Instance.PhoneShakeState(true);
        MonoSingleton<GameUIManager>.Instance.RefreshCurrentPanel();
    }

    public void GameNOPhoneShake()
    {
      
        MonoSingleton<GameDataManager>.Instance.PhoneShakeState(false);
        MonoSingleton<GameUIManager>.Instance.RefreshCurrentPanel();
    }

    public void GameAudio()
    {
        MonoSingleton<GameDataManager>.Instance.AudioState(true);
        MonoSingleton<GameUIManager>.Instance.RefreshCurrentPanel();

        GameUIManager.Instance.PlayBGM();
    }

    public void GameNoAudio()
    {
    
        MonoSingleton<GameDataManager>.Instance.AudioState(false);
        MonoSingleton<GameUIManager>.Instance.RefreshCurrentPanel();

        GameUIManager.Instance.StopBGM();
    }

    public void GameRank()
    {
        UnityEngine.Debug.Log("_____ GameRank _____");

        SignIn();
    }

    public void SignIn() 
    {
#if UNITY_ANDROID
        if (!PlayGamesPlatform.Instance.localUser.authenticated) {
            // Sign in with Play Game Services, showing the consent dialog
            // by setting the second parameter to isSilent=false.
            PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);
        } else {
            SignInCallback(true);
        }
#elif UNITY_IOS
        if(!Social.localUser.authenticated)
            Social.localUser.Authenticate(SignInCallback);
        else
        {
            SignInCallback(true);
        } 
#endif
    }

    public void SignInCallback(bool success) 
    {
#if UNITY_ANDROID
        if (success) 
        {
            if (PlayGamesPlatform.Instance.localUser.authenticated) 
                //PlayGamesPlatform.Instance.ShowLeaderboardUI();
                SendScore();
            else {
                UnityEngine.Debug.Log("Cannot show leaderboard: not authenticated");
            }
        } 
        else 
        {

        }
#elif UNITY_IOS
        if (success) 
        {
            if(Social.localUser.authenticated)
                SendScore();     
        }   
#endif
    }

    void SendScore()
    {
        string boardId = null;
        int HScore = GameDataManager.Instance.MaxScore;
#if UNITY_ANDROID
        if(GameDataManager.Instance.PlayMode == 0)
            boardId = GPGSIds.leaderboard_classicmode;
        else
            boardId = GPGSIds.leaderboard_advancedmode;

        if (PlayGamesPlatform.Instance.localUser.authenticated) {
            PlayGamesPlatform.Instance.ReportScore (HScore, boardId, (bool success) => {
                PlayGamesPlatform.Instance.ShowLeaderboardUI (boardId);
            });
        } else {
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder ().Build ();
            GooglePlayGames.PlayGamesPlatform.InitializeInstance (config);
            //GooglePlayGames.PlayGamesPlatform.DebugLogEnabled = true;
            GooglePlayGames.PlayGamesPlatform.Activate ();
            PlayGamesPlatform.Instance.localUser.Authenticate((bool success) => {
                PlayGamesPlatform.Instance.ReportScore (HScore, boardId, (bool success2) => {
                    PlayGamesPlatform.Instance.ShowLeaderboardUI (boardId);
                });
            });
        }

#elif UNITY_IOS
        if(GameDataManager.Instance.PlayMode == 0)
            boardId = "bestscore";
        else
            boardId = "AdvancedMode";

        Social.ReportScore(HScore, boardId, (bool success) =>
            {
                if (success)
                {
                    Social.ShowLeaderboardUI();
                }
            });
        
#endif
    }

    private void Update()
    {
        if (MonoSingleton<GameDataManager>.Instance.m_isFirstTouch == 0 && Input.GetMouseButtonUp(0))
        {
            MonoSingleton<GameDataManager>.Instance.GAFirstTouch();
        }
    }

    private void OpenPauseAd()
    {
        AdManager.Instance.ShowFull(null);
    }

    private bool Grade()
    {
        if (!MonoSingleton<GameDataManager>.Instance.Grade && MonoSingleton<GameDataManager>.Instance.GradeTimer > 0 && MonoSingleton<GameDataManager>.Instance.GradeTimer % 5 == 0)
        {
            MonoSingleton<GameUIManager>.Instance.OpenGradlePanel(null, 0f);
        }
        return false;
    }

    public void OpenPrivacyPolicy()
    {
        Application.OpenURL("http://www.pier-showcase.com/policy/ms_privacy_policy.html");
    }
}
