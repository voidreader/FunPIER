using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// An enemy AI is typically extended for SIMPLE enemy AI controllers and is used to move control between basic enemy 
	/// movements. For COMPLEX AI you will most likely implement your enemy as a character and your enemy AI as an Input
	/// to that character.
	/// </summary>
	public class EnemyAI : MonoBehaviour
	{

		/// <summary>
		/// How long between decisions, increasing this improves performance but makes the enemey less responsive.
		/// </summary>
		[Tooltip ("How long between decisions, increasing this improves performance but makes the enemey less responsive.")]
		public float decisionInterval;

		/// <summary>
		/// Timer for tracking decision interval.
		/// </summary>
		protected float decisionTimer;

		/// <summary>
		/// Enemy reference.
		/// </summary>
		protected Enemy enemy;

		/// <summary>
		/// Update the timer by frame time, we drive this externally so we can halt the mob from a single place (the Enemy script).
		/// </summary>
		virtual public bool UpdateTimer()
		{
			decisionTimer -= TimeManager.FrameTime;
			if (decisionTimer <= 0) 
			{
				decisionTimer = decisionInterval;
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// Decide the next move.
		/// </summary>
		virtual public EnemyState Decide()
		{
			return EnemyState.DEFAULT;
		}

		/// <summary>
		/// The sense routine used to detect when something changes. Sense is called 
		/// every frame and should be kept as simple as possible. Use Decide() for more
		/// complex logic.
		/// </summary>
		virtual public bool Sense()
		{
			return false;
		}

		/// <summary>
		/// Init this enemy AI.
		/// </summary>
		/// <param name="enemy">Enemy we are the brain for.</param>
		virtual public void Init(Enemy enemy)
		{
			this.enemy = enemy;
		}

		/// <summary>
		/// Gets or sets the current target (or null if there is no target).
		/// </summary>
		virtual public Character CurrentTarget
		{
			get; set;
		}

		/// <summary>
		/// Used to inform the AI we were damaged so an action like FLEE may be triggered.
		/// </summary>
		virtual public void Damaged()
		{
		}

#if UNITY_EDITOR

		/// <summary>
		/// Static info used by the editor.
		/// </summary>
		virtual public EnemyState[] Info
		{
			get
			{
				return new EnemyState[]{EnemyState.DEFAULT};
			}
		}

#endif
	}

}