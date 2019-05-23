using UnityEngine;
using System.Collections;

/// <summary>
/// 「サクッと空気読み。」のステージ選択シーン。
/// </summary>
public class KySceneTrialMenu : KyScene {

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
		KyUtil.SetText(gameObject, "navi/label", KyText.GetText(20104));
		//	メニューアイテムの設定
		ArrayList dataSet = new ArrayList();
		dataSet.Add(new KyButtonLabeled.Data(0, KyText.GetText(20080)));
		dataSet.Add(new KyButtonLabeled.Data(1, KyText.GetText(20081)));
		if (!KyDebugPrefs.MainMenuForDemo) {
			dataSet.Add(new KyButtonLabeled.Data(2, KyText.GetText(20082)));
		}
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

	private int StateEnter() {
		if (mState.Sequence == 0) {
			KySoftKeys.Instance.SetLabels(KySoftKeys.Label.Return, KySoftKeys.Label.None);
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
			KySceneGameTrial scene = (KySceneGameTrial)ChangeScene("KySceneGameTrial");
			scene.Stage = mSelectedStage;
		} else if (mSceneResult == Result.Return) {
			ChangeScene("KySceneMainMenu");
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
		mSelectedStage = MainList.SelectedButtonIndex;
		mSceneResult = Result.Stage;
		mState.ChangeState(StateLeave);
		return 0;
	}

	#endregion

	#region Fields

	public GuiScrollList MainList = null;	//	リストGUI
	private KyStateManager mState = new KyStateManager();	//	ステート管理
	private Result mSceneResult = Result.None;	//	シーンリザルト
	private int mSelectedStage = 0;	//	選択されたステージ番号

	#endregion
}
