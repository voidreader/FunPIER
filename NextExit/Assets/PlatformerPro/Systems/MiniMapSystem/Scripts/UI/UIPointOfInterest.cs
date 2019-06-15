#define PLATFORMER_PRO
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace jnamobile.mmm {

	/// <summary>
	/// Represents a point of interest in the UI.
	/// </summary>
	public class UIPointOfInterest : UIBaseMapComponent {
		
		/// <summary>
		/// The POI we represent.
		/// </summary>
		protected PointOfInterestData poi;
		
		/// <summary>
		/// Initialise POI with given room data. It is expected that the POI will be parented to the
		/// map content GameObject before this method is invoked.
		/// </summary>
		/// <param name="content">Parent map content object.</param>
		/// <param name="room">Room.</param>
		virtual public void Init(UIMapContent content, PointOfInterestData poi) {
			this.poi = poi;
			this.mapContent = content;

			if (baseImage == null) baseImage = GetComponentInChildren<Image>();

			RectTransform rt = gameObject.GetComponent<RectTransform> ();
			rt.SetParent(content.UIPoiContainer, false);
			if (!content.ignorePointColor) baseImage.color = poi.color;
			if (poi.icon != null && !content.ignorePointSpriteOverride) baseImage.sprite = poi.icon;

			rt.sizeDelta = mapContent.pointSize;

			UpdateVisibilityAndPosition ();
		}

		/// <summary>
		/// Unity Update Hook.
		/// </summary>
		void Update() {
			if (poi == null || !enabled) return;
			if (poi.isDynamic) {
				if (mapContent.ShouldShowPointOfInteresest (poi)) {
					UpdatePosition ();
				} else {
					Hide ();
				}
			}
		}

		/// <summary>
		/// Updates the visibility and position based on map content and point of interest settings.
		/// </summary>
		virtual public void UpdateVisibilityAndPosition() {
			if (!mapContent.ShouldShowPointOfInteresest (poi)) {
				Hide ();
				return;
			}
			UpdatePosition ();
			UpdateIcon();
			Show ();
		}

		/// <summary>
		/// Updates the position.
		/// </summary>
		virtual protected void UpdatePosition() {
		
			RectTransform rt = gameObject.GetComponent<RectTransform> ();
			if (mapContent.pointRenderType == PointsOfInterestRenderingType.SHOW_IN_CENTRE ) {
				MapRoomData room = MapManager.Instance.GetRoomForPoi (poi);
				if (room != null && room.MapRoom != null)
				{
					rt.localPosition = new Vector3 (room.MapRoom.transform.position.x * mapContent.mapScale.x, room.MapRoom.transform.position.y * mapContent.mapScale.y, rt.localPosition.z);
				}
			}
			if (mapContent.pointRenderType == PointsOfInterestRenderingType.SHOW_IN_POSITION) {
				rt.localPosition = new Vector3(poi.position.x * mapContent.mapScale.x, poi.position.y * mapContent.mapScale.y, rt.localPosition.z) + mapContent.roomOffset;
			}
		}

		/// <summary>
		/// UPdates icons, for example open/closed door if using platformer pro
		/// </summary>
		virtual protected void UpdateIcon()
		{
			#if PLATFORMER_PRO
			if (poi.PointOfInterest != null && poi.PointOfInterest.Door != null)
			{
				if (poi.PointOfInterest.Door.state == PlatformerPro.DoorState.OPEN || poi.PointOfInterest.Door.keyType == null || poi.PointOfInterest.Door.keyType == "")
				{
					baseImage.sprite = MapManager.Instance.doorPoiData.icon;
					baseImage.color = MapManager.Instance.doorPoiData.color;
				}
				else
				{
					baseImage.sprite = MapManager.Instance.doorPoiData.alternateIcon;
					baseImage.color = MapManager.Instance.doorPoiData.alternateColor;
				}
			}
			else if (poi.PointOfInterest != null && poi.PointOfInterest.RespawnPoint != null)
			{
				if (poi.PointOfInterest.RespawnPoint.IsEnabled)
				{
					baseImage.sprite = MapManager.Instance.respawnPoiData.icon;
					baseImage.color = MapManager.Instance.respawnPoiData.color;
				}
				else
				{
					baseImage.sprite = MapManager.Instance.respawnPoiData.alternateIcon;
					baseImage.color = MapManager.Instance.respawnPoiData.alternateColor;
				}
			}
			#endif
		}
	}
}