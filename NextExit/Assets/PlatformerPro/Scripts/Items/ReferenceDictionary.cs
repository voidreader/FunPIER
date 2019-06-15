using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using jnamobile.mmm;

namespace PlatformerPro {
		
	/// <summary>
	/// Stores asset references associated with a name attribute.
	/// </summary>
	public abstract class ReferenceDictionary <T> : MonoBehaviour  where T : Object {

		/// <summary>
		/// The default dictionary prefab location. If editor can't find a Dictionary one will be created from this prefab.
		/// </summary>
		public static string DefaultDictionaryPrefabLocation
		{
			get
			{
				Debug.Log ("Assets/2DPlatformerPro/Prefabs/Default" + typeof(T).Name + "Dictionary.prefab");
				return "Assets/2DPlatformerPro/Prefabs/Default" + typeof(T).Name + "Dictionary.prefab";
			}
		}
			
		/// <summary>
		/// Assets used in the map.
		/// </summary>
		public List<T> items;

		/// <summary>
		/// Map of assets to names for ffaster access.
		/// </summary>
		private Dictionary<string, T> assets;


		/// <summary>
		/// Create a dictionary for fast access.
		/// </summary>
		protected void Init() {
			assets = new Dictionary<string, T> ();
			foreach (T asset in items) {
				if (asset == null) {
					Debug.LogWarning ("Empty entries found in Dictionary");
				} else {
					if (assets.ContainsKey (asset.name)) {
						Debug.LogWarning ("Duplicates found in Dictionary");
					} else {
						assets.Add (asset.name, asset);
					}
				}
			}
		}
		/// <summary>
		/// Gets the asset for the given name.
		/// </summary>
		/// <returns>The asset or null if name == NONE or the asset wasn't found.</returns>
		/// <param name="name">Asset name.</param>
		public T GetAssetByName(string name) {
#if UNITY_EDITOR
			if (name == "NONE") return null;
			if (items == null) return null;
			T item = items.Where (i=>i.name == name).FirstOrDefault();
			if (item != null) return item;
#endif
			if (assets == null) return null;
			if (assets.ContainsKey (name)) return assets [name];
			return null;
		}
			
		public string[] GetNames() {
			if (items == null) return new string[0];
			List<string> result = items.Select (i => i.name).ToList ();
			result.Insert (0, "NONE");
			return result.ToArray ();
		}

		/// <summary>
		/// Get name for specific asset.
		/// </summary>
		/// <returns>The name of the asset.</returns>
		/// <param name="asset">Asset.</param>
		public string NameOfAsset(T asset) {
			if (items == null) return null;
			if (asset == null) return "NONE";
			if (items.Contains (asset)) return asset.name;
			return null;
		}

		/// <summary>
		/// Add asset to dictionary.
		/// </summary>
		/// <returns>The new asset.</returns>
		/// <param name="asset">Assset.</param>
		public string AddNewAsset(T asset) {
			#if UNITY_EDITOR
			if (gameObject == null) {
				Debug.LogError("Unexpected configuration error in ReferenceDictionary! Try creating a new one.");
				return null;
			}
			GameObject prefab = (GameObject)UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
			if (prefab != null) {
				if (asset == null) return null;
				if (items == null) items = new List<T>();
				if (items.Contains (asset)) return asset.name;
				items.Add (asset);
				UnityEditor.AssetDatabase.StartAssetEditing();
				prefab.GetComponent<ReferenceDictionary<T>>().items = items;
				UnityEditor.AssetDatabase.SaveAssets();
				UnityEditor.AssetDatabase.StopAssetEditing();
				return asset.name;
			}
			else {
				Debug.LogWarning("Your dictionary isn't connected to a prefab. The settings will not be carried across scenes.");
			}
			#endif
			if (asset == null) return null;
			if (items == null) items = new List<T>();
			if (items.Contains (asset)) return asset.name;
			items.Add (asset);
			return asset.name;
		}


	}
}
