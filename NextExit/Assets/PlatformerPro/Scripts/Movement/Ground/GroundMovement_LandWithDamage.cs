#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Ground movement to play a land animation.
	/// </summary>
	public class GroundMovement_LandWithDamage : GroundMovement_SimpleLand
	{

		#region members

		/// <summary>
		/// Min speed for which damage will be calculated.
		/// </summary>
		public float minSpeedForDamage;

		/// <summary>
		/// Is damage constant?
		/// </summary>
		public bool constantDamage;

		/// <summary>
		/// The damage amount or if non-constant damage will be calcualted as (1 + (speed.y - minSpeedForDamage)) * damageMultiplier.
		/// </summary>
		public float damageAmount;

		/// <summary>
		/// Cached character health args.
		/// </summary>
		protected CharacterHealth characterHealth;

		/// <summary>
		/// How much damage should we cause this frame.
		/// </summary>
		protected int damageThisFrame;

		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Special/Land With Damage";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Ground movement which plays a landing animation and causes damage if speed is too high. Should NOT be the only ground movement.";
		
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
		/// The minimum index of the speed for damage.
		/// </summary>
		protected const int MinSpeedForDamageIndex = 2;

		/// <summary>
		/// The index of the constant damage.
		/// </summary>
		protected const int ConstantDamageIndex = 3;
		
		/// <summary>
		/// The index of the damage amount.
		/// </summary>
		protected const int DamageAmountIndex = 4;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 5;

		#endregion

		#region Unity hooks

		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update()
		{
			if (landTimer > 0.0f) 
			{
				landTimer -= TimeManager.FrameTime;
				if (character.IsAttacking) landTimer = 0.0f;
			}
		}

		#endregion

		#region public methods

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			if (damageThisFrame > 0)
			{
				characterHealth.Damage (damageThisFrame);
				damageThisFrame = 0;
			}
			if (landTimer > 0.0f)
			{
				if (allowWalk) Character.DefaultGroundMovement.DoMove ();
			}

		}

		/// <summary>
		/// Initialise the mvoement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
            base.Init(character, movementData);

			if (movementData != null && movementData.Length >= MovementVariableCount)
			{
				minSpeedForDamage = movementData[MinSpeedForDamageIndex].FloatValue;
				constantDamage =  movementData[ConstantDamageIndex].BoolValue;
				damageAmount = movementData[DamageAmountIndex].FloatValue;
			}
			else
			{
				Debug.LogError("Invalid movement data.");
			}

			characterHealth = character.GetComponentInChildren<CharacterHealth> ();
			if (characterHealth == null) Debug.LogWarning ("Land with damage requires a CharacterHealth component to be on the cahracter");

			return this;
		}

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				return AnimationState.LAND;
			}
		}

		/// <summary>
		/// Returns the direction the character is facing. 0 for none, 1 for right, -1 for left.
		/// This overriden version always returns the input direction.
		/// </summary>
		override public int FacingDirection
		{
			get 
			{
				if (allowWalk) return character.DefaultGroundMovement.FacingDirection;
				return 0;
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether this movement wants to control the movement on the ground.
		/// We want this if roll timer is set and user hasn't put in a counter input.
		/// </summary>
		/// <value><c>true</c> if this instance wants control; otherwise, <c>false</c>.</value>
		override public bool WantsGroundControl()
		{
			if (!enabled) return false;
		
			// Existing roll
			if (landTimer > 0.0f)
			{
				return true;
			}

			// New roll
			if (character.Grounded && character.PreviousVelocity.y <= MinSpeedForLand)
			{
				landTimer = landTime;
				if (character.PreviousVelocity.y <= minSpeedForDamage)
				{
					CalculateDamage();
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Works out how much damage we should apply during move.
		/// </summary>
		virtual protected  void CalculateDamage() 
		{
			int damage = (int) (damageAmount + 0.4999f); // We add to the float so we can't get rounding errors given the field is stored as a float
			if (!constantDamage) damage = (int)((1.0f + (character.PreviousVelocity.y - minSpeedForDamage)) * damageAmount);
			damageThisFrame = damage;
		}

		/// <summary>
		/// Does the damage.
		/// </summary>
		virtual protected void DoDamage(int damage)
		{
			characterHealth.Damage(new DamageInfo(damage, DamageType.LANDING, new Vector2(0, -1.0f)));
		}

		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions.
		/// </summary>
		override public void LosingControl()
		{
			landTimer = 0.0f;
			damageThisFrame = 0;
			if (allowWalk) character.DefaultGroundMovement.LosingControl ();
		}

		#endregion

#if UNITY_EDITOR

		#region draw inspector

		/// <summary>
		/// Draws the inspector.
		/// </summary>
		new public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{
			if (movementData == null || movementData.Length < MovementVariableCount)
			{
				movementData = new MovementVariable[MovementVariableCount];
			}

			// Min Speed for Damage
			if (movementData[MinSpeedForDamageIndex] == null) movementData[MinSpeedForDamageIndex] = new MovementVariable(MinSpeedForLand);
			movementData[MinSpeedForDamageIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Min Speed for Damage", "How fast character has to be falling before damage occurs."), movementData[MinSpeedForDamageIndex].FloatValue);
			if (movementData [MinSpeedForDamageIndex].FloatValue > MinSpeedForLand) movementData [MinSpeedForDamageIndex].FloatValue = MinSpeedForLand;

			// Y velocity for roll
			if (movementData[ConstantDamageIndex] == null) movementData[ConstantDamageIndex] = new MovementVariable(true);
			movementData[ConstantDamageIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Constant Damage", "If true fixed damage amount will be used. If false we multiple speed by damage."), movementData[ConstantDamageIndex].BoolValue);

			// Min Speed for Damage
			if (movementData[DamageAmountIndex] == null) movementData[DamageAmountIndex] = new MovementVariable(1);
			if (movementData [ConstantDamageIndex].BoolValue)
			{

				movementData[DamageAmountIndex].FloatValue = (float) EditorGUILayout.IntField(new GUIContent("Damage Amount", "Amount of damage."), (int) (movementData[DamageAmountIndex].FloatValue + 0.499f));
			}
			else
			{
				movementData[DamageAmountIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Damage Multiplier", "Multiplier applied to damage."), movementData[DamageAmountIndex].FloatValue);
			}

			if (movementData [DamageAmountIndex].FloatValue < 0) movementData [DamageAmountIndex].FloatValue = 0;


			// Draw base inspector and copy values
			MovementVariable[] baseMovementData = GroundMovement_SimpleLand.DrawInspector (movementData, ref showDetails, target);
			System.Array.Copy (baseMovementData, movementData, baseMovementData.Length);
			
			return movementData;

		}

		#endregion

#endif
	}

}

