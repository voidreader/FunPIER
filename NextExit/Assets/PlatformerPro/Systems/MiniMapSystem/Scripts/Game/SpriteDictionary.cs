using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using jnamobile.mmm;

namespace PlatformerPro {
		
	/// <summary>
	/// Stores key to sprite references. Required for multi-scene maps.
	/// </summary>
	public class SpriteDictionary : MonoBehaviour {

		/// <summary>
		/// The default sprite dictionary prefab location. If editor can't find a SpriteDictionary one will be created from this prefab.
		/// </summary>
		const string DefaultSpriteDictionaryPrefabLocation = "Assets/PlatformerPro/Prefabs/DefaultSpriteDictionary.prefab";
			
		/// <summary>
		/// Sprites used in the map.
		/// </summary>
		public List<Sprite> items;

		/// <summary>
		/// Map of sprites to names for ffaster access.
		/// </summary>
		private Dictionary<string, Sprite> sprites;

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

		/// <summary>
		/// Create a dictionary for fast access.
		/// </summary>
		void Init() {
			sprites = new Dictionary<string, Sprite> ();
			foreach (Sprite sprite in items) {
				if (sprite == null) {
					Debug.LogWarning ("Empty entries found in SpriteDictionary");
				} else {
					if (sprites.ContainsKey (sprite.name)) {
						Debug.LogWarning ("Duplicates found in SpriteDictionary");
					} else {
						sprites.Add (sprite.name, sprite);
					}
				}
			}
		}
		/// <summary>
		/// Gets the sprite for the given name.
		/// </summary>
		/// <returns>The sprite or null if name == NONE or the sprite wasn't found.</returns>
		/// <param name="name">Name.</param>
		Sprite GetSpriteByName(string name) {
#if UNITY_EDITOR
			if (name == "NONE") return null;
			if (items == null) return null;
			Sprite item = items.Where (i=>i.name == name).FirstOrDefault();
			if (item != null) return item;
#endif
			if (sprites == null) return null;
			if (sprites.ContainsKey (name)) return sprites [name];
			return null;
		}

		/// <summary>
		/// Gets reference to the SpriteDictinoary in the scene.
		/// </summary>
		/// <value>The instance.</value>
		public static SpriteDictionary Instance {
			get;
			protected set;
		}

		public static Sprite GetSprite(string name) {
			FindOrCreateInstance ();
			if (Instance == null) return null;
			return Instance.GetSpriteByName (name);
		}


		string[] GetNames() {
			if (items == null) return new string[0];
			List<string> result = items.Select (i => i.name).ToList ();
			result.Insert (0, "NONE");
			return result.ToArray ();
		}

		/// <summary>
		/// Gets all names.
		/// </summary>
		public static string[] Names {
			get {
				Instance = FindObjectOfType<SpriteDictionary> ();
				if (Instance == null) return new string[] {"NONE"};
				return Instance.GetNames ();
			}
		}

		/// <summary>
		/// Get name for specific sprite.
		/// </summary>
		/// <returns>The name of the sprite.</returns>
		/// <param name="sprite">Sprite.</param>
		string NameOfSprite(Sprite sprite) {
			if (items == null) return null;
			if (sprite == null) return "NONE";
			if (items.Contains (sprite)) return sprite.name;
			return null;
		}

		/// <summary>
		/// Gets name for sprite.
		/// </summary>
		/// <returns>The for sprite.</returns>
		/// <param name="sprite">Sprite.</param>
		public static string NameForSprite(Sprite sprite) {
			FindOrCreateInstance ();
			if (Instance == null) return null;
			return Instance.NameOfSprite (sprite);
		}

		public static SpriteDictionary FindOrCreateInstance()
		{
			if (Instance != null && Instance.gameObject == null) Instance = null;
			if (Instance != null) return Instance;
			Instance = FindObjectOfType<SpriteDictionary> ();
			#if UNITY_EDITOR
			if (Instance == null) {
				Debug.LogWarning("No SpriteDictionary found, creating one from the default prefab");
				GameObject spriteDictionaryPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(DefaultSpriteDictionaryPrefabLocation);
				if (spriteDictionaryPrefab != null) {
					GameObject go = (GameObject) UnityEditor.PrefabUtility.InstantiatePrefab(spriteDictionaryPrefab);
					go.name = "SpriteDictionary";
					Instance = go.GetComponent<SpriteDictionary>();
				} else {
					Debug.LogWarning("No default sprite dictionary prefab found. You will need to create your own SpriteDictionary!");
				}
			}
			#endif
			return Instance;
		}

		/// <summary>
		/// Add sprite to dictionary.
		/// </summary>
		/// <returns>The new sprite.</returns>
		/// <param name="sprite">Sprite.</param>
		string AddNewSprite(Sprite sprite) {
			#if UNITY_EDITOR
			if (gameObject == null) {
				Debug.LogError("Unexpected configuration error in SpriteDictionary! Try creating a new one.");
				return null;
			}
			GameObject prefab = (GameObject)UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
			if (prefab != null) {
				if (sprite == null) return null;
				if (items == null) items = new List<Sprite>();
				if (items.Contains (sprite)) return sprite.name;
				items.Add (sprite);
				UnityEditor.AssetDatabase.StartAssetEditing();
				prefab.GetComponent<SpriteDictionary>().items = items;
				UnityEditor.AssetDatabase.SaveAssets();
				UnityEditor.AssetDatabase.StopAssetEditing();
				return sprite.name;
			}
			else {
				Debug.LogWarning("Your sprite dictionary isn't connected to a prefab. The settings will not be carried across scenes.");
			}
			#endif
			if (sprite == null) return null;
			if (items == null) items = new List<Sprite>();
			if (items.Contains (sprite)) return sprite.name;
			items.Add (sprite);
			return sprite.name;
		}

		/// <summary>
		/// Adds sprite to list.
		/// </summary>
		/// <returns>The sprite.</returns>
		/// <param name="sprite">Sprite.</param>
		public static string AddSprite(Sprite sprite) {
			FindOrCreateInstance ();
			if (Instance == null) return null;
			return Instance.AddNewSprite (sprite);
		}

	}
}
