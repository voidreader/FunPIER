using UnityEngine;
using System.Collections;

/// <summary>
/// チャプターモードの問題選択メニューシーン。
/// </summary>
public class KySceneChapterMenu : KyScene {

	#region Inner Classes

	enum Result {
		None = 0,
		Return,
		Stage,
	}

	#endregion

	#region MonoBehaviour Methods

	void Start () {
		//	位置調整
		KyUtil.AlignTop(KyUtil.FindChild(gameObject, "page"), 0);
		//	リストメニューセットアップ
		Page = KySaveData.ContextData.ChapterStageNo / 10;
		ChangePage(Page);
		MainList.CreateList();
		MainList.ButtonSelected += MainMenuButtonSelected;
		MainList.SetGridPosition(-1);
		MainList.SetGuiEnabled(false);
		//	プレビューセットアップ
		mGameEngine = KyGameEngine.Create();
		mGameEngine.transform.parent = this.transform;
		mGameEngine.PreviewMode = true;
		Transform screen = mGameEngine.TargetScreenTransform;
		screen.localScale = new Vector3(0.75f, 0.75f, 1.0f);
		screen.localPosition = new Vector3(+20, -180, 2.5f);
		KyUtil.AlignBottom(screen.gameObject, 180);
		//	スクロールバーセットアップ
		mScrollBar = KyUtil.GetComponentInChild<GuiScrollBar>(gameObject, "scrollBar");
		mScrollBar.PageChanged += ScrollBarPageChanged;
		mScrollBar.Page = Page;
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
			KyAudioManager.Instance.Stop();
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
			KyAudioManager.Instance.Stop();
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			if (!ScreenFader.Main.FadeRunning) {
				switch (mSceneResult) {
				case Result.Stage: {
					KyAudioManager.Instance.StopAll();
					KySceneGameChapter scene = (KySceneGameChapter)ChangeScene("KySceneGameChapter");
					KySaveData.ContextData.ChapterStageNo = mSelectedStageNo;
					KyStageInfo.Stage[] stageList = KyApplication.StageInfo.StageList1;
					int stageId = stageList[mSelectedStageNo].MainID;
					scene.StageId = stageId;
				} break;
				case Result.Return:
					ChangeScene("KySceneMainMenu");
					break;
				}
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

	#endregion

	#region Methods

	private void ChangePage(int page) {
		Page = page;
		KyStageInfo.Stage[] stageList = KyApplication.StageInfo.StageList1;
		ArrayList dataSet = new ArrayList();
		for (int i = 0; i < ItemCountInPage; ++i) {
			int stageNo = page * 10 + i;
			int stageId = stageList[stageNo].MainID;
			dataSet.Add(new KyButtonLabeled.Data(stageNo, KyText.GetText(10000 + stageId)));
		}
		mDataSet = dataSet;
		MainList.DataSet = dataSet;
		//	ナビゲーションの問題番号更新。
		KyUtil.SetText(gameObject, "page/number", string.Format("{0}/{1}", page+1, PageMax));
	}

	private int MainMenuButtonSelected(object sender) {
		int index = MainList.SelectedButtonIndex;
		if (mSelectedStageNo != index) {
			mSelectedStageNo = index;
			KyStageInfo.Stage[] stageList = KyApplication.StageInfo.StageList1;
			int stageId = stageList[mSelectedStageNo].MainID;
			KyAudioManager.Instance.StopAll();
			
			mGameEngine.StartScriptAsPreview(string.Format("{0:D4}", stageId));
			MainList.SetGuiEnabled(true);
			MainList.SelectedButton.State = GuiButton.ButtonState.Up;
			if (mState.State == StateMain) {
				KyAudioManager.Instance.PlayOneShot("se_cursor");
			}
		} else {
			KyAudioManager.Instance.StopAll();
			KyAudioManager.Instance.PlayOneShot("se_ok");
			mSceneResult = Result.Stage;
			mState.ChangeState(StateLeave);
		}
		return 0;
	}

	private int ScrollBarPageChanged(object sender) {
		ChangePage(mScrollBar.Page);
		MainList.UpdateList();
		MainList.SetGridPosition(-1);
		return 0;
	}

	#endregion

	#region Fields

	public const int ItemCountInPage = 10;	//	ページ内の項目数
	public const int PageMax = 10;	//	ページ数
	public int Page = 0;	//	現在のページ
	public GuiScrollList MainList = null;	//	メインリスト
	public SpriteTextCustom NaviSprite = null;	//
	private KyStateManager mState = new KyStateManager();	//	ステート管理
	private GuiScrollBar mScrollBar = null;
	private ArrayList mDataSet = null;	//	現在のメニューリストのデータセット
	private Result mSceneResult = Result.None;	//	シーンリザルト
	private KyGameEngine mGameEngine = null;	//	スクリプトエンジン
	private int mSelectedStageNo = -1;	//	GUIで選択された問題番号

	#endregion
}
