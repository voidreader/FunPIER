using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace jnamobile.mmm {

	/// <summary>
	/// Base component for things drawn on a map
	/// </summary>
	public abstract class UIBaseMapComponent : MonoBehaviour {
		
		/// <summary>
		/// Holds all visible content. This object will be activated/deactivated when room is shown/hidden.
		/// </summary>
		[Tooltip ("Holds all visible content. This object will be activated/deactivated when component is shown/hidden. Only required if you wish to do things like centering map on a hidden room.")]
		[SerializeField]
		protected GameObject visibleContent;

		/// <summary>
		/// Basic image.
		/// </summary>
		[SerializeField]
		protected Image baseImage;

		/// <summary>
		/// Cached reference to map content we belong to.
		/// </summary>
		protected UIMapContent mapContent;

		/// <summary>
		/// Show UI.
		/// </summary>
		virtual public void Show() {
			if (visibleContent != null) visibleContent.SetActive (true);
			if (baseImage != null) baseImage.enabled = true;
		}
		
		/// <summary>
		/// Hide UI.
		/// </summary>
		virtual public void Hide() {
			if (visibleContent != null) visibleContent.SetActive (false);
			if (baseImage != null) baseImage.enabled = false;
		}
	}
}
