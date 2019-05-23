using UnityEngine;
using System.Collections;

public class SpriteComponent : Sprite {

	void Awake() {
		DeactiveAllSprite();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (AnimationIndex != mAnimationIndexOld) {
			mAnimationIndexOld = AnimationIndex;
			UpdateAnimation();
		}
	}

	public void UpdateAnimation() {
		if (mCurrentSprite != null) {
			mCurrentSprite.gameObject.active = false;
		}
		mCurrentSprite = Sprites[AnimationIndex];
		mCurrentSprite.gameObject.active = true;
	}

	protected void DeactiveAllSprite() {
		foreach (Sprite sprite in Sprites) {
			sprite.gameObject.active = false;
		}
	}

	public Sprite[] Sprites;

	protected int mAnimationIndexOld = -1;
	protected Sprite mCurrentSprite = null;
}
