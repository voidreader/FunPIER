using System;
using UnityEngine;
using System.Collections;

public class KySoftKeys : GuiButtonGroup {

	#region Inner Classes

	public enum Label {
		None = 0,
		Return = 1,
		Intermit = 2,
		Tweet = 3,
		Terminate = 4,
		End,
	}

	public enum State {
		None = 0,
		Enter,
		Leave,
	}

	#endregion

	#region MonoBehaviour Methods

	protected new void Awake() {
		Assert.AssertTrue(sInstance == null);
		sInstance = this;
	}

	protected void Start() {
		Vector3 pos = transform.localPosition;
		pos.y = -Camera.main.orthographicSize;
		transform.localPosition = pos;

		AddButton(LeftButton);
		AddButton(RightButton);

		LabelName = new string[(int)Label.End];
		LabelName[(int)Label.None] = "";
		LabelName[(int)Label.Return] = KyText.GetText(20011);
		LabelName[(int)Label.Intermit] = KyText.GetText(20012);
		LabelName[(int)Label.Tweet] = KyText.GetText(20013);
		LabelName[(int)Label.Terminate] = KyText.GetText(20010);
		//SetLabels(Label.None, Label.None);
	}

	protected void Update() {
		if (!GuiEnabled) { return; }
		if (Application.platform != RuntimePlatform.IPhonePlayer) {
			//	Backボタンによる左ソフトキーの起動
			if (mLeftLabel != Label.None && Input.GetKeyDown(KeyCode.Escape)) {
				DebugUtil.Log("back button!");
				SelectButton(0);
			}
		}
	}

	#endregion

	#region Methods

	//	互換性のため(非推奨)
	public void EnableSoftKeys(bool enable) {
		SetGuiEnabled(enable);
	}

	public void ClearSoftKeys() {
		LeftLabel = Label.None;
		RightLabel = Label.None;
	}

	public void SetLabels(Label left, Label right) {
		LeftLabel = left;
		RightLabel = right;
	}

	private void SetLabel(KyButtonLabeled button, Label label) {
		button.gameObject.SetActiveRecursively(label == Label.None ? false : true);
        button.LabelSprite.CharacterSize = new Vector3(30, 30);//new Vector3(44, 44);
		button.LabelSprite.CharacterSpan = new Vector2(0, 0);
		if (label == Label.Tweet) {
			button.LabelSprite.CharacterSize = new Vector3(36, 44);
			button.LabelSprite.CharacterSpan = new Vector2(-5, 0);
		}
		button.LabelSprite.Text = LabelName[(int)label];
		button.LabelSprite.UpdateAll();
	}

	#endregion

	#region Properties

	public Label LeftLabel {
		get { return mLeftLabel; }
		set {
			mLeftLabel = value;
			SetLabel(LeftButton, value);
		}
	}

	public Label RightLabel {
		get { return mRightLabel; }
		set {
			mRightLabel = value;
			SetLabel(RightButton, value);
		}
	}

	public static KySoftKeys Instance {
		get { return sInstance; }
	}

	#endregion

	#region Fields

	public KyButtonLabeled LeftButton = null;
	public KyButtonLabeled RightButton = null;
	private Label mLeftLabel = Label.None;
	private Label mRightLabel = Label.None;

	private static KySoftKeys sInstance;
	private static string[] LabelName = null;

	#endregion
}
