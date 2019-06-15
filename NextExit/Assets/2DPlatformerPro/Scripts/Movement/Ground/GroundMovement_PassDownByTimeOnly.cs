#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Ground movement_ digital that does a pass down by time.
	/// </summary>
	public class GroundMovement_PassDownByTimeOnly : GroundMovement
	{

		#region members

		/// <summary>
		/// If false press down to passthrough, if true press down + jump to pass through.
		/// </summary>
		public bool requireJump;

		/// <summary>
		/// How high to bobble upwards before passing through downwards.
		/// </summary>
		public float bobbleHeight;

		/// <summary>
		/// How long to wait before giving back control to normal movements.
		/// </summary>
		public float clearanceTime;

		/// <summary>
		/// The clearance timer.
		/// </summary>
		protected float clearanceTimer;

		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Special/Pass Downwards By Time";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Ground movement which allows you to pass downwards through a passthrough multiple platform using a simple timer. Generally its better to use GroundMovement_Passdown.";
		
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
		/// The index of the require jump in the movement data.
		/// </summary>
		protected const int RequireJumpIndex = 0;

		/// <summary>
		/// The index of the bobble height in the movement data.
		/// </summary>
		protected const int BobbleHeightIndex = 1;

		/// <summary>
		/// The index of the clearance time in the movement data.
		/// </summary>
		protected const int ClearanceTimeIndex = 2;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 3;

		/// <summary>
		/// The default speed.
		/// </summary>
		protected const float DefaultClearanceTime = 0.33f;

		#endregion

		#region properties

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				return character.DefaultAirMovement.AnimationState;
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
				return character.Input.HorizontalAxisDigital;
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether this <see cref="PlatformerPro.Movement"/> expects
		/// gravity to be applied after its movement finishes.
		/// </summary>
		override public bool ShouldApplyGravity
		{
			get
			{
				return character.DefaultAirMovement.ShouldApplyGravity;
			}
		}
		
		/// <summary>
		/// Gets a value indicating the current gravity, only used if this
		/// movement doesn't apply the default gravity.
		/// <seealso cref="ShouldApplyGravity()"/>
		/// </summary>
		override public float CurrentGravity
		{
			get
			{
				return character.DefaultAirMovement.CurrentGravity;
			}
		}
		
		
		/// <summary>
		/// Gets a value indicating whether this <see cref="PlatformerPro.Movement"/> expects the
		/// base collisions to be executed after its movement finishes.
		/// </summary>
		override public RaycastType ShouldDoBaseCollisions
		{
			get
			{
				return RaycastType.SIDES;
			}
		}

		#endregion

		#region Unity hooks

		/// <summary>
		/// Unity Update hook
		/// </summary>
		void Update()
		{
			if (clearanceTimer >0) clearanceTimer -= TimeManager.FrameTime;
		}

		#endregion

		#region public methods

		/// <summary>
		/// Gets a value indicating whether this movement wants to control the movement on the ground.
		/// Default is false, with control falling back to default ground. Override if you want particular control.
		/// </summary>
		override public bool WantsGroundControl()
		{
			if (!enabled) return false;
#if UNITY_EDITOR
			if (character.DefaultGroundMovement == this)
			{
				Debug.LogWarning("Pass Down movement cannot be the the default ground movement. " +
				                 "To fix ensure you have another GroundMovement added and make this movement higher in the movement list.");
				return false;
			}
#endif
			// Only on a passthrough
			if ((1 << character.GroundLayer & character.passthroughLayerMask) != 1 << character.GroundLayer) return false;
			// Check Input
			if ((!requireJump && character.Input.VerticalAxisState == ButtonState.DOWN) || 
			    ( requireJump && character.Input.VerticalAxisDigital == -1 && character.Input.JumpButton == ButtonState.DOWN)) return true;
		    return false;
		}

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			// Let the default air movement move us
			character.DefaultAirMovement.DoOverridenMove (true, true, character.Input.HorizontalAxis, character.Input.RunButton);

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

			if (movementData != null && movementData.Length >= MovementVariableCount)
			{
				requireJump = movementData[RequireJumpIndex].BoolValue;
				bobbleHeight = movementData[BobbleHeightIndex].FloatValue;
				clearanceTime = movementData[ClearanceTimeIndex].FloatValue;
			}
			else
			{
				Debug.LogWarning("Invalid movement data.");
			}
			return this;
		}

		/// <summary>
		/// Called when the movement gets control. Typically used to do initialisation of velocity and the like.
		/// </summary>
		override public void GainControl()
		{
			if (bobbleHeight > 0) character.DefaultAirMovement.DoOverridenJump (bobbleHeight, 2);
			clearanceTimer = clearanceTime;
		}

		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions.
		/// </summary>
		override public void LosingControl()
		{
			clearanceTimer = 0;
		}

		/// <summary>
		/// If this is true then the movement wants to maintain control of the character even
		/// if default transition conditions suggest it shouldn't.
		/// </summary>
		override public bool WantsControl()
		{
			if (clearanceTimer > 0) return true;
			return false;
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

			// Require Jump
			if (movementData[RequireJumpIndex] == null) movementData[RequireJumpIndex] = new MovementVariable();
			movementData[RequireJumpIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Use Jump Button", "If false, pressing downwards will passthrough, if true you need to press both down and jump."), movementData[RequireJumpIndex].BoolValue);

			// Bobble Height
			if (movementData[BobbleHeightIndex] == null) movementData[BobbleHeightIndex] = new MovementVariable(0);
			movementData[BobbleHeightIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Bobble Height", "How high should the character bobble before moving downwards."), movementData[BobbleHeightIndex].FloatValue);
			if (movementData[BobbleHeightIndex].FloatValue < 0) movementData[BobbleHeightIndex].FloatValue = 0.0f;

			// Clearance Timer
			if (movementData[ClearanceTimeIndex] == null) movementData[ClearanceTimeIndex] = new MovementVariable(DefaultClearanceTime);
			movementData[ClearanceTimeIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Clearance Time", "How long should this movement maintain control before giving it back to the normal air movement."), movementData[ClearanceTimeIndex].FloatValue);
			if (movementData[ClearanceTimeIndex].FloatValue < 0) movementData[ClearanceTimeIndex].FloatValue = 0.0f;


			return movementData;
		}

		#endregion

#endif
	}

}

