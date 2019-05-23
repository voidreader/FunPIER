using UnityEngine;
using System.Collections;
using SA.Android.Manifest;
using SA.Android.Content.Pm;


/// <summary>
/// 타이틀 화면
/// </summary>
public class KySceneTitle : KyScene {

    bool _initCheckPermission = false;
    readonly string savePermissionCheck = "savepermission";
    

    #region MonoBehaviour Methods

    void Awake() {

#if UNITY_ANDROID
        // 퍼미션 팝업이 오픈되었었는지 확인 
        if (PlayerPrefs.HasKey(savePermissionCheck) && PlayerPrefs.GetInt(savePermissionCheck) > 0)
            _initCheckPermission = true;
        else
            _initCheckPermission = false;
#endif

    }

    void Start () {

		mGameEngine = KyGameEngine.Create();
		mState.ChangeState(StateTitle);


    }

    public override void Update() {

        if (mState != null) {
			mState.Execute();
		}
	}


    


    #endregion

    #region State Methods

    /// <summary>
    /// 기업 로고를 표시하는 상태.
    /// 그러나 Unity 스플래시 윈도우 기능으로 실현하기 위해 현재 미사용.
    /// </summary>
    private int StateCompanyLogo() {
		if (mState.Sequence == 0) {
			//	企業ロゴ画面用スクリプト
			mGameEngine.StartScript("s001");
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			if (!mGameEngine.Running) {
				mState.ChangeState(StateTitle);
			}
		}
		return 0;
	}

    /// <summary>
    ///타이틀 화면을 표시하는 상태.
    /// </summary>
    private int StateTitle() {
		if (mState.Sequence == 0) {
			//	배너 활성화
			if (!KyDebugPrefs.HideGangBanner && KyApplication.Instance.Banner != null) {
                Debug.Log(">> gmode banner active");
                KyApplication.Instance.Banner.SetActive(true);
			}
			//	타이틀 스크립트
			if (Application.platform != RuntimePlatform.IPhonePlayer) {
				KySoftKeys.Instance.SetLabels(KySoftKeys.Label.Terminate, KySoftKeys.Label.None);
			} else {
				KySoftKeys.Instance.SetLabels(KySoftKeys.Label.None, KySoftKeys.Label.None);
			}
			KySoftKeys.Instance.SetGuiEnabled(false);
			mGameEngine.StartScript("s002");
			mState.Sequence++;
		} else if (mState.Sequence == 1) {

#if UNITY_ANDROID

            //GoogleAdmobMgr.Instance.PagePermission.Open();
            // Debug.Log("CheckingInit Starts #1-permission");

            /*
            if (!_initCheckPermission)
                CheckAndroidRuntimePermission();
            */


            // Debug.Log("CheckingInit Starts #1-permission - pass");

            /*
            if (!_initCheckPermission)
                return 2;
            */

#endif

            /*if (Application.platform != RuntimePlatform.IPhonePlayer) {
				KySoftKeys.Instance.SetGuiEnabled(true);
			}*/
            mGameEngine.Pause(false);
			mState.Sequence++;
		} else if (mState.Sequence == 2) {



            if (KySoftKeys.Instance.SelectedButtonIndex == 0) {
				KySoftKeys.Instance.ResetAllButtons();
				KySoftKeys.Instance.SetGuiEnabled(false);
				mState.ChangeState(StateTerminating);
				mGameEngine.Pause(true);
			} else if (!mGameEngine.Running) {

                Destroy(mGameEngine.gameObject);
				//	バナーの無効化
				if (KyApplication.Instance.Banner != null) {
                    KyApplication.Instance.Banner.SetActive(false);
				}
				ChangeScene("KySceneMainMenu");
			}
		}

		return 0;
	}

	/// <summary>
	/// 終了確認ダイアログを表示するステート。
	/// </summary>
	private int StateTerminating() {
		if (mState.Sequence == 0) {
			KySceneDialog scene = (KySceneDialog)PushScene("KySceneDialog");
			Assert.AssertNotNull(scene);
			scene.Message = KyText.GetText(22010);	//	「アプリを終了しますか？」
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			KySceneDialog scene = GetChildScene<KySceneDialog>();
			if (scene.DialogResult == KySceneDialog.Result.Yes) {
				mState.Sequence++;
			} else if (scene.DialogResult == KySceneDialog.Result.No) {
				mState.ChangeState(StateTitle);
				KySoftKeys.Instance.SetGuiEnabled(true);
				mState.Sequence = 1;
			}
			Destroy(scene.gameObject);
		} else if (mState.Sequence == 2) {
			ScreenFader.Main.FadeOut();
			mState.Sequence++;
		} else if (mState.Sequence == 3) {
			if (!ScreenFader.Main.FadeRunning) {
				if (Application.platform == RuntimePlatform.WindowsEditor) {
					Application.Quit();
				} else {
					System.Diagnostics.Process.GetCurrentProcess().Kill();
				}
			}
		}
		return 0;
	}

	#endregion

	#region Fields

	//public GameObject NextScene;

	private KyStateManager mState = new KyStateManager();	//	상태관리
	private KyGameEngine mGameEngine;						//	스크립트 엔진 (XML)

	#endregion

}
