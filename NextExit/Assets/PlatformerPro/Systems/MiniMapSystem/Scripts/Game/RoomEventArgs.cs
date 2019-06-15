using UnityEngine;
using System.Collections;

namespace jnamobile.mmm {

	/// <summary>
	/// Event arguemnts for an event about a room.
	/// </summary>
	public class RoomEventArgs : System.EventArgs
	{
		/// <summary>
		/// Gets or sets the room.
		/// </summary>
		/// <value>The room.</value>
		public MapRoomData Room {
			get; protected set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="jnamobile.mmm.RoomEventArgs"/> class.
		/// </summary>
		/// <param name="room">Room.</param>
		public RoomEventArgs(MapRoomData room) {
			Room = room;
		}
	}
}