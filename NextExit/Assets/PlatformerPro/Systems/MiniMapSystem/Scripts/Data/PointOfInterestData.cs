#define PLATFORMER_PRO
using UnityEngine;
using System.Collections;
using System.Xml.Serialization;

namespace jnamobile.mmm {

	/// <summary>
	/// Stores data about a POI.
	/// </summary>
	[System.Serializable]
	public class PointOfInterestData {

		/// <summary>
		/// The fully qualified name.
		/// </summary>
		public string fqn;

		/// <summary>
		/// The short name.
		/// </summary>
		public string shortName;

		/// <summary>
		/// Category/group for this POI.
		/// </summary>
		public string category;

		/// <summary>
		/// Position of the POI.
		/// </summary>
		public Vector3 position;

		/// <summary>
		/// Name of the sprite to use when displaying this POI.
		/// </summary>
		public string spriteName;

		/// <summary>
		/// Cached reference to the sprite (icon) to use when displaying this room.
		/// </summary>
		private Sprite overrideSprite;

		/// <summary>
		/// Gets the icon from the point of interest if avaialble or from SpriteDictionary if its not.
		/// </summary>
		public Sprite icon {
			get {
				if (pointOfInterest != null) return pointOfInterest.icon;
				if (overrideSprite != null) return overrideSprite;
				if (spriteName == null || spriteName == "NONE") return null;
				#if PLATFORMER_PRO
				overrideSprite = PlatformerPro.SpriteDictionary.GetSprite(spriteName);
				#else
				overrideSprite = SpriteDictionary.GetSprite(spriteName);
				#endif
				return overrideSprite;
			}
		}

		/// <summary>
		/// The color to use for this POI.
		/// </summary>
		public Color color;

		/// <summary>
		/// If true this PointOfInterest will be shown automatically.
		/// </summary>
		public bool revealed;

		/// <summary>
		/// If true this PointOfInterest will not be revealed on the map when player enters room.
		/// </summary>
		public bool hidden;


		/// <summary>
		/// Can this Point of Interest move or be collected? If 'true' UI will update position and visibility each frame. If false you will need to update it manually from code.
		/// </summary>
		public bool isDynamic;

		/// <summary>
		/// Link to the POI if available.
		/// </summary>
		private PointOfInterest pointOfInterest;

		/// <summary>
		/// Get or set the POI associated this data.
		/// </summary>
		[XmlIgnoreAttribute]
		public PointOfInterest PointOfInterest { 
			get {
				if (pointOfInterest == null || pointOfInterest.gameObject == null) pointOfInterest = null;
				return pointOfInterest;
			}
			set {
				pointOfInterest = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="jnamobile.mmm.PointOfInterestData"/> class.
		/// </summary>
		public  PointOfInterestData () {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="jnamobile.mmm.PointOfInterestData"/> class.
		/// </summary>
		/// <param name="poi">Poi.</param>
		public  PointOfInterestData (PointOfInterest poi) {
			Apply (poi);
		}
		/// <summary>
		/// Create a new PointOfInterestData object with the given key (fqn).
		/// </summary>
		/// <returns>The key.</returns>
		public static PointOfInterestData FromKey(string key) {
			PointOfInterestData result = new PointOfInterestData ();
			result.fqn = key;
			return result;
		}

		/// <summary>
		/// Extract PointOfInterestData from the given PointOfInterest and apply to this data object.
		/// </summary>
		/// <param name="poi">The POI.</param>
		public void Apply(PointOfInterest poi) {
			fqn = poi.FullyQualifiedPoiName;
			shortName = poi.PoiName;
			position = poi.gameObject.transform.position;
			spriteName = poi.iconName;
			color = poi.color;
			revealed = poi.revealed;
			hidden = poi.hidden;
			category = poi.category;
			this.pointOfInterest = poi;
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="jnamobile.mmm.PointOfInterestData"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="jnamobile.mmm.PointOfInterestData"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="jnamobile.mmm.PointOfInterestData"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals (object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals (this, obj))
				return true;
			if (obj.GetType () != typeof(PointOfInterestData))
				return false;
			PointOfInterestData other = (PointOfInterestData)obj;
			return fqn == other.fqn;
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="jnamobile.mmm.PointOfInterestData"/> object.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode ()
		{
			unchecked {
				return (fqn != null ? fqn.GetHashCode () : 0);
			}
		}
		

	}
}