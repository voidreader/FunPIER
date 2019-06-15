using UnityEngine;
using System;

namespace PlatformerPro
{

	/// <summary>
	/// An attribute used to indicate a property is a named property which can be changed through 
	/// the UpdateNamedProperty methods of the Character class.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
	public class TaggedProperty : System.Attribute
	{
		/// <summary>
		/// Property name.
		/// </summary>
		public string name;

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.NamedPropertyAttribute"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		public TaggedProperty(string name)
		{
			this.name = name;
		}
	}
}