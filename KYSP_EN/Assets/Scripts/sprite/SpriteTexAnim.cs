using UnityEngine;
using System.Collections;

public class SpriteTexAnim : Sprite {

	public enum AnimLoop {
		None = 0,
		Loop,
	}

	[System.Serializable]
	public class AnimInfo {
		public int BeginFrame = 0;
		public int EndFrame = 0;
		public float Interval = 0.05f;
		public AnimLoop Loop = AnimLoop.Loop;
	}

	[System.Serializable]
	public class FrameInfo {
		public int TextureIndex = 0;
		public Rect ClipRect;
		public Vector2 Offset;
	}

	void Awake() {
		if (GetComponent<Renderer>() == null) {
			gameObject.AddComponent<MeshRenderer>();
		}
		if (GetComponent<Renderer>().sharedMaterial == null) {
			GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended"));
		}
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		mSpriteMesh = new SpriteMesh();
		meshFilter.sharedMesh = mSpriteMesh.Mesh;

		UpdateAnimation();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (mAnimationRunning) {
			if (mAnimInfo.Interval > 0) {
				mElapsedTime += Time.deltaTime;
				if (mElapsedTime >= mAnimInfo.Interval) {
					mElapsedTime = 0;
					FrameIndex++;
					/*if (FrameIndex > mAnimInfo.EndFrame) {
						if (mAnimInfo.Loop == AnimLoop.None) {
							FrameIndex = mAnimInfo.EndFrame;
							mAnimationRunning = false;
						} else if (mAnimInfo.Loop == AnimLoop.Loop) {
							FrameIndex = mAnimInfo.BeginFrame;
						}
					}*/
				}
			}
		}
		if (AnimationIndex != mAnimationIndexOld) {
			mAnimationIndexOld = AnimationIndex;
			UpdateAnimation();
		}
		if (FrameIndex != mFrameIndexOld) {
			if (FrameIndex > mAnimInfo.EndFrame) {
				if (mAnimInfo.Loop == AnimLoop.None) {
					FrameIndex = mAnimInfo.EndFrame;
					mAnimationRunning = false;
				} else if (mAnimInfo.Loop == AnimLoop.Loop) {
					FrameIndex = mAnimInfo.BeginFrame;
				}
			}
			mFrameIndexOld = FrameIndex;
			UpdateFrame();
		}
	}

	public override void StopAnimation() {
		mAnimationRunning = false;
	}

	public override void UpdateAll() {
		UpdateAnimation();
	}

	public void UpdateAnimation() {
		mElapsedTime = 0;
		mAnimationRunning = true;
		mAnimInfo = Animations[AnimationIndex];
		FrameIndex = mAnimInfo.BeginFrame;
	}

	public void UpdateFrame() {
		if (Frames.Length > 0) {
			Assert.AssertTrue(FrameIndex < Frames.Length);
			mFrameInfo = Frames[FrameIndex];
			mTexture = Textures[mFrameInfo.TextureIndex];
			if (mTexture != null) {
				GetComponent<Renderer>().enabled = true;
			} else {
				GetComponent<Renderer>().enabled = false;
				return;
			}
			Vector2 size = new Vector2();
			if (mFrameInfo.ClipRect.width == 0 && mFrameInfo.ClipRect.height == 0) {
				size.x = mTexture.width;
				size.y = mTexture.height;
				mSpriteMesh.DefaultUVs(SpriteFlip.None);
			} else {
				size.x = mFrameInfo.ClipRect.width;
				size.y = mFrameInfo.ClipRect.height;
				mSpriteMesh.UpdateUVs(SpriteFlip.None, mFrameInfo.ClipRect, mTexture);
			}
			mSpriteMesh.UpdateVertices(mFrameInfo.Offset, size);
		} else {
			Assert.AssertTrue(FrameIndex < Textures.Length);
			mFrameInfo = null;
			mTexture = Textures[FrameIndex];
			if (mTexture != null) {
				GetComponent<Renderer>().enabled = true;
			} else {
				GetComponent<Renderer>().enabled = false;
				return;
			}
			mSpriteMesh.UpdateVertices(SpriteAnchor.Middle, SpriteAnchor.Middle,
				new Vector2(mTexture.width, mTexture.height));
			mSpriteMesh.DefaultUVs(SpriteFlip.None);
		}
		mSpriteMesh.UpdateMesh();
		GetComponent<Renderer>().sharedMaterial.mainTexture = mTexture;
	}

	public Texture[] Textures;
	public AnimInfo[] Animations;
	public FrameInfo[] Frames;

	protected int mAnimationIndexOld = -1;
	protected int mFrameIndexOld = -1;
	protected FrameInfo mFrameInfo = null;
	protected AnimInfo mAnimInfo = null;
	protected Texture mTexture = null;
	protected bool mAnimationRunning = true;
	protected float mElapsedTime;

	protected SpriteMesh mSpriteMesh;
}
