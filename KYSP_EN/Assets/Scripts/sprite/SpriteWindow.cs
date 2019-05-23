using UnityEngine;
using System.Collections;

public class SpriteWindow : Sprite {

	#region MonoBehaviour Methods

	void Awake() {
		if (GetComponent<Renderer>() == null) {
			gameObject.AddComponent<MeshRenderer>();
		}
		if (GetComponent<Renderer>().sharedMaterial == null) {
			GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Particles/Alpha Blended"));
		}
		if (Texture != null) {
			GetComponent<Renderer>().sharedMaterial.mainTexture = Texture;
		} else {
			Texture = GetComponent<Renderer>().sharedMaterial.mainTexture;
		}
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		Mesh mesh = new Mesh();
		CreateMesh(mesh);
		meshFilter.sharedMesh = mesh;
	}

	#endregion

	#region Methods

	private void CreateMesh(Mesh mesh) {
		float wx = WindowSize.x, wy = WindowSize.y;
		float cx = CellSize, cy = CellSize;
		float[] posx = new float[4] {
			-wx / 2, -wx / 2 + cx, +wx / 2 - cx, +wx / 2
		};
		float[] posy = new float[4] {
			-wy / 2, -wy / 2 + cy, +wy / 2 - cy, +wy / 2
		};
		Vector3[] pos = new Vector3[16];
		Vector2[] uvs = new Vector2[16];
		int index = 0;
		for (int j = 0; j < 4; ++j) {
			for (int i = 0; i < 4; ++i) {
				pos[index] = new Vector3(posx[i], posy[j], 0);
				uvs[index] = new Vector2((float)i / 3, (float)j / 3);
				index++;
			}
		}
		index = 0;
		int[] tris = new int[54];
		for (int j = 0; j < 3; ++j) {
			for (int i = 0; i < 3; ++i) {
				int n = i + j * 4;
				tris[index + 0] = n;
				tris[index + 1] = n + 1;
				tris[index + 2] = n + 5;
				tris[index + 3] = n;
				tris[index + 4] = n + 5;
				tris[index + 5] = n + 4;
				index += 6;
			}
		}
		mesh.Clear();
		mesh.vertices = pos;
		mesh.uv = uvs;
		mesh.triangles = tris;
	}

	#endregion

	#region Fields

	public Texture Texture;	//	ソーステクスチャ
	public float CellSize = 0;	//	ソーステクスチャのウインドウ片のセルサイズ
	public Vector2 WindowSize;	//	ウインドウサイズ

	#endregion
}
