using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace jnamobile.mmm {

	/// <summary>
	/// Main User Interface class for a map. You need one per map UI (for example one for the mini-map and one for the main map).
	/// </summary>
	public class UIMapContent : MonoBehaviour {

		
		[Header ("General")]
		/// <summary>
		/// Ratio of room grid size to UI grid size.
		/// </summary>
		[Tooltip("Ratio of room grid size to UI grid size. You can stretch your map by using different values for x and y.")]
		public Vector2 mapScale = new Vector2(0.1f, 0.1f);

		/// <summary>
		/// Amount to offset the room in UI position. If this is zero then WorldPosition 0,0 is Map(UI) Position 0,0.
		/// </summary>
		[Tooltip ("Amount to offset the room in UI position. If this is zero then WorldPosition 0,0 is Map(UI) Position 0,0.")]
		public Vector3 roomOffset;

		/// <summary>
		/// All visible items will be created as children of this object . If null 
		/// visible items will be created as children of the main GameObject.
		/// </summary>
		[Tooltip ("All visible items will be created as children of this object . If null visible items will be created as children of the main GameObject.")]
		public GameObject visibleContent;

		[Header ("Behaviour")]

		/// <summary>
		/// Auto centre on current room?
		/// </summary>
		[Tooltip ("Should we auto-centre on current room? Only applies if this componet is a child (or descendant) of a ScrollRect")]
		public bool autoCentre = true;

		/// <summary>
		/// How fast we scroll to target scroll position.
		/// </summary>
		[Tooltip ("How fast we scroll to target scroll position. Only applies if this componet is a child (or descendant) of a ScrollRect")]
		public float scrollSpeed = 1.0f;

		/// <summary>
		/// Tracks if we want to snap to the first room we enter on scene load. Handy for multiscene maps which have multiple entry points. 
		/// </summary>
		[Tooltip ("Tracks if we want to snap to the first room we enter on scene load. Handy for multiscene maps which have multiple entry points.")]
		public bool snapToFirstRoom = true;

		[Header ("Rooms")]

		/// <summary>
		/// Prefab to use for generating new room UI's. If null a generic room will be generated.
		/// </summary>
		[Tooltip ("Prefab to use for generating new room UI's. If null a generic room will be generated.")] 
		public GameObject roomPrefab;
		
		/// <summary>
		/// <summary>
		/// Should we ignore color from room?
		/// </summary>
		[Tooltip ("Should we ignore color from room?")]
		public bool ignoreColor;

		/// <summary>
		/// Should we ignore sprite override from room?
		/// </summary>
		[Tooltip ("Should we ignore sprite override from room?")]
		public bool ignoreSpriteOverride;

		/// <summary>
		/// Color to ue for unrevealed rooms.
		/// </summary>
		[Tooltip ("Color to use for unrevealed rooms.")]
		public Color unrevealedRoomColor = new Color (1, 1, 1, 0.25f);

		/// <summary>
		/// ameObject used to show Current Room
		/// </summary>
		[Tooltip ("GameObject used to show Current Room. Activated and Translated when room changes. If null current room will not be shown.")] 
		public GameObject currentRoomIndicator;

		[Header ("Points of Interest")]

		/// <summary>
		/// How should we show points of interest?
		/// </summary>
		[Tooltip ("How should we show points of interest?")]
		public PointsOfInterestRenderingType pointRenderType;

		/// <summary>
		/// Prefab to use for generating new point of interest UI's. If null a generic room will be generated.
		/// </summary>
		[Tooltip ("Prefab to use for generating new point of interest UI's. If null a generic room will be generated.")] 
		public GameObject pointPrefab;

		/// <summary>
		/// The size of the point of interest GO on the map.
		/// </summary>
		[Tooltip ("The size of the point of interest GameObject on the map.")]
		public Vector2 pointSize;

		/// <summary>
		/// <summary>
		/// Should we ignore color from point of interest?
		/// </summary>
		[Tooltip ("Should we ignore color from Point of Interest?")]
		public bool ignorePointColor;
		
		/// <summary>
		/// Should we ignore sprite override from point of interest?
		/// </summary>
		[Tooltip ("Should we ignore sprite override from Point of Interest?")]
		public bool ignorePointSpriteOverride;

		/// <summary>
		/// ALl room objects get created under this.
		/// </summary>
		protected GameObject roomParent;

		/// <summary>
		/// /// All POI objects get created under this.
		/// </summary>
		protected GameObject poiParent;

		/// <summary>
		/// Maps from rooms to UI components.
		/// </summary>
		protected Dictionary <string, UIRoom> roomUiComponents;

		/// <summary>
		/// Is auto scroll currently enabled.
		/// </summary>
		protected bool isScrolling;


		/// <summary>
		/// Target position to scroll to.
		/// </summary>
		protected Vector2 targetScrollPosition;

		/// <summary>
		/// Cached ScrollRect ref.
		/// </summary>
		protected ScrollRect scroller;

		/// <summary>
		/// List of strings for points of interest that wont be shown on map.
		/// </summary>
		protected List<string> poiCategoryFilter;

		/// <summary>
		/// Map of points to their UI components.
		/// </summary>
		protected Dictionary<string, UIPointOfInterest> poiUiComponents;

		/// <summary>
		/// How much do we offset the current room indicator from the room position. Determined based on the delta of the
		/// Current Room Indicator in relation to its parent at Start.
		/// </summary>
		Vector3 currentRoomIndicatorPositionOffset;

		/// <summary>
		/// How much do we offset the current room indicator from the room position. Determined based on the delta of the
		/// Current Room Indicator in relation to its parent at Start.
		/// </summary>
		Vector2 currentRoomIndicatorSizeDelta;


		/// <summary>
		/// Gets the Transform to which visible content should be added.
		/// </summary>
		/// <value>The content of the visible.</value>
		public Transform UITopLevelContainer {
			get {
				if (visibleContent != null) return visibleContent.transform;
				return gameObject.transform;
			}
		}

		/// <summary>
		/// Gets the Transform to which visible room content should be added.
		/// </summary>
		/// <value>The content of the visible.</value>
		public Transform UIRoomContainer {
			get {
				if (roomParent != null) return roomParent.transform;
				if (visibleContent != null) return visibleContent.transform;
				return gameObject.transform;
			}
		}

		/// <summary>
		/// Gets the Transform to which visible poi content should be added.
		/// </summary>
		/// <value>The content of the visible.</value>
		public Transform UIPoiContainer {
			get {
				if (poiParent != null) return poiParent.transform;
				if (visibleContent != null) return visibleContent.transform;
				return gameObject.transform;
			}
		}

		/// <summary>
		/// Unity Awake hook.
		/// </summary>
		void Awake() {
			Init();
		}

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start() {
			RegisterListeners ();
			scroller = GetComponentInParent<ScrollRect> ();
			if (currentRoomIndicator != null) {
				currentRoomIndicatorPositionOffset = currentRoomIndicator.transform.localPosition;
				currentRoomIndicatorSizeDelta = currentRoomIndicator.GetComponent<RectTransform>().sizeDelta;
			}
		}

		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update() {
			DoScroll ();
		}

		/// <summary>
		/// Unity OnDestroy hook.
		/// </summary>
		void OnDestroy() {
			DeregisterListeners ();
		}

		/// <summary>
		/// Initialise this UI.
		/// </summary>
		virtual protected void Init() {
			roomUiComponents = new Dictionary<string, UIRoom> ();
			poiCategoryFilter = new List<string> ();
			poiUiComponents = new Dictionary<string, UIPointOfInterest> ();

			roomParent = new GameObject ("RoomContent");
			roomParent.transform.parent = UITopLevelContainer;
			RectTransform roomRectTransform = roomParent.AddComponent<RectTransform> ();
			roomRectTransform.anchorMin = Vector2.zero;
			roomRectTransform.anchorMax = Vector2.one;
			roomRectTransform.offsetMin = Vector2.zero;
			roomRectTransform.offsetMax = Vector2.zero;

			poiParent = new GameObject ("PoiContent");
			poiParent.transform.parent = UITopLevelContainer;
			poiParent.transform.SetAsLastSibling ();
			RectTransform poiRectTransform = poiParent.AddComponent<RectTransform> ();
			poiRectTransform.anchorMin = Vector2.zero;
			poiRectTransform.anchorMax = Vector2.one;
			poiRectTransform.offsetMin = Vector2.zero;
			poiRectTransform.offsetMax = Vector2.zero;
		}

		/// <summary>
		/// Registers the listeners.
		/// </summary>
		virtual protected void RegisterListeners() {
			MapManager.Instance.EnteredRoom += HandleEnteredRoom;
			MapManager.Instance.LeftRoom += HandleLeftRoom;
			MapManager.Instance.RevealedRoom += HandleRevealedRoom;
			MapManager.Instance.RevealedPointOfInterest += HandlePointOfInterestRevealed;
			MapManager.Instance.PointOfInterestUpdated += HandlePointOfInterestUpdated;
			MapManager.Instance.PointOfInterestRemoved += HandlePointOfInterestRemoved;
		}


		/// <summary>
		/// Deregisters the listeners.
		/// </summary>
		virtual protected void DeregisterListeners() {
			if (MapManager.Instance != null) {
				MapManager.Instance.EnteredRoom -= HandleEnteredRoom;
				MapManager.Instance.LeftRoom -= HandleLeftRoom;
				MapManager.Instance.RevealedRoom -= HandleRevealedRoom;
				MapManager.Instance.RevealedPointOfInterest -= HandlePointOfInterestRevealed;
				MapManager.Instance.PointOfInterestUpdated -= HandlePointOfInterestUpdated;
				MapManager.Instance.PointOfInterestRemoved -= HandlePointOfInterestRemoved;

			}
		}

		/// <summary>
		/// Creates a UI component for the given room.
		/// </summary>
		/// <returns>The created room UI, or the existing one if already created.</returns>
		/// <param name="room">Room.</param>
		virtual protected UIRoom CreateRoomUI(MapRoomData room) {
			UIRoom ui = null;

			// Don't create a UI for a room that already has one.
			if (roomUiComponents.ContainsKey(room.fqn)) {
				if (MapManager.Instance.showLogMessages) Debug.Log ("MMM: Tried to create a UI for a room that already had a UI.");
				return roomUiComponents[room.fqn];
			}

			if (MapManager.Instance.showLogMessages) Debug.Log ("MMM: Creating UI for room: " + room.fqn);

			if (roomPrefab != null) {
				GameObject uiGo = (GameObject)GameObject.Instantiate(roomPrefab);
				ui = uiGo.GetComponent<UIRoom>();
				if (ui == null) {
					Debug.LogError("MMM: UI Room prefab does not contain a UIRoom component.");
				}
			} else {
				// No prefab found create a generic room
				GameObject uiGo = new GameObject();
				ui = uiGo.AddComponent<UIRoom>();
				Image image = uiGo.AddComponent<Image>();
				image.color = Color.white;
				image.enabled = false;
			}

			if (ui != null) {
				ui.gameObject.name = "UIRoom_" + room.shortName;
				ui.Init (this, room);
				roomUiComponents.Add(room.fqn, ui);
			} else {
				Debug.LogError("MMM: Failed to generate UI for Room");
			}
			return ui;
		}

		/// <summary>
		/// Creates the poi user interface.
		/// </summary>
		/// <returns>The poi user interface.</returns>
		/// <param name="room">Room.</param>
		/// <param name="poi">Poi.</param>
		virtual public UIPointOfInterest CreatePoiUi(PointOfInterestData poi) {
			UIPointOfInterest ui = null;

			if (MapManager.Instance.showLogMessages) Debug.Log ("MMM: Creating UI for point of interest: " + poi.fqn);

			if (pointPrefab != null) {
				GameObject uiGo = (GameObject)GameObject.Instantiate(pointPrefab);
				ui = uiGo.GetComponent<UIPointOfInterest>();
				if (ui == null) {
					Debug.LogError("MMM: UI Point of Interest prefab does not contain a UIPointOfInterest component.");
				}
			} else {
				// No prefab found create a generic room
				GameObject uiGo = new GameObject();
				ui = uiGo.AddComponent<UIPointOfInterest>();
				Image image = uiGo.AddComponent<Image>();
				image.color = Color.white;
				image.enabled = false;
			}

			if (ui != null) {
				ui.gameObject.name = "UIPoint_" + poi.shortName;
				ui.Init (this, poi);
			} else {
				Debug.LogError("MMM: Failed to generate UI for Room");
			}
			return ui;
		}

		/// <summary>
		/// Shows UI for a room that is currently hidden, or creates UI if if does not exist.
		/// </summary>
		/// <param name="room">Room.</param>
		public void RevealRoom(MapRoomData room) {
			if (roomUiComponents.ContainsKey (room.fqn)) {
				roomUiComponents [room.fqn].Show ();
			} else {
				UIRoom roomUi = CreateRoomUI (room);
				roomUi.Show();
			}
			if (currentRoomIndicator.gameObject != null) {
				currentRoomIndicator.transform.SetAsLastSibling ();
			}
		}

		/// <summary>
		/// Centres the on the current room.
		/// </summary>
		public void CentreOnCurrentRoom() {
			CentreOnRoom (MapManager.Instance.CurrentRoom);
		}

		/// <summary>
		/// Centres the on the given room. If the room does not exist it will be created and then hidden.
		/// </summary>
		/// <param name="room">Room.</param>
		public void CentreOnRoom(MapRoom room) {
			RectTransform target = null;
			if (roomUiComponents.ContainsKey (room.FullyQualifiedRoomName)) {
				target = roomUiComponents [room.FullyQualifiedRoomName].GetComponent<RectTransform> ();
			} else {
				CreateRoomUI(MapRoom.GetData(room));
				target = roomUiComponents [room.FullyQualifiedRoomName].GetComponent<RectTransform> ();
				roomUiComponents[room.FullyQualifiedRoomName].Hide();
			}
			if (target != null) CentreOnPosition (target, false);
		}

		/// <summary>
		/// Toggles a point of interest category between shown and hidden.
		/// </summary>
		/// <param name="category">Category to show/hide.</param>
		virtual public void ToggleCategory(string category) {
			if (poiCategoryFilter.Contains (category)) poiCategoryFilter.Remove (category);
			else poiCategoryFilter.Add (category);
			UpdateVisibility();
		}

		/// <summary>
		/// Sets a poiint of interest category to be shown.
		/// </summary>
		/// <param name="category">Category to show.</param>
		virtual public void ShowCategory(string category) {
			if (poiCategoryFilter.Contains (category)) poiCategoryFilter.Remove (category);
			UpdateVisibility();
		}

		/// <summary>
		/// Sets a poiint of interest category to be hidden.
		/// </summary>
		/// <param name="category">Category to hide.</param>
		virtual public void HideCategory(string category) {
			if (!poiCategoryFilter.Contains (category)) poiCategoryFilter.Add (category);
			UpdateVisibility();
		}

		/// <summary>
		/// Refresh the visibility for rooms and points of interest.
		/// </summary>
		virtual public void UpdateVisibility(MapRoomData room) {
			if (roomUiComponents.ContainsKey(room.fqn)) roomUiComponents[room.fqn].UpdateVisibility ();
		}

		/// <summary>
		/// Refresh the visibility for rooms and points of interest.
		/// </summary>
		virtual public void UpdateVisibility() {
			foreach (UIRoom ui in roomUiComponents.Values) ui.UpdateVisibility ();
			foreach (UIPointOfInterest poi in poiUiComponents.Values) poi.UpdateVisibilityAndPosition ();
		}

		/// <summary>
		/// Should we show the given point of interest?
		/// </summary>
		/// <returns><c>true</c>, if point of interesest should be shown, <c>false</c> otherwise.</returns>
		/// <param name="poi">Point of interest.</param>
		virtual public bool ShouldShowPointOfInteresest(PointOfInterestData poi) {
			if (poi.PointOfInterest != null && !poi.PointOfInterest.gameObject.activeInHierarchy) return false;
			if (poi.hidden) return false;
			if (pointRenderType == PointsOfInterestRenderingType.DONT_SHOW) return false;
			if (poiCategoryFilter.Contains (poi.category)) return false;
			return poi.revealed;
		}

		/// <summary>
		/// Centres the on given rect position.
		/// </summary>
		/// <param name="target">Target.</param>
		virtual public void CentreOnPosition(RectTransform target, bool instant) {
			ScrollRect scroller = GetComponentInParent<ScrollRect> ();
			if (scroller != null) {
				Rect rect = GetComponent<RectTransform>().rect;
				float x = (target.localPosition.x + (rect.width / 2.0f)) / rect.width;
				float y =  (target.localPosition.y + (rect.height / 2.0f)) / rect.height;
				if (instant) {
					scroller.normalizedPosition = new Vector2 (x, y);
					isScrolling = false;
				} else {
					targetScrollPosition = new Vector2 (x, y);
					isScrolling = true;
				}
			}
		}

		/// <summary>
		/// Scroll to target position.
		/// </summary>
		virtual protected void DoScroll() {
			if (scroller == null || !isScrolling) return;
			if (scrollSpeed <= 0) {
				scroller.normalizedPosition = targetScrollPosition;
				isScrolling = false;
				return;
			}
			Vector2 diff = new Vector2(targetScrollPosition.x - scroller.normalizedPosition.x, targetScrollPosition.y - scroller.normalizedPosition.y);
			// Snap when we get close
			if (diff.magnitude < 0.001f) {
				scroller.normalizedPosition = targetScrollPosition;
				isScrolling = false;
			} else {
				if ((diff.normalized * scrollSpeed * Time.deltaTime).sqrMagnitude > diff.sqrMagnitude) {
					scroller.normalizedPosition = targetScrollPosition;
					isScrolling = false;
				} else {
					scroller.normalizedPosition += diff.normalized * scrollSpeed * Time.deltaTime;
				}
			}
		}

		/// <summary>
		/// Handles the revealed room event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event args.</param>
		void HandleRevealedRoom (object sender, RoomEventArgs e) {
			if (MapManager.Instance.showLogMessages) Debug.Log ("MMM: Reveal room: " + e.Room.fqn);
			RevealRoom (e.Room);
			UpdateVisibility (e.Room);
		}

		/// <summary>
		/// Handles the left room event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event args.</param>
		void HandleLeftRoom (object sender, RoomEventArgs e) {
			if (MapManager.Instance.showLogMessages) Debug.Log ("MMM: Left room: " + e.Room.fqn);
			if (roomUiComponents.ContainsKey(e.Room.fqn)) roomUiComponents [e.Room.fqn].Left ();
		}

		/// <summary>
		/// Handles the entered room event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event args.</param>
		void HandleEnteredRoom (object sender, RoomEventArgs e) {
			if (MapManager.Instance.showLogMessages) Debug.Log ("MMM: Entered room: " + e.Room.fqn);
			if (!roomUiComponents.ContainsKey(e.Room.fqn)) CreateRoomUI(e.Room);
			roomUiComponents [e.Room.fqn].Entered ();
			RectTransform roomRect = roomUiComponents [e.Room.fqn].GetComponent<RectTransform> ();
			if (autoCentre) CentreOnPosition (roomRect, snapToFirstRoom);
			if (currentRoomIndicator.gameObject != null) {
				RectTransform currentRoomRect = currentRoomIndicator.GetComponent<RectTransform>();
				currentRoomIndicator.SetActive(true);
				currentRoomIndicator.transform.SetAsLastSibling();
				currentRoomRect.position = roomRect.position + currentRoomIndicatorPositionOffset;
				currentRoomRect.sizeDelta = roomRect.sizeDelta + currentRoomIndicatorSizeDelta;
			}
			snapToFirstRoom = false;
		}

		/// <summary>
		/// Handles the point of interest added event by generating matching UI component.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void HandlePointOfInterestRevealed(object sender, PointOfInterestEventArgs e) {
			if (!poiUiComponents.ContainsKey (e.PointOfInterest.fqn))
			{
				UIPointOfInterest poiUi = CreatePoiUi (e.PointOfInterest);
				poiUiComponents.Add (e.PointOfInterest.fqn, poiUi);
			}
			poiUiComponents [e.PointOfInterest.fqn].UpdateVisibilityAndPosition ();
		}

		/// <summary>
		/// Handles the point of interest updated event by updating matching UI component.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void HandlePointOfInterestUpdated(object sender, PointOfInterestEventArgs e) {
			if (!poiUiComponents.ContainsKey (e.PointOfInterest.fqn))
			{
				UIPointOfInterest poiUi = CreatePoiUi (e.PointOfInterest);
				poiUiComponents.Add (e.PointOfInterest.fqn, poiUi);
			}
			poiUiComponents [e.PointOfInterest.fqn].UpdateVisibilityAndPosition ();
		}

		/// <summary>
		/// Handles the point of interest removed even by removing the mathcing UI component.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void HandlePointOfInterestRemoved (object sender, PointOfInterestEventArgs e) {
			if (!poiUiComponents.ContainsKey(e.PointOfInterest.fqn)) return;
			Destroy (poiUiComponents [e.PointOfInterest.fqn].gameObject);
			poiUiComponents.Remove (e.PointOfInterest.fqn);
		}

		/// <summary>
		/// Applies a point of interest filter hididng anything that has a category not in the list.
		/// </summary>
		/// <param name="categories">Categories to show.</param>
		virtual public void ApplyPointOfInterestFilter(List<string> categories) {
			foreach (string fqn in poiUiComponents.Keys) {
				if (categories.Contains(PointOfInterest.GetData(fqn).category)) {
					poiUiComponents[fqn].Show ();
				} else {
					poiUiComponents[fqn].Hide ();
				}
			}
		}

	}
}
