using UnityEngine;
using System.Collections;

public class KyButtonLabeled : GuiButton {

	public class Data {
		public int Index;
		public string Label;
		public Data(int index, string label) {
			Label = label;
			Index = index;
		}
	}

	#region MonoBehaviour Methods

	protected override void Update() {
		base.Update();
		if (mScalingUp) {
			mElapsedTime += Time.deltaTime;
			float t = mElapsedTime / DurationTime;
			float s = Mathf.Lerp(1.0f, ScaleUpValue, t);
			Vector3 scale = new Vector3(s, s, 1.0f);
			transform.localScale = scale;
			if (t >= 1.0f) {
				mScalingUp = false;
				mElapsedTime = 0;
			}
		}
	}

	#endregion

	#region Methods

	public override void Refresh() {
		if (LabelSprite != null) {
			if (State == ButtonState.Disabled) {
				LabelSprite.SetColor(Color.gray);
				EndScale();
			} else if (State == ButtonState.Down) {
				LabelSprite.SetColor(SelectedColor);
				if (ScaleUpValue != 1.0f) {
					BeginScale();
				}
			} else if (State == ButtonState.Up) {
				LabelSprite.SetColor(UpColor);
				EndScale();
			} else if (State == ButtonState.Selected) {
				LabelSprite.SetColor(SelectedColor);
				if (!string.IsNullOrEmpty(ButtonPushSound)) {
					KyAudioManager.Instance.PlayOneShot(ButtonPushSound);
				}
				mScalingUp = false;
				EndScale();
			}
		}
	}

	protected override void OnUserDataChanged() {
		//string label = (string)mUserData;
		Data data = (Data)mUserData;
		ButtonIndex = data.Index;
		if (LabelSprite.Text != data.Label) {
			LabelSprite.Text = data.Label;
			LabelSprite.UpdateAll();
		}
	}

	protected void BeginScale() {
		mScalingUp = true;
		mElapsedTime = 0;
	}

	protected void EndScale() {
		mScalingUp = false;
		mElapsedTime = 0;
		transform.localScale = Vector3.one;
	}

	#endregion

	#region Fields

	public SpriteTextCustom LabelSprite = null;
	public string ButtonPushSound = "se_ok";
	public Color UpColor = Color.black;
	public Color SelectedColor = Color.red;
	public float ScaleUpValue = 1.1f;

	private bool mScalingUp = false;
	private float mElapsedTime = 0;
	private const float DurationTime = 0.05f;
	//private const float ScaleUpValue = 1.1f;

	#endregion
}
