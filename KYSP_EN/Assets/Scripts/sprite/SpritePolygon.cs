using UnityEngine;
using System.Collections;

public class SpritePolygon : Sprite {

	void Awake() {
		if (GetComponent<Renderer>() == null) {
			gameObject.AddComponent<MeshRenderer>();
		}
		if (GetComponent<Renderer>().sharedMaterial == null) {
			GetComponent<Renderer>().sharedMaterial = Material;
		}
		mMeshFilter = gameObject.AddComponent<MeshFilter>();
		mMesh = new Mesh();
		mMeshFilter.sharedMesh = mMesh;
		mVertices = new Vector3[VertexCount];
		mColors = new Color[VertexCount];
		mUVs = new Vector2[VertexCount];
		mTriangles = new int[(VertexCount - 2) * 3];
		for (int i = 0, tri = 0; i < mTriangles.Length; i+=3, tri++) {
			mTriangles[i] = 0;
			mTriangles[i + 1] = tri + 1;
			mTriangles[i + 2] = tri + 2;
		}
		SetColors(Color);
		mMesh.vertices = mVertices;
		mMesh.colors = mColors;
		mMesh.uv = mUVs;
		mMesh.triangles = mTriangles;
	}

	void Start() {
		UpdateAll();
	}

	public override void UpdateAll() {
		UpdateVertices();
		UpdateColors();
	}

	public void UpdateVertices() {
		mMesh.vertices = mVertices;
	}

	public void UpdateColors() {
		mMesh.colors = mColors;
	}

	public void SetVertices(int vertices) {
		mVertices = new Vector3[vertices];
	}

	public void SetColors(Color color) {
		for (int i = 0; i < VertexCount; ++i) {
			mColors[i] = color;
		}
	}

	public Mesh Mesh {
		get { return mMesh; }
	}

	public Vector3[] Vertices {
		get { return mVertices; }
	}

	public Material Material;
	public Color Color;
	public int VertexCount = 3;

	private Mesh mMesh = null;
	private MeshFilter mMeshFilter = null;
	private Vector3[] mVertices = null;
	private Vector2[] mUVs = null;
	private Color[] mColors = null;
	private int[] mTriangles = null;
}
