using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Renders a menu item using a two column layout.
	/// </summary>
	public class UIMenuItemRenderer_TwoColumn : MonoBehaviour, IMenuItemRenderer
	{

		/// <summary>
		/// The left text.
		/// </summary>
		public Text leftText;

		/// <summary>
		/// The right text.
		/// </summary>
		public Text rightText;

		/// <summary>
		/// The default color for the text.
		/// </summary>
		public Color defaultColor = Color.white;

		/// <summary>
		/// The color of the text when selected.
		/// </summary>
		public Color selectedColor = Color.yellow;

		/// <summary>
		/// The menu item we are representing.
		/// </summary>
		protected UIMenuItem menuItem;

		/// <summary>
		/// The menu we belong to.
		/// </summary>
		protected IMenu menu;

		/// <summary>
		/// Update to show the specified menu item.
		/// </summary>
		/// <param name="item">Item.</param>
		virtual public void InitMenuItem(UIMenuItem item)
		{
			menuItem = item;
			menu = (IMenu) item.GetComponentInParent (typeof(IMenu));
			leftText.color = defaultColor;
			rightText.color = defaultColor;
			Refresh ();
		}

		/// <summary>
		/// Force renderer to redraw with latest data.
		/// </summary>
		virtual public void Refresh()
		{
			leftText.text = menuItem.Title;
			rightText.text = menuItem.ExtraInfo;
		}

		/// <summary>
		/// Item was selected.
		/// </summary>
		virtual public void Select()
		{
			leftText.color = selectedColor;
			rightText.color = selectedColor;
		}

		/// <summary>
		/// Item was deselected.
		/// </summary>
		virtual public void Deselect()
		{
			leftText.color = defaultColor;
			rightText.color = defaultColor;
		}

		/// <summary>
		/// item was activated.
		/// </summary>
		virtual public void Activate()
		{
			Refresh ();
		}

		/// <summary>
		/// Item was deactivated.
		/// </summary>
		virtual public void Deactivate()
		{
			Refresh ();
		}

		/// <summary>
		/// Handles a click over by activating the given item. Call this from an event trigger.
		/// </summary>
		virtual public void Click()
		{
			menu.Activate (menuItem);
		}

		/// <summary>
		/// Handles a mouse over by selecting the given item. Call this from an event trigger.
		/// </summary>
		virtual public void MouseOver()
		{
			menu.Select (menuItem);
		}
	}
}