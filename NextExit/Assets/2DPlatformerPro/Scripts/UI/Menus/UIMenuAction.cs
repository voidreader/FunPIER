using UnityEngine;
using System.Collections;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Actions that can occur by clicking a menu item.
	/// </summary>
	public enum UIMenuAction
	{
		SHOW_MENU,
		LOAD_SCENE,
		CONFIGURE_KEY,
		RESTORE_DEFAULT_KEYS,
		SWITCH_INPUT_TYPE,
		MUSIC_VOLUME,
		SFX_VOLUME,
		SEND_MESSAGE,
		ACTIVATE_GAMEOBJECT,
		DEACTIVATE_GAMEOBJECT,
		EXIT,
		PAUSE_AND_HIDE,
		USE_ITEM,
		USE_ITEM_AND_HIDE
	}

}
