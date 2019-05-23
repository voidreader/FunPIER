using UnityEngine;
using System.Collections;

/// <summary>
/// 問題「クーラー」のリモコン用スクリプト。
/// </summary>
public class KyDenkiRemocon : KyScriptObject {

	protected override void Start() {
		base.Start();
		Assert.AssertNotNull(SpriteNumber);
		Assert.AssertNotNull(SpriteMeter);
		Assert.AssertNotNull(mGameEngine);
		SetTemplature(SpriteNumber.Number);
	}

	void SetTemplature(int temp) {
		mTemplature = Mathf.Clamp(temp, SpriteNumber.MinNumber, SpriteNumber.MaxNumber);
		SpriteNumber.Number = mTemplature;
		KyAudioManager.Instance.Play("bgm_aircon", true, GetSoundVolume(mTemplature), 1.0f, 1.0f);
		CommandManager.SetVariable("temp", mTemplature);
		if (mTemplature >= 28) {
			CommandManager.MoveFrame(3000, 0);
		}
	}

	void OnUpTemplature() {
		SetTemplature(SpriteNumber.Number + 1);
	}

	void OnDownTemplature() {
		SetTemplature(SpriteNumber.Number - 1);
	}

	void OnPower() {
		SpriteMeter.Size.y = 0;
		SpriteMeter.UpdateAll();
	}

	private float GetMeterLength(float templature) {
		return Mathf.Lerp(MeterMax, MeterMin, (templature - TemplatureMin) / (TemplatureMax - TemplatureMin)); 
	}

	private float GetSoundVolume(float templature) {
		return Mathf.Lerp(SoundMax, SoundMin, (templature - TemplatureMin) / (TemplatureMax - TemplatureMin)); 
	}

	public const float MeterMin = 10;
	public const float MeterMax = 140;
	public const float SoundMin = 0.0f;
	public const float SoundMax = 1.0f;
	public const float TemplatureMin = 15;
	public const float TemplatureMax = 30;
	public KySpriteNumber SpriteNumber = null;
	public SpriteSimple SpriteMeter = null;
	private int mTemplature;
}
