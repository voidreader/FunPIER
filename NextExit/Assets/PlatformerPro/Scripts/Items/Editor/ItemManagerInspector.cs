using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PlatformerPro
{
	/// <summary>
	/// Custom inspector for item manager.
	/// </summary>
	[CustomEditor (typeof (ItemManager))]
	public class ItemManagerInspector : PersistableInspector 
	{
		/// <summary>
		/// Draw the inspector.
		/// </summary>
		override protected void DrawFooter(PlatformerProMonoBehaviour myTarget)
		{
			if (GUILayout.Button("Edit Item Types")) {
				ItemTypeEditor.ShowMainWindow ();
			}
			base.DrawFooter (myTarget);
		}
	}	
}
