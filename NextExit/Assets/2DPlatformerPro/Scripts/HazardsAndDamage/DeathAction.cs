using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Data for an action that occurs at death (or on game over).
	/// </summary>
	[System.Serializable]
	public class DeathAction
	{
		/// <summary>
		/// What to do.
		/// </summary>
		public DeathActionType actionType;

		/// <summary>
		/// How long to wait before doing it.
		/// </summary>
		public float delay;

		/// <summary>
		/// A string for the scene (for DeathActionType.LOAD_ANOTHER_SCENE) or message name (for DeathActionType.SEND_MESSAGE).
		/// </summary>
		public string supportingData;

		/// <summary>
		/// Game object to receive messages (for DeathActionType.SEND_MESSAGE);
		/// </summary>
		public GameObject supportingGameObject;
	}

}