using UnityEditor;
using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Inspector for the standard input.
	/// </summary>
	[CustomEditor(typeof(StandardInput), false)]
	public class StandardInputInspector : Editor
	{

		/// <summary>
		/// Should we show default settings.
		/// </summary>
		protected bool showDefaults;

		/// <summary>
		/// Typed reference to target.
		/// </summary>
		protected StandardInput myTarget;

		#region Unity hooks

		/// <summary>
		/// Unity OnEnable hook.
		/// </summary>
		void OnEnable()
		{
		}

		/// <summary>
		/// Draw the inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			myTarget = (StandardInput) target;

			// Arguments to load
			string newDataToLoad = EditorGUILayout.TextField (new GUIContent ("Data To Load", "Data identifier for the inputs, should match the identifier in the configurable controls UI."), myTarget.dataToLoad);
			if (myTarget.dataToLoad != newDataToLoad)
			{
				myTarget.dataToLoad = newDataToLoad;
				EditorUtility.SetDirty(target);
			}
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Load from File"))
			{
				string path = EditorUtility.OpenFilePanel("Load Input Settings", "", "xml");
				if (path.Length > 0)
				{
					StandardInputData data = StandardInputData.LoadFromFile (path);
					if (data != null)
					{
						myTarget.LoadInputData(data);
						EditorUtility.SetDirty(target);
					}
				}
			}
			if (GUILayout.Button("Save to File"))
			{
				string path = EditorUtility.SaveFilePanel("Save Input Settings", "", "CustomInput", "xml");
				if (path.Length > 0)
				{
					StandardInputData data = myTarget.GetInputData();
					StandardInputData.SaveToFile(path, data);
				}
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Load from Preferencess"))
			{
				myTarget.LoadInputData(myTarget.dataToLoad);
			}
			if (GUILayout.Button("Clear Preferencess"))
			{
				string prefsName = StandardInput.SavedPreferencePrefix + myTarget.dataToLoad;
				PlayerPrefs.DeleteKey (prefsName);
			}
			
			GUILayout.EndHorizontal();

			
			EditorGUILayout.HelpBox("Note that the defaults below are only applied if no PlayerPreference data has been set.", MessageType.Info);

			// Defaults
			showDefaults = EditorGUILayout.Foldout(showDefaults, "Default Controls");
			if(showDefaults)
			{

				// GUILayout.Label("Controls", EditorStyles.boldLabel);
				DrawDefaultInspector ();

			}
		}

		#endregion
	}
}