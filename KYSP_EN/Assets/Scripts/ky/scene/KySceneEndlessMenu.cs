using UnityEngine;
using System.Collections;

/// <summary>
/// エンドレスモード（エクストラモード）の開始準備画面シーン。
/// </summary>
public class KySceneEndlessMenu : KyScene {

	public enum Result {
		None,
		Return,
		BeginGame,
	}

	#region MonoBehaviour Methods

	public void Start () {
		Assert.AssertNotNull(ScreenPrefab);
		mNextState = StateEnter;
	}

	public void OnDestroy() {
		Destroy(mScreenObject);
	}

	#endregion

	#region State Methods

	private void StateEnter() {
		if (mSequence == 0) {
			mScreenObject = (GameObject)Instantiate(ScreenPrefab);
			mButtonStart = KyUtil.GetComponentInChild<KyButton>(mScreenObject, "btnStart");
			Assert.AssertNotNull(mButtonStart);
			mScoreNumber = KyUtil.GetComponentInChild<KySpriteNumber>(mScreenObject, "score");
			Assert.AssertNotNull(mScoreNumber);
			mScoreNumber.Number = KySaveData.RecordData.ExtraScores[0];
			mScoreNumber.Update();
			mButtonStart.enabled = false;

			Transform ten = KyUtil.GetComponentInChild<Transform>(mScreenObject, "ten");
			Vector3 pos = ten.localPosition;
			pos.x = mScoreNumber.transform.localPosition.x + mScoreNumber.AnchorX + 40;
			ten.localPosition = pos;

			KyAudioManager.Instance.Play("bgm_title", true);
			KySoftKeys.Instance.SetLabels(KySoftKeys.Label.Return, KySoftKeys.Label.None);
			ScreenFader.Main.FadeIn();
			BeginWait(0.3f);
			mSequence++;
		} else if (mSequence == 1) {
			if (!Waiting) {
				mNextState = StateMain;
			}
		}
	}

	private void StateLeave() {
		if (mSequence == 0) {
			KySoftKeys.Instance.EnableSoftKeys(false);
			ScreenFader.Main.FadeOut();
			mSequence++;
		} else if (mSequence == 1) {
			if (!ScreenFader.Main.FadeRunning) {
				Destroy(mScreenObject);
				if (mResult == Result.Return) {
					ChangeScene("KySceneTopMenu");
				} else if (mResult == Result.BeginGame) {
					ChangeScene("KySceneEndless");
				} else {
					Assert.AssertTrue(false);
					ChangeScene("KySceneTopMenu");
				}
				mNextState = null;
			}
		}
	}

	private void StateMain() {
		if (mSequence == 0) {
			KySoftKeys.Instance.EnableSoftKeys(true);
			mButtonStart.enabled = true;
			mButtonStart.Selected = false;
			mSequence++;
		} else if (mSequence == 1) {
			if (KySoftKeys.Instance.SelectedButtonIndex == 0) {
				KySoftKeys.Instance.SelectedButtonIndex = -1;
				KySoftKeys.Instance.EnableSoftKeys(false);
				KyAudioManager.Instance.PlayOneShot("se_ok");
				mResult = Result.Return;
				mNextState = StateLeave;
			} else if (mButtonStart.Selected) {
				KyAudioManager.Instance.PlayOneShot("se_ok");
				mResult = Result.BeginGame;
				mNextState = StateLeave;
			}
		}
	}

	#endregion

	#region Fields

	public GameObject ScreenPrefab;

	private GameObject mScreenObject;
	private KyButton mButtonStart;
	private KySpriteNumber mScoreNumber;
	private Result mResult;

	#endregion

}
