using UnityEngine;
using System.Collections;

/// <summary>
/// 問題「立ち位置」用スクリプト。
/// </summary>
public class KyTatiitiOmae : KyScriptObject {

	protected override void Start () {
		base.Start();
		mScriptDriven = false;
		UpdateDegrees(Mathf.PI);
	}
	
	protected override void UpdateCore () {
		if (!CommandManager.PreviewMode) { mInput.Update(); }
		float dx = mInput.DeltaPosition.x;
		if (dx != 0) {
			float degrees = Mathf.Clamp(mRadians - dx * Mathf.PI / ScaleFactor, 0, Mathf.PI);
			UpdateDegrees(degrees);
		}
	}

	public void OnJudge() {
		float degrees = mRadians / Mathf.Deg2Rad;
		if (degrees < 30) {
			CommandManager.SetVariable("result", 1);
		} else if (60 < degrees && degrees < 120) {
			CommandManager.SetVariable("secret", 1);
		}
	}

	private void UpdateDegrees(float radians) {
		if (mRadians == radians) { return; }
		mRadians = radians;
		Vector3 karepos = KaresiObject.transform.localPosition;
		Vector3 kanopos = KanojoObject.transform.localPosition;
		karepos.x = kanopos.x + Radius * Mathf.Cos(mRadians);
		karepos.y = kanopos.y + Radius * Mathf.Sin(mRadians);
		KaresiObject.transform.localPosition = karepos;
	}

	public GameObject KaresiObject = null;
	public GameObject KanojoObject = null;
	public float Speed = 10.0f;
	public float Radius = 60.0f;
	public float ScaleFactor = 100;

	private KyInputDrag mInput = new KyInputDrag();
	private float mRadians = -1;

}
