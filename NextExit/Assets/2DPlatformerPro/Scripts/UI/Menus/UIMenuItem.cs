#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
#if !UNITY_4_6 && !UNITY_5_1 && !UNITY_5_2
using UnityEngine.SceneManagement;
#endif
using System.Collections;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// An item in a menu.
	/// </summary>
	public class UIMenuItem : MonoBehaviour
	{

		/// <summary>
		/// Title of the menu item.
		/// </summary>
		public string title;

		/// <summary>
		/// Action to take when the item is activated.
		/// </summary>
		public UIMenuAction action;

		/// <summary>
		/// String data supporting the menu item.
		/// </summary>
		public string supportingString;

		/// <summary>
		/// Game object supporting the menu item.
		/// </summary>
		public GameObject supportingGameObject;

		/// <summary>
		/// Gets the title.
		/// </summary>
		virtual public string Title
		{
			get { return title; }
		}

		/// <summary>
		/// Gets additional info to display.
		/// </summary>
		virtual public string ExtraInfo
		{
			get { return ""; }
		}

		/// <summary>
		/// Indicates if this menu item should be active.
		/// </summary>
		virtual public bool IsActive
		{
			get { return true;}
		}

		/// <summary>
		/// Do the menu action.
		/// </summary>
		virtual public void DoAction()
		{
			IMenu currentMenu;
			IMenu menu;
			switch(action)
			{
			case UIMenuAction.ACTIVATE_GAMEOBJECT:
				gameObject.SetActive(true);
				break;
			case UIMenuAction.DEACTIVATE_GAMEOBJECT:
				gameObject.SetActive(false);
				break;
			case UIMenuAction.LOAD_SCENE:
				foreach (Character c in FindObjectsOfType<Character>()) c.AboutToExitScene (supportingString);
				#if !UNITY_4_6 && !UNITY_5_1 && !UNITY_5_2
				LevelManager.PreviousLevel = SceneManager.GetActiveScene().name;
				SceneManager.LoadScene(supportingString);
				#else
				LevelManager.PreviousLevel = Application.loadedLevelName;
				Application.LoadLevel(supportingString);
				#endif
				break;
			case UIMenuAction.SEND_MESSAGE:
				supportingGameObject.SendMessage(supportingString, SendMessageOptions.DontRequireReceiver);
				break;
			case UIMenuAction.CONFIGURE_KEY:
				Debug.LogWarning ("You must use a UIMenuItem_KeyConfig when using the Configure Key action type.");
				break;
			case UIMenuAction.RESTORE_DEFAULT_KEYS:
				Debug.LogWarning ("You must use a UIMenuItem_KeyConfig restore default keys.");
				break;
			case UIMenuAction.SHOW_MENU:
				currentMenu = (IMenu)gameObject.GetComponentInParent(typeof(IMenu));
				menu = (IMenu)supportingGameObject.GetComponent(typeof(IMenu));
				if (currentMenu == null)
				{
					Debug.LogWarning ("Trying to open a sub-menu but can't find the current IMenu.");
				}
				else if (menu == null) 
				{
					Debug.LogWarning ("Trying to open a sub menu that isn't an IMenu");
				}
				else
				{
					currentMenu.Hide ();
					menu.Show ();
				}
				break;
			case UIMenuAction.MUSIC_VOLUME:
				Debug.LogWarning ("You must use a UIMenuItem_Volume when using the MUSIC_VOLUME action type.");
				break;
			case UIMenuAction.SFX_VOLUME:
				Debug.LogWarning ("You must use a UIMenuItem_Volume when using the SFX_VOLUME action type.");
				break;
			case UIMenuAction.PAUSE_AND_HIDE:
				TimeManager.Instance.TogglePause(false);
				IMenu pauseMenu = (IMenu)gameObject.GetComponentInParent(typeof(IMenu));
				pauseMenu.Hide ();
				break;
			case UIMenuAction.USE_ITEM:
				Debug.LogWarning ("You must use a UIMenuItem_Inventory when using the USE_ITEM action type.");
				break;
			case UIMenuAction.USE_ITEM_AND_HIDE:
				Debug.LogWarning ("You must use a UIMenuItem_Inventory when using the USE_ITEM_AND_HIDE action type.");
				break;
			default:
#if UNITY_EDITOR
				Debug.LogWarning ("Exit not supported in Editor");
#else
				Application.Quit();
#endif

				break;
			}
		}

		/// <summary>
		/// Do the action for when the user presses right.
		/// </summary>
		virtual public void DoRightAction()
		{
			// Do nothing
		}

		/// <summary>
		/// Do the action for when the user presses left.
		/// </summary>
		virtual public void DoLeftAction()
		{
			// Do nothing
		}

	}
}