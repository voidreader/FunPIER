using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KyButton : MonoBehaviour {

	public enum State {
		ButtonDownFlag = 0x01,
		ButtonSelectedFlag = 0x02,

		ButtonUp = 0x00,
		ButtonDown = ButtonDownFlag,
		ButtonSelectedUp = ButtonSelectedFlag,
		ButtonSelectedDown = ButtonDownFlag | ButtonSelectedFlag,
	}

	public enum EventType {
		None = 0,
		ButtonUp = 1 << 0,
		ButtonDown = 1 << 1,
		ButtonNotSelected = 1 << 2,
		ButtonSelected = 1 << 3
	}

	#region MonoBehaviour Methods

	public void Start () {
		if (SpriteObject != null) {
			mSprite = SpriteObject.GetComponent<KySpriteAnimation>();
		} else {
			mSprite = GetComponent<KySpriteAnimation>();
		}
		if (mSprite != null) {
			mSprite.AnimationIndex = AnimIndexUp;
		}
		//mState = State.ButtonUp;
		//mEventFlag = EventType.None;
		if (gameObject.transform.parent != null) {
			GameObject parent = gameObject.transform.parent.gameObject;
			mGroup = parent.GetComponent<KyButtonGroup>();
		}
		ChangeButtonState(mState);
	}
	
	public void Update () {
		mEventFlag = EventType.None;
		//if (mWaitFrame > 0) {
		//	mWaitFrame--;
		//	return;
		//}
		switch (mState & State.ButtonDown) {
		case State.ButtonUp:
		case State.ButtonSelectedUp:
			{
				bool check = false;
				if (SoftTouch) {
					if (Input.GetMouseButton(0)) {
						check = true;
					}
				} else {
					if (Input.GetMouseButtonDown(0)) {
						check = true;
					}
				}
				if (check) {
					if (HitTest(Camera.main.ScreenToWorldPoint(Input.mousePosition))) {
						ChangeButtonState(State.ButtonDown);
					}
				}
			} break;
		case State.ButtonDown:
		case State.ButtonSelectedDown:
			{
				if (SoftTouch) {
					if (!Input.GetMouseButton(0) || 
						!HitTest(Camera.main.ScreenToWorldPoint(Input.mousePosition))) {
						ChangeButtonState(State.ButtonUp);
					}
				} else {
					if (!Input.GetMouseButton(0)) {
						ChangeButtonState(State.ButtonUp);
					}
				}
			} break;
		}
		if (mOldState != mState) {
			//DebugUtil.Log(name + " changed from "+mOldState + " to " + mState);
			UpdateSprite();
			State flags = 0;
			flags = ((~mOldState) & mState);
			if ((flags & State.ButtonDownFlag) != 0) {
				mEventFlag |= EventType.ButtonDown;
			}
			if ((flags & State.ButtonSelectedFlag) != 0) {
				mEventFlag |= EventType.ButtonSelected;
			}
			flags = (mOldState & (~mState));
			if ((flags & State.ButtonDownFlag) != 0) {
				mEventFlag |= EventType.ButtonUp;
			}
			if ((flags & State.ButtonSelectedFlag) != 0) {
				mEventFlag |= EventType.ButtonNotSelected;
			}
			//if ((mEventFlag & EventType.ButtonSelected) != 0) {
			if ((mState & State.ButtonSelectedFlag) != 0) {
				if (mGroup) {
					mGroup.OnChildButtonSelected(this);
				}
			}
			mOldState = mState;
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.green;
		if (HitRect.width != 0 && HitRect.height != 0) {
			Gizmos.DrawWireCube(
				transform.position,
				new Vector3(HitRect.width, HitRect.height, 1));
		}
	}

	#endregion

	#region Methods

	public void UpdateSprite() {
		if (mSprite != null) {
			mSprite.AnimationIndex =
				mState == State.ButtonUp ? AnimIndexUp :
				mState == State.ButtonDown ? AnimIndexDown :
				mState == State.ButtonSelectedUp ? AnimIndexSelectedUp :
				mState == State.ButtonSelectedDown ? AnimIndexSelectedDown :
				mSprite.AnimationIndex;
		}
	}

	public void ChangeButtonState(State state) {
		state &= State.ButtonDownFlag;
		if (state == State.ButtonDown) {
			if (Toggle) {
				mState ^= State.ButtonSelectedFlag;
			} else {
				mState |= State.ButtonSelectedFlag;
			}
			mState |= State.ButtonDownFlag;
		} else if (state == State.ButtonUp) {
			mState &= ~State.ButtonDownFlag;
		}
	}

	public void ForceButtonState(State state) {
		mState = state;
		mOldState = state;
		UpdateSprite();
	}

	public bool HitTest(Vector3 pos) {
		if (HitRect.width != 0 && HitRect.height != 0) {
			Rect rect = HitRect;
			rect.x += transform.position.x - rect.width / 2;
			rect.y += transform.position.y - rect.height / 2;
			return rect.Contains(pos);
		} else if (mSprite != null) {
			return mSprite.HitTest(pos);
		} else {
			return false;
		}
	}

	public bool GetEventFlag(EventType eventType) {
		return (mEventFlag & eventType) != 0;
	}

	public static void EnableButton(GameObject obj, bool enable, bool children) {
		if (children) {
			KyButton[] buttons = obj.GetComponentsInChildren<KyButton>();
			foreach (KyButton button in buttons) {
				button.enabled = enable;
			}
		} else {
			KyButton button = obj.GetComponent<KyButton>();
			button.enabled = enable;
		}
	}

	public KyButton[] GetButtonsInChildren() {
		List<KyButton> children = new List<KyButton>();
		foreach (Transform child in transform) {
			KyButton button = child.GetComponent<KyButton>();
			if (button != null) {
				children.Add(button);
			}
		}
		return children.ToArray();
	}

	#endregion

	#region Properties

	public EventType EventFlag {
		get { return mEventFlag; }
	}

	public bool Selected {
		get { return ((int)mState & (int)State.ButtonSelectedFlag) != 0; }
		set {
			if (value) { 
				mState |= State.ButtonSelectedFlag; 
			} else {
				mState &= ~State.ButtonSelectedFlag; 
			}
			UpdateSprite();
		}
	}

	#endregion

	#region Fields

	public int AnimIndexUp = 0;
	public int AnimIndexDown = 0;
	public int AnimIndexSelectedUp = 0;
	public int AnimIndexSelectedDown = 0;
	public bool Toggle = false;
	public bool SoftTouch = false;
	public int Index = 0;
	public Rect HitRect;
	public GameObject SpriteObject;

	protected State mState = State.ButtonUp;
	protected State mOldState = State.ButtonUp;
	protected EventType mEventFlag = EventType.None;
	protected KySpriteAnimation mSprite = null;
	protected KyButtonGroup mGroup = null;

	//private int mWaitFrame = 0;

	#endregion

}
