using UnityEngine;
using System.Collections;

public class SpriteTiled : Sprite {

	protected void Awake() {
		MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
		renderer.material = new Material(Shader.Find("Particles/Alpha Blended"));
		renderer.material.mainTexture = Texture;

		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		mSpriteMesh = new SpriteMesh();
		meshFilter.sharedMesh = mSpriteMesh.Mesh;

		if (PixelFitting) {
			Size.x = TileRect.width;
			Size.y = TileRect.height;
		}
		mSpriteMesh.UpdateVertices(AnchorX, AnchorY, Size);

		UpdateAll();
	}
	
	protected void Update () {
		if (UpdateTime > 0) {
			mElapsedTime += Time.deltaTime;
			if (mElapsedTime > UpdateTime) {
				mElapsedTime -= UpdateTime;
				FrameIndex = (FrameIndex + 1) % TileCount;
			}
		}
		UpdateAll();
	}

	public override int AnimationCount {
		get { return 1; }
	}

	public override int FrameCount {
		get { return TileCount; }
	}

	public override void UpdateAll() {
		if (FrameIndex != mOldFrameIndex) {
			UpdateFrame();
			mOldFrameIndex = FrameIndex;
		}
	}

	protected void UpdateFrame() {
		int p0, p1;
		p1 = System.Math.DivRem(FrameIndex, FirstDimension, out p0);
		float x = TileRect.xMin + p0 * Stride.x;
		float y = TileRect.yMin + p1 * Stride.y;
		Rect clipRect = new Rect(
			x, Texture.height - (y + TileRect.height),
			TileRect.width, TileRect.height);

		//testClipRect = clipRect;
		mSpriteMesh.UpdateUVs(SpriteFlip.None, clipRect, Texture);
		mSpriteMesh.UpdateMesh();
	}

	public Vector2 Size = new Vector2(1, 1);
	public Texture Texture = null;
	public Rect TileRect = new Rect();
	public Vector2 Stride = new Vector2(1, 1);
	public int FirstDimension = 1;
	public int TileCount = 1;
	public bool PixelFitting = true;
	public float UpdateTime = 1.0f;
	public SpriteAnchor AnchorX = SpriteAnchor.Middle;
	public SpriteAnchor AnchorY = SpriteAnchor.Middle;

	private SpriteMesh mSpriteMesh;
	private float mElapsedTime = 0;
	private int mOldFrameIndex = -1;

	//private Rect testClipRect;
}
