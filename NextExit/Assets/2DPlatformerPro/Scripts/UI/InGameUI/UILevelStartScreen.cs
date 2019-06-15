using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Displays some UI content on level start.
	/// </summary>
	public class UILevelStartScreen : MonoBehaviour
	{
		/// <summary>
		/// GameObject holding the content to show.
		/// </summary>
		[Tooltip ("GameObject holding the content to show.")]
		public GameObject visibleComponent;

		/// <summary>
		/// Show on start.
		/// </summary>
		[Tooltip ("Do we show this on scene Start?")]
		public bool showOnStart;

		/// <summary>
		/// Show on respawn.
		/// </summary>
		[Tooltip ("Do we show this on Respawn? Note that scene load wont trigger a respawn so you may want to use showOnStart too.")]
		public bool showOnRespawn;

		/// <summary>
		/// How long in total to show the screen before calling Hide().
		/// </summary>
		public float totalShowTime = 2.0f;

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
		/// Unity Start hook.
		/// </summary>
		void Start()
		{
			Init ();
		}

		/// <summary>
		/// Unity OnDestroy() hook.
		/// </summary>
		void OnDestroy()
		{
			if (showOnRespawn)
			{
				LevelManager.Instance.Respawned -= HandleRespawned;
			}
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		protected virtual void Init()
		{
			if (showOnStart) Show ();
			if (showOnRespawn) LevelManager.Instance.Respawned += HandleRespawned;
		}


		/// <summary>
		/// Handles the respawned event
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event args.</param>
		void HandleRespawned (object sender, SceneEventArgs e)
		{
			Show ();
		}

		/// <summary>
		/// Show this screen.
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
			yield return new WaitForSeconds (totalShowTime);
			Hide();
		}

		/// <summary>
		/// Hide this screen.
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