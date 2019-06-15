using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Displays a GameObject (which typically has text information in it).
	/// </summary>
	public class UIDialog : MonoBehaviour
	{
	
		/// <summary>
		/// Time (in seconds) before the box is hidden after showing. Use 0 to disable auto hide.
		/// </summary>
		[Tooltip ("Time (in seconds) before the box is hidden after showing. Use 0 to disable auto hide.")]
		public float autoHideTime;

		/// <summary>
		/// Effects to play on show.
		/// </summary>
		[Tooltip ("Effects to play on show.")]
		public List<FX_Base> showEffects;

		/// <summary>
		/// The hide effects.
		/// </summary>
		[Tooltip ("Effects to play on hide.")]
		public List<FX_Base> hideEffects;

		/// <summary>
		/// Object to activate on show and deactivate on hide. Can be null if you use effects.
		/// </summary>
		[Tooltip ("Object to activate on show and deactivate on hide. Can be null if you use effects.")]
		public GameObject target;

		/// <summary>
		/// Where in the world should this object be shown.
		/// </summary>
		[Tooltip ("Where in the world should this object be shown. Can be a moving object.")]
		public Transform worldPosition;

		/// <summary>
		/// Should we update world position each frame (use to allow pixel perfect placement in world position).
		/// </summary>
		[Tooltip ("Should we update world position each frame (use to allow pixel perfect placement in world position).")]
		public bool updateWorldPositionEachFrame = true;

		/// <summary>
		/// Reference to the dialog typer or null if there isn't one.
		/// </summary>
		protected UIDialogTyper dialogTyper;

		/// <summary>
		/// Tracks if we are visible or not.
		/// </summary>
		protected bool visible;

		/// <summary>
		/// Are we visible?
		/// </summary>
		public bool Visible
		{
			get { return visible; }
		}

		/// <summary>
		/// Unity Update() hook.
		/// </summary>
		void LateUpdate()
		{
			if (worldPosition != null && updateWorldPositionEachFrame && target.activeInHierarchy)
			{
				((RectTransform)target.transform).position = worldPosition.position;
			}
		}

		/// <summary>
		/// Shows the dialog.
		/// </summary>
		virtual public void ShowDialog()
		{
			ShowDialog (null);
		}

		/// <summary>
		/// Shows the dialog.
		/// </summary>
		/// <param name="worldPosition">World position to use. Only applied if world position is not already set.</param>
		virtual public void ShowDialog(Transform worldPosition)
		{
			if (worldPosition != null)
			{
				if (this.worldPosition == null) this.worldPosition = worldPosition;
			}

			if (this.worldPosition == null) worldPosition = transform;

			((RectTransform)target.transform).position = worldPosition.position;
			if (target != null) target.SetActive (true);
			if (showEffects != null)
			{
				foreach (FX_Base fx in  showEffects)
				{
					fx.StartEffect();
				}
			}
			dialogTyper = GetComponentInChildren <UIDialogTyper> ();
			if (dialogTyper != null) 
			{
				dialogTyper.Show ();
				StartCoroutine(CheckForTyperHide());
			}
			if (autoHideTime > 0) StartCoroutine(AutoHide());
			visible = true;
		}

		/// <summary>
		/// Hides the dialog.
		/// </summary>
		virtual public void HideDialog()
		{
			if (target != null) target.SetActive (false);
			if (hideEffects != null)
			{
				foreach (FX_Base fx in hideEffects)
				{
					fx.StartEffect();
				}
			}
			visible = false;
			StopAllCoroutines ();
		}

		/// <summary>
		/// Coroutine to autohide the dialog.
		/// </summary>
		virtual protected IEnumerator AutoHide()
		{
			yield return new WaitForSeconds(autoHideTime);
			HideDialog();
		}

		/// <summary>
		/// Coroutine to check for closing of a dialog typer.
		/// </summary>
		/// <returns>The for typer hide.</returns>
		virtual protected IEnumerator CheckForTyperHide()
		{
			while (!dialogTyper.ReadyToHide) yield return true;
			HideDialog();
		}
	}
}