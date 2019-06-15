using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformerPro
{
	/// <summary>
	/// User interface scene loader overlay.
	/// </summary>
	public class UISceneLoaderOverlay : PlatformerProMonoBehaviour {

		/// <summary>
		/// Content to show when we load a new scene.
		/// </summary>
		[Tooltip ("Content to show when we load a new scene.")]
		public GameObject visibleContent;

		/// <summary>
		/// Names of scenes for which we will NOT show the loading overlay (for example main menu scene).
		/// </summary>
		[Tooltip ("Names of scenes for which we will NOT show the loading overlay (for example main menu scene).")]
		public List<string> ignoredScenes;

		/// <summary>
		/// Unity Awake hook.
		/// </summary>
		void Awake()
		{
			if (Instance == null)
			{
				DontDestroyOnLoad (this);
				Instance = this;
				Init ();
			}
			else
			{
				Destroy (this);
				Instance.Init ();
			}
		}

		/// <summary>
		/// Init this instance for the current scene.
		/// </summary>
		virtual protected void Init()
		{
			PlatformerProGameManager.Instance.PhaseChanged += PhaseChange;
		}

		/// <summary>
		/// Handles phase change.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event args.</param>
		virtual protected void PhaseChange (object sender, GamePhaseEventArgs e)
		{
			if (e.Phase == GamePhase.READY)
			{
				visibleContent.SetActive(false);
				LevelManager.Instance.WillExitScene += WillExitScene;
			}
		}

		/// <summary>
		/// Handles a scene exit
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event args.</param>
		virtual protected void WillExitScene (object sender, SceneEventArgs e)
		{
			PlatformerProGameManager.Instance.PhaseChanged -= PhaseChange;
			LevelManager.Instance.WillExitScene -= WillExitScene;
			if (!ignoredScenes.Contains (e.NewScene))
			{
				visibleContent.SetActive (true);
			}
		}

		/// <summary>
		/// Gets or sets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static UISceneLoaderOverlay Instance
		{
			get;
			protected set;
		}

	}
}