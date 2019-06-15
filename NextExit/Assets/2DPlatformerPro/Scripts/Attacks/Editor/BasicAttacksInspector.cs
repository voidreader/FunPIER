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
	[CustomEditor (typeof(BasicAttacks), true)]
	public class BasicAttacksInspector : Editor
	{
		bool showBlockingDetails;

		/// <summary>
		/// Draw the inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			// Maintain movement control?
			bool maintainControl = EditorGUILayout.Toggle(new GUIContent("Override Movement", "If true the attack system will take complete control of movement."), ((BasicAttacks)target).attackSystemWantsMovementControl);
			if (maintainControl != ((BasicAttacks)target).attackSystemWantsMovementControl)
			{
				((BasicAttacks)target).attackSystemWantsMovementControl = maintainControl;
				EditorUtility.SetDirty(target);
			}
			if (!maintainControl)
			{
				// Force animation
				bool overrideState = EditorGUILayout.Toggle(new GUIContent("Set Animation State", "If true the attack system will set the animation state, if false it only sets an animation override."), ((BasicAttacks)target).attackSystemWantsAnimationStateOverride);
				if (overrideState != ((BasicAttacks)target).attackSystemWantsAnimationStateOverride)
				{
					((BasicAttacks)target).attackSystemWantsAnimationStateOverride = overrideState;
					EditorUtility.SetDirty(target);
				}
			}

			// Draw each attack
			if (((BasicAttacks)target).attacks == null)  ((BasicAttacks)target).attacks = new List<BasicAttackData> ();
			List<BasicAttackData> attackList =((BasicAttacks)target).attacks.ToList();
			foreach (BasicAttackData data in attackList)
			{
				DrawBasicAttackEditor(data);
			}

			// Add new button
			if (((BasicAttacks)target).CanHaveMultipleAttacks) {
				if (GUILayout.Button ("Add Attack")) {
					BasicAttackData attack = new BasicAttackData ();
					((BasicAttacks)target).attacks.Add (attack);
					EditorUtility.SetDirty (target);
				}
			}
		}

		/// <summary>
		/// Draws the inspector for a basic attack.
		/// </summary>
		/// <param name="attack">Attack.</param>
		virtual protected void DrawBasicAttackEditor(BasicAttackData attack)
		{

			string name = EditorGUILayout.TextField(new GUIContent("Name", "Human readable name (optional)."), attack.name);
			if (name != attack.name)
			{
				attack.name = name;
				EditorUtility.SetDirty(target);
			}

			if (((BasicAttacks)target).CanUserSetAttackType) {
				AttackType type = (AttackType)EditorGUILayout.EnumPopup (new GUIContent ("Attack Type", "Attack which this combo triggers from. Empty or null means trigger from ANY other attack."), attack.attackType);
				if (type != attack.attackType) {
					attack.attackType = type;
					EditorUtility.SetDirty (target);
				}
			}

			AnimationState state = (AnimationState)EditorGUILayout.EnumPopup(new GUIContent("Animation", "The animation state to set for this attack."), attack.animation);
			if (state != attack.animation)
			{
				attack.animation = state;
				EditorUtility.SetDirty(target);
			}
			AttackLocation location = (AttackLocation)EditorGUILayout.EnumPopup(new GUIContent("Location", "Where does the character need to be to trigger this attack."), attack.attackLocation);
			if (location != attack.attackLocation)
			{
				attack.attackLocation = location;
				EditorUtility.SetDirty(target);
			}
			int buttonIndex = EditorGUILayout.IntField(new GUIContent("Action Button", "The index of the action button that needs to be pressed."), attack.actionButtonIndex);
			if (buttonIndex != attack.actionButtonIndex && buttonIndex >= 0f)
			{
				attack.actionButtonIndex = buttonIndex;
				EditorUtility.SetDirty(target);
			}
			float length = EditorGUILayout.FloatField(new GUIContent("Attack Length", "The animation length in seconds."), attack.attackTime);
			if (length != attack.attackTime && length > 0.0f)
			{
				attack.attackTime = length;
				EditorUtility.SetDirty(target);
			}
			float coolDown = EditorGUILayout.FloatField(new GUIContent("Attack Cool Down", "The time after the attack where it cannot be triggered again."), attack.coolDown);
			if (coolDown != attack.coolDown)
			{
				if (coolDown < 0) coolDown = 0;
				attack.coolDown = coolDown;
				EditorUtility.SetDirty(target);
			}
			if (attack.attackType != AttackType.PROJECTILE)
			{
				EditorGUILayout.HelpBox("Melee attacks are not supported in the Lite Edition", MessageType.Warning);
			}
			// Damage Info
			DamageType damageType = (DamageType) EditorGUILayout.EnumPopup(new GUIContent("Damage Type", "The type of damage this attack deals"), attack.damageType);
			if (damageType != attack.damageType)
			{
				attack.damageType = damageType;
				EditorUtility.SetDirty(target);
			}
			int damageAmount = EditorGUILayout.IntField(new GUIContent("Damage Amount", "Amount of damage caused by this attack."), attack.damageAmount);
			if (damageAmount > 0 && damageAmount != attack.damageAmount)
			{
				attack.damageAmount = damageAmount;
				EditorUtility.SetDirty(target);
			}
			if (attack.attackType != AttackType.PROJECTILE)
			{
				EditorGUILayout.HelpBox("Melee attacks are not supported in the Lite Edition", MessageType.Warning);
			}
			else
			{
				// Projectile prefab
				GameObject prefab = (GameObject) EditorGUILayout.ObjectField(new GUIContent("Projectile Prefab", "Prefab to use for the projectile. Created on fire."),
				                                                             attack.projectilePrefab, typeof(GameObject), false);
				if (prefab != attack.projectilePrefab)
				{
					attack.projectilePrefab = prefab;
					EditorUtility.SetDirty(target);
				}
				// Ammo item
				string ammoType = EditorGUILayout.TextField(new GUIContent("Ammo Type", "Item type to use for ammo. Empty means infinite ammo."), attack.ammoType);
				if (ammoType != attack.ammoType)
				{
					attack.ammoType = ammoType;
					EditorUtility.SetDirty(target);
				}
				// Projectile delay
				float projectileDelay = EditorGUILayout.FloatField(new GUIContent("Projectile Delay", "Delay in seconds (from attack start) before the projectile is instantiated."), attack.projectileDelay);
				if (projectileDelay > attack.attackTime) projectileDelay = attack.attackTime;
				if (projectileDelay != attack.projectileDelay)
				{
					attack.projectileDelay = projectileDelay;
					EditorUtility.SetDirty(target);
				}
			}
			if (!((BasicAttacks)target).attackSystemWantsMovementControl)
			{
				showBlockingDetails = EditorGUILayout.Foldout(showBlockingDetails, "Movement Blocking");
				if (showBlockingDetails)
				{
					// Block Jump
					bool blockJump = EditorGUILayout.Toggle(new GUIContent("Block Jump", "If true the attack system will not allow jumping whilst this attack is playing."), attack.blockJump);
					if (blockJump != attack.blockJump)
					{
						attack.blockJump = blockJump;
						EditorUtility.SetDirty(target);
					}
					// Block Wall
					bool blockWall = EditorGUILayout.Toggle(new GUIContent("Block Wall Cling", "If true the attack system will not allow wall cling or slide whilst this attack is playing."), attack.blockWall);
					if (blockWall != attack.blockWall)
					{
						attack.blockWall = blockWall;
						EditorUtility.SetDirty(target);
					}
					// Block Climb
					bool blockClimb = EditorGUILayout.Toggle(new GUIContent("Block Climb", "If true the attack system will not allow ladder or rope climbing whilst this attack is playing."), attack.blockClimb);
					if (blockClimb != attack.blockClimb)
					{
						attack.blockClimb = blockClimb;
						EditorUtility.SetDirty(target);
					}
					// Block Special
					bool blockSpecial = EditorGUILayout.Toggle(new GUIContent("Block Special", "If true the attack system will not allow special movements whilst this attack is playing."), attack.blockSpecial);
					if (blockClimb != attack.blockSpecial)
					{
						attack.blockSpecial = blockSpecial;
						EditorUtility.SetDirty(target);
					}
				}
			}
			else
			{
				// Apply gravity
				bool applyGravity = EditorGUILayout.Toggle(new GUIContent("Apply Gravity", "Should we apply gravity while this attack plays?"), attack.applyGravity);
				if (applyGravity != attack.applyGravity)
				{
					attack.applyGravity = applyGravity;
					EditorUtility.SetDirty(target);
				}
			}

			// Set X to Zero
			bool resetVelocityX = EditorGUILayout.Toggle(new GUIContent("Reset X Velocity", "If true the attack system will set X velocity to zero."), attack.resetVelocityX);
			if (resetVelocityX != attack.resetVelocityX)
			{
				attack.resetVelocityX = resetVelocityX;
				EditorUtility.SetDirty(target);
			}

			// Set Y to Zero
			bool resetVelocityY = EditorGUILayout.Toggle(new GUIContent("Reset Y Velocity", "If true the attack system will set Y velocity to zero."), attack.resetVelocityY);
			if (resetVelocityY != attack.resetVelocityY)
			{
				attack.resetVelocityY = resetVelocityY;
				EditorUtility.SetDirty(target);
			}

			// Remove button
			if (((BasicAttacks)target).CanHaveMultipleAttacks)
			{
					EditorGUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if (GUILayout.Button ("Remove", GUILayout.MinWidth(100)))
					{
						((BasicAttacks)target).attacks.Remove (attack);
						EditorUtility.SetDirty(target);
					}
					EditorGUILayout.EndHorizontal();
			}

			// Draw a line
			GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
		}
	}

}