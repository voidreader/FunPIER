using UnityEngine;
using System.Collections;

public class KyTriggerTouchPosition : KyTrigger {

	void Awake() {
		mCamera = Camera.main;
	}

	void Start() {
		if (Range.width == 0 && Range.height == 0) {
			Range.yMin = -240;
			Range.yMax = 240;
			Range.xMin = -240;
			Range.xMax = 240;
		}
		if (Range.yMin < -240) { Range.yMin = -240; }
		if (Range.yMax > 240) { Range.yMax = 240; }
	}
	
	void Update () {
		if (KyScriptTime.DeltaTime == 0) {
			return;
		}
		if (Input.GetMouseButtonDown(0)) {
			Vector3 wp = mCamera.ScreenToWorldPoint(Input.mousePosition);
			if ((Range.width == 0 && Range.height == 0) || Range.Contains(wp)) {
				Matched = true;
				OnTrigger();
			}
		}
		/*if (Input.touchCount > 0) {
			DebugUtil.Log("touchCount:"+Input.touchCount);
			Touch touch = Input.GetTouch(0);
			Vector3 wp = mCamera.ScreenToWorldPoint(touch.position);
			if ((Range.width == 0 && Range.height == 0) || Range.Contains(wp)) {
				Matched = true;
				OnTrigger();
			}
		}*/
	}

	protected void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(
			new Vector3((Range.xMax + Range.xMin) / 2, (Range.yMax + Range.yMin) / 2, 0),
			new Vector3(Range.width, Range.height, 1));
	}

	/// <summary>
	/// タッチが反応する範囲。(0,0)のときは全範囲。
	/// </summary>
	public Rect Range;

	private Camera mCamera;
}
