using UnityEngine;
using System.Collections;

/// <summary>
/// ���u�Ռ������v�p���U�C�N�X�N���v�g�B
/// </summary>
public class KySyogekiMosaic : MonoBehaviour {

	void Awake() {
		mMesh = GenerateMesh();
		mColors = new Color[mMesh.vertexCount];
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		gameObject.AddComponent<MeshRenderer>();
		GetComponent<Renderer>().material = new Material(Shader.Find("Particles/Alpha Blended"));
		meshFilter.sharedMesh = mMesh;
		GenerateColors();
	}

	void Start () {
	
	}
	
	void Update () {
		mElapsedTime += Time.deltaTime;
		if (mElapsedTime >= UpdateTime) {
			mElapsedTime = 0;
			GenerateColors();
		}
	}

	private Mesh GenerateMesh() {
		Mesh mesh = new Mesh();
		mesh.name = "mesh";

		float anchorx = -Size.x / 2;
		float anchory = -Size.y / 2;
		int segx = (int)Segments.x;
		int segy = (int)Segments.y;
		Vector3[] verts = new Vector3[segx * segy * 4];
		Vector2[] uvs = new Vector2[segx * segy * 4];
		int[] tris = new int[segx * segy * 6];
		float dx = Size.x / segx;
		float dy = Size.y / segy;
		int v = 0;
		int t = 0;
		for (int y = 0; y < segy; ++y) {
			for (int x = 0; x < segx; ++x) {
				verts[v + 0] = new Vector3(anchorx + x * dx, anchory + y * dy);
				verts[v + 1] = new Vector3(anchorx + (x + 1) * dx, anchory + y * dy);
				verts[v + 2] = new Vector3(anchorx + x * dx, anchory + (y + 1) * dy);
				verts[v + 3] = new Vector3(anchorx + (x + 1) * dx, anchory + (y + 1) * dy);

				tris[t + 0] = v + 0;
				tris[t + 1] = v + 2;
				tris[t + 2] = v + 1;
				tris[t + 3] = v + 2;
				tris[t + 4] = v + 1;
				tris[t + 5] = v + 3;

				v += 4;
				t += 6;
			}
		}
		mesh.vertices = verts;
		mesh.uv = uvs;
		mesh.triangles = tris;
		
		return mesh;
	}

	private void GenerateColors() {
		int segx = (int)Segments.x;
		int segy = (int)Segments.y;
		int i = 0;
		for (int y = 0; y < segy; ++y) {
			for (int x = 0; x < segx; ++x) {
				float gray = Random.Range(GrayMin, GrayMax);
				Color color = new Color(gray, gray, gray, 1.0f);
				mColors[i + 0] = color;
				mColors[i + 1] = color;
				mColors[i + 2] = color;
				mColors[i + 3] = color;
				i += 4;
			}
		}
		mMesh.colors = mColors;
	}

	public Vector2 Size;
	public Vector2 Segments;
	public float UpdateTime = 1.0f;
	public float GrayMin = 0.2f;
	public float GrayMax = 0.8f;

	private Color[] mColors;
	private Mesh mMesh;
	private float mElapsedTime;
}
