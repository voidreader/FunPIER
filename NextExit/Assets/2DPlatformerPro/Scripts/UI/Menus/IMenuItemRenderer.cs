using UnityEngine;
using System.Collections;

namespace PlatformerPro.Extras
{
	public interface IMenuItemRenderer 
	{
		/// <summary>
		/// Update to show the specified menu item.
		/// </summary>
		/// <param name="item">Item.</param>
		void InitMenuItem(UIMenuItem item);

		/// <summary>
		/// Force renderer to redraw with latest data.
		/// </summary>
		void Refresh();

		/// <summary>
		/// Item was selected.
		/// </summary>
		void Select();

		/// <summary>
		/// Item was deselected.
		/// </summary>
		void Deselect();

		/// <summary>
		/// item was activated.
		/// </summary>
		void Activate();

		/// <summary>
		/// Item was deactivated.
		/// </summary>
		void Deactivate();

		/// <summary>
		/// Handles a click over by activating the given item. Call this from an event trigger.
		/// </summary>
		void Click ();
		
		/// <summary>
		/// Handles a mouse over by selecting the given item. Call this from an event trigger.
		/// </summary>
		void MouseOver();

	}
}