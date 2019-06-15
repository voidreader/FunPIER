#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A damage movement which plays a random animation from a list.
	/// </summary>
	public class DamageMovement_RandomAnimation : DamageMovement
	{
		
		#region members
		
		/// <summary>
		/// How long the animation should be played for.
		/// </summary>
		public float animationLength;

		/// <summary>
		/// Should we still apply gravity and base collisions?
		/// </summary>
		public bool applyGravity;

		/// <summary>
		/// The states to choose from when in damaged state.
		/// </summary>
		public List<AnimationState> damagedStates;

		/// <summary>
		/// The states to choose from when in death state.
		/// </summary>
		public List<AnimationState> deathStates;

		/// <summary>
		/// The current animation.
		/// </summary>
		protected AnimationState currentAnimation;

		/// <summary>
		/// The animation timer. Counts down for how long we should hold the state.
		/// </summary>
		protected float animationTimer;

		#endregion
		
		#region Unity Hooks
		
		/// <summary>
		/// Unity Update() hook, update the timer.
		/// </summary>
		void Update()
		{
			if (animationTimer > 0) 
			{
				animationTimer -= TimeManager.FrameTime;
			}
		}
		
		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Random Animation";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "A damage movement which plays a random animation by setting an animation state. For death movement it plays a random death animation and does not give control back.";
		
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
		/// The index for the animation length in the movement data.
		/// </summary>
		private const int AnimationLengthIndex = 0;

		/// <summary>
		/// The index for Apply Gravityin the movement data.
		/// </summary>
		private const int ApplyGravityIndex = 1;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 2;
		
		#endregion
		
		#region public methods
		
		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
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
				animationLength = movementData[AnimationLengthIndex].FloatValue;
				applyGravity = movementData[ApplyGravityIndex].BoolValue;
			}
			else
			{
				Debug.LogError("Invalid movement data.");
			}
			
			return this;
		}
		
		
		/// <summary>
		/// If the jump just started force control.
		/// </summary>
		override public bool WantsControl()
		{
			// No need to give control back on a death animation
			if (isDeath) return true;

			if (animationTimer <= 0) 
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
				return applyGravity;
			}
		}

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				return currentAnimation;
			}
		}

		
		/// <summary>
		/// Start the damage movement.
		/// </summary>
		/// <param name="info">Info.</param>
		override public void Damage(DamageInfo info, bool isDeath)
		{
			if (isDeath)
			{
				currentAnimation = AnimationState.DEATH;
				if (deathStates != null && deathStates.Count > 0) currentAnimation = deathStates[Random.Range (0, deathStates.Count)];
			}
			else
			{
				currentAnimation = AnimationState.HURT_NORMAL;
				if (damagedStates != null && damagedStates.Count > 0) currentAnimation = damagedStates[Random.Range (0, damagedStates.Count)];
			}
			animationTimer = animationLength;
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
			
			// Animation Length
			if (movementData[AnimationLengthIndex] == null) movementData[AnimationLengthIndex] = new MovementVariable();
			movementData[AnimationLengthIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Animation Length", "How long to play the damage animation for."), movementData[AnimationLengthIndex].FloatValue);
			if (movementData[AnimationLengthIndex].FloatValue < 0) movementData[AnimationLengthIndex].FloatValue = 0.0f;

			// Apply Gravity Length
			if (movementData[ApplyGravityIndex] == null) movementData[ApplyGravityIndex] = new MovementVariable();
			movementData[ApplyGravityIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Apply Gravity", "Should we still apply gravity and base collisions while the animatino plays?"), movementData[ApplyGravityIndex].BoolValue);

			return movementData;
		}
		
		#endregion

#endif
	}
	
}
