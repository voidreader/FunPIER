using UnityEngine;
using System.Collections;

/// <summary>
/// 第○問画面シーン。
/// </summary>
public class KySceneDaimon : KyScene {

	void Start () {
		Assert.AssertNotNull(ScreenPrehab);
		mNextState = StateEnter;
	}

	private void StateEnter() {
		if (mSequence == 0) {
			mScreenObject = (GameObject)Instantiate(ScreenPrehab);
			mNumber = KyUtil.GetComponentInChild<KySpriteNumber>(mScreenObject, "number");
			mButton = KyUtil.GetComponentInChild<KyButton>(mScreenObject, "button");
			Assert.AssertNotNull(mNumber);
			Assert.AssertNotNull(mButton);
			mButton.enabled = false;
			mNumber.Number = Number;

			KySoftKeys.Instance.SetLabels(KySoftKeys.Label.None, KySoftKeys.Label.None);
			ScreenFader.Main.FadeIn();
			BeginWait(0.3f);
			mSequence++;
		} else if (mSequence == 1) {
			if (!Waiting) {
				mNextState = StateMain;
			}
			//if (!ScreenFader.Main.FadeRunning) {
			//	mNextState = StateMain;
			//}
		}
	}

	private void StateLeave() {
		if (mSequence == 0) {
			ScreenFader.Main.FadeOut();
			mSequence++;
		} else if (mSequence == 1) {
			if (!ScreenFader.Main.FadeRunning) {
				Destroy(mScreenObject);
				PopScene();
			}
		}
	}

	private void StateMain() {
		if (mSequence == 0) {
			mButton.enabled = true;
			BeginWait(2.0f);
			mSequence++;
		} else if (mSequence == 1) {
			if (mButton.GetEventFlag(KyButton.EventType.ButtonDown)) {
				mNextState = StateLeave;
			} else if (!Waiting) {
				mNextState = StateLeave;
			}
		}
	}

	public GameObject ScreenPrehab = null;
	public int Number = 0;

	private GameObject mScreenObject = null;
	private KySpriteNumber mNumber = null;
	private KyButton mButton = null;
}
