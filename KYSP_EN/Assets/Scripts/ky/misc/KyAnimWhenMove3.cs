using UnityEngine;
using System.Collections;

public class KyAnimWhenMove3 : MonoBehaviour {

	// Use this for initialization
	void Start() {
		mOldPosition = transform.position;
		mMovedDistance = 0;
		mSprite = GetComponent<Sprite>();
		if (mSprite == null) {
			Debug.LogWarning("Component KySpriteAnimation not found.");
			Destroy(this);
		}

	}

	// Update is called once per frame
	void Update() {
		Vector3 dif = transform.position - mOldPosition;
		mMovedDistance += Mathf.Clamp(dif.magnitude, 0.0f, IntervalDistance);

		if (mSprite.AnimationIndex != 0 && dif.x > 0) {
			Vector3 scale = Vector3.one;
			scale.x = -1;
			transform.localScale = scale;
		} else
		if (mSprite.AnimationIndex != 0 && dif.x < 0) {
			Vector3 scale = Vector3.one;
			transform.localScale = scale;
		}

		if (mMovedDistance >= IntervalDistance) {
			mMovedDistance -= IntervalDistance;
		}
		mOldPosition = transform.position;
	}

	public float IntervalDistance = 16.0f;

	private Vector3 mOldPosition;
	private float mMovedDistance;

	private Sprite mSprite;

}
