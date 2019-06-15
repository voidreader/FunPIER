using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace jnamobile.mmm {

	/// <summary>
	/// Collection of discovered/revealed rooms and points of interest that get saved by the MapManager.
	/// </summary>
	[System.Serializable]
	public class SaveData {
		
		/// <summary>
		/// Rooms that have been discovered/revealed.
		/// </summary>
		public List<MapRoomData> rooms;

		/// <summary>
		/// POIs that have been discovered/revealed
		/// </summary>
		public List<PointOfInterestData> pois;
	}
}