using UnityEngine;
using System.Collections;

/// <summary>
/// 体験版ゲーム後の製品版購入シーン。
/// </summary>
public class KyScenePurchase : KyScene {

	public void Start() {
		mNextState = StateEnter;
	}

	private void StateEnter() {
		if (mSequence == 0) {
			mScreenObject = (GameObject)Instantiate(ScreenPrefab);
			mButtonPurchase = KyUtil.GetComponentInChild<KyButton>(mScreenObject, "btnPurchase");
			mButtonReturn = KyUtil.GetComponentInChild<KyButton>(mScreenObject, "btnReturn");
			Assert.AssertNotNull(mScreenObject);
			Assert.AssertNotNull(mButtonPurchase);
			Assert.AssertNotNull(mButtonReturn);

			KyButton.EnableButton(mScreenObject, false, true);
			ScreenFader.Main.FadeIn();
			mSequence++;
		} else if (mSequence == 1) {
			if (!ScreenFader.Main.FadeRunning) {
				mNextState = StateMain;
			}
		}
	}

	private void StateLeave() {
		if (mSequence == 0) {
			ScreenFader.Main.FadeOut();
			mSequence++;
		} else if (mSequence == 1) {
			if (!ScreenFader.Main.FadeRunning) {
				Destroy(mScreenObject);
				ChangeScene("KySceneTopMenu");
			}
		}
	}

	private void StateMain() {
		if (mSequence == 0) {
			KyButton.EnableButton(mScreenObject, true, true);
			mSequence++;
		} else if (mSequence == 1) {
			if (mButtonPurchase.GetEventFlag(KyButton.EventType.ButtonDown)) {
				KyAudioManager.Instance.PlayOneShot("se_ok");
				KyApplication.Instance.OpenOfficialUrl();
				mSequence = 0;  
			} else if (mButtonReturn.GetEventFlag(KyButton.EventType.ButtonDown)) {
				KyAudioManager.Instance.PlayOneShot("se_ok");
				mNextState = StateLeave;
			}
		}
	}

	public GameObject ScreenPrefab;

	private GameObject mScreenObject;
	private KyButton mButtonPurchase;
	private KyButton mButtonReturn;
}
