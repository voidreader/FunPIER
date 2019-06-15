using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml.Serialization;

namespace PlatformerPro
{
	/// <summary>
	/// Handles shared audio settings like volume.
	/// </summary>
	public class AudioManager : MonoBehaviour
	{

		/// <summary>
		/// Player pref key.
		/// </summary>
		const string PLAYER_PREF_NAME = "PP_AUDIO_DATA_4792";

		/// <summary>
		/// Music volume to use if no data is saved.
		/// </summary>
		const float DEFAULT_MUSIC_VOLUME = 0.33f;

		/// <summary>
		/// Sfx volume to use if no data is saved.
		/// </summary>
		const float DEFAULT_SFX_VOLUME = 1.0f;

		/// <summary>
		/// The settings data.
		/// </summary>
		protected AudioManagerData data;

		/// <summary>
		/// Gets or sets the sfx volume.
		/// </summary>
		public float SfxVolume {
			get { return data.sfxVolume;}
			set {
				if (value > 1.0f) value = 1.0f;
				if (value < 0) value = 0;
				data.sfxVolume = value;
				Save();
				foreach (SoundEffect se in GameObject.FindObjectsOfType<SoundEffect>())
				{
					se.UpdateVolume();
				}
			}
		}

		/// <summary>
		/// Gets or sets the music volume.
		/// </summary>
		public float MusicVolume {
			get { return data.musicVolume;}
			set
			{
				if (value > 1.0f) value = 1.0f;
				if (value < 0) value = 0;
				data.musicVolume = value;
				Save();
				foreach (MusicPlayer mp in GameObject.FindObjectsOfType<MusicPlayer>())
				{
					mp.UpdateVolume();
				}
			}
		}

		/// <summary>
		/// Save the audio settings.
		/// </summary>
		virtual protected void Save()
		{
			using(StringWriter writer = new StringWriter())
			{
				XmlSerializer serializer = new XmlSerializer(typeof(AudioManagerData));
				serializer.Serialize(writer, data);
				PlayerPrefs.SetString(PLAYER_PREF_NAME, writer.ToString());
			}
		}

		/// <summary>
		/// Load the audio settings.
		/// </summary>
		virtual protected void Load()
		{
			string stringData = PlayerPrefs.GetString(PLAYER_PREF_NAME, "");
			if (stringData.Length > 0)
			{
				using (StringReader reader = new StringReader(stringData)){
					XmlSerializer serializer = new XmlSerializer(typeof(AudioManagerData));
					data = (AudioManagerData) serializer.Deserialize(reader);
				}
			}
			else
			{
				data = new AudioManagerData();
				data.musicVolume = DEFAULT_MUSIC_VOLUME;
				data.sfxVolume = DEFAULT_SFX_VOLUME;
			}
		}

		#region static methods and members

		/// <summary>
		/// The singleton instance.
		/// </summary>
		protected static AudioManager instance;

		/// <summary>
		/// Get the instance
		/// </summary>
		public static AudioManager Instance
		{
			get
			{
				if (instance == null) CreateNewAudioManager();
				return instance;
			}
		}

		/// <summary>
		/// Creates the audio manager.
		/// </summary>
		protected static void CreateNewAudioManager()
		{
			GameObject go = new GameObject ();
			go.name = "AudioManager";
			instance = go.AddComponent<AudioManager> ();
			instance.Load ();
			DontDestroyOnLoad (go);
		}

		#endregion
	}

	/// <summary>
	/// Audio manager sound data
	/// </summary>
	[System.Serializable]
	public class AudioManagerData
	{
		/// <summary>
		/// The volume of the sound effects. 
		/// </summary>
		public float sfxVolume;
		
		/// <summary>
		/// The volume of the sound music. 
		/// </summary>
		public float musicVolume;

		/// <summary>
		/// Are we muting all sounds regardless of volume?
		/// </summary>
		public bool muted;
	}

}