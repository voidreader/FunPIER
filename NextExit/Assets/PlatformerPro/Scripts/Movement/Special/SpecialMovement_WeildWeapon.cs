#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A special movement which plays the weild animation, and then sets an animator override. Because it sends events
	/// this movement cannot be selected from drop down and must be added directly.
	/// </summary>
	public class SpecialMovement_WeildWeapon : SpecialMovement
	{
		/// <summary>
		/// Name of the animation override to set.
		/// </summary>
		[Tooltip ("Name of the animation override to set.")]
		public string overrideName;

		/// <summary>
		/// Which action button triggers weilding.
		/// </summary>
		[Tooltip ("Index of the action button that triggers weilding.")]
		public int actionButton;

		/// <summary>
		/// List of movements to enable when weapon is weilded.
		/// </summary>
		[Tooltip ("List of movements to enable when weapon is weilded.")]
		public Movement[] weaponMovements;

		/// <summary>
		/// List of movements to disable when weapon is weilded.
		/// </summary>
		[Tooltip ("List of movements to disable when weapon is weilded.")]
		public Movement[] nonWeaponMovements;

		/// <summary>
		/// Are we weilding this weapon?
		/// </summary>
		protected bool weildActive;

		/// <summary>
		/// Have we started a weild
		/// </summary>
		protected bool weildStarted;

		/// <summary>
		/// Cached event arguments.
		/// </summary>
		protected CharacterEventArgs args;

		/// <summary>
		/// Cached reference to the animator.
		/// </summary>
		protected Animator myAnimator;

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		//	private const string Name = "Weild Weapon";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		// private const string Description = "A movement that lets you switch weapon by pressing a button. It plays an animation then sets an animation override. Use an event responder to enable and disable attacks.";
		
		/// <summary>
		/// Static movement info used by the editor.
		/// </summary>
		new public static MovementInfo Info
		{
			get
			{
				// Return null as this movement can't be added using the standard movement inspector.
				return new MovementInfo(null, null);
			}
		}

		#endregion

		/// <summary>
		/// Event for weilding a weapon, use this to do things like enable weapon attacks.
		/// </summary>
		public event System.EventHandler <CharacterEventArgs> WeildedWeapon;

		/// <summary>
		/// Raises the weilded weapon event.
		/// </summary>
		virtual protected void OnWeildedWeapon()
		{
			if (WeildedWeapon != null) WeildedWeapon (this, args);
		}

		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update()
		{
			if (weildStarted)
			{
				AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
				if (info.normalizedTime >= 1.0f)
				{
					// Check for animation end
					weildStarted = false;
					if (weildActive)
					{
						character.RemoveAnimationOverride(overrideName);
						foreach (Movement m in nonWeaponMovements)
						{
							m.Enabled = true;
						}
						weildActive = false;
					}
					else
					{
						character.AddAnimationOverride(overrideName);
						foreach (Movement m in weaponMovements)
						{
							m.Enabled = true;
						}
						weildActive = true;
					
					}
					// TODO Update this event to include details of what was weilded/unweilded
					OnWeildedWeapon();
				}
			}
		}
		
		/// <summary>
		/// Initialise the mvoement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			AssignReferences (character);
			args = new CharacterEventArgs (character);
			myAnimator = character.GetComponentInChildren<Animator> ();
			if (myAnimator == null) Debug.LogWarning ("WeildWeapon requires an animator to check for end of the weild weapon animation");
//			if (movementData != null && movementData.Length >= MovementVariableCount)
//			{
//				overrideName = movementData[OverrideNameIndex].StringValue;
//				actionButton = movementData[ActionButtonIndex].IntValue;
//
//			}
//			else
//			{
//				Debug.LogError("Invalid movement data.");
//			}
			return this;
		}

		/// <summary>
		/// Gets a value indicating whether this movement wants to do a special move.
		/// </summary>
		override public bool WantsSpecialMove()
		{
			if (character.Input.GetActionButtonState(actionButton) == ButtonState.DOWN)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Start the special mvoe
		/// </summary>
		override public void DoSpecialMove()
		{
			weildStarted = true;
			// Disable movements
			if (weildActive)
			{
				// Disable weapon movements
				foreach (Movement m in weaponMovements)
				{
					m.Enabled = false;
				}
			}
			else
			{
				// Disable non-weapon movements
				foreach (Movement m in nonWeaponMovements)
				{
					m.Enabled = false;
				}
			}
		}

		/// <summary>
		/// If this is true then the movement wants to maintain control of the character even
		/// if default transition conditions suggest it shouldn't.
		/// </summary>
		override public bool ForceMaintainControl()
		{
			if (weildStarted) return true;
		    return false;
		}

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				if (weildActive) return AnimationState.ATTACK_WEILD1;
				return AnimationState.ATTACK_WEILD0;
			}
		}

	}

}
