using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// String extensions.
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Does the string array contain the specified string.
		/// </summary>
		/// <param name="arr">Arr.</param>
		/// <param name="value">Value.</param>
		public static bool Contains(this string[] arr, string value)
		{
			if (arr == null || arr.Length == 0) return false;
			for (int i = 0 ; i < arr.Length; i++)
			{
				if (arr[i] == value) return true;
			}
			return false;
		}
	}
}