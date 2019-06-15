#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A special movement which takes control while character is lokced on a rail platform.
	/// </summary>
	public class SpecialMovement_Rail : SpecialMovement
	{
		
		#region members

		/// <summary>
		/// Cached reference to the animator.
		/// </summary>
		protected Animator myAnimator;

		/// <summary>
		/// Current rail.
		/// </summary>
		protected RailPlatform rail;

		#endregion
		
		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Rail Movement";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "A special movement which takes control while character is lokced on a rail platform.";
		
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
		
		#region public methods
		
		
		/// <summary>
		/// Initialise the mvoement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			AssignReferences (character);
			
			myAnimator = character.GetComponentInChildren<Animator> ();
			if (myAnimator == null) Debug.LogWarning ("Rail special movement could not find an animator.");

			return this;
		}
		
		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				if (rail != null)
				{
					return rail.AnimationState;
				}
				return AnimationState.IDLE;
			}
		}

		/// <summary>
		/// Gets the facing direction.
		/// </summary>
		/// <value>The facing direction.</value>
		override public int FacingDirection
		{
			get 
			{
				if (rail != null)
				{
					return rail.facingDirection;
				}
				return 0;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this movement wants to do a special move.
		/// </summary>
		/// <value><c>true</c> if this instance wants control; otherwise, <c>false</c>.</value>
		override public bool WantsSpecialMove()
		{
			if (rail != null)
			{
				if (ShouldBeAttachedToRail(rail))
				{
					return true;
				}
				else
				{
					rail = null;
					return false;
				}
			}
			if (character.ParentPlatform is RailPlatform)
			{
				if (((RailPlatform)character.ParentPlatform).CheckForActivate(character, this))
				{
					return true;
				}
			}
			return false;
		}
		
		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{

		}

		/// <summary>
		/// Start the rail movement
		/// </summary>
		/// <param name="rail">The rail platform.</param>
		virtual public bool ShouldBeAttachedToRail(RailPlatform rail)
		{
			// Is my parent
			if (character.ParentPlatform != rail) return false;
			// Activated
			if (!rail.Activated) return false;
			// Action button
			if (rail.requiredActionButton != -1 && character.Input.GetActionButtonState(rail.requiredActionButton) != ButtonState.HELD) return false;
			// Must be grounded
			if (!character.Grounded) return false;
			// Jump
			if (rail.allowJump && character.DefaultAirMovement.WantsJump ()) return false;
			this.rail = rail;
			return true;
		}

		/// <summary>
		/// Called when the movement gets control. Typically used to do initialisation of velocity and the like.
		/// </summary>
		override public void GainControl()
		{
			character.SetVelocityX (0);
			character.SetVelocityY (0);
		}

		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions.
		/// </summary>
		override public void LosingControl()
		{
			character.SetVelocityX (0);
			character.SetVelocityY (0);
			rail = null;
		}


		/// <summary>
		/// Shoulds the apply gravity.
		/// </summary>
		/// <returns><c>true</c>, if apply gravity was shoulded, <c>false</c> otherwise.</returns>
		override public bool ShouldApplyGravity
		{
			get 
			{
				return true;
			}
		}
		
		/// <summary>
		/// Gets the should do base collisions.
		/// </summary>
		/// <value>The should do base collisions.</value>
		override public RaycastType ShouldDoBaseCollisions
		{
			get
			{
				return RaycastType.ALL;
			}
		}
		
		#endregion

		#if UNITY_EDITOR
		
		#region draw inspector
		
		/// <summary>
		/// Draws the inspector.
		/// </summary>
		public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{
			if (movementData == null || movementData.Length != 0)
			{
				movementData = new MovementVariable[0];
			}
			EditorGUILayout.HelpBox ("Movement settings are controlled by the Rail Platform", MessageType.Info);
			return movementData;
		}
		
		#endregion
		
		#endif
		
	}
	
}

