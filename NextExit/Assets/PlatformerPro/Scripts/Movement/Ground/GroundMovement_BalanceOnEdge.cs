#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Ground movement_ digital.
	/// </summary>
	public class GroundMovement_BalanceOnEdge: GroundMovement
	{

		#region members

		/// <summary>
		/// If true the movement will not allow user to run off an edge. If false balanace animation will only show if velocity = 0
		/// </summary>
		public bool haltMovement;

		/// <summary>
		/// The number of colliders that should be hitting the ground in order for balance to be played.
		/// </summary>
		public int numberOfColliders;

		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Balance on Edge";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Ground movement which plays a balance animation when character is on the edge of a platform.";
		
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
		/// The index for the halt movement value in the movement data.
		/// </summary>
		protected const int HaltMovementIndex = 0;

		/// <summary>
		/// The index of the number of colliders value in the movement data.
		/// </summary>
		protected const int NumberOfCollidersIndex = 1;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 2;

		/// <summary>
		/// The default speed.
		/// </summary>
		protected const float DefaultColliders = 1;

		#endregion

		#region public methods

		/// <summary>
		/// Gets a value indicating whether this movement wants to control the movement on the ground.
		/// Default is false, with control falling back to default ground. Override if you want particular control.
		/// </summary>
		/// <value>true</value>
		/// <c>false</c>
		/// <returns><c>true</c>, if ground control was wantsed, <c>false</c> otherwise.</returns>
		override public bool WantsGroundControl() 
		{
			// Although this should really be based above ground and below air this little condition will ensure that it doesn't block jump
			if (character.DefaultAirMovement.WantsJump()) return false;

			if (character.GroundedFootCount != 0 && character.GroundedFootCount <= numberOfColliders)
			{
				if (haltMovement) 
				{
					// We need to work out if user is pressing opposite direction of the ledge
					float extent = 0.0f;
					for (int i = 0; i < character.Colliders.Length; i++)
					{
						if (character.Colliders[i].RaycastType == RaycastType.FOOT)
						{
							RaycastHit2D hit = character.GetClosestCollision(i);
							if (hit.collider != null)
							{
								extent += character.Colliders[i].Extent.x;
							}
						}
					}
					if (extent == 0) Debug.LogWarning("If you want to use halt on movement with your balance on edge movment then you need to set an odd number of colliders");
					if (extent > 0 && character.Input.HorizontalAxisDigital == 1) return false;
					if (extent < 0 && character.Input.HorizontalAxisDigital == -1) return false;
					return true;
				}
				else
				{
					if (character.Velocity.x == 0 && character.Input.HorizontalAxisDigital == 0) return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Does any movement that MUST be done after collissions are calculated.
		/// </summary>
		override public void PostCollisionDoMove()
		{
			if (enabled && !character.rotateToSlopes) SnapToGround ();
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
				haltMovement = movementData[HaltMovementIndex].BoolValue;
				numberOfColliders = movementData[NumberOfCollidersIndex].IntValue;
			}
			else
			{
				Debug.LogError("Movement data is invalid.");
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
				return AnimationState.IDLE_BALANCE;
			}
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

			// Halt movement
			if (movementData[HaltMovementIndex] == null) movementData[HaltMovementIndex] = new MovementVariable();
			movementData[HaltMovementIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Halt Movement", "If true the movement will not allow user to run off an edge. If false balanace animation will only show if velocity = 0"), movementData[HaltMovementIndex].BoolValue);
		
			// Number of Colliders
			if (movementData[NumberOfCollidersIndex] == null) movementData[NumberOfCollidersIndex] = new MovementVariable(DefaultColliders);
			movementData[NumberOfCollidersIndex].IntValue = EditorGUILayout.IntField(new GUIContent("Number of Colliders", "The number of colliders that should be hitting the ground in order for balance to be played."), movementData[NumberOfCollidersIndex].IntValue);
			if (movementData [NumberOfCollidersIndex].IntValue < 1) movementData [NumberOfCollidersIndex].IntValue = 1;
			return movementData;
		}

		#endregion

#endif
	}

}

