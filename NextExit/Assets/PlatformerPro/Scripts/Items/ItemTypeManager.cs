using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace PlatformerPro 
{
	public class ItemTypeManager : MonoBehaviour
	{

		[Header ("Slots")]
		/// <summary>
		/// Some default slots.
		/// </summary>
		public List<string> slots = new List<string> {"NONE", "WEAPON", "OFF-HAND", "HELM", "CHEST", "LEGS", "BOOTS", "RING", "NECK"};

       
        [Header ("Item Data")]

        /// <summary>
        /// How to load item data.
        /// </summary>
        public ItemDataLoadType loadType;

        /// <summary>
        /// The item type data location.
        /// </summary>
        public string itemTypeDataLocation;

        /// <summary>
        /// The loaded.
        /// </summary>
        protected bool loaded = false;

		/// <summary>
		/// The loaded item data.
		/// </summary>
		protected List<ItemTypeData> itemData;

        /// <summary>
        /// Constant to use for cases where an empty or none type is required.
        /// </summary>
        public const string NONE_TYPE = "NONE";

		#region Unity hooks

		/// <summary>
		/// Unity Awake hook.
		/// </summary>
		void Awake()
		{
			Init ();
			Instance = this;
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void Init()
		{
			if (itemTypeDataLocation == null || itemTypeDataLocation == "")
			{
				Debug.LogWarning ("No Item Type Data Location specified. Items and Item Manager will not work");
				return;
			}
			Reload ();
		}

		virtual public void Reload() {
            switch (loadType)
            {
                case ItemDataLoadType.RESOURCE:
                    string resourceName = Path.GetFileNameWithoutExtension(itemTypeDataLocation);
                    TextAsset rawItemData = Resources.Load<TextAsset>(resourceName);
                    if (rawItemData == null)
                    {
                        itemData = new List<ItemTypeData>();
                    } 
                    else 
                    { 
                        itemData = ItemTypeData.Load(rawItemData.bytes);
                    }
                    loaded = true;
                    break;
                case ItemDataLoadType.FILE_SYSTEM:
                    itemData = ItemTypeData.Load(Application.dataPath + Path.DirectorySeparatorChar + itemTypeDataLocation);
                    loaded = true;
                    break;
            }
		}

		/// <summary>
		/// Getts all item types.
		/// </summary>
		/// <returns>The types.</returns>
		public List<string> ItemTypes {
			get {
				#if !UNITY_EDITOR
				Debug.LogWarning ("ItemTypeManager.ItemTypes is expensive. Its intended for editor usage");
				#endif
				return itemData.Select (i => i.typeId).ToList ();
			}
		}

		/// <summary>
		/// Getts all item types. be careful, this DOES NOT RETURN A COPY.
		/// </summary>
		/// <returns>The types.</returns>
		public List<ItemTypeData> ItemData {
			get {

				return itemData;
			}
		}

		/// <summary>
		/// Gets the type data for the given type.
		/// </summary>
		/// <returns>The type data.</returns>
		/// <param name="typeId">Type Identifier.</param>
		public ItemTypeData GetTypeData(string typeId) 
		{
			for (int i = 0; i < itemData.Count; i++) 
			{
				if (itemData[i].typeId == typeId) return itemData[i];
			}
			return null;
		}

		#endregion

		/// <summary>
		/// Stores Item Type Manager instance.
		/// </summary>
		protected static ItemTypeManager _instance;

		/// <summary>
		/// Gets a static reference to the  Item Type manager if one exists.
		/// </summary>
		/// <value>The instance.</value>
		public static ItemTypeManager Instance
		{
			get {
				if (_instance != null) return _instance;
				_instance = FindObjectOfType<ItemTypeManager> ();
				if (_instance != null)
				{
					_instance.Init ();
				} else
				{
					_instance = null;
				}
				return _instance;
			}
			protected set {
				_instance = value;
			}
		}

	}

    public enum ItemDataLoadType
    {
        RESOURCE,
        ASSET_BUNDLE,
        FILE_SYSTEM
    }
}
