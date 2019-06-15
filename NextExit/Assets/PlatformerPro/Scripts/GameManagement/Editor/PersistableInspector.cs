#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlatformerPro
{
	/// <summary>
	/// Inspector for persistable classes.
	/// </summary>
	[CustomEditor(typeof(Persistable), true)]
	[CanEditMultipleObjects]
	public class PersistableInspector : PlatformerProMonoBehaviourInspector
	{
		
		SerializedProperty usePersistenceDefaults;
		SerializedProperty enablePersistence;
		SerializedProperty saveOnAnyChange;
		SerializedProperty persistenceType;

		override protected void OnEnable()
		{
			usePersistenceDefaults = serializedObject.FindProperty("usePersistenceDefaults");
			enablePersistence = serializedObject.FindProperty("enablePersistence");
			saveOnAnyChange = serializedObject.FindProperty("saveOnAnyChange");
			persistenceType = serializedObject.FindProperty("persistenceType");
		}

		/// <summary>
		/// Draws the footer.
		/// </summary>
		override protected void DrawFooter(PlatformerProMonoBehaviour myTarget)
		{
			Persistable myPersistable = (Persistable)myTarget;
			EditorGUILayout.PropertyField (usePersistenceDefaults);
			if (!myPersistable.usePersistenceDefaults)
			{
				EditorGUILayout.PropertyField (enablePersistence);
				if (enablePersistence.boolValue)
				{
					EditorGUILayout.PropertyField (saveOnAnyChange);
					EditorGUILayout.PropertyField (persistenceType);
					EditorGUILayout.HelpBox (myPersistable.persistenceType.GetDescription (), MessageType.Info);
				}
			}
			serializedObject.ApplyModifiedProperties();
			if (myPersistable.EnablePersistence)
			{
				DrawPersistenceButtons (myPersistable);
			}
			serializedObject.ApplyModifiedProperties();
		}


		/// <summary>
		/// Draws the persistence fold out.
		/// </summary>
		virtual protected void DrawPersistenceButtons(Persistable myPersistable)
		{
			EditorGUILayout.BeginHorizontal ();
			if (myPersistable.ShouldShowSavedData)
			{
				if (GUILayout.Button ("Hide Data"))
				{
					myPersistable.ShouldShowSavedData = false;
				}
			} else
			{
				if (GUILayout.Button ("Show Data"))
				{
					myPersistable.ShouldShowSavedData = true;
				}
			}
			if (GUILayout.Button ("Reset Data"))
			{
				myPersistable.ResetSaveData ();
			}
			EditorGUILayout.EndHorizontal ();

			if (myPersistable.ShouldShowSavedData)
			{
				myPersistable.ShowSaveData ();
			}
		}

	}
}
