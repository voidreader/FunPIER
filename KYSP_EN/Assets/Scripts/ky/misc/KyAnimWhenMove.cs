using UnityEngine;
using System.Collections;

public class KyAnimWhenMove : MonoBehaviour {

	void Start () {
		mOldPosition = transform.position;
		mMovedDistance = 0;

		mSprite = GetComponent<Sprite>();
		if (mSprite == null) {
			mSpriteAnimation = GetComponent<KySpriteAnimation>();
			if (mSpriteAnimation == null) {
				Debug.LogWarning("Component KySpriteAnimation not found.");
				Destroy(this);
			}
		}
	}
	
	void Update () {
		Vector3 dif = transform.position - mOldPosition;
		mMovedDistance += Mathf.Clamp(dif.magnitude, 0.0f, IntervalDistance);
		if (mMovedDistance >= IntervalDistance) {
			if (mSprite != null) {
				mSprite.FrameIndex++;
			} else {
				mSpriteAnimation.Frame++;
			}
			mMovedDistance -= IntervalDistance;
		}
		mOldPosition = transform.position;
	}

	public float IntervalDistance = 16.0f;

	private Vector3 mOldPosition;
	private float mMovedDistance;
	private KySpriteAnimation mSpriteAnimation;
	private Sprite mSprite;
}
