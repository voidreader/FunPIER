/**
 * This code is part of Platformer PRO and is copyright John Avery 2014.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// This is the basic behaviour from which all movement behaviours are derived. To create your 
	/// own movement extend this class.
	/// </summary>
	public abstract class Movement : MonoBehaviour
	{
		#region members

		/// <summary>
		/// Cached reference to the character.
		/// </summary>
		protected Character character;

		#endregion

		#region properties

#if UNITY_EDITOR
		/// <summary>
		/// Sets the character for this movement.
		/// </summary>
		virtual public Character Character
		{
			get
			{
				Character character  = GetComponentInParent<Character>();
				if (character == null) Debug.LogWarning("Make sure your character is Active when configuring movements.");
				return character;
			}
		}
#endif

		/// <summary>
		/// A custom enable which base movements can use to pass on enable values.
		/// </summary>
		/// <value>The enabled.</value>
		virtual public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="PlatformerPro.Movement"/> expects
		/// gravity to be applied after its movement finishes.
		/// </summary>
		virtual public bool ShouldApplyGravity
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Gets a value indicating the current gravity, only used if this
		/// movement doesn't apply the default gravity.
		/// <seealso cref="ShouldApplyGravity()"/>
		/// </summary>
		virtual public float CurrentGravity
		{
			get
			{
				return character.DefaultGravity;
			}
		}

		
		/// <summary>
		/// Gets a value indicating whether this <see cref="PlatformerPro.Movement"/> expects the
		/// base collisions to be executed after its movement finishes.
		/// </summary>
		virtual public RaycastType ShouldDoBaseCollisions
		{
			get
			{
				return RaycastType.ALL;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="PlatformerPro.Movement"/> expects the
		/// rotations to be calculated and applied by the character.
		/// </summary>
		virtual public bool ShouldDoRotations
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		virtual public AnimationState AnimationState
		{
			get 
			{
				return AnimationState.IDLE;
			}
		}

		/// <summary>
		/// Gets the animation override state that this movement wants to set.
		/// </summary>
		virtual public string OverrideState
		{
			get 
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the priority of the animation state that this movement wants to set.
		/// </summary>
		virtual public int AnimationPriority
		{
			get 
			{
				return 0;
			}
		}

		/// <summary>
		/// Returns the direction the character is facing. 0 for none, 1 for right, -1 for left.
		/// </summary>
		virtual public int FacingDirection
		{
			get 
			{
				return 0;
			}
		}

		/// <summary>
		/// If true then don't allow base collisions to reset velocity in X.
		/// </summary>
		virtual public bool PreventXVelocityReset
		{
			get
			{
				return false;
			}
		}

		#endregion

		#region movement info constants and properties
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Movement";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "The base movement class, you shound't be seeing this did you forget to override MovementInfo?";
		
		/// <summary>
		/// Static movement info used by the editor.
		/// </summary>
		public static MovementInfo Info
		{
			get
			{
				return new MovementInfo(Name, Description);
			}
		}
		
		#endregion

		#region public methods

		/// <summary>
		/// Initialise this movement and retyrn a reference to the ready to use movement.
		/// </summary>
		virtual public Movement Init(Character character)
		{
			this.character = character;
			return this;
		}

		
		/// <summary>
		/// Initialise the movement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		virtual public Movement Init(Character character, MovementVariable[] movementData)
		{
			Debug.LogError("The Movement class shouldn't be initialised with movement data. Did you forget to override the Init method?");
			return this;
		}

		/// <summary>
		/// Called after initialisation to check if this movement is configured correctly. 
		/// Typically used to stop wrapper movements being the default and ending up in infinite loops.
		/// </summary>
		virtual public string PostInitValidation()
		{
			return null;
		}

		/// <summary>
		/// Moves the character.
		/// </summary>
		virtual public void DoMove()
		{

		}

		/// <summary>
		/// Does any movement that MUST be done after collissions are calculated.
		/// </summary>
		virtual public void PostCollisionDoMove()
		{
			
		}

		/// <summary>
		/// If this is true then the movement wants to maintain control of the character even
		/// if default transition conditions suggest it shouldn't.
		/// </summary>
		/// <returns><c>true</c> if control is wanted, <c>false</c> otherwise.</returns>
		virtual public bool WantsControl()
		{
			return false;
		}

		/// <summary>
		/// Called when the movement gets control. Typically used to do initialisation of velocity and the like.
		/// </summary>
		virtual public void GainControl()
		{
		
		}

		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions.
		/// </summary>
		virtual public void LosingControl()
		{
		}

		/// <summary>
		/// Determines whether this character is grounded. Only applied if ShouldDoBaseCollisions returned false this frame.
		/// Else the IsGrounded() method from the base collisions will be used.
		/// </summary>
		/// <returns><c>true</c> if this character is grounded; otherwise, <c>false</c>.</returns>
		virtual public bool IsGrounded()
		{
			return false;
		}

		/// <summary>
		/// How does this movement use Velocity.
		/// </summary>
		virtual public VelocityType VelocityType
		{
			get
			{
				return VelocityType.RELATIVE_X_WORLD_Y;
			}
		}

		#endregion

		#region utiltiy methods

		/// <summary>
		/// Utility method which checks for side collissions.
		/// </summary>
		/// <returns><c>true</c>, if sides hit something, <c>false</c> otherwise.</returns>
		/// <param name="character">Character to check.</param>
		/// <param name="requiredHits">Required number of side hits.</param>
		/// <param name="types">Side types to check.</param>
		static public bool CheckSideCollisions(Character character, int requiredHits, RaycastType types)
		{
			int hitCount = 0;
			for (int i = 0; i < character.Colliders.Length; i++)
			{
				if ((character.Colliders[i].RaycastType & types) == character.Colliders[i].RaycastType)
				{
					RaycastHit2D hit = character.GetClosestCollision(i);
					if (hit.collider != null)
					{
						hitCount++;
						if (hitCount >= requiredHits) return true;
					}
				}
			}
			return false;
		}

		#endregion
	}

}