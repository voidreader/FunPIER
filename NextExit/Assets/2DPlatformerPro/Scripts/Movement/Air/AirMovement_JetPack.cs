#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	///  Jet pack based based air movement.
	/// </summary>
	public class AirMovement_JetPack : AirMovement
	{
		
		#region members

		/// <summary>
		/// The accleration applied in y when the jetpack is active.
		/// </summary>
		[TaggedProperty ("agility")]
		public float jetpackAcceleration;

		/// <summary>
		/// How much fuel the jetpack uses per second.
		/// </summary>
		public float jetpackFuelConsumption;

		/// <summary>
		/// The id of a stackabe item used for fuel.
		/// </summary>
		public string fuelItemType;

		/// <summary>
		/// The (horizontal) speed the character moves at in the air.
		/// </summary>
		[TaggedProperty ("agility")]
		[TaggedProperty ("speedLimit")]
		public float airSpeed;

		/// <summary>
		/// The maximum speed that can be achieved in the y direction.
		/// </summary>
		[TaggedProperty ("agility")]
		public float maxSpeed;

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
		private const string Name = "Special/JetPack/Standard";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Air movement which handles a jetpack but not jump (you will also need another " +
										   " Air Movement to handle jump if jump is required).";
		
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
		/// The default jetpack acceleration.
		/// </summary>
		protected const float DefaultJetpackAcceleration = 45.0f;
		
		/// <summary>
		/// The default jetpack fuel consumption.
		/// </summary>
		protected const float DefaultJetpackFuelConsumption = 0.0f;

		/// <summary>
		/// The default air speed.
		/// </summary>
		protected const float DefaultAirSpeed = 5.0f;

		/// <summary>
		/// The default max speed.
		/// </summary>
		protected const float DefaultMaxSpeed = 5.0f;

		/// <summary>
		/// The index for the Jet Pack Velocity in the movement data.
		/// </summary>
		protected const int JetpackAcclerationIndex = 0;
		
		/// <summary>
		/// The index for the Jet Pack Fuel Consumption in the movement data.
		/// </summary>
		protected const int JetpackFuelConsumptionIndex = 1;

		/// <summary>
		/// The index for the Fuel Item Type in the movement data.
		/// </summary>
		protected const int FuelItemTypeIndex = 2;

		/// <summary>
		/// The index for the Air Speed in the movement data.
		/// </summary>
		protected const int AirSpeedIndex = 3;

		/// <summary>
		/// The index for the Max Speed in the movement data.
		/// </summary>
		protected const int MaxSpeedIndex = 4;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 5;
		
		#endregion
		
		#region properties
		
		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				if (IsJetPackEngaged())
				{
					return AnimationState.JETPACK;
				}
				else if (character.Velocity.y >= 0)
				{
					return AnimationState.AIRBORNE;
				}
				else
				{
					return AnimationState.FALL;
				}
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
		/// This class will handle gravity internally.
		/// </summary>
		override public bool ShouldApplyGravity
		{
			get
			{
				return false;
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
				return character.DefaultGravity;
			}
		}

		#endregion
		
		#region public methods

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
				jetpackAcceleration = movementData[JetpackAcclerationIndex].FloatValue;
				jetpackFuelConsumption = movementData[JetpackFuelConsumptionIndex].FloatValue;
				fuelItemType = movementData[FuelItemTypeIndex].StringValue;
				airSpeed = movementData[AirSpeedIndex].FloatValue;
				maxSpeed = movementData[MaxSpeedIndex].FloatValue;
			}
			else
			{
				Debug.LogError("Invalid movement data.");
			}

			if (jetpackFuelConsumption > 0.0f)
			{
				itemManager = character.GetComponentInChildren<ItemManager>();
				if (itemManager == null) itemManager = character.GetComponentInChildren<ItemManager>();
				if (itemManager == null) Debug.LogError("A Jetpack that burns fuel requires an ItemManager");
			}

			return this;
		}
	
		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			MoveInX(character.Input.HorizontalAxis , character.Input.HorizontalAxisDigital, character.Input.RunButton);
			MoveInY();
		}

		/// <summary>
		/// IF jetpack is engaged keep control
		/// </summary>
		override public bool WantsControl()
		{
			if (!enabled) return false;
			if (IsJetPackEngaged()) return true;
			return false;
		}

		
		/// <summary>
		/// IF jetpack is engaged take control
		/// </summary>
		override public bool WantsAirControl()
		{
			if (!enabled) return false;
			if (IsJetPackEngaged()) return true;
			if (character.Velocity.y > 0) return true;
			return false;
		}

		/// <summary>
		/// Is the jet pack engaged.
		/// </summary>
		/// <returns><c>true</c>, if pack engaged was jeted, <c>false</c> otherwise.</returns>
		virtual public bool IsJetPackEngaged()
		{
			if (jetpackFuelConsumption <= 0.0f || itemManager.ItemCount(fuelItemType) > 0)
			{
				if (character.Input.JumpButton == ButtonState.DOWN || 
				    character.Input.JumpButton == ButtonState.HELD) return true;
			}
			return false;
		}
		
		#endregion
		
		#region protected methods

		/// <summary>
		/// Do the X movement.
		/// </summary>
		override protected void MoveInX (float horizontalAxis, int horizontalAxisDigital, ButtonState runButton)
		{
			if (horizontalAxisDigital == 1)
			{
				character.SetVelocityX(airSpeed);
				character.Translate(airSpeed * TimeManager.FrameTime, 0, true);
			}
			else if (horizontalAxisDigital == -1)
			{
				character.SetVelocityX(-airSpeed);
				character.Translate(-airSpeed * TimeManager.FrameTime, 0, true);
			}
			else
			{
				character.SetVelocityX(0);
			}
		}
		
		/// <summary>
		/// Do the Y movement.
		/// </summary>
		override protected void MoveInY ()
		{
			// Apply gravity
			if (!character.Grounded)
			{
				character.AddVelocity(0, TimeManager.FrameTime * character.DefaultGravity, false);
			}

			if (IsJetPackEngaged())
			{
				// Apply jetpack thrust
				character.AddVelocity( 0, jetpackAcceleration * TimeManager.FrameTime, false);

				// Burn fuel 
				// Here we don't care if the exact amount is not present, a few rounding errors want hurt, but in a simulation game you
				// could set acceleration based on the actual amount of fuel burnt
				if (jetpackFuelConsumption > 0.0f)
				{
					// Track fuel usage less than 1.
					subIntFuelUsage += jetpackFuelConsumption * TimeManager.FrameTime;
					if (subIntFuelUsage > 0.0f)
					{
						itemManager.ConsumeItem(fuelItemType, (int) subIntFuelUsage);
						subIntFuelUsage -= (int)subIntFuelUsage;
					}
				}

				// Limit speed
				if (character.Velocity.y > maxSpeed) character.SetVelocityY(maxSpeed);


			}

			// Translate
			character.Translate(0, character.Velocity.y * TimeManager.FrameTime, true);
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

			// Air speed
			if (movementData[AirSpeedIndex] == null) movementData[AirSpeedIndex] = new MovementVariable(DefaultAirSpeed);
			movementData[AirSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Air Speed (x)", "How fast the character moves in the X direction whle in the air."), movementData[AirSpeedIndex].FloatValue);
			if (movementData[AirSpeedIndex].FloatValue < 0) movementData[AirSpeedIndex].FloatValue = 0.0f;

			// Jetpack Acceleration
			if (movementData[JetpackAcclerationIndex] == null) movementData[JetpackAcclerationIndex] = new MovementVariable(DefaultJetpackAcceleration);
			movementData[JetpackAcclerationIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Jetpack Acceleration", "The rate at which the jetpack accelerates the character."), movementData[JetpackAcclerationIndex].FloatValue);
			if (movementData[JetpackAcclerationIndex].FloatValue < 0) movementData[JetpackAcclerationIndex].FloatValue = 0.0f;

			// Max speed
			if (movementData[MaxSpeedIndex] == null) movementData[MaxSpeedIndex] = new MovementVariable(DefaultMaxSpeed);
			movementData[MaxSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Max Speed (y)", "Maximum y speed the jetpack can accelerate the character to."), movementData[MaxSpeedIndex].FloatValue);
			if (movementData[MaxSpeedIndex].FloatValue < 0) movementData[MaxSpeedIndex].FloatValue = 0.0f;

			// Jetpack Fuel Consumption
			if (movementData[JetpackFuelConsumptionIndex] == null) movementData[JetpackFuelConsumptionIndex] = new MovementVariable(DefaultJetpackFuelConsumption);
			movementData[JetpackFuelConsumptionIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Fuel Consumption", "How much fuel the jetpack uses per second, use 0 for unlimited use."), movementData[JetpackFuelConsumptionIndex].FloatValue);
			if (movementData[JetpackFuelConsumptionIndex].FloatValue < 0) movementData[JetpackFuelConsumptionIndex].FloatValue = 0.0f;

			// Jetpack Fuel Item Type
			if (movementData[JetpackFuelConsumptionIndex].FloatValue > 0.0f)
			{
				if (movementData[FuelItemTypeIndex] == null) movementData[FuelItemTypeIndex] = new MovementVariable();
				movementData[FuelItemTypeIndex].StringValue = EditorGUILayout.TextField(new GUIContent("Fuel Item Type", "The Item Type of the Item used for jetpack fuel."), movementData[FuelItemTypeIndex].StringValue);
			}

			return movementData;
		}
		
		#endregion

#endif

	}
	
}