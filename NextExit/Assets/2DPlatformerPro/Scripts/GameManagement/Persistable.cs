using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace PlatformerPro
{
	/// <summary>
	/// Base class for objects that can be saved.
	/// </summary>
	public abstract class Persistable : MonoBehaviour, ICharacterReference
	{
		/// <summary>
		/// Load on Awake()
		/// </summary>
		[Header ("Load Options")]
		[HideInInspector]
		public bool loadOnAwake;
		
		/// <summary>
		/// Load on Start()
		/// </summary>
		[HideInInspector]
		public bool loadOnStart;
		
		/// <summary>
		/// Load on Character Load
		/// </summary>
		[HideInInspector]
		public bool loadOnCharacterLoad;
		
		/// <summary>
		/// Save whenever this value changes (not supported by all objects).
		/// </summary>
		[Header ("Save Options")]
		[HideInInspector]
		public bool saveOnChange;
		
		/// <summary>
		/// Save when the cahracter dies.
		/// </summary>
		[HideInInspector]
		public bool saveOnDeath;
		
		/// <summary>
		/// Save when the game is over.
		/// </summary>
		[HideInInspector]
		public bool saveOnGameOver;
		
		/// <summary>
		/// Save when the scene is exited.
		/// </summary>
		[HideInInspector]
		public bool saveOnSceneExit;
		
		/// <summary>
		/// Reset whenever the character dies.
		/// </summary>
		[Header ("Reset Options")]
		[HideInInspector]
		public bool resetOnDeath;
		
		/// <summary>
		/// Reset whenever the game ends.
		/// </summary>
		[HideInInspector]
		public bool resetOnGameOver;
		
		/// <summary>
		/// Reset whenever the scene is exited.
		/// </summary>
		[HideInInspector]
		public bool resetOnSceneExit;

		/// <summary>
		/// The character loader.
		/// </summary>
		protected CharacterLoader loader;

		/// <summary>
		/// The base player preference identifier.
		/// </summary>
		public const string BasePlayerPrefId = "PP.Persistent.";

		#region events

		/// <summary>
		/// Sent when this persistable is loaded.
		/// </summary>
		public event System.EventHandler <System.EventArgs> Loaded;

		/// <summary>
		/// Raises the loaded event.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="amount">Number collected.</param>
		/// <param name="character">Character.</param>
		virtual protected void OnLoaded()
		{
			if (Loaded != null)
			{
				Loaded(this, null);
			}
		}

		#endregion

		/// <summary>
		/// Unity Awake() hook.
		/// </summary>
		void Awake()
		{
			if (loadOnAwake) Load(this);
		}

		/// <summary>
		/// Unity Start() hook.
		/// </summary>
		void Start()
		{
			Init ();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void Init()
		{
			if (loadOnStart) Load(this);
			if (saveOnSceneExit || resetOnSceneExit || loadOnCharacterLoad)
			{
				if (Character == null)
				{
					if (saveOnSceneExit) Debug.LogWarning ("Can't save " + name + " (" + this.GetType() + ") on scene exit as no character could be found.");
					if (resetOnSceneExit) Debug.LogWarning ("Can't reset " + name + " on scene exit as no character could be found.");
				}
				else
				{
					Character.WillExitScene += HandleWillExitScene;
					loader = CharacterLoader.GetCharacterLoaderForCharacter(Character);
					if (loadOnCharacterLoad && loader != null) loader.CharacterLoaded += HandleCharacterLoaded;
				}
			}

			if (saveOnDeath || saveOnGameOver || resetOnDeath || resetOnGameOver){
				CharacterHealth characterHealth = null;
				if (this is CharacterHealth) characterHealth = (CharacterHealth)this;
				else
				{
					if (Character == null) Debug.LogWarning ("Couldn't find a character");
					else characterHealth = Character.gameObject.GetComponentInChildren<CharacterHealth>();
				}
				if (characterHealth != null) 
				{
					// Add handlers to character health events
					if (saveOnDeath || resetOnDeath) characterHealth.Died += HandleDied;
					if (saveOnGameOver || resetOnGameOver) characterHealth.GameOver += HandleGameOver;
				}
				else 
				{
					if (saveOnDeath) Debug.LogWarning ("Can't save " + name + " on death as no character health could be found.");
					if (saveOnGameOver) Debug.LogWarning ("Can't save " + name + " on game over as no character health could be found.");
					if (resetOnGameOver) Debug.LogWarning ("Can't reset " + name + " on game over as no character health could be found.");
				}
			}
		}

		/// <summary>
		/// Handles the character loaded.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event args.</param>
		void HandleCharacterLoaded (object sender, CharacterEventArgs e)
		{
			if (loadOnCharacterLoad) Load (this);
		}
		
		/// <summary>
		/// Handles the will exit scene event
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event.</param>
		virtual protected void HandleWillExitScene (object sender, SceneEventArgs e)
		{
			if (saveOnSceneExit) Save (this);
			if (resetOnSceneExit) Reset (this);
		}
		
		/// <summary>
		/// Handles the game over event
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event.</param>
		virtual protected  void HandleGameOver (object sender, DamageInfoEventArgs e)
		{
			if (saveOnGameOver) Save (this);
			if (resetOnGameOver) Reset (this);
		}
		
		/// <summary>
		/// Handles the died event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		virtual protected  void HandleDied (object sender, DamageInfoEventArgs e)
		{
			if (saveOnDeath) Save (this);
			if (resetOnDeath) Reset (this);
		}

		/// <summary>
		/// Save the given persistable.
		/// </summary>
		/// <param name="p">Persistable to save.</param>
		public static void Save(Persistable p)
		{
			object saveData = p.SaveData;
			// We assume all saveData is annotated with [Serializable] we could add a condition for ISeriazable too.
			if (saveData.GetType().IsSerializable)
			{
				using(StringWriter writer = new StringWriter())
				{
					XmlSerializer serializer = new XmlSerializer(saveData.GetType());
					serializer.Serialize(writer, saveData);
					PlayerPrefs.SetString(BasePlayerPrefId + p.Identifier, writer.ToString());
				}
			}
			else
			{
				Debug.LogError("Save data for " + p + " is not serializable.");
			}
		}

		/// <summary>
		/// Load the given persistable.
		/// </summary>
		/// <param name="p">Persistable to laod.</param>
		public static void Load(Persistable p)
		{
			string data = PlayerPrefs.GetString(BasePlayerPrefId + p.Identifier, "");
			if (data.Length > 0)
			{
				using (StringReader reader = new StringReader(data)){
					XmlSerializer serializer = new XmlSerializer(p.SavedObjectType());
					object savedObject = serializer.Deserialize(reader);
					p.ApplySaveData(savedObject);
				}
			}
			else
			{
				p.ResetSaveData();
			}
		}

		/// <summary>
		/// Resets the given persistable.
		/// </summary>
		/// <param name="p">Persistable to reset.</param>
		public static void Reset(Persistable p)
		{
			PlayerPrefs.DeleteKey (BasePlayerPrefId + p.Identifier);
			p.ResetSaveData();
		}

		#region abstract methods

		/// <summary>
		/// Gets the character.
		/// </summary>
		public abstract Character Character { get; }

		/// <summary>
		/// Gets the data to save.
		/// </summary>
		public abstract object SaveData { get; }

		/// <summary>
		/// Get a unique identifier to use when saving the data (for example this could be used for part of the file name or player prefs name).
		/// </summary>
		public  abstract string Identifier{ get; }

		/// <summary>
		/// Applies the save data to the object.
		/// </summary>
		public abstract void ApplySaveData(object t);

		/// <summary>
		/// Get the type of object this Persistable saves.
		/// </summary>
		public abstract System.Type SavedObjectType();

		/// <summary>
		/// Resets the save data back to default.
		/// </summary>
		public abstract void ResetSaveData();

		#endregion
	}

}
