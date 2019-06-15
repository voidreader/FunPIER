using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Possible type for an <c>BasicRaycast</c>. Different types drive different behaviours, for example
	/// a FOOT collider will generally only push the character upwards, whereas a HEAD collider will generally
	/// only push the character down.
	/// </summary>
	public enum RaycastType
	{
		NONE 		= 	0,
		SIDE_LEFT 	=	1,
		SIDE_RIGHT	=	2,
		FOOT		=	4,
		HEAD		=	8,
		SIDES		= 	SIDE_LEFT ^ SIDE_RIGHT,
		ALL 		= 	SIDES ^ HEAD ^ FOOT,
		ANY			=	ALL
	}

	/// <summary>
	/// Raycast type extensions.
	/// </summary>
	public static class RaycastTypeExtensions
	{
		/// <summary>
		/// Returns true if the this raycast type  is or contains the given raycast type.
		/// </summary>
		/// <param name="go">Go.</param>
		/// <param name="type">Type.</param>
		public static bool Contains(this RaycastType container, RaycastType type)
		{
			if ((container & type) == type) return true;
			return false;
		}
	}
}