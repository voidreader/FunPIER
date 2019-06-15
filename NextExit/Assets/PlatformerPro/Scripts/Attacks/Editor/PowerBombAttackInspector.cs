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
	/// Editor for power bomb attacks.
	/// </summary>
	[CustomEditor (typeof(PowerBombAttack), false)]
	public class PowerBombAttackInspector : BasicAttacksInspector
	{
		/// <summary>
		/// Draw the inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			EditorGUILayout.HelpBox ("Note that some of the attack settings such as the Attack Length and HitBox window are ignored for this attack.", MessageType.Info);
			// Unity says we don't need to do this, but if we don't do this then the automatic serialised object updates get the same name as the last object recorded
			Undo.FlushUndoRecordObjects ();
			Undo.RecordObject (target, "Power Bomb Update");
			// Always maintain control with a power bomb
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
				((BasicAttacks)target).attacks[0].name = "PowerBomb";
			}
			
			DrawBasicAttackEditor(((BasicAttacks)target).attacks[0]);
			
			float pauseTime = EditorGUILayout.FloatField(new GUIContent("Pause Time", "How long to pause for."), ((PowerBombAttack)target).pauseTime);
			if (pauseTime != ((PowerBombAttack)target).pauseTime && pauseTime > 0.0f)
			{
				((PowerBombAttack)target).pauseTime = pauseTime;
			}

			float dropGravity = EditorGUILayout.FloatField(new GUIContent("Drop Gravity", "How much gravity to apply during the drop."), ((PowerBombAttack)target).dropGravity);
			if (dropGravity != ((PowerBombAttack)target).dropGravity && dropGravity < 0.0f)
			{
				((PowerBombAttack)target).dropGravity = dropGravity;
			}

			float landingTime = EditorGUILayout.FloatField(new GUIContent("Landing Time", "How long do we stay in the landing state."), ((PowerBombAttack)target).landingTime);
			if (dropGravity != ((PowerBombAttack)target).landingTime && landingTime > 0.0f)
			{
				((PowerBombAttack)target).landingTime = landingTime;
			}

			float minVelocity = EditorGUILayout.FloatField(new GUIContent("Minimum Velocity", "You must be going faster than this to trigger the attack. Use this to stop power bomb when falling."), ((PowerBombAttack)target).minVelocity);
			if (minVelocity != ((PowerBombAttack)target).minVelocity)
			{
				((PowerBombAttack)target).minVelocity = minVelocity;
			}

			float maxVelocity = EditorGUILayout.FloatField(new GUIContent("Maximum Velocity", "You must be going slower than this to trigger the attack. Use this to ensure you are near the peak of your jump when power bombing."), ((PowerBombAttack)target).maxVelocity);
			if (maxVelocity != ((PowerBombAttack)target).maxVelocity )
			{
				((PowerBombAttack)target).maxVelocity = maxVelocity;
			}

			bool requireDownKey = EditorGUILayout.Toggle(new GUIContent("Require Down Key", "Require user to press the down key as well as the action key to trigger power bomb."), ((PowerBombAttack)target).requireDownKey);
			if (requireDownKey != ((PowerBombAttack)target).requireDownKey)
			{
				((PowerBombAttack)target).requireDownKey = requireDownKey;
			}

			CharacterHitBox hitBox = (CharacterHitBox) EditorGUILayout.ObjectField (new GUIContent ("Landing Hit Box", "This hit box is enabled after landing. Use it to cause damage to nearby enemies when you land"),  ((PowerBombAttack)target).landingDamageHitHox, typeof(CharacterHitBox), true);
			if (hitBox != ((PowerBombAttack)target).landingDamageHitHox)
			{
				((PowerBombAttack)target).landingDamageHitHox = hitBox;
			}
		}
	}
}