using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Interface implemented by things taht draw a value provided by a UIValueProvider.
	/// </summary>
	public interface IValueRenderer
	{

		/// <summary>
		/// Render the value for a given provider.
		/// </summary>
		/// <param name="value">Provider to get value from.</param>
		void Render(UIValueProvider provider);
	}
}