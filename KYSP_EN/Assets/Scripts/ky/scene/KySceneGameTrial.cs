using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 「サクッと空気読み。」ゲーム進行管理シーン。
/// </summary>
public class KySceneGameTrial : KyScene {

	public enum Result {
		None = 0,
		Return,
		Tweet,
	}

	#region MonoBehaviour Methods

	void Start () {
		mGameEngine = KyGameEngine.Create();
		mGameEngine.transform.parent = transform;
		mScore = 0;
		mStageIndex = 1;
		mState.ChangeState(StatePrepare);
	}

	public override void Update() {
		mState.Execute();
	}

	#endregion

	#region State Methods

	/// <summary>
	/// このシーンを終了するためのステート。
	/// </summary>
	private int StateLeave() {
		if (mState.Sequence == 0) {
			KySoftKeys.Instance.SetGuiEnabled(false);
			ScreenFader.Main.FadeOut();
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			if (!ScreenFader.Main.FadeRunning) {
				if (mSceneResult == Result.Return) {
					ChangeScene("KySceneMainMenu");
				} else if (mSceneResult == Result.Tweet) {
					int comment =
						mScore == 10 ? 1 :
						KySaveData.RecordData.GameClearCount[2] == 1 ? 0 : 2;
					string tweetMessage = KyText.BuildTwitterMessage(
						2, 0, 0,
						KyDesignParams.GetParam(500 + mScore),
						comment);

					KySceneTweet scene = ChangeScene("KySceneTweet") as KySceneTweet;
					scene.NextScene = "KySceneMainMenu";
					scene.tweetMessage = tweetMessage;
				}
			}
		}
		return 0;
	}

	/// <summary>
	/// 問題セット番号から問題リストを生成するステート。
	/// </summary>
	private int StatePrepare() {
		mStageCount = 10;
		mStageList = new KyStageInfo.Stage[mStageCount];
		for (int i = 0; i < mStageCount; ++i) {
			int stageId = KyDesignParams.GetParam(100 + 10 * Stage + i);
			DebugUtil.Log("stage id : " + stageId);
			mStageList[i] = KyApplication.StageInfo.Stages[stageId];
		}
		mState.ChangeState(StateIntroduction);
		return 0;
	}

	private int StateIntroduction() {
		if (mState.Sequence == 0) {
			KySoftKeys.Instance.SetLabels(KySoftKeys.Label.None, KySoftKeys.Label.None);
			mGameEngine.StartScript("s003");
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			if (!mGameEngine.Running) {
				mState.ChangeState(StateStageMain);
			}
		}
		return 0;
	}

	private int StateStageMain() {
		if (mState.Sequence == 0) {
			//	第○問
			if (KyDebugPrefs.MainSkipDaimon) {
				mState.Sequence++;
			} else {
				//	第○問シーンをロードして実行(終了後に次のシーケンスへ)。
				KySceneDaimon scene = (KySceneDaimon)PushScene("KySceneDaimon");
				scene.Number = mStageIndex;
				mState.Sequence++;
			}
		} else if (mState.Sequence == 1) {
			//	問題リストから問題IDを取得してスクリプト実行。
			KySoftKeys.Instance.SetLabels(KySoftKeys.Label.Intermit, KySoftKeys.Label.None);
			int stageId = mStageList[mStageIndex - 1].MainID;
			string name = string.Format("{0:D4}", stageId);
			mGameEngine.StartScript(name);
			mState.Sequence++;
		} else if (mState.Sequence == 2) {
			mGameEngine.Pause(false);
			mState.Sequence++;
		} else if (mState.Sequence == 3) {
			//	スクリプト実行中のポーリング。
			if (KySoftKeys.Instance.SelectedButtonIndex == 0) {
				//	中断確認ダイアログ表示。
				KySoftKeys.Instance.ResetAllButtons();
				KySoftKeys.Instance.SetGuiEnabled(false);
				mGameEngine.Pause(true);
				KySceneDialog scene = (KySceneDialog)PushScene("KySceneDialog");
				scene.Message = KyText.GetText(22011);	//	「ゲームを中断しますか？」
				mState.Sequence++;
			} else if (!mGameEngine.Running) {
				//	スクリプト終了。
				KySoftKeys.Instance.EnableSoftKeys(false);
				mState.Sequence += 2;
			}
		} else if (mState.Sequence == 4) {
			//	中断確認ダイアログ結果処理。
			KySceneDialog scene = GetChildScene<KySceneDialog>();
			if (scene.DialogResult == KySceneDialog.Result.Yes) {
				mSceneResult = Result.Return;
				mState.ChangeState(StateLeave);
			} else if (scene.DialogResult == KySceneDialog.Result.No) {
				KySoftKeys.Instance.SetGuiEnabled(true);
				mState.ChangeState(StateStageMain);
				mState.Sequence = 2;
			}
			Destroy(scene.gameObject);
		} else if (mState.Sequence == 5) {
			if (mGameEngine.Result == 1) {
				mScore++;
			}
			//	シークレット達成かどうかを調べて、
			//	初めて達成した場合はフラグを設定してセーブ。
			int stageId = mStageList[mStageIndex - 1].MainID;
			bool secret = mGameEngine.CommandManager.GetVariable("secret") == 1.0f;
			if (secret) {
				int stageNo = KyApplication.StageInfo.Stages[stageId].StageNo - 1;
				if (!KySaveData.RecordData.SecretFlags[stageNo]) {
					KySaveData.RecordData.SecretFlags[stageNo] = true;
					KySaveData.Instance.Save(KySaveData.DataKind.Record);
				}
			}
			mStageIndex++;
			if (mStageIndex > mStageCount) {
				mState.ChangeState(StateResult);
			} else {
				//	まだ続く場合
				mState.Sequence = 0;
			}
		}
		return 0;
	}

	private int StateResult() {
		if (mState.Sequence == 0) {
			//KyIntermitResult.Score = mScore;
			//	クリア回数をインクリメント
			KySaveData.RecordData.GameClearCount[2] = (byte)Mathf.Clamp(KySaveData.RecordData.GameClearCount[2] + 1, 0, 100);
			KyIntermitResult.Comment = KyDesignParams.GetParam(500 + mScore);
            // KySoftKeys.Instance.SetLabels(KySoftKeys.Label.Return, KySoftKeys.Label.Tweet);
            KySoftKeys.Instance.SetLabels(KySoftKeys.Label.Return, KySoftKeys.Label.None);
            KySoftKeys.Instance.SetGuiEnabled(true);
			mGameEngine.StartScript("s007");
			mState.Sequence++;
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
			 else if (!mGameEngine.Running) {
				mState.ChangeState(StateLeave);
				mState.Sequence = 1;
				mSceneResult = Result.Return;
			}
		}
		return 0;
	}

	#endregion

	#region Fields

	public int Stage = 0;	//	ステージ番号
	private KyStateManager mState = new KyStateManager();	//	ステート管理。
	private KyGameEngine mGameEngine;	//	スクリプトエンジン。
	private KyStageInfo.Stage[] mStageList;	//	問題リストの参照。
	private int mStageIndex = 0;	//	問題番号。
	private int mStageCount = 0;	//	問題数。
	private int mScore = 0;	//	総得点。
	private Result mSceneResult = Result.None;

	#endregion

}
