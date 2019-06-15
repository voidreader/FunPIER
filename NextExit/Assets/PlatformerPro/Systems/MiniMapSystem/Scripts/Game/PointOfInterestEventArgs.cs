using UnityEngine;
using System.Collections;

namespace jnamobile.mmm {

	/// <summary>
	/// Event arguments for an event about a Point of Interest.
	/// </summary>
	public class PointOfInterestEventArgs : System.EventArgs
	{
		/// <summary>
		/// Gets or sets the room.
		/// </summary>
		/// <value>The room.</value>
		public PointOfInterestData PointOfInterest {
			get; protected set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="jnamobile.mmm.PoiEventArgs"/> class.
		/// </summary>
		/// <param name="poi">Point of interest.</param>
		/// <param name="room">Room.</param>
		public PointOfInterestEventArgs(PointOfInterestData poi) {
			PointOfInterest = poi;
		}
	}
}