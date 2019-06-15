using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Interface for objects that are pullable.
	/// </summary>
	public interface IPullable
	{
		/// <summary>
		/// Pull the pullable.
		/// </summary>
		/// <param name="character">Character doing the pulling.</param>
		/// <param name="amount">Amount being pulled.</param>
		void Pull(IMob character, Vector2 amount);

		/// <summary>
		/// Gets the objects mass.
		/// </summary>
		/// <value>The mass.</value>
		float Mass { get; }
	}
}
