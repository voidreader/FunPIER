using UnityEngine;
using System.Collections;

public class KyStateManager {

	public KyStateManager() {
	}

	public KyStateManager(System.Func<int> initialState) {
		ChangeState(initialState);
	}

	public void Execute() {
		if (mState != null) {
			mState();
		}
	}

	public void ChangeState(System.Func<int> state) {
		mState = state;
		mSequence = 0;
	}

	public System.Func<int> State {
		get { return mState; }
	}

	public int Sequence {
		get { return mSequence; }
		set { mSequence = value; }
	}

	private System.Func<int> mState = null;
	private int mSequence = 0;
}
