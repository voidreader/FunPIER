#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// An enemy movement that simply runs in a given direction optionaly shotting projectiles.
	/// </summary>
	public class EnemyMovement_FreeFollowTarget : EnemyMovement
    { 

       
        [Header("Movement")]
        /// <summary>
        /// How fast does enemy swim/fly when chasing.
        /// </summary>
        [Tooltip ("How fast does enemy swim/fly when chasing.")]
        public float chasingSpeed = 3.0f;

        /// <summary>
        /// How fast does enemy swim/fly when returning home.
        /// </summary>
        [Tooltip("How fast does enemy swim/fly when returning home.")]
        public float returningSpeed = 2.0f;

        /// <summary>
        /// Stops the enemy spinning on the spot when really close to the player.
        /// </summary>
        [Tooltip("Stops the enemy spinning on the spot when really close to the player. Enemy will only turn when this far away.")]
        public float turnLeeway = 0.25f;

        [Header ("Detection")]
        /// <summary>
        /// Layers to check for characters.
        /// </summary>
        public LayerMask detectionLayers;

        /// <summary>
        /// Range of the proximity sense.
        /// </summary>
        [Tooltip("Range of the proximity sense.")]
        public float detectionRange = 5.0f;

        /// <summary>
        /// How far enemy will chase player before giving up and returning home.
        /// </summary>
        [Tooltip("How far enemy will chase player before giving up and returning home.")]
        public float travelRange = 10.0f;

        /// <summary>
        /// If we are returning home don't look for character for this long.
        /// </summary>
        [Tooltip("If we are returning home don't look for character for this long.")]
        public float returnHomeTime = 2.0f;

        [Header("Rotation")]
        /// <summary>
        /// If non null this game object will be rotated to face the travel direction.
        /// </summary>
        [Tooltip("If non null this game object will be rotated to face the travel direction.")]
        public GameObject rotationPoint;

        /// <summary>
        /// How fast to rotate.
        /// </summary>
        [Tooltip("How fast to rotate.")]
        public float rotationSpeed = 180.0f;

        /// <summary>
        /// Offset from rotatino point about which we rotate.
        /// </summary>
        [Tooltip ("Offset from rotatino point about which we rotate.")]
        public Vector3 rotationOffset;

        [Header("Geometry")]
        /// <summary>
        /// Layers to check for geometry.
        /// </summary>
        [Tooltip ("Layers to check for geometry.")]
        public LayerMask geometryLayers;

        /// <summary>
        /// How far we look past characters transform when checking for geometry.
        /// </summary>
        [Tooltip ("How far we look past characters transform when checking for geometry.")]
        public float lookahead = 0.5f;

        /// <summary>
        /// Home position.
        /// </summary>
        protected Vector2 initialPosition;

        /// <summary>
        /// Used to store results of detection overlap call.
        /// </summary>
        protected Collider2D[] proximityColliders;

        /// <summary>
        /// Used to store results of geometry detection
        /// </summary>
        protected RaycastHit2D[] geometryHits;

        /// <summary>
        /// Which way are we chasing.
        /// </summary>
        protected Vector2 currentChaseDirection;

        /// <summary>
        /// The return home timer.
        /// </summary>
        protected float returnHomeTimer;

        /// <summary>
        /// Track the facing direction.
        /// </summary>
        protected int facingDirection;

        /// <summary>
        /// Tracks how long we have been blocked by geometry.
        /// </summary>
        protected float noMoveTime;

        /// <summary>
        /// Direction we are moving in.
        /// </summary>
        protected Vector2 movement = Vector2.zero;

        #region constants

        /// <summary>
        /// Human readable name.
        /// </summary>
        private const string Name = "Free FollowTarget";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "An enemy movement that follows a target in any direction (flying or swimming) target.";

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
        /// Returns the direction the character is facing. 0 for none, 1 for right, -1 for left.
        /// </summary>
        override public int FacingDirection
		{
			get 
			{
                return facingDirection;
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
            initialPosition = transform.position;
            proximityColliders = new Collider2D[1];
            geometryHits = new RaycastHit2D[2];
            return this;
		}

        void Update()
        {
            if (returnHomeTimer > 0) returnHomeTimer -= TimeManager.FrameTime;
        }

        /// <summary>
        /// Moves the character.
        /// </summary>
        override public bool DoMove()
		{
			// This movement can be the default or it can handle the patrol state.
			if (enemy.State != EnemyState.DEFAULT && enemy.State != EnemyState.PATROL && enemy.State != EnemyState.CHARGING) return false;

           

            // Detect Player internally if the enemy is not sequence driven enemy
            if (!(enemy is SequenceDrivenEnemy))
            {
                if (returnHomeTimer <= 0) DetectTarget();
            }

            float distanceFromHome = Vector2.Distance(transform.position, initialPosition);

            // If target
            if (enemy.CurrentTarget != null)
            {
                Vector2 targetDirection = (enemy.CurrentTargetTransform.position - transform.position).normalized;
                float targetDistance = Vector2.Distance(enemy.CurrentTargetTransform.position, transform.position);
                if (distanceFromHome > travelRange)
                {
                    enemy.CurrentTarget = null;
                    returnHomeTimer = returnHomeTime;
                }
                else
                {
                    if (targetDistance > turnLeeway)
                    {
                        currentChaseDirection = targetDirection;
                    }
                    movement = currentChaseDirection * chasingSpeed * TimeManager.FrameTime;
                }
            }
            // No target (not else as we can clear target in the target bock)
            if (enemy.CurrentTarget == null)
            {
                Vector2 direction = (initialPosition - (Vector2)transform.position).normalized;
                movement = direction * returningSpeed * TimeManager.FrameTime;
                // Don't overshoot
                if (movement.magnitude >= distanceFromHome)
                {
                    movement = initialPosition - (Vector2)transform.position;
                }
            }

            // Move 
            if (movement.magnitude > 0.001f)
            {
                if (movement.x > 0) facingDirection = 1;
                else if (movement.x < 0) facingDirection = -1;
                else facingDirection = 0;
                if (CheckGeometry(movement))
                {
                    enemy.Translate(movement.x, movement.y, true);
                    if (rotationPoint != null) DoRotate(movement);
                    noMoveTime = 0;
                }
                else
                {
                    noMoveTime += TimeManager.FrameTime;
                    if (noMoveTime > 2.0f)
                    {
                        enemy.CurrentTarget = null;
                        returnHomeTimer = returnHomeTime;
                    }
                }
            } else { 
                if (enemy.LastFacedDirection == -1) DoRotate(Vector2.left);
                else DoRotate(Vector2.right);
            }
            return true;
		}

        virtual protected bool CheckGeometry(Vector2 dir)
        {
            int hit = Physics2D.RaycastNonAlloc(transform.position, dir, geometryHits, lookahead, geometryLayers);
            if (hit > 0) return false;
            return true;
        }

        virtual protected void DoRotate(Vector2 dir)
        {
            float targetRotation = 0.0f;
            float deg = Mathf.Rad2Deg * Mathf.Atan2(dir.x, dir.y);
            if (targetRotation < 0 && deg >= 180.0f) deg = -180.0f;
            targetRotation = deg;
            float difference = -targetRotation - rotationPoint.transform.localEulerAngles.z;
            // Shouldn't really happen but just in case
            if (difference > 180) difference = difference - 360;
            if (difference < -180) difference = difference + 360;
            Vector3 rotateAround = rotationPoint.transform.position + rotationOffset;

            if (difference > rotationSpeed * TimeManager.FrameTime) difference = rotationSpeed * TimeManager.FrameTime;
            if (difference < -rotationSpeed * TimeManager.FrameTime) difference = -rotationSpeed * TimeManager.FrameTime;
            rotationPoint.transform. RotateAround(rotateAround, new Vector3(0, 0, 1), difference);
        }

        virtual protected bool DetectTarget()
        {
            Character character = null;
            ICharacterReference characterRef = null;
            if (detectionRange > 0.0f)
            {
                int hits = Physics2D.OverlapCircleNonAlloc(transform.position, detectionRange, proximityColliders, detectionLayers);
                if (hits > 0)
                {
                    // Always pick first target in list
                    characterRef = (ICharacterReference)proximityColliders[0].gameObject.GetComponent(typeof(ICharacterReference));
                    if (characterRef == null)
                    {
                        character = proximityColliders[0].gameObject.GetComponent<Character>();
                    }
                    else
                    {
                        character = characterRef.Character;
                    }
                    if (enemy.CurrentTarget != character) 
                    {
                        enemy.CurrentTarget = character;
                        currentChaseDirection = (character.transform.position - transform.position).normalized;
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Called when the enemy hits the character.
        /// </summary>
        /// <param name="character">Character.</param>
        /// <param name="info">Damage info.</param>
        override public void HitCharacter(Character character, DamageInfo info)
		{
            enemy.CurrentTarget = null;
            returnHomeTimer = returnHomeTime;
        }
		
		/// <summary>
		/// Called by the enemy to switch (x) direction of the movement. Note that not all 
		/// movements need to support this, they may do nothing.
		/// </summary>
		override public void SwitchDirection()
		{
		
		}


        /// <summary>
        /// Called by the enemy to switch (x) direction of the movement. Note that not all 
        /// movements need to support this, they may do nothing.
        /// </summary>
        override public AnimationState AnimationState
        {
            get 
            {
                if (movement.magnitude > 0.001f) return AnimationState.SWIM;
                return AnimationState.IDLE;
            }
        }


        #endregion

        #region protected methods

        /// <summary>
        /// Set the direction of the charge.
        /// </summary>
        /// <param name="direction">Direction.</param>
        override public void SetDirection(Vector2 direction)
		{
			Debug.LogWarning ("EnemyMovememt_ChargeAtTarget doesn't support SetDirection . It always runs at target");
		}

        #endregion

#if UNITY_EDITOR
        /// <summary>
        /// Draw gizmos for showing sight range.
        /// </summary>
        virtual public void OnDrawGizmos()
        {
            Handles.color = new Color(1, 0, 0, 0.25f);
            Handles.DrawSolidDisc(transform.position, new Vector3(0, 0, 1), detectionRange);
            Handles.color = new Color(1, 0.64f, 0, 0.25f);
            Handles.DrawSolidDisc(transform.position, new Vector3(0, 0, 1), travelRange);

            Gizmos.color = Color.white;
            Handles.color = Color.white;
        }
#endif

    }

}