#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using PlatformerPro.Validation;

namespace PlatformerPro
{

	[CustomEditor(typeof(Item), true)]
	[CanEditMultipleObjects]
	public class ItemInspector : PersistableObjectInspector
	{
		Character character;

        protected SerializedProperty data;
        protected SerializedProperty itemId;
		protected SerializedProperty amount;
        protected SerializedProperty durability;
        protected SerializedProperty xp;
        protected SerializedProperty useDefaultPersistence;
		protected SerializedProperty poi;

		override protected void OnEnable()
		{
			base.OnEnable ();
            data = serializedObject.FindProperty("instanceData");
            itemId = data.FindPropertyRelative("itemId");
			amount = data.FindPropertyRelative("amount");
            durability = data.FindPropertyRelative("durability");
            xp = data.FindPropertyRelative("xp");
            useDefaultPersistence = serializedObject.FindProperty("useDefaultPersistence");
			poi = serializedObject.FindProperty("poi");
		}

		/// <summary>
		/// Draw the inspector GUI.
		/// </summary>
		override protected void DrawInspector()
		{
			
			serializedObject.Update();

			if (ItemTypeManager.Instance == null)
			{
				EditorGUILayout.HelpBox("You need to add an ItemTypeManager to your scene and add some item data.", MessageType.Warning);
				if (GUILayout.Button("Do It Now")) {
					ItemTypeEditor.ShowMainWindow ();
				}
				return;
			}
			EditorGUILayout.PropertyField (itemId);
			ItemTypeData typeData = ItemTypeManager.Instance.GetTypeData (itemId.stringValue);
			if (typeData == null)
			{
				if (GUILayout.Button("Edit Item Data")) {
					ItemTypeEditor.ShowMainWindow ();
				}
				return;
			}
			if (typeData.itemClass == ItemClass.UNIQUE || typeData.itemClass == ItemClass.INSTANT)
			{
				amount.intValue = 1;
				GUI.enabled = false;
				EditorGUILayout.PropertyField (amount, new GUIContent ("Amount", "Number of the items in this stack."));
				GUI.enabled = true;
			}
			else
			{
                if (amount.intValue == 0) amount.intValue = 1;
				EditorGUILayout.PropertyField (amount, new GUIContent ("Amount", "Number of the items in this stack."));
			}

            if (typeData.itemClass != ItemClass.INSTANT && typeData.maxDurability > 0)
            {
                EditorGUILayout.PropertyField(durability, new GUIContent("Durability", "Current item durability."));
                EditorGUILayout.HelpBox("Max: " + typeData.maxDurability, MessageType.None);
                if (durability.intValue > typeData.maxDurability) durability.intValue = typeData.maxDurability;
            }
            else
            {
                durability.intValue = 0;
            }

            // TODO We could allow users to set their own starting XP
            if (typeData.itemClass != ItemClass.INSTANT)
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(xp, new GUIContent("XP (Read Only)", "Current item XP."));
                GUI.enabled = true;
            }

            // Persistence
            PersistableObject.UpdateGuid ((PersistableObject)target);
			EditorGUILayout.PropertyField (useDefaultPersistence);
			serializedObject.ApplyModifiedProperties ();

			if (!useDefaultPersistence.boolValue)
			{
				base.DrawInspector ();
			} 

			// Map
			EditorGUILayout.PropertyField (poi);

			serializedObject.ApplyModifiedProperties ();

			if (GUILayout.Button("Edit Item Data")) {
				ItemTypeEditor.ShowMainWindow ();
			}

			Validate();

		}

		void Validate()
		{
			Collider2D c = ((Component)target).gameObject.GetComponent<Collider2D>();
			if (c == null) {
				EditorGUILayout.HelpBox("Items should be on the same GameObject as a Collider2D", MessageType.Warning);
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Add BoxCollider2D", EditorStyles.miniButton))
				{
					c = ((Component) target).gameObject.AddComponent<BoxCollider2D>();
					c.isTrigger = true;
				}
				GUILayout.EndHorizontal();
			} 
			else if (!c.isTrigger)
			{
				EditorGUILayout.HelpBox("The Items Collider2D should be a trigger.", MessageType.Warning);
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Fix Now", EditorStyles.miniButton))
				{
					c.isTrigger = true;
				}
				GUILayout.EndHorizontal();
			}
			PlatformerProEditorUtils.ValidateLayer(target, "Collectible_Projectile", 15, false);
		}
	}
}
#endif