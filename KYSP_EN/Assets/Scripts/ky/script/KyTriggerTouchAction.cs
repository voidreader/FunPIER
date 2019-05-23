using UnityEngine;
using System.Collections;

public class KyTriggerTouchAction : KyTrigger {

	public enum ActionType {
		None = 0,
		Slide,
	}

	public enum State {
		None = 0,
		TouchUp,
		TouchDown,
		Interval,
	}

	void Start () {
		AdjustRange();
		mDirect2 = Direction.sqrMagnitude;
		mState = State.TouchUp;
	}
	
	void Update () {
		if (KyScriptTime.DeltaTime == 0) {
			return;
		}
		switch (mState) {
		case State.TouchUp: {
			if (Input.GetMouseButton(0)) {
				Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				if ((Range.width == 0 && Range.height == 0) || Range.Contains(wp)) {
					mState = State.TouchDown;
					mStartPos = wp;
					mEndPos = mStartPos;
				}
			}
		} break;
		case State.TouchDown: {
			if (!Input.GetMouseButton(0)) {
				mState = State.TouchUp;
			} else {
				mEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				//float dot = Vector3.Dot((mEndPos - mStartPos) / (Time.deltaTime * 1000), Direction);
				float dot = Vector3.Dot(mEndPos - mStartPos, Direction) / 5;
				if (dot >= mDirect2) {
					Matched = true;
					OnTrigger();
					mState = State.Interval;
				}
			}
		} break;
		case State.Interval: {
			mElapsedTime += KyScriptTime.DeltaTime;
			if (mElapsedTime >= IntervalTime) {
				mState = State.TouchUp;
				mElapsedTime = 0;
			}
		} break;
		}
	}

	protected void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(
			new Vector3((Range.xMax + Range.xMin) / 2, (Range.yMax + Range.yMin) / 2, 0),
			new Vector3(Range.width, Range.height, 1));
	}

	protected void AdjustRange() {
		if (Range.width == 0 && Range.height == 0) {
			Range.yMin = -240;
			Range.yMax = 240;
			Range.xMin = -240;
			Range.xMax = 240;
		}
		if (Range.yMin < -240) { Range.yMin = -240; }
		if (Range.yMax > 240) { Range.yMax = 240; }
	}

	public ActionType Action = ActionType.None;
	public Vector3 Direction = new Vector3(0, 0, 0);	//	反応する方向。大きさも含める。
	public Rect Range;	//	タッチが反応する範囲。(0,0)のときは全範囲。
	public float IntervalTime = 0.0f;

	private State mState = State.None;
	private Vector3 mStartPos = new Vector3(0, 0, 0);
	private Vector3 mEndPos = new Vector3(0, 0, 0);
	private float mDirect2 = 0.0f;
	private float mElapsedTime = 0;
}
