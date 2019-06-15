using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Attack event arguments.
	/// </summary>
	public class AttackEventArgs : CharacterEventArgs
	{
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		public string Name
		{
			get;
			protected set;
		}

		// TODO Add some more data?

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.CharacterEventArgs"/> class.
		/// </summary>
		/// <param name="character">Character.</param>
		public AttackEventArgs(Character character, BasicAttackData attackData) : base (character)
		{
			Name = Name;
		}

		/// <summary>
		/// Updates the attack started arguments.
		/// </summary>
		/// <param name="attackData">Attack data.</param>
		virtual public void UpdateAttackStartedArgs(string name)
		{
			Name = name;
		} 
	}
}