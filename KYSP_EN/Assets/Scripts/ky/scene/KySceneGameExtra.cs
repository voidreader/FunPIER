using UnityEngine;
using System.Collections;

/// <summary>
/// 「エクストラ」ゲーム進行管理シーン。
/// </summary>
public class KySceneGameExtra : KyScene {

	#region Inner Classes

	public enum StageKind {
		Wave = 0,
		Nawatobi,
		Baseball,
	}

	public enum Result {
		None = 0,
		Return,
		Finish,
		Tweet,
	}

	#endregion

	#region MonoBehaviour Methods

	//	初期化処理を記述します。
	void Start () {
		mGameEngine = KyGameEngine.Create();
		mGameEngine.transform.parent = transform;
		mGameEngine.EndlessMode = true;
		mState.ChangeState(StateStageMain);
	}
	
	//	フレーム毎の処理を記述します。
	public override void Update() {
		mState.Execute();
	}

	#endregion

	#region State Methods

	private int StateLeave() {
		if (mState.Sequence == 0) {
			KySoftKeys.Instance.SetGuiEnabled(false);
			ScreenFader.Main.FadeOut();
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			if (!ScreenFader.Main.FadeRunning) {
				if (mSceneResult == Result.Return) {
					ChangeScene("KySceneExtraMenu");
				} else if (mSceneResult == Result.Tweet) {
					int comment =
						mPrevScore == 0 ? 0 :
						mNewRecord ? 1 : 2;
					string tweetMessage = KyText.BuildTwitterMessage(
						3, mScore, 0, (int)Stage, comment);
					KySceneTweet scene = ChangeScene("KySceneTweet") as KySceneTweet;
					scene.NextScene = "KySceneExtraMenu";
					scene.tweetMessage = tweetMessage;
				} else {
					ChangeScene("KySceneExtraMenu");
				}
			}
		}
		return 0;
	}

	private int StatePrepare() {
		return 0;
	}

	private int StateStageMain() {
		if (mState.Sequence == 0) {
			//	問題リストから問題IDを取得してスクリプト実行。
			KySoftKeys.Instance.SetLabels(KySoftKeys.Label.Intermit, KySoftKeys.Label.None);
			if (Stage == StageKind.Wave) {
				mGameEngine.StartScript("0006");
			} else if (Stage == StageKind.Nawatobi) {
				mGameEngine.StartScript("e002");
			} else if (Stage == StageKind.Baseball) {
				mGameEngine.StartScript("e003");
			}
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			mGameEngine.Pause(false);
			mState.Sequence++;
		} else if (mState.Sequence == 2) {
			//	スクリプト実行中のポーリング。
			if (KySoftKeys.Instance.SelectedButtonIndex == 0) {
				DebugUtil.Log("dialog");
				//	中断確認ダイアログ表示。
				KySoftKeys.Instance.ResetAllButtons();
				KySoftKeys.Instance.SetGuiEnabled(false);
				mGameEngine.Pause(true);
				KySceneDialog scene = (KySceneDialog)PushScene("KySceneDialog");
				scene.Message = KyText.GetText(22011);	//	「ゲームを中断しますか？」
				mState.Sequence++;
			} else if (!mGameEngine.Running) {
				//	スクリプト終了。
				mScore = mGameEngine.Result;
				KySoftKeys.Instance.EnableSoftKeys(false);
				mState.Sequence += 2;
			}
		} else if (mState.Sequence == 3) {
			//	中断確認ダイアログ結果処理。
			KySceneDialog scene = GetChildScene<KySceneDialog>();
			if (scene.DialogResult == KySceneDialog.Result.Yes) {
				mSceneResult = Result.Return;
				mState.ChangeState(StateLeave);
			} else if (scene.DialogResult == KySceneDialog.Result.No) {
				KySoftKeys.Instance.EnableSoftKeys(true);
				mState.ChangeState(StateStageMain);
				mState.Sequence = 1;
			}
			Destroy(scene.gameObject);
		} else if (mState.Sequence == 4) {
			mPrevScore = KySaveData.RecordData.ExtraScores[(int)Stage];
			if (mPrevScore < mScore) {
				DebugUtil.Log("new record! : " + mGameEngine.Result);
				//	新記録を出したら更新してセーブ。
				KySaveData.RecordData.ExtraScores[(int)Stage] = mScore;
				KySaveData.Instance.Save(KySaveData.DataKind.Record);
				mState.ChangeState(StateResult);
				mNewRecord = true;
			} else {
				mState.ChangeState(StateResult);
				mNewRecord = false;
			}
		}
		return 0;
	}

	private int StateResult() {

		if (mState.Sequence == 0) {
			GameObject go = Instantiate(ResultViewPrefab) as GameObject;
			go.transform.parent = transform;
			//	メッセージの設定
			if (mNewRecord) {
				KyUtil.SetText(go, "message", KyText.GetText(20056));
				KyUtil.GetComponentInChild<SpriteTextCustom>(go, "message").FontColor = Color.red;
				KyAudioManager.Instance.PlayOneShot("jg_goukaku");
			} else {
				KyUtil.SetText(go, "message", KyText.GetText(20055));
				KyAudioManager.Instance.PlayOneShot("se_result");
			}
			//	得点表示
			KySpriteNumber number = KyUtil.GetComponentInChild<KySpriteNumber>(go, "score");
			number.Number = mScore;
			number.Update();
			Transform ten = KyUtil.GetComponentInChild<Transform>(go, "ten");
			Vector3 pos = ten.localPosition;
			pos.x = number.transform.localPosition.x + number.AnchorX + 40;
			ten.localPosition = pos;
            // KySoftKeys.Instance.SetLabels(KySoftKeys.Label.Return, KySoftKeys.Label.Tweet);
            KySoftKeys.Instance.SetLabels(KySoftKeys.Label.Return, KySoftKeys.Label.None);
            KySoftKeys.Instance.SetGuiEnabled(true);
			ScreenFader.Main.FadeIn(0);
			mState.Sequence++;

            // 광고 삽입
            GoogleAdmobMgr.Instance.ShowInterstitial();

		} else if (mState.Sequence == 1) {
			if (KySoftKeys.Instance.SelectedButtonIndex != -1) {
				if (KySoftKeys.Instance.SelectedButtonIndex == 0) {
					mSceneResult = Result.Return;
				} else if (KySoftKeys.Instance.SelectedButtonIndex == 1) {
					mSceneResult = Result.Tweet;
				}
				KySoftKeys.Instance.ResetAllButtons();
				mState.ChangeState(StateLeave);
				mState.Sequence = 0;
			}
		}
		return 0;
	}

	#endregion

	#region Fields

	public StageKind Stage = StageKind.Wave;	//	問題の種類。
	public GameObject ResultViewPrefab = null;	//	リザルトビュープレハブ。
	private KyStateManager mState = new KyStateManager();	//	ステート管理。
	private KyGameEngine mGameEngine;	//	スクリプトエンジン。
	private Result mSceneResult = Result.None;
	private int mScore = 0;	//	今回の得点。
	private int mPrevScore = 0;	//	今までの最高得点。
	private bool mNewRecord = false;	//	記録更新。

	#endregion
}
