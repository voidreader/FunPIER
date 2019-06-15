#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Air movement which allows flying.
	/// </summary>
	public class AirMovement_Fly : AirMovement
	{
		
		#region members
		
		/// <summary>
		/// The max speed the character moves in the air.
		/// </summary>
		public Vector2 maxAirSpeed;
		
		/// <summary>
		/// Acceleration in X and Y.
		/// </summary>
		public Vector2 airAcceleration;
		
		/// <summary>
		/// Air drag in x and y (controls how quickly you stop).
		/// </summary>
		public Vector2 airDrag;

		/// <summary>
		/// If non null/empty this is the item required to fly.
		/// </summary>
		public string item;

		/// <summary>
		/// If item is present and this is bigger than zero then the item will be consumed like fule or energy.
		/// </summary>
		public int itemConsumptionRate;

		/// <summary>
		/// Cached copy of the item manager if this jet pack requires fuel.
		/// </summary>
		protected ItemManager itemManager;

		/// <summary>
		/// We still want to use fuel even if we use less than 1 fuel per frame. This is where we track it.
		/// </summary>
		protected float subIntFuelUsage;

		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Special/Fly";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Air movement which allows you to fly and hover.";
		
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
		/// The default max air speed.
		/// </summary>
		protected static Vector2 DefaultMaxAirSpeed = new Vector2(4, 3);
		
		/// <summary>
		/// The default air accelration.
		/// </summary>
		protected static Vector2 DefaultAirAcceleration = new Vector2(40, 40);

		/// <summary>
		/// The default air drag.
		/// </summary>
		protected static Vector2 DefaultAirDrag = new Vector2(10, 10);

		/// <summary>
		/// The index of the max air speed in the movement data.
		/// </summary>
		protected const int MaxAirSpeedIndex = 0;
		
		/// <summary>
		/// The index of the air acceleration in the movement data.
		/// </summary>
		protected const int AirAccelerationIndex = 1;

		/// <summary>
		/// The index of the air drag in the movement data.
		/// </summary>
		protected const int AirDragIndex = 2;

		/// <summary>
		/// The index of the item in the movement Data
		/// </summary>
		protected const int ItemIndex = 3;
		
		/// <summary>
		/// The index of the item consumption rate in the movement Data.
		/// </summary>
		protected const int ItemConsumptionRateIndex = 4;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 5;
		
		#endregion
		
		#region properties
		
		/// <summary>
		/// This class will handle gravity internally.
		/// </summary>
		override public bool ShouldApplyGravity
		{
			get
			{
				if (!CheckItems()) return true;
				return false;
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether this movement wants to intiate the jump.
		/// </summary>
		/// <value><c>true</c> if this instance should jump; otherwise, <c>false</c>.</value>
		override public bool WantsJump()
		{
			if (!enabled) return false;
			// Check input and items
			if (character.Input.VerticalAxisDigital > 0 && CheckItems())  return true;
			return false;
		}

		/// <summary>
		/// Gets a value indicating whether this movement wants to control the movement in the air.
		/// Default is false with movement falling back to default air. Override if you want control.
		/// </summary>
		/// <value>true</value>
		/// <c>false</c>
		/// <returns><c>true</c>, if air control was wantsed, <c>false</c> otherwise.</returns>
		override public bool WantsAirControl()
		{
			if (!enabled) return false;
			// Check input and items
			if (!character.Grounded && CheckItems())  return true;
			return false;
		}

		#endregion
		
		#region public methods
		
		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			MoveInX(character.Input.HorizontalAxis , character.Input.HorizontalAxisDigital, character.Input.RunButton);
			MoveInY();
			// Burn fuel 
			if (item != null && item != "" && itemConsumptionRate > 0)
			{
				// Track fuel usage less than 1.
				subIntFuelUsage += (float)itemConsumptionRate * TimeManager.FrameTime;
				if (subIntFuelUsage > 0.0f)
				{
					itemManager.ConsumeItem(item, (int) subIntFuelUsage);
					subIntFuelUsage -= (int)subIntFuelUsage;
				}
			}

		}
		
		/// <summary>
		/// Initialise this instance.
		/// </summary>
		override public Movement Init(Character character)
		{
			this.character = character;
			if (item != null && item != "")
			{
				itemManager = character.GetComponentInChildren<ItemManager>();
				if (itemManager == null) itemManager = character.GetComponentInChildren<ItemManager>();
				if (itemManager == null) Debug.LogError("A fly movement that burns fuel/energy requires an ItemManager");
			}
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
			
			// Set variables
			if (movementData != null && movementData.Length == MovementVariableCount)
			{
				maxAirSpeed = movementData[MaxAirSpeedIndex].Vector2Value;
				airAcceleration = movementData[AirAccelerationIndex].Vector2Value;
				airDrag = movementData[AirDragIndex].Vector2Value;
				item = movementData[ItemIndex].StringValue;
				itemConsumptionRate = movementData[ItemConsumptionRateIndex].IntValue;
			}
			else
			{
				Debug.LogError("Invalid movement data.");
			}
			if (item != null && item != "")
			{
				itemManager = character.GetComponentInChildren<ItemManager>();
				if (itemManager == null) itemManager = character.GetComponentInChildren<ItemManager>();
				if (itemManager == null) Debug.LogError("A fly movement that burns fuel/energy requires an ItemManager");
			}
			return this;
		}
		
		/// <summary>
		/// If the jump just started force control.
		/// </summary>
		override public bool WantsControl()
		{
			if (!enabled) return false;
			// If on the ground but up being pressed keep control
			if (character.Input.VerticalAxisDigital > 0 && character.Grounded) return true;
			return false;
		}
		
		/// <summary>
		/// Called when the movement loses control. Reset the jump count.
		/// </summary>
		override public void LosingControl()
		{

		}
		
		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				if (!CheckItems()) return AnimationState.FALL;
				return AnimationState.FLY;
			}
		}
		
		/// <summary>
		/// Gets the priority for the animation state.
		/// </summary>
		override public int AnimationPriority
		{
			get 
			{
				return 0;
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
		
		#endregion
		
		#region protected methods
		
		/// <summary>
		/// Does the X movement.
		/// </summary>
		override protected void MoveInX (float horizontalAxis, int horizontalAxisDigital, ButtonState runButton)
		{
			 if (character.Input.HorizontalAxisDigital == 0)
			{
				// Apply drag
				if (character.Velocity.x > 0) 
				{
					character.AddVelocity(-character.Velocity.x * airDrag.x * TimeManager.FrameTime, 0, false);
					if (character.Velocity.x < 0) character.SetVelocityX(0);
				}
				else if (character.Velocity.x < 0) 
				{
					character.AddVelocity(-character.Velocity.x * airDrag.x * TimeManager.FrameTime, 0, false);
					if (character.Velocity.x > 0) character.SetVelocityX(0);
				}
			}
			else
			{
				// Apply acceleration
				character.AddVelocity((float)character.Input.HorizontalAxisDigital * airAcceleration.x * TimeManager.FrameTime, 0, false);
				
				// Limit to max speed
				if (character.Velocity.x > maxAirSpeed.x) 
				{
					character.SetVelocityX(maxAirSpeed.x);
				}
				else if (character.Velocity.x < -maxAirSpeed.x) 
				{
					character.SetVelocityX(-maxAirSpeed.x);
				}
			}
			// Translate
			character.Translate(character.Velocity.x * TimeManager.FrameTime, 0, true);
		}
		
		/// <summary>
		/// Do the Y movement.
		/// </summary>
		override protected void MoveInY ()
		{
			if (!CheckItems())
			{
				// No fuel fall as normal - handed by gravity
				
			}
			else if (character.Input.VerticalAxisDigital == 0)
			{
				// Apply drag
				if (character.Velocity.y > 0) 
				{
					character.AddVelocity(0, -character.Velocity.y * airDrag.y * TimeManager.FrameTime, false);
					if (character.Velocity.y < 0) character.SetVelocityY(0);
				}
				else if (character.Velocity.y < 0) 
				{
					character.AddVelocity(0, -character.Velocity.y * airDrag.y * TimeManager.FrameTime, false);
					if (character.Velocity.y > 0) character.SetVelocityY(0);
				}
			}
			else
			{
				// Apply acceleration
				character.AddVelocity(0, (float)character.Input.VerticalAxisDigital * airAcceleration.y * TimeManager.FrameTime, false);
		
				// Limit to max speed
				if (character.Velocity.y > maxAirSpeed.y) 
				{
					character.SetVelocityY(maxAirSpeed.y);
				}
				else if (character.Velocity.y < -maxAirSpeed.y) 
				{
					character.SetVelocityY(-maxAirSpeed.y);
				}
			}

			// Translate
			character.Translate(0, character.Velocity.y * TimeManager.FrameTime, true);
		}
		
		/// <summary>
		///  Do the jump by translating and applying velocity.
		/// </summary>
		override public void DoJump()
		{
			// No need to do anything
		}
		
		/// <summary>
		/// Do the jump with overriden height and jumpCount.
		/// </summary>
		override public void DoOverridenJump(float newHeight, int newJumpCount)
		{
			if ( newJumpCount == 1 )
				RPGSoundManager.Instance.PlayEffectSound( 1 );
			else if ( newJumpCount == 2 )
				RPGSoundManager.Instance.PlayEffectSound( 2 );
			Debug.LogWarning("Fly movement does not support overriden jump");
		}

		/// <summary>
		/// Check that the required items  is present.
		/// </summary>
		/// <returns><c>true</c>, if items was checked, <c>false</c> otherwise.</returns>
		virtual protected bool CheckItems()
		{
			if (item == null || item == "") return true;
			if (itemManager.ItemCount (item) > 0) return true;
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
			
			// Max Air speed
			if (movementData[MaxAirSpeedIndex] == null) movementData[MaxAirSpeedIndex] = new MovementVariable(DefaultMaxAirSpeed);
			movementData[MaxAirSpeedIndex].Vector2Value = EditorGUILayout.Vector2Field(new GUIContent("Max Air Speed", "How fast the character can move in the air (x and y)."), movementData[MaxAirSpeedIndex].Vector2Value);

			// Acceleration
			if (movementData[AirAccelerationIndex] == null) movementData[AirAccelerationIndex] = new MovementVariable(DefaultAirAcceleration);
			movementData[AirAccelerationIndex].Vector2Value = EditorGUILayout.Vector2Field(new GUIContent("Acceleration", "How fast the character accelerates (x and y)."), movementData[AirAccelerationIndex].Vector2Value);

			// DragAir
			if (movementData[AirDragIndex] == null) movementData[AirDragIndex] = new MovementVariable(DefaultAirDrag);
			movementData[AirDragIndex].Vector2Value = EditorGUILayout.Vector2Field(new GUIContent("Drag", "Controls how fast the cahracter slows down (x and y)."), movementData[AirDragIndex].Vector2Value);

			// Item
			if (movementData[ItemIndex] == null) movementData[ItemIndex] = new MovementVariable();
			movementData[ItemIndex].StringValue = EditorGUILayout.TextField(new GUIContent("Item", "If not-empty the character requires the given item in order to be able to fly."), movementData[ItemIndex].StringValue);

			// Item consumption
			if (movementData[ItemConsumptionRateIndex] == null) movementData[ItemConsumptionRateIndex] = new MovementVariable(0);
			if (movementData[ItemIndex].StringValue != null && movementData[ItemIndex].StringValue != "")
			{
				// Item consumption rate
				movementData[ItemConsumptionRateIndex].IntValue = EditorGUILayout.IntField(new GUIContent("Consumption Rate", "If non zero the item will be treated like fuel or energy, and consumed at this rate per second."), movementData[ItemConsumptionRateIndex].IntValue);
				if (movementData[ItemConsumptionRateIndex].IntValue < 0) movementData[ItemConsumptionRateIndex].IntValue = 0;
			}

			return movementData;
		}
		
		#endregion
#endif
	}
	
}