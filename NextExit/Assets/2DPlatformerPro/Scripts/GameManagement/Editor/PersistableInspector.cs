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
	/// Inspector for characters classes.
	/// </summary>
	[CustomEditor(typeof(Persistable), true)]
	public class PersistableInspector : Editor
	{
		/// <summary>
		/// Should we show the persistable foldout.
		/// </summary>
		protected bool showFoldout;

		/// <summary>
		/// Unity hook.
		/// </summary>
		public override void OnInspectorGUI()
		{
			DrawInspector ();
		}

		/// <summary>
		/// Draws the inspector.
		/// </summary>
		virtual protected void DrawInspector()
		{
			DrawDefaultInspector ();
			showFoldout = EditorGUILayout.Foldout (showFoldout, "Persistence");
			if (showFoldout) DrawPersistenceFoldOut ();
		}

		/// <summary>
		/// Draws the persistence fold out.
		/// </summary>
		virtual protected void DrawPersistenceFoldOut()
		{
			Persistable myTarget = (Persistable)target;

// TODO Add more options to allow users to edit these
			EditorGUILayout.BeginHorizontal();
//			GUILayout.Button ("Edit");
			if (GUILayout.Button ("Reset"))
			{
				myTarget.ResetSaveData();
			}
//			GUILayout.Button ("Load...");
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.LabelField ("Loading", EditorStyles.boldLabel);
			UpdateBool (ref myTarget.loadOnAwake, "Load on Awake");
			UpdateBool (ref myTarget.loadOnStart, "Load on Start");
			UpdateBool (ref myTarget.loadOnCharacterLoad, "Load on Character Load");
			EditorGUILayout.LabelField ("Saving", EditorStyles.boldLabel);
			UpdateBool (ref myTarget.saveOnDeath, "Save on Death");
			UpdateBool (ref myTarget.saveOnGameOver, "Save on Game Over");
			UpdateBool (ref myTarget.saveOnSceneExit, "Save on Scene Exit");
			UpdateBool (ref myTarget.saveOnChange, "Save on Change");
			EditorGUILayout.LabelField ("Resetting", EditorStyles.boldLabel);
			UpdateBool (ref myTarget.resetOnDeath, "Reset on Death");
			UpdateBool (ref myTarget.resetOnGameOver, "Reset on Game Over");
			UpdateBool (ref myTarget.resetOnSceneExit, "Reset on Scene Exit");

		}

		/// <summary>
		/// Draw button for update a bool and set dirty on change.
		/// </summary>
		/// <param name="targetBool">Target bool.</param>
		/// <param name="title">Title.</param>
		virtual protected void UpdateBool(ref bool targetBool, string title)
		{
			bool result = EditorGUILayout.Toggle (title, targetBool);
			if (result != targetBool)
			{
				Undo.RecordObject(target, "Persistence Update");
				targetBool = result;
				EditorUtility.SetDirty (target);
			}
		}

	}
}
