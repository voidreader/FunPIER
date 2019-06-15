using UnityEngine;
#if !UNITY_4_6 && !UNITY_5_1 && !UNITY_5_2
using UnityEngine.SceneManagement;
#endif
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{

	/// <summary>
	/// Manages respawning and respawn points.
	/// </summary>
	public class LevelManager : Persistable
	{
	
		/// <summary>
		/// Name of the current level
		/// </summary>
		public string levelName;

		/// <summary>
		/// Unlock the level name automatically when level completes.
		/// </summary>
		public bool unlockLevelOnComplete = true;

		/// <summary>
		/// Should we automatically load another scene on level complete.
		/// Leave blank if there is a level complete screen which will do the loading.
		/// </summary>
		public string sceneToLoadOnLevelComplete;

		/// <summary>
		/// Should we clear respawn points when the scene loads?
		/// </summary>
		public bool clearRespawnPointsOnAwake = false;

		/// <summary>
		/// Scene to load when user exits level select.
		/// </summary>
		[Tooltip ("Scene to load when user exits level select.")]
		public string exitMenuScene;

		/// <summary>
		/// The currently active respawn point identifier.
		/// </summary>
		protected string activeRespawnPoint;

		/// <summary>
		/// List of all enabled respawn points.
		/// </summary>
		protected List<string> enabledRespawnPoints;

		/// <summary>
		/// The level lock data.
		/// </summary>
		protected List<LevelLockData> levelLockData;

		/// <summary>
		/// Character to use for persistence events. Generally this is for resetting locks on game over.
		/// </summary>
		protected Character character;

		/// <summary>
		/// Character loader to use for persistence events. Generally this is for resetting locks on game over.
		/// </summary>
		protected CharacterLoader characterLoader;

		/// <summary>
		/// Has a respawn point been set?
		/// </summary>
		public bool IsActive 
		{
			get { return activeRespawnPoint == null; }
		}

		/// <summary>
		/// Get a copy of the list of enabled respawn points.
		/// </summary>
		public  List<string> EnabledRespawnPoints
		{
			get 
			{
				List<string> result = new List<string>();
				result.AddRange(enabledRespawnPoints);
				return result;
			}
		}

		/// <summary>
		/// Exit the scene.
		/// </summary>
		public void Exit()
		{
			foreach (Character c in FindObjectsOfType<Character>()) c.AboutToExitScene(sceneToLoadOnLevelComplete);
			#if !UNITY_4_6 && !UNITY_5_1 && !UNITY_5_2
			LevelManager.PreviousLevel = SceneManager.GetActiveScene().name;
			SceneManager.LoadScene(exitMenuScene);
			#else
			LevelManager.PreviousLevel = Application.loadedLevelName;
			Application.LoadLevel(exitMenuScene);
			#endif
		}

		/// <summary>
		/// Const key to append in front of level lock string.
		/// </summary>
		public const string PLAYER_PREF_KEY = "LevelManager";

		#region events

		/// <summary>
		/// Event for scene load.
		/// </summary>
		public event System.EventHandler <SceneEventArgs> SceneLoaded;

		/// <summary>
		/// Event for character respawn.
		/// </summary>
		public event System.EventHandler <SceneEventArgs> Respawned;

		
		/// <summary>
		/// Event for level completed.
		/// </summary>
		public event System.EventHandler <LevelEventArgs> LevelComplete;
		
		/// <summary>
		/// Raises the respawned event.
		/// </summary>
		/// <param name="respawnPoint">Respawn point.</param>
		virtual protected void OnRespawned(string respawnPoint)
		{
			if (Respawned != null)
			{
				#if !UNITY_4_6 && !UNITY_5_1 && !UNITY_5_2
				Respawned(this, new SceneEventArgs(null, SceneManager.GetActiveScene().name, respawnPoint));
				#else
				Respawned(this, new SceneEventArgs(null, Application.loadedLevelName, respawnPoint));
				#endif

			}
		}

		/// <summary>
		/// Raises the scene loaded event on Start.
		/// </summary>
		virtual protected void OnSceneLoaded()
		{
			if (SceneLoaded != null)
			{
				#if !UNITY_4_6 && !UNITY_5_1 && !UNITY_5_2
				SceneLoaded(this, new SceneEventArgs(null, SceneManager.GetActiveScene().name));
				#else
				SceneLoaded(this, new SceneEventArgs(null, Application.loadedLevelName));
				#endif

			}
		}

		/// <summary>
		/// Raises the level complete event.
		/// </summary>
		/// <param name="respawnPoint">Respawn point.</param>
		virtual protected void OnLevelComplete(string levelName)
		{
			if (LevelComplete != null)
			{
				LevelComplete(this, new LevelEventArgs(levelName));
			}
		}

		#endregion

		#region Unity Hooks

		/// <summary>
		/// Unity Awake hook.
		/// </summary>
		void Awake()
		{
			if (instance != null) Debug.LogError ("More than one LevelManager found in the scene.");
			Instance = this;
			if (loadOnAwake) Load(this);
			if (clearRespawnPointsOnAwake) {
				enabledRespawnPoints = new List<string>();
				activeRespawnPoint = null;
				Save (this);
			}
		}

		/// <summary>
		/// Unity OnDestory hook.
		/// </summary>
		void OnDestroy()
		{
			DoDestroy ();
		}

		#endregion

		#region public methods

		/// <summary>
		/// Indicates level is complete.
		/// </summary>
		virtual public void LevelCompleted()
		{
			OnLevelComplete (levelName);
			if (levelName != null && levelName != "" && unlockLevelOnComplete) UnlockLevel (levelName);
			if (sceneToLoadOnLevelComplete != null && sceneToLoadOnLevelComplete != "") {
				foreach (Character c in FindObjectsOfType<Character>()) c.AboutToExitScene(sceneToLoadOnLevelComplete);
				#if !UNITY_4_6 && !UNITY_5_1 && !UNITY_5_2
				SceneManager.LoadScene(sceneToLoadOnLevelComplete);
				#else
				Application.LoadLevel (sceneToLoadOnLevelComplete);
				#endif
			}
		}

		/// <summary>
		/// Enables a spawn point but doesn't set it as active.
		/// </summary>
		virtual public void EnableRespawnPoint(string respawnPoint)
		{
			if (!enabledRespawnPoints.Contains(respawnPoint)) 
			{
				enabledRespawnPoints.Add (respawnPoint);
				if (saveOnChange) Save (this);
			}
		}

		/// <summary>
		/// Enables and actiavtes the respawn point.
		/// </summary>
		/// <param name="respawnPoint">Respawn point.</param>
		virtual public void ActivateRespawnPoint(string respawnPoint)
		{
			EnableRespawnPoint(respawnPoint);
			activeRespawnPoint = respawnPoint;
			if (saveOnChange) Save (this);
		}

		/// <summary>
		/// Respawn the specified character at the currently active reswpawn point.
		/// </summary>
		/// <param name="character">Character.</param>
		virtual public void Respawn(Character character)
		{
			Respawn (character, activeRespawnPoint);
		}

		/// <summary>
		/// Respawn the specified character at the specified respawn point.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="respawnPoint">Respawn point.</param>
		virtual public void Respawn(Character character, string respawnPoint)
		{
			RespawnPoint[] respawnPoints = GameObject.FindObjectsOfType<RespawnPoint> ();
			foreach (RespawnPoint r in respawnPoints)
			{
				if (r.identifier == respawnPoint)
				{
					ActivateRespawnPoint(respawnPoint);
					character.transform.position = r.transform.position;
					character.Respawn ();
					OnRespawned(r.identifier);
					return;
				}
			}
			foreach (RespawnPoint r in respawnPoints)
			{
				if (r.isDefaultStartingPoint)
				{
					character.transform.position = r.transform.position;
					character.Respawn ();
					OnRespawned(r.identifier);
					return;
				}
			}
			Debug.LogWarning ("Could not find " + respawnPoint + " or default respawn point.");
		}

		/// <summary>
		/// Clear all repsawn points (but dont reset level locks).
		/// </summary>
		virtual public void ClearRespawns()
		{
			activeRespawnPoint = null;
			enabledRespawnPoints.Clear ();
			if (saveOnChange) Save (this);
		}

		
		/// <summary>
		/// Unlocks the given level (or object).
		/// </summary>
		virtual public void UnlockLevel(string levelKey)
		{
			bool alreadyUnlocked = false;
			if (levelLockData == null)levelLockData = new List<LevelLockData>();
			for(int i = 0; i < levelLockData.Count; i++)
			{
				if (levelLockData[i].levelKey == levelKey && levelLockData[i].isUnlocked) alreadyUnlocked = true;
			}
			
			if (!alreadyUnlocked)
			{
				LevelLockData newUnlock = new LevelLockData(levelKey, true);
				levelLockData.Add(newUnlock);
				if (saveOnChange) Save (this);
			}
		}
		
		/// <summary>
		/// Unlocks the given level (or object).
		/// </summary>
		virtual public void LockLevel(string levelKey)
		{
			if (levelLockData == null)levelLockData = new List<LevelLockData>();
			for(int i = 0; i < levelLockData.Count; i++)
			{
				if (levelLockData[i].levelKey == levelKey && levelLockData[i].isUnlocked)
				{
					levelLockData.RemoveAt(i);
					if (saveOnChange)  Save (this);
				}
			}
		}
		
		/// <summary>
		/// Check if the given level (or other object) is unlocked.
		/// </summary>
		virtual public bool IsUnlocked(string levelKey)
		{
			if (levelLockData == null)levelLockData = new List<LevelLockData>();
			for(int i = 0; i < levelLockData.Count; i++)
			{
				if (levelLockData[i].levelKey == levelKey && levelLockData[i].isUnlocked)
				{
					return true;
				}
			}
			return false;
		}

		#endregion

		#region protected methods

		override protected void Init()
		{
			if (loadOnStart || resetOnSceneExit || loadOnCharacterLoad || saveOnDeath || saveOnGameOver || resetOnDeath)
			{
				Debug.LogWarning("Level Manager only supports load on awake, reset on game over, and save on change.");
			}
			if (resetOnGameOver)
			{
				character = FindObjectOfType <Character>();
				if (character == null)
				{
					characterLoader = CharacterLoader.GetCharacterLoader ();
					if (characterLoader != null) 
					{
						characterLoader.CharacterLoaded += HandleCharacterLoaded;
					}
					else
					{
						Debug.LogWarning("Level Manager screen has saveOnGameOver but it cannot find a Character.");
					}
				}
				else
				{
					// We have a character now Init the base
					base.Init ();
				}
			}
			else
			{
				base.Init ();
			}
			OnSceneLoaded ();
		}

		/// <summary>
		/// Handles the character loaded event by setting the character.
		/// </summary>
		virtual protected void HandleCharacterLoaded (object sender, CharacterEventArgs e)
		{
			if (character == null)
			{
				character = e.Character;
				// We have a character now Init the base
				base.Init ();
			}
		}

		/// <summary>
		/// Do the destroy actions.
		/// </summary>
		virtual protected void DoDestroy()
		{
			if (characterLoader != null)
			{
				characterLoader.CharacterLoaded -= HandleCharacterLoaded;
			}
		}


		#endregion

		#region static methods

		/// <summary>
		/// The instance.
		/// </summary>
		protected static LevelManager instance;
	
		/// <summary>
		/// Gets a reference to this object.
		/// </summary>
		public static LevelManager Instance
		{
			get
			{
				if (instance == null) CreateNewLevelManager();

				return instance;
			}
			protected set
			{
				instance = value;
			}
		}


		/// <summary>
		/// Keeps track of the previous level.
		/// </summary>
		public static string PreviousLevel
		{
			get; set;
		}

		/// <summary>
		/// Creates the level manager.
		/// </summary>
		protected static void CreateNewLevelManager()
		{
			GameObject go = new GameObject ();
			go.name = "LevelManager";

			instance = go.AddComponent<LevelManager> ();
			// Set some default persistence settings
			instance.loadOnAwake = true;
			if (GameObject.FindObjectOfType<CharacterHealth> () != null) instance.resetOnGameOver = true;
			instance.saveOnChange = true;
			// Load on awake wont be triggered ... so trigger it
			Load (instance);
			instance.Init ();
		}

		#endregion

		
		#region persistable methods
		
		/// <summary>
		/// Gets the character.
		/// </summary>
		override public Character Character
		{ 
			get
			{
				return character;
			}
		}

		/// <summary>
		/// Gets the data to save.
		/// </summary>
		override public object SaveData { get { return new LevelManagerData(activeRespawnPoint, enabledRespawnPoints, levelLockData); } }
		
		/// <summary>
		/// Get a unique identifier to use when saving the data (for example this could be used for part of the file name or player prefs name).
		/// </summary>
		override public string Identifier{ get { return PLAYER_PREF_KEY; } }
		
		/// <summary>
		/// Applies the save data to the object.
		/// </summary>
		override public void ApplySaveData(object t)
		{
			levelLockData = ((LevelManagerData)t).levelLockData;
			activeRespawnPoint = ((LevelManagerData)t).activeRespawnPoint;
			enabledRespawnPoints = ((LevelManagerData)t).enabledRespawnPoints;
		}
		
		/// <summary>
		/// Get the type of object this Persistable saves.
		/// </summary>
		override public System.Type SavedObjectType()
		{
			return typeof (LevelManagerData);
		}
		
		/// <summary>
		/// Resets the save data back to default.
		/// </summary>
		override public void ResetSaveData()
		{
			levelLockData = new List<LevelLockData> ();
			activeRespawnPoint = null;
			enabledRespawnPoints = new List<string> ();
			Save (this);
		}
		
		#endregion

	}
}