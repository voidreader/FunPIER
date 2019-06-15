#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace PlatformerPro
{
	/// <summary>
	/// Stores data about samples to drive the sample browser.
	/// </summary>
	[System.Serializable] 
	public class SampleSceneData
	{
		#region serialised fields 
		
		public string scenePath;
		public string texturePath;
		public string title;
		public string text;
		public string[] keyFeatures;

		#endregion
		
		public const string RelativeDataPath = "SampleSceneData.xml";
		
		protected static List<SampleSceneData> samples;
		
		/// <summary>
		/// Gets the current settings or loads them if null.
		/// </summary>
		/// <value>The instance.</value>
		public static  List<SampleSceneData> Samples
		{
			get
			{
				if (samples == null) Load();
				return samples;
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
					XmlSerializer serializer = new XmlSerializer (typeof(List<SampleSceneData>));
					samples = (List<SampleSceneData>)serializer.Deserialize (reader);
				}
			}
			catch (IOException)
			{
				samples = null;
			}
		}
		
		/// <summary>
		/// Save the data.
		/// </summary>
		public static void Save()
		{

			if (samples != null)
			{
				using (StreamWriter writer = new StreamWriter(Application.dataPath + Path.DirectorySeparatorChar + RelativeDataPath))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(List<SampleSceneData>));
					serializer.Serialize(writer, samples);
				}
			}
		}
	}
}
#endif