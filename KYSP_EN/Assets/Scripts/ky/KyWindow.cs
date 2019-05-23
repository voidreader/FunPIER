using UnityEngine;
using System.Collections;

public class KyWindow : MonoBehaviour {

	void Awake() {
	}

	// Use this for initialization
	void Start() {
		Assert.AssertNotNull(Texture);

		mFrames = new GameObject[9];
		int index = 0;
		for (int j = 0; j < 3; ++j) {
			for (int i = 0; i < 3; ++i) {
				GameObject obj = new GameObject("frame"+index);
				KySprite sprite = obj.AddComponent<KySprite>();
				sprite.Texture = Texture;
				sprite.ClipRect = new Rect(
					GridSize * i, GridSize * j,
					GridSize, GridSize);
				obj.transform.parent = transform;
				mFrames[index] = obj;
				index++;
			}
		}
		mWindowSizeModified = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (mWindowSizeModified) {
			UpdateWindowSize();
			mWindowSizeModified = false;
		}
	}

	protected void UpdateWindowSize() {
		float offx = (WindowSize.x - GridSize) / 2;
		float offy = (WindowSize.y - GridSize) / 2;
		int index = 0;
		for (int j = 0; j < 3; ++j) {
			for (int i = 0; i < 3; ++i) {
				Vector3 pos = mFrames[index].transform.localPosition;
				pos.x = (i - 1) * offx;
				pos.y = - (j - 1) * offy;
				pos.z = 0;
				mFrames[index].transform.localPosition = pos;
				index++;
			}
		}
		Vector3 scale;
		scale = mFrames[1].transform.localScale;
		scale.x = WindowSize.x - 2 * GridSize;
		mFrames[1].transform.localScale = scale;
		mFrames[7].transform.localScale = scale;

		scale = mFrames[3].transform.localScale;
		scale.y = WindowSize.y - 2 * GridSize;
		mFrames[3].transform.localScale = scale;
		mFrames[5].transform.localScale = scale;

		scale = mFrames[4].transform.localScale;
		scale.x = WindowSize.x - 2 * GridSize;
		scale.y = WindowSize.y - 2 * GridSize;
		mFrames[4].transform.localScale = scale;
	}

	public Texture Texture;
	public float GridSize;
	public Vector2 WindowSize;

	private bool mWindowSizeModified;
	private GameObject[] mFrames;
}
