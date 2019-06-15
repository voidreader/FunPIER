using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Simple extension to basic attacks which checks for button HELD instead of button down.
	/// </summary>
	public class BasicAttacksWithAutoFire: BasicAttacks
	{
	
		#region movement info constants and properties
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Basic Attacks with Auto-fire";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "Adds autofire to basic attacks.";
		
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


		/// <summary>
		/// Is the input correct for the given attack. This implmenetation is simple a key press, but another could
		/// be more complex (queueable combo attacks, or complex key combinations).
		/// </summary>
		/// <returns><c>true</c>, if input was checked, <c>false</c> otherwise.</returns>
		/// <param name="attackData">Attack data.</param>
		override protected bool CheckInput(BasicAttackData attackData)
		{
			if (character.Input.GetActionButtonState(attackData.actionButtonIndex) == ButtonState.HELD ||
			    character.Input.GetActionButtonState(attackData.actionButtonIndex) == ButtonState.DOWN) return true;
			return false;
		}

	}

}