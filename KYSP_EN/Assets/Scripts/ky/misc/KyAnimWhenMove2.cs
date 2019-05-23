using UnityEngine;
using System.Collections;

public class KyAnimWhenMove2 : MonoBehaviour {

	// Use this for initialization
	void Start() {
		mOldPosition = transform.position;
		mMovedDistance = 0;

		mSpriteAnimation = GetComponent<KySpriteAnimation>();
		if (mSpriteAnimation == null) {
			Debug.LogWarning("Component KySpriteAnimation not found.");
			Destroy(this);
		}
	}

	// Update is called once per frame
	void Update() {
		Vector3 dif = transform.position - mOldPosition;
		mMovedDistance += Mathf.Clamp(dif.magnitude, 0.0f, IntervalDistance);

		if (mSpriteAnimation.AnimationIndex != 0 && dif.x > 0) {
			mSpriteAnimation.AnimationIndex = 0;
		} else
		if (mSpriteAnimation.AnimationIndex != 1 && dif.x < 0) {
			mSpriteAnimation.AnimationIndex = 1;
		}

		if (mMovedDistance >= IntervalDistance) {
			mSpriteAnimation.Frame++;
			mMovedDistance -= IntervalDistance;
		}
		mOldPosition = transform.position;
	}

	public float IntervalDistance = 16.0f;

	private Vector3 mOldPosition;
	private float mMovedDistance;
	private KySpriteAnimation mSpriteAnimation;
}
