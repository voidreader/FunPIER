using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Enemy movement which hides the enemy and plays a deploy/undeploy animation.
	/// </summary>
	public class EnemyMovement_HideWithDeploy : EnemyMovement
	{
		
		#region members

		/// <summary>
		/// The deploy time.
		/// </summary>
		[Tooltip ("How long it takes to deploy from/undeploy to hiding state.")]
		public float deployTime = 0.5f;

		/// <summary>
		/// Should we also remove hazards when hiding? 
		/// </summary>
		[Tooltip ("Should we make the enemy hide by disabling their hurt box? If false we can optionally make them invulnerable " +
				  "(which can make enemies act like a shield for other enemies).")]
		public bool disableHurtBox;

		/// <summary>
		/// If true character will be invulnerable while hidng.
		/// </summary>
		public bool makeInvulnerable = true;

		/// <summary>
		/// Should we also remove hazards when hiding? 
		/// </summary>
		[Tooltip ("Should we also remove hazards when hiding? If not the enemy will still be able to damage the player while hiding.")]
		public bool removeHazards;

		/// <summary>
		/// The animation state to play if no hiding or deploying is happening.
		/// </summary>
		[Tooltip ("The animation state to play if no hiding or deploying is happening.")]
		public AnimationState defaultAnimationState = AnimationState.IDLE;

		/// <summary>
		/// How long to wait before enabling hazard collider. Its often useful to wait a 
		/// moment after hiding before making the hazards cause damage, particualrly if there is no deploy time.
		/// </summary>
		[Tooltip("How long to wait before enabling hazard collider. Its often useful to wait a moment after hiding before making the hazards cause damage, particualrly if there is no deploy time. ")]
		public float delayForRenablingHazard = 0.0f;

		/// <summary>
		/// Track our internal state.
		/// </summary>
		protected LocalState state = LocalState.DEPLOYED;

		/// <summary>
		/// The deploy timer.
		/// </summary>
		protected float deployTimer;

		/// <summary>
		/// The hurt box collider.
		/// </summary>
		protected Collider2D hurtCollider;
		
		/// <summary>
		/// The hazard collider.
		/// </summary>
		protected Collider2D hazardCollider;

		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Hide/With Deploy";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Enemy movement which hides the enemy and plays a deploy/undeploy animation.";
		
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

		#endregion
		
		#region properties
		
		
		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				if (state == LocalState.DEPLOYED) return AnimationState.IDLE;
				if (deployTime > 0)
				{
					if (state == LocalState.UNDEPLOYING) return AnimationState.UNDEPLOY;
					if (state == LocalState.DEPLOYING) return AnimationState.DEPLOY;
				}
				if (state == LocalState.HIDING) return AnimationState.HIDING;
				return defaultAnimationState;
			}
		}
		
		#endregion
		
		#region Unity hooks

		/// <summary>
		/// Unity Update() hook.
		/// </summary>
		void Update()
		{
			if (deployTime > 0 && deployTimer > 0)
			{
				deployTimer -= TimeManager.FrameTime;
				if (deployTimer <= 0.0f)
				{
					if (state == LocalState.UNDEPLOYING)
					{
						state = LocalState.HIDING;
					}
					else if (state == LocalState.DEPLOYING)
					{
						state = LocalState.DEPLOYED;
						if (disableHurtBox) hurtCollider.enabled = true;
						else enemy.MakeVulnerable();
						if (removeHazards) StartCoroutine(EnableHazardAfterDelay());
					}
				}
			}
		}

		#endregion

		#region public methods
		
		/// <summary>
		/// Initialise this movement and return a reference to the ready to use movement.
		/// </summary>
		override public EnemyMovement Init(Enemy enemy)
		{
			this.enemy = enemy;
			EnemyHurtBox hurtBox = enemy.gameObject.GetComponentInChildren<EnemyHurtBox> ();
			if (hurtBox) 
			{
				hurtCollider = hurtBox.GetComponent<Collider2D>();
			}
			else
			{
				Debug.LogError("Hiding enemy has no hurt box so there is nothing to disable on hide.");
			}
			if (removeHazards)
			{
				Hazard hazard = enemy.gameObject.GetComponentInChildren<Hazard> ();
				if (hazard) 
				{
					hazardCollider = hazard.GetComponent<Collider2D>();
				}
				else
				{
					Debug.LogError("Hiding enemy has 'remove hazards' checked has no Hazard was found.");
				}
			}
			return this;
		}

		/// <summary>
		/// Delay the enabling of the hazard.
		/// </summary>
		protected IEnumerator EnableHazardAfterDelay()
		{
			float timer = delayForRenablingHazard;
			while (timer > 0) {
				timer -= TimeManager.FrameTime;
				yield return true;
			}
			hazardCollider.enabled = true;
		}

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public bool DoMove()
		{
			if (!disableHurtBox && makeInvulnerable) enemy.MakeInvulnerable(float.MaxValue);
			if (state == LocalState.DEPLOYED)
			{
				deployTimer = deployTime;
				state = LocalState.UNDEPLOYING;
				if (disableHurtBox) hurtCollider.enabled = false;
				if (removeHazards) hazardCollider.enabled = false;
			}

			return false;
		}

		/// <summary>
		/// Called when this movement is losing control. In this case we want to play the deploy animation to bring us back to ready state.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		override public bool LosingControl()
		{
			if (state != LocalState.DEPLOYED)
    		{
				if (state == LocalState.DEPLOYING) 
				{
					if (deployTimer <= 0.0f) 
					{
						state = LocalState.DEPLOYED;
						// Re-enable hitboxes before we lose control
						if (disableHurtBox) hurtCollider.enabled = true;
						if (removeHazards) StartCoroutine(EnableHazardAfterDelay());
						return false;
					}
				}
				else 
				{
					deployTimer = deployTime;
					state = LocalState.DEPLOYING;
				}
				return true;
			}
			// Re-enable hitboxes before we lose control
			if (disableHurtBox) hurtCollider.enabled = true;
			if (removeHazards) StartCoroutine(EnableHazardAfterDelay());
			return false;
		}

		#endregion

		protected enum LocalState 
		{
			DEPLOYED, DEPLOYING, HIDING, UNDEPLOYING
		}

	}
}
