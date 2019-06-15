using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace PlatformerPro
{
	/// <summary>
	/// Static class for managing persistable objects.
	/// </summary>
	public class PersistableObjectManager : PlatformerProMonoBehaviour
	{
		/// <summary>
		/// Should we save data on change.
		/// </summary>
		protected bool saveOnChange;

		/// <summary>
		/// The persistence data.
		/// </summary>
		protected Dictionary<string, PersistableObjectData> objectData;

		/// <summary>
		/// Used to ensure that the will exit scene save happens at end of frame after all other objects have set their state.
		/// </summary>
		protected bool doLateSave;


		/// <summary>
		/// Stores all pref kyes so we can reset data.
		/// </summary>
		protected static List<string> allPrefsIdentifiers = new List<string> ();

		/// <summary>
		/// Unity LateUpdate hook.
		/// </summary>
		void LateUpdate()
		{
			if (doLateSave)
			{
				doLateSave = false;
				Save ();
			}
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		protected void Init()
		{
			objectData = new Dictionary<string, PersistableObjectData> ();
			Load ();
			InitEvents ();
		}
			
		/// <summary>
		/// Unity Destory hook.
		/// </summary>
		void OnDestroy()
		{
			if (PlatformerProGameManager.Instance != null)
			{
				PlatformerProGameManager.Instance.PhaseChanged -= HandlePhaseChanged;
			}
		}
		/// <summary>
		/// Handles the phase changed.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void HandlePhaseChanged (object sender, GamePhaseEventArgs e)
		{
			if (e.Phase == GamePhase.GAME_OVER)
			{
				ResetAll ();
			}
			else if (e.Phase == GamePhase.READY)
			{
				LevelManager.Instance.WillExitScene += HandleWillExitScene;
			}
		}

		/// <summary>
		/// Gets the state for the object with the given guid. This is not a copy!
		/// </summary>
		/// <param name="guid">GUID.</param>
		/// <param name="defaultStateIsDisabled">If true the object starts disabled.</param>
		public PersistableObjectData GetState(string guid, bool defaultStateIsDisabled)
		{
			if (objectData.ContainsKey (guid))
			{
				return objectData [guid];
			}
			else
			{
				PersistableObjectData data = new PersistableObjectData ();
				data.guid = guid;
				data.state = !defaultStateIsDisabled;
				objectData.Add (guid, data);
				return data;
			}
		}

		/// <summary>
		/// Sets the state for the object with the given guid.
		/// </summary>
		/// <param name="guid">GUID.</param>
		/// <param name="state">State to set.</param>
		/// <param name="extraInfo">Extra info.</param>
		public void SetState(string guid, bool state, string extraInfo)
		{
			if (objectData.ContainsKey (guid))
			{
				objectData [guid].state = state;
				objectData [guid].extraStateInfo = extraInfo;
			}
			else
			{
				PersistableObjectData data = new PersistableObjectData ();
				data.guid = guid;
				data.state = state;
				data.extraStateInfo = extraInfo;
				objectData.Add (guid, data);
			}
			if (saveOnChange) Save ();
		}

		/// <summary>
		/// Updates persistable object state.
		/// </summary>
		public void Save()
		{
			// Debug.Log ("Saving persistable objects");
			using(StringWriter writer = new StringWriter())
			{
				XmlSerializer serializer = new XmlSerializer(typeof(List<PersistableObjectData>));
				serializer.Serialize(writer, GetSaveData());
				PlayerPrefs.SetString(PlayerPrefsIdentifier, writer.ToString());
			}
			if (!allPrefsIdentifiers.Contains (PlayerPrefsIdentifier))
			{
				
				allPrefsIdentifiers.Add (PlayerPrefsIdentifier);
				using (StringWriter writer = new StringWriter ())
				{
					XmlSerializer serializer = new XmlSerializer (typeof(List<string>));
					serializer.Serialize (writer, allPrefsIdentifiers);
					PlayerPrefs.SetString (UniqueDataIdentifier, writer.ToString ());
				}
			}
		}

		/// <summary>
		/// Reset persistable object state.
		/// </summary>
		public void ResetAll()
		{
			foreach (string id in allPrefsIdentifiers)
			{
				PlayerPrefs.SetString(id, "");
			}
		}

		/// <summary>
		/// Reset persistable object state.
		/// </summary>
		public void ResetCurrentlevel()
		{
			Debug.Log ("Resetting current levels persistable objects");
			PlayerPrefs.SetString(PlayerPrefsIdentifier, "");
		}

		/// <summary>
		/// Load the saved data from prefs.
		/// </summary>
		protected void Load()
		{
			string identifiers = PlayerPrefs.GetString (UniqueDataIdentifier, "");
			if (allPrefsIdentifiers.Count == 0 && identifiers.Length > 0)
			{
				// Debug.Log ("Loading all prefs list");
				using (StringReader reader = new StringReader(identifiers)){
					XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
					allPrefsIdentifiers = (List<string>) serializer.Deserialize(reader);
				}
			}
			// Debug.Log ("Loading persistable objects");
			string data = PlayerPrefs.GetString(PlayerPrefsIdentifier, "");
			if (data.Length > 0)
			{
				List<PersistableObjectData> saveData;
				using (StringReader reader = new StringReader(data)){
					XmlSerializer serializer = new XmlSerializer(typeof(List<PersistableObjectData>));
					saveData = (List<PersistableObjectData>) serializer.Deserialize(reader);
					foreach (PersistableObjectData p in saveData)
					{
						objectData.Add (p.guid, p);
					}
				}
			}
		}

		/// <summary>
		/// Find references and initialise all the event listeners..
		/// </summary>
		void InitEvents()
		{
			PlatformerProGameManager.Instance.PhaseChanged += HandlePhaseChanged;
		}

		/// <summary>
		/// Hanlde scene exit by saving
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void HandleWillExitScene (object sender, SceneEventArgs e)
		{
			// Defer save to end of frame
			doLateSave = true;
		}

		/// <summary>
		/// Handles the game ending.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event data.</param>
		virtual public void HandleGameOver (object sender, DamageInfoEventArgs e)
		{
			ResetAll ();
		}

		/// <summary>
		/// Convert dictionary into savable list.
		/// </summary>
		/// <returns>The save data.</returns>
		protected List<PersistableObjectData> GetSaveData()
		{
			return objectData.Values.ToList ();
		}

		#region static methods

		/// <summary>
		/// The player preference identifier.
		/// </summary>
		public const string UniqueDataIdentifier = "PersistableObjectManagerData";

		/// <summary>
		/// The player preference identifier.
		/// </summary>
		virtual public string PlayerPrefsIdentifier
		{
			get
			{
				string levelName = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name;
				return string.Format("{0}_{1}", UniqueDataIdentifier, levelName);
			}
		}

		/// <summary>
		/// Creates a new time manager.
		/// </summary>
		public static PersistableObjectManager CreateNewPersistableObjectManager(bool saveOnChange)
		{
			GameObject go = new GameObject ();
			go.name = "PersistableObjectManager";
			go.hideFlags = HideFlags.HideInHierarchy;
			PersistableObjectManager instance = go.AddComponent<PersistableObjectManager> ();
			instance.Init ();
			instance.saveOnChange = saveOnChange;
			return instance;
		}

		#endregion

	}
}
