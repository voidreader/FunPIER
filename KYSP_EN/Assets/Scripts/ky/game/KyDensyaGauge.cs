using UnityEngine;
using System.Collections;

/// <summary>
/// 問題「ヘッドフォン」ボリュームメーター用スクリプト。
/// </summary>
public class KyDensyaGauge : KyScriptObject {

	protected override void Start () {
		base.Start();
		UpdateVolume(DefaultVolume);
	}
	
	protected override void UpdateCore () {
		if (!mEnabled) { return; }
		if (!CommandManager.PreviewMode) { mInput.Update(); }
		float dy = mInput.DeltaPosition.y;
		if (dy != 0) {
			UpdateVolume(mVolume + dy / GaugeLength);
			int newStage =
				mVolume > 0.6f ? 2 :
				mVolume > 0.3f ? 1 : 0;
			if (newStage != mStage) {
				mStage = newStage;
				if (CommandManager != null) {
					if (mStage == 2) {
						CommandManager.MoveFrame(1000, 0);
					} else if (mStage == 1) {
						CommandManager.MoveFrame(2000, 0);
					} else if (mStage == 0) {
						CommandManager.MoveFrame(3000, 0);
					}
				}
			}
		}
	}

	private void UpdateVolume(float volume) {
		mVolume = Mathf.Clamp01(volume);
		Vector2 size = GaugeSprite.Size;
		size.y = mVolume * GaugeLength;
		GaugeSprite.Size = size;
		GaugeSprite.UpdateAll();
		KyAudioManager.Instance.Play("bgm_bco_phone", true, mVolume / 2, 1, 0);
		CommandManager.SetVariable("volume", (int)(mVolume * 100));
	}

	public void OnEnabled() {
		mEnabled = true;
	}

	public void OnDisabled() {
		mEnabled = false;
	}

	public void OnJudge() {
		if (CommandManager == null) { return; }
		if (mStage == 0) {
			CommandManager.SetVariable("result", 1);
		} else if (mVolume > 0.9f) {
			CommandManager.SetVariable("result", 2);
		} else {
			CommandManager.SetVariable("result", 0);
		}
		if (mVolume <= 0.05f) {
			CommandManager.SetVariable("secret", 1);
		}
	}

	public SpriteSimple GaugeSprite = null;
	public float GaugeLength = 140;
	public float GaugeScaleFactor = 0.5f;
	public float DefaultVolume = 0.8f;

	private KyInputDrag mInput = new KyInputDrag();	//	ドラッグ入力モジュール
	private float mVolume = 1.0f;		//	音量[0-1]
	private bool mEnabled = false;
	private int mStage = 2;
}
