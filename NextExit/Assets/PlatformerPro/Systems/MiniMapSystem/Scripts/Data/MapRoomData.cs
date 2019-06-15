#define PLATFORMER_PRO
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace jnamobile.mmm {

	/// <summary>
	/// Stores data about a map room.
	/// </summary>
	[System.Serializable]
	public class MapRoomData {

		/// <summary>
		/// The fully qualified name.
		/// </summary>
		public string fqn;

		/// <summary>
		/// The short name.
		/// </summary>
		public string shortName;

		/// <summary>
		/// Cached position reference.
		/// </summary>
		protected Vector3 _position;

		/// <summary>
		/// Position of room.
		/// </summary>
		public Vector3 position {
			get {
				if (MapManager.Instance.worldOrientation == WorldOrientation.X_Y) {
					return _position;
				} else {
					return new Vector3 (_position.x, _position.z, _position.y);
				}
			}
			set {
				_position = value;
			}
		}

		/// <summary>
		/// Width in grid units
		/// </summary>
		public int width;

		/// <summary>
		/// Height in grid units.
		/// </summary>
		public int height;

		/// <summary>
		/// Name of the sprite to use when displaying this room.
		/// </summary>
		public string spriteName;

		/// <summary>
		/// Reference to the sprite to use when displaying this room.
		/// </summary>
		private Sprite overrideSprite;

		/// <summary>
		/// Gets the override sprite from the MapRoom if available or from SpriteDictionary if its not.
		/// </summary>
		public Sprite OverrideSprite {
			get {
				if (mapRoom != null) return mapRoom.overrideSprite;
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
		/// The color override.
		/// </summary>
		public Color colorOverride;

		/// <summary>
		/// Has this room been revealed?
		/// </summary>
		public bool revealed;

		/// <summary>
		/// Link to the map room if available.
		/// </summary>
		private MapRoom mapRoom;

		/// <summary>
		/// Get or set the MapRoom associated this data.
		/// </summary>
		[XmlIgnoreAttribute]
		public MapRoom MapRoom { 
			get {
				if (mapRoom == null || mapRoom.gameObject == null) mapRoom = null;
				return mapRoom;
			}
			set {
				mapRoom = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="jnamobile.mmm.MapRoomData"/> class.
		/// </summary>
		public  MapRoomData () {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="jnamobile.mmm.MapRoomData"/> class.
		/// </summary>
		/// <param name="mapRoom">Map room to pull data from.</param>
		public  MapRoomData (MapRoom mapRoom) {
			Apply (mapRoom);
		}

		/// <summary>
		/// Create a new MapRoomData with the given key (fqn).
		/// </summary>
		/// <returns>The key.</returns>
		public static MapRoomData FromKey(string key) {
			MapRoomData result = new MapRoomData ();
			result.fqn = key;
			return result;
		}

		/// <summary>
		/// Extract MapRoomData form the given MapRoom.
		/// </summary>
		/// <param name="mapRoom">The Map Room.</param>
		public void Apply(MapRoom mapRoom) {
			fqn = mapRoom.FullyQualifiedRoomName;
			shortName = mapRoom.RoomName;
			position = mapRoom.gameObject.transform.position;
			width = mapRoom.Width;
			height = mapRoom.Height;
			spriteName = mapRoom.overrideSpriteName;
			colorOverride = mapRoom.colorOverride;
			this.mapRoom = mapRoom;
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="jnamobile.mmm.MapRoomData"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="jnamobile.mmm.MapRoomData"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="jnamobile.mmm.MapRoomData"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals (object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals (this, obj))
				return true;
			if (obj.GetType () != typeof(MapRoomData))
				return false;
			MapRoomData other = (MapRoomData)obj;
			return fqn == other.fqn;
		}

		/// <summary>
		/// Serves as a hash function for a <see cref="jnamobile.mmm.MapRoomData"/> object.
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