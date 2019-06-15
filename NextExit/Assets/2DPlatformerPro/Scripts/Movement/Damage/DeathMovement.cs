using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A wrapper class for handling the characters movement (or lack thereof) when the character is hit.
	/// </summary>
	public class DeathMovement : BaseMovement <DeathMovement>
	{

		#region movement info constants and properties
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Death Movement";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "The base death movement class, you shouldn't be seeing this did you forget to create a new MovementInfo?";

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

	}

}