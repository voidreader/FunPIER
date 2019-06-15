using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Useful extenions for game objects
	/// </summary>
	public static class GameObjectExtensions
	{
		// TODO This is all done now.
//		/// <summary>
//		/// Gets a component of the given type in the ancestors or returns null if no component of the given type found in the ancestors.
//		/// The word 'Parent' is used because the functionality mirrors that of GetComponentInChildren()
//		/// </summary>
//		/// <returns>The component in parent.</returns>
//		/// <param name="go">Go.</param>
//		/// <typeparam name="T">The 1st type parameter.</typeparam>
//		public static T GetComponentInParent<T>(this GameObject go) where T : Component
//		{
//			T result = go.GetComponent<T>();
//			if (result != null) return result;
//			if (go.transform.parent != null) return go.transform.parent.gameObject.GetComponentInParent<T>();
//			return null;
//		}
//
		/// <summary>
		/// Gets the component in children including inactive.
		/// </summary>
		/// <returns>The component in children.</returns>
		/// <param name="includeInactive">If set to <c>true</c> include inactive.</param>
		public static T GetComponentInChildren<T>(this GameObject go, bool includeInactive) where T : Component
		{
			T[] allResults = go.GetComponentsInChildren<T>(includeInactive);
			if (allResults.Length > 0) return allResults[0];
			return null;
		}
	}

}