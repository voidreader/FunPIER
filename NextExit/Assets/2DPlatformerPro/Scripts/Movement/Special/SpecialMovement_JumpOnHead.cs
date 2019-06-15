#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A special movement which provides a more robust (but slightly more performance expensive) alternative to the JumpOnHeadHitBox.
	/// </summary>
	public class SpecialMovement_JumpOnHead : SpecialMovement
	{
		#region members

		/// <summary>
		/// The amount of damage a jump on head does.
		/// </summary>
		[Tooltip ("The amount of damage a jump on head does.")]
		public int damageAmount;
		
		/// <summary>
		/// The height of the bobble.
		/// </summary>
		[Tooltip ("How high to bobble (bounce).")]
		public float bobbleHeight;
		
		/// <summary>
		/// If true enemies in the hiding state will be 'kicked' even if the character isn't jumping.
		/// </summary>
		[Tooltip ("If true enemies in the HIDING state will be 'kicked' even if the character isn't jumping.")]
		public bool kickHidingEnemies;

		/// <summary>
		/// The layers to check for enemies.
		/// </summary>
		[Tooltip ("The layers to check for enemies.")]
		public LayerMask enemyLayerMask;

		/// <summary>
		/// Did we just jump on an an enemy.
		/// </summary>
		protected bool waitingToJump;

		/// <summary>
		/// Did we just kick an enemy.
		/// </summary>
		protected bool waitingToKick;

		/// <summary>
		/// Timer to control how long we play the kicked animation for;
		/// </summary>
		protected float kickAnimationTimer;

		/// <summary>
		/// Cached reference to enemy hurt box.
		/// </summary>
		protected IHurtable enemyHurtBox;

		/// <summary>
		/// Cached damage info.
		/// </summary>
		protected DamageInfo damageInfo;

		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Jump On Head";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "A special movement which provides a more robust (but more expensive) alternative to the JumpOnHeadHitBox.";
		
		/// <summary>
		/// Static movement info used by the editor.
		/// </summary>
		new public static MovementInfo Info
		{
			get
			{
				return new MovementInfo(Name, Description);
			}
		}

		/// <summary>
		/// The index of the damage amount in movement data.
		/// </summary>
		protected const int DamageAmountIndex = 0;

		/// <summary>
		/// The index of the bobble height in movement data.
		/// </summary>
		protected const int BobbleHeightIndex = 1;

		/// <summary>
		/// The index of kick hiding enemies in movement data.
		/// </summary>
		protected const int KickHidingEnemiesIndex = 2;

		/// <summary>
		/// The index of the enemy layer mask in the movement data.
		/// </summary>
		protected const int EnemyLayermaskIndex = 3;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 4;

		private const float kickTime = 0.25f;

		#endregion

		#region properties

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				if (kickAnimationTimer > 0) 
				{
					return AnimationState.ATTACK_KICK;
				}
				return character.DefaultAirMovement.AnimationState;
			}
		}

		#endregion

		#region Unity Hooks

		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update()
		{
			if (kickAnimationTimer > 0) kickAnimationTimer -= TimeManager.FrameTime;
		}

		#endregion

		#region public methods
		
		/// <summary>
		/// Initialise this instance.
		/// </summary>
		override public Movement Init(Character character)
		{
			this.character = character;
			damageInfo = new DamageInfo (damageAmount, DamageType.PHYSICAL, Vector2.zero, character);
			return this;
		}

		/// <summary>
		/// Initialise the movement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			this.character = character;
			
			if (movementData != null && movementData.Length >= MovementVariableCount)
			{
				damageAmount = movementData[DamageAmountIndex].IntValue;
				bobbleHeight = movementData[BobbleHeightIndex].FloatValue;
				kickHidingEnemies = movementData[KickHidingEnemiesIndex].BoolValue;
				enemyLayerMask = (LayerMask) movementData[KickHidingEnemiesIndex].IntValue;
			}
			else
			{
				Debug.LogError("Invalid movement data.");
			}
			damageInfo = new DamageInfo (damageAmount, DamageType.PHYSICAL, Vector2.zero, character);
			return this;
		}

		/// <summary>
		/// Gets a value indicating whether this movement wants to do a special move.
		/// </summary>
		/// <value><c>true</c> if this instance wants control; otherwise, <c>false</c>.</value>
		override public bool WantsSpecialMove()
		{
			if (CheckForJumpOnHead ()) return true;
			if (CheckForKick ()) return true;
			return false;
		}

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			if (waitingToJump) 
			{
				if (bobbleHeight > 0) character.DefaultAirMovement.DoOverridenJump(bobbleHeight, 1);
				if (enemyHurtBox != null && enemyHurtBox is EnemyHurtBox)
				{
					damageInfo.Direction = transform.position - ((EnemyHurtBox)enemyHurtBox).transform.position;
					enemyHurtBox.Damage(damageInfo);
				}
				waitingToJump = false;
			}
			else if (waitingToKick)
			{
				if (enemyHurtBox != null && enemyHurtBox is EnemyHurtBox)
				{
					damageInfo.Direction = transform.position - ((EnemyHurtBox)enemyHurtBox).transform.position;
					enemyHurtBox.Damage(damageInfo);
				}
				waitingToKick = false;
				kickAnimationTimer = kickTime;
			}
		}

		#endregion

		#region protected methods

		/// <summary>
		/// Checks for jumping on head.
		/// </summary>
		/// <returns><c>true</c>, if character is jumping on an enmies head.</returns>
		virtual protected bool CheckForJumpOnHead()
		{
			if (character.Velocity.y >= 0) return false;
			IHurtable enemyHurtBox = CheckCollisions (RaycastType.FOOT);
			if (enemyHurtBox == null) return false;
			this.enemyHurtBox = enemyHurtBox;
			waitingToJump = true;
			return true;
		}
		/// <summary>
		/// Checks for jumping on kicking.
		/// </summary>
		/// <returns><c>true</c>, if character is kicking an enemy.</returns>
		virtual protected bool CheckForKick()
		{
			IHurtable enemyHurtBox = null;
			if (!kickHidingEnemies) return false;
			if (character.Velocity.x < 0) 
			{
				enemyHurtBox = CheckCollisions (RaycastType.SIDE_LEFT);
				if (enemyHurtBox == null) return false;
				if (enemyHurtBox.Mob is Enemy && ((Enemy)enemyHurtBox.Mob).State == EnemyState.HIDING)
				{
					this.enemyHurtBox = enemyHurtBox;
					waitingToKick = true;
					return true;
				}
			}
			else if (character.Velocity.x > 0) 
			{
				enemyHurtBox = CheckCollisions (RaycastType.SIDE_RIGHT);
				if (enemyHurtBox == null) return false;
				if (enemyHurtBox.Mob is Enemy && ((Enemy)enemyHurtBox.Mob).State == EnemyState.HIDING)
				{
					this.enemyHurtBox = enemyHurtBox;
					waitingToKick = true;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Checks the given collisions type for enemies.
		/// </summary>
		/// <returns>Returns an IHurtable if found, or null if not found.</returns>
		virtual protected IHurtable CheckCollisions(RaycastType type)
		{
			for (int i = 0; i < character.Colliders.Length; i++)
			{
				if (character.Colliders[i].RaycastType == type)
				{
					for (int j = 0; j < character.CurrentCollisions[i].Length; j++)
					{
						if (character.CurrentCollisions[i][j].collider != null && 
						    ((1 << character.CurrentCollisions[i][j].collider.gameObject.layer & enemyLayerMask) == enemyLayerMask))
						{
							IHurtable enemyHurtBox = (IHurtable) character.CurrentCollisions[i][j].collider.GetComponent(typeof(IHurtable));
							if (enemyHurtBox != null) return enemyHurtBox;
						}
					}
				}
			}
			return null;
		}

		#endregion

#if UNITY_EDITOR
		
		#region draw inspector

		public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{
			if (movementData == null || movementData.Length < MovementVariableCount)
			{
				movementData = new MovementVariable[MovementVariableCount];
			}

			// Damage amount
			if (movementData[DamageAmountIndex] == null) movementData[DamageAmountIndex] = new MovementVariable(1);
			movementData[DamageAmountIndex].IntValue = EditorGUILayout.IntField(new GUIContent("Damage Amount", "How much damage jumping on head does."), movementData[DamageAmountIndex].IntValue);
			if (movementData [DamageAmountIndex].IntValue < 0) movementData [DamageAmountIndex].IntValue = 0;


			// Bobble Height
			if (movementData[BobbleHeightIndex] == null) movementData[BobbleHeightIndex] = new MovementVariable(1.0f);
			movementData[BobbleHeightIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Bobble Height", "How high to bobble (bounce)."), movementData[BobbleHeightIndex].FloatValue);
			if (movementData [BobbleHeightIndex].FloatValue < 0) movementData [BobbleHeightIndex].FloatValue = 0;


			// Kick Hiding Enemies
			if (movementData[KickHidingEnemiesIndex] == null) movementData[KickHidingEnemiesIndex] = new MovementVariable();
			movementData[KickHidingEnemiesIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Kick Hiding Enemies", "If true enemies in the HIDING state will be 'kicked' even if the character isn't jumping."), movementData[KickHidingEnemiesIndex].BoolValue);

			// Layer Mask
			if (movementData[EnemyLayermaskIndex] == null) movementData[EnemyLayermaskIndex] = new MovementVariable();
			movementData[EnemyLayermaskIndex].IntValue = EditorGUILayout.LayerField(new GUIContent("Enemy Layer Mask", "Layers to check for enemies, these layers MUST also be in the character layer mask."), movementData[EnemyLayermaskIndex].IntValue);

			// Check for correct layer
			if (target != null)
			{
				if (((1 << movementData[EnemyLayermaskIndex].IntValue) & target.layerMask) != (1 << movementData[EnemyLayermaskIndex].IntValue))
				{
					EditorGUILayout.HelpBox("Enemy layer MUST be in the overall layer mask. Update the character.", MessageType.Error);
				}
			}

			return movementData;
		}
		
		#endregion
		
#endif
	}
}
