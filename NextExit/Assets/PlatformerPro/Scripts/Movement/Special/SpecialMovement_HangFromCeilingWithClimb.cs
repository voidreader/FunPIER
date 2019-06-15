#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Hang from the ceiling on collision.
	/// </summary>
	public class SpecialMovement_HangFromCeilingWithClimb : SpecialMovement_HangFromCeiling
	{

		/// <summary>
		/// Layers we can climb horizontally on.
		/// </summary>
		public int horizontalClimbLayers;

		/// <summary>
		/// The speed we climb across the ceiling.
		/// </summary>
		public float horizontalClimbSpeed;

		/// <summary>
		/// Can we climb off the end of the ceiling?
		/// </summary>
		public bool dismountOnExtent;

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Ceiling Hang with Horizontal Climb";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Hang from the ceiling on collision. CLimb horizontally on specified layers.";
		
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
		/// The index of the horizontal climb speed in the movement data.
		/// </summary>
		protected const int HorizontalClimbLayersIndex = 8;

		/// <summary>
		/// The index of the horizontal climb speed.
		/// </summary>
		protected const int HorizontalClimbSpeedIndex = 9;


		/// <summary>
		/// The index of dimount on extent.
		/// </summary>
		protected const int DismountOnExtentIndex = 10;

		/// <summary>
		/// The size of the movement variable array in the movement data.
		/// </summary>
		private const int MovementVariableCount = 11;

		#endregion

		/// <summary>
		/// Initialise the movement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		/// <param name="ignored">Ignored.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			base.Init (character, movementData);
			if (movementData != null && movementData.Length >= MovementVariableCount)
			{
				horizontalClimbLayers = movementData [HorizontalClimbLayersIndex].IntValue;
				horizontalClimbSpeed = movementData [HorizontalClimbSpeedIndex].FloatValue;
				dismountOnExtent = movementData [DismountOnExtentIndex].BoolValue;
			}
			return Init (character);
		}

		/// <summary>
		/// Gets a value indicating whether this movement wants to do a special move.
		/// </summary>
		/// <returns><c>true</c>, if special move was wanted, <c>false</c> otherwise.</returns>
		override public bool WantsSpecialMove()
		{
			if (fallTimer > 0) return false;
			if (state == CeilingHangState.DONE) return false;
			if (state == CeilingHangState.NONE)
			{
				// TODO Reevalaute these conditions
				if ( CheckHeadCollisions())
				{
					if (hangableLayers == 0 ||
					    ((1 << (currentHeadCollider.gameObject.layer)) & (int) hangableLayers) == ((1 << currentHeadCollider.gameObject.layer)))
					{	
						return true;
					}
				}
				return false;
			}
			
			else if (state == CeilingHangState.HANG)
			{
				// Down to release
				if (character.Input.VerticalAxisDigital == -1) 
				{
					fallTimer = fallTime;
					state = CeilingHangState.DROP;
					return false;
				}
				// Up to climb (second condition not required but makes it easier to understand)
				if (canClimb &&
				    character.Input.VerticalAxisState == ButtonState.DOWN && 
				    character.Input.VerticalAxisDigital == 1 &&
				    (climbableLayers == 0 || ((1 << (currentHeadCollider.gameObject.layer)) & (int) climbableLayers) == ((1 << currentHeadCollider.gameObject.layer))))
				{		
					state = CeilingHangState.CLIMB_START;
				}
				
			}
			return true;
		}

		/// <summary>
		/// Start the special mvoe
		/// </summary>
		override public void DoMove()
		{

			if (horizontalClimbSpeed > 0 && character.Input.HorizontalAxisDigital != 0 && state == CeilingHangState.HANG && 
			    (horizontalClimbLayers == 0 || ((1 << (currentHeadCollider.gameObject.layer)) & (int) horizontalClimbLayers) == (1 << currentHeadCollider.gameObject.layer)))
			{
				character.Translate(((float)character.Input.HorizontalAxisDigital) * horizontalClimbSpeed * TimeManager.FrameTime, 0, true);
				character.SetVelocityX(((float)character.Input.HorizontalAxisDigital) * horizontalClimbSpeed);
				character.DoBaseCollisionsForRaycastType(RaycastType.HEAD);
				if (!CheckHeadCollisions())
				{
					if (dismountOnExtent)
					{
						fallTimer = fallTime;
						state = CeilingHangState.DROP;
					}
					else
					{
						character.Translate( ((float)character.Input.HorizontalAxisDigital) * -horizontalClimbSpeed * TimeManager.FrameTime, 0, true);
						character.SetVelocityX(0);
					}
				}
			}
		}

		/// <summary>
		/// Called when the movement loses control.
		/// </summary>
		override public void LosingControl()
		{
			state = CeilingHangState.NONE;
		}

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				if (state == CeilingHangState.CLIMB_START) return AnimationState.CEILING_CLIMB;
				if (state == CeilingHangState.CLIMBING) return AnimationState.CEILING_CLIMB;
				if (state == CeilingHangState.LAUNCH_START) return AnimationState.CEILING_CLIMB_LAUNCH;
				if (state == CeilingHangState.LAUNCH) return AnimationState.CEILING_CLIMB_LAUNCH;
				if (state == CeilingHangState.HANG && character.Velocity.x != 0) return AnimationState.CEILING_CLIMB_HORIZONTAL;
				return AnimationState.CEILING_HANG;
			}
		}


#if UNITY_EDITOR

		/// <summary>
		/// Draws the inspector.
		/// </summary>
		new public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{
			if (movementData == null || movementData.Length < MovementVariableCount)
			{
				movementData = new MovementVariable[MovementVariableCount];
			}

			// Draw base inspector and copy values
			MovementVariable[] baseMovementData = SpecialMovement_HangFromCeiling.DrawInspector (movementData, ref showDetails, target);
			System.Array.Copy (baseMovementData, movementData, baseMovementData.Length);

			// Horizontal Climb Speed
			if (movementData [HorizontalClimbSpeedIndex] == null) movementData [HorizontalClimbSpeedIndex] = new MovementVariable ();
			movementData [HorizontalClimbSpeedIndex].FloatValue = EditorGUILayout.FloatField (new GUIContent ("Horizontal Climb Speed", "Sepeed at which we climb horizontally."), movementData [HorizontalClimbSpeedIndex].FloatValue);
			if  (movementData [HorizontalClimbSpeedIndex].FloatValue < 0) movementData [HorizontalClimbSpeedIndex].FloatValue  = 0;

			string[] layerNames = GenerateLayerNames ();


			// Climbable layers
			if (movementData [HorizontalClimbLayersIndex] == null) movementData [HorizontalClimbLayersIndex] = new MovementVariable ();
			movementData [HorizontalClimbLayersIndex].IntValue = EditorGUILayout.MaskField(new GUIContent ("Horizontal Climb Layers", "Layers we can climb across on."), 
			                                                                               movementData [HorizontalClimbLayersIndex].IntValue,
			                                                                         	   layerNames);
			
			// Dismount on Extent
			if (movementData [DismountOnExtentIndex] == null) movementData [DismountOnExtentIndex] = new MovementVariable ();
			movementData [DismountOnExtentIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent ("Dismount on Extent ", "Should we dismounnt from the ceiling when we reach the edge."), 
			                                                                        movementData [DismountOnExtentIndex].BoolValue);

			return movementData;
		}

#endif
	}

}
