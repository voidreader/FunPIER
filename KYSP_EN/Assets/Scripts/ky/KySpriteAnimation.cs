using UnityEngine;
using System.Collections;

public class KySpriteAnimation : MonoBehaviour {

	public enum AnimLoop {
		None = 0,
		Loop,
	}

	[System.Serializable]
	public class AnimationInfo {
		public int BeginFrame = 0;
		public int EndFrame = 0;
		public float Interval = 0.05f;
		public AnimLoop Loop = AnimLoop.Loop;
	}

	[System.Serializable]
	public class FrameInfo {
		public int TextureIndex = 0;
		public Rect ClipRect;
		public Vector3 Position;
	}

	public void Awake() {
		if (Animations.Length == 0) {
			int length = 0;
			if (Frames.Length != 0) {
				length = Frames.Length;
			} else {
				length = Textures.Length;
			}
			Animations = new AnimationInfo[length];
			for (int i = 0; i < length; ++i) {
				Animations[i] = new AnimationInfo();
				Animations[i].BeginFrame = i;
				Animations[i].EndFrame = i;
				Animations[i].Interval = 0;
			}
		}

		mOldFrame = -1;
		mIndex = AnimationIndex;
		mAnimInfo = Animations[mIndex];
		mFrame = mAnimInfo.BeginFrame;
		mAnimRunning = true;
	}

	public void Start() {
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		if (meshFilter == null) {
			meshFilter = gameObject.AddComponent<MeshFilter>();
			Mesh mesh = (Mesh)Resources.Load("GameMeshes/SimplePlane");
			Assert.AssertNotNull(mesh);
			meshFilter.sharedMesh = mesh;
		}

		if (GetComponent<Renderer>() == null) {
			gameObject.AddComponent<MeshRenderer>();
			GetComponent<Renderer>().material = new Material(Shader.Find("Particles/Alpha Blended"));
		}
		if (GetComponent<Renderer>().material == null) {
			GetComponent<Renderer>().material = new Material(Shader.Find("Particles/Alpha Blended"));
		}
	}
	
	public void Update () {
		if (Textures == null || Textures.Length == 0) { return; }

		if (mIndex != AnimationIndex) {
			mIndex = AnimationIndex;
			mAnimInfo = Animations[mIndex];
			mFrame = mAnimInfo.BeginFrame;
			mAnimRunning = true;
		}

		UpdateFrame();

		float interval = mAnimInfo.Interval;
		if (interval <= 0) {
			mAnimRunning = false;
			return; 
		}
		if (!mAnimRunning) { return; }
		//mElapsedTime += Time.deltaTime;
		mElapsedTime += KyScriptTime.DeltaTime;
		while (mElapsedTime >= interval) {
			Frame += (int)(mElapsedTime / interval);
			mElapsedTime %= interval;
		}
	}

	protected void UpdateFrame() {
		if (mOldFrame == mFrame) { return; }
		Texture texture;
		if (HasFrameInfo) {
			int textureIndex = Frames[mFrame].TextureIndex;
			texture = Textures[textureIndex];
		} else {
			texture = Textures[mFrame];
		}
		GetComponent<Renderer>().material.mainTexture = texture;
		if (texture != null) {
			GetComponent<Renderer>().enabled = true;
			if (HasFrameInfo) {
				FrameInfo info = Frames[mFrame];
				GetComponent<Renderer>().material.mainTextureOffset = new Vector2(
					info.ClipRect.xMin / texture.width,
					(texture.height - info.ClipRect.yMin - info.ClipRect.height) / texture.height);
				GetComponent<Renderer>().material.mainTextureScale = new Vector2(
					info.ClipRect.width / texture.width,
					info.ClipRect.height / texture.height);
				transform.localScale = new Vector3(
					info.ClipRect.width,
					info.ClipRect.height,
					1);
				if (UsePosition) {
					transform.localPosition = info.Position;
				}
			} else {
				transform.localScale = new Vector3(
					texture.width,
					texture.height,
					1);
			}
		} else {
			GetComponent<Renderer>().enabled = false;
		}

		mOldFrame = mFrame;
	}

	public bool HitTest(Vector3 pos) {
		Rect rect = new Rect();
		rect.xMin = transform.position.x - transform.lossyScale.x / 2;
		rect.xMax = transform.position.x + transform.lossyScale.x / 2;
		rect.yMin = transform.position.y - transform.lossyScale.y / 2;
		rect.yMax = transform.position.y + transform.lossyScale.y / 2;
		return rect.Contains(pos);
	}

	public void Restart() {
		mFrame = mAnimInfo.BeginFrame;
		mAnimRunning = true;
	}

	public int Frame {
		get { return mFrame; }
		set { 
			mFrame = value;
			mAnimRunning = true;
			if (mFrame > mAnimInfo.EndFrame) {
				mFrame = mAnimInfo.BeginFrame +
					((mFrame - mAnimInfo.BeginFrame) % (mAnimInfo.EndFrame - mAnimInfo.BeginFrame + 1));
				if (mAnimInfo.Loop == AnimLoop.Loop) {
					mFrame %= FrameCount;
				} else {
					mFrame = mAnimInfo.EndFrame;
					mAnimRunning = false;
				}
			}
		}
	}

	public int FrameCount {
		get {
			if (HasFrameInfo) {
				return Frames.Length;
			} else {
				return Textures.Length;
			}
		}
	}

	public bool HasFrameInfo {
		get { return (Frames != null && Frames.Length > 0); }
	}

	public bool AnimRunning {
		get { return mAnimRunning; }
	}

	public Texture[] Textures;
	public AnimationInfo[] Animations;
	public FrameInfo[] Frames;
	//public float Interval = 0.05f;
	public int AnimationIndex = 0;
	public bool UsePosition = false;

	private float mElapsedTime;
	private int mFrame;
	private int mOldFrame;
	private int mIndex;
	private bool mAnimRunning;
	private AnimationInfo mAnimInfo;
}
