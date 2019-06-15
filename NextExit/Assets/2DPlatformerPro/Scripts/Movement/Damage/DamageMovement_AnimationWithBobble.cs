#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A damage movement which plays an animation and bobbles the character in to the air.
	/// </summary>
	public class DamageMovement_AnimationWithBobble : DamageMovement
	{
		
		#region members

		/// <summary>
		/// How high should the character bobble.
		/// </summary>
		public float bobbleHeight;

		/// <summary>
		/// The characters starting position (pre-bobble).
		/// </summary>
		protected float originalHeight;

		/// <summary>
		/// Derived initial velocity based on jumpHeight and time.
		/// </summary>
		protected float initialVelocity;

		/// <summary>
		/// Direction character was facing when the bobble started.
		/// </summary>
		protected int facingDirection;

		/// <summary>
		/// Have we started a bobble.
		/// </summary>
		protected bool hasAppliedVelocity;

		/// <summary>
		/// If character was parented to a platform ensure they have left it before handing back control.
		/// </summary>
		protected bool hasLeftPlatform;

		#endregion
		
		#region Unity Hooks
		
		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Animation with Bobbble";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "A damage movement which plays an animation and bobbles the character in to the air. When used as a death animation the character will bobble then fall indefinitely.";
		
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
		/// The index for the bobble height in the movement data.
		/// </summary>
		private const int BobbleHeightIndex = 0;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 1;
		
		#endregion
		
		#region public methods
		
		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			character.AddVelocity(0, TimeManager.FrameTime * character.DefaultGravity, false);
			character.Translate(0, character.Velocity.y * TimeManager.FrameTime, true);
			// Make sure we don't travel too far if we aren't dying
			if (!isDeath && character.Transform.position.y < originalHeight) character.Translate(0, originalHeight - character.Transform.position.y, true);
			hasAppliedVelocity = true;
			if (!hasLeftPlatform && character.ParentPlatform == null) hasLeftPlatform = true;
		}

		virtual protected void DoBobble()
		{
			hasLeftPlatform = (character.ParentPlatform == null || isDeath);
			originalHeight = character.Transform.position.y;
			character.SetVelocityY(initialVelocity);
			hasAppliedVelocity = false;
			if (isDeath) character.ParentPlatform = null;
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
				bobbleHeight = movementData[BobbleHeightIndex].FloatValue;
			}
			else
			{
				Debug.LogError("Invalid movement data.");
			}

			// Calculate initial velocity
			initialVelocity = Mathf.Sqrt(-2.0f * character.DefaultGravity * bobbleHeight);

			return this;
		}
		
		
		/// <summary>
		/// If the jump just started force control.
		/// </summary>
		override public bool WantsControl()
		{
			// Character is dying no need to give control back
			if (isDeath) return true;
			if (hasAppliedVelocity && (character.transform.position.y <= originalHeight || (hasLeftPlatform && character.ParentPlatform != null)))
			{
				return false;
			}
			return true;
		}
		
		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				if (isDeath) return AnimationState.DEATH;
				return AnimationState.HURT_NORMAL;
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
				return facingDirection;
			}
		}

		/// <summary>
		/// Ignore base collissions, we will be returning to starting position anyway.
		/// </summary>
		override public RaycastType ShouldDoBaseCollisions
		{
			get
			{
				if (!isDeath) return RaycastType.FOOT;
				return RaycastType.NONE;
			}
		}

		/// <summary>
		/// Start the damage movement.
		/// </summary>
		/// <param name="info">Info.</param>
		override public void Damage(DamageInfo info, bool isDeath)
		{
			DoBobble();
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
			if (movementData[BobbleHeightIndex] == null) movementData[BobbleHeightIndex] = new MovementVariable();
			movementData[BobbleHeightIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Bobble Height", "How high the character should bobble."), movementData[BobbleHeightIndex].FloatValue);
			if (movementData[BobbleHeightIndex].FloatValue < 0) movementData[BobbleHeightIndex].FloatValue = 0.0f;

			return movementData;
		}
		
		#endregion

#endif

	}
	
}
