using UnityEngine;
using System.Collections;

/// <summary>
/// 「オプション」画面シーン。
/// </summary>
public class KySceneSettings : KyScene {

	#region MonoBehaviour Methods

	void Start () {
		//	テキストの設定
		KyUtil.SetText(gameObject, "navi/label", KyText.GetText(20101));
		KyUtil.SetText(gameObject, "buttonGroup/btn0", KyText.GetText(20030));
		KyUtil.SetText(gameObject, "buttonGroup/btn1", KyText.GetText(20031));
		KyUtil.SetText(gameObject, "buttonGroup/btn2", KyText.GetText(20032));
		KyUtil.SetText(gameObject, "buttonGroup/btn3", KyText.GetText(20033));
		//	GUIの設定
		mButtonGroup = KyUtil.GetComponentInChild<GuiButtonGroup>(gameObject, "buttonGroup");
		mButtonGroup.ButtonSelected += MenuButtonSelected;

		for (int i = 0; i < KyConst.VolumeStepCount; ++i) {
			mVolButton[i] = KyUtil.GetComponentInChild<GuiButton>(gameObject, "buttonGroup/btn0/vol" + i);
			mVolButton[i].StateChanged += VolButtonStateChanged;
		}
		for (int i = 0; i < 2; ++i) {
			mYuragiButton[i] = KyUtil.GetComponentInChild<GuiButton>(gameObject, "buttonGroup/btn1/vol" + i);
			mYuragiButton[i].StateChanged += YuragiButtonStateChanged;
		}
		SetGuiEnabled(false);
		//	その他の設定
		for (int i = 0; i < KyConst.VolumeStepCount; ++i) {
			mVolumeTable[i] = (float)KyDesignParams.GetParam(10 + i) / 100.0f;
		}
		LoadSettings();
		mGameEngine = KyGameEngine.Create();
		mGameEngine.transform.parent = transform;
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
			SetGuiEnabled(false);
			//MainList.SetGuiEnabled(false);
			ScreenFader.Main.FadeOut();
			KySaveData.Instance.Save(KySaveData.DataKind.Preferences);
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			if (!ScreenFader.Main.FadeRunning) {
				ChangeScene("KySceneMainMenu");
				Destroy(mGameEngine);
			}
		}
		return 0;
	}

	/// <summary>
	/// 入力待ちのメインステート。
	/// </summary>
	private int StateMain() {
		if (mState.Sequence == 0) {
			KySoftKeys.Instance.SetGuiEnabled(true);
			SetGuiEnabled(true);
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			if (KySoftKeys.Instance.SelectedButtonIndex == 0) {
				KySoftKeys.Instance.ResetAllButtons();
				mState.ChangeState(StateLeave);
			}
		}
		return 0;
	}

	/// <summary>
	/// 「遊び方」選択時のステート。
	/// </summary>
	private int StateHowToPlay() {
		if (mState.Sequence == 0) {
			KySoftKeys.Instance.EnableSoftKeys(false);
			SetGuiEnabled(false);
			ScreenFader.Main.FadeOut();
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			if (!ScreenFader.Main.FadeRunning) {
				KySoftKeys.Instance.SetLabels(KySoftKeys.Label.None, KySoftKeys.Label.None);
				KyUtil.FindChild(gameObject, "buttonGroup").SetActiveRecursively(false);
				KyUtil.FindChild(gameObject, "navi").SetActiveRecursively(false);
				mGameEngine.StartScript("s003");
				mState.Sequence++;
			}
		} else if (mState.Sequence == 2) {
			if (!mGameEngine.Running) {
				KyUtil.FindChild(gameObject, "buttonGroup").SetActiveRecursively(true);
				KyUtil.FindChild(gameObject, "navi").SetActiveRecursively(true);
				mButtonGroup.ResetAllButtons();
				mState.ChangeState(StateEnter);
			}
		}
		return 0;
	}

	/// <summary>
	/// 「セーブデータを消す」選択時のステート。
	/// </summary>
	private int StateInitData() {
		if (mState.Sequence == 0) {
			KySoftKeys.Instance.EnableSoftKeys(false);
			SetGuiEnabled(false);
			KySceneDialog scene = PushScene("KySceneDialog") as KySceneDialog;
			scene.Message = KyText.GetText(22012);
			mState.Sequence++;
		} else if (mState.Sequence == 1) {
			KySceneDialog scene = GetChildScene<KySceneDialog>();
			if (scene.DialogResult == KySceneDialog.Result.Yes) {
				//
				mState.Sequence++;
			} else if (scene.DialogResult == KySceneDialog.Result.No) {
				mState.ChangeState(StateMain);
			}
			mButtonGroup.ResetAllButtons();
			Destroy(scene.gameObject);
		} else if (mState.Sequence == 2) {
			//	ここで初期化。
			KySaveData.Instance.Initialize();
			KySaveData.Instance.SaveAll();

			ScreenFader.Main.FadeOut(2.0f);
			KyAudioManager.Instance.Stop(1.0f);
			mState.Sequence++;
		} else if (mState.Sequence == 3) {
			if (!ScreenFader.Main.FadeRunning) {
				KyAudioManager.Instance.MasterVolume =
					mVolumeTable[KySaveData.PrefsData.SoundVolume];
				ChangeScene("KySceneTitle");
				Destroy(mGameEngine);
			}
		}
		return 0;
	}

	#endregion

	#region Methods

	private void SetGuiEnabled(bool enabled) {
		KyUtil.ForEachChildIn(gameObject,
			delegate(GameObject go) {
				GuiButton button = go.GetComponent<GuiButton>();
				if (button != null) { button.GuiEnabled = enabled; }
			}, true);
	}

	/// <summary>
	///	メインメニューボタン選択時のイベントハンドラ
	/// </summary>
	private int MenuButtonSelected(object sender) {
		GuiButtonGroup button = sender as GuiButtonGroup;
		if (button.SelectedButtonIndex == 2) {
			//	「遊び方」の場合
			mState.ChangeState(StateHowToPlay);
		} else if (button.SelectedButtonIndex == 3) {
			//	「セーブデータを消す」の場合
			mState.ChangeState(StateInitData);
		}
		return 0;
	}

	/// <summary>
	/// 「音量」ボタン変更時のイベントハンドラ
	/// </summary>
	private int VolButtonStateChanged(object sender) {
		GuiButton button = sender as GuiButton;
		if (button.State == GuiButton.ButtonState.Down) {
			int index = button.ButtonIndex;
			for (int i = 0; i < index; ++i) {
				mVolButton[i].State = GuiButton.ButtonState.Selected;
				mVolButton[i].Refresh();
			}
			for (int i = index + 1; i < KyConst.VolumeStepCount; ++i) {
				mVolButton[i].State = GuiButton.ButtonState.Up;
				mVolButton[i].Refresh();
			}
			KyAudioManager.Instance.MasterVolume = mVolumeTable[index];
			KySaveData.PrefsData.SoundVolume = index;
		}
		return 0;
	}

	/// <summary>
	/// 「揺らぎ」ボタン変更時のイベントハンドラ
	/// </summary>
	private int YuragiButtonStateChanged(object sender) {
		GuiButton button = sender as GuiButton;
		if (button.State == GuiButton.ButtonState.Down) {
			int index = button.ButtonIndex;
			mYuragiButton[1 - index].State = GuiButton.ButtonState.Up;
			mYuragiButton[1 - index].Refresh();
			KySaveData.PrefsData.YuragiEnabled = (index == 1);
		}
		return 0;
	}

	/// <summary>
	/// 現在の設定でGUIの状態を初期化します。
	/// </summary>
	private void LoadSettings() {
		int volume = KySaveData.PrefsData.SoundVolume;
		for (int i = 0; i <= volume; ++i) {
			mVolButton[i].State = GuiButton.ButtonState.Selected;
			mVolButton[i].Refresh();
		}
		for (int i = volume + 1; i < KyConst.VolumeStepCount; ++i) {
			mVolButton[i].State = GuiButton.ButtonState.Up;
			mVolButton[i].Refresh();
		}
		int yuragi = KySaveData.PrefsData.YuragiEnabled ? 1 : 0;
		mYuragiButton[yuragi].State = GuiButton.ButtonState.Selected;
		mYuragiButton[yuragi].Refresh();
	}

	#endregion

	#region Fields

	private KyStateManager mState = new KyStateManager();
	private GuiButtonGroup mButtonGroup = null;
	private GuiButton[] mVolButton = new GuiButton[KyConst.VolumeStepCount];
	private GuiButton[] mYuragiButton = new GuiButton[2];
	private float[] mVolumeTable = new float[KyConst.VolumeStepCount];
	private KyGameEngine mGameEngine = null;
	#endregion
}
