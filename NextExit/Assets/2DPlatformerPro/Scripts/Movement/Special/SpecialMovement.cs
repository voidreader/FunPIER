using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A wrapper class for handling special moveemnts that dont fall in to the standard categories.
	/// </summary>
	public class SpecialMovement : BaseMovement <SpecialMovement>
	{

		#region movement info constants and properties
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Special Movement";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "The base special movement class, you shouldn't be seeing this did you forget to create a new MovementInfo?";

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
		/// Gets a value indicating whether this movement wants to do a special move.
		/// </summary>
		/// <value><c>true</c> if this instance wants control; otherwise, <c>false</c>.</value>
		virtual public bool WantsSpecialMove()
		{
			return false;
		}

		/// <summary>
		/// Start the special mvoe
		/// </summary>
		virtual public void DoSpecialMove()
		{

		}

	}
}