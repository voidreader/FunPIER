#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml.Serialization;

namespace PlatformerPro
{
	/// <summary>
	/// Stores user settings. 
	/// </summary>
	[System.Serializable]
	public class PlatformerProSettings
	{
		#region serialised fields

		/// <summary>
		/// Should we show the welcome screen on startup?
		/// </summary>
		public bool showTipOnStartUp = true;

		/// <summary>
		/// Should we show the welcome screen on startup?
		/// </summary>
		public bool supressDamageWarning = false;

		#endregion

		public const string RelativeDataPath = "PlatformerProSettings.xml";

		public static PlatformerProSettings instance;

		/// <summary>
		/// Gets the current settings or loads them if null.
		/// </summary>
		/// <value>The instance.</value>
		public static PlatformerProSettings Instance {
			get
			{
				if (instance == null) Load();
				return instance;
			}
		}

		/// <summary>
		/// Load the data.
		/// </summary>
		protected static void Load()
		{
			try 
			{
				using (StreamReader reader = new StreamReader(Application.dataPath + Path.DirectorySeparatorChar + RelativeDataPath))
				{
					XmlSerializer serializer = new XmlSerializer (typeof(PlatformerProSettings));
					instance = (PlatformerProSettings)serializer.Deserialize (reader);
				}
			}
			catch (IOException)
			{
				instance = new PlatformerProSettings();
			}
		}

		/// <summary>
		/// Save the data.
		/// </summary>
		public static void Save()
		{
			if (instance != null)
			{
				using (StreamWriter writer = new StreamWriter(Application.dataPath + Path.DirectorySeparatorChar + RelativeDataPath))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(PlatformerProSettings));
					serializer.Serialize(writer, instance);
				}
			}
		}
	}
}
#endif