/**
 * This code is part of Platformer PRO and is copyright John Avery 2014.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A character reference can be attached to any object to provide an easy way to access the related character without
	/// needing to check for parents.
	/// </summary>
	public class CharacterReference : MonoBehaviour, ICharacterReference
	{
		/// <summary>
		/// The cached character reference.
		/// </summary>
		protected Character character;

		/// <summary>
		/// Gets the character reference.
		/// </summary>
		/// <value>The character.</value>
		public Character Character
		{
			get { return character; }
			set { character = value; }
		}

		/// <summary>
		/// Unity start hook.
		/// </summary>
		void Start()
		{
			Init ();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void Init()
		{
			character = gameObject.GetComponentInParent<Character>();
			if (character == null) 
			{
				Debug.LogError ("A CharacterReference must be the child of a character");
			}
		}
	}
}
