using UnityEngine;
using System.Collections;

public class KySceneMainMenu : KyScene {

	#region Inner Classes

	public enum Result {
		None = 0,
		Consider,
		Inconsider,
		Extra,
		Chapter,
		Trial,
		Secret,
		Record,
		Option,
		Demo,
		License,
		Return,
		Tweet,
        Unlock,
        Redirect

	}

    #endregion

    

    #region MonoBehaviour Methods
    
    void Start() {



        // 위치 조정
        // KyUtil.AlignTop(KyUtil.FindChild(gameObject, "menu"), -120);
        // 제목 레이블 설정
        NaviSprite.Text = KyText.GetText(20100);	//	메뉴
		NaviSprite.UpdateAll();
		//	メニュー項目の設定
		ArrayList dataSet = new ArrayList();

        // DEBUG : 처음부터 메인 메뉴 풀 오픈
        if (KyDebugPrefs.MainMenuForDemo) {

			dataSet.Add(new KyButtonLabeled.Data((int)Result.Trial, KyText.GetText(20024)));
			dataSet.Add(new KyButtonLabeled.Data((int)Result.Option, KyText.GetText(20027)));
			dataSet.Add(new KyButtonLabeled.Data((int)Result.License, KyText.GetText(20029)));
			MainList.Scrollable = false;
		} else { // 한번이라도 클리어 해야 풀 메뉴 오픈 
			if (KySaveData.RecordData.GameClearCount[0] >= 1 || KyDebugPrefs.OpenFullMainMenu) {
				dataSet.Add(new KyButtonLabeled.Data((int)Result.Consider, KyText.GetText(20000)));
				dataSet.Add(new KyButtonLabeled.Data((int)Result.Inconsider, KyText.GetText(20021)));
				// dataSet.Add(new KyButtonLabeled.Data((int)Result.Trial, KyText.GetText(20024)));
				dataSet.Add(new KyButtonLabeled.Data((int)Result.Extra, KyText.GetText(20022)));
				// dataSet.Add(new KyButtonLabeled.Data((int)Result.Chapter, KyText.GetText(20023)));
				dataSet.Add(new KyButtonLabeled.Data((int)Result.Secret, KyText.GetText(20025)));
				dataSet.Add(new KyButtonLabeled.Data((int)Result.Record, KyText.GetText(20026)));
                // dataSet.Add(new KyButtonLabeled.Data((int)Result.Unlock, KyText.GetText(24102)));
                // dataSet.Add(new KyButtonLabeled.Data((int)Result.Redirect, KyText.GetText(20046)));
                dataSet.Add(new KyButtonLabeled.Data((int)Result.Option, KyText.GetText(20027)));
				// dataSet.Add(new KyButtonLabeled.Data((int)Result.License, KyText.GetText(20029)));
			} else {
				dataSet.Add(new KyButtonLabeled.Data((int)Result.Consider, KyText.GetText(20000)));
				dataSet.Add(new KyButtonLabeled.Data((int)Result.Trial, KyText.GetText(20024)));
                // dataSet.Add(new KyButtonLabeled.Data((int)Result.Unlock, KyText.GetText(24102)));
                // dataSet.Add(new KyButtonLabeled.Data((int)Result.Redirect, KyText.GetText(20046)));
                dataSet.Add(new KyButtonLabeled.Data((int)Result.Option, KyText.GetText(20027)));
				dataSet.Add(new KyButtonLabeled.Data((int)Result.License, KyText.GetText(20029)));
                MainList.Scrollable = false;
			}

			if (KyDebugPrefs.OpenDevelopMenu) {
				dataSet.Add(new KyButtonLabeled.Data((int)Result.Demo, KyText.GetText(20028)));
			}
		}
		
		//	메뉴 설정
		MainList.DataSet = dataSet;
		MainList.ButtonSelected += MainMenuButtonSelected;
		MainList.CreateList();
		MainList.SetGridPosition(-1);
		MainList.SetGuiEnabled(false);
		// 초기 상태 설정
		mState.ChangeState(StateEnter);



        // 배너 추가
        GoogleAdmobMgr.Instance.RequestBanner();
        GoogleAdmobMgr.Instance.RequestInterstitial();
        GoogleAdmobMgr.Instance.RequestRewardAd();
        GoogleAdmobMgr.Instance.OpenCross();

        


    }

	public override void Update() {
		if (mState != null) {
			mState.Execute();
		}
	}

	#endregion

	#region State Methods

	private int StateEnter() {
		if (mState.Sequence == 0) {
            //DEBUG:메인 메뉴에서 중얼거린다.
            if (KyDebugPrefs.TweetFromMainMenu) {
				KySoftKeys.Instance.SetLabels(KySoftKeys.Label.Return, KySoftKeys.Label.Tweet);
			} else {
				KySoftKeys.Instance.SetLabels(KySoftKeys.Label.Return, KySoftKeys.Label.None);
			}
			
			KySoftKeys.Instance.SetGuiEnabled(false);
			KyAudioManager.Instance.Play("bgm_title", true);
			ScreenFader.Main.FadeIn();
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			if (!ScreenFader.Main.FadeRunning) {
				mState.ChangeState(StateMain);
			}
		}
		return 0;
	}

	private int StateLeave() {
		if (mState.Sequence == 0) {
			KySoftKeys.Instance.EnableSoftKeys(false);
			MainList.SetGuiEnabled(false);
			ScreenFader.Main.FadeOut();
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			if (!ScreenFader.Main.FadeRunning) {
				mState.ChangeState(StateNextState);
			}
		}
		return 0;
	}

	private int StateMain() {
		if (mState.Sequence == 0) {
			KySoftKeys.Instance.SetGuiEnabled(true);
			MainList.SetGuiEnabled(true);
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			if (KySoftKeys.Instance.SelectedButtonIndex == 0) {
				KySoftKeys.Instance.ResetAllButtons();
				mSceneResult = Result.Return;
				mState.ChangeState(StateLeave);
			} else if (KySoftKeys.Instance.SelectedButtonIndex == 1) {
				KySoftKeys.Instance.ResetAllButtons();
				mSceneResult = Result.Tweet;
				mState.ChangeState(StateLeave);
			}
		}
		return 0;
	}

	private int StateNextState() {
		if (mSceneResult == Result.Consider) {
			if (KySaveData.IntermitData1.StageIndex == 0) {
				KySceneGameMain scene = (KySceneGameMain)ChangeScene("KySceneGameMain");
				KyAudioManager.Instance.Stop();
				scene.GameMode = KyConst.GameMode.KukiYomi;
			} else {
				KySceneContinue scene = (KySceneContinue)ChangeScene("KySceneContinue");
				scene.GameMode = KyConst.GameMode.KukiYomi;
			}
		} else if (mSceneResult == Result.Inconsider) {
			if (KySaveData.IntermitData2.StageIndex == 0) {
				KySceneGameMain scene = (KySceneGameMain)ChangeScene("KySceneGameMain");
				KyAudioManager.Instance.Stop();
				scene.GameMode = KyConst.GameMode.Yomanai;
			} else {
				KySceneContinue scene = (KySceneContinue)ChangeScene("KySceneContinue");
				scene.GameMode = KyConst.GameMode.Yomanai;
			}
		} else if (mSceneResult == Result.Extra) {
			ChangeScene("KySceneExtraMenu");
		} else if (mSceneResult == Result.Chapter) {
			ChangeScene("KySceneChapterMenu");
		} else if (mSceneResult == Result.Trial) {
			ChangeScene("KySceneTrialMenu");
		} else if (mSceneResult == Result.Secret) {
			ChangeScene("KySceneSecretMenu");
		} else if (mSceneResult == Result.Record) {
			ChangeScene("KySceneRecord");
		} else if (mSceneResult == Result.Option) {
			ChangeScene("KySceneSettings");
		} else if (mSceneResult == Result.License) {
			ChangeScene("KySceneGameInfo");
		} else if (mSceneResult == Result.Demo) {
			KySceneGameMain scene = ChangeScene("KySceneGameMain") as KySceneGameMain;
			KyAudioManager.Instance.Stop();
			scene.GameMode = KyConst.GameMode.KukiYomi;
			scene.DemoMode = true;
		} else if (mSceneResult == Result.Return) {
			ChangeScene("KySceneTitle");
		} else if (mSceneResult == Result.Tweet) {
			KySceneTweet scene = (KySceneTweet)ChangeScene("KySceneTweet");
			scene.NextScene = "KySceneMainMenu";
			scene.tweetMessage = KyText.GetText(23101) + Random.Range(0, System.Int32.MaxValue).ToString();
		}
        else if (mSceneResult == Result.Unlock) {
            Application.OpenURL("http://onelink.to/v35ghw"); // 프로버전 오픈 
            ChangeScene("KySceneMainMenu");
        }
        else if (mSceneResult == Result.Redirect) {
            Application.OpenURL("http://onelink.to/azvmtn"); // 추천게임
            ChangeScene("KySceneMainMenu");
        }
        else {
			//	ここには来ないはず。
			ChangeScene("KySceneMainMenu");
		}
		return 0;
	}

	private int MainMenuButtonSelected(object sender) {
		KyButtonLabeled.Data data = (KyButtonLabeled.Data)MainList.SelectedButton.UserData;
		mSceneResult = (Result)data.Index;
		mState.ChangeState(StateLeave);
		return 0;
	}

	#endregion

	#region Fields

	public GuiScrollList MainList = null;	//	リストGUI
	public SpriteTextCustom NaviSprite = null;	//	ナビゲーションスプライト
	private KyStateManager mState = new KyStateManager();	//	ステート管理
	private Result mSceneResult = Result.None;	//	シーンリザルト

	#endregion
}
