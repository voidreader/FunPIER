using UnityEngine;
using System.Collections;

namespace PlatformerPro.Extras
{
	public interface IMenu 
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PlatformerPro.Extras.IMenu"/> is accepting input.
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		bool AcceptingInput
		{
			get; set;
		}

		/// <summary>
		/// Gets the index of the currently selected item.
		/// </summary>
		/// <value>The current selecton.</value>
		int CurrentSelection
		{
			get;
		}

		/// <summary>
		/// Force UI to refresh.
		/// </summary>
		void Refresh();

		/// <summary>
		/// Show this menu.
		/// </summary>
		void Show();

		/// <summary>
		/// Hide this menu.
		/// </summary>
		void Hide();

		/// <summary>
		/// Select the specified menuItem.
		/// </summary>
		/// <param name="menuItem">Menu item.</param>
		void Select(UIMenuItem menuItem);

		/// <summary>
		/// Activate the specified menuItem.
		/// </summary>
		/// <param name="menuItem">Menu item.</param>
		void Activate(UIMenuItem menuItem);

	}
}