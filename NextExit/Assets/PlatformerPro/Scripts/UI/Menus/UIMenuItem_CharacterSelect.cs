#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
#if !UNITY_4_6 && !UNITY_4_7 && !UNITY_5_1 && !UNITY_5_2
using UnityEngine.SceneManagement;
#endif
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Menu item for selecting a character.
	/// </summary>
	public class UIMenuItem_CharacterSelect : UIMenuItem
	{
		/// <summary>
		/// Name of the character. Will be supplied as extra info to renderers.
		/// </summary>
		[Tooltip ("Alternate name of the character to use. Supplied as ExtraInfo to renderers. The ID of the character should be in Supporting String.")]
		public string altCharacterName;

		/// <summary>
		/// If non-empty scene with this name will be loaded.
		/// </summary>
		[Tooltip ("If non-empty scene with this name will be loaded.")]
		public string sceneToLoadOnSelect;

		/// <summary>
		/// Gets additional info to display.
		/// </summary>
		override public string ExtraInfo
		{
			get { return altCharacterName; }
		}

		/// <summary>
		/// Indicates if this menu item should be active.
		/// </summary>
		override public bool IsActive
		{
			get { return true;}
		}

		/// <summary>
		/// Hitting the action key does nothing for this menu item type.
		/// </summary>
		override public void DoAction()
		{
			// MUTLIPLAYER TODO
			PlatformerProGameManager.SetCharacterForPlayer (0, supportingString);
			if (sceneToLoadOnSelect != null && sceneToLoadOnSelect != "") 
			{
				#if !UNITY_4_6 && !UNITY_4_7 && !UNITY_5_1 && !UNITY_5_2
				LevelManager.PreviousLevel = SceneManager.GetActiveScene().name;
				SceneManager.LoadScene(sceneToLoadOnSelect);
				#else
				LevelManager.PreviousLevel = Application.loadedLevelName;
				Application.LoadLevel(sceneToLoadOnSelect);
				#endif
			}
		}

	}
}