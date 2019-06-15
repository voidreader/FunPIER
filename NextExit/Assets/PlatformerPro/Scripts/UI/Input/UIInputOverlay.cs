using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Shows a GamObject when a function is called or the [TAB] key is pressed. Intended for use with a UI input overlay.
	/// THis is primarily for development purposes although could serve as a basis for something in a final product.
	/// </summary>
	public class UIInputOverlay : MonoBehaviour {

		/// <summary>
		/// The object to activate/deacitvate when tab is pressed.
		/// </summary>
		public GameObject visibleContent;

		/// <summary>
		/// If true user can toggle this themselves by pressing TAB.
		/// </summary>
		public bool useTabKey = true;

		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update()
		{
			if (useTabKey && UnityEngine.Input.GetKeyDown (KeyCode.Tab))
			{
				ToggleShowHide ();
			}
		}

		/// <summary>
		/// Show or hide the visibleContent.
		/// </summary>
		public void ToggleShowHide()
		{
			if (visibleContent.activeSelf)
			{
				visibleContent.SetActive (false);
			} else
			{
				visibleContent.SetActive (true);
				foreach (UIShowButton sb  in visibleContent.GetComponentsInChildren<UIShowButton>())
				{
					sb.UpdateText ();
				}
			}
		}

		/// <summary>
		/// Force to show with the specified input.
		/// </summary>
		/// <param name="input">Input.</param>
		public void Show(Input input)
		{
			visibleContent.SetActive (true);
			foreach (UIShowButton sb in visibleContent.GetComponentsInChildren<UIShowButton>())
			{
				sb.input = input;
				sb.UpdateText ();
			}
		}

		/// <summary>
		/// Force to hide.
		/// </summary>
		public void Hide()
		{
			visibleContent.SetActive (false);
		}
	}
}
