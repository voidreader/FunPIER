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
	/// Editor for dash attacks.
	/// </summary>
	[CustomEditor (typeof(DashAttack), false)]
	public class DashAttackInspector : BasicAttacksInspector
	{
		/// <summary>
		/// Draw the inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{	
			// Unity says we don't need to do this, but if we don't do this then the automatic serialised object updates get the same name as the last object recorded
			Undo.FlushUndoRecordObjects ();
			Undo.RecordObject (target, "Dash Attack Update");
			// Always maintain control with a dash
			bool maintainControl = true;
			if (maintainControl != ((BasicAttacks)target).attackSystemWantsMovementControl)
			{
				((BasicAttacks)target).attackSystemWantsMovementControl = maintainControl;
			}

			// Draw one attack
			if (((BasicAttacks)target).attacks == null)
			{ 
				((BasicAttacks)target).attacks = new List<BasicAttackData> ();
				((BasicAttacks)target).attacks.Add(new BasicAttackData());
				((BasicAttacks)target).attacks[0].name = "Dash";
			}

			DrawBasicAttackEditor(((BasicAttacks)target).attacks[0]);

			float speed = EditorGUILayout.FloatField(new GUIContent("Dash Speed", "How fast the dash attack is"), ((DashAttack)target).dashSpeed);
			if (speed != ((DashAttack)target).dashSpeed && speed > 0.0f)
			{
				((DashAttack)target).dashSpeed = speed;
			}
		}
	}
}