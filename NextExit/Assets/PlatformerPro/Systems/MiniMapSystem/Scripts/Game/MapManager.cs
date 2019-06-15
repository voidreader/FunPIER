#define PLATFORMER_PRO

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.IO;
#if PLATFORMER_PRO
using PlatformerPro;
#endif

namespace jnamobile.mmm {

	/// <summary>
	/// Managed high-level map settings.
	/// </summary>
	public class MapManager : MonoBehaviour {

		/// <summary>
		/// The smallest size that can be indiated on the map. Must be same size or smaller than the smallest room size.
		/// </summary>
		[Tooltip ("The smallest size that can be indiated on the map. Must be same size or smaller than the smallest room size.")]
		public float gridSize;

		/// <summary>
		/// The GameObject that makes map pieces visible when it enters them. Usually the player.
		/// </summary>
		[Tooltip ("The GameObject that makes map pieces visible when it enters them. Usually the player.")]
		public GameObject player;

		/// <summary>
		/// Tag to use to find the player if not specified above.
		/// </summary>
		[Tooltip ("If the 'player' GameObject is empty we will use the tag below to try and find a player.")]
		public string findPlayerByTag = "Player";

		/// <summary>
		/// Is the mapy X/Y or X/Z?
		/// </summary>
		#if PLATFORMER_PRO
		[HideInInspector]
		#endif
		[Tooltip ("Is the map XY or XZ oriented.")]
		public WorldOrientation worldOrientation;

		[Header ("Persistence")]

		/// <summary>
		/// Name to use when saving and loading the map.
		/// </summary>
		[ContextMenuItem("Reset", "ResetSaveData")]
		[Tooltip ("Name to use when saving and loading the map. If you have more than one map this must be unique." )]
		public string mapName = "Map";

		/// <summary>
		/// Should we auto save the map whenever a room is entered?
		/// </summary>
		[SerializeField]
		[Tooltip ("Should we auto save the map whenever a room is entered? If false you will need to call Save() manually")]
		protected bool autoSave = true;
		#if PLATFORMER_PRO
		[Header ("Platformer PRO")]

		/// <summary>
		/// Should we show rooms that the player hasn't yet visited.
		/// </summary>
		[Tooltip ("By default do we show all rooms the player hasn't visited?")]
		public bool showUnrevealedRooms;

		/// <summary>
		/// Any strings in this list will be automaticall shown on the map as POI.
		/// </summary>
		[Tooltip ("How do we treat ites?")]
		public List<ItemToPoiMapping> itemsShownOnMap;

		/// <summary>
		/// The door poi data.
		/// </summary>
		[Tooltip ("How do we handle doors?")]
		public ExtraPoiData doorPoiData;

		/// <summary>
		/// The respawn poi data.
		/// </summary>
		[Tooltip ("How do we handle doors?")]
		public ExtraPoiData respawnPoiData;

		#endif

		[Header ("Debugging")]
		/// <summary>
		/// If true show log messages via Debug.Log
		/// </summary>
		public bool showLogMessages;

		/// <summary>
		/// The currently active room. i.e. the room the player is in.
		/// </summary>
		protected MapRoom currentRoom;

		/// <summary>
		/// Save counter, we save when this equals zero.
		/// </summary>
		protected int needsToSave;

		/// <summary>
		/// Clear all map data. Call this before loading a new map scene.
		/// </summary>
		virtual public void Reset() {
			MapRoom.ResetAllMapRooms ();
			PointOfInterest.ResetAllPointsOfInterest ();
			currentRoom = null;
			Instance = null;
		}

		/// <summary>
		/// Anythign this far away is considered out of range no matter what.
		/// </summary>
		/// <value>The clearly out distance.</value>
		virtual public float ClearlyOutDistance {
			get {
				return gridSize * 100.0f;
			}
		}
		
		/// <summary>
		/// Gets the current room.
		/// </summary>
		/// <value>The current room.</value>
		public MapRoom CurrentRoom {
			get {
				return currentRoom;
			}
		}

		/// <summary>
		/// Sent when player enters a room.
		/// </summary>
		public event System.EventHandler <RoomEventArgs> EnteredRoom;

		/// <summary>
		/// Sent when player left a room.
		/// </summary>
		public event System.EventHandler <RoomEventArgs> LeftRoom;

		/// <summary>
		/// Sent when player revealed a room.
		/// </summary>
		public event System.EventHandler <RoomEventArgs> RevealedRoom;

		/// <summary>
		/// Sent when a Point of Interest is added to a room.
		/// </summary>
		public event System.EventHandler <PointOfInterestEventArgs> RevealedPointOfInterest;

		/// <summary>
		/// Sent when a Point of Interest is changed.
		/// </summary>
		public event System.EventHandler <PointOfInterestEventArgs> PointOfInterestUpdated;

		/// <summary>
		/// Sent when a Point of Interest is removed from a room.
		/// </summary>
		public event System.EventHandler <PointOfInterestEventArgs> PointOfInterestRemoved;

		/// <summary>
		/// Raises the entered room event.
		/// </summary>
		/// <param name="room">Room.</param>
		virtual protected void OnEnteredRoom(MapRoom room) {
			MapRoomData data = MapRoom.GetData (room);
			if (EnteredRoom != null) EnteredRoom (this, new RoomEventArgs (data));
		}

		/// <summary>
		/// Raises the left room event.
		/// </summary>
		/// <param name="room">Room.</param>
		virtual protected void OnLeftRoom(MapRoom room) {
			MapRoomData data = MapRoom.GetData (room);
			if (LeftRoom != null) LeftRoom (this, new RoomEventArgs (data));
		}

		/// <summary>
		/// Raises the revealed room event.
		/// </summary>
		/// <param name="room">Room.</param>
		virtual protected void OnRevealedRoom(MapRoom room) {
			MapRoomData data = MapRoom.GetData (room);
			if (RevealedRoom != null) RevealedRoom (this, new RoomEventArgs (data));
		}

		/// <summary>
		/// Raises the revealed room event.
		/// </summary>
		/// <param name="room">Room.</param>
		virtual protected void OnRevealedRoom(MapRoomData data) {
			if (RevealedRoom != null) RevealedRoom (this, new RoomEventArgs (data));
		}
		
		/// <summary>
		/// Raises the poi added event.
		/// </summary>
		/// <param name="poi">Point of interest.</param>
		/// <param name="room">Room.</param>
		virtual protected void OnRevealedPointOfInterest(PointOfInterestData poi) {
			if (RevealedPointOfInterest != null) RevealedPointOfInterest (this, new PointOfInterestEventArgs (poi));
		}


		/// <summary>
		/// Raises the poi changed event.
		/// </summary>
		/// <param name="poi">Point of interest.</param>
		/// <param name="room">Room.</param>
		virtual protected void OnPointOfInterestUpdated(PointOfInterestData poi) {
			if (PointOfInterestUpdated != null) PointOfInterestUpdated (this, new PointOfInterestEventArgs (poi));
		}

		/// <summary>
		/// Raises the poi removed event.
		/// </summary>
		/// <param name="poi">Point of interest.</param>
		/// <param name="room">Room.</param>
		virtual protected void OnPointOfInterestRemoved(PointOfInterestData poi) {
			if (PointOfInterestRemoved != null) PointOfInterestRemoved (this, new PointOfInterestEventArgs (poi));
		}

		/// <summary>
		/// Unity Awake hook.
		/// </summary>
		void Awake() {
			Instance = this;
			Load ();
			#if PLATFORMER_PRO
			DoPoiCreateItems();
			DoPoiCreateDoors();
			DoPoiCreateRespawnPoints();
			#endif
		}

		void Start() {
			StartCoroutine(LoadAndScanExistingData ());
		}

		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update() {
			if (player == null && findPlayerByTag != null && findPlayerByTag != "") {
				player = GameObject.FindGameObjectWithTag (findPlayerByTag);
			}
		}

		/// <summary>
		/// Unity Late Update hook.
		/// </summary>
		void LateUpdate() {
			if (autoSave) {
				if (needsToSave == 0) Save ();
				if (needsToSave > 0) needsToSave--;
			}
		}

		/// <summary>
		/// Loads the and scan existing data. Done in a coroutine to give one frame for rooms to be initialised.
		/// </summary>
		/// <returns>The and scan existing data.</returns>
		virtual protected IEnumerator LoadAndScanExistingData() {
			yield return true;
			if (showUnrevealedRooms)
			{
				foreach (MapRoomData room in MapRoom.AllMapRoomData)
				{
					ShowRoom (room);
				}
			}
			LoadExistingRooms ();
		}

		/// <summary>
		/// Automaticaly creates POI from Items.
		/// </summary>
		virtual protected void DoPoiCreateItems()  {
			#if PLATFORMER_PRO
			if (itemsShownOnMap == null || itemsShownOnMap.Count == 0) return;
			Item[] items = FindObjectsOfType<Item> ();
			for (int i = 0; i < items.Length; i++) {	
				if (items[i].enabled && items[i].poi == null) {
					ItemToPoiMapping mapping = itemsShownOnMap.Where(m => m.itemType == items[i].ItemId).FirstOrDefault();
					if (mapping != null) {
						CreatePoiForItem(mapping, items[i]);
					}
				}
			}
			#endif
		}

		#if PLATFORMER_PRO
		/// <summary>
		/// Automaticaly creates POI from Items.
		/// </summary>
		virtual protected void DoPoiCreateDoors()  {
			if (!doorPoiData.showObjects) return;
			Door[] doors = FindObjectsOfType<Door> ();
			for (int i = 0; i < doors.Length; i++) {	
				if (doors[i].enabled && doors[i].poi == null) {
					string name = string.Format("DOOR_POI_{0:F1}_{1:F2}", doors[i].transform.position.x, doors[i].transform.position.y);
					GameObject poiGo = new GameObject (name);
					poiGo.transform.parent = doors[i].gameObject.transform;
					poiGo.transform.localPosition = Vector3.zero;
					PointOfInterest poi = poiGo.AddComponent<PointOfInterest> ();
					poi.category = doorPoiData.category;
					// TODO Better way to get a name
					poi.label = doors[i].name;
					// We will consider a door as 'unlocked' if it is OPEN or it has no key
					if (doors[i].state == DoorState.OPEN || doors[i].keyType == null || doors[i].keyType == "")
					{
						poi.icon = doorPoiData.icon;
						poi.color = doorPoiData.color;
						poi.revealed = doorPoiData.revealByDefault;
					}
					else
					{
						poi.icon = doorPoiData.alternateIcon;
						poi.color = doorPoiData.alternateColor;
						poi.revealed = doorPoiData.revealByDefault;
					}
					// Link them up so they know about each other
					doors[i].poi = poi;
					poi.Door = doors[i];
				}
			}
		}

		virtual protected void DoPoiCreateRespawnPoints()  {
			if (!respawnPoiData.showObjects) return;
			RespawnPoint[] respawns = FindObjectsOfType<RespawnPoint> ();
			for (int i = 0; i < respawns.Length; i++) {	
				if (respawns[i].enabled && respawns[i].poi == null && respawns[i].GetComponentInParent<Door>() == null) {
					string name = string.Format("RESPAWN_POI_{0:F1}_{1:F2}", respawns[i].transform.position.x, respawns[i].transform.position.y);
					GameObject poiGo = new GameObject (name);
					poiGo.transform.parent = respawns[i].gameObject.transform;
					poiGo.transform.localPosition = Vector3.zero;
					PointOfInterest poi = poiGo.AddComponent<PointOfInterest> ();
					poi.category = respawnPoiData.category;
					// TODO Better way to get a name
					poi.label = respawns[i].identifier;
					// Show active or inactive
					if (LevelManager.Instance.EnabledRespawnPoints.Contains(respawns[i].identifier))
					{
						poi.icon = respawnPoiData.icon;
						poi.color = respawnPoiData.color;
						poi.revealed = respawnPoiData.revealByDefault;
					}
					else
					{
						poi.icon = respawnPoiData.alternateIcon;
						poi.color = respawnPoiData.alternateColor;
						poi.revealed = respawnPoiData.revealByDefault;
					}
					// Link them up so they know about each other
					respawns[i].poi = poi;
					poi.RespawnPoint = respawns[i];
				}
			}
		}

		/// <summary>
		/// Creates a POI representing the given item.
		/// </summary>
		/// <param name="mapping">Mapping.</param>
		/// <param name="item">Item.</param>
		virtual protected void CreatePoiForItem(ItemToPoiMapping mapping, Item item) {
			string name = string.Format("POI_{0}_{1:F1}_{2:F2}", mapping.itemType, item.transform.position.x, item.transform.position.y);
			GameObject poiGo = new GameObject (name);
			poiGo.transform.parent = item.gameObject.transform;
			poiGo.transform.localPosition = Vector3.zero;
			PointOfInterest poi = poiGo.AddComponent<PointOfInterest> ();
			poi.category = mapping.category;
			poi.label = ItemTypeManager.Instance.GetTypeData (mapping.itemType).humanReadableName;
			poi.icon = mapping.icon;
			poi.color = mapping.color;
			poi.revealed = mapping.revealed;
			poi.hidden = mapping.hidden;
			// Link them up so they know about each other
			item.poi = poi;
			poi.Item = item;
		}

		#endif

		/// <summary>
		/// Loads the existing rooms from the static list in MapRoom.
		/// </summary>
		virtual protected void LoadExistingRooms () {
			foreach (MapRoomData m in MapRoom.AllMapRoomData) {
				if (m.revealed) RevealRoom (m);
			}
		}

		/// <summary>
		/// Enter the given room.
		/// </summary>
		/// <param name="room">Room.</param>
		virtual public void EnterRoom(MapRoom room) {
			currentRoom = room;
			OnEnteredRoom (room);
		}

		/// <summary>
		/// Leave the given room.
		/// </summary>
		/// <param name="room">Room.</param>
		virtual public void LeaveRoom(MapRoom room) {
			if (currentRoom == room) currentRoom = null;
			OnLeftRoom (room);
		}

		/// <summary>
		/// Reveal the given room and any auto reveal rooms.
		/// </summary>
		/// <param name="room">Room.</param>
		virtual public void RevealRoom(MapRoom room) {
			RevealRoom (room, true);
		}

		/// <summary>
		/// Reveal the given room.
		/// </summary>
		/// <param name="room">Room.</param>
		/// <param name="autoShowRooms">Should we show autoshow rooms.</param>
		virtual public void RevealRoom(MapRoom room, bool autoShowRooms) {
			if (autoShowRooms) {
				foreach (MapRoom asr in room.AutoShowRooms) {
					RevealRoom (asr, false);
				}
			}
			RevealRoom (MapRoom.GetData (room));
		}

		/// <summary>
		/// Reveal the given room.
		/// </summary>
		/// <param name="data">Room data.</param>
		virtual public void RevealRoom(MapRoomData data) {
			data.revealed = true;
			foreach (PointOfInterestData poi in PointOfInterest.AllPointsOfInterestData) {
				if (!poi.hidden && 
					data.MapRoom != null &&
					poi.PointOfInterest != null && 
					data.MapRoom.CheckObjectInRoom(poi.PointOfInterest.gameObject)) {
					poi.revealed = true;
					RevealPointOfInterest (poi);
				}
			}
			if (autoSave) needsToSave = 3;
			OnRevealedRoom (data);
		}

		/// <summary>
		/// Gets the room for a given POI. Returns null if no matchin room found.
		/// </summary>
		/// <returns>The room for poi.</returns>
		/// <param name="poi">POI to check.</param>
		virtual public MapRoomData GetRoomForPoi(PointOfInterestData poi) {
			foreach (MapRoomData room in MapRoom.AllMapRoomData) {
				if (room.MapRoom.CheckObjectInRoom (poi.PointOfInterest.gameObject))
				{
					return room;
				}
			}
			return null;
		}

		/// <summary>
		/// Showa  room without revealint it.
		/// </summary>
		/// <param name="data">Room data.</param>
		virtual public void ShowRoom(MapRoomData data) {
			if (autoSave) needsToSave = 3;
			OnRevealedRoom (data);
		}

		/// <summary>
		/// Add the given point of interest to the given room.
		/// </summary>
		/// <param name="poi">Point of Interest.</param>
		/// <param name="room">Room.</param>
		virtual public void RevealPointOfInterest(PointOfInterestData poi) {
			if (!poi.hidden) poi.revealed = true;
			if (autoSave) needsToSave = 3;
			OnRevealedPointOfInterest (poi);
		}

		/// <summary>
		/// Reveals the given point of interest.
		/// </summary>
		/// <param name="poi">Point of Interest.</param>
		virtual public void RevealPointOfInterest(PointOfInterest poi) {
			RevealPointOfInterest (PointOfInterest.GetData (poi));
		}

		/// <summary>
		/// Update the given point of interest.
		/// </summary>
		/// <param name="poi">Point of Interest.</param>
		virtual public void UpdatePointOfInterest(PointOfInterestData poi) {
			if (autoSave) needsToSave = 3;
			OnPointOfInterestUpdated (poi);
		}

		/// <summary>
		/// Update the given point of interest.
		/// </summary>
		/// <param name="poi">Point of Interest.</param>
		/// <param name="room">Room.</param>
		virtual public void UpdatePointOfInterest(PointOfInterest poi) {
			UpdatePointOfInterest (PointOfInterest.GetData (poi));
		}
			
		/// <summary>
		/// Removed the given point of interest from the given room.
		/// </summary>
		/// <param name="poi">Point of Interest.</param>
		/// <param name="room">Room.</param>
		virtual public void RemovePointOfInterest(PointOfInterestData poi) {
			if (autoSave) needsToSave = 3;
			OnPointOfInterestRemoved (poi);
		}

		/// <summary>
		/// Removes the given point of interest.
		/// </summary>
		/// <param name="poi">Point of Interest.</param>
		virtual public void RemovePointOfInterest(PointOfInterest poi) {
			RemovePointOfInterest (PointOfInterest.GetData (poi));
		}

		/// <summary>
		/// Log a message if showLogMessages = true.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="sender">Sender.</param>
		virtual public void Log(string message, Object sender) {
			if (showLogMessages) {
				Debug.Log ("MMM: " + message + " (" + (sender != null ? sender.name : "Unknown") + ")");
			}
		}

		/// <summary>
		/// Save data to player prefs. Override if you want to save data another way.
		/// </summary>
		virtual public void Save() {
			SaveData data = new SaveData();
			data.rooms = MapRoom.AllMapRoomData;
			data.pois = PointOfInterest.AllPointsOfInterestData;
			// Save to player prefs
			try {
				XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
				using (StringWriter writer = new StringWriter()) {
					serializer.Serialize(writer, data);
					PlayerPrefs.SetString(GetSaveKey(), writer.ToString());
				}
			} catch (System.Exception ex) {
				Debug.LogError ("Failed to save map data " + ex.Message);
			}
			needsToSave = -1;
		}

		/// <summary>
		/// Load data from player prefs. Override if you want to get data another way.
		/// </summary>
		virtual public void Load() {
			XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
			string rawData = PlayerPrefs.GetString(GetSaveKey(), "");
			if (rawData != null && rawData.Length > 0) {
				using (StringReader reader = new StringReader(rawData)) {
					SaveData data = (SaveData) serializer.Deserialize(reader);
					// TODO Consolidate points of interest
					MapRoom.ResetAllMapRooms(data.rooms);
					PointOfInterest.ResetAllPointsOfInterest(data.pois);
				}
			} else {
				// No save game found but thats okay
			}
		}

		/// <summary>
		/// Clear all map data.
		/// </summary>
		virtual public void ResetSaveData() {
			PlayerPrefs.DeleteKey (GetSaveKey());
		}

		/// <summary>
		/// Gets the key used to store daya in prefs.
		/// </summary>
		public string GetSaveKey() {
			return "MAPDATA_" + SaveGameId + "_" + mapName;
		}

		/// <summary>
		/// Setting this allows you to save multiple games in different 'slots'.
		/// </summary>
		/// <value>The save game identifier.</value>
		public static int SaveGameId {
			get; set;
		}

		/// <summary>
		/// Stores reference to the MapManager.
		/// </summary>
		static protected MapManager instance;

		/// <summary>
		/// Gets the singleton MapManager.
		/// </summary>
		/// <value>The MapManager.</value>
		static public MapManager Instance {
			get {
				if (instance == null || instance.gameObject == null) {
					instance = FindObjectOfType<MapManager>();
				}
				return instance;
			}
			protected set {
				if (instance != null && instance != value) Debug.LogWarning("Assigning a MapManager when one already existed");
				instance = value;
			}
		}
	}

	/// <summary>
	/// World orientation, tpyically X_Y for 2D and X_Z for 3D.
	/// </summary>
	public enum WorldOrientation {
		
		X_Y
		#if !PLATFORMER_PRO
 		,
		X_Z
		#endif
	}

	#if PLATFORMER_PRO
	[System.Serializable]
	public class ItemToPoiMapping 
	{
		/// <summary>
		/// The item type that will be converted in to a POI.
		/// </summary>
		[Tooltip ("The item type that will be converted in to a POI.")]
		public string itemType;

		/// <summary>
		/// The category of this POI.
		/// </summary>
		[Tooltip ("The category of this Point of Interest. Used for filtering.")]
		public string category = "DEFAULT";

		/// <summary>
		/// Sprite to use when drawing this item.
		/// </summary>
		[SerializeField]
		[Tooltip ("Icon to use for this Point of Interest. If null default will be used.")]
		public Sprite icon;

		/// <summary>
		/// Color to use for this POI.
		/// </summary>
		[Tooltip ("Color to use for this Point of Interest")]
		public Color color = Color.white;

		/// <summary>
		/// If true this PointOfInterest will be shown automatically.
		/// </summary>
		[Tooltip ("If true this PointOfInterest will be shown automatically.")]
		public bool revealed;

		/// <summary>
		/// If true this PointOfInterest will not be revealed on the map when player enters room. To show it you must set hidden to false and update the map content visibility.
		/// </summary>
		[Tooltip ("If true this PointOfInterest will not be revealed on the map when player enters room. To show it you must call Show().")]
		public bool hidden;

	}
	#endif

	#if PLATFORMER_PRO
	[System.Serializable]
	public class ExtraPoiData 
	{
		/// <summary>
		/// The item type that will be converted in to a POI.
		/// </summary>
		[Tooltip ("Should we show this object.")]
		public bool showObjects;

		/// <summary>
		/// The category of this POI.
		/// </summary>
		[Tooltip ("The category of this Point of Interest. Used for filtering.")]
		public string category = "CATEGORY";

		/// <summary>
		/// Sprite to use when drawing this item.
		/// </summary>
		[SerializeField]
		[Tooltip ("Icon to use for normal state.")]
		public Sprite icon;

		/// <summary>
		/// Sprite to use when drawing this item.
		/// </summary>
		[SerializeField]
		[Tooltip ("Icon to use for alternate state.")]
		public Sprite alternateIcon;

		/// <summary>
		/// Color to use for this POI.
		/// </summary>
		[Tooltip ("Color to use for doors")]
		public Color color = Color.white;

		/// <summary>
		/// Color to use for this POI.
		/// </summary>
		[Tooltip ("Color to use for alternate state.")]
		public Color alternateColor = Color.white;

		/// <summary>
		/// If true this PointOfInterest will be shown automatically.
		/// </summary>
		[Tooltip ("If true objects will be shown automatically, unless they have a POI which specifically overrides this.")]
		public bool revealByDefault = true;

	}
	#endif

}
