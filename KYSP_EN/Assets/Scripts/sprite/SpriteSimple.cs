using UnityEngine;
using System.Collections;

public class SpriteSimple : Sprite {

	protected void Awake() {
		if (GetComponent<Renderer>() == null) {
			gameObject.AddComponent<MeshRenderer>();
			
		}
		if (GetComponent<Renderer>().sharedMaterial == null) {
			GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Particles/Alpha Blended"));
			DebugUtil.Log("create material : " + gameObject.name);
		}
		if (Texture != null) {
			UpdateTexture();
		} else {
			Texture = GetComponent<Renderer>().sharedMaterial.mainTexture;
		}
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		mSpriteMesh = new SpriteMesh();
		meshFilter.sharedMesh = mSpriteMesh.Mesh;
		UpdateAll(); 
	}

	protected void OnDestroy() {
	}

	public override int AnimationCount {
		get { return 1; }
	}

	public override int FrameCount {
		get { return 1; }
	}

	public override void UpdateAll() {
		if (ClipEnabled) {
			mSpriteMesh.UpdateUVs(Flip, ClipRect, Texture);
			if (PixelFitting) {
				Size.x = ClipRect.width;
				Size.y = ClipRect.height;
			}
		} else {
			mSpriteMesh.DefaultUVs(Flip);
			if (PixelFitting && Texture != null) {
				Size.x = Texture.width;
				Size.y = Texture.height;
			}
		}
		mSpriteMesh.UpdateVertices(AnchorX, AnchorY, Size);
		mSpriteMesh.UpdateColors(MainColor);
		mSpriteMesh.UpdateMesh();
	}

	public void UpdateTexture() {
		GetComponent<Renderer>().sharedMaterial.mainTexture = Texture;
	}

	public Vector2 Size = new Vector2(1, 1);
	public Texture Texture = null;
	public Rect ClipRect = new Rect();
	public Color MainColor = new Color(1, 1, 1, 1);
	public bool ClipEnabled = false;
	public bool PixelFitting = true;
	public SpriteAnchor AnchorX = SpriteAnchor.Middle;
	public SpriteAnchor AnchorY = SpriteAnchor.Middle;
	public SpriteFlip Flip = SpriteFlip.None;

	protected SpriteMesh mSpriteMesh;
}
