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
	[CustomEditor (typeof(MultiProjectileAttacks), true)]
	public class MultiProjectileAttacksInspector : Editor
	{
		bool showBlockingDetails;

		/// <summary>
		/// Draw the inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			// Unity says we don't need to do this, but if we don't do this then the automatic serialised object updates get the same name as the last object recorded
			Undo.FlushUndoRecordObjects ();
			Undo.RecordObject (target, "MultiProjectile Attack Update");
			// Maintain movement control?
			bool maintainControl = EditorGUILayout.Toggle(new GUIContent("Override Movement", "If true the attack system will take complete control of movement."), ((MultiProjectileAttacks)target).attackSystemWantsMovementControl);
			if (maintainControl != ((MultiProjectileAttacks)target).attackSystemWantsMovementControl)
			{
				((MultiProjectileAttacks)target).attackSystemWantsMovementControl = maintainControl;
			}
			if (!maintainControl)
			{
				// Force animation
				bool overrideState = EditorGUILayout.Toggle(new GUIContent("Set Animation State", "If true the attack system will set the animation state, if false it only sets an animation override."), ((MultiProjectileAttacks)target).attackSystemWantsAnimationStateOverride);
				if (overrideState != ((MultiProjectileAttacks)target).attackSystemWantsAnimationStateOverride)
				{
					((MultiProjectileAttacks)target).attackSystemWantsAnimationStateOverride = overrideState;
				}
			}

			// Draw each attack
			if (((MultiProjectileAttacks)target).projectileData == null)  ((MultiProjectileAttacks)target).projectileData = new List<MultiProjectileAttackData> ();
			List<MultiProjectileAttackData> attackList = null;
			attackList = ((MultiProjectileAttacks)target).projectileData.Cast<MultiProjectileAttackData>().ToList();
			
			foreach (MultiProjectileAttackData data in attackList)
			{
				DrawMultiProjectileAttackEditor(data);
			}

			// Add new button
			if (GUILayout.Button("Add Attack"))
			{
				MultiProjectileAttackData attack = new MultiProjectileAttackData();
				((MultiProjectileAttacks)target).projectileData.Add(attack);
			}
		}

		/// <summary>
		/// Draws the inspector for a basic attack.
		/// </summary>
		/// <param name="attack">Attack.</param>
		virtual protected void DrawMultiProjectileAttackEditor(MultiProjectileAttackData attack)
		{

			// Add new button
			if (GUILayout.Button("Remove Attack"))
			{
				((MultiProjectileAttacks)target).projectileData.Remove(attack);
				return;
			}

			string name = EditorGUILayout.TextField(new GUIContent("Name", "Human readable name (optional)."), attack.name);
			if (name != attack.name)
			{
				attack.name = name;
			}

			GUI.enabled = false;
			EditorGUILayout.EnumPopup(new GUIContent("Attack Type", "Attack which this combo triggers from. Empty or null means trigger from ANY other attack."), AttackType.PROJECTILE);
			GUI.enabled = true;

			// Force projectile
			attack.attackType = AttackType.PROJECTILE;

			AnimationState state = (AnimationState)EditorGUILayout.EnumPopup(new GUIContent("Animation", "The animation state to set for this attack."), attack.animation);
			if (state != attack.animation)
			{
				attack.animation = state;
			}
			AttackLocation location = (AttackLocation)EditorGUILayout.EnumPopup(new GUIContent("Location", "Where does the character need to be to trigger this attack."), attack.attackLocation);
			if (location != attack.attackLocation)
			{
				attack.attackLocation = location;
			}
			GUILayout.Label ("Controls", EditorStyles.boldLabel);
			int buttonIndex = EditorGUILayout.IntField(new GUIContent("Action Button", "The index of the action button that needs to be pressed."), attack.actionButtonIndex);
			if (buttonIndex != attack.actionButtonIndex && buttonIndex >= 0f)
			{
				attack.actionButtonIndex = buttonIndex;
			}

			FireInputType fireInputType = (FireInputType)EditorGUILayout.EnumPopup(new GUIContent("Fire Input Type", "Set how we determine when to fire."), attack.fireInputType);
			if (fireInputType != attack.fireInputType)
			{
				attack.fireInputType = fireInputType;
			}

			GUILayout.Label ("Timing", EditorStyles.boldLabel);
			float length = EditorGUILayout.FloatField(new GUIContent("Attack Length", "The animation length in seconds."), attack.attackTime);
			if (length != attack.attackTime && length > 0.0f)
			{
				attack.attackTime = length;
			}

			GUILayout.Label ("Damage", EditorStyles.boldLabel);
			// Damage Info
			DamageType damageType = (DamageType) EditorGUILayout.EnumPopup(new GUIContent("Damage Type", "The type of damage this attack deals"), attack.damageType);
			if (damageType != attack.damageType)
			{
				attack.damageType = damageType;
			}
			int damageAmount = EditorGUILayout.IntField(new GUIContent("Damage Amount", "Amount of damage caused by this attack."), attack.damageAmount);
			if (damageAmount > 0 && damageAmount != attack.damageAmount)
			{
				attack.damageAmount = damageAmount;
			}

			GUILayout.Label ("Other", EditorStyles.boldLabel);
			if (!((MultiProjectileAttacks)target).attackSystemWantsMovementControl)
			{
				showBlockingDetails = EditorGUILayout.Foldout(showBlockingDetails, "Movement Blocking");
				if (showBlockingDetails)
				{
					// Block Jump
					bool blockJump = EditorGUILayout.Toggle(new GUIContent("Block Jump", "If true the attack system will not allow jumping whilst this attack is playing."), attack.blockJump);
					if (blockJump != attack.blockJump)
					{
						attack.blockJump = blockJump;
					}
					// Block Wall
					bool blockWall = EditorGUILayout.Toggle(new GUIContent("Block Wall Cling", "If true the attack system will not allow wall cling or slide whilst this attack is playing."), attack.blockWall);
					if (blockWall != attack.blockWall)
					{
						attack.blockWall = blockWall;
					}
					// Block Climb
					bool blockClimb = EditorGUILayout.Toggle(new GUIContent("Block Climb", "If true the attack system will not allow ladder or rope climbing whilst this attack is playing."), attack.blockClimb);
					if (blockClimb != attack.blockClimb)
					{
						attack.blockClimb = blockClimb;
					}
					// Block Special
					bool blockSpecial = EditorGUILayout.Toggle(new GUIContent("Block Special", "If true the attack system will not allow special movements whilst this attack is playing."), attack.blockSpecial);
					if (blockClimb != attack.blockSpecial)
					{
						attack.blockSpecial = blockSpecial;
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
				}
				
			}

			GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));

			// Ammo item
			string ammoType = EditorGUILayout.TextField(new GUIContent("Ammo Type", "Item type to use for ammo. Empty means infinite ammo."), attack.ammoType);
			if (ammoType != attack.ammoType)
			{
				attack.ammoType = ammoType;
			}

			// Projectiles 
			if (attack.projectileData == null) attack.projectileData = new List<MultiProjectileData> ();


			List<MultiProjectileData> projectiles = attack.projectileData.ToList ();
			foreach (MultiProjectileData projectile in projectiles)
			{
				DrawProjectileEditor(projectile, attack);
			}

			// Add new button
			if (GUILayout.Button("Add Projectile"))
			{
				MultiProjectileData projectile = new MultiProjectileData();
				attack.projectileData.Add(projectile);
			}

			// Draw a line
			GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));

		}
				
		virtual protected void DrawProjectileEditor(MultiProjectileData projectile, MultiProjectileAttackData attack)
		{
			// Projectile prefab
			GameObject prefab = (GameObject) EditorGUILayout.ObjectField(new GUIContent("Projectile Prefab", "Prefab to use for the projectile. Created on fire."),
			                                                             projectile.projectilePrefab, typeof(GameObject), false);
			if (prefab != projectile.projectilePrefab)
			{
				projectile.projectilePrefab = prefab;
			}
				
			// Projectile delay
			float projectileDelay = EditorGUILayout.FloatField(new GUIContent("Projectile Delay", "Delay in seconds (from attack start) before the projectile is instantiated."), projectile.delay);
			// if (projectileDelay > attack.attackTime) projectileDelay = attack.attackTime;
			if (projectileDelay != projectile.delay)
			{
				projectile.delay = projectileDelay;
			}

			// Position
			Vector2 positionOffset = EditorGUILayout.Vector2Field(new GUIContent("Position Offset", "Additional position offset to apply to this projectile."), projectile.positionOffset);
			if (positionOffset.x != projectile.positionOffset.x || positionOffset.y != projectile.positionOffset.y )
			{
				projectile.positionOffset = positionOffset;
			}

			// Angle
			float angleOffset =  EditorGUILayout.FloatField(new GUIContent("Angle Offset", "Additional rotation to apply to this prefab in degrees."), projectile.angleOffset);
			if (angleOffset != projectile.angleOffset)
			{
				projectile.angleOffset = angleOffset;
			}

			// Flip X/Y
			EditorGUILayout.BeginHorizontal();
			bool flipX =  EditorGUILayout.Toggle(new GUIContent("Flip X", "Flip X offset."), projectile.flipX);
			if (flipX != projectile.flipX)
			{
				projectile.flipX = flipX;
			}
			GUILayout.FlexibleSpace ();
			bool flipY =  EditorGUILayout.Toggle(new GUIContent("Flip Y", "Flip Y offset."), projectile.flipY);
			if (flipY != projectile.flipY)
			{
				projectile.flipY = flipY;
			}

			EditorGUILayout.EndHorizontal();

			// Remove projectile
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button ("Remove Projectile", GUILayout.MinWidth(100)))
			{
				attack.projectileData.Remove(projectile);
			}
			EditorGUILayout.EndHorizontal();

			// Draw a line
			// GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));

		}
	}
}