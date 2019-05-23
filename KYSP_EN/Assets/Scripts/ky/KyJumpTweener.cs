using UnityEngine;
using System.Collections;

public class KyJumpTweener : MonoBehaviour {

	enum JumpState {
		Landing = 0,
		Jumping,
	}
	
	void Update () {
		if (KyScriptTime.DeltaTime == 0) { return; }
		if (mJumping) {
			mElapsedTime += KyScriptTime.DeltaTime;
			float t = mElapsedTime / JumpTime;
			t = Mathf.Clamp(t, 0, 2);
			float y = -(t - 1) * (t - 1) + 1;
			Vector3 displacement = new Vector3();
			displacement.x = Mathf.Lerp(0, JumpWidth, t / 2);
			displacement.y = y * JumpHeight;
			transform.localPosition = mOriginPos + displacement;
			if (t >= 2.0f) {
				mJumping = false;
			}
		}
	}

	public void Jump() {
		if (!mJumping) {
			mJumping = true;
			mElapsedTime = 0.0f;
			mOriginPos = transform.localPosition;
		}
	}

	public bool Jumping {
		get { return mJumping; }
	}

	public float JumpTime = 1.0f;
	public float JumpHeight = 40.0f;
	public float JumpWidth = 0.0f;
	private Vector3 mOriginPos = Vector3.zero;
	private bool mJumping = false;
	private float mElapsedTime = 0.0f;
}
