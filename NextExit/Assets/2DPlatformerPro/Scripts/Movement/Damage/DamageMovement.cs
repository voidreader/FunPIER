using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A wrapper class for handling the characters movement (or lack thereof) when the character is dying.
	/// </summary>
	public class DamageMovement : BaseMovement <DamageMovement>
	{

		#region movement info constants and properties
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Damage Movement";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "The base damage movement class, you shouldn't be seeing this did you forget to create a new MovementInfo?";

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
		/// Information about the damage sustained.
		/// </summary>
		protected DamageInfo damageInfo;

		/// <summary>
		/// Is this being used as a death movement.
		/// </summary>
		protected bool isDeath;

		/// <summary>
		/// For damage the default is to not apply gravity.
		/// </summary>
		override public bool ShouldApplyGravity
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Start the damage movement.
		/// </summary>
		/// <param name="info">Info.</param>
		/// <param name="isDeath">Is the movement being used for death.</param>
		virtual public void Damage(DamageInfo info, bool isDeath)
		{

		}

		/// <summary>
		/// Gets the priority of the animation state that this movement wants to set. Death and damage need higher defaults.
		/// </summary>
		override public int AnimationPriority
		{
			get 
			{
				if (isDeath) return 100;
				return 50;
			}
		}

	}

}