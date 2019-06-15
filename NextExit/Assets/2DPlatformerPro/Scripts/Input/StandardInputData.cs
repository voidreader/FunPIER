using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml.Serialization;

namespace PlatformerPro
{

	[System.Serializable]
	public class StandardInputData {

		/// <summary>
		/// Should the controller input be enabled.
		/// </summary>
		public bool enableController;

		/// <summary>
		/// Should the keyboard input be enabled.
		/// </summary>
		public bool enableKeyboard;

		/// <summary>
		/// The horizontal joystick axis.
		/// </summary>
		public string horizontalAxisName = "Joystick1Axis1";

		/// <summary>
		/// Should we reverse the values of the horizontal axis.
		/// </summary>
		public bool reverseHorizontalAxis;

		/// <summary>
		/// Threshold for digital input to be considered non-zero.
		/// </summary>
		public float digitalHorizontalThreshold = 0.25f;
		
		/// <summary>
		/// The vertical joystick axis.
		/// </summary>
		public string verticalAxisName = "Joystick1Axis2";
	
		/// <summary>
		/// Should we reverse the values of the vertival axis.
		/// </summary>
		public bool reverseVerticalAxis;

		/// <summary>
		/// Threshold for digital input to be considered non-zero.
		/// </summary>
		public float digitalVerticalThreshold = 0.25f;

		/// <summary>
		/// The alternateHorizontal joystick axis.
		/// </summary>
		public string altHorizontalAxisName = "Joystick1Axis7";
		
		/// <summary>
		/// Should we reverse the values of the horizontal axis.
		/// </summary>
		public bool reverseAltHorizontalAxis;
		
		/// <summary>
		/// Threshold for digital input to be considered non-zero.
		/// </summary>
		public float digitalAltHorizontalThreshold = 0.25f;
		
		/// <summary>
		/// The alternate vertical joystick axis.
		/// </summary>
		public string altVerticalAxisName = "Joystick1Axis8";
		
		/// <summary>
		/// Should we reverse the values of the vertival axis.
		/// </summary>
		public bool reverseAltVerticalAxis;
		
		/// <summary>
		/// Threshold for digital input to be considered non-zero.
		/// </summary>
		public float digitalAltVerticalThreshold = 0.25f;

		/// <summary>
		/// The right key.
		/// </summary>
		public KeyCode right = KeyCode.RightArrow;
		
		/// <summary>
		/// The left key.
		/// </summary>
		public KeyCode left = KeyCode.LeftArrow;
		
		/// <summary>
		/// The up key.
		/// </summary>
		public KeyCode up = KeyCode.UpArrow;
		
		/// <summary>
		/// The down key.
		/// </summary>
		public KeyCode down = KeyCode.DownArrow;
		
		/// <summary>
		/// The jump key.
		/// </summary>
		public KeyCode jump = KeyCode.Z;
		
		/// <summary>
		/// The run key.
		/// </summary>
		public KeyCode run = KeyCode.X;

		/// <summary>
		/// The pause key.
		/// </summary>
		public KeyCode pause = KeyCode.P;

		/// <summary>
		///  The action buttons with the first value in the array being the default.
		/// </summary>
		public KeyCode[] actionButtons;

		/// <summary>
		/// Loads input data from Unity resource folder
		/// </summary>
		/// <returns>The loaded data or null if data not loaded.</returns>
		/// <param name="resourceName">Resource name.</param>
		public static StandardInputData LoadFromResource(string resourceName)
		{
			TextAsset asset = Resources.Load(resourceName) as TextAsset;
			if (asset != null)
			{
				using (Stream stream = new MemoryStream(asset.bytes)){
					XmlSerializer serializer = new XmlSerializer(typeof(StandardInputData));
					StandardInputData inputData = (StandardInputData) serializer.Deserialize(stream);
					return inputData;
				}
			}
			else
			{
				Debug.LogError ("Tried to load an input file but the resource named " + resourceName + " was not found:");
			}
			return null;
		}

		/// <summary>
		/// Loads input data from file.
		/// </summary>
		/// <returns>The loaded data or null if data not loaded.</returns>
		/// <param name="fullPath">Full path.</param>
		public static StandardInputData LoadFromFile(string fullPath)
		{
			if (fullPath.Length != 0) {
				using (StreamReader reader = new StreamReader(fullPath))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(StandardInputData));
					StandardInputData inputData = (StandardInputData) serializer.Deserialize(reader);
					return inputData;
				}
			}
			else
			{
				Debug.LogError ("Tried to load an input file but the file path was empty");
			}
			return null;
		}

		public static void SaveToFile(string fullPath, StandardInputData data)
		{
			if (fullPath.Length != 0) {
				using (StreamWriter writer = new StreamWriter(fullPath))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(StandardInputData));
					serializer.Serialize(writer, data);
				}
			}
			else
			{
				Debug.LogError ("Tried to save an input file but no input path was specified.");
			}
		}

	}
}