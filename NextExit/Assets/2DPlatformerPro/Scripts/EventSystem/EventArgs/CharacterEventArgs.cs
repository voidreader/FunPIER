using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Character event arguments.
	/// </summary>
	public class CharacterEventArgs : System.EventArgs
	{
		/// <summary>
		/// Gets or sets the character.
		/// </summary>
		/// <value>The character.</value>
		public Character Character
		{
			get;
			protected set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.CharacterEventArgs"/> class.
		/// </summary>
		/// <param name="character">Character.</param>
		public CharacterEventArgs(Character character)
		{
			Character = character;
		}
	}
}
