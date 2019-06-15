using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Things the default character health can do when you die.
	/// </summary>
	[System.Flags]
	public enum DeathActionType
	{

		RESPAWN					=	1,		// Trigger the standard reswpawn scripts
		RELOAD_SCENE			=	2,		// Reload the current scene
		LOAD_ANOTHER_SCENE		=	4,		// Load an arbitrary scene 
		SEND_MESSAGE			=	8,		// Send message to a gameobject
		CLEAR_RESPAWN_POINTS	=	16,		// Clear respawn points
		DESTROY_CHARACTER		=	32,		// Destroy game object.
		RESET_DATA				=   64,		// Reset data for the persistable objects
		RESET_SCORE				=	128		// Reset the given score
	}

}