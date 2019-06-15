#define PLATFORMER_PRO
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace jnamobile.mmm {

	public class PointOfInterest : MonoBehaviour {

		/// <summary>
		/// Human readable name for the POI (optional).
		/// </summary>
		[Tooltip ("Human readable name for the POI (optional).")]
		[SerializeField] 
		protected string poiName;

		/// <summary>
		/// The category of this POI.
		/// </summary>
		[Tooltip ("The category of this Point of Interest. Used for filtering.")]
		public string category = "DEFAULT";

		/// <summary>
		/// The label or name of this POI.
		/// </summary>
		[Tooltip ("The label or name of this Point of Interest.")]
		public string label;

		/// <summary>
		/// If non null, the map sprite will be replaced with the given sprite.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		public string iconName;

		/// <summary>
		/// Sprite to use when drawing this item.
		/// </summary>
		[SerializeField]
		[SpriteAssignerAttribute ("iconName")]
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

		#if PLATFORMER_PRO
		/// <summary>
		/// Reference to assocaited Item component.
		/// </summary>
		/// <value>The item.</value>
		public PlatformerPro.Item Item 
		{
			get; set;
		}

		/// <summary>
		/// Reference to assocaited Door component.
		/// </summary>
		/// <value>The door.</value>
		public PlatformerPro.Door Door 
		{
			get; set;
		}

		/// <summary>
		/// Reference to assocaited RespawnPoint component.
		/// </summary>
		/// <value>The door.</value>
		public PlatformerPro.RespawnPoint RespawnPoint 
		{
			get; set;
		}


		#endif
		/// <summary>
		/// Fully qualified name, scene name plus poi name.
		/// </summary>
		protected string fqn;

		/// <summary>
		/// Gets the name of the room.
		/// </summary>
		public string FullyQualifiedPoiName {
			get {
				return fqn;
			}
		}

		/// <summary>
		/// Gets the name of the room.
		/// </summary>
		public string PoiName {
			get {
				if (poiName != null && poiName != "") return poiName;
				return gameObject.name;
			}
			set {
				poiName = value;
			}
		}

		/// <summary>
		/// Unity awake hook.
		/// </summary>
		void Awake() {
			fqn = SceneManager.GetActiveScene ().name + "_" + PoiName;
			AddPointOfInterest (this);
		}


		/// <summary>
		/// Gets the POI position taking in to account the MapManagers orientation setting.
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
			
		/// <summary>
		/// Static list of all map rooms.
		/// </summary>
		static Dictionary<string, PointOfInterestData> allPointsOfInterests;

		public static PointOfInterestData GetData(PointOfInterest room) {
			if (allPointsOfInterests.ContainsKey (room.FullyQualifiedPoiName)) return allPointsOfInterests [room.FullyQualifiedPoiName];
			return null;
		}

		public static PointOfInterestData GetData(string fqn) {
			if (allPointsOfInterests.ContainsKey (fqn)) return allPointsOfInterests [fqn];
			return null;
		}

		/// <summary>
		/// Gets list of all map rooms.
		/// </summary>
		public static List<PointOfInterestData> AllPointsOfInterestData {
			get {
				if (allPointsOfInterests == null) allPointsOfInterests = new Dictionary<string, PointOfInterestData> ();
				return allPointsOfInterests.Values.ToList();
			}
		}

		/// <summary>
		/// Reset list of all pois back to null.
		/// </summary>
		public static void ResetAllPointsOfInterest() {
			allPointsOfInterests = null;
		}


		/// <summary>
		/// Reset list of all pois back to specified list.
		/// </summary>
		public static void ResetAllPointsOfInterest(List<PointOfInterestData> data) {
			if (allPointsOfInterests != null) {
				allPointsOfInterests.Clear ();
			} else {
				allPointsOfInterests = new Dictionary<string, PointOfInterestData> ();
			}
			if (data == null) return;
			foreach (PointOfInterestData p in data) {
				allPointsOfInterests.Add (p.fqn, p);
			}
		}


		public static void AddPointOfInterest(PointOfInterest poi) {
			if (allPointsOfInterests == null) allPointsOfInterests = new Dictionary<string, PointOfInterestData> ();
			// Maintain a static list of rooms.
			if (!allPointsOfInterests.ContainsKey (poi.FullyQualifiedPoiName)) {
				allPointsOfInterests.Add (poi.FullyQualifiedPoiName, new PointOfInterestData(poi));
			} else {
				PointOfInterestData existingPoi = allPointsOfInterests [poi.FullyQualifiedPoiName];
				if (existingPoi == null)
				{
					allPointsOfInterests [poi.FullyQualifiedPoiName] = new PointOfInterestData (poi);
					return;
				}
				if (existingPoi.PointOfInterest == poi)
				{
					return;
				}
				if (existingPoi.PointOfInterest == null) {
					existingPoi.PointOfInterest = poi;
					return;
				}
				Debug.LogWarning ("PointODebug.Log (\"Poi match\");fInterest names are not globally unique. Multiscene/saving/loading may not work.");
			}
		}

	}
}