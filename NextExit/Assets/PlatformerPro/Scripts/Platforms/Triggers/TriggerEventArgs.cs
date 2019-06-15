using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Trigger event arguments.
	/// </summary>
	public class TriggerEventArgs : CharacterEventArgs
	{
		/// <summary>
		/// Gets or sets the trigger.
		/// </summary>
		/// <value>The character.</value>
		public Trigger Trigger
		{
			get;
			protected set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.CharacterEventArgs"/> class.
		/// </summary>
		/// <param name="character">Character.</param>
		public TriggerEventArgs(Trigger trigger, Character character) : base(character)
		{
			Trigger = trigger;
		}
	}
}
