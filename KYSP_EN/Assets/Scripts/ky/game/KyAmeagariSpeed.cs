using UnityEngine;
using System.Collections;

/// <summary>
/// 問題「雨あがり」のスピードメーター用スクリプト。
/// </summary>
public class KyAmeagariSpeed : KyScriptObject {

	protected override void Start () {
		base.Start();
		Transform car = transform.parent.Find("car");
		if (car != null) {
			CarObject = car.gameObject;
		}
		mScriptDriven = true;
		UpdateSpeed(DefaultSpeed);
	}
	
	protected override void UpdateCore() {
		if (!mEnabled) { return; }
		if (!CommandManager.PreviewMode) { mInput.Update(); }
		float speed = mSpeed;
		//	加速度による速度変化
		speed += Acceleration * DeltaTime;
		//	ドラッグによる速度変化
		float dy = mInput.DeltaPosition.y;
		if (dy != 0) {
			speed += dy / GaugeLength * DragFactor;
		}
		speed = Mathf.Clamp01(speed);
		if (speed != mSpeed) {
			UpdateSpeed(speed);
		}
		//	車の移動
		Vector3 pos = CarObject.transform.localPosition;
		pos.y += mSpeed * SpeedFactor * DeltaTime;
		CarObject.transform.localPosition = pos;
	}

	private void UpdateSpeed(float speed) {
		mSpeed = speed;
		Vector2 size = GaugeSprite.Size;
		size.y = mSpeed * GaugeLength;
		GaugeSprite.Size = size;
		GaugeSprite.UpdateAll();
		//KyAudioManager.Instance.Play("bgm_car", true, mSpeed, 1, 0);
		CommandManager.SetVariable("speed", mSpeed);
	}

	public float DefaultSpeed = 0.5f;	//	初期スピード。
	public float Acceleration = 0.2f;	//	１秒間に増加するスピード量。
	public float SpeedFactor = 30.0f;	//	スピード最大時の車のピクセル移動量。
	public float DragFactor = 0.5f;
	public float GaugeLength = 120.0f;	//	スピードメーターの最大長。
	public GameObject CarObject = null;
	public SpriteSimple GaugeSprite = null;

	private bool mEnabled = true;
	private float mSpeed;	//	スピードメータの量。0が最低、1が最速。
	private KyInputDrag mInput = new KyInputDrag();	//	ドラッグ入力モジュール
}
