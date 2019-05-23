using UnityEngine;
using System.Collections;


public class KySceneRecord : KyScene {

	#region MonoBehaviour Methods

	void Start () {



        GameObject go = (GameObject)Instantiate(RecordViewPrefab);
		go.name = "recordView";
		go.transform.parent = transform;
		mRecordView = go.GetComponent<KyRecordView>();
		mRecordView.InitShowMessage = true;
		mRecordView.ViewMode = 1;
		mRecordView.ChartAnimDuration = 1.0f;

		//	画面上端に合わせる。
		Transform navi = transform.Find("navi");
		Vector3 pos = navi.localPosition;
		pos.y = Camera.main.orthographicSize;
		navi.localPosition = pos;
		Transform mode = transform.Find("recordView/mode");
		pos = mode.localPosition;
		pos.y = Camera.main.orthographicSize - 68;
		mode.localPosition = pos;

		mArrowLeft = KyUtil.GetComponentInChild<KyButtonSprite>(gameObject, "navi/leftArrow");
		mArrowRight = KyUtil.GetComponentInChild<KyButtonSprite>(gameObject, "navi/rightArrow");
		mArrowLeft.ButtonSelected += ArrowLeftButtonSelected;
		mArrowRight.ButtonSelected += ArrowRightButtonSelected;

		if (KySaveData.RecordData.GameClearCount[1] != 0) {
			mPageMax = PageMax;
		} else {
			mPageMax = 3;
		}

		ChangePage(0);
		mRecordView.BeginAnimChart();
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

	/// <summary>
	/// このシーンをフェードアウトして終了するためのステート。
	/// </summary>
	private int StateLeave() {
		if (mState.Sequence == 0) {
			KySoftKeys.Instance.EnableSoftKeys(false);
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
			KySoftKeys.Instance.EnableSoftKeys(true);
			//mRecordView.BeginAnimChart();
			//mRecordView.UpdateChart(0.0f);
			mState.Sequence ++;
		} else if (mState.Sequence == 1) {
			if (KySoftKeys.Instance.SelectedButtonIndex == 0) {
				KySoftKeys.Instance.ResetAllButtons();
				mState.ChangeState(StateLeave);
			}
		}
		return 0;
	}

	private int StateMovePage() {
		return 0;
	}

	#endregion

	#region Methods

	private void ChangePage(int page) {
		Page = page;
		int mode = page / 3;
		int recordType = page % 3;
		KyUtil.SetText(gameObject, "navi/title", KyText.GetText(20070+recordType));
		float[] scores = KySaveData.RecordData.GetScoresInRate(mode, recordType);
		mRecordView.GameMode = mode == 0 ? KyConst.GameMode.KukiYomi : KyConst.GameMode.Yomanai;
		mRecordView.SetupScore(scores);
		//mRecordView.BeginAnimChart();
		//mRecordView.ShowMessage(true);
	}

	private int ArrowLeftButtonSelected(object sender) {
		ChangePage((Page - 1 + mPageMax) % mPageMax);
		mRecordView.UpdateChart(1.0f);
		mArrowLeft.State = GuiButton.ButtonState.Up;
		return 0;
	}

	private int ArrowRightButtonSelected(object sender) {
		ChangePage((Page + 1) % mPageMax);
		mRecordView.UpdateChart(1.0f);
		mArrowRight.State = GuiButton.ButtonState.Up;
		return 0;
	}

	#endregion

	#region Fields

	public const int PageMax = 6;

	public GameObject RecordViewPrefab = null;
	public int Page = 0;

	private KyStateManager mState = new KyStateManager();	//	ステート管理。
	private KyRecordView mRecordView = null;
	private KyButtonSprite mArrowLeft = null;
	private KyButtonSprite mArrowRight = null;
	private int mPageMax = 1;

	#endregion
}
