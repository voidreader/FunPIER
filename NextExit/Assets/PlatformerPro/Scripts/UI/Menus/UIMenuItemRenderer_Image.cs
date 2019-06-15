using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Renders a menu item using the title as the name of a texture and the extra info as text.
	/// </summary>
	public class UIMenuItemRenderer_Image : MonoBehaviour, IMenuItemRenderer
	{

		/// <summary>
		/// The image to display the texture in.
		/// </summary>
		public Image image;

		/// <summary>
		/// The text field to display the title.
		/// </summary>
		public Text textField;

		/// <summary>
		/// The default color for the text.
		/// </summary>
		public Color defaultColor = new Color(0.5f,0.5f,0.5f,1.0f);

		/// <summary>
		/// The color of the text when selected.
		/// </summary>
		public Color selectedColor = Color.white;

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
			textField.color = defaultColor;
			image.color = defaultColor;
			Refresh ();
		}

		/// <summary>
		/// Force renderer to redraw with latest data.
		/// </summary>
		virtual public void Refresh()
		{
			textField.text = menuItem.ExtraInfo;
			if (image.mainTexture.name != menuItem.Title)
			{
				Sprite texture = (Sprite) Resources.Load (menuItem.Title , typeof(Sprite));
				image.sprite = texture;
			}
		}

		/// <summary>
		/// Item was selected.
		/// </summary>
		virtual public void Select()
		{
			textField.color = selectedColor;
			image.color = selectedColor;
		}

		/// <summary>
		/// Item was deselected.
		/// </summary>
		virtual public void Deselect()
		{
			textField.color = defaultColor;
			image.color = defaultColor;
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
		/// Handles a mouse over by selecting the given item. Call this from an event trigger.
		/// </summary>
		virtual public void MouseOver()
		{
			menu.Select (menuItem);
		}

		/// <summary>
		/// Handles a mouse over by selecting the given item. Call this from an event trigger.
		/// </summary>
		virtual public void Click()
		{
			menu.Activate (menuItem);
		}

	}
}