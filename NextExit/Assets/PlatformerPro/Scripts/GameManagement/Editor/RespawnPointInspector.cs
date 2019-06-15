#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace PlatformerPro
{

	[CustomEditor(typeof(RespawnPoint), true)]
	[CanEditMultipleObjects]
	public class RespawnPointInspector : PersistableObjectInspector
	{
		protected SerializedProperty isDefaultStartingPoint;
		protected SerializedProperty identifier;
		protected SerializedProperty autoProximityTriggerRange;
		protected SerializedProperty autoSwitchSprite;
		protected SerializedProperty poi;
		protected SerializedProperty useDefaultPersistence;

		override protected void OnEnable()
		{
			base.OnEnable ();
			isDefaultStartingPoint = serializedObject.FindProperty("isDefaultStartingPoint");
			identifier = serializedObject.FindProperty("identifier");
			autoProximityTriggerRange = serializedObject.FindProperty("autoProximityTriggerRange");
			autoSwitchSprite = serializedObject.FindProperty("autoSwitchSprite");
			poi = serializedObject.FindProperty("poi");
			useDefaultPersistence = serializedObject.FindProperty("useDefaultPersistence");
		}

		override protected void DrawInspector()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField (isDefaultStartingPoint);
			EditorGUILayout.PropertyField (identifier);
			EditorGUILayout.PropertyField (autoProximityTriggerRange);
			EditorGUILayout.PropertyField (autoSwitchSprite);

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

		}
	}
}
#endif