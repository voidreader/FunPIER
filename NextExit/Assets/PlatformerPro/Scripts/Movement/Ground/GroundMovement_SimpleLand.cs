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
	public class GroundMovement_SimpleLand : GroundMovement
	{

		#region members

		/// <summary>
		/// How long to play landing animation for.
		/// </summary>
		public float landTime;
		
		/// <summary>
		/// Can character still move on ground while landing animation is being played.
		/// </summary>
		public bool allowWalk;

		/// <summary>
		/// How long till we stop rolling
		/// </summary>
		protected float landTimer;

		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Special/Land";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Ground movement which plays a landing animation. Should NOT be the only ground movement.";
		
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
		/// The index for the land time.
		/// </summary>
		protected const int LandTimeIndex = 0;

		/// <summary>
		/// The index for allow walk in the movement data.
		/// </summary>
		protected const int AllowWalkIndex = 1;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 2;

		/// <summary>
		/// The minimum speed for land. Could be made a variable.
		/// </summary>
		protected const float MinSpeedForLand = -2.0f;

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
			AssignReferences (character);

			if (movementData != null && movementData.Length >= MovementVariableCount)
			{
				landTime = movementData[LandTimeIndex].FloatValue;
				allowWalk = movementData[AllowWalkIndex].BoolValue;
			}
			else
			{
				Debug.LogError("Invalid movement data.");
			}
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
			if (landTimer > 0.0f && character.Grounded)
			{
				return true;
			}

			// New roll
			if (character.Grounded && character.PreviousVelocity.y <= MinSpeedForLand)
			{
				landTimer = landTime;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions.
		/// </summary>
		override public void LosingControl()
		{
			landTimer = 0.0f;
			if (allowWalk) character.DefaultGroundMovement.LosingControl ();
		}

		#endregion

#if UNITY_EDITOR

		#region draw inspector

		/// <summary>
		/// Draws the inspector.
		/// </summary>
		public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{
			if (movementData == null || movementData.Length < MovementVariableCount)
			{
				movementData = new MovementVariable[MovementVariableCount];
			}

			// Roll speed
			if (movementData[LandTimeIndex] == null) movementData[LandTimeIndex] = new MovementVariable(0);
			movementData[LandTimeIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Land Time", "How long we play landing animation for."), movementData[LandTimeIndex].FloatValue);
			if (movementData[LandTimeIndex].FloatValue < 0) movementData[LandTimeIndex].FloatValue = 0.0f;

			// Y velocity for roll
			if (movementData[AllowWalkIndex] == null) movementData[AllowWalkIndex] = new MovementVariable();
			movementData[AllowWalkIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Allow Walk", "If true character can still move using default ground movement while landing animation is played."), movementData[AllowWalkIndex].BoolValue);

			return movementData;
		}

		#endregion

#endif
	}

}

