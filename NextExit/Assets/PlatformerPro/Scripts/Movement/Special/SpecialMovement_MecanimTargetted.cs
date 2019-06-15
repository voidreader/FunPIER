#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A special movement which uses mecanim target. Typically for acrobatic moves.
	/// </summary>
	public class SpecialMovement_MecanimTargetted : SpecialMovement
	{

		#region members

		/// <summary>
		/// Do we need to push the jump button to start this movement.
		/// </summary>
		public bool requiresJumpButton;

		/// <summary>
		/// How fast  we need to be going before we can start this movement.
		/// </summary>
		public float requiredSpeed;

		/// <summary>
		/// Limb that will be targetted to the target position from the SpecialPlatform.
		/// </summary>
		public AvatarTarget targetLimb;

		/// <summary>
		/// Normalised time that the targetting starts.
		/// </summary>
		public float targetStartTime;

		/// <summary>
		/// Normalised time that the targetting ends.
		/// </summary>
		public float targetEndTime;

		/// <summary>
		/// Animation state that we will set when playing this special move.
		/// </summary>
		public AnimationState animationState;

		/// <summary>
		/// Cached copy of the target position.
		/// </summary>
		protected Vector3 targetPosition;

		/// <summary>
		/// Have we started the move?
		/// </summary>
		protected bool moveStarted;

		/// <summary>
		/// Cached reference to the animator.
		/// </summary>
		protected Animator myAnimator;

		/// <summary>
		/// Copy of the original value of the apply root motion variable. We will set state back to this on move completion.
		/// </summary>
		protected bool originalRootMotionState;

		/// <summary>
		/// Direction we were facing when move began.
		/// </summary>
		protected int facingDirection;

		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Mecanim/Targetted";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "A special movement which uses mecanim target. Typically for acrobatic moves.";
		
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
		/// The requires jump button index.
		/// </summary>
		protected const int RequiresJumpButtonIndex = 0;

		/// <summary>
		/// The index of the required speed.
		/// </summary>
		protected const int RequiredSpeedIndex = 1;

		/// <summary>
		/// The index of the target limb as int value.
		/// </summary>
		protected const int TargetLimbIndex = 2;

		/// <summary>
		/// The index of the target start time value.
		/// </summary>
		protected const int TargetStartTimeIndex = 3;

		/// <summary>
		/// The index of the target end time value.
		/// </summary>
		protected const int TargetEndTimeIndex = 4;

		/// <summary>
		/// The index of the animation as in value.
		/// </summary>
		protected const int AnimationAsIntIndex = 5;

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 6;

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
			myAnimator = GetComponentInChildren<Animator> ();
			if (myAnimator == null) Debug.LogWarning ("Mecanim driven special movement could not find an animator.");
			originalRootMotionState = myAnimator.applyRootMotion;
			if (movementData != null && movementData.Length >= MovementVariableCount)
			{
				requiresJumpButton = movementData[RequiresJumpButtonIndex].BoolValue;
				requiredSpeed = movementData[RequiredSpeedIndex].FloatValue;
				targetLimb = (AvatarTarget) movementData[TargetLimbIndex].IntValue;
				targetStartTime = movementData[TargetStartTimeIndex].FloatValue;
				targetEndTime = movementData[TargetEndTimeIndex].FloatValue;
				animationState = (AnimationState) movementData[AnimationAsIntIndex].IntValue;
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
				if (moveStarted) return animationState;
				return AnimationState.IDLE;
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
		/// Gets a value indicating whether this movement wants to do a special move.
		/// </summary>
		/// <value><c>true</c> if this instance wants control; otherwise, <c>false</c>.</value>
		override public bool WantsSpecialMove()
		{
			if (character.StoodOnPlatform is SpecialMovementPlatform) return CheckPlatform ((SpecialMovementPlatform) character.StoodOnPlatform);
			return false;
		}

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{

			// Check if animation is finished
			AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
			if (info.IsName(animationState.AsString()))
			{
				if( info.normalizedTime >= 1.0f)
				{
					moveStarted = false;
				}
			}
		}

		void LateUpdate()
		{
			if (moveStarted)
			{
				// Check if animation is finished
				AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
				if (info.IsName(animationState.AsString()) && info.normalizedTime < 1.0f)
				{
					ProcessMatchTarget ();
				}
			}
		}

		/// <summary>
		/// If this is true then the movement wants to maintain control of the character even
		/// if default transition conditions suggest it shouldn't.
		/// </summary>
		/// <returns><c>true</c> if control is wanted, <c>false</c> otherwise.</returns>
		override public bool ForceMaintainControl()
		{
			return moveStarted;
		}

		/// <summary>
		/// Start the special move.
		/// </summary>
		override public void DoSpecialMove()
		{
			// Shouldn't get this but lets check in case some subclass does something different
			if (character.StoodOnPlatform == null || !(character.StoodOnPlatform is SpecialMovementPlatform)) return;
			targetPosition = ((SpecialMovementPlatform)character.StoodOnPlatform).targetPosition;
			moveStarted = true;
			myAnimator.applyRootMotion = true;
			// No residual velocity for mecanim targetted movements
			character.SetVelocityX(0);
			character.SetVelocityY(0);
		}

		override public void LosingControl()
		{
			moveStarted = false;
			myAnimator.applyRootMotion = originalRootMotionState;
		}

		/// <summary>
		/// Shoulds the apply gravity.
		/// </summary>
		/// <returns><c>true</c>, if apply gravity was shoulded, <c>false</c> otherwise.</returns>
		override public bool ShouldApplyGravity
		{
			get 
			{
				return false;
			}
		}

		/// <summary>
		/// Gets the should do base collisions.
		/// </summary>
		/// <value>The should do base collisions.</value>
		override public RaycastType ShouldDoBaseCollisions
		{
			get
			{
				return RaycastType.NONE;
			}
		}

		#endregion

		/// <summary>
		/// Check if we should do a move based on the special platform.
		/// </summary>
		/// <returns><c>true</c>, if platform was checked, <c>false</c> otherwise.</returns>
		/// <param name="platform">Platform.</param>
		virtual protected bool CheckPlatform(SpecialMovementPlatform platform)
		{
			if (Mathf.Abs (character.Velocity.x) < requiredSpeed) return false;
			if (requiresJumpButton && character.Input.JumpButton != ButtonState.DOWN) return false;
			if (platform.movement != animationState) return false;
			if (character.FacingDirection != platform.requiredFacingDirection) return false;
			return true;
		}

		virtual protected void ProcessMatchTarget()
		{
			float weight = 0;
			AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
			if (info.normalizedTime > targetEndTime) return;
			weight = (info.normalizedTime - targetStartTime) / (targetEndTime - targetStartTime);
			if (weight > 0)
			{
				myAnimator.MatchTarget(targetPosition,
				                       myAnimator.transform.rotation, 
				                       targetLimb, 
				                       new MatchTargetWeightMask(Vector3.one, 0), 
				                       targetStartTime,
				                       targetEndTime);
			}
		}
			
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
			// Target Limb
			if (movementData[AnimationAsIntIndex] == null) movementData[AnimationAsIntIndex] = new MovementVariable(3000);
			movementData[AnimationAsIntIndex].IntValue = (int)(AnimationState) EditorGUILayout.EnumPopup(new GUIContent("Movement", "The Animation State which also serves as the movement id."), (AnimationState)movementData[AnimationAsIntIndex].IntValue);

			// Requires Jump Button
			if (movementData[RequiresJumpButtonIndex] == null) movementData[RequiresJumpButtonIndex] = new MovementVariable();
			movementData[RequiresJumpButtonIndex].BoolValue = EditorGUILayout.Toggle(new GUIContent("Requires Jump", "Does the player need to press jump to trigger this action?"), movementData[RequiresJumpButtonIndex].BoolValue);

			// Required Speed
			if (movementData[RequiredSpeedIndex] == null) movementData[RequiredSpeedIndex] = new MovementVariable();
			movementData[RequiredSpeedIndex].FloatValue = EditorGUILayout.FloatField(new GUIContent("Required Speed", "How fast the character needs to be moving in X before this move is triggered."), movementData[RequiredSpeedIndex].FloatValue);

			// Target Limb
			if (movementData[TargetLimbIndex] == null) movementData[TargetLimbIndex] = new MovementVariable();
			movementData[TargetLimbIndex].IntValue = (int)(AvatarTarget) EditorGUILayout.EnumPopup(new GUIContent("Target Limb", "Which limb do we target to the target position?"), (AvatarTarget)movementData[TargetLimbIndex].IntValue);

			// Min/Max targetting
			if (movementData[TargetStartTimeIndex] == null) movementData[TargetStartTimeIndex] = new MovementVariable();
			if (movementData[TargetEndTimeIndex] == null) movementData[TargetEndTimeIndex] = new MovementVariable();
			float min = movementData [TargetStartTimeIndex].FloatValue;
			float max = movementData [TargetEndTimeIndex].FloatValue;
			EditorGUILayout.MinMaxSlider(new GUIContent("Targeting Timespan", "When does the targetting start and stop?"), ref min, ref max, 0, 1.0f);
		
			if (max <= min + 0.01f) max = min + 0.01f;
			if (min != movementData [TargetStartTimeIndex].FloatValue || max != movementData [TargetEndTimeIndex].FloatValue)
			{
				movementData [TargetStartTimeIndex].FloatValue = min;
				movementData [TargetEndTimeIndex].FloatValue = max;
				EditorUtility.SetDirty(target);
			}

			return movementData;
		}

		#endregion

#endif
	}

}

