using UnityEngine;
using System.Collections;

/// <summary>
/// 「エクストラ」のステージ選択シーン。
/// </summary>
public class KySceneExtraMenu : KyScene {

	#region Inner Classes

	enum Result {
		None = 0,
		Return,	//	ソフトキー「戻る」
		Stage,	//	いずれかのメニュー項目を選択
	}

	#endregion

	#region MonoBehaviour Methods

	void Start () {
		//	タイトルラベルの設定
		KyUtil.SetText(gameObject, "navi/label", KyText.GetText(20105));
		KyUtil.SetText(gameObject, "scoreLabel", KyText.GetText(20050));
		//	メニューアイテムの設定
		ArrayList dataSet = new ArrayList();
		dataSet.Add(new KyButtonLabeled.Data(0, KyText.GetText(20052)));
		dataSet.Add(new KyButtonLabeled.Data(1, KyText.GetText(20053)));
		dataSet.Add(new KyButtonLabeled.Data(2, KyText.GetText(20054)));
		//	メニュー設定
		MainList.DataSet = dataSet;
		MainList.ButtonSelected += MainMenuButtonSelected;
		MainList.CreateList();
		MainList.SetGuiEnabled(false);
		//	初期ステート
		mState.ChangeState(StateEnter);
	}
	
	void Update () {
		if (mState != null) {
			mState.Execute();
		}
	}

	#endregion

	#region State Methods

	/// <summary>
	/// シーン開始ステート。
	/// </summary>
	private int StateEnter() {
		if (mState.Sequence == 0) {
			MainList.ButtonGroup.SelectButton(0);
			KySoftKeys.Instance.SetLabels(KySoftKeys.Label.Return, KySoftKeys.Label.None);
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

	/// <summary>
	/// シーン終了ステート。
	/// </summary>
	private int StateLeave() {
		if (mState.Sequence == 0) {
			KySoftKeys.Instance.EnableSoftKeys(false);
			MainList.SetGuiEnabled(false);
			ScreenFader.Main.FadeOut();
			if (mSceneResult == Result.Stage) {
				KyAudioManager.Instance.Stop(2.0f);
			}
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			if (!ScreenFader.Main.FadeRunning) {
				mState.ChangeState(StateNextScene);
			}
		}
		return 0;
	}

	/// <summary>
	/// 次のシーンへ遷移するステート。
	/// </summary>
	private int StateNextScene() {
		if (mSceneResult == Result.Stage) {
			KySceneGameExtra scene = (KySceneGameExtra)ChangeScene("KySceneGameExtra");
			scene.Stage = (KySceneGameExtra.StageKind)mSelectedStage;
		} else if (mSceneResult == Result.Return) {
			ChangeScene("KySceneMainMenu");
		}
		return 0;
	}

	/// <summary>
	/// ユーザ入力待ちステート。
	/// </summary>
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
			}
		}
		return 0;
	}

	#endregion

	#region Methods

	/// <summary>
	/// メニュー項目選択イベントハンドラ
	/// </summary>
	private int MainMenuButtonSelected(object sender) {
		int index = MainList.SelectedButtonIndex;
		if (mSelectedStage != index) {
			ChangeSelectedStage(index);
			MainList.SetGuiEnabled(true);
			MainList.SelectedButton.State = GuiButton.ButtonState.Up;
			if (mState.State == StateMain) {
				KyAudioManager.Instance.PlayOneShot("se_cursor");
			}
		} else {
			mSceneResult = Result.Stage;
			mState.ChangeState(StateLeave);
			KyAudioManager.Instance.PlayOneShot("se_ok");
		}
		return 0;
	}

	private void ChangeSelectedStage(int stage) {
		mSelectedStage = stage;
		Preview.FrameIndex = mSelectedStage;
		string score = KySaveData.RecordData.ExtraScores[stage].ToString() + KyText.GetText(20051);
		KyUtil.SetText(gameObject, "score", score);
	}

	#endregion

	#region Fields

	public GuiScrollList MainList = null;	//	リストGUI
	public Sprite Preview = null;	//	プレビュースプライト
	private KyStateManager mState = new KyStateManager();	//	ステート管理
	private Result mSceneResult = Result.None;	//	シーンリザルト
	private int mSelectedStage = -1;	//	選択されたステージ番号

	#endregion
}
