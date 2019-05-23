using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 「サクッと空気読み。」ゲーム進行管理シーン。
/// </summary>
public class KySceneLicense : KyScene {

	public enum Result {
		None = 0,
		Return,
	}

	#region MonoBehaviour Methods

	void Start () {
		mGameEngine = KyGameEngine.Create();
		mGameEngine.transform.parent = transform;
		mState.ChangeState(StateIntroduction);
	}

	public override void Update() {
		mState.Execute();
	}

	#endregion

	#region State Methods

	/// <summary>
	/// このシーンを終了するためのステート。
	/// </summary>
	private int StateLeave() {
		if (mState.Sequence == 0) {
			KySoftKeys.Instance.SetGuiEnabled(false);
			ScreenFader.Main.FadeOut();
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			if (!ScreenFader.Main.FadeRunning) {
				if (mSceneResult == Result.Return) {
					ChangeScene("KySceneLicenseMenu");
				}
			}
		}
		return 0;
	}

	private int StateIntroduction() {
		if (mState.Sequence == 0) {
			KySoftKeys.Instance.SetLabels(KySoftKeys.Label.None, KySoftKeys.Label.None);
			//KySoftKeys.Instance.SetLabels(KySoftKeys.Label.Return, KySoftKeys.Label.None); // hamson_debug
			if (LicenseType == 0) {
				mGameEngine.StartScript("s008");
			} else if (LicenseType == 1) {
				mGameEngine.StartScript("s009");
			}
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			if (!mGameEngine.Running) {
				mState.ChangeState(StateLeave);
				mState.Sequence = 1;
				mSceneResult = Result.Return;
			}
		}
		return 0;
	}

	#endregion

	#region Fields
	public int LicenseType = 0;
	private KyStateManager mState = new KyStateManager();	//	ステート管理。
	private KyGameEngine mGameEngine;	//	スクリプトエンジン。
	private Result mSceneResult = Result.None;

	#endregion

}
