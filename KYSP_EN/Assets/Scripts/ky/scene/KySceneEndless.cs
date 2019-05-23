using UnityEngine;
using System.Collections;

/// <summary>
/// エンドレスモードのゲーム画面シーン。
/// </summary>
public class KySceneEndless : KyScene {

	void Start () {
		Assert.AssertNotNull(HighScorePrefab);
		mGameEngine = KyGameEngine.Create();
		mGameEngine.EndlessMode = true;
		mNextState = StateGameMain;
	}

	private void StateGameReady() {
		mNextState = StateGameMain;
	}

	private void StateGameMain() {
		if (mSequence == 0) {
			KySoftKeys.Instance.SetLabels(KySoftKeys.Label.Intermit, KySoftKeys.Label.None);
			mGameEngine.StartScript("0010");
			mSequence++;
		} else if (mSequence == 1) {
			KySoftKeys.Instance.EnableSoftKeys(true);
			mGameEngine.Pause(false);
			mSequence++;
		} else if (mSequence == 2) {
			if (KySoftKeys.Instance.SelectedButtonIndex == 0) {
				KySoftKeys.Instance.SelectedButtonIndex = -1;
				KyAudioManager.Instance.PlayOneShot("se_ok");
				KySoftKeys.Instance.EnableSoftKeys(false);
				mNextState = StateIntermission;
				mGameEngine.Pause(true);
			} else if (!mGameEngine.Running) {
				if (KySaveData.RecordData.ExtraScores[0] < mGameEngine.Result) {
					mNextState = StateNewRecord;
				} else {
					mNextState = StateLeave;
					mInitialSequence = 1;
				}
			}
		}
	}

	private void StateIntermission() {
		if (mSequence == 0) {
			KySceneDialog dialog = (KySceneDialog)PushScene("KySceneDialog");
			dialog.Message = KyText.GetText(22011);	//	「ゲームを中断しますか？」
			//dialog.MessageIndex = 1;
			mSequence++;
		} else if (mSequence == 1) {
			KySceneDialog dialog = GetChildScene<KySceneDialog>();
			if (dialog.DialogResult == KySceneDialog.Result.Yes) {
				KyAudioManager.Instance.Stop();
				mNextState = StateLeave;
			} else if (dialog.DialogResult == KySceneDialog.Result.No) {
				mNextState = StateGameMain;
				mInitialSequence = 1;
			}
			Destroy(dialog);
		}
	}

	private void StateNewRecord() {
		//	最高記録更新画面
		if (mSequence == 0) {
			KySoftKeys.Instance.SetLabels(KySoftKeys.Label.None, KySoftKeys.Label.None);
			mHighScore = (GameObject)Instantiate(HighScorePrefab);
			mNumber = KyUtil.GetComponentInChild<KySpriteNumber>(mHighScore, "score");
			Assert.AssertNotNull(mNumber);
			mNumber.Number = mGameEngine.Result;
			mNumber.Update();
			KySaveData.RecordData.ExtraScores[0] = mGameEngine.Result;
			KySaveData.Instance.Save(KySaveData.DataKind.Record);
			KyAudioManager.Instance.PlayOneShot("jg_goukaku");

			Transform ten = KyUtil.GetComponentInChild<Transform>(mHighScore, "ten");
			Vector3 pos = ten.localPosition;
			pos.x = mNumber.transform.localPosition.x + mNumber.AnchorX + 40;
			ten.localPosition = pos;

			mSequence++;
		} else if (mSequence == 1) {
			ScreenFader.SetColorMain(new Color(0, 0, 0, 0));
			mSequence++;
		} else if (mSequence == 2) {
			if (Input.GetMouseButton(0)) {
				mNextState = StateSubmitScore;
			}
		}
	}

	private void StateSubmitScore() {
		//	ここでハイスコアを送信します。


		mNextState = StateLeave;
	}

	private void StateLeave() {
		if (mSequence == 0) {
			ScreenFader.Main.FadeOut();
			mSequence++;
		} else if (mSequence == 1) {
			if (!ScreenFader.Main.FadeRunning) {
				Destroy(mGameEngine.gameObject);
				Destroy(mHighScore);
				ChangeScene("KySceneEndlessMenu");
			}
		}
	}

	public GameObject HighScorePrefab;

	private KyGameEngine mGameEngine;
	private GameObject mHighScore;
	private KySpriteNumber mNumber;
}
