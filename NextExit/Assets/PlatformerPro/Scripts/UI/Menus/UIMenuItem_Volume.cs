using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace PlatformerPro.Extras
{
	
	/// <summary>
	/// Menu item which is used for configuring a key.
	/// </summary>
	public class UIMenuItem_Volume : UIMenuItem
	{

		/// <summary>
		/// The type of volume is this control for.
		/// </summary>
		[Tooltip ("The type of volume is this control for.")]
		public VolumeType applyTo;

		/// <summary>
		/// How much to increment (or decrement) the volume by.
		/// </summary>
		public float increment = 0.2f;
		
		/// <summary>
		/// Cached menu reference.
		/// </summary>
		protected UIBasicMenu menu;

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start()
		{
			menu = gameObject.GetComponentInParent<UIBasicMenu> ();
		}

		/// <summary>
		/// Does the action.
		/// </summary>
		override public void DoAction()
		{
			// Pressing enter/action does nothing.
		}

		/// <summary>
		/// Do the action for when the user presses right.
		/// </summary>
		override public void DoRightAction()
		{
			if (applyTo == VolumeType.MUSIC)
			{
				AudioManager.Instance.MusicVolume += increment;
				if (menu != null) menu.Refresh();
			}
			else
			{
				AudioManager.Instance.SfxVolume += increment;
				if (menu != null) menu.Refresh();
			}
		}

		/// <summary>
		/// Do the action for when the user presses left.
		/// </summary>
		override public void DoLeftAction()
		{
			if (applyTo == VolumeType.MUSIC)
			{
				AudioManager.Instance.MusicVolume -= increment;
				if (menu != null) menu.Refresh();
			}
			else
			{
				AudioManager.Instance.SfxVolume -= increment;
				if (menu != null) menu.Refresh();
			}
		}

		/// <summary>
		/// Sets the volume to a specific value.
		/// </summary>
		/// <param name="volume">Volume.</param>
		public void SetVolume(float volume)
		{
			if (AudioManager.Instance.MusicVolume != volume)
			{
				AudioManager.Instance.MusicVolume = volume;
				if (menu != null) menu.Refresh();
			}
		}

	}

	/// <summary>
	/// Volume type.
	/// </summary>
	public enum VolumeType
	{
		MUSIC,
		SFX
	}

}