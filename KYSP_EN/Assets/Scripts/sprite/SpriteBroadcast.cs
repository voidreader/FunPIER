using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteBroadcast : Sprite {

	void Start () {
		List<Sprite> sprites = new List<Sprite>();
		foreach (Transform child in transform) {
			Sprite sprite = child.GetComponent<Sprite>();
			if (sprite != null) {
				sprites.Add(sprite);
			}
		}
		Sprites = sprites.ToArray();
	}
	
	void Update () {
		if (AnimationIndex != mAnimationIndexOld) {
			mAnimationIndexOld = AnimationIndex;
			foreach (Sprite sprite in Sprites) {
				sprite.AnimationIndex = AnimationIndex;
				sprite.UpdateAll();
			}
		}
	}

	public override void UpdateAll() {
		foreach (Sprite sprite in Sprites) {
			sprite.UpdateAll();
		}
	}

	public Sprite[] Sprites;
	protected int mAnimationIndexOld = -1;
}
