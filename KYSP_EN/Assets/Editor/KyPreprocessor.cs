using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

/// <summary>
/// アセットがインポートされたときに自動的に設定を変更するメソッド群。
/// </summary>
public class KyPreprocessor : AssetPostprocessor {

	/// <summary>
	/// 各テクスチャがインポートされたときに呼ばれる。
	/// GameTextureのみ、TextureTypeをGUIとする。
	/// </summary>
	void OnPreprocessTexture() {
		TextureImporter importer = assetImporter as TextureImporter;
		if (importer != null) {
			if (assetPath.Contains("Resources/GameTextures/")) {
				importer.textureType = TextureImporterType.GUI;
				importer.npotScale = TextureImporterNPOTScale.None;
				//importer.wrapMode = TextureWrapMode.Clamp;
				importer.mipmapEnabled = false;

				DebugUtil.Log("KyPreprocessor set texture importer for " + Path.GetFileName(assetPath));
			}
		}
	}

	void OnPreprocessAudio() {
		AudioImporter importer = assetImporter as AudioImporter;
		if (importer != null) {
			if (assetPath.Contains("Resources/GameAudioClips")) {
				importer.threeD = false;
				DebugUtil.Log("KyPreprocessor set audio importer for " + Path.GetFileName(assetPath));
			}
		}
	}

}
