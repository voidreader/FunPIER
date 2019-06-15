#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A damage movement which plays an animation based on the damage type.
	/// </summary>
	public class DamageMovement_AnimationPerDamageType : DamageMovement
	{
		
		#region members

		/// <summary>
		/// The damage animations mappings.
		/// </summary>
		public List<DamageTypeToDamageAnimation>  damageAnimations;

		/// <summary>
		/// The death animation mappings.
		/// </summary>
		public List<DamageTypeToDamageAnimation>  deathAnimations;

		/// <summary>
		/// The animation timer. Counts down for how long we should hold the state.
		/// </summary>
		protected float animationTimer;

		/// <summary>
		/// DamageType we are playing animation for.
		/// </summary>
		protected int currentAnimation;

		#endregion
		
		#region Unity Hooks
		
		/// <summary>
		/// Unity Update() hook, update the timer.
		/// </summary>
		void Update()
		{
			if (animationTimer > 0) 
			{
				animationTimer -= TimeManager.FrameTime;
			}
		}
		
		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Animation per Damage Type";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = " A damage movement which plays different animations based on the damage type.";
		
		/// <summary>
		/// Static movement info used by the editor.
		/// </summary>
		new public static MovementInfo Info
		{
			get
			{
				// Return null, can't be added via death animation selector at this time.
				return new MovementInfo(null, null);
			}
		}

		#endregion
		
		#region public methods
		
		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
		}
		
		/// <summary>
		/// Initialise the mvoement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			AssignReferences (character);
			if (deathAnimations.Count == 0) Debug.LogWarning ("No death animations specified.");
			return this;
		}
		
		/// <summary>
		/// If the jump just started force control.
		/// </summary>
		override public bool ForceMaintainControl()
		{
			// No need to give control back on a death animation
			if (isDeath) return true;

			if (animationTimer <= 0) 
			{
				return false;
			}
			return true;
		}

		
		/// <summary>
		/// For damage the default is to not apply gravity.
		/// </summary>
		override public bool ShouldApplyGravity
		{
			get
			{
				return isDeath ? deathAnimations[currentAnimation].applyGravity : damageAnimations[currentAnimation].applyGravity;
			}
		}

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				return isDeath ? deathAnimations[currentAnimation].animationState : damageAnimations[currentAnimation].animationState;
			}
		}

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public RaycastType ShouldDoBaseCollisions
		{
			get 
			{
				return isDeath ? deathAnimations[currentAnimation].collisionsToApply : damageAnimations[currentAnimation].collisionsToApply;
			}
		}

		
		/// <summary>
		/// Start the damage movement.
		/// </summary>
		/// <param name="info">Info.</param>
		override public void Damage(DamageInfo info, bool isDeath)
		{
			int tmpAnimationIndex = -1;
			this.isDeath = isDeath;
			if (isDeath) 
			{
				// Find a match
				for (int i = 0; i < deathAnimations.Count; i++)
				{
					if (deathAnimations[i].damageType == info.DamageType) 
					{
						tmpAnimationIndex = i;
						animationTimer = deathAnimations[i].animationTime;
						break;
					}
				}
				// No match try to find type == NONE
				for (int i = 0; i < deathAnimations.Count; i++)
				{
					if (deathAnimations[i].damageType == DamageType.NONE) 
					{
						tmpAnimationIndex = i;
						animationTimer = deathAnimations[i].animationTime;
						break;
					}
				}
			}
			else
			{
				// Find a match
				for (int i = 0; i < damageAnimations.Count; i++)
				{
					if (damageAnimations[i].damageType == info.DamageType) 
					{
						tmpAnimationIndex = i;
						animationTimer = damageAnimations[i].animationTime;
						break;
					}
				}
				// No match try to find type == NONE
				for (int i = 0; i < damageAnimations.Count; i++)
				{
					if (damageAnimations[i].damageType == DamageType.NONE) 
					{
						tmpAnimationIndex = i;
						animationTimer = damageAnimations[i].animationTime;
						break;
					}
				}
			}
			if (tmpAnimationIndex != -1)
			{
				currentAnimation = tmpAnimationIndex;
			}
			else
			{
#if UNITY_EDITOR
				Debug.LogWarning(string.Format ("Couldn't find a damage/death animation for the given damage type ({0}). Use DamageType.NONE to specify a default.", info.DamageType));
#endif
				currentAnimation = 0;
			}
		}

		#endregion

	}

	/// <summary>
	/// Maps Damage type to Damage Animation.
	/// </summary>
	[System.Serializable]
	public class DamageTypeToDamageAnimation
	{
		/// <summary>
		/// The type of the damage triggering the animation.
		/// </summary>
		[Tooltip ("The type of the damage triggering the animation.")]
		public DamageType damageType;

		/// <summary>
		/// The animation state to play.
		/// </summary>
		[Tooltip ("The animation state to play.")]
		public AnimationState animationState;

		/// <summary>
		/// The animation time. How long does the movement want control while this animation plays? (Ignored for death).
		/// </summary>
		[Tooltip ("The animation time. How long does the movement want control while this animation plays? (Ignored for death).")]
		public float animationTime;

		/// <summary>
		/// Should we apply graivty and base collisions
		/// </summary>
		[Tooltip ("Should we apply gravity and base collisions?")]
		public bool applyGravity;

		/// <summary>
		/// What if any collisions should be apply.
		/// </summary>
		[Tooltip ("What if any collisions should be apply.")]
		public RaycastType collisionsToApply = RaycastType.FOOT;
	}
}
