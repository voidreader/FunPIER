using UnityEngine;
using System.Collections;

public class Sprite : MonoBehaviour {

	public bool HitTest(Vector3 pos) {
		return false;
	}

	public virtual void StopAnimation() {
	}

	public virtual void UpdateAll() {
	}

	public virtual int AnimationCount {
		get { return 0; }
	}

	public virtual int FrameCount {
		get { return 0; }
	}

	public int AnimationIndex = 0;
	public int FrameIndex = 0;
}
