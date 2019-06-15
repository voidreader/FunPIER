using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// User interface for the pause menu. It shows the background then hands over to another menu.
	/// </summary>
	public class UIPauseMenu : UIScreen {

		/// <summary>
		/// The menu that contains the items to select.
		/// </summary>
		public GameObject actualMenuGo;

		/// <summary>
		/// If you show one or more inventories during pause pick the one you want to be focused by default.
		/// </summary>
		public UIInventory defaultInventory;

		/// <summary>
		/// The menuComponent as IMenu.
		/// </summary>
		// TODO Do we still need this
		protected IMenu actualMenu;

		/// <summary>
		/// Unity Start() hook.
		/// </summary>
		void Start()
		{
			Init ();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void Init()
		{
			if (actualMenuGo != null) 
			{
				actualMenu = (IMenu) actualMenuGo.GetComponent(typeof(IMenu));
				if (actualMenu == null) Debug.LogWarning ("Could not find IMenu on the PauseMenu's 'actualMenu'");
			}
		}

		/// <summary>
		/// Show this menu.
		/// </summary>
		virtual public void Pause() 
		{
			StartCoroutine (DoShow ());
			if (actualMenu != null) actualMenu.Show ();
			if (defaultInventory != null) UIInventory.SetActiveInventoryAndPosition (defaultInventory, 0);
		}

		/// <summary>
		/// Unpause this instance.
		/// </summary>
		virtual public bool UnPause()
		{
			// If you unpause and you are at top level menu then unpause
			if (UIBasicMenu.ActiveMenu == (UIBasicMenu) actualMenu || actualMenu == null)
			{
				StartCoroutine (DoHide ());
				if (actualMenu != null) actualMenu.Hide ();
				return true;
			}
			// But if not at top level menu instead move back through menu system
			else
			{
				if (UIBasicMenu.ActiveMenu.backItem != null) 
				{
					UIBasicMenu.ActiveMenu.backItem.DoAction();
				}
				else
				{
					Debug.LogWarning("Pressed back while in a pause menu sub-menu, but no back item could be found!");
				}
				return false;
			}
		}

		/// <summary>
		/// Do the hide.
		/// </summary>
		override public void Hide()
		{
			if (TimeManager.Instance.Paused) TimeManager.Instance.UnPause (false);
			base.Hide ();
		}

	}
}
