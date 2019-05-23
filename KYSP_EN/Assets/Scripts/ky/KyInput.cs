using UnityEngine;
using System.Collections;

public class KyInputCrossKey {

	public enum State {
		None = 0,
		TouchUp,
		TouchDown,
	}

	public enum Direction {
		None = 0,
		Up = 0x01 << 0,
		Down = 0x01 << 1,
		Left = 0x01 << 2,
		Right = 0x01 << 3,
		All = Up | Down | Left | Right,
	}

	public KyInputCrossKey() {
		Clear();
	}

	public void Clear() {
		mState = State.TouchUp;
		mStartPos = new Vector3(0, 0, 0);
		mEndPos = new Vector3(0, 0, 0);
		mDirSlide = Direction.None;
	}
	
	public void Update () {
		if (!mEnabled) { return; }
		mDirSlide = Direction.None;
		switch (mState) {
		case State.TouchUp: {
			if (Input.GetMouseButton(0)) {
				Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				if (HitTest(pos)) {
					mState = State.TouchDown;
					mStartPos = pos;
					mEndPos = mStartPos;
				}
			}
		} break;
		case State.TouchDown: {
			if (!Input.GetMouseButton(0)) {
				mState = State.TouchUp;
			} else {
				mEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Vector3 dif = mEndPos - mStartPos;
				if ((mDirMask & Direction.Up) != 0 && dif.y >= mThreshold) {
					mDirSlide = Direction.Up;
					mStartPos = mEndPos;
					//mStartPos.y += mThreshold;
				} else if ((mDirMask & Direction.Down) != 0 && dif.y <= -mThreshold) {
					mDirSlide = Direction.Down;
					mStartPos = mEndPos;
					//mStartPos.y -= mThreshold;
				} else if ((mDirMask & Direction.Left) != 0 && dif.x <= -mThreshold) {
					mDirSlide = Direction.Left;
					mStartPos = mEndPos;
					//mStartPos.x -= mThreshold;
				} else if ((mDirMask & Direction.Right) != 0 && dif.x >= mThreshold) {
					mDirSlide = Direction.Right;
					mStartPos = mEndPos;
					//mStartPos.x += mThreshold;
				}
				if (mDirSlide != Direction.None) {
					//mState = State.TouchUp;
					//mStartPos = mEndPos;
				}
			}
		} break;
		}
	}

	public bool HitTest(Vector3 pos) {
		if (mHitRect.width == 0 || mHitRect.height == 0) {
			return true;
		} else {
			return mHitRect.Contains(pos);
		}
	}

	public bool Enabled {
		get { return mEnabled; }
		set { mEnabled = value; }
	}

	public Direction DirectionMask {
		get { return mDirMask; }
		set { mDirMask = value; }
	}

	public float Threshold {
		get { return mThreshold; }
		set { mThreshold = value; }
	}

	public Direction Slide {
		get { return mDirSlide; }
	}

	public GameObject Owner {
		get { return mOwner; }
		set { mOwner = value; }
	}

	private bool mEnabled = true;						//	trueのとき入力処理が有効。
	private Direction mDirMask = Direction.All;			//	入力を監視する方向をマスク。
	private float mThreshold = 5.0f;					//	入力判定のための閾値。
	private Direction mDirSlide = Direction.None;		//	入力判定結果。Updateの度にクリア。
	private State mState = State.None;					//	内部管理用のステート。
	private Vector3 mStartPos = new Vector3(0, 0, 0);	//	タッチを開始した位置。
	private Vector3 mEndPos = new Vector3(0, 0, 0);		//	現在のタッチ位置。
	private Rect mHitRect = new Rect(-240, -240, 480, 480);
	private GameObject mOwner = null;
}

public class KyInputDrag {

	public enum State {
		None = 0,
		TouchUp,
		TouchDown,
		
	}

	public enum Trigger {
		None = 0,
		TouchUp = 1 << 0,
		TouchDown = 1 << 1,
		DoubleTouchDown = 1 << 2,
	}

	public void Update() {
		if (!mEnabled) { return; }
		mTrigger = Trigger.None;
		switch (mState) {
		case State.TouchUp: {
				if (Input.GetMouseButtonDown(0)) {
					Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					if (HitTest(pos)) {
						mState = State.TouchDown;
						mTrigger = Trigger.TouchDown;
						mStartPos = pos;
						mEndPos = mStartPos;
						if (mSingleClickTime != 0 && (Time.time - mSingleClickTime) < mDoubleClickTimeSpan) {
							mTrigger |= Trigger.DoubleTouchDown;
							mSingleClickTime = 0;
						} else {
							mSingleClickTime = Time.time;
						}
					}
				}
			} break;
		case State.TouchDown: {
				if (!Input.GetMouseButton(0)) {
					mState = State.TouchUp;
					mTrigger = Trigger.TouchUp;
					mDeltaPos = Vector3.zero;
				} else {
					Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					mDeltaPos = pos - mEndPos;
					mDragPos = pos - mStartPos;
					mEndPos = pos;
				}
			} break;
		}
	}

	public bool HitTest(Vector3 pos) {
		if (mHitRect.width == 0 || mHitRect.height == 0) {
			return true;
		} else {
			if (mOwner == null) {
				return mHitRect.Contains(pos);
			} else {
				Rect rect = mHitRect;
				rect.xMin = mHitRect.xMin + mOwner.transform.position.x;
				rect.yMin = mHitRect.yMin + mOwner.transform.position.y;
				rect.width = mHitRect.width * mOwner.transform.lossyScale.x;
				rect.height = mHitRect.height * mOwner.transform.lossyScale.y;
				return rect.Contains(pos);
			}
		}
	}

	public bool Enabled {
		get { return mEnabled; }
		set { mEnabled = value; }
	}

	public Rect HitRect {
		get { return mHitRect; }
		set { mHitRect = value; }
	}

	public Vector3 DeltaPosition {
		get { return mDeltaPos; }
	}

	public Vector3 DragPosition {
		get { return mDragPos; }
	}

	public State InputState {
		get { return mState; }
	}

	public Trigger InputTrigger {
		get { return mTrigger; }
	}

	public Vector3 StartPosition {
		get { return mStartPos; }
	}

	public Vector3 EndPosition {
		get { return mEndPos; }
	}

	public GameObject Owner {
		get { return mOwner; }
		set { mOwner = value; }
	}

	private bool mEnabled = true;						//	trueのとき入力処理が有効。
	private Rect mHitRect = new Rect(-240, -240, 480, 480);	//	タッチ開始可能な範囲。0のときは制限なし。
	private State mState = State.TouchUp;					//	内部管理用のステート。
	private Trigger mTrigger = Trigger.None;
	private Vector3 mStartPos = new Vector3(0, 0, 0);	//	タッチを開始した位置。
	private Vector3 mEndPos = new Vector3(0, 0, 0);		//	現在のタッチ位置。
	private Vector3 mDeltaPos = new Vector3(0, 0, 0);	//	前回のUpdateからの位置変位。
	private Vector3 mDragPos = new Vector3(0, 0, 0);	//
	private GameObject mOwner = null;
	private float mSingleClickTime = 0;
	private float mDoubleClickTimeSpan = 0.3f;
}
