using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlatformerPro
{
	/// <summary>
	/// Base class for an enemy.
	/// </summary>
	public class Enemy : MonoBehaviour, IMob
	{
		#region public members

		[Header ("Health")]
		/// <summary>
		/// Enemies health.
		/// </summary>
		[Range (1, 100)]
		public int health = 1;

		/// <summary>
		/// How long the enemy is invulnerable to damage after being hit.
		/// </summary>
		[Range (0, 10)]
		[Tooltip ("How long the enemy is invulnerable to damage after being hit.")]
		public float invulnerableTime = 0.0f;

		[Header ("Interactions")]

		/// <summary>
		/// Should movements keep moving while falling.
		/// </summary>
		[Tooltip("If true movements will still being active while falling, otherwise state will be set to FALL.")]
		public bool continueMovementOnFall;

		/// <summary>
		/// Should we switch colliders when the enemy changes direction (used
		/// for asymmetric characters).
		/// </summary>
		public bool switchCollidersOnDirectionChange;

		/// <summary>
		/// The facing direction on start. Typically 1 or -1.
		/// </summary>
		[Range (-1, 1)]
		public int startFacingDirection = 1;

		/// <summary>
		/// Does this enemy interact with platforms?
		/// </summary>
		[Tooltip("If true the enemy will interact with platforms like a character. This can have a performance impact if you have a lot of enemies.")]
		public bool enemyInteractsWithPlatforms;

		[Space (10)]
		[Header ("Collisions")]
		/// <summary>
		/// The layers that are considered when working out collisions. i.e. the things we can't move through.
		/// </summary>
		/// <seealso cref="Character.layerMask"/>
		public LayerMask geometryLayerMask;

		/// <summary>
		/// Does the character fall if there is no platform below it?
		/// </summary>
		[HideInInspector]
		public bool characterCanFall;

		/// <summary>
		/// Maximum fall speed (note that any individual movement may override this if it wants to).
		/// </summary>
		[HideInInspector]
		public float terminalVelocity = -15f;

		/// <summary>
		/// Should we use the characters gravity or a custom one.
		/// </summary>
		[HideInInspector]
		public bool useCharacterGravity;

		/// <summary>
		/// Custom gravity.
		/// </summary>
		[HideInInspector]
		public float customGravity = -15f;

		/// <summary>
		/// If the cahracter can fall where are its feet in y?
		/// </summary>
		[HideInInspector]
		public float feetHeight;

		/// <summary>
		/// How far do we look ahead when determining if we are grounded.
		/// </summary>
		[HideInInspector]
		public float groundedLookAhead;


		/// <summary>
		/// If the cahracter can fall where are its feet in x (offset from centre)?
		/// </summary>
		[HideInInspector]
		public float feetWidth;

		/// <summary>
		/// Does the character detect side collisions?
		/// </summary>
		[HideInInspector]
		public bool detectSideCollisions;

		/// <summary>
		/// The height of the side collisions.
		/// </summary>
		[HideInInspector]
		public float sideHeight;

		/// <summary>
		/// The width of the side collisions.
		/// </summary>
		[HideInInspector]
		public float sideWidth;

		/// <summary>
		/// If the charcter runs in to the wall should it switch direciton on all movements?
		/// </summary>
		[HideInInspector]
		public bool switchDirectionOnSideHit;

		#endregion

		#region protected members

		/// <summary>
		/// Cached copy of the enemies transform.
		/// </summary>
		protected Transform myTransform;

		/// <summary>
		/// Reference to the class that controls the enemy movement.
		/// </summary>
		protected EnemyMovement movement;

		/// <summary>
		/// Reference to the class that controls the enemy death movement.
		/// </summary>
		protected EnemyMovement deathMovement;

		/// <summary>
		/// The enemies current animation state.
		/// </summary>
		protected AnimationState animationState;

		/// <summary>
		/// The enemies current animation overridestate.
		/// </summary>
		protected string overrideState;

		/// <summary>
		/// A cached animation event args that we update so we don't need to allocate.
		/// </summary>
		protected AnimationEventArgs animationEventArgs;

		/// <summary>
		/// Is this enemy active?
		/// </summary>
		protected bool Active = true;

		/// <summary>
		/// Timer for invulnerability. Character is invulnerable when this is greater than zero.
		/// </summary>
		protected float invulnerableTimer = -1.0f;

		/// <summary>
		/// Cached copy of damage event args to save on allocations.
		/// </summary>
		protected DamageInfoEventArgs damageEventArgs;

		/// <summary>
		/// The enemy ai (can be null meaning only movements will control the character).
		/// </summary>
		protected EnemyAI enemyAi;

		/// <summary>
		/// The direction currently faced. Used for switch collider calculations and for AI that needs actual facing direction.
		/// </summary>
		protected int currentFacingDirection;
		
		/// <summary>
		/// If we switch colliders when the character turns this is the list of transforms of Collider2D's to switch.
		/// </summary>
		protected Transform[] collidersForSwitching;

		/// <summary>
		/// The right foot collider.
		/// </summary>
		protected NoAllocationSmartFeetcast rightFoot;

		/// <summary>
		/// The left foot collider.
		/// </summary>
		protected NoAllocationSmartFeetcast leftFoot;

		/// <summary>
		/// The right side collider.
		/// </summary>
		protected NoAllocationSmartSidecast rightSide;
		
		/// <summary>
		/// The left side collider.
		/// </summary>
		protected NoAllocationSmartSidecast leftSide;

		/// <summary>
		/// If gravity comes from a character reference this is the gravity we are using.
		/// </summary>
		protected Character characterForGravity;

		/// <summary>
		/// Is the enemy grounded.
		/// </summary>
		protected bool grounded;

		/// <summary>
		/// The angle of the ignored slope.
		/// </summary>
		protected float ignoredSlope;

		/// <summary>
		/// The platform collision arguments.
		/// </summary>
		protected PlatformCollisionArgs platformCollisionArgs;

		/// <summary>
		/// Stores the aprent platform if we have one.
		/// </summary>
		protected Platform parentPlatform;

		
		/// <summary>
		/// The current target.
		/// </summary>
		protected Character currentTarget;

		/// <summary>
		/// A list of colliders to ingore during collisions.
		/// </summary>
		protected HashSet<Collider2D> ignoredColliders;

		#endregion

		#region constants

		/// <summary>
		/// Max slope an enemy will calcualte/walk on
		/// </summary>
		protected const float MAX_ENEMY_SLOPE = 60.0f;

		/// <summary>
		/// Default side look ahead.
		/// </summary>
		protected const float DefaultSideLookAhead = 0.05f;

		#endregion
		#region events
		
		/// <summary>
		/// Event for anmation state changes.
		/// </summary>
		public event System.EventHandler <AnimationEventArgs> ChangeAnimationState;
		
		/// <summary>
		/// Event for damage.
		/// </summary>
		public event System.EventHandler <DamageInfoEventArgs> Damaged;
		
		/// <summary>
		/// Event for death.
		/// </summary>
		public event System.EventHandler <DamageInfoEventArgs> Died;

		/// <summary>
		/// Event for being hit but not damaged.
		/// </summary>
		public event System.EventHandler <DamageInfoEventArgs> Collided;

		/// <summary>
		/// Event for damaging a character.
		/// </summary>
		public event System.EventHandler <DamageInfoEventArgs> CharacterDamaged;
	
		/// <summary>
		/// Raises the damaged event.
		/// </summary>
		/// <param name="info">Info.</param>
		virtual protected void OnDamaged(DamageInfo info)
		{
			if (Damaged != null)
			{
				damageEventArgs.UpdateDamageInfoEventArgs(info);
				Damaged(this, damageEventArgs);
			}
		}

		/// <summary>
		/// Raises the died event.
		/// </summary>
		/// <param name="info">Info.</param>
		virtual protected void OnDied(DamageInfo info)
		{
			if (Died != null)
			{
				damageEventArgs.UpdateDamageInfoEventArgs(info);
				Died(this, damageEventArgs);
			}
		}

		/// <summary>
		/// Raises the collided event.
		/// </summary>
		/// <param name="info">Info.</param>
		virtual protected void OnCollided(DamageInfo info)
		{
			if (Collided != null)
			{
				damageEventArgs.UpdateDamageInfoEventArgs(info);
				Collided(this, damageEventArgs);
			}
		}

		/// <summary>
		/// Raises the change animation state event. This version is public as it 
		/// just triggers an update not a change.
		/// </summary>
		virtual public void OnChangeAnimationState()
		{
			if (ChangeAnimationState != null)
			{
				animationEventArgs.UpdateAnimationOverrideState(OverrideState);
				ChangeAnimationState(this, animationEventArgs);
			}
		}
		
		/// <summary>
		/// Raises the change animation state event.
		/// </summary>
		/// <param name="state">Current state.</param>
		/// <param name="previousState">Previous state.</param>
		virtual protected void OnChangeAnimationState(AnimationState state, AnimationState previousState)
		{
			if (ChangeAnimationState != null)
			{
				animationEventArgs.UpdateAnimationEventArgs(state, previousState, OverrideState);
				ChangeAnimationState(this, animationEventArgs);
			}
		}
		
		/// <summary>
		/// Raises the change animation state event.
		/// </summary>
		/// <param name="state">Current state.</param>
		/// <param name="previousState">Previous state.</param>
		/// <param name="priority">Animaton state priority.</param>
		virtual protected void OnChangeAnimationState(AnimationState state, AnimationState previousState, int priority)
		{
			if (ChangeAnimationState != null)
			{
				animationEventArgs.UpdateAnimationEventArgs(state, previousState, OverrideState, priority);
				ChangeAnimationState(this, animationEventArgs);
			}
		}

		/// <summary>
		/// Raises the hit character event.
		/// </summary>
		/// <param name="info">Damage info.</param>
		virtual protected void OnCharacterDamaged(DamageInfo info)
		{
			if (CharacterDamaged != null)
			{
				damageEventArgs.UpdateDamageInfoEventArgs(info);
				CharacterDamaged(this, damageEventArgs);
			}
		}

		#endregion


		#region properties

		
		/// <summary>
		/// Gets cache copy of the transform.
		/// </summary>
		virtual public Transform Transform
		{
			get
			{
				return myTransform;
			}
		}

		/// <summary>
		/// Returns the direction the enemy is facing. 0 for none, 1 for right, -1 for left.
		/// </summary>
		virtual public int FacingDirection
		{
			get 
			{
				if (movement != null) return movement.FacingDirection;
				return 0;
			}
		}

		/// <summary>
		/// The enemies current animation state.
		/// </summary>
		virtual public AnimationState AnimationState
		{
			get
			{
				return animationState;
			}
			protected set
			{
				if (value != animationState)
				{
					OnChangeAnimationState(value, animationState);
					animationState = value;
				}
			}
		}

		
		/// <summary>
		/// The enemies current animation override state.
		/// </summary>
		virtual public string OverrideState
		{
			get
			{
				return overrideState;
			}
			protected set
			{
				if (value != overrideState)
				{
					overrideState = value;
					OnChangeAnimationState();
				}
			}
		}

		/// <summary>
		/// The characters velocity.
		/// </summary>
		virtual public Vector2 Velocity
		{
			get; set;
		}
		
		/// <summary>
		/// The characters velocity in the previous frame.
		/// </summary>
		virtual public Vector2 PreviousVelocity
		{
			get; set;
		}


		/// <summary>
		/// Is the character currently invulnerable?
		/// </summary>
		/// <value>The character.</value>
		virtual public bool IsInvulnerable
		{
			get
			{
				// Dead characters can't take damage
				if (health <= 0) return true;
				return invulnerableTimer > 0.0f;
			}
		}

		/// <summary>
		/// The current AI state.
		/// </summary>
		/// <value>The state.</value>
		virtual public EnemyState State
		{
			get; protected set;
		}
		
		/// <summary>
		/// Returns the direction the character is facing, but if direction is currently 0 instead returns the direction last faced.
		/// </summary>
		virtual public int LastFacedDirection
		{
			get 
			{
				return currentFacingDirection != 0 ? currentFacingDirection : 1;
			}
		}

		/// <summary>
		/// If slopes are on this is the rotation we are rotating towards.
		/// </summary>
		virtual public float SlopeTargetRotation
		{
			get; protected set;
		}
		
		/// <summary>
		/// If slopes are on this is the rotation we are currently on.
		/// </summary>
		virtual public float SlopeActualRotation
		{
			get
			{
				return 0.0f;
			}
		}

		/// <summary>
		/// The characters rotation in the previous frame.
		/// </summary>
		virtual public float PreviousRotation
		{
			get; set;
		}

		/// <summary>
		/// Gets the ignored slope. The ignored slope is used primary for internal
		/// physics calculations and is the largest slope that was ignored by the side colliders last frame.
		/// </summary>
		virtual public float IgnoredSlope
		{
			get
			{
				return ignoredSlope;
			}
			set
			{
				if (value > ignoredSlope) ignoredSlope = value;
			}
		}

		/// <summary>
		/// Gets the minimum angle at which geometry is considered a wall.
		/// </summary>
		virtual public float MinSideAngle
		{
			get
			{
				return MAX_ENEMY_SLOPE;
			}
		}

		/// <summary>
		/// Gets or sets the characters current z layer.
		/// </summary>
		virtual public int ZLayer {
			get;
			set;
		}

		/// <summary>
		/// Gets the enemy AI or null if there is no AI.
		/// </summary>
		/// <value>The enemy A.</value>
		virtual public EnemyAI AI
		{
			get { return enemyAi;}
		}

		
		/// <summary>
		/// Returns true if we are grounded or false otherwise.
		/// </summary>
		virtual public bool Grounded
		{
			get
			{
				return grounded;
			}
		}

		/// <summary>
		/// If grounded this is the layer of the ground. Undefined if not grounded.
		/// </summary>
		virtual public int GroundLayer {
			get;
			protected set;
		}

		/// <summary>
		/// Gravity for this enemy
		/// </summary>
		virtual public float Gravity
		{
			get
			{
				return (useCharacterGravity ? characterForGravity.Gravity : customGravity);
			}
		}

		/// <summary>
		/// Gets the current movement or null if no movement is acgtive (for example when falling with continueMovementOnFall off).
		/// </summary>
		virtual public EnemyMovement CurrentMovement
		{
			get 
			{
				if (State == PlatformerPro.EnemyState.DAMAGED || State == PlatformerPro.EnemyState.DEAD) return deathMovement;
				if (State == EnemyState.FALLING && !continueMovementOnFall) return null;
				if (movement is EnemyMovement_Distributor) return ((EnemyMovement_Distributor)movement).CurrentMovement;
				return movement;
			}
		}

		/// <summary>
		/// Gets the friction.
		/// </summary>
		/// <value>The friction.</value>
		virtual public float Friction
		{
			get; set;
		}

		/// <summary>
		/// The current targets position or enemies position if no target found.
		/// </summary>
		virtual public Transform CurrentTargetTransform
		{
			get
			{
				if (currentTarget == null) return null;
				return currentTarget.transform;
			}	
		}

		/// <summary>
		/// The current target.
		/// </summary>
		virtual public Character CurrentTarget
		{
			get
			{
				return currentTarget;
			}
		}

		#endregion

		#region unity hooks

		/// <summary>
		/// Get all related behaviours and initialise.
		/// </summary>
		void Start()
		{
			Init();
		}

		/// <summary>
		/// Do the movement.
		/// </summary>
		void Update()
		{	
			DoUpdate ();
		}

	
		#endregion

		#region protected methods
		
		/// <summary>
		/// Set up the character
		/// </summary>
		virtual protected void Init()
		{
			// Cache transform
			myTransform = transform;
			
			// Find and initialise movement
			deathMovement = GetComponentInChildren<EnemyDeathMovement>();
			if (deathMovement != null)
				deathMovement = deathMovement.Init (this);
			else
				Debug.LogWarning ("Enemies should have a death movement attached. Use EnemyMovement_Damage_PlayAnimationOnly if unsure.");
			movement = GetComponent<EnemyMovement_Distributor>();
			if (movement == null) 
			{
				foreach (EnemyMovement em in GetComponents<EnemyMovement>())
				{
					if (!(em is EnemyDeathMovement)) movement = em;
				}
			}
			if (movement != null) movement = movement.Init(this);

			// Create variable for animation state
			animationEventArgs = new AnimationEventArgs(AnimationState.NONE, AnimationState.NONE, null);

			// Create variable for damage
			damageEventArgs = new DamageInfoEventArgs();

			// Set facing direction
			currentFacingDirection = startFacingDirection;

			// Get AI
			enemyAi = GetComponent<EnemyAI>();
			if (enemyAi != null) enemyAi.Init (this);

			if (switchCollidersOnDirectionChange)
			{
				collidersForSwitching = GetComponentsInChildren<Collider2D>().Select(c=>c.transform).ToArray();
			}

			// Feet colliders and gravity
			if (characterCanFall)
			{
				rightFoot = new NoAllocationSmartFeetcast();
				rightFoot.Mob = this;
				rightFoot.RaycastType = RaycastType.FOOT;
				rightFoot.Extent = new Vector3(feetWidth, feetHeight);
				rightFoot.Transform = transform;
				rightFoot.LayerMask = geometryLayerMask;
				rightFoot.LookAhead = groundedLookAhead;
				leftFoot = new NoAllocationSmartFeetcast();
				leftFoot.Mob = this;
				leftFoot.RaycastType = RaycastType.FOOT;
				leftFoot.Extent = new Vector3(-feetWidth, feetHeight);
				leftFoot.Transform = transform;
				leftFoot.LayerMask = geometryLayerMask;
				leftFoot.LookAhead = groundedLookAhead;

				if (useCharacterGravity)
				{
					characterForGravity = (Character) FindObjectOfType(typeof(Character));
					// TODO Add CharacterLoader
				}
			}

			if (detectSideCollisions)
			{
				rightSide = new NoAllocationSmartSidecast();
				rightSide.Mob = this;
				rightSide.RaycastType = RaycastType.SIDE_RIGHT;
				rightSide.Extent = new Vector3(sideWidth, sideHeight);
				rightSide.Transform = transform;
				rightSide.LayerMask = geometryLayerMask;
				rightSide.LookAhead = DefaultSideLookAhead;
				leftSide = new NoAllocationSmartSidecast();
				leftSide.Mob = this;
				leftSide.RaycastType = RaycastType.SIDE_LEFT;
				leftSide.Extent = new Vector3(-sideWidth, sideHeight);
				leftSide.Transform = transform;
				leftSide.LayerMask = geometryLayerMask;
				leftSide.LookAhead = DefaultSideLookAhead;
			}

			if (enemyInteractsWithPlatforms)
			{
				platformCollisionArgs = new PlatformCollisionArgs ();
				platformCollisionArgs.Character = this;
				transform.parent = null;
			}

			// Create ignored colliders list
			ignoredColliders = new HashSet<Collider2D> ();

		}

		/// <summary>
		/// Run each frame to determine and execute move.
		/// </summary>
		virtual protected void DoUpdate()
		{
			// Track if we have already decided what to do
			bool hasAlreadyDecided = false;
			if (enabled)
			{
				// Update invulnerable timer
				if (invulnerableTimer >= 0 && State != EnemyState.DEAD) 
				{
					invulnerableTimer -= TimeManager.FrameTime;
					if (invulnerableTimer <= 0.0f) 
					{
						if (enemyAi != null)
						{
							State = enemyAi.Decide();
						}
						else
						{
							State = EnemyState.DEFAULT;
						}
						hasAlreadyDecided = true;
					}
				}
				
				// Do base collisions
				if (characterCanFall) DoBaseCollisions ();
				else if (detectSideCollisions) ApplySideCollisions();

				// Override animation state if falling
				if (characterCanFall && !IsInvulnerable)
				{
					if (!grounded)
					{
						State = EnemyState.FALLING;
						if (!continueMovementOnFall) AnimationState = AnimationState.FALL;
					}
					else if (State == EnemyState.FALLING)
					{
						// If we are grounded and were falling, decide what to do next when we land.
						if (enemyAi != null)
						{
							State = enemyAi.Decide();
						}
						else
						{
							State = EnemyState.DEFAULT;
						}
						hasAlreadyDecided = true;
					}
				}
				
				if (!hasAlreadyDecided) DecideOnNextMovement();
				
				// Velocity
				PreviousVelocity = Velocity;
				// Theres a little inconsistency here, characters use actual rotation but enemies use target rotation for previous rotation
				PreviousRotation = SlopeTargetRotation;
				
				// Death movement
				if (State == EnemyState.DEAD && deathMovement != null)
				{
					// Movement
					deathMovement.DoMove();
					
					// Animation
					AnimationState = deathMovement.AnimationState;
				}
				else if (State == EnemyState.DAMAGED && deathMovement != null)
				{
					// Movement
					deathMovement.DoMove();
					
					// Animation
					AnimationState = deathMovement.AnimationState;
					
					// Tell the AI we got damaged
					if (enemyAi != null) enemyAi.Damaged();
				}
				// move
				else if (movement != null && (State != EnemyState.FALLING || continueMovementOnFall)) 
				{
					
					// Movement
					movement.DoMove();
					
					// Animation
					AnimationState = movement.AnimationState;
				}
				
				// Check for facing direction changes
				if (switchCollidersOnDirectionChange)
				{
					if (FacingDirection != 0 && currentFacingDirection != 0 && currentFacingDirection != FacingDirection)
					{
						SwitchColliders();
					}
				}
				
				// Ignore zero direction
				if (FacingDirection != 0) currentFacingDirection = FacingDirection;
				
			}
		}


		/// <summary>
		/// Switches left and right collider direction and flips position of all colliders about y axis.
		/// </summary>
		virtual protected void SwitchColliders()
		{
			// Switch Collider 2D's
			for (int i = 0; i < collidersForSwitching.Length; i++)
			{
				collidersForSwitching[i].localPosition = new Vector3(-collidersForSwitching[i].localPosition.x, collidersForSwitching[i].localPosition.y, collidersForSwitching[i].localPosition.z);
			}
		}

		/// <summary>
		/// Does the base collisions.
		/// </summary>
		virtual protected void DoBaseCollisions()
		{
			
			// TODO Make this order configurable without coding
			
			// Cache velocity
			// float y = Velocity.y; float x = Velocity.x; 
			
			if (detectSideCollisions)
			{
				ApplySideCollisions ();
			}
			
			// Gravity and feet collisions if character can fall
			if (characterCanFall) 
			{
				ApplyFeetCollisions ();
				if (!grounded) ApplyGravity();
			}
			
			// TODO Side collisions after fall too?
			//			if (x >= 0) ApplyBaseCollisionsForRaycastType(RaycastType.SIDE_LEFT);
			//			if (x <= 0) ApplyBaseCollisionsForRaycastType(RaycastType.SIDE_RIGHT);
			
		}



		/// <summary>
		/// Decides on the next movement. Extracted out so it can be easily overriden.
		/// </summary>
		virtual protected void DecideOnNextMovement()
		{
			// Check AI (if we have one and we aren't currently being damaged or dead)
			if (enemyAi != null && State != EnemyState.DAMAGED && State != EnemyState.DEAD && (State != EnemyState.FALLING || grounded))
			{
				if (enemyAi.UpdateTimer())
				{
					State = enemyAi.Decide();
				}
				else if (enemyAi.Sense())
				{
					State = enemyAi.Decide();
				}
			}
		}

		#endregion

		#region public methods

		/// <summary>
		/// Call this to switch the direction of all associated movements.
		/// </summary>
		virtual public void SwitchDirection()
		{
			// Don't switch when damaged or dead
			if (State == EnemyState.DAMAGED || State == EnemyState.DEAD) return;
			currentFacingDirection *= -1;
			if (switchCollidersOnDirectionChange) SwitchColliders();
			movement.SwitchDirection();
			OnChangeAnimationState ();
		}

		/// <summary>
		/// Called when the enemy hits the character causing damage.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="info">Damage info.</param>
		virtual public void HitCharacter(Character character, DamageInfo info)
		{
			// Tell the movement we hit the character, it might want to change animation state
			if (movement != null) movement.HitCharacter(character, info);
			OnCharacterDamaged (info);
		}

		/// <summary>
		/// Called when the enemies current attack finished
		/// </summary>
		virtual public void AttackFinished()
		{
			MovementComplete ();
			if (State != EnemyState.DEAD && State != EnemyState.DAMAGED) State = EnemyState.DEFAULT;
		}

		/// <summary>
		/// Called when a compeletable movement finishes.
		/// </summary>
		virtual public void MovementComplete()
		{
			// Enemyy doesn't do anything with this call but subclasses may.
		}

		/// <summary>
		/// Reset this instance.
		/// </summary>
		virtual public void Reset()
		{
			State = EnemyState.DEFAULT;
			SetVelocityX (0);
			SetVelocityY (0);
			if (currentFacingDirection != startFacingDirection)
			{
				if (switchCollidersOnDirectionChange) SwitchColliders();
			}
			currentFacingDirection = startFacingDirection;

			// Enable colliders
			EnemyHurtBox hurtBox = gameObject.GetComponentInChildren<EnemyHurtBox> ();
			if (hurtBox != null && hurtBox.GetComponent<Collider2D>() != null) hurtBox.GetComponent<Collider2D>().enabled = true;
			Hazard hazard = gameObject.GetComponentInChildren<Hazard> ();
			if (hazard != null && hazard.GetComponent<Collider2D>() != null) hazard.GetComponent<Collider2D>().enabled = true;
		}

		/// <summary>
		/// Damage the enemy with the specified damage information.
		/// </summary>
		/// <param name="info">The damage info.</param>
		virtual public void Damage(DamageInfo info)
		{
			if (!IsInvulnerable)
			{
				health -= info.Amount;
				if (health <= 0) 
				{
					Die(info);
					OnDied(info);
					State = EnemyState.DEAD;
				}
				else
				{
					if (deathMovement != null) deathMovement.DoDamage(info);
					OnDamaged (info);
					State = EnemyState.DAMAGED;
					invulnerableTimer = invulnerableTime;
				}
			}
			else
			{
				OnCollided(info);
			}
		}

		
		/// <summary>
		/// Heal the enemy by the specified amount.
		/// </summary>
		/// <param name="amount">Amount.</param>
		virtual public void Heal(int amount)
		{
			health += amount;
		}

		/// <summary>
		/// Kill the enemy.
		/// </summary>
		/// <param name="info">Info.</param>
		virtual public void Die(DamageInfo info)
		{
			if (deathMovement != null) 
			{
				deathMovement.DoDeath(info);
				// Disable colliders
				EnemyHurtBox hurtBox = gameObject.GetComponentInChildren<EnemyHurtBox> ();
				if (hurtBox != null && hurtBox.GetComponent<Collider2D>() != null) hurtBox.GetComponent<Collider2D>().enabled = false;
				EnemyHitBox hitBox = gameObject.GetComponentInChildren<EnemyHitBox> ();
				if (hitBox != null && hitBox.GetComponent<Collider2D>() != null) hitBox.GetComponent<Collider2D>().enabled = false;
				Hazard hazard = gameObject.GetComponentInChildren<Hazard> ();
				if (hazard != null && hazard.GetComponent<Collider2D>() != null) hazard.GetComponent<Collider2D>().enabled = false;
			}
			else
			{
				Destroy(gameObject);
			}
		}
	
		/// <summary>
		/// Translate the character by the supplied amount.
		/// </summary>
		/// <param name="x">The x amount.</param>
		/// <param name="y">The y amount.</param>
		/// <param name="applyYTransformsInWorldSpace">Should Y transforms be in world space instead of realtive to the
		/// character position? Default is true.</param>
		virtual public void Translate (float x, float y, bool applyYTransformsInWorldSpace)
		{
			if (applyYTransformsInWorldSpace)
			{
				myTransform.Translate(0, y, 0, Space.World);
				myTransform.Translate(x, 0, 0, Space.Self);
			}
			else
			{
				myTransform.Translate(x, y, 0, Space.Self);
			}
		}
		
		/// <summary>
		/// Adds velocity.
		/// </summary>
		/// <param name="x">The velocity to add to x.</param>
		/// <param name="y">The velocity to add to y.</param>
		virtual public void AddVelocity(float x, float y)
		{
			Velocity = new Vector2(Velocity.x + x, Velocity.y + y);
			if (Velocity.y < terminalVelocity) Velocity = new Vector2(Velocity.x, terminalVelocity);
		}
		
		/// <summary>
		/// Sets X velocity.
		/// </summary>
		/// <param name="x">The new x speed.</param>
		virtual public void SetVelocityX(float x)
		{
			Velocity = new Vector2(x, Velocity.y);
		}
		
		/// <summary>
		/// Sets Y velocity.
		/// </summary>
		/// <param name="x">The new y speed.</param>
		virtual public void SetVelocityY(float y)
		{
			Velocity = new Vector2(Velocity.x, y);
			if (Velocity.y < terminalVelocity) Velocity = new Vector2(Velocity.x, terminalVelocity);
		}

		/// <summary>
		/// Adds an animation override. NOTE: Currently only one override is supported at a time.
		/// Note overrides are used to refer to an animator or layer override (depending on animation type) not forcing 
		/// the override of a single animation which is done with ForceAnimation().
		/// </summary>
		/// <param name="overrideState">Override State, null values iwll be ignored.</param>
		virtual public void AddAnimationOverride(string overrideState)
		{
			if (overrideState != null) 
			{
				OverrideState = overrideState;
				OnChangeAnimationState();
			}
		}
		
		/// <summary>
		/// Removes an animation override. NOTE: Currently only one override is supported at a time.
		/// </summary>
		/// <param name="overrideState">Override State, will be removed if it matches current value.</param>
		virtual public void RemoveAnimationOverride(string overrideState)
		{
			if (OverrideState == overrideState) 
			{
				OverrideState = null;
				OnChangeAnimationState();
			}
		}

		/// <summary>
		/// Applies the gravity.
		/// </summary>
		virtual public void ApplyGravity()
		{
			// Apply acceleration
			AddVelocity(0, TimeManager.FrameTime * (useCharacterGravity ? characterForGravity.Gravity : customGravity));
			// Limit to terminal velocity
			if (Velocity.y < terminalVelocity) SetVelocityY(terminalVelocity);
			// Translate - note that y is applied relatively which means the character is always pushed towards the platform they are on with constant force
			// not physially accurate but it feels right when playing. You can always override with your own gravity class if desired :)
			Translate(0, Velocity.y * TimeManager.FrameTime, false);
		}

		/// <summary>
		/// Makes the enemy invulnerable for the given number of settings.
		/// </summary>
		/// <param name="invulnerableTime">Invulnerable time in seconds.</param>
		virtual public void MakeInvulnerable(float invulnerableTime)
		{
			invulnerableTimer = invulnerableTime;
		}

		/// <summary>
		/// Makes the enemy vulnerable.
		/// </summary>
		virtual public void MakeVulnerable()
		{
			invulnerableTimer = -1.0f;
			if (enemyAi != null)
			{
				State = enemyAi.Decide();
			}
			else
			{
				State = EnemyState.DEFAULT;
			}
		}


		/// <summary>
		/// Applies the feet collisions.
		/// </summary>
		virtual public void ApplyFeetCollisions()
		{
			float deepestFootPenetration = 0.0f;
			float smallestFraction = float.MaxValue;
			int closest = -1;
			float distanceToGround = float.MaxValue;
			int feetCount = 0;
			float leftFootSlope = 0;
			float rightFootSlope = 0;
			float deg;
			bool hasParented = false;
			GroundLayer = -1;

			grounded = false;
			
			// Get collisions
			RaycastHit2D[] leftFeetCollisions = leftFoot.GetRaycastHits();
			RaycastHit2D[] rightFeetCollisions = rightFoot.GetRaycastHits();

			// Left foot
			for (int i = 0; i < leftFeetCollisions.Length; i++)
			{	
				// Make sure we have a collision to support raycast colliders which use statically allocated hits arrays 
				if (leftFeetCollisions[i].collider != null)
				{
					if (leftFeetCollisions[i].fraction > 0 && leftFeetCollisions[i].fraction < smallestFraction)
					{
						smallestFraction = leftFeetCollisions[i].fraction;
						closest = i;
					}
				} 
			}

			// Found a collision
			if (closest != -1)
			{
				// float penetration = leftFoot.Length - (leftFeetCollisions[closest].fraction * (leftFoot.Length + leftFoot.LookAhead));
				float penetration = (leftFeetCollisions[closest].fraction * (leftFoot.Length + leftFoot.LookAhead)) - leftFoot.Length;
				if (penetration < deepestFootPenetration)
				{
					deepestFootPenetration = penetration;
				}
				// Check for platforms
				// The memory allocation only occurs in the editor, its not the GC trap that is seems
				if (enemyInteractsWithPlatforms)
				{
					Platform platform = leftFeetCollisions[closest].collider.GetComponent<Platform>();
					if (platform != null) 
					{
						platformCollisionArgs.RaycastCollider = leftFoot;
						platformCollisionArgs.Penetration = penetration;
						bool parent = platform.Collide(platformCollisionArgs);
						if (parent && !hasParented) 
						{
							if (parentPlatform != null)
							{
								parentPlatform.UnParent();
							}
							parentPlatform = platform;
							transform.parent = platform.transform;
							platform.Parent();
							hasParented = true;
						}
					}
				}

				if (penetration <= groundedLookAhead)
				{

					// Check for grounded
					grounded = true;
					if (penetration < distanceToGround) distanceToGround = penetration;
					feetCount++;
					deg = Mathf.Rad2Deg * Mathf.Atan2(leftFeetCollisions[closest].normal.x, leftFeetCollisions[closest].normal.y);
					if (deg <= MAX_ENEMY_SLOPE && deg >= -MAX_ENEMY_SLOPE) 
					{
						leftFootSlope = deg;
					}
					GroundLayer = leftFeetCollisions[closest].collider.gameObject.layer;
				}
			}
			
			// Right foot
			closest = -1;
			smallestFraction = float.MaxValue;
			for (int i = 0; i < rightFeetCollisions.Length; i++)
			{	
				// Make sure we have a collision to support raycast colliders which use statically allocated hits arrays 
				if (rightFeetCollisions[i].collider != null)
				{
					if (rightFeetCollisions[i].fraction > 0 && rightFeetCollisions[i].fraction < smallestFraction)
					{
						smallestFraction = rightFeetCollisions[i].fraction;
						closest = i;
					}
				}
			}
			// Found a collision
			if (closest != -1)
			{				
				// float penetration = rightFoot.Length - (rightFeetCollisions[closest].fraction * (rightFoot.Length + rightFoot.LookAhead));
				float penetration = (rightFeetCollisions[closest].fraction * (rightFoot.Length + rightFoot.LookAhead)) - rightFoot.Length;
				if (penetration < deepestFootPenetration)
				{
					deepestFootPenetration = penetration;
				}
				// Check for platforms
				// The memory allocation only occurs in the editor, its not the GC trap that is seems
				if (enemyInteractsWithPlatforms)
				{
					Platform platform = rightFeetCollisions[closest].collider.GetComponent<Platform>();
					if (platform != null) 
					{
						platformCollisionArgs.RaycastCollider = rightFoot;
						platformCollisionArgs.Penetration = penetration;
						bool parent = platform.Collide(platformCollisionArgs);
						if (parent && !hasParented) 
						{
							if (parentPlatform == platform)
							{
								hasParented = true;
							}
							else 
							{
								if (parentPlatform != null)
								{
									parentPlatform.UnParent();
								}
								parentPlatform = platform;
								transform.parent = platform.transform;
								platform.Parent();
								hasParented = true;
							}
						}
					}
				}
				if (penetration <= groundedLookAhead)
				{
					// Check for grounded
					grounded = true;
					if (penetration < distanceToGround) distanceToGround = penetration;
					feetCount++;
					deg = Mathf.Rad2Deg * Mathf.Atan2(rightFeetCollisions[closest].normal.x, rightFeetCollisions[closest].normal.y);
					if (deg <= MAX_ENEMY_SLOPE && deg >= -MAX_ENEMY_SLOPE) 
					{
						rightFootSlope = deg;
					}
					GroundLayer = rightFeetCollisions[closest].collider.gameObject.layer;
				}
			}
			
			// On the ground so set y velocity to 0
			if (grounded && Velocity.y <= 0) SetVelocityY(0);

			
			// Translate above ground
			if (deepestFootPenetration < 0.0f)
			{
				Translate(0, -deepestFootPenetration, false);
			}
			// Most ground movements will want to keep us aligned with ground, so do that.
			else if (grounded && movement.ShouldSnapToGround && State != EnemyState.DEAD && State != EnemyState.DAMAGED) 
			{
				Translate(0, -distanceToGround, true);
			}
			// Set slope rotation
			if (feetCount > 0) SlopeTargetRotation = (rightFootSlope + leftFootSlope) / feetCount;
			
			// Unparent if we aren't on platform any longer
			if (enemyInteractsWithPlatforms && !hasParented)
			{
				transform.parent = null;
				if (parentPlatform !=null) 
				{
					parentPlatform.UnParent();
					parentPlatform = null;
				}
			}
		}

		/// <summary>
		/// Applies the Side collisions.
		/// </summary>
		virtual public void ApplySideCollisions()
		{
			float deepestSidePenetration = 0.0f;
			float smallestFraction = float.MaxValue;
			int closest = -1;
			int deepestDirection = 0;

			// Get collisions
			RaycastHit2D[] leftSideCollisions = leftSide.GetRaycastHits();
			RaycastHit2D[] rightSideCollisions = rightSide.GetRaycastHits();

			// Left Side - Only applied if standing still or moving left, you may need to override this
			// if your character is affected by fast moving platforms
			if (currentFacingDirection <= 0 || Velocity.x < 0)
			{
				for (int i = 0; i < leftSideCollisions.Length; i++)
				{	
					// Make sure we have a collision to support raycast colliders which use statically allocated hits arrays 
					if (leftSideCollisions[i].collider != null)
					{
						if (leftSideCollisions[i].fraction > 0 && leftSideCollisions[i].fraction < smallestFraction)
						{
							smallestFraction = leftSideCollisions[i].fraction;
							closest = i;
						}
					}  
				}
				// Found a collision
				if (closest != -1)
				{
					float penetration = leftSide.Length - (leftSideCollisions[closest].fraction * (leftSide.Length + leftSide.LookAhead));
					if (penetration > deepestSidePenetration)
					{
						deepestSidePenetration = penetration;
						deepestDirection = -1;
					}
				}
			}
			
			// Right Side - Only applied if standing still or moving right, you may need to override this
			// if your character is affected by fast moving platforms
			if (currentFacingDirection >= 0 || Velocity.x > 0)
			{
				closest = -1;
				for (int i = 0; i < rightSideCollisions.Length; i++)
				{	
					// Make sure we have a collision to support raycast colliders which use statically allocated hits arrays 
					if (rightSideCollisions[i].collider != null)
					{
						if (rightSideCollisions[i].fraction > 0 && rightSideCollisions[i].fraction < smallestFraction)
						{
							smallestFraction = rightSideCollisions[i].fraction;
							closest = i;
						}
					}
				}
				// Found a collision
				if (closest != -1)
				{				
					float penetration = rightSide.Length - (rightSideCollisions[closest].fraction * (rightSide.Length + rightSide.LookAhead));
					if (penetration > deepestSidePenetration)
					{
						deepestSidePenetration = penetration;
						deepestDirection = 1;
					}
				}
			}

			if (deepestSidePenetration > 0.0f)
			{
				Translate(-deepestDirection * deepestSidePenetration, 0, false);
				SwitchDirection();
				if (deepestDirection > 0 && Velocity.x > 0) SetVelocityX(0);
				if (deepestDirection < 0 && Velocity.x < 0) SetVelocityX(0);
			}

		}

		/// <summary>
		/// Check if a given collider should be ignored in collisions.
		/// </summary>
		/// <returns><c>true</c> if this instance is ignoring he specified collider; otherwise, <c>false</c>.</returns>
		/// <param name="collider">Collider.</param>
		virtual public bool IsColliderIgnored(Collider2D collider) {
			if (ignoredColliders.Contains (collider)) return true;
			return false;
		}
		
		/// <summary>
		/// Adds a collider to the list of ignored colliders.
		/// </summary>
		/// <param name="collider">Collider.</param>
		virtual public void AddIgnoredCollider(Collider2D collider)
		{
			ignoredColliders.Add (collider);
		}
		
		/// <summary>
		/// Removes a collider from the list of ignored colliders.
		/// </summary>
		/// <param name="collider">Collider.</param>
		virtual public void RemoveIgnoredCollider(Collider2D collider)
		{
			ignoredColliders.Remove (collider);
		}

		#endregion
	}
}