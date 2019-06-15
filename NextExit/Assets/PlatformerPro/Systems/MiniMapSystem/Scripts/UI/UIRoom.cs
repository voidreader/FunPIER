using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace jnamobile.mmm {

	/// <summary>
	/// Represents a room in the UI.
	/// </summary>
	public class UIRoom : UIBaseMapComponent {

		/// <summary>
		/// The room we represent.
		/// </summary>
		protected MapRoomData room;

		/// <summary>
		/// Initialise room with given room data. It is expected that the room will be parented to the
		/// map content GameObject before this method is invoked.
		/// </summary>
		/// <param name="content">Parent map content object.</param>
		/// <param name="room">Room.</param>
		virtual public void Init(UIMapContent content, MapRoomData room) {
			this.room = room;
			this.mapContent = content;

			if (baseImage == null) baseImage = GetComponentInChildren<Image>();

			RectTransform rt = gameObject.GetComponent<RectTransform> ();
			rt.SetParent(content.UIRoomContainer, false);
			if (!content.ignoreColor) baseImage.color = room.colorOverride;
			if (room.OverrideSprite != null && !content.ignoreSpriteOverride) baseImage.sprite = room.OverrideSprite;

			rt.sizeDelta = new Vector2((float)room.width * content.mapScale.x * MapManager.Instance.gridSize, (float)room.height * content.mapScale.y * MapManager.Instance.gridSize);
			rt.localPosition = new Vector3(room.position.x * content.mapScale.x, room.position.y * content.mapScale.y, rt.localPosition.z) + content.roomOffset;

		}

		/// <summary>
		/// Update room to show entered state.
		/// </summary>
		virtual public void Entered() {
			Show ();
		}

		/// <summary>
		/// Update room to remove entered state.
		/// </summary>
		virtual public void Left() {
		}



		virtual public void UpdateVisibility() {
			
			// Update color
			if (!room.revealed && baseImage != null) {
				baseImage.color = mapContent.unrevealedRoomColor;
			} else {
				if (mapContent.ignoreColor) {
					baseImage.color = Color.white;
				} else {
					baseImage.color = room.colorOverride;
				}
			}
		}

	}
}