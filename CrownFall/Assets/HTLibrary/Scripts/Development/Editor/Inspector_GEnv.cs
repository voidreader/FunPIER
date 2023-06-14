using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	[CustomEditor(typeof(GEnv))]
	public class Inspector_GEnv : Editor
	{
		//---------------------------------------
		private GUIStyle _headerGUIStyle = null;

		//---------------------------------------
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			GEnv pTarget = target as GEnv;
			EditorGUILayout.BeginVertical();

			//-----
			if (_headerGUIStyle == null)
			{
				_headerGUIStyle = new GUIStyle();
				_headerGUIStyle.fontStyle = FontStyle.Bold;
			}
			GUILayout.Space(20);
			GUILayout.Label("PROTECTION CLIENT - FEATURES", _headerGUIStyle);

			//-----
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("REFRESH ALL MD5"))
				pTarget.DoRefreshAllMD5Keys();

			EditorGUILayout.EndHorizontal();

			//-----
			EditorGUILayout.EndVertical();
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}
