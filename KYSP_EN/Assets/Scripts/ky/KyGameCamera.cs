using UnityEngine;
using System.Collections;

public class KyGameCamera : MonoBehaviour {

	void Awake() {
		Assert.AssertNotNull(Material);
		GetComponent<Camera>().orthographic = true;
		GetComponent<Camera>().orthographicSize = 240;
		GetComponent<Camera>().aspect = 1.0f;

		if (Material.mainTexture != null) {
			mRenderTexture = (RenderTexture)Material.mainTexture;
		}
		SetupRenderTexture();
		mRenderTexture.Release();
	}

	void OnApplicationPause(bool pause) {
		if (!pause) {
			mRenderTexture.Release();
		}
	}

	private RenderTexture SetupRenderTexture() {
		if (!RenderTextureEnabled) {
			mRenderTexture = new RenderTexture(
				480, 480, 16, RenderTextureFormat.RGB565);
			mRenderTexture.filterMode = FilterMode.Bilinear;
			mRenderTexture.wrapMode = TextureWrapMode.Clamp;
		}
		GetComponent<Camera>().targetTexture = mRenderTexture;
		//Material.mainTexture = mRenderTexture;
		return mRenderTexture;
	}

	public bool RenderTextureEnabled {
		get {
			return (mRenderTexture != null);
		}
	}

	public RenderTexture RenderTexture {
		get { return mRenderTexture; }
	}

	public Material Material = null;

	public RenderTexture mRenderTexture = null;

}
