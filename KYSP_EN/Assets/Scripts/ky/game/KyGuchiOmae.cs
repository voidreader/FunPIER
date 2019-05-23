using UnityEngine;
using System.Collections;

public class KyGuchiOmae : KyScriptObject {

	#region Methods

	protected override void Start() {
		base.Start();
		mState.ChangeState(StateWaitInput);
	}

	protected override void UpdateCore() {
		if (mFinished) { return; }
		mState.Execute();
	}

	private int StateWaitInput() {
		if (!CommandManager.PreviewMode) { 
			mInput.Update();
			mInputDrag.Update();
		}
		if (mEnabled) {
			if ((mInput.Slide & KyInputCrossKey.Direction.Down) != 0) {
				TargetSprite.AnimationIndex = 1;
				TargetSprite.UpdateAll();
				mInput.Clear();
				if (mReady) {
					mCount++;
					mReady = false;
				}
				KyAudioManager.Instance.PlayOneShot("se_cancel");
				mState.ChangeState(StateInterval);
			}
		}
		if ((mInput.Slide & KyInputCrossKey.Direction.Left) != 0 ||
			(mInput.Slide & KyInputCrossKey.Direction.Right) != 0) {
			mState.ChangeState(StateMoveHolizon);
		}
		return 0;
	}

	private int StateMoveHolizon() {
		if (!CommandManager.PreviewMode) { mInputDrag.Update(); }
		if (mInputDrag.InputState == KyInputDrag.State.TouchUp) {
			mState.ChangeState(StateWaitInput);
		} else {
			float dx = mInputDrag.DeltaPosition.x;
			if (dx != 0) {
				Vector3 pos = transform.localPosition;
				pos.x += dx;
				transform.localPosition = pos;
			}
		}
		return 0;
	}

	private int StateInterval() {
		mElapsedTime += DeltaTime;
		if (mElapsedTime > 1.0f) {
			mElapsedTime = 0;
			mState.ChangeState(StateWaitInput);
		}
		return 0;
	}

	public void OnGetReady() {
		mReady = true;
		mEnabled = true;
	}

	public void OnDisabled() {
		mEnabled = false;
	}

	public void OnFinished() {
		mFinished = true;
	}

	public void OnJudge() {
		if (mCount >= 4) {
			CommandManager.SetVariable("result", 1);
		} else {
			CommandManager.SetVariable("result", 0);
		}
	}

	#endregion

	#region Fields

	public Sprite TargetSprite = null;
	private KyInputCrossKey mInput = new KyInputCrossKey();
	private KyInputDrag mInputDrag = new KyInputDrag();
	private float mElapsedTime = 0.0f;
	private bool mReady = false;
	private bool mEnabled = false;
	private bool mFinished = false;
	private int mCount = 0;
	private KyStateManager mState = new KyStateManager();

	#endregion
}
