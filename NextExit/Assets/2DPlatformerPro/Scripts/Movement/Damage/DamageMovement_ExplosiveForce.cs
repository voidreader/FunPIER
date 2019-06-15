#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A damage movement which knocks back the character.
	/// </summary>
	public class DamageMovement_ExplosiveForce : DamageMovement
	{
		
		#region members
		
		/// <summary>
		/// How much force to apply.
		/// </summary>
		public Vector2 forceMultiplier;

		/// <summary>
		/// If true, we let the air movement do the movement, else we do it here.
		/// </summary>
		public bool useAirMovement;

		/// <summary>
		/// How long do we keep control for?
		/// </summary>
		public float explosionTime;

		/// <summary>
		/// Drag to apply in X if we aren't using air movement.
		/// </summary>
		public float drag;

		/// <summary>
		/// If true just apply force in direction of x, ignoring extent of x damage direction, and ignoring y direction completely.
		/// </summary>
		public bool fixedForce;

		/// <summary>
		/// If true lose contorl when hitting the ground.
		/// </summary>
		public bool cancelOnGrounded;

		/// <summary>
		/// Timer for the explosion
		/// </summary>
		protected float explosionTimer;

		/// <summary>
		/// The explosion force.
		/// </summary>
		protected Vector2 explosionForce;

		#endregion
		
		#region Unity Hooks
		
		/// <summary>
		/// Unity Update() hook, update the timer.
		/// </summary>
		void Update()
		{
			if (explosionTimer > 0) 
			{
				explosionTimer -= TimeManager.FrameTime;
			}
		}
		
		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Explosion Force";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "A damage movement which knocks back the character.";
		
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
		/// The index for the  force multiplier in the movement data.
		/// </summary>
		protected const int ForceMultiplierIndex = 0;

		/// <summary>
		/// The index for use air movement in the movement data.
		/// </summary>
		protected const int UseAirMovementIndex = 1;

		/// <summary>
		/// The index for the explosion time in the movement data.
		/// </summary>
		protected const int ExplosionTimeIndex = 2;

		/// <summary>
		/// The index of the drag in the movement data.
		/// </summary>
		protected const int DragIndex = 3;

		/// <summary>
		/// The index of the fixed force value in the movement data.
		/// </summary>
		protected const int FixedForceIndex = 4;

		/// <summary>
		/// The index of the cancel on grounded index value in the movement data.
		/// </summary>
		protected const int CancelOnGroundedIndex = 5;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 6;

		/// <summary>
		/// Default explosion time
		/// </summary>
		protected const float DefaultExplosionTime = 1.0f;

		/// <summary>
		/// Default explosion force
		/// </summary>
		protected static Vector2 DefaultExplosionForce = new Vector2(5.0f, 10.0f);

		/// <summary>
		/// The default drag.
		/// </summary>
		protected const float DefaultDrag = 5.0f;

		#endregion
		
		#region public methods
		
		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			if (useAirMovement)
			{
				character.DefaultAirMovement.DoOverridenMove (true, true, 0, ButtonState.NONE);
			}
			else
			{
				// X move

				// Apply drag (we use a friction like equation which seems to look better than a drag like one)
				if (character.Velocity.x > 0) 
				{
					character.AddVelocity(-character.Velocity.x * drag * TimeManager.FrameTime, 0, true);
					if (character.Velocity.x < 0) character.SetVelocityX(0);
				}
				else if (character.Velocity.x < 0) 
				{
					character.AddVelocity(-character.Velocity.x * drag * TimeManager.FrameTime, 0, true);
					if (character.Velocity.x > 0) character.SetVelocityX(0);
				}
			
				// Translate
				character.Translate(character.Velocity.x * TimeManager.FrameTime, 0, true);

				// Y Move

				// Apply gravity
				if (!character.Grounded )
				{
					character.AddVelocity(0, TimeManager.FrameTime * character.DefaultGravity, false);
				}
				// Translate
				character.Translate(0, character.Velocity.y * TimeManager.FrameTime, true);
			}
		}
		
		/// <summary>
		/// Initialise this instance.
		/// </summary>
		override public Movement Init(Character character)
		{
			this.character = character;
			return this;
		}
		
		/// <summary>
		/// Initialise the mvoement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			this.character = character;
			
			// Set variables
			if (movementData != null && movementData.Length == MovementVariableCount)
			{
				forceMultiplier = movementData[ForceMultiplierIndex].Vector2Value;
				explosionTime = movementData[ExplosionTimeIndex].FloatValue;
				useAirMovement = movementData[UseAirMovementIndex].BoolValue;
				drag = movementData[DragIndex].FloatValue;
				fixedForce = movementData[FixedForceIndex].BoolValue;
				cancelOnGrounded = movementData[CancelOnGroundedIndex].BoolValue;
			}
			else
			{
				Debug.LogError("Invalid movement data.");
			}
			
			return this;
		}
		
		
		/// <summary>
		/// If the timer is non-zero keep control.
		/// </summary>
		override public bool WantsControl()
		{
			if (isDeath) return true;
			if (character.Grounded && cancelOnGrounded) return false;
			if (explosionTimer <= 0) 
			{
				return false;
			}
			return true;
		}

		
		/// <summary>
		/// For damage the default is to not apply gravity.
		/// </summary>
		override public bool ShouldApplyGravity
		{
			get
			{
				return useAirMovement ? character.DefaultAirMovement.ShouldApplyGravity : false;
			}
		}

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				return AnimationState.HURT_NORMAL;
			}
		}

		
		/// <summary>
		/// Start the damage movement.
		/// </summary>
		/// <param name="info">Info.</param>
		override public void Damage(DamageInfo info, bool isDeath)
		{
			explosionTimer = explosionTime;
			if (fixedForce)
			{
				explosionForce = new Vector2(info.Direction.x > 0 ? -forceMultiplier.x : forceMultiplier.x, forceMultiplier.y);
			}
			else
			{
				explosionForce = new Vector2(info.Direction.x * -forceMultiplier.x, info.Direction.y * -forceMultiplier.y);
			}
			character.SetVelocityX(explosionForce.x);
			character.SetVelocityY(explosionForce.y);
			this.isDeath = isDeath;
		}

		#endregion

#if UNITY_EDITOR
		
		#region draw inspector
		/// <summary>
		/// Draws the inspector.
		/// </summary>
		public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{
			if (movementData == null || movementData.Length != MovementVariableCount)
			{
				movementData = new MovementVariable[MovementVariableCount];
			}

			// Fixed Force
			if (movementData[FixedForceIndex] == null) movementData[FixedForceIndex] = new MovementVariable();
			movementData[FixedForceIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Fixed Force", "If true the explosion force below is used and everything but the direction of the force in x is ignored."), movementData[FixedForceIndex].BoolValue);

			// Explosion force
			if (movementData[ForceMultiplierIndex] == null) movementData[ForceMultiplierIndex] = new MovementVariable(DefaultExplosionForce);
			movementData[ForceMultiplierIndex].Vector2Value = EditorGUILayout.Vector2Field(new GUIContent("Explosion Force", "How strong is the explosion in x and y?"), movementData[ForceMultiplierIndex].Vector2Value);

			// Explosion Length
			if (movementData[ExplosionTimeIndex] == null) movementData[ExplosionTimeIndex] = new MovementVariable(DefaultExplosionTime);
			movementData[ExplosionTimeIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Explosion Time", "How long to maintain control of the character."), movementData[ExplosionTimeIndex].FloatValue);
			if (movementData[ExplosionTimeIndex].FloatValue < 0) movementData[ExplosionTimeIndex].FloatValue = 0.0f;

			// Use air movement
			if (movementData[UseAirMovementIndex] == null) movementData[UseAirMovementIndex] = new MovementVariable();
			movementData[UseAirMovementIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Use Air Movement", "If true the default air movement will be used to move the character."), movementData[UseAirMovementIndex].BoolValue);

			if (!movementData[UseAirMovementIndex].BoolValue)
			{
				// Drag 
				if (movementData[DragIndex] == null) movementData[DragIndex] = new MovementVariable(DefaultDrag);
				movementData[DragIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Drag (X)", "Drag to apply in X."), movementData[DragIndex].FloatValue);
				if (movementData[DragIndex].FloatValue < 1) movementData[DragIndex].FloatValue = 1.0f;

			}
			// Cancel on grounded
			if (movementData[CancelOnGroundedIndex] == null) movementData[CancelOnGroundedIndex] = new MovementVariable();
			movementData[CancelOnGroundedIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Cancel on Grounded", "If true the movement will lose control as soon as character is grounded."), movementData[CancelOnGroundedIndex].BoolValue);

			return movementData;
		}
		
		#endregion

#endif
	}
	
}
