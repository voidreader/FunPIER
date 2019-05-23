using UnityEngine;
using System.Collections;

public enum SpriteAnchor {
	Minimum = 0x00,
	Middle = 0x01,
	Maximum = 0x02,
}

public enum SpriteFlip {
	None = 0,
	FlipX = 1,
	FlipY = 2,
	FlipXY = 3,
}

public class SpriteMesh {

	public SpriteMesh() {
		mMesh = new Mesh();
		mVertices = new Vector3[4];
		mColors = new Color[4];
		mUVs = new Vector2[4];
		mTriangles = new int[6];

		for (int i = 0; i < 4; ++i) {
			mColors[i] = new Color(1, 1, 1, 1);
		}
		DefaultUVs(SpriteFlip.None);
		UpdateTriangles();
	}

	public void UpdateMesh() {
		mMesh.vertices = mVertices;
		mMesh.uv = mUVs;
		mMesh.colors = mColors;
		mMesh.triangles = mTriangles;
	}

	protected void UpdateTriangles() {
		mTriangles[0] = 0;
		mTriangles[1] = 2;
		mTriangles[2] = 3;

		mTriangles[3] = 0;
		mTriangles[4] = 3;
		mTriangles[5] = 1;
	}

	public void UpdateVertices(Vector2 pos, Vector2 size) {
		float ax = pos.x - size.x / 2;
		float ay = pos.y - size.y / 2;
		mVertices[0] = new Vector3(ax, ay, 0);
		mVertices[1] = new Vector3(ax + size.x, ay, 0);
		mVertices[2] = new Vector3(ax, ay + size.y, 0);
		mVertices[3] = new Vector3(ax + size.x, ay + size.y, 0);
	}

	public void UpdateVertices(SpriteAnchor anchorX, SpriteAnchor anchorY, Vector2 size) {
		float ax = 0;
		float ay = 0;
		if ((anchorX & SpriteAnchor.Middle) != 0) {
			ax = -size.x / 2;
		} else if ((anchorX & SpriteAnchor.Maximum) != 0) {
			ax = -size.x;
		}
		if ((anchorY & SpriteAnchor.Middle) != 0) {
			ay = -size.y / 2;
		} else if ((anchorY & SpriteAnchor.Maximum) != 0) {
			ay = -size.y;
		}
		mVertices[0] = new Vector3(ax, ay, 0);
		mVertices[1] = new Vector3(ax + size.x, ay, 0);
		mVertices[2] = new Vector3(ax, ay + size.y, 0);
		mVertices[3] = new Vector3(ax + size.x, ay + size.y, 0);
	}

	public void UpdateUVs(SpriteFlip flip, Rect clipRect, Texture texture) {
		float xmin = clipRect.xMin / texture.width;
		float xmax = clipRect.xMax / texture.width;
		float ymin = clipRect.yMin / texture.height;
		float ymax = clipRect.yMax / texture.height;
		SetUVs(flip, xmin, xmax, ymin, ymax);
	}

	public void DefaultUVs(SpriteFlip flip) {
		SetUVs(flip, 0, 1, 0, 1);
	}

	public void SetUVs(SpriteFlip flip, float xmin, float xmax, float ymin, float ymax) {
		mUVs[sIndexOrder[(int)flip, 0]] = new Vector2(xmin, ymin);
		mUVs[sIndexOrder[(int)flip, 1]] = new Vector2(xmax, ymin);
		mUVs[sIndexOrder[(int)flip, 2]] = new Vector2(xmin, ymax);
		mUVs[sIndexOrder[(int)flip, 3]] = new Vector2(xmax, ymax);
	}

	public void UpdateColors(Color color) {
		for (int i = 0; i < 4; ++i) {
			mColors[i] = color;
		}
	}

	public Mesh Mesh {
		get { return mMesh; }
	}

	private static readonly int[,] sIndexOrder = new int[4, 4] {
		{ 0, 1, 2, 3 },
		{ 1, 0, 3, 2 },
		{ 2, 3, 0, 1 },
		{ 3, 2, 1, 0 }
	};
	private Mesh mMesh;
	private Vector3[] mVertices;
	private Vector2[] mUVs;
	private Color[] mColors;
	private int[] mTriangles;
}