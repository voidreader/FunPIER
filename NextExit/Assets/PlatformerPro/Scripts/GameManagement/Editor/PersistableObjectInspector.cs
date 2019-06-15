using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PlatformerPro
{

	[CustomEditor(typeof(PersistableObject), true)]
	[CanEditMultipleObjects]
	public class PersistableObjectInspector : PlatformerProMonoBehaviourInspector
	{
		public static Dictionary <string, PersistableObject> guidRegister = new Dictionary <string, PersistableObject> ();

		protected SerializedProperty enablePersistence;
		protected SerializedProperty guid;
		protected SerializedProperty targetGameObject;
		protected SerializedProperty persistenceImplementation;
		protected SerializedProperty defaultStateIsDisabled;

		override protected void OnEnable()
		{
			base.OnEnable ();
			enablePersistence = serializedObject.FindProperty("enablePersistence");
			targetGameObject = serializedObject.FindProperty("target");
			persistenceImplementation = serializedObject.FindProperty("persistenceImplementation");
			defaultStateIsDisabled = serializedObject.FindProperty("defaultStateIsDisabled");
			guid = serializedObject.FindProperty("guid");
		}

		public override void OnInspectorGUI()
		{
			DrawHeader((PlatformerProMonoBehaviour) target);
			GUILayout.Space (5);
			DrawInspector ();
			GUILayout.Space (5);
			DrawFooter((PlatformerProMonoBehaviour) target);
		}

		virtual protected void DrawInspector()
		{
			serializedObject.Update();
			PersistableObject.UpdateGuid ((PersistableObject)target);
			EditorGUILayout.PropertyField (enablePersistence);
			if (enablePersistence.boolValue)
			{
				EditorGUILayout.PropertyField (persistenceImplementation);
				EditorGUILayout.PropertyField (targetGameObject);
				EditorGUILayout.PropertyField (defaultStateIsDisabled);
				EditorGUILayout.PropertyField (guid);
			}
			serializedObject.ApplyModifiedProperties ();
		}



	}
}
