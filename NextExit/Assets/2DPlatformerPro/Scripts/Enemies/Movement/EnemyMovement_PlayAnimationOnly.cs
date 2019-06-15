using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Enemy movement which just plays an animation which is mapped to enemy state.
	/// </summary>
	public class EnemyMovement_PlayAnimationOnly : EnemyMovement, ICompletableMovement
	{

		#region members

		/// <summary>
		/// Which animation state to play?
		/// </summary>
		public List<EnemyStateToAnimation> animations;

		/// <summary>
		/// If true we will send a move compelte to the enemy when the normalised time reaches 1.0f.
		/// Requires an Animator Reference.
		/// </summary>
		public bool sendMoveComplete;

		/// <summary>
		/// Cached reference to the animator.
		/// </summary>
		public Animator myAnimator;

		/// <summary>
		/// Cached dictionary of the mappings.
		/// </summary>
		protected Dictionary<EnemyState, AnimationState> mappingLookup;

		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Play Animation Only";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "An enemy movement that simply plays an animation based on character state.";
		
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
				if (mappingLookup.ContainsKey(enemy.State))
				{
					return mappingLookup[enemy.State];
				}
				return AnimationState.NONE;
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
			// Create cached copy of mappins as lookup
			mappingLookup = new Dictionary<EnemyState, AnimationState>();
			// mappingLookup.Comparer = new System.Comparison<EnemyState> ();
			foreach (EnemyStateToAnimation m in animations)
			{
				mappingLookup.Add(m.state, m.animation);
			}
			if (sendMoveComplete && myAnimator == null)
			{
				myAnimator = enemy.GetComponentInChildren<Animator>();
				if (myAnimator == null) Debug.LogWarning("Send Move Complete is true but no animator is assigned, anbd one could not be found");
			}

			return this;
		}
		
		/// <summary>
		/// Moves the character.
		/// </summary>
		override public bool DoMove()
		{
			if (!mappingLookup.ContainsKey (enemy.State)) return false;
			if (sendMoveComplete)
			{
				AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
				if (info.IsName(mappingLookup[enemy.State].AsString()))
				{
					Debug.Log (info.normalizedTime );
					if (info.normalizedTime >= 1.0f) enemy.MovementComplete();
				}
			}
			return true;
		}

		#endregion
		
	}
}
