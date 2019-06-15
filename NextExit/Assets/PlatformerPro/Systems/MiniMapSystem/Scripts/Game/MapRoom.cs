#define PLATFORMER_PRO

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace jnamobile.mmm {

	/// <summary>
	/// Represents a room on the map. A room is revealed when you enter it.
	/// </summary>
	public class MapRoom : MonoBehaviour {

		[Header ("Settings")]

		/// <summary>
		/// Human readable name for the room (optional).
		/// </summary>
		[Tooltip ("Human readable name for the room (optional).")]
		[SerializeField] 
		protected string roomName;

		/// <summary>
		/// Width in Grid units. Grid unit size is assigned in the MapManager.
		/// </summary>
		[Tooltip ("Width in Grid units. Grid unit size is assigned in the MapManager.")]
		[Range(1,100)]
		[SerializeField]
		protected int width;

		/// <summary>
		/// Height in Grid units.
		/// </summary>
		[Tooltip ("Height in Grid units. Grid unit size is assigned in the MapManager.")]
		[Range(1,100)]
		[SerializeField]
		protected int height;


		[Header ("Visuals")]
		
		/// <summary>
		/// Hue to apply to the sprite when shown on the map.
		/// </summary>
		public Color colorOverride = Color.white;
		
		/// <summary>
		/// If non null, the map sprite will be replaced with the given sprite.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		public string overrideSpriteName;

		/// <summary>
		/// Sprite to use when drawing this item.
		/// </summary>
		[SerializeField]
		[SpriteAssignerAttribute ("overrideSpriteName")]
		public Sprite overrideSprite;

		[Header ("Relationships")]

		/// <summary>
		/// Reference to rooms which we should automatically show when this room is entered.
		/// </summary>
		[Tooltip ("Reference to rooms which we should automatically show when this room is entered.")]
		[SerializeField]
		protected List<MapRoom> autoShowRooms;

		#if PLATFORMER_PRO

		[Header ("Platformer Pro")]

		/// <summary>
		/// If true the room will also automatically create a camera zone with the same bounds as the room.
		/// </summary>
		[Tooltip ("If true the room will also automatically create a camera zone with the same bounds as the room.")]
		[SerializeField]

		protected bool roomIsCameraZone = true;
		#endif

		/// <summary>
		/// Fully qialifed name, scene name plus room name.
		/// </summary>
		protected string fqn;

		/// <summary>
		/// Gets the name of the room.
		/// </summary>
		public string RoomName {
			get {
				if (roomName != null && roomName != "") return roomName;
				return gameObject.name;
			}
		}

		/// <summary>
		/// Gets the name of the room.
		/// </summary>
		public string FullyQualifiedRoomName {
			get {
				return fqn;
			}
		}

		/// <summary>
		/// Gets the auto show rooms. Note this is not a copy!
		/// </summary>
		public List<MapRoom> AutoShowRooms {
			get {
				return autoShowRooms;
			}
		}

		/// <summary>
		/// Track is this piece has been revealed.
		/// </summary>
		protected bool isRevealed;

		/// <summary>
		/// Cached min bounds.
		/// </summary>
		protected Vector2 min;

		/// <summary>
		/// Cached max bounds.
		/// </summary>
		protected Vector2 max;

		/// <summary>
		/// Gets the width in grid units.
		/// </summary>
		/// <value>The width.</value>
		public int Width {
			get { return width; }
		}

		/// <summary>
		/// Gets the height in grid units.
		/// </summary>
		/// <value>The height.</value>
		public int Height {
			get { return height; }
		}

		/// <summary>
		/// Unity awake hook.
		/// </summary>
		void Awake() {
			fqn = SceneManager.GetActiveScene ().name + "_" + RoomName;
			#if PLATFORMER_PRO
			if (roomIsCameraZone) CreateCameraZone();
			#endif
		}

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start() {
			AddMapRoom (this);
			min = (Vector2)Position - new Vector2 (MapManager.Instance.gridSize * width / 2.0f, MapManager.Instance.gridSize * height / 2.0f);
			max = (Vector2)Position + new Vector2 (MapManager.Instance.gridSize * width / 2.0f, MapManager.Instance.gridSize * height / 2.0f);
		}

		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update()
		{
			if (!enabled) return;
			UpdateForPlayer (MapManager.Instance.player);
		}

		/// <summary>
		/// Gets the map position taking in to account the MapManagers orientation setting.
		/// </summary>
		/// <value>The position.</value>
		public Vector3 Position {
			get {
				if (MapManager.Instance.worldOrientation == WorldOrientation.X_Y) {
					return transform.position;
				} else {
					return new Vector3(transform.position.x, transform.position.z, transform.position.y);
				}
			}
		}

		#if PLATFORMER_PRO
		virtual protected void CreateCameraZone() {
			PlatformerPro.PlatformCamera camera = FindObjectOfType<PlatformerPro.PlatformCamera> ();
			if (camera == null) return;
			GameObject zoneGo = new GameObject ();
			zoneGo.transform.parent = transform;
			zoneGo.transform.localPosition = Vector3.zero;
			PlatformerPro.CameraZone zone = zoneGo.AddComponent<PlatformerPro.CameraZone> ();
			zone.width = MapManager.Instance.gridSize * width;
			zone.height = MapManager.Instance.gridSize * height;
			zone.cameraZOffset = camera.transform.position.z - zoneGo.transform.position.z ;
		}
		#endif

		/// <summary>
		/// Updates room state based on given player GameObject.
		/// </summary>
		/// <param name="player">Player.</param>
		protected void UpdateForPlayer(GameObject player) {
			if (player == null) return;
			if (Vector2.Distance (transform.position, player.transform.position) > MapManager.Instance.ClearlyOutDistance) {
				OutOfRange();
			}

			if (CheckObjectInRoom (player)) {
				if (!MapManager.Instance.CurrentRoom == this) MapManager.Instance.EnterRoom(this);
				if (!isRevealed) MapManager.Instance.RevealRoom(this);
				isRevealed = true;
			} else {
				if (MapManager.Instance.CurrentRoom == this)  MapManager.Instance.LeaveRoom(this);
			}

		}

		/// <summary>
		/// Checks if an object (typically player) is in room and returns true if it is. If you override this you 
		/// should not chaage any state from this method. Instead use its result to drive changes.
		/// </summary>
		/// <param name="obj">Object to check such as the player GO.</param>
		virtual public bool CheckObjectInRoom(GameObject obj) {
			if (obj.transform.position.x >= min.x && obj.transform.position.x <= max.x)
			{
				if (MapManager.Instance.worldOrientation == WorldOrientation.X_Y && obj.transform.position.y >= min.y && obj.transform.position.y <= max.y) return true;
				#if !PLATFORMER_PRO
				if (MapManager.Instance.worldOrientation == WorldOrientation.X_Z && obj.transform.position.z >= min.y && obj.transform.position.z <= max.y) return true;
				#endif
			}
			return false;
		}
			
		/// <summary>
		/// Called when player is out of range.
		/// </summary>
		protected void OutOfRange() {
			if (MapManager.Instance.CurrentRoom == this) MapManager.Instance.LeaveRoom (this);
		}

		/// <summary>
		/// Unity Gizmo hook.
		/// </summary>
		void OnDrawGizmos() {
			if (MapManager.Instance == null) {
				Debug.LogWarning("You must add a MapManager to your scene before you can use the MapRoom");
				return;
			}
			Gizmos.color = new Color (colorOverride.r, colorOverride.g, colorOverride.b, 0.25f);
			if (MapManager.Instance.worldOrientation == WorldOrientation.X_Y) {
				Gizmos.DrawCube (transform.position, new Vector3 (MapManager.Instance.gridSize * (float)width, MapManager.Instance.gridSize * (float)height, 0.1f));
			} else {
				Gizmos.DrawCube (transform.position, new Vector3 (MapManager.Instance.gridSize * (float)width, 0.1f, MapManager.Instance.gridSize * (float)height));
			}
		}

		/// <summary>
		/// Static list of all map rooms.
		/// </summary>
		static Dictionary<string, MapRoomData> allMapRooms;

		public static MapRoomData GetData(MapRoom room) {
			if (allMapRooms.ContainsKey (room.FullyQualifiedRoomName)) return allMapRooms [room.FullyQualifiedRoomName];
			return null;
		}
			
		/// <summary>
		/// Gets the room data for a given room with the matching fully qualified name..
		/// </summary>
		/// <returns>The room data or null if no match found..</returns>
		/// <param name="fqn">Fully qualified name of the room to find data for.</param>
		public static MapRoomData GetData(string fqn) {
			if (allMapRooms.ContainsKey (fqn)) return allMapRooms [fqn];
			return null;
		}

		/// <summary>
		/// Gets list of all map rooms.
		/// </summary>
		public static List<MapRoomData> AllMapRoomData {
			get {
				if (allMapRooms == null) allMapRooms = new Dictionary<string, MapRoomData> ();
				return allMapRooms.Values.ToList();
			}
		}

		/// <summary>
		/// Reset list of all rooms back to null.
		/// </summary>
		public static void ResetAllMapRooms() {
			allMapRooms = null;
		}

		/// <summary>
		/// Reset list of all rooms back to specified list.
		/// </summary>
		public static void ResetAllMapRooms(List<MapRoomData> data) {
			if (allMapRooms != null) {
				allMapRooms.Clear ();
			} else {
				allMapRooms = new Dictionary<string, MapRoomData> ();
			}
			if (data == null) return;
			foreach (MapRoomData m in data) {
				allMapRooms.Add (m.fqn, m);
			}
		}

		/// <summary>
		/// Add a map room to the list of all map rooms.
		/// </summary>
		/// <param name="room">MapRoom to add.</param>
		public static void AddMapRoom(MapRoom room) {
			if (allMapRooms == null) allMapRooms = new Dictionary<string, MapRoomData> ();
			// Maintain a static list of rooms.
			if (!allMapRooms.ContainsKey (room.FullyQualifiedRoomName)) {
				allMapRooms.Add (room.FullyQualifiedRoomName, new MapRoomData (room));
			} else {
				MapRoomData existingRoom = allMapRooms [room.FullyQualifiedRoomName];
				if (existingRoom.MapRoom == room) {
					return;
				}
				if (existingRoom == null) {
					allMapRooms[room.FullyQualifiedRoomName] = new MapRoomData(room);
					return;
				} 
				if (existingRoom.MapRoom == null) {
					existingRoom.MapRoom = room;
					return;
				}
				Debug.LogWarning ("MapRoom names are not globally unique. Multiscene/saving/loading may not work.");
			}
		}
	}

}
