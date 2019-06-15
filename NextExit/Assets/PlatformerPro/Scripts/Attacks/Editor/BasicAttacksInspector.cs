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
			// Unity says we don't need to do this, but if we don't do this then the automatic serialised object updates get the same name as the last object recorded
			Undo.FlushUndoRecordObjects ();
			Undo.RecordObject (target, "Attack Update");
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
			if (((BasicAttacks)target).attacks == null)  ((BasicAttacks)target).attacks = new List<BasicAttackData> ();
			List<BasicAttackData> attackList =((BasicAttacks)target).attacks.ToList();
			foreach (BasicAttackData data in attackList)
			{
				DrawBasicAttackEditor(data);
			}
			
			if (target is BlockAttack)
			{
				BlockDirection dir = (BlockDirection)EditorGUILayout.EnumPopup(new GUIContent("Block Direction", "Direction to block"), ((BlockAttack)target).blockDirection);
				if (dir != ((BlockAttack)target).blockDirection)
				{
					((BlockAttack)target).blockDirection = dir;
				}
			}

			// Add new button
			if (((BasicAttacks)target).CanHaveMultipleAttacks) {
				if (GUILayout.Button ("Add Attack")) {
					BasicAttackData attack = new BasicAttackData ();
					((BasicAttacks)target).attacks.Add (attack);
				}
			}
		}

		/// <summary>
		/// Draws the inspector for a basic attack.
		/// </summary>
		/// <param name="attack">Attack.</param>
		virtual protected void DrawBasicAttackEditor(BasicAttackData attack)
		{
			if (!(target is ComboAttacks))
			{
				string name = EditorGUILayout.TextField(new GUIContent("Name", "Human readable name (optional)."), attack.name);
				if (name != attack.name)
				{
					attack.name = name;
				}
			}
			if (((BasicAttacks)target).CanUserSetAttackType) {
				AttackType type = (AttackType)EditorGUILayout.EnumPopup (new GUIContent ("Attack Type", "Attack which this combo triggers from. Empty or null means trigger from ANY other attack."), attack.attackType);
				if (type != attack.attackType) {
					attack.attackType = type;
				}
			}

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

			if (fireInputType == FireInputType.FIRE_WHEN_RELEASED) 
			{
				GUILayout.Label ("Charge", EditorStyles.boldLabel);
				if (attack.chargeThresholds == null) attack.chargeThresholds = new float[0];
				if (attack.chargeThresholds.Length == 0)
				{
					GUILayout.Label ("Charge Mode is Analog");
					if (GUILayout.Button ("Switch to Digital"))
					{
						attack.chargeThresholds = new float[]{ 0 };
					}
				}
				else
				{
					GUILayout.Label ("Charge Mode is Digital");
					if (GUILayout.Button ("Switch to Analog"))
					{
						attack.chargeThresholds = new float[0];
					} 
					else
					{
						List<float> chargeThresholds = attack.chargeThresholds.ToList ();
						List<int> indexesToRemove = new List<int> ();
						for (int i = 0; i < chargeThresholds.Count; i++)
						{
							EditorGUILayout.BeginHorizontal ();
							chargeThresholds [i] = EditorGUILayout.FloatField (new GUIContent ("Level " + (i + 1) + ": ", "Time required to reach level " + (i + 1)), chargeThresholds [i]);
							if (GUILayout.Button ("Remove", EditorStyles.miniButton))
							{
								indexesToRemove.Add (i);
							}
							EditorGUILayout.EndHorizontal ();
						}
						foreach (int i in indexesToRemove)
						{
							chargeThresholds.RemoveAt (i);
						}
						EditorGUILayout.BeginHorizontal ();
						GUILayout.FlexibleSpace ();
						if (GUILayout.Button ("Add", EditorStyles.miniButton))
						{
							chargeThresholds.Add (chargeThresholds.Last() + 1.0f);
						}
						EditorGUILayout.EndHorizontal ();
						attack.chargeThresholds = chargeThresholds.OrderBy (t => t).ToArray();
					}
				}
			}
			GUILayout.Label ("Timing", EditorStyles.boldLabel);
			float length = EditorGUILayout.FloatField(new GUIContent("Attack Length", "The animation length in seconds."), attack.attackTime);
			if (length != attack.attackTime && length > 0.0f)
			{
				attack.attackTime = length;
			}
			float coolDown = EditorGUILayout.FloatField(new GUIContent("Attack Cool Down", "The time after the attack where it cannot be triggered again."), attack.coolDown);
			if (coolDown != attack.coolDown)
			{
				if (coolDown < 0) coolDown = 0;
				attack.coolDown = coolDown;
			}
			if (attack.attackType != AttackType.PROJECTILE)
			{
				float min = attack.attackHitBoxStart;
				float max = attack.attackHitBoxEnd;
				EditorGUILayout.MinMaxSlider(new GUIContent("Hit Window", "Enable and disable point for the characters hit box."), ref min, ref max, 0, 1);
				if (max <= min + 0.1f) max += 0.1f;
				if (min != attack.attackHitBoxStart || max != attack.attackHitBoxEnd)
				{
					attack.attackHitBoxStart = min;
					attack.attackHitBoxEnd = max;
				}
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
			if (attack.attackType != AttackType.PROJECTILE)
			{
				// Hit Box
				CharacterHitBox hitBox = (CharacterHitBox) EditorGUILayout.ObjectField(new GUIContent("Hit Box", "The character hit box that will be enabled when this attack starts"), 
				                                                                       attack.hitBox, typeof(CharacterHitBox), true);
				if (hitBox != attack.hitBox)
				{
					attack.hitBox = hitBox;
				}
			}
			else
			{
				// Projectile prefab
				GameObject prefab = (GameObject) EditorGUILayout.ObjectField(new GUIContent("Projectile Prefab", "Prefab to use for the projectile. Created on fire."),
				                                                             attack.projectilePrefab, typeof(GameObject), false);
				if (prefab != attack.projectilePrefab)
				{
					attack.projectilePrefab = prefab;
				}
				// Ammo item
				string ammoType = EditorGUILayout.TextField(new GUIContent("Ammo Type", "Item type to use for ammo. Empty means infinite ammo."), attack.ammoType);
				if (ammoType != attack.ammoType)
				{
					attack.ammoType = ammoType;

				}
				// Projectile delay
				float projectileDelay = EditorGUILayout.FloatField(new GUIContent("Projectile Delay", "Delay in seconds (from attack start) before the projectile is instantiated."), attack.projectileDelay);
				if (projectileDelay > attack.attackTime) projectileDelay = attack.attackTime;
				if (projectileDelay != attack.projectileDelay)
				{
					attack.projectileDelay = projectileDelay;
				}
			}
            GUILayout.Label("Durability", EditorStyles.boldLabel);
            string durabilitySlot = EditorGUILayout.TextField(new GUIContent("Damage Slot", "If an item is in this slot it will take durability damage each time the weapon connects."), attack.durabilitySlot);
            if (durabilitySlot != attack.durabilitySlot)
            {
                attack.durabilitySlot = durabilitySlot;
            }
            if (durabilitySlot != null && durabilitySlot != "" && durabilitySlot != "NONE") { 
                bool loseDurabilityOnUse = EditorGUILayout.Toggle(new GUIContent("Lose on Use", "If true every time the attack is used the item in the slot will take damage."), attack.loseDurabilityOnUse);
                if (loseDurabilityOnUse != attack.loseDurabilityOnUse)
                {
                    attack.loseDurabilityOnUse = loseDurabilityOnUse;
                }
            }
            GUILayout.Label ("Other", EditorStyles.boldLabel);
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

			// Set X to Zero
			bool resetVelocityX = EditorGUILayout.Toggle(new GUIContent("Reset X Velocity", "If true the attack system will set X velocity to zero."), attack.resetVelocityX);
			if (resetVelocityX != attack.resetVelocityX)
			{
				attack.resetVelocityX = resetVelocityX;
			}

			// Set Y to Zero
			bool resetVelocityY = EditorGUILayout.Toggle(new GUIContent("Reset Y Velocity", "If true the attack system will set Y velocity to zero."), attack.resetVelocityY);
			if (resetVelocityY != attack.resetVelocityY)
			{
				attack.resetVelocityY = resetVelocityY;
			}

			// Remove button
			if (((BasicAttacks)target).CanHaveMultipleAttacks)
			{
					EditorGUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if (GUILayout.Button ("Remove", GUILayout.MinWidth(100)))
					{
						if (target is ComboAttacks)
						{
							((ComboAttacks)target).comboAttacks.Remove ((ComboAttackData)attack);
						}
						else
						{
							((BasicAttacks)target).attacks.Remove (attack);
						}
						
					}
					EditorGUILayout.EndHorizontal();
			}

			// Draw a line
			GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
		}
	}

}