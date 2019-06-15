using UnityEngine;
using System.Collections;

namespace PlatformerPro.Extras
{
	public class UIVolumePip : MonoBehaviour
	{
		/// <summary>
		/// The volume.
		/// </summary>
		protected float volume;

		/// <summary>
		/// The volume item.
		/// </summary>
		protected UIMenuItem_Volume volumeItem;
		
		/// <summary>
		/// The menu we belong to.
		/// </summary>
		protected IMenu menu;

		/// <summary>
		/// Init with the specified volumeItem and volume.
		/// </summary>
		/// <param name="volumeItem">Volume item.</param>
		/// <param name="volume">Volume.</param>
		virtual public void Init(UIMenuItem_Volume volumeItem, float volume)
		{
			this.volume = volume;
			this.volumeItem = volumeItem;
			menu = (IMenu) volumeItem.GetComponentInParent (typeof(IMenu));
		}
		
		/// <summary>
		/// Handles a mouse over by selecting the given item. Call this from an event trigger.
		/// </summary>
		virtual public void MouseOver()
		{
			menu.Select (volumeItem);
			volumeItem.SetVolume(volume);
		}

	}
}