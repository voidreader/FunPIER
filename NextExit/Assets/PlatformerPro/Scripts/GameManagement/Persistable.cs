using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace PlatformerPro
{
	/// <summary>
	/// Base class for objects that can be saved.
	/// </summary>
	public abstract class Persistable : PlatformerProMonoBehaviour, ICharacterReference
	{

		/// <summary>
		/// Should we use the persistence defaults or override with our own values.
		/// </summary>
		[Tooltip("Should we use the persistence defaults or override with our own values.")]
		[HideInInspector]
		[Header ("Persistence")]
		public bool usePersistenceDefaults = true;

		/// <summary>
		/// Should we enable persistence.
		/// </summary>
		[HideInInspector]
		[Tooltip("Should we enable persistence.")]
		public bool enablePersistence = true;

		/// <summary>
		/// Save whenever this value changes (Note: not supported by all objects).
		/// </summary>
		[Tooltip("Save whenever this value changes (Note: not supported by all objects).")]
		[HideInInspector]
		public bool saveOnAnyChange;

		/// <summary>
		/// When do we reset persistence?
		/// </summary>
		[Tooltip("When do we reset persistence?")]
		[HideInInspector]
		[EnumMask]
		public PersistenceResetType persistenceType;

		/// <summary>
		/// Have we loaded the data yet?
		/// </summary>
		protected bool loaded;

		/// <summary>
		/// The base player preference identifier.
		/// </summary>
		public const string BasePlayerPrefId = "PP.Persistent.";

		/// <summary>
		/// The player preference identifier.
		/// </summary>
		virtual public string PlayerPrefsIdentifier
		{
			get
			{
				string levelName = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name;
				return string.Format("{0}_{1}_{2}", BasePlayerPrefId, Identifier, Character.PlayerId);
			}
		}

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

		/// <summary>
		/// Gets the value for persistence type taking in to account defaults.
		/// </summary>
		virtual public bool EnablePersistence
		{
			get
			{
				if (usePersistenceDefaults) return PlatformerProGameManager.Instance.enablePersistence;
				return enablePersistence;
			}
		}

		/// <summary>
		/// Gets the value for persistence type taking in to account defaults.
		/// </summary>
		virtual protected PersistenceResetType PersistenceType
		{
			get
			{
				if (usePersistenceDefaults) return PlatformerProGameManager.Instance.defaultPersistenceReset;
				return persistenceType;
			}
		}

		/// <summary>
		/// Gets the value for save on change type taking in to account defaults.
		/// </summary>
		virtual protected bool SaveOnChange
		{
			get
			{
				if (usePersistenceDefaults) return PlatformerProGameManager.Instance.saveOnAnyChange;
				return saveOnAnyChange;
			}
		}

		#endregion

		/// <summary>
		/// Unity Start() hook.
		/// </summary>
		void Start()
		{
			if (EnablePersistence)
			{
				PlatformerProGameManager.Instance.PhaseChanged += PhaseChange;
			}
		}



		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void ConfigureEventListeners ()
		{
			if (EnablePersistence)
			{
				Character.WillExitScene += HandleWillExitScene;
                if (Character.CharacterHealth != null)
                {
                    Character.CharacterHealth.Died += HandleDied;
                    Character.CharacterHealth.GameOver += HandleGameOver;
                }
				LevelManager.Instance.Respawned += HandleRespawn;
			}		
		}

		/// <summary>
		/// Handle a change of game phase, look for persistence event and apply.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		virtual protected void PhaseChange (object sender, GamePhaseEventArgs e)
		{
		}

		/// <summary>
		/// Handle a change of game phase, look for persistence event and apply.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		virtual protected void HandleRespawn (object sender, CharacterEventArgs e)
		{
			if (e.Character == Character)
			{
				Load (this);
			}
		}

		/// <summary>
		/// Handles the will exit scene event
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event.</param>
		virtual protected void HandleWillExitScene (object sender, SceneEventArgs e)
		{
			if ((PersistenceType & PersistenceResetType.RESET_ON_SCENE_EXIT) == PersistenceResetType.RESET_ON_SCENE_EXIT)
			{
				Reset (this);
			}
			else
			{
				Save (this);
			}
		}
		
		/// <summary>
		/// Handles the game over event
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event.</param>
		virtual protected void HandleGameOver (object sender, CharacterEventArgs e)
		{
			if ((PersistenceType & PersistenceResetType.RESET_ON_GAME_OVER) == PersistenceResetType.RESET_ON_GAME_OVER)
			{
				Reset (this);
			}
		}
		
		/// <summary>
		/// Handles the died event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		virtual protected  void HandleDied (object sender, DamageInfoEventArgs e)
		{
			if ((PersistenceType & PersistenceResetType.RESET_ON_DEATH) == PersistenceResetType.RESET_ON_DEATH)
			{
				Reset (this);
			}
			else
			{
				Save (this);
			}
		}

		/// <summary>
		/// Save the given persistable.
		/// </summary>
		/// <param name="p">Persistable to save.</param>
		public static void Save(Persistable p)
		{
			// Debug.Log ("Saving: " + p.GetType().Name);
			object saveData = p.SaveData;
			// We assume all saveData is annotated with [Serializable] we could add a condition for ISeriazable too.
			if (saveData.GetType().IsSerializable)
			{
				using(StringWriter writer = new StringWriter())
				{
					XmlSerializer serializer = new XmlSerializer(p.SavedObjectType(), p.GetExtraTypes());
					serializer.Serialize(writer, saveData);
					PlayerPrefs.SetString(p.PlayerPrefsIdentifier, writer.ToString());
				}
			}
			else
			{
				Debug.LogError("Save data for " + p + " is not serializable.");
			}
		}

		/// <summary>
		/// Support complex object serialisation by passing additional types to seralizer.
		/// </summary>
		virtual public System.Type[] GetExtraTypes() 
		{
			return null;
		}

		/// <summary>
		/// Load the given persistable.
		/// </summary>
		/// <param name="p">Persistable to laod.</param>
		public static void Load(Persistable p)
		{
			// Debug.Log ("Load: " + p.GetType ().Name);
			object savedObject = LoadSavedData (p);
			if (savedObject != null)
			{
				p.ApplySaveData(savedObject);
			}
			else
			{
				p.ResetSaveData();
			}
			p.loaded = true;
		}

		/// <summary>
		/// Does the actual load and returns raw object.
		/// </summary>
		/// <returns>The saved data.</returns>
		/// <param name="p">P.</param>
		protected static object LoadSavedData(Persistable p) {
			string data = PlayerPrefs.GetString(p.PlayerPrefsIdentifier, "");
			if (data.Length > 0)
			{
				using (StringReader reader = new StringReader(data)){
					XmlSerializer serializer = new XmlSerializer(p.SavedObjectType(), p.GetExtraTypes());
					object savedObject = serializer.Deserialize(reader);
					return savedObject;
				}
			}
			return null;
		}

		/// <summary>
		/// Resets the given persistable.
		/// </summary>
		/// <param name="p">Persistable to reset.</param>
		public static void Reset(Persistable p)
		{
			PlayerPrefs.DeleteKey (p.PlayerPrefsIdentifier);
			p.ResetSaveData();
		}

		#region abstract methods

		/// <summary>
		/// Gets the character.
		/// </summary>
		public abstract Character Character { get; set; }

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

#if UNITY_EDITOR

		/// <summary>
		/// Can this persistable object show its saved data?
		/// </summary>
		virtual public bool ShouldShowSavedData
		{
			get 
			{
				return false;
			}
			set 
			{
				if (value) Debug.LogWarning ("This persistable cannot show its saved data in the inspector.");
			}
		}

		/// <summary>
		/// Shows currently saved data.
		/// </summary>
		virtual public void ShowSaveData() {}
#endif

		#endregion
	}

	[System.Flags]
	public enum PersistenceResetType
	{
		RESET_ON_GAME_OVER = 1,
		RESET_ON_DEATH = 2,
		RESET_ON_SCENE_EXIT = 4
	}

	public static class PersistenceTypeEnumExtensions
	{
		public static string GetDescription(this PersistenceResetType me)
		{
			switch(me)
			{
			case PersistenceResetType.RESET_ON_GAME_OVER:
				return "Saved data will be reset after the players game ends.";
			case PersistenceResetType.RESET_ON_DEATH: return "Saved data will be reset after the character dies.";
			case PersistenceResetType.RESET_ON_SCENE_EXIT: return "Saved data will be reset after the character leaves a scene";
			}
			return "No information available.";
		}
	}

}
