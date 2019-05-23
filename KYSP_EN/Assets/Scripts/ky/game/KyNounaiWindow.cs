using UnityEngine;
using System.Collections;

public class KyNounaiWindow : KyScriptObject {

	protected override void Start() {
		base.Start();
		mInputTitle.Owner = gameObject;
		mInputTitle.HitRect = new Rect(-152, -40, 265, 40);
		mInputCross.Owner = gameObject;
		mInputCross.HitRect = new Rect(112, -40, 40, 40);
		mWindowPos = transform.localPosition;
		ChangeState(StateWindowNormal);
	}

	protected override void UpdateCore() {
		if (!mEnabled) { return; }
		if (State != null) {
			State();
		}
	}

	private int StateWindowNormal() {
		if (mSequence == 0) {
			transform.localPosition = mWindowPos;
			mInputTitle.HitRect = new Rect(-152, -40, 265, 40);
			mInputCross.HitRect = new Rect(112, -40, 40, 40);
			WindowSprite.FrameIndex = 0;
			mSequence++;
		} else if (mSequence == 1) {
			if (!CommandManager.PreviewMode) { mInputCross.Update(); }
			if (mInputCross.InputTrigger == KyInputDrag.Trigger.TouchDown) {
				KyAudioManager.Instance.PlayOneShot("se_click");
				ChangeState(StateWindowDestroy);
			}

			if (!CommandManager.PreviewMode) { mInputTitle.Update(); }
			if ((mInputTitle.InputTrigger & KyInputDrag.Trigger.DoubleTouchDown) != 0) {
				mWindowPos = transform.localPosition;
				ChangeState(StateWindowFull);
			} else if ((mInputTitle.InputTrigger & KyInputDrag.Trigger.TouchDown) != 0) {
				//KyAudioManager.Instance.PlayOneShot("se_cursor");
			} else if (mInputTitle.InputState == KyInputDrag.State.TouchDown) {
				Vector3 pos = transform.localPosition;
				pos += mInputTitle.DeltaPosition;
				pos.y = Mathf.Clamp(pos.y, -142, 182);
				transform.localPosition = pos;
			}
		}
		return 0;
	}

	private int StateWindowFull() {
		if (mSequence == 0) {
			transform.localPosition = new Vector3(0, 182, transform.localPosition.z);
			mInputTitle.HitRect = new Rect(-229, -40, 418, 40);
			mInputCross.HitRect = new Rect(189, -40, 40, 40);
			WindowSprite.FrameIndex = 1;
			mSequence++;
		} else if (mSequence == 1) {
			if (!CommandManager.PreviewMode) { mInputCross.Update(); }
			if (mInputCross.InputTrigger == KyInputDrag.Trigger.TouchDown) {
				KyAudioManager.Instance.PlayOneShot("se_click");
				ChangeState(StateWindowDestroy);
			}

			if (!CommandManager.PreviewMode) { mInputTitle.Update(); }
			if ((mInputTitle.InputTrigger & KyInputDrag.Trigger.DoubleTouchDown) != 0) {
				ChangeState(StateWindowNormal);
			}
		}
		return 0;
	}

	private int StateWindowDestroy() {
		WindowSprite.FrameIndex = 2;
		ChangeState(null);
		CommandManager.MoveFrame(1000, 0);
		return 0;
	}

	public void OnEnabled() {
		mEnabled = true;
	}

	public void OnDisabled() {
		mEnabled = false;
	}

	public void OnJudge() {
		if (State == StateWindowNormal) {
			float x = transform.localPosition.x;
			float y = transform.localPosition.y;
			if (x <= -360 || 360 <= x || y <= -120) {
				CommandManager.SetVariable("result", 1);
			} else {
				CommandManager.SetVariable("result", 0);
			}
		} else if (State == StateWindowFull) {
			CommandManager.SetVariable("secret", 1);
			CommandManager.SetVariable("result", 0);
		} else if (State == StateWindowDestroy) {
			CommandManager.SetVariable("result", 1);
		}
	}

	public Sprite WindowSprite = null;

	private KyInputDrag mInputTitle = new KyInputDrag();
	private KyInputDrag mInputCross = new KyInputDrag();
	private Vector3 mWindowPos = Vector3.zero;
	private bool mEnabled = true;

	private System.Func<int> State = null;
	private int mSequence = 0;
	private void ChangeState(System.Func<int> state) {
		State = state;
		mSequence = 0;
	}
}
