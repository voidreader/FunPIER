#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// An enemy movement that flies up and down.
	/// </summary>
	public class EnemyMovement_FlyUpAndDown : EnemyMovement
	{
		#region members

		/// <summary>
		/// The distance from starting position to the top extent.
		/// </summary>
		public float topOffset;
		
		/// <summary>
		/// The distance from starting position to the bottom extent.
		/// </summary>
		public float bottomOffset;
		
		/// <summary>
		/// The speed the enemy moves at.
		/// </summary>
		public float speed;

		/// <summary>
		/// The top extent.
		/// </summary>
		protected float topExtent;
		
		/// <summary>
		/// The bottom extent.
		/// </summary>
		protected float bottomExtent;

		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Fly";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "An enemy movement that flies up and down. It doesn't consider any geometry or collisions, it flies a fixed path.";

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
				// We are always walking if patrolling
				return AnimationState.FLY;
			}
		}
		
		/// <summary>
		/// Returns the direction the character is facing. 0 for none, 1 for right, -1 for left.
		/// </summary>
		override public int FacingDirection
		{
			get 
			{
				if (speed > 0) return 1;
				if (speed < 0) return -1;
				return 0;
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

			topExtent = enemy.transform.position.y + topOffset;
			bottomExtent = enemy.transform.position.y - bottomOffset;

			return this;
		}
		
		/// <summary>
		/// Moves the character.
		/// </summary>
		override public bool DoMove()
		{
			// This movement can be the default or it can handle the patrol state or falling states.
			if (enemy.State != EnemyState.DEFAULT && enemy.State != EnemyState.PATROL && enemy.State != EnemyState.FLYING ) 
			{
				Debug.LogWarning("The Fly movement cant handle the state: " + enemy.State);
				return false;
			}

			if (speed > 0)
			{
				// We have the additional check so we can beter support enemies starting at the wrong spot
				if (enemy.Transform.position.y >= topExtent)
				{
					speed *= -1;
				}
				else
				{
					enemy.Translate(0, speed * TimeManager.FrameTime, false);
					if (enemy.Transform.position.y > topExtent)
					{
						// Should we set this directly or add a new method to enemy?
						enemy.Transform.position = new Vector3(enemy.Transform.position.x, topExtent, enemy.Transform.position.z);
						speed *= -1;
					}
				}
			}
			else if (speed < 0)
			{
				if (enemy.Transform.position.y <= bottomExtent)
				{
					speed *= -1;
				}
				else
				{
					enemy.Translate(0, speed * TimeManager.FrameTime, false);
					if (enemy.Transform.position.y < bottomExtent)
					{
						// Should we set this directly or add a new method to enemy?
						enemy.Transform.position = new Vector3(enemy.Transform.position.x, bottomExtent, enemy.Transform.position.z);
						speed *= -1;
					}
				}
			}
			return true;
		}

		#endregion

#if UNITY_EDITOR

		/// <summary>
		/// Draw handles for showing extents.
		/// </summary>
		void OnDrawGizmos()
		{
			DrawGizmos();
		}

		/// <summary>
		/// Draw gizmos for showing extents.
		/// </summary>
		virtual public void DrawGizmos()
		{
			// These handles don't make sense once the game is playing as they would move
			if (!Application.isPlaying)
			{
				// TODO Better bounds, currently it shows the centre position

				Gizmos.color = Color.red;
				Gizmos.DrawLine(transform.position,  transform.position + new Vector3(0, topOffset, 0));
				Gizmos.DrawLine(transform.position + new Vector3(0.25f, topOffset, 0), transform.position + new Vector3(-0.25f, topOffset, 0));
				Gizmos.DrawLine(transform.position,  transform.position + new Vector3(0, -bottomOffset, 0));
				Gizmos.DrawLine(transform.position + new Vector3(0.25f, -bottomOffset, 0), transform.position + new Vector3(-0.25f, -bottomOffset, 0));			
			}
		}
#endif

	}

}