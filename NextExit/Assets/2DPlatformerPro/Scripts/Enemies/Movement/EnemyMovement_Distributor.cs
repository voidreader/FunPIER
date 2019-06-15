using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlatformerPro
{
	/// <summary>
	/// An enemy movement class which allows multiple movements to be combined by proxying off DoMove
	/// calls to the right movement based on the enemy state.
	/// </summary>
	[ExecuteInEditMode]
	public class EnemyMovement_Distributor : EnemyMovement {

		/// <summary>
		/// The array associating states to movements.
		/// </summary>
		public EnemyStateToMovement[] statesToMovements;

		/// <summary>
		/// The current movement.
		/// </summary>
		protected EnemyMovement currentMovement;

		/// <summary>
		/// Cached reference to the enemies normal movement.
		/// </summary>
		protected EnemyMovement defaultMovement;

		/// <summary>
		/// A list of movements from statesToMovements with each movement only appearing once.
		/// </summary>
		protected EnemyMovement[] uniqueMovements;

		#region movement info constants and properties
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Movement Distributor";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = " An enemy movement class which allows multiple movements to be combined by proxying off DoMove() calls to the right movement based on the enemy state.";
		
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
				if (currentMovement != null) return currentMovement.AnimationState;
				return AnimationState.NONE;
			}
		}
		
		/// <summary>
		/// Returns the direction the character is facing. 0 for none, 1 for right, -1 for left.
		/// </summary>
		override public int FacingDirection
		{
			get 
			{
				if (currentMovement != null) return currentMovement.FacingDirection;
				return 0;
			}
		}

		/// <summary>
		/// Gets the current movement.
		/// </summary>
		/// <value>The current movement.</value>
		virtual public EnemyMovement CurrentMovement
		{
			get { return currentMovement; }
		}

		#endregion

		#region public methods

		/// <summary>
		/// Initialise the movement with the given movement data.
		/// </summary>
		/// <param name="enemy">The enemy.</param>
		/// <param name="movementData">Movement data.</param>
		override public EnemyMovement Init(Enemy enemy)
		{
			base.Init (enemy);
			int anyMoveCount = 0;
			// Initialise and check each movement
			for (int i = 0; i < statesToMovements.Length; i++)
			{
				if (statesToMovements[i].state == EnemyState.DEFAULT) anyMoveCount++;

				if (statesToMovements[i].movement == null) 
				{
					Debug.LogError ("The state " + statesToMovements[i].state + " does not have a movement assigned.");
				}
				else 
				{
					statesToMovements[i].movement.Init (enemy);
					if (statesToMovements[i].state == EnemyState.DEFAULT) defaultMovement = statesToMovements[i].movement;
				}
			}

			// Report any move errors
			if (anyMoveCount == 0)  Debug.LogError ("You must have a move assigned to the DEFAULT enemy state");
			else if (anyMoveCount > 1) Debug.LogWarning ("You have more than one move assigned to the ANY state, only the first will be used.");

			currentMovement = defaultMovement;

			uniqueMovements = statesToMovements.Select (s => s.movement).Distinct().ToArray();
			
			return this;
		}

		
		/// <summary>
		/// Moves the character.
		/// </summary>
		override public bool DoMove()
		{
			EnemyMovement previousMovement = currentMovement;
			for (int i = 0; i < statesToMovements.Length; i++)
			{
				if (statesToMovements[i].state == enemy.State)
				{
					if (statesToMovements[i].movement != previousMovement)
					{
						if (previousMovement.LosingControl()) 
						{
							// The previous movement wants to hold on to control, don't update it.
							return true;
						}
						else
						{
							statesToMovements[i].movement.GainingControl();
							currentMovement = statesToMovements[i].movement;
						}
					}
					if (currentMovement.DoMove ()) return true;
				}
			}
			if (currentMovement == null) {
				currentMovement = defaultMovement;
				return currentMovement.DoMove();
			}
			return false;
		}
		
		/// <summary>
		/// Do the damage movement
		/// </summary>
		override public void DoDamage(DamageInfo info)
		{
			for (int i = 0; i < statesToMovements.Length; i++)
			{
				if (statesToMovements[i].state == EnemyState.DAMAGED) currentMovement = statesToMovements[i].movement;
			}
			currentMovement.DoDamage(info);
		}
		
		/// <summary>
		/// Do the death movement
		/// </summary>
		override public void DoDeath(DamageInfo info)
		{
			for (int i = 0; i < statesToMovements.Length; i++)
			{
				if (statesToMovements[i].state == EnemyState.DEAD) currentMovement = statesToMovements[i].movement;
			}
			currentMovement.DoDeath(info);
		}
		
		/// <summary>
		/// Called when the enemy hits the character.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="info">Damage info.</param>
		override public void HitCharacter(Character character, DamageInfo info)
		{
			for (int i = 0; i < statesToMovements.Length; i++)
			{
				if (statesToMovements[i].state == EnemyState.HITTING) currentMovement = statesToMovements[i].movement;
			}
			currentMovement.HitCharacter(character, info);
		}
		
		/// <summary>
		/// Called by the enemy to switch (x) direction of the movement. Note that not all 
		/// movements need to support this, they may do nothing.
		/// </summary>
		override public void SwitchDirection()
		{
			for (int i = 0; i < uniqueMovements.Length; i++)
			{
				uniqueMovements[i].SwitchDirection();
			}
		}

		#endregion

#if UNITY_EDITOR
		void OnEnable()
		{
			if (!Application.isPlaying)
			{
				if (statesToMovements == null || statesToMovements.Length == 0) Reset ();
			}
		}

		void Reset()
		{
			EnemyAI ai = (EnemyAI) gameObject.GetComponent (typeof(EnemyAI));
			statesToMovements = new EnemyStateToMovement[ai.Info.Length];
			if (ai != null) for (int i = 0; i <  ai.Info.Length; i++)
			{
				statesToMovements[i] = new EnemyStateToMovement();
				statesToMovements[i].state = ai.Info[i];
			}
		}
#endif
	}
}