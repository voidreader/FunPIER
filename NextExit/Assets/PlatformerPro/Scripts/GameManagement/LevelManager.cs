using UnityEngine;
#if !UNITY_4_6 && !UNITY_4_7 && !UNITY_5_1 && !UNITY_5_2
using UnityEngine.SceneManagement;
#endif
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

        [Header ("Level Compeletion")]
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
        /// Should we clear all respawn points for this level on level complete?
        /// </summary>
        public bool clearRespawnsOnComplete = true;

        [Header("Other")]
        /// <summary>
        /// Scene to load when user exits level select.
        /// </summary>
        [Tooltip ("Scene to load when user exits level select.")]
		public string exitMenuScene;

        /// <summary>
        /// Should we save the name of the sceen to the saved scene data?
        /// </summary>
        [Tooltip("Should we save the active scene in to the scene data. We will update and save this whenever character is respawned.")]
        public bool saveSceneName = false;

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
        /// Stores the SceneManager name of the last entered scene.
        /// </summary>
        protected string lastEnteredScene;

        /// <summary>
        /// Has a respawn point been set?
        /// </summary>
        public bool IsActive 
		{
			get { return activeRespawnPoint == null; }
		}

		/// <summary>
		/// Return the active resapwn point
		/// </summary>
		public string ActiveRespawnPoint
		{
			get { return activeRespawnPoint; }
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
		public event System.EventHandler <CharacterEventArgs> Respawned;

		/// <summary>
		/// Event for level completed.
		/// </summary>
		public event System.EventHandler <LevelEventArgs> LevelComplete;

		/// <summary>
		/// Sent when we will exit a scene.
		/// </summary>
		public event System.EventHandler <SceneEventArgs> WillExitScene;


		/// <summary>
		/// Raises the respawned event.
		/// </summary>
		/// <param name="respawnPoint">Respawn point.</param>
		virtual protected void OnRespawned(Character character, int playerId)
		{
			if (Respawned != null)
			{
				Respawned (this, new CharacterEventArgs (character, playerId));
			}
		}

		/// <summary>
		/// Raises the scene loaded event on Start.
		/// </summary>
		virtual protected void OnSceneLoaded()
		{
			if (SceneLoaded != null)
			{
                SceneLoaded(this, new SceneEventArgs(null, SceneManager.GetActiveScene().name));
			}
		}

		/// <summary>
		/// Raises the level complete event.
		/// </summary>
		/// <param name="levelName">Name of complete level.</param>
		virtual protected void OnLevelComplete(string levelName)
		{
			if (LevelComplete != null)
			{
				LevelComplete(this, new LevelEventArgs(levelName));
			}
		}

        /// <summary>
        /// Raises the level complete event.
        /// </summary>
        /// <param name="newLevelName">Respawn point.</param>
        virtual protected void OnWillExitScene(string newLevelName)
		{
			if (WillExitScene != null)
			{
				WillExitScene(this, new SceneEventArgs(levelName, newLevelName));
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
			enabledRespawnPoints = new List<string>();
			levelLockData = new List<LevelLockData>();
			if (PlatformerProGameManager.Instance != null)
			{
				PlatformerProGameManager.Instance.PhaseChanged += HandlePhaseChange;
				PlatformerProGameManager.Instance.CharacterLoaded += HandleCharacterLoaded;
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
			if (levelName != null && levelName != "" && unlockLevelOnComplete) UnlockLevel (levelName);
            if (clearRespawnsOnComplete) ClearRespawns();
            Save(this);
            OnWillExitScene(sceneToLoadOnLevelComplete);
            OnLevelComplete(levelName);
            if (sceneToLoadOnLevelComplete != null && sceneToLoadOnLevelComplete != "") {
				LoadScene (sceneToLoadOnLevelComplete);
			}
		}


		/// <summary>
		///Load the given scene.
		/// </summary>
		virtual public void LoadScene(string sceneName)
		{
			foreach (Character c in PlatformerProGameManager.Instance.LoadedCharacters)
			{
				if (c != null) c.AboutToExitScene (sceneName);
			}
			Save (this);
			OnWillExitScene (sceneName);
			#if !UNITY_4_6 && !UNITY_4_7 && !UNITY_5_1 && !UNITY_5_2
			LevelManager.PreviousLevel = SceneManager.GetActiveScene().name;
			SceneManager.LoadScene(sceneName);
			#else
			LevelManager.PreviousLevel = Application.loadedLevelName;
			Application.LoadLevel(sceneName);
			#endif
		}


		/// <summary>
		/// Enables a spawn point but doesn't set it as active.
		/// </summary>
		virtual public void EnableRespawnPoint(string respawnPoint)
		{
			if (!enabledRespawnPoints.Contains(respawnPoint)) 
			{
				enabledRespawnPoints.Add (respawnPoint);
				if (SaveOnChange) Save (this);
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
			if (SaveOnChange) Save (this);
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
            if (saveSceneName)
            {
                lastEnteredScene = SceneManager.GetActiveScene().name;
                Save(this);
            }
			RespawnPoint[] respawnPoints = GameObject.FindObjectsOfType<RespawnPoint> ();
			if (respawnPoint != null)
			{
				foreach (RespawnPoint r in respawnPoints)
				{
					if (r.identifier == respawnPoint)
					{
						if (!r.dontActivateOnRespawn) ActivateRespawnPoint (respawnPoint);
						character.transform.position = r.transform.position;
						character.Respawn ();
						OnRespawned (character, character.PlayerId);
						return;
					}
				}
			}
			foreach (RespawnPoint r in respawnPoints)
			{
				if (r.isDefaultStartingPoint)
				{
					character.transform.position = r.transform.position;
					character.Respawn ();
					OnRespawned (character, character.PlayerId);
					return;
				}
			}
			Debug.LogWarning ("Could not find respawn point named ' " + respawnPoint + "' nor a default respawn point.");
		}

		/// <summary>
		/// Clear all repsawn points (but dont reset level locks).
		/// </summary>
		virtual public void ClearRespawns()
		{
			activeRespawnPoint = null;
			enabledRespawnPoints.Clear ();
			if (SaveOnChange) Save (this);
		}

		
		/// <summary>
		/// Unlocks the given level (or object).
		/// </summary>
		virtual public void UnlockLevel(string levelKey)
		{
			bool alreadyUnlocked = false;
			for(int i = 0; i < levelLockData.Count; i++)
			{
				if (levelLockData[i].levelKey == levelKey && levelLockData[i].isUnlocked) alreadyUnlocked = true;
			}
			
			if (!alreadyUnlocked)
			{
				LevelLockData newUnlock = new LevelLockData(levelKey, true, 1);
				levelLockData.Add(newUnlock);
				if (SaveOnChange) Save (this);
			}
		}

        /// <summary>
        /// Unlocks the given level (or object) and Sets a complete rating (i.e. snumber of stars, percentage, etc).
        /// </summary>
        virtual public void UnlockLevel(string levelKey, int completeRating)
        {
            bool alreadyUnlocked = false;
            for (int i = 0; i < levelLockData.Count; i++)
            {
                if (levelLockData[i].levelKey == levelKey && levelLockData[i].isUnlocked) alreadyUnlocked = true;
            }

            if (!alreadyUnlocked)
            {
                LevelLockData newUnlock = new LevelLockData(levelKey, true, completeRating);
                levelLockData.Add(newUnlock);
                if (SaveOnChange) Save(this);
            }
        }


        /// <summary>
        /// Unlocks the given level (or object).
        /// </summary>
        virtual public void LockLevel(string levelKey)
		{
			for(int i = 0; i < levelLockData.Count; i++)
			{
				if (levelLockData[i].levelKey == levelKey && levelLockData[i].isUnlocked)
				{
					levelLockData.RemoveAt(i);
					if (SaveOnChange)  Save (this);
				}
			}
		}
		
		/// <summary>
		/// Check if the given level (or other object) is unlocked.
		/// </summary>
		virtual public bool IsUnlocked(string levelKey)
		{
			for(int i = 0; i < levelLockData.Count; i++)
			{
				if (levelLockData[i].levelKey == levelKey && levelLockData[i].isUnlocked)
				{
					return true;
				}
			}
			return false;
		}

        virtual public int Rating(string levelKey)
        {
            for (int i = 0; i < levelLockData.Count; i++)
            {
                if (levelLockData[i].levelKey == levelKey)
                {
                    return levelLockData[i].rating;
                }
            }
            return 0;
        }

        #endregion

        #region protected methods

        virtual protected void Init() 
		{
            if (saveSceneName) lastEnteredScene = SceneManager.GetActiveScene().name;
            ConfigureEventListeners ();
        }

		override protected void ConfigureEventListeners ()
		{
			base.ConfigureEventListeners();
			OnSceneLoaded ();
        }

		virtual protected void HandlePhaseChange (object sender, GamePhaseEventArgs e)
		{
			if (e.Phase == GamePhase.CHARACTERS_LOADED)
			{
				Load (this);
			}
		}

		/// <summary>
		/// The player preference identifier.
		/// </summary>
		override public string PlayerPrefsIdentifier
		{
			get
			{
				// Level manager is not attached to a character
				// MUTLIPLAYER TODO - Different spawn points for different characters
				string levelName = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name;
				return string.Format("{0}_{1}_{2}", BasePlayerPrefId, Identifier, 0);
			}
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
				base.ConfigureEventListeners();
			}
		}

		/// <summary>
		/// Do the destroy actions.
		/// </summary>
		virtual protected void DoDestroy()
		{
			if (PlatformerProGameManager.Instance != null)
			{
				PlatformerProGameManager.Instance.PhaseChanged += HandlePhaseChange;
				PlatformerProGameManager.Instance.CharacterLoaded += HandleCharacterLoaded;
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
            set
            {
                Debug.LogWarning("Can't set character on LevelManager");
            }
        }

		/// <summary>
		/// Gets the data to save.
		/// </summary>
		override public object SaveData { get { return new LevelManagerData(activeRespawnPoint, enabledRespawnPoints, levelLockData, lastEnteredScene); } }
		
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
            lastEnteredScene = null;
            Save (this);
		}
		
		#endregion

	}
}