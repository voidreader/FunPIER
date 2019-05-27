using UnityEngine;
using System.Collections;

public class KySprite : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Assert.AssertNotNull(Texture);

		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		Mesh mesh = (Mesh)Resources.Load("GameMeshes/SimplePlane");
		Assert.AssertNotNull(mesh);
		meshFilter.sharedMesh = mesh;
		gameObject.AddComponent<MeshRenderer>();
		GetComponent<Renderer>().material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended"));

		if (ClipRect.width != 0 && ClipRect.height != 0) {
			GetComponent<Renderer>().material.mainTextureOffset = new Vector2(
				ClipRect.xMin / Texture.width,
				(Texture.height - ClipRect.yMin - ClipRect.height) / Texture.height);
			GetComponent<Renderer>().material.mainTextureScale = new Vector2(
				ClipRect.width / Texture.width,
				ClipRect.height / Texture.height);
			transform.localScale = new Vector3(
				ClipRect.width,
				ClipRect.height,
				1);
		} else {
			transform.localScale = new Vector3(
				Texture.width,
				Texture.height,
				1);
		}
		GetComponent<Renderer>().material.mainTexture = Texture;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Texture Texture;
	public Rect ClipRect;
}
