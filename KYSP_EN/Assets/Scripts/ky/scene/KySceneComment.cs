using UnityEngine;
using System.Collections;

/// <summary>
/// チャプターモードで表示されるつぶやき画面シーン。
/// </summary>
public class KySceneComment : KyScene {

	#region MonoBehaviour Methods
	
	void Start () {
		//	テキストセットアップ
		string comment = KyText.GetText(12000 + StageId);
		KyUtil.SetText(gameObject, "comment/text", comment);
		//	位置調整
		KyUtil.AlignTop(KyUtil.FindChild(gameObject, "navi"), 0);

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
	/// 初期化およびフェードイン用のステート。
	/// </summary>
	private int StateEnter() {
		if (mState.Sequence == 0) {
			KySoftKeys.Instance.SetLabels(KySoftKeys.Label.None, KySoftKeys.Label.None);
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
	/// 終了処理およびフェードアウト用のステート。
	/// </summary>
	private int StateLeave() {
		if (mState.Sequence == 0) {
			KySoftKeys.Instance.SetGuiEnabled(false);
			ScreenFader.Main.FadeOut();
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			if (!ScreenFader.Main.FadeRunning) {
				ChangeScene("KySceneChapterMenu");
			}
		}
		return 0;
	}

	/// <summary>
	/// 入力待ちステート。
	/// </summary>
	private int StateMain() {
		mInput.Update();
		if (mInput.InputTrigger == KyInputDrag.Trigger.TouchDown) {
			mState.ChangeState(StateLeave);
		}
		return 0;
	}

	#endregion

	#region Fields

	public int StageId = 0;		//	問題ID

	private KyStateManager mState = new KyStateManager();	//	ステート管理。
	private KyInputDrag mInput = new KyInputDrag();

	#endregion
}
