using UnityEngine;
using System.Collections;

public class KyButtonSprite : GuiButton {

	#region MonoBahaviour Methods

	protected override void Start() {
		mOriginalScale = transform.localScale;
		base.Start();
	}

	protected override void Update() {
		base.Update();
		if (mScalingUp) {
			mElapsedTime += Time.deltaTime;
			float t = mElapsedTime / DurationTime;
			float s = Mathf.Lerp(1.0f, ScaleUpValue, t);
			//Vector3 scale = new Vector3(s, s, 1.0f);
			transform.localScale = mOriginalScale * s;
			if (t >= 1.0f) {
				mScalingUp = false;
				mElapsedTime = 0;
			}
		}
	}

	#endregion

	#region Methods

	public override void Refresh() {
		if (BaseSprite != null) {
			DebugUtil.Log("refresh");
			if (State == ButtonState.Disabled) {
				EndScale();
			} else if (State == ButtonState.Down) {
				BeginScale();
				BaseSprite.AnimationIndex = SelectedAnimIndex;
				BaseSprite.UpdateAll();
			} else if (State == ButtonState.Up) {
				EndScale();
				BaseSprite.AnimationIndex = UpAnimIndex;
				BaseSprite.UpdateAll();
			} else if (State == ButtonState.Selected) {
				KyAudioManager.Instance.PlayOneShot(ButtonPushSound);
				BaseSprite.AnimationIndex = SelectedAnimIndex;
				BaseSprite.UpdateAll();
				mScalingUp = false;
				EndScale();
			}
		}
	}

	protected void BeginScale() {
		mScalingUp = true;
		mElapsedTime = 0;
	}

	protected void EndScale() {
		mScalingUp = false;
		mElapsedTime = 0;
		transform.localScale = mOriginalScale;
	}

	#endregion

	#region Fields

	public Sprite BaseSprite = null;
	public string ButtonPushSound = "se_ok";
	public int UpAnimIndex = 0;
	public int SelectedAnimIndex = 1;
	
	private bool mScalingUp = false;
	private float mElapsedTime = 0;
	private const float DurationTime = 0.05f;
	private const float ScaleUpValue = 1.1f;
	private Vector3 mOriginalScale = Vector3.one;

	#endregion
}
