using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace jnamobile.mmm {

	/// <summary>
	/// Updates a text component to show the room name of the current room.
	/// </summary>
	[RequireComponent (typeof (Text))]
	public class UICurrentRoomName : MonoBehaviour {


		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start () {
			MapManager.Instance.EnteredRoom += HandleEnteredRoom;
		}

		/// <summary>
		/// Handles the entered room event by updating text.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void HandleEnteredRoom (object sender, RoomEventArgs e)
		{
			GetComponent<Text> ().text = e.Room.shortName;
		}

		/// <summary>
		/// On Destroy hook.
		/// </summary>
		void OnDestroy() {
			if (MapManager.Instance != null) MapManager.Instance.EnteredRoom -= HandleEnteredRoom;
		}
	}
}