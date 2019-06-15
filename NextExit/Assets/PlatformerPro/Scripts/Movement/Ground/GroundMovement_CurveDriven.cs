#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Ground movement which sets x speed based on an animation curve and its relationship to animator normalised time.
	/// </summary>
	public class GroundMovement_CurveDriven : GroundMovement_Digital, IFlippableGravityMovement
	{

		#region members

		/// <summary>
		/// The curve defining the speed the character moves at.
		/// </summary>
		public AnimationCurve speedCurve;

		/// <summary>
		/// If true find an animator and use the normalised time of the animation to determine the speed.
		/// </summary>
		public bool useAnimationTime;

		/// <summary>
		/// If we aren't using animation time then what time scale should each loop represent.
		/// </summary>
		public float loopTime;

		/// <summary>
		/// The loop timer.
		/// </summary>
		protected float loopTimer;

		/// <summary>
		/// Animator reference.
		/// </summary>
		protected Animator myAnimator;

		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Special/Curve Driven";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Ground movement which sets x speed based on an animation curve and its relationship to animator normalised time.";
		
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
		/// The index for the speed value in the movement data.
		/// </summary>
		protected const int SpeedCurveIndex = 0;

		/// <summary>
		/// The index for the speed value in the movement data.
		/// </summary>
		protected const int UseAnimationTimeIndex = 1;

		/// <summary>
		/// The index of the loop time in the movement data.
		/// </summary>
		protected const int LoopTimeIndex = 2;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 3;

		#endregion



		#region public methods

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			float curveTime = 0.0f;
			if (!useAnimationTime)
			{
				loopTimer += TimeManager.FrameTime;
				if (loopTimer > loopTime) loopTimer -= loopTime;
				curveTime = (loopTimer / loopTime);
			}
			else
			{
				// Get normalised time from animator, but only if its playing walk animation
				AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
				if (AnimationState == AnimationState.WALK && info.IsName(AnimationState.WALK.AsString()))
				{
					curveTime = info.normalizedTime % 1.0f;
				}
				else
				{
					curveTime = 0.0f;
				}
			}
			// Set frame speed - if friction is bigger than 2 we will slow the character down.
			float frameSpeed = speedCurve.Evaluate (curveTime);

			if (character.Friction > 2.0f) frameSpeed *= (2.0f / character.Friction );
#if UNITY_EDITOR
			if (Character.Friction >= 0 && Character.Friction < 2.0f) Debug.LogError("A friction less than 2 has no affect on digitial movements.");
#endif

			if (character.Input.HorizontalAxisDigital == 1)
			{
				character.SetVelocityX(character.IsGravityFlipped ? -frameSpeed : frameSpeed);
				character.Translate((character.IsGravityFlipped ? -frameSpeed : frameSpeed) * TimeManager.FrameTime, 0, false);
			}
			else if (character.Input.HorizontalAxisDigital == -1)
			{
				character.SetVelocityX(character.IsGravityFlipped ? frameSpeed : -frameSpeed);
				character.Translate((character.IsGravityFlipped ? frameSpeed : -frameSpeed) * TimeManager.FrameTime, 0, false);
			}
			else
			{
				character.SetVelocityX(0);
			}
		}

		
		/// <summary>
		/// Does any movement that MUST be done after collissions are calculated.
		/// </summary>
		override public void PostCollisionDoMove() {
			if (enabled && !character.rotateToSlopes) SnapToGround ();
		}

		/// <summary>
		/// Initialise the movement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			AssignReferences (character);

			if (movementData != null && movementData.Length >= MovementVariableCount)
			{
				speedCurve = movementData[SpeedCurveIndex].CurveValue;
				useAnimationTime = movementData[UseAnimationTimeIndex].BoolValue;
				if (!useAnimationTime) loopTime = movementData[LoopTimeIndex].FloatValue;
			}
			else
			{
				Debug.LogError("Invalid movement data");
			}

			if (useAnimationTime)
			{
				myAnimator = character.gameObject.GetComponentInChildren<Animator> ();
				if (myAnimator == null) Debug.LogWarning("Trying to sync with animator time but no animator could be found.");
			}

			return this;
		}




		#endregion

#if UNITY_EDITOR

		#region draw inspector

		/// <summary>
		/// Draws the inspector.
		/// </summary>
		new public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{
			if (movementData == null || movementData.Length < MovementVariableCount)
			{
				movementData = new MovementVariable[MovementVariableCount];
			}

			// Walk speed curve
			if (movementData[SpeedCurveIndex] == null || movementData[SpeedCurveIndex].CurveValue == null) movementData[SpeedCurveIndex] = new MovementVariable(new AnimationCurve());
			movementData[SpeedCurveIndex].CurveValue = EditorGUILayout.CurveField(new GUIContent("Speed Curve", "Curve defining the cahracters x speed."), movementData[SpeedCurveIndex].CurveValue);

			// Use animation time
			if (movementData[UseAnimationTimeIndex] == null) movementData[UseAnimationTimeIndex] = new MovementVariable();
			movementData[UseAnimationTimeIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Use Animation Time", "Should we sync the curve position with the normalised time of the animation?"), movementData[UseAnimationTimeIndex].BoolValue);

			// Loop time
			if (movementData[LoopTimeIndex] == null) movementData[LoopTimeIndex] = new MovementVariable(1.0f);
			if (!movementData[UseAnimationTimeIndex].BoolValue)
			{
				movementData[LoopTimeIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Loop Time", "How long is each loop?"), movementData[LoopTimeIndex].FloatValue);
				if (movementData[LoopTimeIndex].FloatValue <= 0) movementData[LoopTimeIndex].FloatValue = 1.0f;
			}

			return movementData;
		}

		#endregion

#endif
	}

}

