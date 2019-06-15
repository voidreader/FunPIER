using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// This data class stores information about the movement which is used by the editor.
	/// </summary>
	public class MovementInfo
	{
		/// <summary>
		/// The name of the movement.
		/// </summary>
		public string Name
		{
			get; protected set;
		}

		/// <summary>
		/// Rich text description of the movement.
		/// </summary>
		public string Description
		{
			get; protected set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.MovementInfo"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="description">Description.</param>
		public MovementInfo(string name, string description)
		{
			this.Name = name;
			this.Description = description;
		}
	}

}