#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Ground movement which allows character to push things.
	/// </summary>
	public class GroundMovement_Push : GroundMovement
	{
		
		#region members
		
		/// <summary>
		/// Speed adjustment factor.
		/// </summary>
		public float speedAdjustment;

		/// <summary>
		/// Do we push the character using force? If false we just use transform.
		/// </summary>
		public bool pushAsForce;

		/// <summary>
		/// How much can the character push.
		/// </summary>
		public float maxWeight;

		/// <summary>
		/// Should we consider object weight when adjusting speed.
		/// </summary>
		public bool weightAffectsSpeed;

		/// <summary>
		/// The current thing being pushed.
		/// </summary>
		protected Pushable pushable;

		/// <summary>
		/// How far is the pushable offset when the push starts.
		/// </summary>
		protected float initialPushOffset;

		/// <summary>
		/// The direction we are pushing.
		/// </summary>
		protected float pushDirection;

		#endregion
		
		#region constants

		/// <summary>
		/// Don't allow pushing if moving in Y faster than this.
		/// </summary>
		protected const float MaxYVelocity = 0.525f;

		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Special/Simple Push";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Ground movement which allows character to push things. This should not be the default movements be should instead used in conjunction with another ground movement.";
		
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
		/// The index of the speed adjustment.
		/// </summary>
		protected const int SpeedAdjustmentIndex = 0;

		/// <summary>
		/// The index of the max weight.
		/// </summary>
		protected const int MaxWeightIndex = 1;

		/// <summary>
		/// The index of weight affects speed.
		/// </summary>
		protected const int WeightAffectsSpeedIndex = 2;

		/// <summary>
		/// The index of push as force.
		/// </summary>
		protected const int PushAsForceIndex = 3;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 4;
		
		#endregion

		#region public methods
		
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
				speedAdjustment = movementData[SpeedAdjustmentIndex].FloatValue;
				maxWeight = movementData[MaxWeightIndex].FloatValue;
				weightAffectsSpeed = movementData[WeightAffectsSpeedIndex].BoolValue;
				pushAsForce = movementData[PushAsForceIndex].BoolValue;
			}
			else
			{
				Debug.LogError("Invalid movement data for Ground Movement Push");
			}

			return this;
		}

		/// <summary>
		/// Gets a value indicating whether this movement wants to control the movement on the ground.
		/// This movement wants control if the side colliders are pushing against a box.
		/// </summary>
		/// <value><c>true</c> if this instance wants control; otherwise, <c>false</c>.</value>
		override public bool WantsGroundControl()
		{
			if (!enabled) return false;
			if (character.DefaultGroundMovement == this)
			{
				Debug.LogError("The Push movement can't be the default ground movement. Disabling movement.");
				enabled = false;
			}

			// Early out, only push if grounded.
			if (!character.Grounded) return false;

			// Early out, only push if pressing in a direction.
			if ( character.Input.HorizontalAxisDigital == 0) return false;

			// Early out, if last frame we were pusing one way, we want to lose control for at least one frame before pushing another way
			if (pushDirection == -character.Input.HorizontalAxisDigital) return false;

			int hitCount = 0;
			Pushable matchingPushable = null;
			float pushableDistance = 1;
			float nonPushableDistance = 1;
			RaycastType typeToCheck = (character.Input.HorizontalAxisDigital == 1 ? RaycastType.SIDE_RIGHT : RaycastType.SIDE_LEFT);
			initialPushOffset = 0;
			for (int i = 0; i < character.Colliders.Length; i++)
			{
				if (character.Colliders[i].RaycastType == typeToCheck)
				{
					RaycastHit2D hit = character.GetClosestCollision(i);
					if (hit.collider != null)
					{
						Pushable currentPushable = hit.collider.GetComponent<Pushable>();
						// Pushing against something that isn't pushable TODO add distance checks.
						if (currentPushable == null) 
						{
							if (hit.fraction < nonPushableDistance) nonPushableDistance = hit.fraction;
						}
						else
						{
							if (currentPushable != matchingPushable)
							{
								// Found a closer pushable (or if matching pushable is null we just found a pushable)
								if (hit.fraction < pushableDistance) 
								{
									pushableDistance = hit.fraction;
									matchingPushable = currentPushable;
									initialPushOffset = (hit.fraction * (character.Colliders[i].Length + character.Colliders[i].LookAhead)) - character.Colliders[i].Length - character.Colliders[i].LookAhead;
									hitCount = 1;
								}
							}
							else
							{
								hitCount++;
							}
						}
					}
				}
			}

			if (matchingPushable != null && pushableDistance < nonPushableDistance && initialPushOffset < 0 ) 
			{
				// If the block has started falling don't allow it to be pushed.
				if (Mathf.Abs (matchingPushable.GetComponent<Rigidbody2D>().velocity.y) >= MaxYVelocity) return false;
				pushable = matchingPushable;
				initialPushOffset *= -character.Input.HorizontalAxisDigital;
				pushDirection = character.Input.HorizontalAxisDigital;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			if (enabled)
			{
				character.DefaultGroundMovement.DoMove ();
				if (pushAsForce)
				{
					// We assume here that the ground movement uses a relative X speed
					pushable.Push (character, Quaternion.Euler(0,0, -character.SlopeTargetRotation) *  new Vector2(character.Velocity.x, 0) * speedAdjustment, pushAsForce);
				}
				else
				{
					float adjustment = speedAdjustment;
					if (weightAffectsSpeed) 
					{
						adjustment /= Mathf.Log10(pushable.Mass * 10);
					}
					pushable.Push (character, Quaternion.Euler(0,0, -character.SlopeTargetRotation) *  new Vector2(character.Velocity.x, 0) * adjustment * TimeManager.FrameTime, pushAsForce);
				}
				
#if UNITY_EDITOR
				if (character.DefaultGroundMovement.VelocityType != VelocityType.RELATIVE_X_WORLD_Y && character.DefaultGroundMovement.VelocityType != VelocityType.RELATIVE_X_WORLD_Y) Debug.LogWarning("Push movement expects ground movement to have a relative x velocity");
#endif
			}
		}

		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions.
		/// </summary>
		override public void LosingControl()
		{
			pushDirection = 0;
			pushable = null;
			// Because we use another movement to do the movement we call LosingControl on it too to ensure any values are reset there too
			if (enabled) character.DefaultGroundMovement.LosingControl ();
		}

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				return AnimationState.PUSH;
			}
		}

		// <summary>
		/// Returns the direction the character is facing. 0 for none, 1 for right, -1 for left.
		/// This overriden version always returns the default ground movements facing direction.
		/// </summary>
		override public int FacingDirection
		{
			get 
			{
				if (character.DefaultGroundMovement != this) return character.DefaultGroundMovement.FacingDirection;
				return 0;
			}
		}

		/// <summary>
		/// Don't allow base collisions to reset X velocity.
		/// </summary>
		override public bool PreventXVelocityReset
		{
			get
			{
				return true;
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

			// Push as force
			if (movementData[PushAsForceIndex] == null) movementData[PushAsForceIndex] = new MovementVariable(false);
			movementData[PushAsForceIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Push with Physics2D", "If true we push the pushable by applying a force. If false we translate it directly."), movementData[PushAsForceIndex].BoolValue);

			if (!movementData[PushAsForceIndex].BoolValue)
			{
				// Speed Adjustment Factor
				if (movementData[SpeedAdjustmentIndex] == null) movementData[SpeedAdjustmentIndex] = new MovementVariable(0.7f);
				movementData[SpeedAdjustmentIndex].FloatValue = EditorGUILayout.Slider(new GUIContent("Speed Adjustment", "To what extent does pushing affect the character speed. 1 for no effect."), movementData[SpeedAdjustmentIndex].FloatValue, 0, 1);
				if (movementData[SpeedAdjustmentIndex].FloatValue < 0) movementData[SpeedAdjustmentIndex].FloatValue = 0.0f;
				if (movementData[SpeedAdjustmentIndex].FloatValue > 1) movementData[SpeedAdjustmentIndex].FloatValue = 0.0f;

				// Max weight
				if (movementData[MaxWeightIndex] == null) movementData[MaxWeightIndex] = new MovementVariable(100.0f);
				movementData[MaxWeightIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Max Weight", "The maximum weight the character can push."), movementData[MaxWeightIndex].FloatValue);
				if (movementData[MaxWeightIndex].FloatValue < 0) movementData[MaxWeightIndex].FloatValue = 0.0f;

				// Weight affects speed
				if (movementData[WeightAffectsSpeedIndex] == null) movementData[WeightAffectsSpeedIndex] = new MovementVariable(false);
				movementData[WeightAffectsSpeedIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Weigh Affects Speed", "If true the weight will have an affect on characters speed."), movementData[WeightAffectsSpeedIndex].BoolValue);
				if (movementData[MaxWeightIndex].FloatValue < 0) movementData[MaxWeightIndex].FloatValue = 0.0f;
			}
			else
			{
				// Speed Adjustment Factor
				if (movementData[SpeedAdjustmentIndex] == null) movementData[SpeedAdjustmentIndex] = new MovementVariable(0.7f);
				movementData[SpeedAdjustmentIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Force Adjustment", "Characters weight will be this value for the purpose of force calculation."), movementData[SpeedAdjustmentIndex].FloatValue);
				if (movementData[SpeedAdjustmentIndex].FloatValue < 0) movementData[SpeedAdjustmentIndex].FloatValue = 0.0f;

			}
			return movementData;
		}
		
		#endregion
		
#endif

	}
}