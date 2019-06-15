using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PlatformerPro
{
	/// <summary>
	/// The master Game Manager controls loading characters, triggering persistence, and setting up the 
	/// game ready to play.
	/// </summary>
	public class PlatformerProGameManager : PlatformerProMonoBehaviour
	{
		/// <summary>
		/// List of all available characters. These can be loaded at the start of the game or while the game is in progress.
		/// If there is only one character in this list it will be automatically loaded.
		/// </summary>
		[Tooltip ("List of all available characters. These can be loaded at the start of the game or while the game is in progress." +
				  "If there is only one character in this list it will be automatically loaded.")]
		public List<AvailableCharacterData> availableCharacters;

		[Header ("Persistence")]

		/// <summary>
		/// Should we enable saving and loading?
		/// </summary>
		[Tooltip ("Should we enable saving and loading? This is only a default and can be overriden by individual types.")]
		public bool enablePersistence = true;

		/// <summary>
		/// How do we handle persistence reset?
		/// </summary>
		[Tooltip ("When does saved data reset. This is only a default and can be overriden by individual types.")]
		[EnumMask]
		public PersistenceResetType defaultPersistenceReset;

		/// <summary>
		/// Will we save always, or only when the cahracter dies or leaves a scene.
		/// </summary>
		[Tooltip ("Will we save always, or only when the character dies or leaves a scene. Always saving can have" +
				  " a significant performance hit, particualrly on mobile.")]
		public bool saveOnAnyChange;

        /// <summary>
        /// Save always, but only in the editor.
        /// </summary>
        [Tooltip("Save always, but only in the editor.")]
        public bool saveOnAnyChangeEditorOnly;

        [Header ("Object Persistence")]

		/// <summary>
		/// Should we (by default) persist platformer pro object like doors, platforms, items, etc. You can override on invidiual objects.
		/// </summary>
		[Tooltip ("Should we (by default) persist platformer pro object like doors, platforms, items, etc. You can override on invidiual objects.")]
		public bool persistObjectsInLevel;

		/// <summary>
		/// Shoudl we persist item state by default.
		/// </summary>
		[Tooltip ("How Should we persist item state by default.")]
		public PersistableObjectType itemPersistenceType = PersistableObjectType.ACTIVATE_DEACTIVATE;

		/// <summary>
		/// Should we persist enemy state by default.
		/// </summary>
		[EnumMask]
		[Tooltip ("How Should we persist enemy state by default.")]
		public PersistableEnemyType enemyPersistenceType = PersistableEnemyType.ALIVE_DEAD;


		[Header ("Game End")]

		/// <summary>
		/// When does the game end.
		/// </summary>
		[Tooltip ("When does the game end")]
		public GameOverType endGameWhen;

        [Header("Other")]

        /// <summary>
        /// Set to true if this is not a normal playable level.
        /// </summary>
        [Tooltip("Set to true if this is not a normal playable level")]
        public bool isMapOrMenu;

		/// <summary>
		/// Tracks if we have initialised an instance of the game manager.
		/// </summary>
		protected static bool initialised;

		/// <summary>
		/// List of character ids which should be loaded on Start(). One slot per player. If a slot is null
		/// tha given player is not playing.
		/// </summary>
		protected static List<string> charactersToLoad;

		/// <summary>
		/// All the loaded characters.
		/// </summary>
		protected List<Character> loadedCharacters;

		/// <summary>
		/// Constant for a player id that matches any player.
		/// </summary>
		public const int ANY_PLAYER = -1;

		#region properties

		/// <summary>
		/// Gets the header string used to describe the component.
		/// </summary>
		/// <value>The header.</value>
		override public string Header
		{
			get
			{
				return "The master Game Manager controls loading characters, triggering persistence, and " +
					"setting up the game ready to play. Its a required component.";
			}
		}

		/// <summary>
		/// Validate required components.
		/// </summary>
		/// <param name="myTarget"></param>
		override public void Validate(PlatformerProMonoBehaviour myTarget)
		{
			base.Validate(myTarget);
			#if UNITY_EDITOR
			if (enablePersistence)
			{
				LevelManager l = FindObjectOfType<LevelManager>();
				if (l == null)
				{
					ShowValidationHeader();
					EditorGUILayout.HelpBox("A LevelManager is required if Persistence is enabled", MessageType.Info);
					GUILayout.BeginHorizontal();
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Create LevelManager", EditorStyles.miniButton))
					{
						GameObject go = new GameObject();
						go.name = "LevelManager";
						go.transform.parent = myTarget.transform;
						l = go.AddComponent<LevelManager>();
						l.levelName = "New Level";
					}
					GUILayout.EndHorizontal();
				}
			}
			#endif
		}
		
		/// <summary>
		/// The game loading phase.
		/// </summary>
		protected GamePhase gamePhase;

		/// <summary>
		/// Gets or sets the game loading phase and send an event when set.
		/// </summary>
		virtual public GamePhase GamePhase
		{
			get
			{
				return gamePhase;
			}
			protected set
			{
				if (value != gamePhase)
				{
					gamePhase = value;
					OnPhaseChanged (gamePhase);
				}
			}

		}

		/// <summary>
		/// Returns all loaded characters. WARNING: For performance reasons this is not a copy.
		/// </summary>
		virtual public List<Character> LoadedCharacters
		{
			get
			{
				return loadedCharacters;
			}
		}

		/// <summary>
		/// Gets or sets the persistable object manager.
		/// </summary>
		/// <value>The persistable object manager.</value>
		virtual public PersistableObjectManager PersistableObjectManager
		{
			get;
			protected set;
		}

		#endregion

		/// <summary>
		/// Event fired when a character finishes loading.
		/// </summary>
		public event System.EventHandler <CharacterEventArgs> CharacterLoaded;

		/// <summary>
		/// Event fired when a character is removed.
		/// </summary>
		public event System.EventHandler <CharacterEventArgs> CharacterRemoved;

		/// <summary>
		/// Event fired when Game Phase changes.
		/// </summary>
		public event System.EventHandler <GamePhaseEventArgs> PhaseChanged;


		/// <summary>
		/// Raises the character loaded event.
		/// </summary>
		virtual protected void OnCharacterLoaded(Character character)
		{
			if (CharacterLoaded != null)
			{
				CharacterLoaded(this, new CharacterEventArgs(character));
			}
		}

		/// <summary>
		/// Raises the character removed event.
		/// </summary>
		virtual protected void OnCharacterRemoved(Character character)
		{
			if (CharacterRemoved != null)
			{
				CharacterRemoved(this, new CharacterEventArgs(character));
			}
		}


		/// <summary>
		/// Raises the phase change event.
		/// </summary>
		/// <param name="phase">Completed phase.</param>
		virtual protected void OnPhaseChanged(GamePhase phase)
		{
			if (PhaseChanged != null)
			{
				PhaseChanged(this, new GamePhaseEventArgs(phase));
			}
		}

		/// <summary>
		/// Unity Awake hook.
		/// </summary>
		void Awake ()
		{
			Init ();
		}

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start () 
		{
			PostInit ();
		}

		/// <summary>
		/// Initialise this instance. Called form Awake.
		/// </summary>
		virtual protected void Init()
		{
			instance = this;
			loadedCharacters = new List<Character> ();
#if UNITY_EDITOR
            if (saveOnAnyChangeEditorOnly) saveOnAnyChange = true;
#endif
            PersistableObjectManager = PersistableObjectManager.CreateNewPersistableObjectManager (saveOnAnyChange);
			initialised = true;
			GamePhase = GamePhase.SCENE_ENTERED;

		}

		/// <summary>
		/// Called from start. Use this to get references, etc. 
		/// </summary>
		virtual protected void PostInit()
		{
			DoLoad ();
		}
			
		/// <summary>
		/// Loads the character.
		/// </summary>
		virtual protected void LoadCharacter()
		{
			DoLoad ();
		}

		/// <summary>
		/// Does the load after a delay.
		/// </summary>
		virtual protected void DoLoad()
		{
            // Special handling for non playable/map levels
            if (isMapOrMenu)
            {
                GamePhase = GamePhase.CHARACTERS_LOADED;
                StartCoroutine(PostLoad());
                return;
            }

            // Instantiate
            if (availableCharacters == null || availableCharacters.Count == 0)
			{
                Character c = FindObjectOfType<Character>();
                if (c != null)
                {
                    AvailableCharacterData a = new AvailableCharacterData();
                    a.characterInScene = c;
                    Debug.LogWarning("You should define your character in the GameManager settings. Currently using first result returned by FindObjectOfType<Character>()");
                } else { 
                    Debug.LogError ("CRITICAL: Game Manager must define at least one character!");
                }
                return;
			}
			if (charactersToLoad == null || charactersToLoad.Count == 0)
			{
				// Load default character
				LoadCharacter (0, availableCharacters[0].characterId);
			} 
			else
			{
				for (int i = 0; i < charactersToLoad.Count; i++)
				{
					if (charactersToLoad [i] != null)
					{
						LoadCharacter (i, charactersToLoad [i]);
					}
				}
			}
			GamePhase = GamePhase.CHARACTERS_LOADED;
			StartCoroutine (PostLoad ());
		}

		/// <summary>
		/// After start frame tell send events for loaded characters.
		/// </summary>
		/// <returns>The character loaded events.</returns>
		virtual protected IEnumerator PostLoad()
		{
			yield return true;
			// Spawn
			for (int i = 0; i < loadedCharacters.Count; i++)
			{
				if (loadedCharacters[i] != null) LevelManager.Instance.Respawn (loadedCharacters [i]);
			}
			// Send Character events
			for (int i = 0; i < loadedCharacters.Count; i++)
			{
				if (loadedCharacters[i] != null) OnCharacterLoaded (loadedCharacters [i]);
			}
			// Listen for character events
			for (int i = 0; i < loadedCharacters.Count; i++)
			{
				if (loadedCharacters [i] != null && loadedCharacters [i].CharacterHealth != null)
				{
					loadedCharacters [i].CharacterHealth.GameOver += HandleGameOver;
				}
			}
			GamePhase = GamePhase.READY;
		}

		/// <summary>
		/// Loads the character with the given id.
		/// </summary>
		/// <param name="playerId">Player identifier.</param>
		/// <param name="characterId">CharacterId.</param>
		virtual protected void LoadCharacter(int playerId, string characterId)
		{
			Character character = null;
			AvailableCharacterData data = availableCharacters.Where (c => c.characterId == characterId).FirstOrDefault ();
			if (data == null)
			{
				Debug.LogError("Couldn't find a character for characterId: " + characterId);
				return;
			}
			if (data.characterInScene != null)
			{
				// Use scene character
				character = data.characterInScene;
			}
			else if (data.characterPrefab != null)
			{
				// Create character
				GameObject go = GameObject.Instantiate (data.characterPrefab);
				character = go.GetComponent<Character> ();
				if (character == null)
				{
					Debug.LogError ("The character prefab in the Game Manager must have a Character component at the top level of the object hierarchy");
					return;
				}
			}
			character.PlayerId = playerId;
			while (loadedCharacters.Count <= playerId + 1)
			{
				loadedCharacters.Add (null);
			}
			loadedCharacters[playerId] = character;
			if (!character.gameObject.activeSelf) character.gameObject.SetActive (true);
		}

		/// <summary>
		/// Handle game over events
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		virtual protected void HandleGameOver (object sender, CharacterEventArgs e)
		{
			loadedCharacters [e.Character.PlayerId] = null;
			if (endGameWhen == GameOverType.ONE_CHARACTER_HAS_NO_LIVES || loadedCharacters.Where(c=>c != null).Count() == 0)
			{
				GamePhase = GamePhase.GAME_OVER;
			}
		}

		/// <summary>
		/// Send the character loaded event after yielding for a frame.
		/// </summary>
		/// <returns>The load.</returns>
		/// <param name="c">C.</param>
		virtual protected IEnumerator PostLoad(Character c)
		{
			yield return true;
			LevelManager.Instance.Respawn (loadedCharacters [c.PlayerId]);
			OnCharacterLoaded (c);
		}

		#region public methods

		/// <summary>
		/// Gets the character for fiven player identifier. If id = -1 return first active character.
		/// Returns null if no character for playerId
		/// </summary>
		/// <returns>The character for given player identifier.</returns>
		/// <param name="playerId">Player identifier.</param>
		public Character GetCharacterForPlayerId(int playerId)
		{
			if (playerId == ANY_PLAYER)
			{
				for (int i = 0; i < loadedCharacters.Count; i++)
				{
					if (loadedCharacters [i] != null) return loadedCharacters [i];
				}
			}
			if (loadedCharacters.Count > playerId && playerId >= 0)
			{
				return loadedCharacters [playerId];
			}
			return null;
		}

		/// <summary>
		/// Spawns a new character. Fails if a character with the supplied player id exists.
		/// </summary>
		/// <param name="playerId">Player identifier.</param>
		/// <param name="characterId">Character identifier.</param>
		public void SpawnNewCharacter(int playerId, string characterId)
		{
			if (loadedCharacters.Count < playerId || loadedCharacters[playerId] == null)
			{
				LoadCharacter(playerId, characterId);
				StartCoroutine (PostLoad (loadedCharacters [playerId]));
			}
			else 
			{
				Debug.LogWarning("Couldn't spawn a new character for player id " + playerId + " as that player already exists.");
			}
		}

		/// <summary>
		/// Repalce a character with a different one.
		/// </summary>
		/// <param name="playerId">Player identifier.</param>
		/// <param name="characterId">Character identifier.</param>
		public void ReplaceCharacter(int playerId, string characterId)
		{
			if (loadedCharacters.Count > playerId && loadedCharacters[playerId] != null)
			{
				Vector3 position = loadedCharacters [playerId].transform.position;
				loadedCharacters [playerId].CharacterHealth.GameOver -= HandleGameOver;
				OnCharacterRemoved (loadedCharacters [playerId]);
				Destroy (loadedCharacters [playerId].gameObject);
				LoadCharacter(playerId, characterId);
				loadedCharacters [playerId].transform.position = position;
				StartCoroutine (PostLoad (loadedCharacters [playerId]));
			}
			else 
			{
				Debug.LogWarning("Couldn't repalce a new character for player id " + playerId + " as the player doesn't exist.");
			}
		}

		/// <summary>
		/// Spawns a new character. Fails if a character with the supplied player id exists.
		/// </summary>
		/// <param name="playerId">Player identifier.</param>
		/// <param name="characterId">Character identifier.</param>
		public void SwitchCharacter(int currentPlayerId, int newPlayerId)
		{
			if (loadedCharacters.Count > currentPlayerId && loadedCharacters[currentPlayerId] != null &&
				loadedCharacters.Count > newPlayerId && loadedCharacters[newPlayerId] != null)
			{
				loadedCharacters [currentPlayerId].InputEnabled = false;
				loadedCharacters [newPlayerId].InputEnabled = true;
			}
			else 
			{
				Debug.LogWarning ("Couldn't switch characters as one of the player ids didn't exist");
			}
		}
		#endregion

		#region static behaviour

		/// <summary>
		/// Stores the singleton instance.
		/// </summary>
		protected static PlatformerProGameManager instance;

		/// <summary>
		/// Gets a character loader (generally for single character games only).
		/// </summary>
		/// <returns>A character loader or null if none found.</returns>
		public static PlatformerProGameManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<PlatformerProGameManager> ();
					#if UNITY_EDITOR
					if (Application.isPlaying)
					{
						if (instance != null) instance.Init ();
					}
					#else
					if (instance != null) instance.Init ();
					#endif
				}
				if (instance == null)
				{
					if (!initialised) Debug.LogError ("CRITICAL ERROR: You must have a PlatformerProGameManager in your scene.");
					return null;
				}
				return instance;
			}
			protected set
			{
				instance = value;
			}
		}

		/// <summary>
		/// Sets the character to load for a given player;
		/// </summary>
		/// <param name="playerId">Player identifier.</param>
		/// <param name="characterId">Character identifier.</param>
		public static void SetCharacterForPlayer(int playerId, string characterId)
		{
			if (playerId < 0)
			{
				Debug.LogWarning ("The playerId must be 0 or larger.");
				return;
			}
			else if (playerId >= 8)
			{
				Debug.LogWarning ("The playerId " + playerId + " is too large. Character will not be loaded.");
				return;
			}
			if (charactersToLoad == null) charactersToLoad = new List<string> ();
			while (charactersToLoad.Count <= playerId + 1)
			{
				charactersToLoad.Add (null);
			}
			charactersToLoad[playerId] = characterId;
		}

		#endregion
	}

	/// <summary>
	/// Characters that can be loaded.
	/// </summary>
	[System.Serializable]
	public class AvailableCharacterData
	{
		/// <summary>
		/// Unique id for the character.
		/// </summary>
		[Tooltip ("Unique id for the character.")]
		public string characterId;

		/// <summary>
		/// The character prefab used to create the character.
		/// </summary>
		[Tooltip ("The character prefab used to create the character.")]
		public GameObject characterPrefab;

		/// <summary>
		/// If the character is already in the scene you can link here. This is not the preferred method
		/// but makes it easier to work with some other assets without writing code.
		/// </summary>
		[Tooltip ("If the character is already in the scene you can link here. This is not the preferred method" +
				  "but makes it easier to work with some other assets without writing code.")]
		public Character characterInScene;

		/// <summary>
		/// Optional sprite used to show who character is both in game and in inspector .
		/// </summary>
		[Tooltip ("Optional sprite used to show who character is both in game and in inspector.")]
		public Sprite characterProfileImage;

	}
		
	/// <summary>
	/// Tracks where we are the initialisiation phases.
	/// </summary>
	public enum GamePhase
	{
		NONE,
		SCENE_ENTERED,
		CHARACTERS_LOADED,
		READY,
		GAME_OVER
	}

	/// <summary>
	/// When does the game end?
	/// </summary>
	public enum GameOverType
	{
		ONE_CHARACTER_HAS_NO_LIVES,
		ALL_CHARACTERS_HAVE_NO_LIVES
	}
}