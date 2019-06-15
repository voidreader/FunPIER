using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PlatformerPro {
	
	public class PrefabDictionary : ReferenceDictionary <GameObject>
	{

		/// <summary>
		/// Unity Awake hook.
		/// </summary>
		void Awake() {
			if (Instance == null) {
				Instance = this;
				Init ();
			} else {
				Destroy (this);
			}
		}


		public static PrefabDictionary FindOrCreateInstance()
		{
			if (Instance != null && Instance.gameObject == null) Instance = null;
			if (Instance != null) return Instance;
			Instance = FindObjectOfType<PrefabDictionary> ();
			#if UNITY_EDITOR
			if (Instance == null) {
				Debug.LogWarning("No Dictionary found, creating one from the default prefab");
				GameObject assetDictionaryPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(DefaultDictionaryPrefabLocation);
				if (assetDictionaryPrefab != null) {
					GameObject go = (GameObject) UnityEditor.PrefabUtility.InstantiatePrefab(assetDictionaryPrefab);
					go.name = "PrefabDictionary";
					Instance = go.GetComponent<PrefabDictionary>();
				} else {
					Debug.LogWarning("No default reference dictionary prefab found. You will need to create your own ReferenceDictionary!");
				}
			}
			#endif
			return Instance;
		}

		/// <summary>
		/// Gets reference to the Dictionary in the scene.
		/// </summary>
		/// <value>The instance.</value>
		protected static PrefabDictionary Instance {
			get;
			set;
		}
	}

}
