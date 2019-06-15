#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace PlatformerPro
{

	[CustomEditor(typeof(Platform), true)]
	[CanEditMultipleObjects]
	public class PlatformInspector : PersistableObjectInspector
	{
		
		protected SerializedProperty automaticActivation;
		protected SerializedProperty automaticDeactivation;
		protected SerializedProperty friction;
		protected SerializedProperty conditionsEnableCollisions;
		protected SerializedProperty useDefaultPersistence;

		override protected void OnEnable()
		{
			base.OnEnable ();
			automaticActivation = serializedObject.FindProperty("automaticActivation");
			automaticDeactivation = serializedObject.FindProperty("automaticDeactivation");
			friction = serializedObject.FindProperty("friction");
			conditionsEnableCollisions = serializedObject.FindProperty("conditionsEnableCollisions");
			useDefaultPersistence = serializedObject.FindProperty("useDefaultPersistence");
		}

		/// <summary>
		/// Draw the inspector GUI.
		/// </summary>
		override protected void DrawInspector()
		{
			DrawDefaultInspector ();

			serializedObject.Update();

			EditorGUILayout.PropertyField (friction);

			if (((Platform)target).ForcedActivation == PlatformActivationType.NONE)
			{
				EditorGUILayout.PropertyField (automaticActivation);
			}
			if (((Platform)target).ForcedDeactivation == PlatformDeactivationType.NONE)
			{
				EditorGUILayout.PropertyField (automaticDeactivation);
			}

			EditorGUILayout.PropertyField (conditionsEnableCollisions);

			// Persistence
			PersistableObject.UpdateGuid ((PersistableObject)target);
			EditorGUILayout.PropertyField (useDefaultPersistence);
			serializedObject.ApplyModifiedProperties ();
			if (!useDefaultPersistence.boolValue)
			{
				base.DrawInspector ();
			} 
			serializedObject.ApplyModifiedProperties ();

		}
	}
}
#endif