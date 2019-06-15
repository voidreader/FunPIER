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
	/// Editor for basic attacks.
	/// </summary>
	[CustomEditor (typeof(ComboAttacks), false)]
	public class ComboAttackInspector : BasicAttacksInspector
	{
		/// <summary>
		/// Draw the inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			// Unity says we don't need to do this, but if we don't do this then the automatic serialised object updates get the same name as the last object recorded
			Undo.FlushUndoRecordObjects ();
			Undo.RecordObject (target, "Combo Attack Update");
			// Maintain movement control?
			bool maintainControl = EditorGUILayout.Toggle(new GUIContent("Override Movement", "If true the attack system will take complete control of movement."), ((BasicAttacks)target).attackSystemWantsMovementControl);
			if (maintainControl != ((BasicAttacks)target).attackSystemWantsMovementControl)
			{
				((BasicAttacks)target).attackSystemWantsMovementControl = maintainControl;
			}
			if (!maintainControl)
			{
				// Force animation
				bool overrideState = EditorGUILayout.Toggle(new GUIContent("Set Animation State", "If true the attack system will set the animation state, if false it only sets an animation override."), ((BasicAttacks)target).attackSystemWantsAnimationStateOverride);
				if (overrideState != ((BasicAttacks)target).attackSystemWantsAnimationStateOverride)
				{
					((BasicAttacks)target).attackSystemWantsAnimationStateOverride = overrideState;
				}
			}
			
			// Draw each attack
			if (((ComboAttacks)target).comboAttacks == null)  ((ComboAttacks)target).comboAttacks = new List<ComboAttackData> ();
			List<ComboAttackData> attackList = ((ComboAttacks)target).comboAttacks.ToList();
			foreach (ComboAttackData data in attackList)
			{
				DrawComboAttackEditor(data);
			}
			
			// Add new button
			if (GUILayout.Button("Add Attack"))
			{
				ComboAttackData attack = new ComboAttackData();
				((ComboAttacks)target).comboAttacks.Add(attack);
			}
		}
		
		/// <summary>
		/// Draws the inspector for a combo attack.
		/// </summary>
		/// <param name="attack">Attack.</param>
		virtual protected void DrawComboAttackEditor(ComboAttackData attack)
		{

			string name = EditorGUILayout.TextField(new GUIContent("Name", "Human readable name (optional)."), attack.name);
			if (name != attack.name)
			{
				attack.name = name;
			}

			EditorGUILayout.LabelField ("Combo Data", EditorStyles.boldLabel);
		
			// Type
			ComboType comboType = (ComboType) EditorGUILayout.EnumPopup(new GUIContent("Combo Type ", "What type of combo move is this."), attack.comboType);
			if (comboType != attack.comboType)
			{
				attack.comboType = comboType;
			}
			EditorGUILayout.HelpBox (comboType.GetDescription(), MessageType.Info);


			// Inital Attack
			string initialAttack = EditorGUILayout.TextField(new GUIContent("Initial Attack", "Human readable name (optional)."), attack.initialAttack);
			if (initialAttack != attack.initialAttack)
			{
				attack.initialAttack = initialAttack;
			}

			// Window
			float min = attack.minWindowTime;
			float max = attack.maxWindowTime;
			EditorGUILayout.MinMaxSlider(new GUIContent("Trigger Window", "The window within the initial attack where the combos action button must be pressed (normalised time)."), ref min, ref max, 0, 1);
			if (min > 0.9f) min = 0.9f;
			if (max <= min + 0.1f) max = min + 0.1f;
			if (min != attack.minWindowTime || max != attack.maxWindowTime)
			{
				attack.minWindowTime = min;
				attack.maxWindowTime = max;
			}

			// Combo count
			EditorGUILayout.LabelField ("Attack Data", EditorStyles.boldLabel);
			DrawBasicAttackEditor(attack);
		}
	}
}
