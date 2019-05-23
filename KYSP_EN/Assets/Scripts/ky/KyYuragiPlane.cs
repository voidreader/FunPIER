using UnityEngine;
using System.Collections;

public class KyYuragiPlane : MonoBehaviour {

	void Awake() {
		PlaneMeshGenerator gen = new PlaneMeshGenerator(
			new Vector2(480, 480),
			WidthSegment, HeightSegment);
		mMeshFilter = gameObject.AddComponent<MeshFilter>();
		mMesh = gen.Generate();
		mMeshFilter.sharedMesh = mMesh;

		MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
		renderer.sharedMaterial = Material;

		mVertNum = mMesh.vertices.Length;
		mSrcPos = new Vector3[mVertNum];
		mDstPos = new Vector3[mVertNum];
		mMesh.vertices.CopyTo(mSrcPos, 0);
		mMesh.vertices.CopyTo(mDstPos, 0);
	}

	void Start () {
		
	}
	
	void Update () {
		//	揺らぎ
		int hCount = WidthSegment + 1;
		int vCount = HeightSegment + 1;
		int index = 0;
		if (mYuragiEnabled) {
			mElapsedTime += Time.deltaTime;
			if (mElapsedTime >= UpdateTime) {
				for (int y = 1; y < vCount-1; ++y) {
					for (int x = 1; x < hCount-1; ++x) {
						index = x + y * hCount;
						Vector3 vert = mSrcPos[index];
						vert.x += Random.Range(-YuragiRadius, YuragiRadius);
						vert.y += Random.Range(-YuragiRadius, YuragiRadius);
						mDstPos[index] = vert;
					}
				}
				mMesh.vertices = mDstPos;
				mElapsedTime = 0;
			}
		}
		//	フェード
	}

	public void SetYuragiEnabled(bool enable) {
		mYuragiEnabled = enable;
		if (enable) {
		} else {
			mMesh.vertices = mSrcPos;
		}
	}

	public void BeginFadeIn() {
		mFadeDuration = 1.0f;
		mFadeTime = 0;
	}

	public void SetupPreview() {
		int hCount = WidthSegment + 1;
		int vCount = HeightSegment + 1;
		int cx = hCount / 2;
		int cy = vCount / 2;
		Color[] colors = mMesh.colors;
		int index = 0;
		for (int y = 0; y < vCount; ++y) {
			for (int x = 0; x < hCount; ++x) {
				float dist = Mathf.Max(Mathf.Abs(x - cx), Mathf.Abs(y - cy));
				//float dist = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));
				float alpha = Mathf.Clamp01(2.0f * (cx - dist) / (cx));
				colors[index] = new Color(1, 1, 1, alpha);
				/*if (x == 0 || y == 0 || x == hCount - 1 || y == vCount - 1) {
					colors[index] = new Color(1, 1, 1, 0);
				} else {
					colors[index] = new Color(1, 1, 1, 0.6f);
				}*/
				index++;
			}
		}
		mMesh.colors = colors;
	}

	public void UpdateFade(float rate) {
		int hCount = WidthSegment + 1;
		int vCount = HeightSegment + 1;
		int cx = hCount / 2;
		int cy = vCount / 2;
		Color[] colors = mMesh.colors;
		int index = 0;
		for (int y = 0; y < vCount; ++y) {
			for (int x = 0; x < hCount; ++x) {
				int dist = Mathf.Max(Mathf.Abs(x - cx), Mathf.Abs(y - cy));
				float alpha = Mathf.Clamp01(rate * (cx - dist) / cx);
				colors[index] = new Color(1, 1, 1, alpha);
				index++;
			}
		}
		mMesh.colors = colors;
	}

	public int WidthSegment = 10;
	public int HeightSegment = 10;
	public float YuragiRadius = 10.0f;
	public float UpdateTime = 0.2f;
	public Material Material;

	private float mElapsedTime = 0;
	private Mesh mMesh = null;
	private MeshFilter mMeshFilter;
	private int mVertNum;
	private bool mYuragiEnabled;
	private Vector3[] mSrcPos;
	private Vector3[] mDstPos;
	private float mFadeDuration = 0;
	private float mFadeTime = 0;
}
