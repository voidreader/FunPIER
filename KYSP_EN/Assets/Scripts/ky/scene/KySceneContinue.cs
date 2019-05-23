using UnityEngine;
using System.Collections;

/// <summary>
/// メインモード「空気読み。」「読まない。」のつづきからを選択するシーン。
/// </summary>
public class KySceneContinue : KyScene {

	#region Inner Classes

	public enum Result {
		None = 0,
		NewGame,
		Continue,
		Return,
	};

	#endregion

	#region MonoBehaviour Methods

	void Start () {
		//	タイトルラベルの設定
		int textId = GameMode == KyConst.GameMode.KukiYomi ? 20102 : 20103;
		NaviSprite.Text = KyText.GetText(textId);
		//	メニューアイテムの設定
		ArrayList dataSet = new ArrayList();
		dataSet.Add(new KyButtonLabeled.Data((int)Result.NewGame, KyText.GetText(20040)));
		dataSet.Add(new KyButtonLabeled.Data((int)Result.Continue, KyText.GetText(20041)));
		//	メニュー設定
		MainList.DataSet = dataSet;
		MainList.ButtonSelected += MainMenuButtonSelected;
		MainList.CreateList();
		MainList.SetGuiEnabled(false);
		//	初期ステート
		mState.ChangeState(StateEnter);
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

	private int StateLeave() {
		if (mState.Sequence == 0) {
			KySoftKeys.Instance.EnableSoftKeys(false);
			MainList.SetGuiEnabled(false);
			ScreenFader.Main.FadeOut();
			if (mSceneResult == Result.NewGame || mSceneResult == Result.Continue) {
				KyAudioManager.Instance.Stop(2.0f);
			}
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			if (!ScreenFader.Main.FadeRunning) {
				switch (mSceneResult) {
				case Result.NewGame: {
					//	メモリ上のみで中断データを初期化。
					//	次にセーブが行われるまでセーブデータは無事。
					if (GameMode == KyConst.GameMode.KukiYomi) {
						KySaveData.IntermitData1.Initialize();
					} else {
						KySaveData.IntermitData2.Initialize();
					}
					KySceneGameMain scene = (KySceneGameMain)ChangeScene("KySceneGameMain");
					scene.GameMode = GameMode;
				} break;
				case Result.Continue: {
					KySceneGameMain scene = (KySceneGameMain)ChangeScene("KySceneGameMain");
					scene.GameMode = GameMode;
				} break;
				case Result.Return:
					ChangeScene("KySceneMainMenu");
					break;
				}
			}
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
	public SpriteTextCustom NaviSprite = null;	//	ナビゲーションスクリプト
	public KyConst.GameMode GameMode = KyConst.GameMode.KukiYomi;	//	ゲームモード
	private KyStateManager mState = new KyStateManager();	//	ステート管理
	private Result mSceneResult = Result.None;	//	シーンリザルト

	#endregion
}
