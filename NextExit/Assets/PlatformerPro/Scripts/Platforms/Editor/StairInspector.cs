using UnityEditor;
using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Stair inspector.
	/// </summary>
	[CustomEditor (typeof(Stairs), true)]
	public class StairInspector : Editor
	{
		override public void OnInspectorGUI()
		{
			DrawDefaultInspector ();
			EditorGUILayout.HelpBox ("Top: " + ((Stairs)target).GetTopMountDescription (), MessageType.Info);
			EditorGUILayout.HelpBox ("Bottom: " + ((Stairs)target).GetBottomMountDescription (), MessageType.Info);
			EditorGUILayout.HelpBox ("Air: " + ((Stairs)target).GetAirMountDescription (), MessageType.Info);
		}
	
	}
}
