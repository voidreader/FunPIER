#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	public class PersistenceMenuItem  {

#if TOP_LEVEL_MENU
		[MenuItem ("Platformer PRO/Persistence/Reset All Player Prefs")]
#else
		[MenuItem ("Assets/Platformer PRO/Persistence/Reset All Player Prefs")]
#endif
		public static void ResetAllPlayerPrefs()
		{
			PlayerPrefs.DeleteAll ();
		}
	}
}

#endif