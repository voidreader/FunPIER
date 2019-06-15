using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;	
using System.Xml.Serialization;

namespace PlatformerPro 
{
	/// <summary>
	/// Editor for items.
	/// </summary>
	public class ItemTypeEditor :  EditorWindow {

		/// <summary>
		/// Reference to the window
		/// </summary>
		public static ItemTypeEditor window;

		bool initialised;
		Rect itemSelectRect;
		Rect itemDetailsHeaderUpperRightRect;
		Rect itemDetailsHeaderLowerRightRect;
		Rect itemDetailsHeaderLeftRect;
		Rect itemDetailsRect;
		Rect headingRect;
		int selectedAbilityTab;

		List<string> abilityTabs = new List<string>();

		List <ItemTypeData> itemData;

		static ItemTypeManager manager;

		/// <summary>
		/// Some default slots.
		/// </summary>
		public List<string> slots = new List<string> {"NONE", "WEAPON", "OFF-HAND", "HELM", "CHEST", "LEGS", "BOOTS", "RING", "NECK"};

		int selectedItemIndex = -1;

		static GUIStyle headingStyle;
		static Vector2 itemSelectScrollPosition;
		Vector2 itemBehaviourScrollPosition;

		const float columnWidth = 320.0f;
		const float StandardVerticalSpacing = 8;
		const float windowHeight = 600.0f;
		const float borderSize = 4.0f;
		const float labelWidth = 100.0f;

		public static void ShowMainWindow()
		{
			// Lets assume that everyone window is at least 520 x 448
			InitWindow();
			window.Show ();
		}

		static void InitWindow() {
			float windowWidth = (columnWidth * 3.0f) + 1.0f;
			Rect rect = new Rect((Screen.currentResolution.width - windowWidth) / 2.0f,
				(Screen.currentResolution.height - windowHeight) / 2.0f,
				windowWidth , windowHeight);
			window = (ItemTypeEditor) EditorWindow.GetWindowWithRect(typeof(ItemTypeEditor), rect, true, "Item Type Editor");
			window.minSize = new Vector2 (windowWidth, windowHeight);
			window.maxSize = new Vector2 (windowWidth, windowHeight);
			window.position = rect;
			window.Init ();
		}

		void Init() {
			if (initialised) return;

			itemSelectRect = new Rect (-2, -2, columnWidth, windowHeight + 4);

			headingRect = new Rect(itemSelectRect.max.x + borderSize, borderSize, columnWidth * 2.0f - (borderSize * 2), 40.0f);

			itemDetailsHeaderLeftRect = new Rect (itemSelectRect.max.x + borderSize, itemSelectRect.min.y + 32.0f, columnWidth  - (borderSize * 2) , (2 * windowHeight / 3) - (borderSize + 32.0f));

			itemDetailsHeaderUpperRightRect = new Rect (itemDetailsHeaderLeftRect.max.x + (borderSize), itemSelectRect.min.y + 32.0f, columnWidth - (borderSize) , (windowHeight / 3) );

			itemDetailsHeaderLowerRightRect = new Rect (itemDetailsHeaderLeftRect.max.x + (borderSize), itemDetailsHeaderUpperRightRect.max.y + borderSize, columnWidth - (borderSize) , (windowHeight / 3) - 32.0f - (2.0f * borderSize));

			itemDetailsRect = new Rect (itemSelectRect.max.x + borderSize, itemDetailsHeaderLeftRect.max.y + borderSize, columnWidth * 2.0f - (borderSize * 2), windowHeight / 3 - (borderSize + 32.0f));

			headingStyle = new GUIStyle(EditorStyles.label);
			headingStyle.fontStyle = FontStyle.Bold;
			headingStyle.fontSize = 16;
			FindOrCreateItemManager ();
			if (manager.itemTypeDataLocation != null && manager.itemTypeDataLocation != "") Load ();
			initialised = true;
		}

		void FindOrCreateItemManager() {
			if (manager != null) return;
			manager = FindObjectOfType<ItemTypeManager> ();
			if (manager == null)
			{
				Debug.Log ("Creating a new ItemTypeManager as we couldn't find one in the scene.");
				GameObject go = new GameObject ("ItemTypeManager");
				manager = go.AddComponent<ItemTypeManager> ();
			}
		}

		void OnGUI () {

			if (!initialised) Init ();
			GUI.backgroundColor = Color.gray;
			GUILayout.BeginArea(itemSelectRect, EditorStyles.helpBox);
			GUI.backgroundColor = Color.white;
			DrawItemSelectPanel ();
			GUILayout.EndArea ();
			if (selectedItemIndex == -1)
			{
				GUILayout.BeginArea(itemDetailsRect);
				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				GUILayout.Label ("No item selected!");
				GUILayout.FlexibleSpace ();
				GUILayout.EndHorizontal ();
				GUILayout.EndArea ();
				return;
			}
			GUILayout.BeginArea (headingRect);
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			GUILayout.Label (itemData [selectedItemIndex].humanReadableName, headingStyle);
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();
			GUILayout.EndArea ();

			GUI.backgroundColor = new Color (0.65f, 0.85f, 1.0f);
			GUILayout.BeginArea (itemDetailsHeaderLeftRect, EditorStyles.helpBox);
			//GUI.backgroundColor = Color.white;
			DrawItemHeaderLeft ();
			GUILayout.EndArea ();

			GUILayout.BeginArea(itemDetailsHeaderUpperRightRect, EditorStyles.helpBox);
			itemBehaviourScrollPosition = GUILayout.BeginScrollView (itemBehaviourScrollPosition); //, EditorStyles.helpBox);
			DrawItemBehaviour ();
			GUILayout.EndScrollView ();
			GUILayout.EndArea ();

			GUILayout.BeginArea(itemDetailsHeaderLowerRightRect, EditorStyles.helpBox);
			DrawSprites ();
			GUILayout.EndArea ();

			GUILayout.BeginArea(itemDetailsRect, EditorStyles.helpBox);

			DrawItemDetailPanel ();
			GUILayout.EndArea ();
			GUI.backgroundColor = Color.white;
		}

		void OnDestroy() {
			Save();
		}

		void Save() {
			if (manager.itemTypeDataLocation == null || manager.itemTypeDataLocation == "")
				SaveAs ();
			else
				Save (Application.dataPath  + Path.DirectorySeparatorChar + manager.itemTypeDataLocation);
		}

		void SaveAs() {
			string folder = Application.dataPath + Path.DirectorySeparatorChar  + "Resources";
			if (manager.itemTypeDataLocation != null && manager.itemTypeDataLocation != "") folder = System.IO.Path.GetDirectoryName (manager.itemTypeDataLocation);
			if (!Directory.Exists (folder))
			{
				try {
					Directory.CreateDirectory (folder);
				} catch (System.Exception) {
				}
			}
			string savedGameLocation = EditorUtility.SaveFilePanel ("Save Item Type Data", folder, "ItemTypeData", "txt");
			if (savedGameLocation != null && savedGameLocation != "")
			{
				string relativePath = GetRelativePath (Application.dataPath  + Path.DirectorySeparatorChar, savedGameLocation);
				if (relativePath != savedGameLocation && relativePath.Contains("Resources"))
				{
					manager.itemTypeDataLocation = relativePath;
					Save ( Application.dataPath + Path.DirectorySeparatorChar + manager.itemTypeDataLocation );
				} else
				{
					Debug.LogWarning("Path was not inside assets folder or was not a resource folder, file will be saved but the ItemTypeManager path will not be updated");
					Save (savedGameLocation, false);
				}
			}

		}

		void Save(string location, bool saveLocation = true) {
			//try {
				XmlSerializer serializer = new XmlSerializer(typeof(List<ItemTypeData>));
				using (MemoryStream writer = new MemoryStream() ) {
					serializer.Serialize(writer, itemData);
					byte[] gameData = writer.GetBuffer();
					if (gameData != null) {
						File.WriteAllBytes(location, gameData);
						if (saveLocation) {
							PlayerPrefs.SetString ("SaveGameFolderName", location);
							manager.Reload();
						}
					}
				}
			//} catch (System.Exception ex) {
			//	Debug.LogError ("Failed to save item data to: " + location + " with error: " + ex.Message);
			//}
		}

		/// <summary>
		/// Loads data from location in ItemTypeManager if it exists, or prompts user if it does not.
		/// </summary>
		void Load() {
			if (manager.itemTypeDataLocation == null || manager.itemTypeDataLocation == "")
				LoadFrom ();
			else
				Load (Application.dataPath + Path.DirectorySeparatorChar + manager.itemTypeDataLocation);
		}

		/// <summary>
		/// Loads data from the specified location.
		/// </summary>
		/// <param name="location">Location.</param>
		void Load (string location) {
			itemData = ItemTypeData.Load(location);
			itemData = itemData.OrderBy(x=>x.typeId).ToList();
		}

		/// <summary>
		/// Prompts user for load path then loads data from user selected location.
		/// </summary>
		void LoadFrom() {
			string folder = Application.dataPath + Path.DirectorySeparatorChar  + "Resources";
			try {
				if (manager.itemTypeDataLocation != null && manager.itemTypeDataLocation !=  "") folder = Path.GetDirectoryName (manager.itemTypeDataLocation);
			} catch (System.ArgumentException) {}
			string savedGameLocation = EditorUtility.OpenFilePanel("Load Game", folder, "txt");
			if (savedGameLocation != null && savedGameLocation != "")
			{
				string relativePath = GetRelativePath (Application.dataPath + Path.DirectorySeparatorChar, savedGameLocation);
				if (relativePath != savedGameLocation && relativePath.Contains("Resources"))
				{
					manager.itemTypeDataLocation = relativePath;
					Load ( Application.dataPath + Path.DirectorySeparatorChar + manager.itemTypeDataLocation );
				} else
				{
					Debug.LogWarning("Path was not inside assets folder or was not a resource folder, file will be loaded but the ItemTypeManager path will not be updated");
					Load (savedGameLocation);
				}
			}
		}

		bool Ensure() {
			if (itemData == null) itemData = new List<ItemTypeData> ();
			return true;
		}

		void DrawItemSelectPanel() {
			if (!Ensure ()) return;
			DrawItemActions ();
			GUILayout.Space (StandardVerticalSpacing);
			DrawItemScrollView ();

		}

		void DrawItemHeaderLeft() {
			if (!Ensure ())
				return;
			if (selectedItemIndex == -1)
				return;

			GUILayout.Label ("Common Properties", EditorStyles.centeredGreyMiniLabel);
			GUILayout.Space (StandardVerticalSpacing);
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("ID", GUILayout.Width (labelWidth));
			itemData [selectedItemIndex].typeId = EditorGUILayout.TextField (itemData [selectedItemIndex].typeId);
			GUILayout.EndHorizontal ();

			GUILayout.Space (StandardVerticalSpacing);

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Name", GUILayout.Width (labelWidth));
			itemData [selectedItemIndex].humanReadableName = EditorGUILayout.TextField (itemData [selectedItemIndex].humanReadableName);
			GUILayout.EndHorizontal ();

			GUILayout.Space (StandardVerticalSpacing);

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Level", GUILayout.Width (labelWidth));
			itemData [selectedItemIndex].level = EditorGUILayout.IntField (itemData [selectedItemIndex].level);
			GUILayout.EndHorizontal ();

			GUILayout.Space (StandardVerticalSpacing);

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Price", GUILayout.Width (labelWidth));
			itemData [selectedItemIndex].price = EditorGUILayout.IntField (itemData [selectedItemIndex].price);
			GUILayout.EndHorizontal ();

			GUILayout.Space (StandardVerticalSpacing);

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Stacking", GUILayout.Width (labelWidth));
			itemData [selectedItemIndex].itemClass = (ItemClass)EditorGUILayout.EnumPopup (itemData [selectedItemIndex].itemClass);
			GUILayout.EndHorizontal ();

			GUILayout.Space (StandardVerticalSpacing);

			switch (itemData [selectedItemIndex].itemClass)
			{
			case ItemClass.NORMAL:
				EditorGUILayout.HelpBox ("A normal item stacks in the inventory according to stack size.", MessageType.None);
				EditorGUI.indentLevel++;
				itemData [selectedItemIndex].itemMax = EditorGUILayout.IntField ("Item Max", itemData [selectedItemIndex].itemMax);
                if (itemData[selectedItemIndex].itemMax < 1) itemData[selectedItemIndex].itemMax = 1;
                itemData [selectedItemIndex].maxPerStack = EditorGUILayout.IntField ("Stack Max", itemData [selectedItemIndex].maxPerStack);
                if (itemData[selectedItemIndex].maxPerStack < 1) itemData[selectedItemIndex].maxPerStack = 1;
                itemData [selectedItemIndex].startingCount = EditorGUILayout.IntField ("Starting Count", itemData [selectedItemIndex].startingCount);
				EditorGUI.indentLevel--;
				break;
			case ItemClass.UNIQUE:
				EditorGUILayout.HelpBox ("A character may only have one of a unique item.", MessageType.None);
				EditorGUI.indentLevel++;
				GUI.enabled = false;
				itemData [selectedItemIndex].itemMax = 1;
				EditorGUILayout.IntField ("Item Max", itemData [selectedItemIndex].itemMax);
				itemData [selectedItemIndex].maxPerStack = 1;
				EditorGUILayout.IntField ("Stack Max", itemData [selectedItemIndex].maxPerStack);
				GUI.enabled = true;
				itemData [selectedItemIndex].startingCount = EditorGUILayout.IntField ("Starting Count", itemData [selectedItemIndex].startingCount);
				EditorGUI.indentLevel--;
				break;
			case ItemClass.NON_INVENTORY:
				EditorGUILayout.HelpBox ("A non-inventory item stacks in its own custom stack and does not use space in the inventory.", MessageType.None);
				EditorGUI.indentLevel++;
				itemData [selectedItemIndex].itemMax = EditorGUILayout.IntField ("Item Max", itemData [selectedItemIndex].itemMax);
				GUI.enabled = false;
				itemData [selectedItemIndex].maxPerStack = itemData [selectedItemIndex].itemMax;
				EditorGUILayout.IntField ("Stack Max", itemData [selectedItemIndex].maxPerStack);
				GUI.enabled = true;
				itemData [selectedItemIndex].startingCount = EditorGUILayout.IntField ("Starting Count", itemData [selectedItemIndex].startingCount);
				EditorGUI.indentLevel--;
				break;
			case ItemClass.INSTANT:
				EditorGUILayout.HelpBox ("An instant item is consumed when it is picked up and never stored.", MessageType.None);
				EditorGUI.indentLevel++;
				GUI.enabled = false;
				itemData [selectedItemIndex].itemMax = 1;
				EditorGUILayout.IntField ("Item Max", itemData [selectedItemIndex].itemMax);
				itemData [selectedItemIndex].maxPerStack = 1;
				EditorGUILayout.IntField ("Stack Max", itemData [selectedItemIndex].maxPerStack);
				itemData [selectedItemIndex].startingCount = 0;
				EditorGUILayout.IntField ("Starting Count", itemData [selectedItemIndex].startingCount);
				GUI.enabled = true;
				EditorGUI.indentLevel--;
				break;
			}
			// Clean up
			if (itemData [selectedItemIndex].startingCount < 0)
			{
				itemData [selectedItemIndex].startingCount = 0;
			}
			if (itemData [selectedItemIndex].startingCount > itemData [selectedItemIndex].itemMax)
			{
				itemData [selectedItemIndex].startingCount = itemData [selectedItemIndex].itemMax;
			}
			if (itemData [selectedItemIndex].maxPerStack > itemData [selectedItemIndex].itemMax)
			{
				itemData [selectedItemIndex].maxPerStack = itemData [selectedItemIndex].itemMax;
			}
			GUILayout.Space (StandardVerticalSpacing);

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Allow Drop", GUILayout.Width (labelWidth));
			itemData [selectedItemIndex].allowDrop = EditorGUILayout.Toggle (itemData [selectedItemIndex].allowDrop);
			GUILayout.EndHorizontal ();

			if (itemData [selectedItemIndex].allowDrop)
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Drop Prefab", GUILayout.Width (labelWidth));
				GameObject dropPrefab = (GameObject)EditorGUILayout.ObjectField (itemData [selectedItemIndex].dropPrefab, typeof(GameObject), false);
				GUILayout.EndHorizontal ();
				if (dropPrefab != itemData [selectedItemIndex].dropPrefab)
				{
					itemData [selectedItemIndex].dropPrefab = dropPrefab;
					if (itemData [selectedItemIndex].dropPrefab != null)
					{
						itemData [selectedItemIndex].dropPrefabName = PrefabDictionary.FindOrCreateInstance ().AddNewAsset (itemData [selectedItemIndex].dropPrefab);
					} 
					else
					{
						itemData [selectedItemIndex].inGameSpriteName = null;
					}
				}
				if (itemData [selectedItemIndex].dropPrefab == null)
				{
					EditorGUILayout.HelpBox ("If no drop prefab is specific dropping the object will cause it to be destroyed.", MessageType.Info);
				}
			}
		}

		void DrawItemBehaviour() {
			if (!Ensure ())
				return;
			if (selectedItemIndex == -1)
				return;

			GUILayout.Label ("Behaviour", EditorStyles.centeredGreyMiniLabel);

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Behaviour", GUILayout.Width(labelWidth));
			itemData[selectedItemIndex].itemBehaviour = (ItemBehaviour)EditorGUILayout.EnumPopup(itemData[selectedItemIndex].itemBehaviour);
			GUILayout.EndHorizontal ();

			GUILayout.Space (StandardVerticalSpacing);
			int selectedSlotIndex = 0;

			if (itemData [selectedItemIndex].itemBehaviour == ItemBehaviour.EQUIPPABLE)
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Slot", GUILayout.Width (labelWidth));
				selectedSlotIndex = slots.IndexOf (itemData [selectedItemIndex].slot);
				if (selectedSlotIndex < 0) selectedSlotIndex = 0;
				int newSlotIndex = EditorGUILayout.Popup (selectedSlotIndex, slots.ToArray ());
				if (newSlotIndex >= 0) itemData [selectedItemIndex].slot = slots [newSlotIndex];
				GUILayout.EndHorizontal ();

				GUILayout.Space (StandardVerticalSpacing);

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Max Durability", GUILayout.Width(labelWidth));
				if (itemData [selectedItemIndex].maxDurability == -1)
				{
					if (GUILayout.Button ("Breakable", EditorStyles.miniButton))
					{
						itemData [selectedItemIndex].maxDurability = 99;
					}
				} else
				{
					GUILayout.FlexibleSpace ();
					itemData [selectedItemIndex].maxDurability = EditorGUILayout.IntField (itemData [selectedItemIndex].maxDurability);
					if (GUILayout.Button ("Unbreakable", EditorStyles.miniButton))
					{
						itemData [selectedItemIndex].maxDurability = -1;
					}
				}
				GUILayout.EndHorizontal ();

				GUILayout.Space (StandardVerticalSpacing);
			}

			if (itemData [selectedItemIndex].itemBehaviour == ItemBehaviour.POWER_UP)
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Duration", GUILayout.Width(labelWidth));
				if (itemData [selectedItemIndex].effectDuration < 0)
				{
					GUILayout.Label ("Lasts until Death");
					if (GUILayout.Button ("Make Temporary", EditorStyles.miniButton))
					{
						itemData [selectedItemIndex].effectDuration = 1.0f;
					}
				} else
				{
					GUILayout.FlexibleSpace ();
					itemData [selectedItemIndex].effectDuration = EditorGUILayout.FloatField (itemData [selectedItemIndex].effectDuration);
					if (GUILayout.Button ("Last Until Death", EditorStyles.miniButton))
					{
						itemData [selectedItemIndex].effectDuration = -1.0f;
					}
				}
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Reset On Damage", GUILayout.Width(labelWidth));
				itemData [selectedItemIndex].resetEffectOnDamage = EditorGUILayout.Toggle(itemData [selectedItemIndex].resetEffectOnDamage);
				GUILayout.EndHorizontal ();
				GUILayout.Space (StandardVerticalSpacing);
			}


			abilityTabs.Clear ();
			abilityTabs.Add ("Score");

			if (itemData [selectedItemIndex].itemBehaviour != ItemBehaviour.NONE) {
				if (itemData [selectedItemIndex].itemBehaviour == ItemBehaviour.CONSUMABLE)
				{
					abilityTabs.Add ("Healing");
				}
				else
				{
					abilityTabs.Add ("Attack");
					abilityTabs.Add ("Defence");
					abilityTabs.Add ("Agility");
				}
			}
			if (abilityTabs.Count <= selectedAbilityTab) selectedAbilityTab = 0;
			selectedAbilityTab = GUILayout.Toolbar (selectedAbilityTab, abilityTabs.ToArray ());
			switch (abilityTabs [selectedAbilityTab])
			{
			case "":
				break;
			case "Score":
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Score Type", GUILayout.Width (labelWidth));
				itemData [selectedItemIndex].scoreType = EditorGUILayout.TextField (itemData [selectedItemIndex].scoreType);
				GUILayout.EndHorizontal ();
				if (itemData [selectedItemIndex].itemClass != ItemClass.INSTANT)
				{
					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Score on Collect", GUILayout.Width (labelWidth));
					itemData [selectedItemIndex].scoreOnCollect = EditorGUILayout.IntField (itemData [selectedItemIndex].scoreOnCollect);
					GUILayout.EndHorizontal ();
				}
				if (itemData [selectedItemIndex].itemBehaviour != ItemBehaviour.EQUIPPABLE && itemData [selectedItemIndex].itemBehaviour != ItemBehaviour.UPGRADE)
				{
					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Score on Consume", GUILayout.Width (labelWidth));
					itemData [selectedItemIndex].scoreOnConsume = EditorGUILayout.IntField (itemData [selectedItemIndex].scoreOnConsume);
					GUILayout.EndHorizontal ();
				}
				break;
			case "Attack":
				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button ("Reset Values", EditorStyles.miniButton))
				{
					itemData [selectedItemIndex].damageType = DamageType.NONE;
					itemData [selectedItemIndex].damageMultiplier = 1.0f;
					itemData [selectedItemIndex].weaponSpeedMultiplier = 1.0f;
				}
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Damage Type", GUILayout.Width (labelWidth));
				itemData [selectedItemIndex].damageType = (DamageType) EditorGUILayout.EnumPopup (itemData [selectedItemIndex].damageType);
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Damage Multiplier", GUILayout.Width (labelWidth));
				itemData [selectedItemIndex].damageMultiplier = EditorGUILayout.FloatField (itemData [selectedItemIndex].damageMultiplier);
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Speed Multiplier", GUILayout.Width (labelWidth));
				itemData [selectedItemIndex].weaponSpeedMultiplier = EditorGUILayout.FloatField (itemData [selectedItemIndex].weaponSpeedMultiplier);
				GUILayout.EndHorizontal ();
				break;
			case "Defence":
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Invulnerability", GUILayout.Width (labelWidth));
				itemData [selectedItemIndex].invulnerability = EditorGUILayout.Toggle (itemData [selectedItemIndex].invulnerability);
				GUILayout.EndHorizontal ();
				break;
			case "Agility":
				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button ("Reset Values", EditorStyles.miniButton))
				{
					itemData [selectedItemIndex].moveSpeedMultiplier = 1.0f;
					itemData [selectedItemIndex].runSpeedMultiplier = 1.0f;
					itemData [selectedItemIndex].jumpHeightMultiplier = 1.0f;
					itemData [selectedItemIndex].jumpCount = 1;
				}
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Move Speed Multiplier", GUILayout.Width (labelWidth));
				itemData [selectedItemIndex].moveSpeedMultiplier = EditorGUILayout.FloatField (itemData [selectedItemIndex].moveSpeedMultiplier);
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Run Speed Multiplier", GUILayout.Width (labelWidth));
				itemData [selectedItemIndex].runSpeedMultiplier = EditorGUILayout.FloatField (itemData [selectedItemIndex].runSpeedMultiplier);
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Jump Height Multiplier", GUILayout.Width (labelWidth));
				itemData [selectedItemIndex].jumpHeightMultiplier = EditorGUILayout.FloatField (itemData [selectedItemIndex].jumpHeightMultiplier);
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Extra Jump Count", GUILayout.Width (labelWidth));
				itemData [selectedItemIndex].jumpCount = EditorGUILayout.IntField (itemData [selectedItemIndex].jumpCount);
				GUILayout.EndHorizontal ();
				break;
			case "Healing":
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Health Adjustment", GUILayout.Width (labelWidth));
				itemData [selectedItemIndex].healthAdjustment = EditorGUILayout.IntField (itemData [selectedItemIndex].healthAdjustment);
				GUILayout.EndHorizontal ();
				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Max Health Adjustment", GUILayout.Width (labelWidth));
				itemData [selectedItemIndex].maxHealthAdjustment = EditorGUILayout.IntField (itemData [selectedItemIndex].maxHealthAdjustment);
				GUILayout.EndHorizontal ();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Breath Adjustment", GUILayout.Width(labelWidth));
                itemData[selectedItemIndex].breathAdjustment = EditorGUILayout.IntField(itemData[selectedItemIndex].breathAdjustment);
                GUILayout.EndHorizontal();
                break;
			}
		}

		void DrawSprites() {
			if (!Ensure ()) return;
			if (selectedItemIndex == -1) return;

			GUILayout.Label ("Sprites", EditorStyles.centeredGreyMiniLabel);
			GUILayout.Space (StandardVerticalSpacing);

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Icon");
			GUILayout.FlexibleSpace ();
			GUILayout.Label ("In Game");
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();

			Sprite icon = (Sprite) EditorGUILayout.ObjectField(itemData [selectedItemIndex].Icon, typeof (Sprite), false, GUILayout.Width(96), GUILayout.Height(96));
			if (icon != itemData [selectedItemIndex].Icon)
			{
				itemData [selectedItemIndex].icon = icon;
				if (itemData [selectedItemIndex].icon != null)
				{
					itemData [selectedItemIndex].iconSpriteName = SpriteDictionary.AddSprite (itemData [selectedItemIndex].icon);
				} 
				else
				{
					itemData [selectedItemIndex].iconSpriteName = null;
				}
			}
			GUILayout.FlexibleSpace ();

			Sprite inGameSprite = (Sprite) EditorGUILayout.ObjectField(itemData [selectedItemIndex].InGameSprite, typeof (Sprite), false, GUILayout.Width(96), GUILayout.Height(96));
			if (inGameSprite != itemData [selectedItemIndex].InGameSprite)
			{
				itemData [selectedItemIndex].inGameSprite = inGameSprite;
				if (itemData [selectedItemIndex].inGameSprite != null)
				{
					itemData [selectedItemIndex].inGameSpriteName = SpriteDictionary.AddSprite (itemData [selectedItemIndex].inGameSprite);
				} 
				else
				{
					itemData [selectedItemIndex].inGameSpriteName = null;
				}
			}

			GUILayout.EndHorizontal ();
		}

		void DrawItemDetailPanel() {
			if (!Ensure ()) return;
			if (selectedItemIndex == -1) return;
			int itemToRemove = -1;

			GUILayout.Label ("Custom Properties", EditorStyles.centeredGreyMiniLabel);
			GUILayout.Space (StandardVerticalSpacing);
			List<CustomItemProperty> props = itemData [selectedItemIndex].defaultProperties;
			if (props == null) {
				props = new List<CustomItemProperty> ();
				itemData [selectedItemIndex].defaultProperties = props;
			}

			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			if (GUILayout.Button ("Add New", EditorStyles.miniButton)) {;
				props.Add (new CustomItemProperty("New Property"));
			}
			GUILayout.EndHorizontal ();
			GUILayout.Space (StandardVerticalSpacing);

			// Headers

			GUI.enabled = false;
			GUILayout.BeginHorizontal ();
			EditorGUILayout.TextField ("Name", EditorStyles.centeredGreyMiniLabel, GUILayout.Width(labelWidth));
			GUILayout.FlexibleSpace ();
			EditorGUILayout.TextField ("String Value", EditorStyles.centeredGreyMiniLabel);
			EditorGUILayout.TextField ("Int Value", EditorStyles.centeredGreyMiniLabel);
			EditorGUILayout.TextField ("Float Value", EditorStyles.centeredGreyMiniLabel);
			EditorGUILayout.TextField ("Object Value",EditorStyles.centeredGreyMiniLabel);
			GUILayout.Button ("X", EditorStyles.miniButton);
			GUILayout.EndHorizontal ();
			GUI.enabled = true;

			for (int i = 0; i < props.Count; i++)
			{
				GUILayout.BeginHorizontal ();
				props[i].name = EditorGUILayout.TextField (props[i].name, EditorStyles.label, GUILayout.Width(labelWidth));
				GUILayout.FlexibleSpace ();
				props [i].stringValue = EditorGUILayout.TextField (props [i].stringValue);
				props [i].intValue = EditorGUILayout.IntField (props [i].intValue);
				props [i].floatValue = EditorGUILayout.FloatField (props [i].floatValue);
				props [i].objectValue = EditorGUILayout.ObjectField (props [i].objectValue, typeof (Object), false);

				GUI.color = Color.red;
				GUI.backgroundColor = Color.white;
				if (GUILayout.Button ("X", EditorStyles.miniButton)) {
					itemToRemove = i;
				}
				GUI.color = Color.white;
				GUI.backgroundColor = new Color (0.65f, 0.85f, 1.0f);
				GUILayout.EndHorizontal ();
			}
			if (itemToRemove >= 0 && itemToRemove < props.Count)
			{
				props.RemoveAt (itemToRemove);
			}

			GUILayout.Space (StandardVerticalSpacing);
		}

		void DrawItemActions() {
			GUILayout.Label ("All Items", EditorStyles.centeredGreyMiniLabel);
			GUILayout.Space (StandardVerticalSpacing);
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Load", EditorStyles.miniButton)) {
				LoadFrom ();
			}
			GUILayout.FlexibleSpace ();
			if (selectedItemIndex == -1) GUI.enabled = false;
			if (GUILayout.Button ("Clone", EditorStyles.miniButton)) {
				CloneSelectedItem ();
				selectedItemIndex = itemData.Count - 1; 
			}
			GUI.enabled = true;
			if (GUILayout.Button ("Add New", EditorStyles.miniButton)) {
				CreateNewItem ();
				selectedItemIndex = itemData.Count - 1; 
			}
			GUILayout.EndHorizontal ();
		}

		void DrawItemScrollView() {
			int newSelection = selectedItemIndex;
			int itemToRemove = -1;
			itemSelectScrollPosition = GUILayout.BeginScrollView (itemSelectScrollPosition, EditorStyles.helpBox);
			for (int i = 0; i < itemData.Count; i++) {
				GUILayout.BeginHorizontal ();
				if (i == selectedItemIndex)
				{
					GUI.color = new Color (0.65f, 0.85f, 1.0f);
					if (GUILayout.Button (itemData [i].typeId, EditorStyles.label, GUILayout.MinWidth(columnWidth - 64))) {
						newSelection = -1;
					}
					GUI.color = Color.white;
				} 
				else
				{
					if (GUILayout.Button (itemData [i].typeId, EditorStyles.label,  GUILayout.MinWidth(columnWidth - 64))) {
						newSelection = i;
					}
				}
				GUILayout.FlexibleSpace ();
				GUI.color = Color.red;
				if (GUILayout.Button ("X", EditorStyles.miniButton)) {
					itemToRemove = i;
				}
				GUI.color = Color.white;
				GUILayout.EndHorizontal ();
			}
			GUILayout.EndScrollView ();
			selectedItemIndex = newSelection;
			if (itemToRemove >= 0 && itemToRemove < itemData.Count)
			{
				itemData.RemoveAt (itemToRemove);
				if (selectedItemIndex == itemToRemove)
				{
					selectedItemIndex = -1;
				} else if (selectedItemIndex > itemToRemove)
				{
					selectedItemIndex--;
				}
			}
		}

		void CreateNewItem() {
			ItemTypeData item = new ItemTypeData ();
			item.typeId = "NEW_ITEM";
			item.humanReadableName = "New Item";
			item.itemClass = ItemClass.NORMAL;
			itemData.Add (item);
		}
	
		void CloneSelectedItem() 
		{
			if (selectedItemIndex == -1) return;
			ItemTypeData item = new ItemTypeData (itemData [selectedItemIndex]);
			item.typeId = item.typeId + " (Clone)";
			itemData.Add (item);
		}

		string GetRelativePath(string from, string to)
		{
			System.Uri pathUri = new System.Uri("file://" + to);
			System.Uri folderUri = new System.Uri("file://" + from);
			return folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar).ToString();
		}
	}
}