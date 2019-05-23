using UnityEngine;
using System.Collections;

public class KyDragControl : MonoBehaviour {

	void Awake() {
		mCamera = Camera.main;
		mTouchState = TouchState.None;
	}

	void Start() {

	}

	void Update() {
		if (KyScriptTime.DeltaTime == 0) {
			return;
		}
		/*if (mTouchState == TouchState.None) {
			if (Input.touchCount > 0) {
				Touch touch = Input.GetTouch(0);
				if (touch.phase == TouchPhase.Began) {
					if (!OnlyGrab || Contains(touch.position)) {
						KyTweener tweener = GetComponent<KyTweener>();
						if (tweener) {
							Destroy(tweener);
						}
						mOrigin = transform.position;
						Vector3 wpoint = mCamera.ScreenToWorldPoint(touch.position);
						wpoint.z = transform.position.z;
						mOldPosition = wpoint;
					}
				} else if (touch.phase == TouchPhase.Moved) {
					Vector3 wpoint = mCamera.ScreenToWorldPoint(touch.position);
					Vector3 tpoint = transform.position;
					wpoint.z = transform.position.z;
					tpoint += wpoint - mOldPosition;
					mOldPosition = wpoint;
					if (UseRange) {
						if (Range.width != 0 || Range.height != 0) {
							tpoint.x = Mathf.Clamp(tpoint.x, Range.xMin, Range.xMax);
							tpoint.y = Mathf.Clamp(tpoint.y, Range.yMin, Range.yMax);
						}
					}
					transform.position = tpoint;
				} else if (touch.phase == TouchPhase.Ended) {
					if (Rewind) {
						mTouchState = TouchState.Releasing;
					}
				}
			}
		} else if (mTouchState == TouchState.Releasing) {
			if (mFrameCount >= RewindWait) {
				transform.position = mOrigin;
				mTouchState = TouchState.None;
			} else {
				mFrameCount++;
			}
		}*/
		if (mTouchState == TouchState.None) {
			if (Input.GetMouseButton(0)) {
				if (Contains(Input.mousePosition)) {
					KyTweener tweener = GetComponent<KyTweener>();
					if (tweener) {
						Destroy(tweener);
					}
					mTouchState = TouchState.Touching;
					mOrigin = transform.position;
					Vector3 wpoint = mCamera.ScreenToWorldPoint(Input.mousePosition);
					wpoint.z = transform.position.z;
					mOldPosition = wpoint;
				}
			}
		}
		else
		if (mTouchState == TouchState.Touching) {
			if (Input.GetMouseButton(0)) {
				Vector3 wpoint = mCamera.ScreenToWorldPoint(Input.mousePosition);
				Vector3 tpoint = transform.position;
				wpoint.z = transform.position.z;
				tpoint += (wpoint - mOldPosition) * ScaleFactor;
				mOldPosition = wpoint;
				if (UseRange) {
					if (Range.width != 0 || Range.height != 0) {
						tpoint.x = Mathf.Clamp(tpoint.x, Range.xMin, Range.xMax);
						tpoint.y = Mathf.Clamp(tpoint.y, Range.yMin, Range.yMax);
					}
				}
				transform.position = tpoint;
			}
			else {
				mFrameCount = 0;
				mTouchState = TouchState.Releasing;
			}
		} else
		if (mTouchState == TouchState.Releasing) {
			if (Rewind) {
				if (mFrameCount >= RewindWait) {
					transform.position = mOrigin;
					mTouchState = TouchState.None;
				}
				else {
					mFrameCount++;
				}
			}
			else {
				mTouchState = TouchState.None;
			}
		}

	}

	bool Contains(Vector3 pos) {
		Vector3 w = mCamera.ScreenToWorldPoint(pos);
		if (!ScreenRange.Contains(w)) { return false; }
		if (OnlyGrab) {
			Rect rect = new Rect();
			rect.xMin = GrabRange.xMin + transform.position.x;
			rect.yMin = GrabRange.yMin + transform.position.y;
			rect.xMax = GrabRange.xMax + transform.position.x;
			rect.yMax = GrabRange.yMax + transform.position.y;
			return rect.Contains(w);
		} else {
			return true;
		}
	}

	enum TouchState {
		None,
		Touching,
		Releasing,
	}

	public Rect Range = new Rect(0, 0, 0, 0);
	public Rect GrabRange = new Rect(0, 0, 0, 0);
	public Rect ScreenRange = new Rect(-240, -320, 480, 640);
	public bool UseRange = false;
	public bool Rewind = false;
	public bool OnlyGrab = false;
	public int RewindWait = 1;
	public float ScaleFactor = 1.0f;

	private Camera mCamera;
	private TouchState mTouchState;
	private Vector3 mOldPosition;
	private Vector3 mOrigin;
	private int mFrameCount;
}
