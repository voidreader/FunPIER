#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// An enemy movement that quickly snaps upwards at player if they come within range.
	/// </summary>
	public class EnemyMovement_VerticalSnapper : EnemyMovement
	{
		#region members

		[Header ("Movement")]

		/// <summary>
		/// The speed the enemy snaps upwards at.
		/// </summary>
		[Tooltip ("The speed the enemy charges upwards at. Must be grater than 0.")]
		public float speed = 5.0f;

		/// <summary>
		/// The speed the enemy retracts back at.
		/// </summary>
		[Tooltip ("The speed the enemy retracts back at. Must be grater than 0.")]
		public float retractSpeed = 2.0f;

		/// <summary>
		/// How far the enemy can charge upwards.
		/// </summary>
		[Tooltip ("How far the enemy can charge upwards.")]
		public float moveRange = 2.0f;

		/// <summary>
		/// If true automatically retract after reaching the top position. If false stay in top position.
		/// </summary>
		[Tooltip ("If true automatically retract after reaching the top position. If false stay in top position until character leaves sight range.")]
		public bool autoRetractAtTop = false;

		/// <summary>
		/// If true automatically retract after reaching the top position. If false stay in top position.
		/// </summary>
		[Tooltip ("If true snapper must fully retract before charging again. If false charge will start at any time (doesn't make sense if autoRetractAtTop is true).")]
		public bool mustFullyRetract = true;

		[Header ("Sight")]

		/// <summary>
		/// How far the enemy can 'see'.
		/// </summary>
		[Tooltip ("How far the enemy can 'see'.")]
		public float sightRange = 2.0f;

		/// <summary>
		/// The layers that trigger the snapping charge.
		/// </summary>
		[Tooltip ("The layers that trigger the snapping charge.")]
		public LayerMask sightLayers;

		[Header ("Damage")]

		/// <summary>
		/// The damage amount.
		/// </summary>
		[Tooltip ("The amount of damage done.")]
		public int damageAmount = 1;
		
		/// <summary>
		/// The type of the damage.
		/// </summary>
		[Tooltip ("The type of damage.")]
		public DamageType damageType = DamageType.PHYSICAL;

		[Header ("Animation")]

		/// <summary>
		/// The animation state to set while charging.
		/// </summary>
		[Tooltip ("The animation state to set while charging.")]
		public AnimationState animationState = AnimationState.ATTACK;

		/// <summary>
		/// The animation state to set while retracting.
		/// </summary>
		[Tooltip ("The animation state to set while retracting.")]
		public AnimationState animationStateRetract = AnimationState.WALK;
		
		/// <summary>
		/// The current state.
		/// </summary>
		protected SnapperState state = SnapperState.IDLE;

        /// <summary>
        /// The position we start at.
        /// </summary>
        protected Vector2 startingPos;

        /// <summary>
        /// The position we end at.
        /// </summary>
        protected Vector2 endingPos;

        #endregion

        #region constants

        /// <summary>
        /// Human readable name.
        /// </summary>
        private const string Name = "Vertical Snapper";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "An enemy movement that quickly snaps upwards at player if they come within range. Generally not used with an AI.";

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
				if (state == SnapperState.CHARGING) return animationState;
				if (state == SnapperState.RETRACTING) return animationStateRetract;
				return AnimationState.IDLE;
			}
		}
		
		/// <summary>
		/// Returns the direction the character is facing. 0 for none, 1 for right, -1 for left.
		/// </summary>
		override public int FacingDirection
		{
			get 
			{
				return 0;
			}
		}

		#endregion

		#region Unity hooks
		
		/// <summary>
		/// Unity Update() hook.
		/// </summary>
		void Update()
		{

		}
		
		#endregion

		#region public methods
		
		/// <summary>
		/// Initialise this movement and return a reference to the ready to use movement.
		/// </summary>
		override public EnemyMovement Init(Enemy enemy)
		{
			this.enemy = enemy;
            startingPos = enemy.transform.localPosition;
            endingPos = enemy.transform.localPosition + (enemy.transform.up * moveRange);
            if (retractSpeed < 0)
			{
				Debug.LogWarning("Retract speed was smaller than 0");
				retractSpeed = 0;
			}
			if (speed < 0)
			{
				Debug.LogWarning("Charge speed was smaller than 0");
				speed = 0;
			}
			if (autoRetractAtTop && !mustFullyRetract)
			{
				Debug.LogWarning("It doesn't make sense to have autoRetract == true and mustFullyRetract == false. Setting mustFullyRetract to true.");
				mustFullyRetract = true;
			}
			return this;
		}
		
		/// <summary>
		/// Moves the character.
		/// </summary>
		override public bool DoMove()
		{
			CheckForPlayer();
            float distToMove = 0;
            switch (state)
			{
               
				case SnapperState.CHARGING:
                    distToMove = TimeManager.FrameTime * speed;
                    if (distToMove > Vector2.Distance(enemy.transform.position, endingPos))
                    {
                        if (autoRetractAtTop) state = SnapperState.RETRACTING;
                        state = SnapperState.IDLE;
                        distToMove = Vector2.Distance(enemy.transform.position, endingPos);
                    }
                    enemy.Translate(0, distToMove, false);
                    break;
				case SnapperState.RETRACTING:
                    distToMove = TimeManager.FrameTime * -retractSpeed;
                    if (distToMove < -Vector2.Distance(enemy.transform.position, startingPos))
                    {
                        state = SnapperState.IDLE;
                        distToMove = -Vector2.Distance(enemy.transform.position, startingPos);
                    }
                    enemy.Translate(0, distToMove, false);
					break;
			}

		//	if (enemy.transform.position.y > endingYPos) enemy.Translate (0, endingYPos - enemy.transform.position.y, true);
		//	if (enemy.transform.position.y < startingYPos) enemy.Translate (0, startingYPos - enemy.transform.position.y, true);

			return true;
		}

		/// <summary>
		/// Called when the enemy hits the character.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="info">Damage info.</param>
		override public void HitCharacter(Character character, DamageInfo info)
		{

		}
		
		/// <summary>
		/// Called by the enemy to switch (x) direction of the movement. Note that not all 
		/// movements need to support this, they may do nothing.
		/// </summary>
		override public void SwitchDirection()
		{
			// Do nothing
		}

		#endregion

		#region protected methods

		/// <summary>
		/// Checks for the player and updates state.
		/// </summary>
		virtual protected void CheckForPlayer()
		{
			RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, sightRange, sightLayers);
			if (hit.collider != null)
			{
				Character character = null;
				ICharacterReference characterRef = (ICharacterReference)hit.collider.gameObject.GetComponent (typeof(ICharacterReference));
				if (characterRef == null)
				{
					character = hit.collider.gameObject.GetComponent<Character> ();
				} else
				{
					character = characterRef.Character;
				}
				if (character != null)
				{
					if (state == SnapperState.IDLE || !mustFullyRetract)
						state = SnapperState.CHARGING;
				} 
                else
				{
                    if (Vector2.Distance(enemy.transform.position, endingPos) < 0.01f)
                    {
                        state = SnapperState.RETRACTING;
                    }
                }
			}
			else
			{
                if (Vector2.Distance(enemy.transform.position, endingPos) < 0.01f)
                {
                    state = SnapperState.RETRACTING;
                }
            }
		}

		#endregion


#if UNITY_EDITOR

		/// Draw gizmos for showing sight range.
		/// </summary>
		virtual public void OnDrawGizmos()
		{
			
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position,  transform.position + transform.up * sightRange);
		}

#endif

	}

	/// <summary>
	/// States of the enemy snapper.
	/// </summary>
	public enum SnapperState
	{
		IDLE,
		CHARGING,
		RETRACTING
	}

}