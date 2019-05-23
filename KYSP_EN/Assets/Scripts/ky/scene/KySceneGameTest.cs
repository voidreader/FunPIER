using UnityEngine;
using System.Collections;

public class KySceneGameTest : KyScene {

	#region MonoBehaviour Methods

	void Start() {
		mGameEngine = KyGameEngine.Create();
		mNextState = StateGameMain;
		mScore = 0;
	}

	#endregion

	#region State Methods

	private void StateGameMain() {
		if (mSequence == 0) {
			KySoftKeys.Instance.SetLabels(KySoftKeys.Label.Intermit, KySoftKeys.Label.None);
			mGameEngine.StartScript(StageName);
			mSequence++;
		} else if (mSequence == 1) {
			KySoftKeys.Instance.EnableSoftKeys(true);
			mGameEngine.Pause(false);
			mSequence++;
		} else if (mSequence == 2) {
			if (KySoftKeys.Instance.SelectedButtonIndex == 0) {
				KyAudioManager.Instance.PlayOneShot("se_ok");
				KySoftKeys.Instance.EnableSoftKeys(false);
				mNextState = StateIntermission;
				mGameEngine.Pause(true);
				KySoftKeys.Instance.SelectedButtonIndex = -1;
			} else if (!mGameEngine.Running) {
				KySoftKeys.Instance.EnableSoftKeys(false);
				int result = mGameEngine.Result;
				mScore += result;
				mSequence = 0;
			}
		}
	}

	private void StateIntermission() {
		if (mSequence == 0) {
			KySceneDialog scene = (KySceneDialog)PushScene("KySceneDialog");
			//scene.MessageIndex = 1;
			scene.Message = KyText.GetText(22011);	//	「ゲームを中断しますか？」
			mSequence++;
		} else if (mSequence == 1) {
			KySceneDialog scene = GetChildScene<KySceneDialog>();
			if (scene.DialogResult == KySceneDialog.Result.Yes) {
				mNextState = StateLeave;
			} else if (scene.DialogResult == KySceneDialog.Result.No) {
				mNextState = StateGameMain;
				mInitialSequence = 1;
			}
			Destroy(scene.gameObject);
		}
	}

	private void StateLeave() {
		if (mSequence == 0) {
			KySoftKeys.Instance.EnableSoftKeys(false);
			ScreenFader.Main.FadeOut();
			mSequence++;
		} else if (mSequence == 1) {
			if (!ScreenFader.Main.FadeRunning) {
				Destroy(mGameEngine.gameObject);
				ChangeScene("KySceneGameTest");
			}
		}
	}

	#endregion

	#region Fields

	public string StageName;
	private KyGameEngine mGameEngine;
	private int mScore = 0;

	#endregion

}
