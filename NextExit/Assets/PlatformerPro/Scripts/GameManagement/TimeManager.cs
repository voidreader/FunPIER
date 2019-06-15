using UnityEngine;
using System.Collections;
using PlatformerPro.Extras;

namespace PlatformerPro
{
	/// <summary>
	/// Handles time related operations such as getting the current frame time. This
	/// implementation is a simple wrapper on delta time that limits the maximum time of a
	/// frame to a user defined maximum value.
	/// </summary>
	public class TimeManager : PlatformerProMonoBehaviour
	{
		/// <summary>
		/// The maximum permissable frame time.
		/// </summary>
		public float maximumFrameTime = DefaultMaximumFrameTime;

		/// <summary>
		/// The pause menu.
		/// </summary>
		protected UIPauseMenu pauseMenu;

		/// <summary>
		/// The default maximum frame time.
		/// </summary>
		public const float DefaultMaximumFrameTime = 0.033f;

		/// <summary>
		/// Gets the header string used to describe the component.
		/// </summary>
		/// <value>The header.</value>
		override public string Header
		{
			get
			{
				return "Handles time related operations such as getting the current frame time. This " +
					   "implementation is a simple wrapper on delta time that limits the maximum time " +
					   "of a frame to a user defined maximum value.";
			}
		}

		/// <summary>
		/// Event for pause.
		/// </summary>
		public event System.EventHandler <System.EventArgs> GamePaused;

		/// <summary>
		/// Event for unpause.
		/// </summary>
		public event System.EventHandler <System.EventArgs> GameUnPaused;

		/// <summary>
		/// Raises the game paused event.
		/// </summary>
		virtual public void OnGamePaused()
		{
			if (GamePaused != null)
			{
				GamePaused(this, null);
			}
		}

		/// <summary>
		/// Raises the game unpaused event.
		/// </summary>
		virtual public void OnGameUnPaused()
		{
			if (GameUnPaused != null)
			{
				GameUnPaused(this, null);
			}
		}

		#region static methods and properties

		/// <summary>
		/// Are we paused?
		/// </summary>
		protected static bool paused;

		/// <summary>
		/// Gets the frame time.
		/// </summary>
		public static float FrameTime
		{
			get
			{
				if (paused) return 0.0f;
				if (Instance == null) return Mathf.Min(Time.deltaTime, DefaultMaximumFrameTime);
				return Mathf.Min(Time.deltaTime, Instance.maximumFrameTime);
			}
		}

		/// <summary>
		/// Gets the maximum possible frame time.
		/// </summary>
		public static float MaxFrameTime
		{
			get
			{
				if (Instance == null) return DefaultMaximumFrameTime;
				return Instance.maximumFrameTime;
			}
		}

		/// <summary>
		/// The time manager instance.
		/// </summary>
		protected static TimeManager instance;

		/// <summary>
		/// Gets a static reference to the time manager if one exists.
		/// </summary>
		/// <value>The instance.</value>
		public static TimeManager Instance
		{
			get
			{
				if (instance == null) CreateNewTimeManager();
				return instance;
			}
			protected set
			{
				instance = value;
			}
		}

		/// <summary>
		/// Gets the instance only if it exists will not create a new one. Safe to call from OnDestroy while unregistering listeners.
		/// </summary>
		public static TimeManager SafeInstance
		{
			get { return instance;}
		}

		/// <summary>
		/// Creates a new time manager.
		/// </summary>
		protected static void CreateNewTimeManager()
		{
			GameObject go = new GameObject ();
			go.name = "TimeManager";
			instance = go.AddComponent<TimeManager> ();
		}

		#endregion

		#region Unity hooks

		/// <summary>
		/// Unity Awake hook.
		/// </summary>
		void Awake()
		{
			if (instance != null) Debug.LogError ("More than one TimeManager found in the scene.");
			Instance = this;
			paused = false;
		}

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start()
		{
			pauseMenu = FindObjectOfType<UIPauseMenu> ();
		}

		#endregion

		/// <summary>
		/// Pause the game.
		/// </summary>
		/// <param name="setTimeScale">If set to <c>true</c> then Time.timeScale will also be set to 0.</param>
		public void TogglePause(bool setTimeScale)
		{
            if (PlatformerProGameManager.Instance.GamePhase == GamePhase.GAME_OVER) return;
            if (paused) UnPause (setTimeScale);
			else Pause(setTimeScale);
		}

		/// <summary>
		/// Are we paused?
		/// </summary>
		public bool Paused
		{
			get { return paused; }
            set { paused = value; }
		}

		/// <summary>
		/// Pause the game.
		/// </summary>
		/// <param name="setTimeScale">If set to <c>true</c> then Time.timeScale will also be set to 0.</param>
		public void Pause(bool setTimeScale)
		{
            if (PlatformerProGameManager.Instance.GamePhase == GamePhase.GAME_OVER) return;
			paused = true;
			if (pauseMenu != null)
			{
				pauseMenu.Pause();
			}
			if (setTimeScale) Time.timeScale = 0;
			OnGamePaused ();
		}

		/// <summary>
		/// Unpause the game.
		/// </summary>
		/// <param name="setTimeScale">If set to <c>true</c> then Time.timeScale will also be set to 1.</param>
		public void UnPause(bool setTimeScale)
		{
			if (pauseMenu != null)
			{
				// Only unpause if the pause menu is at the last menu
				if (pauseMenu.UnPause())
				{
					paused = false;
					if (setTimeScale) Time.timeScale = 1;
					OnGameUnPaused ();
				}
			}
			else
			{
				paused = false;
				if (setTimeScale) Time.timeScale = 1;
				OnGameUnPaused ();
			}
		}

		/// <summary>
		/// Unpause the game.
		/// </summary>
		/// <param name="newTimeScale"> Time.timeScale will  be set to newTimeScale.</param>
		public void UnPause(float newTimeScale)
		{
			if (pauseMenu != null)
			{
				// Only unpause if the pause menu is at the last menu
				if (pauseMenu.UnPause())
				{
					paused = false;
					Time.timeScale = newTimeScale;
					OnGameUnPaused ();
				}
			}
			else
			{
				paused = false;
				Time.timeScale = newTimeScale;
				OnGameUnPaused ();
			}
		}
	}

}