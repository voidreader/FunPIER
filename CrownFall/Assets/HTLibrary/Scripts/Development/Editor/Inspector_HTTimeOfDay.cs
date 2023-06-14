using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	[CustomEditor(typeof(HTTimeOfDay))]
	public class Inspector_HTTimeOfDay : Editor
	{
		//---------------------------------------
		private GUIStyle _headerGUIStyle = null;

		//---------------------------------------
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			HTTimeOfDay pTarget = target as HTTimeOfDay;
			EditorGUILayout.BeginVertical();

			//-----
			if (_headerGUIStyle == null)
			{
				_headerGUIStyle = new GUIStyle();
				_headerGUIStyle.fontStyle = FontStyle.Bold;
			}
			GUILayout.Space(20);
			GUILayout.Label("TIME OF DAY TEST", _headerGUIStyle);

			//-----
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Current Time");
			float fCurTime = EditorGUILayout.Slider(pTarget.Tod_CurrentTime, 0.0f, pTarget.Tod_LengthOfDay);
			pTarget.Tod_CurrentTime = fCurTime;

			EditorGUILayout.EndHorizontal();

			//-----
			EditorGUILayout.EndVertical();
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}
