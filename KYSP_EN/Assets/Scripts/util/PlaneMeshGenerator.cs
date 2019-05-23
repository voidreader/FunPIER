using UnityEngine;
using System.Collections;

public class PlaneMeshGenerator {

	/*public class Description {
		public Vector2 PlaneSize;
		public int WidthSegments;
		public int HeightSegments;
	}*/

	public PlaneMeshGenerator(Vector2 size, int segX, int segY) {
		mPlaneSize = size;
		mWidthSegments = segX;
		mHeightSegments = segY;
	}

	public Mesh Generate() {
		Mesh mesh = new Mesh();
		mesh.name = "mesh";

		float width = mPlaneSize.x;
		float height = mPlaneSize.y;
		int hCount = mWidthSegments + 1;
		int vCount = mHeightSegments + 1;
		int numVertices = hCount * vCount;
		int numTriangles = mWidthSegments * mHeightSegments * 6;
		Vector3[] verts = new Vector3[numVertices];
		Vector2[] uvs = new Vector2[numVertices];
		Color[] colors = new Color[numVertices];
		int[] triangles = new int[numTriangles];

		int index = 0;
		float uvFactorX = 1.0f / mWidthSegments;
		float uvFactorY = 1.0f / mHeightSegments;
		float scaleX = mPlaneSize.x / mWidthSegments;
		float scaleY = mPlaneSize.y / mHeightSegments;

		for (int y = 0; y < vCount; ++y) {
			for (int x = 0; x < hCount; ++x) {
				verts[index] = new Vector3(
					x * scaleX - width / 2.0f,
					y * scaleY - height / 2.0f);
				uvs[index] = new Vector2(x * uvFactorX, y * uvFactorY);
				colors[index] = new Color(1, 1, 1, 1);
				index++;
			}
		}

		index = 0;
		for (int y = 0; y < mHeightSegments; ++y) {
			for (int x = 0; x < mWidthSegments; ++x) {
				triangles[index++] = (y * hCount) + x;
				triangles[index++] = ((y + 1) * hCount) + x;
				triangles[index++] = (y * hCount) + x + 1;
				triangles[index++] = ((y + 1) * hCount) + x;
				triangles[index++] = ((y + 1) * hCount) + x + 1;
				triangles[index++] = (y * hCount) + x + 1;
			}
		}

		mesh.vertices = verts;
		mesh.uv = uvs;
		mesh.colors = colors;
		mesh.triangles = triangles;

		return mesh;
	}

	public Vector2 PlaneSize {
		get { return mPlaneSize; }
		set { mPlaneSize = value; }
	}

	private Vector2 mPlaneSize;
	private int mWidthSegments = 0;
	private int mHeightSegments = 0;
}
