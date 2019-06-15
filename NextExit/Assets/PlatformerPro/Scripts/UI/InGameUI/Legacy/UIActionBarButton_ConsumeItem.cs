using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A button on an action bar that consumes an item.
	/// </summary>
	public class UIActionBarButton_ConsumeItem : UIActionBarButton 
	{
		[Header ("Item Data")]
		/// <summary>
		/// The item.
		/// </summary>
		[SerializeField]
		protected string item;

		/// <summary>
		/// Init this instance.
		/// </summary>
		override protected void Init()
		{
			itemId = item;
			base.Init ();
		}

		/// <summary>
		/// Handles the click event.
		/// </summary>
		override protected void DoPointerClick()
		{
			if (!canClickWhenPaused && TimeManager.Instance.Paused) return;
			OnActivated ();
			itemManager.UseItem (itemId, 1);
		}

	}
}
