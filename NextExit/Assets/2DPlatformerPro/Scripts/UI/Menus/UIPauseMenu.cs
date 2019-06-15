using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// User interface for the pause menu. It shows the background then hands over to another menu.
	/// </summary>
	public class UIPauseMenu : MonoBehaviour {

		/// <summary>
		/// The pointer image.
		/// </summary>
		[Tooltip ("The part of the menu that is actually drawn. This is activated/deactived as the game is paused/unpaused.")]
		public GameObject visibleComponent;

		/// <summary>
		/// The menu that contains the items to select.
		/// </summary>
		public GameObject actualMenuGo;

		/// <summary>
		/// The show effects.
		/// </summary>
		[Tooltip ("Effects to play when showing this menu.")]
		public List<FX_Base> showEffects;
		
		/// <summary>
		/// The hide effects.
		/// </summary>
		[Tooltip ("Effects to play when hiding this menu.")]
		public List<FX_Base> hideEffects;

		/// <summary>
		/// The menuComponent as IMenu.
		/// </summary>
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
		}

		/// <summary>
		/// Do the show.
		/// </summary>
		virtual protected IEnumerator DoShow()
		{
			yield return true; //new WaitForSeconds (0.1f);
			visibleComponent.SetActive (true);
			visibleComponent.SetActive (true);
			if (showEffects != null && showEffects.Count > 0)
			{
				foreach(FX_Base effect in showEffects)
				{
					effect.StartEffect();
				}
			}
		}

		/// <summary>
		/// Unpause this instance.
		/// </summary>
		virtual public bool UnPause()
		{
			// If you unpause and you are at top level menu then unpause
			if (UIBasicMenu.ActiveMenu == actualMenu || actualMenu == null)
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
		virtual protected IEnumerator DoHide()
		{
			yield return true;
			if (hideEffects != null && hideEffects.Count > 0)
			{
				foreach(FX_Base effect in hideEffects)
				{
					effect.StartEffect();
				}
			}
			else 
			{
				visibleComponent.SetActive (false);
			}
		}

	
	}
}
