using UnityEngine;
using System.Collections;

namespace jnamobile.mmm {

	/// <summary>
	/// Shows player position on the map.
	/// </summary>
	public class UIPlayerIndicator : MonoBehaviour {

		/// <summary>
		/// Should we rotate with player?
		/// </summary>
		[Tooltip ("Should we match player rotation?")]
		public RotationMatchType rotateWithPlayer;

		/// <summary>
		/// Cached player object.
		/// </summary>
		protected GameObject player;

		/// <summary>
		/// Cached rect transform. Theoretically we can just cast transform but this can't hurt.
		/// </summary>
		protected RectTransform rt;

		/// <summary>
		/// Cached reference to the parent MapContent.
		/// </summary>
		protected UIMapContent mapContent;

		/// <summary>
		/// Gets the player position transformed to map orientation.
		/// </summary>
		public Vector3 PlayerPosition {
			get {
				if (MapManager.Instance.worldOrientation == WorldOrientation.X_Y) {
					return player.transform.position;
				} else {
					return new Vector3(player.transform.position.x, player.transform.position.z, player.transform.position.y);
				}
			}
		}

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start () {
			player = MapManager.Instance.player;
			rt = GetComponent<RectTransform> ();
			mapContent = GetComponentInParent<UIMapContent> ();
		}
		
		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update () {
			rt.localPosition = new Vector3 (PlayerPosition.x * mapContent.mapScale.x, PlayerPosition.y * mapContent.mapScale.y, rt.localPosition.z);
			switch (rotateWithPlayer) {
			case RotationMatchType.X:
				transform.localRotation = Quaternion.Euler (0, 0, -player.transform.rotation.eulerAngles.x);
				break;
			case RotationMatchType.Y:
				transform.localRotation = Quaternion.Euler (0, 0, -player.transform.rotation.eulerAngles.y);
				break;
			case RotationMatchType.Z:
				transform.localRotation = Quaternion.Euler (0, 0, -player.transform.rotation.eulerAngles.z);
				break;
			}
		}
	}

	/// <summary>
	/// Enum elaborating options for a UIPlayerIndicators rotation display.
	/// </summary>
	public enum RotationMatchType {
		DONT_ROTATE,
		X,
		Y,
		Z
	}
}