using UnityEngine;
using System.Collections;

public class KyApplication : KyScene {

	#region MonoBehaviour Methods

	public void Awake () {
		if (mInstance != null) {
			Destroy(this);
			return;
		}
		DebugUtil.Log("Application Awake");
		mInstance = this;
		mStageInfo = new KyStageInfo();

#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			/*
			TouchScreenKeyboard.autorotateToPortrait = false;
			TouchScreenKeyboard.autorotateToPortraitUpsideDown = false;
			TouchScreenKeyboard.autorotateToLandscapeRight = false;
			TouchScreenKeyboard.autorotateToLandscapeLeft = false;
			*/
		}
#endif
		AdjustMainCamera();
		//	데이터의 준비
		KyText.Instance.Load("Others/kysp_text");
		mStageInfo.Load("Others/kysp_stage");
		KyDesignParams.Instance.Load("Others/kysp_params");
		KySaveData.Instance.Initialize();
		KySaveData.Instance.LoadAll();
	}

	public void Start () {
		Debug.Log("Application Start");

		LoadSoundVolume();
		KyAudioManager.Instance.MasterVolume = GetVolumeFromIndex(KySaveData.PrefsData.SoundVolume);

		if (ProfilerEnabled) {
			UnityEngine.Profiling.Profiler.logFile = "kysp.log";
			UnityEngine.Profiling.Profiler.enabled = true;
		}

        //
		PushScene(StartupScene, false);
	}



    #endregion

    #region Methods

    public void OnApplicationPause(bool pause) {
		//	アプリケーション復帰時に、
		//	左ソフトキーが「中断」ならば中断キーを押したことにする。
		if (pause) {
            return;
        }


        if (GoogleAdmobMgr.Instance != null && GoogleAdmobMgr.Instance.IsCoolingPauseAds) {
            GoogleAdmobMgr.Instance.Cooling();
            return;
        }




		if (KyDebugPrefs.DontPauseWhenResume) { return; }
		if (KySoftKeys.Instance == null) { return; }
		if (!KySoftKeys.Instance.LeftButton.enabled) { return; }
		if (KySoftKeys.Instance.LeftLabel != KySoftKeys.Label.Intermit) { return; }
		GuiButton button = KySoftKeys.Instance.LeftButton;
		if (button != null) {
			button.ChangeState(GuiButton.ButtonState.Selected);
		}


		if(GoogleAdmobMgr.Instance != null)
			GoogleAdmobMgr.Instance.ShowInterstitial();

        
	}

	public void OpenOfficialUrl() {
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			Application.OpenURL(KyApplication.Instance.OfficialUrlForIPhone);
		} else if (Application.platform == RuntimePlatform.Android) {
			Application.OpenURL(KyApplication.Instance.OfficialUrlForAndroid);
		} else {
			Application.OpenURL(KyApplication.Instance.OfficialUrlForIPhone);
		}
	}

	public float GetVolumeFromIndex(int index) {
		return mVolumeTable[index];
	}

	private void LoadSoundVolume() {
		for (int i = 0; i < KyConst.VolumeStepCount; ++i) {
			mVolumeTable[i] = (float)KyDesignParams.GetParam(10 + i) / 100.0f;
		}
	}

    /// <summary>
    /// 카메라 조정 
    /// </summary>
	private void AdjustMainCamera() {
		Debug.Log("screen height = " + Screen.height);
		Debug.Log("screen width  = " + Screen.width);
		float ratio = (float)Screen.height / (float)Screen.width;
		Camera.main.orthographicSize = 240 * ratio;
        Screen.sleepTimeout = 0;
	}

	#endregion

	#region Properties

	public static KyApplication Instance {
		get {
			if (mInstance == null) {
				mInstance = new GameObject("KyApplication").AddComponent<KyApplication>();
			}
			return mInstance;
		}
	}

	public static KyStageInfo StageInfo {
		get {
			if (mInstance != null) {
				return mInstance.mStageInfo;
			} else {
				return null;
			}
		}
	}

	public int TrialScore {
		get { return mTrialScore; }
		set { mTrialScore = value; }
	}

	#endregion

	#region Fields

	public GameObject StartupScene;	//	시작 화면
	public GameObject Banner;	//	Gang 배너 인스턴스 
	public string OfficialUrlForIPhone;
	public string OfficialUrlForAndroid;

	public bool ProfilerEnabled = false;
	private int mTrialScore;

	private static KyApplication mInstance;
	private KyStageInfo mStageInfo;
	private float[] mVolumeTable = new float[KyConst.VolumeStepCount];

	#endregion
}

/// <summary>
/// このアプリケーションに関する定数を集約するクラスです。
/// </summary>
public static class KyConst {
	public const int VolumeStepCount = 6;	//	ボリュームの段階数
	public const int StageCountYomu = 100;	//	モード「空気読み。」の問題数。
	public const int StageCountYomanai = 50;	//	モード「読まない。」の問題数。
	public const int ScoreCategoryCount = (int)ScoreCategory.Count;	//	配点カテゴリ数。
	public const int ExtraStageCount = (int)ExtraStage.Count;	//	エクストラ問題数。

	public const int TwitterIDLength = 8;	//	TwitterユーザIDの最大長。
	public const int TwitterNameLength = 20;	//	Twitterユーザ名の最大長。
	public const int TwitterTokenLength = 128;	//	Twitterユーザトークンの最大長。
	public const int TwitterTokenSecretLength = 128;	//	Twitterユーザトークンシークレットの最大長。

	public static readonly System.Text.Encoding GlobalEncoding = System.Text.Encoding.UTF8;	//	アプリケーションのエンコーディング。

	/// <summary>
	/// 과거의 너 레코드 유형 
	/// </summary>
	public enum RecordType {
		Recent = 0,	//	최근의 너
		Maximum,	//	최고의 너
		Minimum,	//	최저의 너
		Count
	}

	/// <summary>
	/// 問題配点のカテゴリ
	/// </summary>
	public enum ScoreCategory {
		Makenai = 0,	//	고집!
		Omoiyari,		//	배려
		Tokekomu,		//	약한
		Oyakume,		//	역할?
		WitSa,			//	위트
		Ouyou,			//	응용력
		Count,
	}

	/// <summary>
	/// エクストラステージ
	/// </summary>
	public enum ExtraStage {
		Extra1,
		Extra2,
		Extra3,
		Count,
	}

	/// <summary>
	/// メインゲームモード
	/// </summary>
	public enum GameMode {
		KukiYomi = 0,	//	진지하게
		Yomanai = 1,	//	엉뚱하게
		Trial = 2,		//	맛보기
		Extra = 3,		//	엑스트라
		Chapter = 4,	//	챕터모드 
		Count,
	}
}

/// <summary>
/// デバッグモード設定
/// 基本的には、すべてfalseがリリース設定。
/// </summary>
public static class KyDebugPrefs {
#if UNITY_EDITOR
	public const bool MainSkipIntro = false;		//	メインモードでイントロダクション(説明画面)の表示を省略する。
	public const bool MainSkipDaimon = false;		//	メインモードで「第○問」の表示を省略する。
	public const bool MainSkipIntermit = false;		//	メインモードで中間結果画面の表示を省略する。
	public const bool MainRandomScore = false;		//	メインモードで問題の正誤に関係なく総合結果をランダムにする。
	public const bool StageResultSound = true;		//	各問題の結果をサウンドで通知する。
	public const bool OpenFullMainMenu = true;		//	はじめからメインメニューがフルオープン。
	public const bool OpenDevelopMenu = false;		//	開発用メニューをオープン。
	public const bool OpenAllSecrets = false;		//	すべての隠しのリストを解除する。
	public const bool DontPauseWhenResume = false;	//	비활성화할때 Pause 창 띄우지 않음.
	public const bool TweetTestString = false;		//	Twitterのつぶやきでテスト用文字列を使用する。
	public const bool TweetFromMainMenu = false;		//	メインメニューからつぶやくことができる。
	public const bool HideGangBanner = false;		//	タイトル画面でGangバナーを非表示にします。
	public const bool MainMenuForDemo = false;

	public const int MainStageCountCap = 100;		//	메인모드에서 스테이지 수를 지정한 수치 이하로 제한한다.
#else
	public const bool MainSkipIntro = false;
	public const bool MainSkipDaimon = false;
	public const bool MainSkipIntermit = false;
	public const bool MainRandomScore = false;
	public const bool StageResultSound = true;
	public const bool OpenFullMainMenu = true; // Pro
	public const bool OpenDevelopMenu = false;
	public const bool OpenAllSecrets = false;
	public const bool DontPauseWhenResume = false;
	public const bool TweetTestString = false;
	public const bool TweetFromMainMenu = false;
	public const bool HideGangBanner = false;
	public const bool MainMenuForDemo = false;

	public const int MainStageCountCap = 100;
#endif
}