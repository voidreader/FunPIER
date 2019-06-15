using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PlatformerPro
{
	/// <summary>
	/// Item type manager inspector.
	/// </summary>
	[CustomEditor (typeof (ItemTypeManager))]
	public class ItemTypeManagerInspector : Editor {

		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Edit Item Types")) {
				ItemTypeEditor.ShowMainWindow ();
			}
			DrawDefaultInspector ();

		}
	}
}