using UnityEngine;
using System.Collections;

/// <summary>
/// 「サクッと空気読み。」のステージ選択シーン。
/// </summary>
public class KySceneLicenseMenu : KyScene {

	#region Inner Classes

	enum Result {
		None = 0,
		Return,	//	ソフトキー「戻る」
		License,
	}

	#endregion

	#region MonoBehaviour Methods

	void Start () {
		//	タイトルラベルの設定
		KyUtil.SetText(gameObject, "navi/label", KyText.GetText(20107));
		//	メニューアイテムの設定
		ArrayList dataSet = new ArrayList();
		dataSet.Add(new KyButtonLabeled.Data(0, KyText.GetText(20090)));
		dataSet.Add(new KyButtonLabeled.Data(1, KyText.GetText(20091)));
		//if (!KyDebugPrefs.MainMenuForDemo) { // hamson_debug
		//	dataSet.Add(new KyButtonLabeled.Data(2, KyText.GetText(20082)));
		//}
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
		if (mSceneResult == Result.License) {
			KySceneLicense scene = (KySceneLicense)ChangeScene("KySceneLicense");
			scene.LicenseType = mSelectedMenu;
		} else if (mSceneResult == Result.Return) {
			ChangeScene("KySceneGameInfo");
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
		mSelectedMenu = MainList.SelectedButtonIndex;
		mSceneResult = Result.License;
		mState.ChangeState(StateLeave);
		return 0;
	}

	#endregion

	#region Fields

	public GuiScrollList MainList = null;	//	リストGUI
	private KyStateManager mState = new KyStateManager();	//	ステート管理
	private Result mSceneResult = Result.None;	//	シーンリザルト
	private int mSelectedMenu = 0;	//	選択されたステージ番号

	#endregion
}
