using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace PlatformerPro
{
	/// <summary>
	/// Draws enums as a mask field.
	/// </summary>
	[CustomPropertyDrawer(typeof(ItemTypeAttribute))]
	public class ItemTypeAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
            property.stringValue = DrawItemTypeSelector(position, property.stringValue, label);
		}

        public static string DrawItemTypeSelector(Rect position, string currentValue, GUIContent label, bool allowNone = false)
        {
            List<string> itemTypes = ItemTypeManager.Instance.ItemTypes;
            if (itemTypes.Count == 0)
            {
                EditorGUILayout.HelpBox("No items types defined", MessageType.Warning);
                return "";
            }
            if (allowNone)
            {
                itemTypes.Insert(0, ItemTypeManager.NONE_TYPE);
            }
            GUIContent[] itemTypesAsGui = itemTypes.Select(i => new GUIContent(i)).ToArray();
            int itemTypeIndex = itemTypes.IndexOf(currentValue);
            if (itemTypeIndex == -1) itemTypeIndex = 0;
            int selectedIndex = EditorGUI.Popup(position, label, itemTypeIndex, itemTypesAsGui);
            return itemTypes[selectedIndex];
        }

        public static string DrawItemTypeSelectorLayout(string currentValue, GUIContent label, bool allowNone = false)
        {
            if (ItemTypeManager.Instance == null) return "";
            List<string> itemTypes = ItemTypeManager.Instance.ItemTypes;
            if (itemTypes.Count == 0)
            {
                EditorGUILayout.HelpBox("No items types defined", MessageType.Warning);
                return "";
            }
            if (allowNone)
            {
                itemTypes.Insert(0, ItemTypeManager.NONE_TYPE);
            }
            GUIContent[] itemTypesAsGui = itemTypes.Select(i => new GUIContent(i)).ToArray();
            int itemTypeIndex = itemTypes.IndexOf(currentValue);
            if (itemTypeIndex == -1) itemTypeIndex = 0;
            int selectedIndex = EditorGUILayout.Popup(label, itemTypeIndex, itemTypesAsGui);
            return itemTypes[selectedIndex];
        }


    }
}