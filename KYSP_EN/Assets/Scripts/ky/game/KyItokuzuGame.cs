using UnityEngine;
using System.Collections;

public class KyItokuzuGame : KyScriptObject {

	protected override void Start() {
		base.Start();
		mKeyboardObject = KyUtil.FindSibling(gameObject, "keyboard");
		mLefthandObject = KyUtil.FindSibling(gameObject, "handL");
		mRighthandObject = KyUtil.FindSibling(gameObject, "handR");
		mItokuzuObject = KyUtil.FindSibling(gameObject, "itokuzu");
		ChangeState(StateWaitInput);
	}

	protected override void UpdateCore() {
		if (!mEnabled) { return; }
		if (State != null) {
			State();
		}
	}

	private int StateWaitInput() {
		if (!CommandManager.PreviewMode) { mInput.Update(); }
		if ((mInput.InputTrigger & KyInputDrag.Trigger.TouchDown) != 0) {
			if (KyUtil.ContainsIn(mRighthandObject, mRighthandRect, mInput.StartPosition)) {
				KyAudioManager.Instance.PlayOneShot("se_move");
				ChangeState(StateDragRighthand);
			} else if (KyUtil.ContainsIn(mLefthandObject, mLefthandRect, mInput.StartPosition)) {
				KyAudioManager.Instance.PlayOneShot("se_move");
				ChangeState(StateDragLefthand);
			} else if (KyUtil.ContainsIn(mKeyboardObject, mKeyboardRect, mInput.StartPosition)) {
				KyAudioManager.Instance.PlayOneShot("se_move");
				ChangeState(StateDragKeyboard);
			}
		}
		return 0;
	}

	private int StateDragKeyboard() {
		if (!CommandManager.PreviewMode) { mInput.Update(); }
		if (mInput.InputState == KyInputDrag.State.TouchDown) {
			mKeyboardObject.transform.localPosition += mInput.DeltaPosition;
			mKeyboardObject.transform.localPosition = KyUtil.ClampByRect(
				mKeyboardObject.transform.localPosition, mMoveRange);
			if (mKeyboardObject.transform.localPosition.y > 120) {
				ChangeState(StateFailure);
			}
		} else {
			ChangeState(StateWaitInput);
		}
		CheckItokuzu();
		return 0;
	}

	private int StateDragLefthand() {
		if (!CommandManager.PreviewMode) { mInput.Update(); }
		if (mInput.InputState == KyInputDrag.State.TouchDown) {
			mLefthandObject.transform.localPosition += mInput.DeltaPosition;
			mLefthandObject.transform.localPosition = KyUtil.ClampByRect(
				mLefthandObject.transform.localPosition, mMoveRange);
			MoveItokuzuByHand(mLefthandObject, mInput.DeltaPosition);
			if (mLefthandObject.transform.localPosition.y > 120) {
				ChangeState(StateFailure);
			}
		} else { 
			ChangeState(StateWaitInput);
		}
		CheckItokuzu();
		return 0;
	}

	private int StateDragRighthand() {
		if (!CommandManager.PreviewMode) { mInput.Update(); }
		if (mInput.InputState == KyInputDrag.State.TouchDown) {
			mRighthandObject.transform.localPosition += mInput.DeltaPosition;
			mRighthandObject.transform.localPosition = KyUtil.ClampByRect(
				mRighthandObject.transform.localPosition, mMoveRange);
			MoveItokuzuByHand(mRighthandObject, mInput.DeltaPosition);
			if (mRighthandObject.transform.localPosition.y > 120) {
				ChangeState(StateFailure);
			}
		} else {
			ChangeState(StateWaitInput);
		}
		CheckItokuzu();
		return 0;
	}

	private int StateFailure() {
		CommandManager.MoveFrame(4000, 0);
		ChangeState(null);
		return 0;
	}

	private void CheckItokuzu() {
		if (KyUtil.ContainsIn(mKeyboardObject, mGoalRect, mItokuzuObject.transform.localPosition)) {
			CommandManager.MoveFrame(1000, 0);
			ChangeState(null);
		}
	}

	private void MoveItokuzuByHand(GameObject hand, Vector3 delta) {
		Vector2 dif = mItokuzuObject.transform.localPosition - hand.transform.localPosition;
		if (Vector2.Dot(dif, new Vector2(delta.x, delta.y)) <= 0) { return; }
		if (dif.magnitude < mHandLength) {
			mItokuzuObject.transform.localPosition += delta;
		}
	}

	public void OnEnabled() {
		mEnabled = true;
	}

	public void OnDisabled() {
		mEnabled = false;
	}

	private GameObject mKeyboardObject = null;
	private GameObject mLefthandObject = null;
	private GameObject mRighthandObject = null;
	private GameObject mItokuzuObject = null;
	private Rect mKeyboardRect = new Rect(-239, -94, 478, 188);
	private Rect mLefthandRect = new Rect(-85, -85, 171, 171);
	private Rect mRighthandRect = new Rect(-100, -79, 200, 159);
	private Rect mGoalRect = new Rect(-199, -44, 398, 88);
	private Rect mMoveRange = new Rect(-240, -240, 480, 480);
	private float mHandLength = 100;
	private bool mEnabled = false;

	private KyInputDrag mInput = new KyInputDrag();

	private System.Func<int> State = null;
	//private int mSequence = 0;
	private void ChangeState(System.Func<int> state) {
		State = state;
		//mSequence = 0;
	}

}
