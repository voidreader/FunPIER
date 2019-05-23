using UnityEngine;
using System.Collections;

/// <summary>
/// シークレットリストシーン。
/// </summary>
public class KySceneSecretMenu : KyScene {

	#region MonoBehaviour Methods

	void Start() {
		//	位置調整
		KyUtil.AlignTop(KyUtil.FindChild(gameObject, "page"), 0);
		//KyUtil.AlignTop(KyUtil.FindChild(gameObject, "menu"), -100);
		//	リストメニューセットアップ
		mPage = 0;
		ChangePage(mPage);
		MainList.CreateList();
		MainList.SetGridPosition(-1);
		MainList.SetGuiEnabled(false);
		//	スクロールバーセットアップ
		mScrollBar = KyUtil.GetComponentInChild<GuiScrollBar>(gameObject, "scrollBar");
		mScrollBar.PageChanged += ScrollBarPageChanged;
		mScrollBar.Page = mPage;
		//	初期ステート
		mState.ChangeState(StateEnter);
	}

	public override void Update() {
		mState.Execute();
	}

	#endregion

	#region State Methods

	private int StateEnter() {
		if (mState.Sequence == 0) {
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

	private int StateLeave() {
		if (mState.Sequence == 0) {
			KySoftKeys.Instance.EnableSoftKeys(false);
			MainList.SetGuiEnabled(false);
			ScreenFader.Main.FadeOut();
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			if (!ScreenFader.Main.FadeRunning) {
				ChangeScene("KySceneMainMenu");
			}
		}
		return 0;
	}

	private int StateMain() {
		if (mState.Sequence == 0) {
			KySoftKeys.Instance.SetGuiEnabled(true);
			MainList.SetGuiEnabled(true);
			MainList.ButtonGroup.SetGuiEnabled(false);
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			if (KySoftKeys.Instance.SelectedButtonIndex == 0) {
				KySoftKeys.Instance.ResetAllButtons();
				mState.ChangeState(StateLeave);
			}
		}
		return 0;
	}

	#endregion

	#region Methods

	private void ChangePage(int page) {
		mPage = page;
		KyStageInfo.Stage[] stageList = KyApplication.StageInfo.StageList1;
		BitArray secretFlags = KySaveData.RecordData.SecretFlags;
		ArrayList dataSet = new ArrayList();
		for (int i = 0; i < ItemCountInPage; ++i) {
			int stageNo = page * 10 + i;
			int stageId = stageList[stageNo].MainID;
			string text = secretFlags[stageNo] ? KyText.GetText(11000 + stageId) : KyText.GetText(20061);
			dataSet.Add(new KyButtonLabeled.Data(stageId, text));
		}
		MainList.DataSet = dataSet;
		MainList.CreateList();
		MainList.ButtonGroup.SetGuiEnabled(false);
		//	ナビゲーションの問題番号更新。
		KyUtil.SetText(gameObject, "page/number", string.Format("{0}/{1}", page + 1, PageMax));
		//KyUtil.SetText(gameObject, "navi/stageNo1", (page * 10 + 1).ToString());
		//KyUtil.SetText(gameObject, "navi/stageNo2", ((page + 1) * 10).ToString());
		//KyUtil.SetText(gameObject, "menu/group/navi/pageNum", (page + 1).ToString());
	}

	private int ScrollBarPageChanged(object sender) {
		ChangePage(mScrollBar.Page);
		MainList.UpdateList();
		MainList.SetGridPosition(-1);
		return 0;
	}

	#endregion

	#region Fields

	public const int ItemCountInPage = 10;
	public const int PageMax = 10;
	public GuiScrollList MainList = null;
	public SpriteTextCustom NaviSprite = null;
	private KyStateManager mState = new KyStateManager();
	private GuiScrollBar mScrollBar = null;
	private int mPage = -1;	//	現在のページ

	#endregion
}
