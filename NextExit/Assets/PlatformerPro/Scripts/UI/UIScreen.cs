using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// A screen is a user interface component that can be shown and hidden.
	/// </summary>
	public class UIScreen : PlatformerProMonoBehaviour 
	{
		/// <summary>
		/// The pointer image.
		/// </summary>
		[Tooltip ("The part of the menu that is actually drawn. This is activated/deactived as the menu is shown/hidden.")]
		public GameObject visibleComponent;

		/// <summary>
		/// The show effects.
		/// </summary>
		[Tooltip ("Effects to play when showing this menu.")]
		public List<FX_Base> showEffects;

		/// <summary>
		/// The hide effects.
		/// </summary>
		[Tooltip ("Effects to play when hiding this menu.")]
		public List<FX_Base> hideEffects;

		/// <summary>
		/// Gets the header string used to describe the component.
		/// </summary>
		/// <value>The header.</value>
		override public string Header
		{
			get
			{
				return "A screen is a user interface component that can be shown and hidden with optional effects.";
			}
		}

		/// <summary>
		/// Show this menu.
		/// </summary>
		virtual public void Show() 
		{
			StartCoroutine (DoShow ());
		}

		/// <summary>
		/// Do the show.
		/// </summary>
		virtual protected IEnumerator DoShow()
		{
			yield return true;
			visibleComponent.SetActive (true);
			if (showEffects != null && showEffects.Count > 0)
			{
				foreach(FX_Base effect in showEffects)
				{
					effect.StartEffect();
				}
			}
        }

		/// <summary>
		/// Hide this menu.
		/// </summary>
		virtual public void Hide()
		{
			StartCoroutine (DoHide ());
		}

		/// <summary>
		/// Do the hide.
		/// </summary>
		virtual protected IEnumerator DoHide()
		{
			yield return true;
			if (hideEffects != null && hideEffects.Count > 0)
			{
				foreach(FX_Base effect in hideEffects)
				{
					effect.StartEffect();
				}
			}
			else 
			{
				visibleComponent.SetActive (false);
			}

        }
	}
}