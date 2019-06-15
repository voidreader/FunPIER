using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A TimeManager extension which kills all characters after a timer expires. 
	/// </summary>
	public class TimeManagerWithTimer : TimeManager 
	{
		/// <summary>
		/// Time limit in seconds. 
		/// </summary>
		[Tooltip ("Time limit in seconds. After this reaches zero all characters will be killed.")]
		[SerializeField]
		protected float timeLimit;

		/// <summary>
		/// Maximum time limit.
		/// </summary>
		[Tooltip ("Maximum Time. Power-ups/items will not be able to increase the timer above this number.")]
		[SerializeField]
		protected float maxTime;

		/// <summary>
		/// The actual timer, when this reaches zero all characters will be killed.
		/// </summary>
		protected float timer;

		/// <summary>
		/// have we timed out and sent the kill message.
		/// </summary>
		protected bool killSent;

		/// <summary>
		/// Cached reference to level manager used for event listeners.
		/// </summary>
		protected LevelManager levelManager;

		/// <summary>
		/// Gets the current timer value.
		/// </summary>
		public float CurrentTime
		{
			get { return timer; }
		}

		/// <summary>
		/// Unity Awake hook.
		/// </summary>
		void Awake()
		{
			if (instance != null) Debug.LogError ("More than one TimeManager found in the scene.");
			Instance = this;
			paused = false;
			RegisterListeners ();
			ResetTimer ();
		}

		/// <summary>
		/// Unity update hook.
		/// </summary>
		void Update() {
			if (!paused && !killSent)
			{
				timer -= FrameTime;
				if (timer <= 0) 
				{
					timer = 0;
					KillAllCharacters();
				}
			}
		}

		/// <summary>
		/// Unity OnDestroy hook.
		/// </summary>
		void OnDestroy()
		{
			DeregisterListeners ();
		}

		/// <summary>
		/// Adds the specified amount of time. You can also use a negative value to remove time.
		/// </summary>
		/// <returns>The time.</returns>
		/// <param name="extraTime">Extra time.</param>
		public float AddTime(float extraTime) {
			timer += extraTime;
			if (timer > maxTime) timer = maxTime;
			if (timer < 0) timer = 0;
			return timer;
		}

		/// <summary>
		/// Kills all characters.
		/// </summary>
		protected void KillAllCharacters() 
		{
			DamageInfo info = new DamageInfo(0, DamageType.TIME_EXPIRED, Vector2.zero);
			foreach (CharacterHealth c in FindObjectsOfType<CharacterHealth>())
			{
				c.Kill(info);
			}
			killSent = true;
		}


		/// <summary>
		/// Resets the timer.
		/// </summary>
		protected void ResetTimer()
		{
			if (timeLimit < 1) timeLimit = 1.0f;
			timer = timeLimit;
			if (maxTime < timeLimit) maxTime = timeLimit;
			killSent = false;
		}

		/// <summary>
		/// Registers the listeners.
		/// </summary>
		protected void RegisterListeners()
		{
			levelManager = LevelManager.Instance;
			levelManager.SceneLoaded += HandleLoadOrRespawn;
			levelManager.Respawned += HandleLoadOrRespawn;

		}
		
		/// <summary>
		/// Registers the listeners.
		/// </summary>
		protected void DeregisterListeners()
		{
			if (levelManager != null)
			{
				levelManager.SceneLoaded += HandleLoadOrRespawn;
				levelManager.Respawned += HandleLoadOrRespawn;
			}
		}


		/// <summary>
		/// Handles the load or respawn event by resetting timer.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		protected void HandleLoadOrRespawn (object sender, SceneEventArgs e)
		{
			ResetTimer ();
		}
	}
}