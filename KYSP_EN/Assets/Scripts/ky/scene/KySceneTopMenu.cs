using UnityEngine;
using System.Collections;

/// <summary>
/// トップメニュー画面シーン。
/// </summary>
public class KySceneTopMenu : KyScene {

	public enum Result {
		None = 0,
		GameMain,
		GameExtra,
		Option,
		Return,
	}

	#region MonoBehaviour Methods

	void Start () {
		Assert.AssertNotNull(TopMenuPrefab);

		mTopMenu = (GameObject)Instantiate(TopMenuPrefab);
		mTopMenuButton = mTopMenu.GetComponent<KyButtonGroup>();
		Assert.AssertNotNull(mTopMenuButton);
		KyButton.EnableButton(mTopMenu, false, true);
		mNextState = StateEnter;
	}

	#endregion

	#region State Methods

	private void StateEnter() {
		if (mSequence == 0) {
			KySoftKeys.Instance.LeftLabel = KySoftKeys.Label.Return;
			KyAudioManager.Instance.Play("bgm_title", true);
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
			KyButton.EnableButton(mTopMenu, false, true);
			KySoftKeys.Instance.EnableSoftKeys(false);
			if (mResult == Result.GameMain) {
				ScreenFader.Main.FadeOut(1.0f);
			} else {
				ScreenFader.Main.FadeOut();
			}
			mSequence++;
		} else if (mSequence == 1) {
			if (!ScreenFader.Main.FadeRunning) {
				mSequence++;
			}
		} else if (mSequence == 2) {
			KySoftKeys.Instance.ClearSoftKeys();
			Destroy(mTopMenu);
			if (mResult == Result.GameMain) {
				ChangeScene(GameMainScene);
			} else if (mResult == Result.GameExtra) {
				ChangeScene(GameExtraScene);
			} else if (mResult == Result.Option) {
				ChangeScene(OptionScene);
			} else if (mResult == Result.Return) {
				ChangeScene("KySceneTitle");
			} else {
				PopScene();
			}
		}
	}

	private void StateMain() {
		if (mSequence == 0) {
			KyButton.EnableButton(mTopMenu, true, true);
			KySoftKeys.Instance.EnableSoftKeys(true);
			mSequence++;
		} else if (mSequence == 1) {
			if (mTopMenuButton.SelectedButtonIndex >= 0) {
				int index = mTopMenuButton.SelectedButtonIndex;
				KyAudioManager.Instance.PlayOneShot("se_ok");
				if (index == 0) {
					mResult = Result.GameMain;
					KyAudioManager.Instance.Stop(3.0f);
					mNextState = StateLeave;
				} else if (index == 1) {
					mResult = Result.GameExtra;
					mNextState = StateLeave;
				} else if (index == 2) {
					mResult = Result.Option;
					mNextState = StateLeave;
				} else if (index == 3) {
					KyApplication.Instance.OpenOfficialUrl();
					KyButton.EnableButton(mTopMenu, false, true);
					KySoftKeys.Instance.EnableSoftKeys(false);
					BeginWait(0.5f);
					mSequence++;
				}
			} else if (KySoftKeys.Instance.SelectedButtonIndex >= 0) {
				KyAudioManager.Instance.PlayOneShot("se_ok");
				if (KySoftKeys.Instance.SelectedButtonIndex == 0) {
					mResult = Result.Return;
					mNextState = StateLeave;
				}
				KySoftKeys.Instance.SelectedButtonIndex = -1;
			}
		} else if (mSequence == 2) {
			if (!Waiting) {
				mTopMenuButton.SelectedButtonIndex = -1;
				mSequence = 0;
			}
		}
	}

	#endregion

	#region Fields

	public GameObject TopMenuPrefab;

	public GameObject GameMainScene;
	public GameObject GameExtraScene;
	public GameObject OptionScene;

	private GameObject mTopMenu;
	private KyButtonGroup mTopMenuButton;
	private KyButton mButtonGameMain;
	private KyButton mButtonExtra;
	private KyButton mButtonOption;
	private KyButton mButtonBuy;

	private Result mResult;
	private GameObject mNextScene;

	#endregion

}
