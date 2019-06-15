using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Different ways we can impleemnt the persisted state in the game world.
	/// </summary>
	public enum PersistableObjectType
	{
		ACTIVATE_DEACTIVATE,
		DESTROY,
		SEND_MESSAGE,
		CUSTOM
	}

	/// <summary>
	/// Different ways we can impleemnt the persisted state for an enemy.
	/// </summary>
	[System.Flags]
	public enum PersistableEnemyType
	{
		ALIVE_DEAD  			= 1,
		HEALTH					= 2,
		STATE					= 4,
		POSITION_AND_VELOCITY	= 8
	}

	public static class PersistableObjectTypeExtensions
	{
		public static string GetDescription(this PersistableObjectType me)
		{
			switch(me)

			{
			case PersistableObjectType.ACTIVATE_DEACTIVATE: return "Activate or deactivate GameObject based on persistence state.";
			case PersistableObjectType.DESTROY: return "Destroy GameObject if persistence state is false.";
			case PersistableObjectType.SEND_MESSAGE: return "Send SetPersistenceState() message to the GameObject.";
			case PersistableObjectType.CUSTOM: return "Do a custom persistable action. There are inbuilt actions for Doors, RespawnPoints, Enemies and Platforms, but you can easily write your own.";
			}
			return "No information available.";
		}
	}

}
