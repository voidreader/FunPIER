using UnityEngine;
using System.Collections;

public class SpriteUtil {

	public static void CreateSpriteFromTexture(GameObject obj, Texture texture) {
		SpriteMesh sm = new SpriteMesh();
		sm.UpdateVertices(SpriteAnchor.Middle, SpriteAnchor.Middle, new Vector2(texture.width, texture.height));
		sm.UpdateMesh();
		if (obj.GetComponent<MeshFilter>() == null) {
			MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = sm.Mesh;
		}
		if (obj.GetComponent<MeshRenderer>() == null && texture != null) {
			MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
			Material material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended"));
			DebugUtil.Log("create material : " + obj.name);
			renderer.sharedMaterial = material;
			renderer.sharedMaterial.mainTexture = texture;
		}
	}

	public static void SetVerticesColor(GameObject obj, Color color) {
		MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
		if (meshFilter != null) {
			Color[] colors = meshFilter.sharedMesh.colors;
			for (int i = 0; i < colors.Length; ++i) {
				colors[i] = color;
			}
			meshFilter.sharedMesh.colors = colors;
		} else {
			MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
			if (renderer.material != null) {
				renderer.material.color = color;
			}
		}
	}

	public static Color GetVerticesColor(GameObject obj) {
		MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
		if (meshFilter != null && meshFilter.sharedMesh.colors.Length > 0) {
			return meshFilter.sharedMesh.colors[0];
		} else {
			return new Color();
		}
	}

}
