using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Door event arguments.
	/// </summary>
	public class DoorEventArgs : CharacterEventArgs
	{

		/// <summary>
		/// Gets or sets the door.
		/// </summary>
		public Door Door
		{
			get;
			protected set;
		}


		/// <summary>
		/// Gets or sets the door state.
		/// </summary>
		public DoorState DoorState
		{
			get;
			protected set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.DoorEventArgs"/> class.
		/// </summary>
		/// <param name="door">Door.</param>
		/// <param name="chracater">Chracater.</param>
		/// <param name="state">State.</param>
		public DoorEventArgs (Door door, Character character) : base (character)
		{
			Door = door;
		}

	}
	
}
