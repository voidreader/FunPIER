using UnityEngine;
using System.Collections;

public class KySceneFinalResult : KyScene {

	public enum Result {
		None = 0,
		Return,
		Tweet
	}

	#region MonoBehaviour Methods

	void Start () {
		GameObject go = (GameObject)Instantiate(RecordViewPrefab);
		go.name = "recordView";
		go.transform.parent = transform;
		mRecordView = go.GetComponent<KyRecordView>();
		mRecordView.InitShowMessage = false;
		mRecordView.ViewMode = 0;
		mRecordView.GameMode = GameMode;

		KyUtil.SetText(gameObject, "navi/label", KyText.GetText(20043));
		
		//	画面上端をアンカーに
		GameObject navi = KyUtil.FindChild(gameObject, "navi");
		Vector3 pos = navi.transform.localPosition;
		pos.y = Camera.main.orthographicSize;
		navi.transform.localPosition = pos;
		Transform mode = transform.Find("recordView/mode");
		pos = mode.localPosition;
		pos.y = Camera.main.orthographicSize - 68;
		mode.localPosition = pos;

		mState.ChangeState(StateEnter);
	}

	public override void Update() {
		if (mState != null) {
			mState.Execute();
		}
	}

	#endregion

	#region State Methods

	/// <summary>
	/// このシーンを初期化してフェードインするためのステート。
	/// </summary>
	private int StateEnter() {
		if (mState.Sequence == 0) {
			KySoftKeys.Instance.SetGuiEnabled(false);
			ScreenFader.Main.FadeIn();
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			if (!ScreenFader.Main.FadeRunning) {
				mState.ChangeState(StateMain);
			}
		}
		return 0;
	}


	/// <summary>
	/// このシーンをフェードアウトして終了するためのステート。
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
					//	ツイート文の組立て。
					int totalScore = KySaveData.RecordData.GetScoreSumInPercent((int)GameMode, (int)KyConst.RecordType.Recent);
					int comment =
						totalScore == 100 ? 1 :
						KySaveData.RecordData.GameClearCount[(int)GameMode] == 1 ? 0 :
						KySaveData.ContextData.RecordBetterThanLast ? 2 : 3;
					string tweetMessage = KyText.BuildTwitterMessage(
						(int)GameMode,
						totalScore,
						mRecordView.RecordMaxCategory,
						mRecordView.RecordRank,
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
	/// グラフや結果表示・ユーザ入力を待つためのステート。
	/// </summary>
	private int StateMain() {
		if (mState.Sequence == 0) {
			//	グラフアニメーション開始
			float[] scores = KySaveData.RecordData.GetScoresInRate((int)GameMode, (int)KyConst.RecordType.Recent);
			mRecordView.SetupScore(scores);
			mRecordView.BeginAnimChart();
			KyAudioManager.Instance.PlayOneShot("se_graph");
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			//	グラフアニメーション待ち。
			mInput.Update();
			if (mInput.InputTrigger == KyInputDrag.Trigger.TouchDown ||
				!mRecordView.ChartAnimating) {
				//	タッチでアニメーション省略。
				mRecordView.StopAnimChart();
				mRecordView.UpdateChart(1.0f);
				mState.Sequence++;
			}
		} else if (mState.Sequence == 2) {
            //	ソフトキー準備。
            //KySoftKeys.Instance.SetLabels(KySoftKeys.Label.Return, KySoftKeys.Label.Tweet);
            KySoftKeys.Instance.SetLabels(KySoftKeys.Label.Return, KySoftKeys.Label.None);

            KySoftKeys.Instance.SetGuiEnabled(true);
			KyAudioManager.Instance.PlayOneShot("se_result");
			mRecordView.ShowMessage(true);
			mState.Sequence++;
		} else if (mState.Sequence == 3) {
			//	ソフトキー待ち。
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


	public Result SceneResult {
		get { return mSceneResult; }
	}

	#endregion

	#region Fields

	public GameObject RecordViewPrefab = null;					//	結果画面ビュー
	public KyConst.GameMode GameMode = KyConst.GameMode.KukiYomi;	//	ゲームモード

	private KyStateManager mState = new KyStateManager();	//	ステート管理
	private KyInputDrag mInput = new KyInputDrag();			//	入力管理
	private Result mSceneResult = Result.None;				//	シーンリザルト
	private KyRecordView mRecordView = null;

	#endregion
}
