#if UNITY_EDITOR
using UnityEditor;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlatformerPro
{
	/// <summary>
	/// Inspector for enemies.
	/// </summary>
	[CustomEditor(typeof(RotatingEnemy), false)]
	public class RotatingEnemyInspector : EnemyInspector
	{
		/// <summary>
		/// The scene view inspector identifier.
		/// </summary>
		new protected static int sceneViewInspectorId = -1;

		/// <summary>
		/// Draw the scene GUI (i.e. draw collider editors if they are active)
		/// </summary>
		void OnSceneGUI()
		{
			DoSceneGui ();
		}

		/// <summary>
		/// Draw the inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			sceneViewInspectorId = this.GetInstanceID ();
			
			EditorGUILayout.HelpBox("The base enemy class, can be used for all kinds of enemies from simple to complex.", MessageType.Info, true);
			GUILayout.Space (10);
			DrawDefaultInspector();
			DrawFallInspector ((Enemy) target);
			ShowWarnings();
			DrawEnemyDebugger ((Enemy) target);
			
		}

		/// <summary>
		/// Does the scene GUI.
		/// </summary>
		override protected void DoSceneGui()
		{
			if (sceneViewInspectorId != this.GetInstanceID ()) 
			{
				// For some reason Unity creates multiple isntances and uses one to draw the scene view and another to draw the inspector
				// This is a workaround for that
				RotatingEnemyInspector c = (RotatingEnemyInspector) EditorUtility.InstanceIDToObject(sceneViewInspectorId);
				if (c != null) 
				{
					c.OnSceneGUI();
					return;
				}
			}
			
			if (editFeet)
			{
				ShowEditFeet((Enemy) target);
			}
			else if (editSides)
			{
				ShowEditSides((Enemy) target);
			}
		}

	}
}

#endif 