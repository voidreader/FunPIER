using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Renders a menu item using a two column layout.
	/// </summary>
	public class UIMenuItemRenderer_Volume : MonoBehaviour, IMenuItemRenderer
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
		/// The images that make up the volume bar.
		/// </summary>
		public Image[] volumeImages;

		/// <summary>
		/// A (typically invisible) 'pip' thats used to set zero volume on touch devices.
		/// </summary>
		public GameObject zeroVolumePip;

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
			for (int i = 0; i < volumeImages.Length; i++)
			{
				UIVolumePip pip = volumeImages[i].GetComponent<UIVolumePip>();
				if (pip != null && item is UIMenuItem_Volume)
				{
					pip.Init((UIMenuItem_Volume) item, (1.0f / volumeImages.Length) * (i + 1));
				}
			}
			if (zeroVolumePip != null)
			{
				UIVolumePip zvPip = zeroVolumePip.GetComponent<UIVolumePip>();
				if (zvPip != null && item is UIMenuItem_Volume)
				{
					zvPip.Init((UIMenuItem_Volume) item, 0);
				}
			}
			Refresh ();
		}

		/// <summary>
		/// Force renderer to redraw with latest data.
		/// </summary>
		virtual public void Refresh()
		{
			leftText.text = menuItem.Title;
			rightText.text = menuItem.ExtraInfo;
			if (menuItem is UIMenuItem_Volume) 
			{
				
				if (((UIMenuItem_Volume)menuItem).applyTo == VolumeType.MUSIC)
				{
					int count = (int)(AudioManager.Instance.MusicVolume * volumeImages.Length);
					for (int i = 0; i < volumeImages.Length; i++)
					{
						if (i < count) volumeImages[i].enabled = true;
						else volumeImages[i].enabled = false;
					}
				}
				else
				{
					int count = (int)(AudioManager.Instance.SfxVolume * volumeImages.Length);
					for (int i = 0; i < volumeImages.Length; i++)
					{
						if (i < count) volumeImages[i].enabled = true;
						else volumeImages[i].enabled = false;
					}
				}
			}
			else
			{
				for (int i = 0; i < volumeImages.Length; i++)
				{
					volumeImages[i].enabled = false;
				}
			}
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