using UnityEngine;
using System.Collections;

/// <summary>
/// チャプターモードから起動されるゲームシーン。
/// 問題画面→つぶやき画面の流れ。
/// </summary>
public class KySceneGameChapter : KyScene {

	#region Inner Classes

	enum Result {
		None = 0,
		Return,
		Clear,
	}

	#endregion

	#region MonoBehaviour Methods

	void Start () {
		mGameEngine = KyGameEngine.Create();
		mState.ChangeState(StateStageMain);
	}
	
	public override void Update() {
		if (mState != null) {
			mState.Execute();
		}
	}

	#endregion 

	#region State Methods

	private int StateLeave() {
		if (mState.Sequence == 0) {
			KySoftKeys.Instance.SetGuiEnabled(false);
			ScreenFader.Main.FadeOut();
			KyAudioManager.Instance.StopAll();
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			if (!ScreenFader.Main.FadeRunning) {
				Destroy(mGameEngine.gameObject);
				switch (mSceneResult) {
				case Result.Return:
					ChangeScene("KySceneChapterMenu");
					break;
				case Result.Clear: {
					KySceneComment scene = (KySceneComment)ChangeScene("KySceneComment");
					scene.StageId = StageId;
				} break;
				}
			}
		}
		return 0;
	}

	private int StateStageMain() {
		if (mState.Sequence == 0) {
			KySoftKeys.Instance.SetLabels(KySoftKeys.Label.Intermit, KySoftKeys.Label.None);
			string name = string.Format("{0:D4}", StageId);
			mGameEngine.StartScript(name);
			mState.Sequence++;
			return 0;
		} else if (mState.Sequence == 1) {
			//mGameEngine.Pause(false);
			mState.Sequence++;
		} else if (mState.Sequence == 2) {
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
				mSceneResult = Result.Clear;
				//	シークレット達成かどうかを調べて、
				//	初めて達成した場合はフラグを設定してセーブ。
				bool secret = mGameEngine.CommandManager.GetVariable("secret") == 1.0f;
				if (secret) {
					int stageNo = KyApplication.StageInfo.Stages[StageId].StageNo - 1;
					if (!KySaveData.RecordData.SecretFlags[stageNo]) {
						KySaveData.RecordData.SecretFlags[stageNo] = true;
						KySaveData.Instance.Save(KySaveData.DataKind.Record);
					}
				}
				mState.ChangeState(StateLeave);
				mState.Sequence = 1;
			}
		} else if (mState.Sequence == 3) {
			//	中断確認ダイアログ結果処理。
			KySceneDialog scene = GetChildScene<KySceneDialog>();
			if (scene.DialogResult == KySceneDialog.Result.Yes) {
				mSceneResult = Result.Return;
				mState.ChangeState(StateLeave);
			} else if (scene.DialogResult == KySceneDialog.Result.No) {
				mState.ChangeState(StateStageMain);
				KySoftKeys.Instance.SetGuiEnabled(true);
				mGameEngine.Pause(false);
				mState.Sequence = 1;
			}
			Destroy(scene.gameObject);
		}
		return 0;
	}

	#endregion

	#region Fields

	public int StageId = 0;		//	問題ID
	private KyStateManager mState = new KyStateManager();	//	ステート管理。
	private KyGameEngine mGameEngine = null;				//	スクリプトエンジン。
	private Result mSceneResult = Result.None;				//	シーンリザルト。

	#endregion
}
