using UnityEngine;
using System.Collections;

public class SpriteRect : Sprite {

	void Awake() {
		mMeshFilter = gameObject.AddComponent<MeshFilter>();
		MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();

		mMesh = new Mesh();
		mVertices = new Vector3[4];
		mUVs = new Vector2[4];
		mTriangles = new int[6];
		mColors = new Color[4];

		UpdateMesh();

		renderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended"));
		renderer.material.mainTexture = Texture;
	}

	void Start () {
	
	}
	
	void Update () {
		//float color = Mathf.Pow(Mathf.Sin(Time.realtimeSinceStartup), 2);
		//Colors[0] = new Color(color, 0, 0, color);
		//UpdateColors();
	}

	private void UpdateVerts() {
		mVertices[0] = new Vector3(mAnchorX, mAnchorY, 0);
		mVertices[1] = new Vector3(mAnchorX + Width, mAnchorY, 0);
		mVertices[2] = new Vector3(mAnchorX, mAnchorY + Height, 0);
		mVertices[3] = new Vector3(mAnchorX + Width, mAnchorY + Height, 0);
	}

	private void UpdateAnchor() {
		mAnchorX = -Width / 2;
		mAnchorY = -Height / 2;
	}

	private void UpdateTriangles() {
		mTriangles[0] = 0;
		mTriangles[1] = 2;
		mTriangles[2] = 3;

		mTriangles[3] = 0;
		mTriangles[4] = 3;
		mTriangles[5] = 1;
	}

	private void SetDefaultColors() {
		for (int i = 0; i < 4; ++i) {
			mColors[i] = TintColor;
		}
	}

	private void UpdateColors() {
		mMesh.colors = mColors;
	}

	private void SetDefaultUVs() {
		mUVs[0] = new Vector2(0, 0);
		mUVs[1] = new Vector2(1, 0);
		mUVs[2] = new Vector2(0, 1);
		mUVs[3] = new Vector2(1, 1);
	}

	private void UpdateMesh() {
		UpdateTriangles();
		UpdateAnchor();
		UpdateVerts();
		SetDefaultColors();
		SetDefaultUVs();

		mMesh.vertices = mVertices;
		mMesh.uv = mUVs;
		mMesh.triangles = mTriangles;
		mMesh.colors = mColors;
		mMeshFilter.sharedMesh = mMesh;
	}

	public Color[] Colors {
		get { return mColors; }
	}

	public Vector2[] UVs {
		get { return mUVs; }
	}

	public float Width = 1;
	public float Height = 1;
	public Color TintColor = new Color();
	public Texture Texture = null;

	private Mesh mMesh;
	private MeshFilter mMeshFilter;
	private Vector3[] mVertices;
	private Vector2[] mUVs;
	private int[] mTriangles;
	private Color[] mColors;
	private float mAnchorX = 0;
	private float mAnchorY = 0;
}
