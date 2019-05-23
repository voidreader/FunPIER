using UnityEngine;
using System.Collections;

public class KySpriteUVAnim : SpriteSimple {

	public enum AnimTypes {
		None,
		LeftToRight,
		RightToLeft,
		TopToBottom,
		BottomToTop
	}

	void Start() {
		if (AnimType == AnimTypes.LeftToRight) {
			ClipRect.xMax = 0;
		} else if (AnimType == AnimTypes.RightToLeft) {
			ClipRect.xMin = Texture.width;
		} else if (AnimType == AnimTypes.TopToBottom) {
			ClipRect.yMin = Texture.height;
		} else if (AnimType == AnimTypes.BottomToTop) {
			ClipRect.yMax = 0;
		}
		UpdateAll();
	}

	void Update () {
		if (mPlaying) {
			mElapsedTime += KyScriptTime.DeltaTime;
			if (mElapsedTime > Duration) {
				mPlaying = false;
			}
			if (AnimType == AnimTypes.LeftToRight) {
				ClipRect.xMax = Mathf.Clamp01(mElapsedTime / Duration) * Texture.width;
			} else if (AnimType == AnimTypes.RightToLeft) {
				ClipRect.xMin = (1 - Mathf.Clamp01(mElapsedTime / Duration)) * Texture.width;
			} else if (AnimType == AnimTypes.TopToBottom) {
				ClipRect.yMin = (1 - Mathf.Clamp01(mElapsedTime / Duration)) * Texture.height;
			} else if (AnimType == AnimTypes.BottomToTop) {
				ClipRect.yMax = Mathf.Clamp01(mElapsedTime / Duration) * Texture.height;
			}
			
			UpdateAll();
		}
	}

	public float Duration = 2.0f;
	public AnimTypes AnimType = AnimTypes.None;
	protected float mElapsedTime = 0;
	protected bool mPlaying = true;
	
}
