#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if !UNITY_4_6 && !UNITY_5_1 && !UNITY_5_2
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
#endif

namespace PlatformerPro
{
	/// <summary>
	/// Shows details of samples, currently disabled.
	/// </summary>
	[InitializeOnLoad]
	public class SampleInfoPopUp : EditorWindow
	{
		#region static variables
		
		/// <summary>
		/// Reference to the window
		/// </summary>
		public static SampleInfoPopUp window;

		/// <summary>
		/// The width of the sample select image.
		/// </summary>
		protected static int imageWidth = 928;

		/// <summary>
		/// The height of the sample select image.
		/// </summary>
		protected static int imageHeight = 181;

		/// <summary>
		/// Style for heading.
		/// </summary>
		protected static GUIStyle headingStyle;

		/// <summary>
		/// Style for text summary.
		/// </summary>
		protected static GUIStyle textStyle;

		/// <summary>
		/// Style for dot points.
		/// </summary>
		/// 
		protected static GUIStyle dotPointStyle;

		/// <summary>
		/// Style for sections.
		/// </summary>
		protected static GUIStyle sectionStyle;

		#endregion
		
		/// <summary>
		/// Holds the platformer pro intro texture
		/// </summary>
		protected Texture2D currentTexture;

		/// <summary>
		/// Holds the platformer pro intro texture
		/// </summary>
		protected string currentText;

		/// <summary>
		/// Points to currently selected scene.
		/// </summary>
		protected int selectedItemIndex;

		/// <summary>
		/// Cached copy of available textures.
		/// </summary>
		protected Texture2D[] loadedTextures;

	
#if TOP_LEVEL_MENU
		//[MenuItem ("Platformer PRO/Sample Browser")]
#else
		//[MenuItem ("Assets/Platformer PRO/Sample Browser")]
#endif
		public static void ShowSampleBrowser()
		{
			// Set up styles
			headingStyle = new GUIStyle ();
			headingStyle.fontSize = 32;
			headingStyle.alignment = TextAnchor.MiddleCenter;
			headingStyle.normal.textColor = Color.white;
			textStyle = new GUIStyle ();
			textStyle.alignment = TextAnchor.MiddleCenter;
			textStyle.fontStyle = FontStyle.Italic;
			textStyle.normal.textColor = Color.white;
			textStyle.wordWrap = true;
			dotPointStyle = new GUIStyle ();
			dotPointStyle.alignment = TextAnchor.MiddleCenter;
			dotPointStyle.normal.textColor = Color.white;
			dotPointStyle.wordWrap = true;
			sectionStyle = new GUIStyle ();
			sectionStyle.alignment = TextAnchor.MiddleCenter;
			sectionStyle.fontStyle = FontStyle.Bold;
			sectionStyle.normal.textColor = Color.white;


			// Lets assume that everyone can fit this on their screen
			float windowWidth = Mathf.Min (Screen.currentResolution.width - 64.0f, (imageWidth / 4.0f) + 640.0f);
			float windowHeight = Mathf.Min (Screen.currentResolution.height - 64.0f, 640.0f);
			Rect rect = new Rect((Screen.currentResolution.width - windowWidth) / 2.0f,
			                     (Screen.currentResolution.height - windowHeight) / 2.0f,
			                     windowWidth , windowHeight);
			window = (SampleInfoPopUp) EditorWindow.GetWindowWithRect(typeof(SampleInfoPopUp), rect, true, "Sample Browser");
			window.minSize = new Vector2 (windowWidth, windowHeight);
			window.maxSize = new Vector2 (windowWidth, windowHeight);
			window.position = rect;

			// Preload textures
			List<SampleSceneData> sceneData = SampleSceneData.Samples;
			if (sceneData != null)
			{
				window.loadedTextures = new Texture2D[sceneData.Count];
				for (int i = 0; i < sceneData.Count; i++)
				{
					window.loadedTextures[i] = Resources.Load <Texture2D> (sceneData[i].texturePath);
				}
			}


			window.Show();

		}

		/// <summary>
		/// Draw the intro window
		/// </summary>
		void OnGUI ()
		{
			List<SampleSceneData> sceneData = SampleSceneData.Samples;

			if (sceneData == null)
			{
				GUILayout.Space(10);
				GUILayout.Label ("Unable to load sample data", headingStyle);
				GUILayout.Space(10);
				GUILayout.Label("You probaby haven't extracted the samples!", sectionStyle);
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.Button("Extract Samples Now");
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.FlexibleSpace(); 
			}
			else
			{
				Rect iconArea = new Rect(8, 8, imageWidth * 0.5f, window.maxSize.y - 16 );
				Rect textArea = new Rect(iconArea.xMax + 4, 8, window.maxSize.x -  (imageWidth * 0.5f) - 20.0f, window.maxSize.y - 16);
				GUI.Box(new Rect(iconArea.xMax + 2, 0, 1, window.maxSize.y),"");
				// Selector box
				GUILayout.BeginArea (iconArea);
				for (int i = 0; i < sceneData.Count; i++)
				{
					if (selectedItemIndex == i)
					{
						GUI.color = new Color(1,1,1,1);
						GUILayout.Box (loadedTextures[i], GUIStyle.none, GUILayout.Width(imageWidth * 0.5f), GUILayout.Height((imageHeight * 0.5f) + 1.0f));
					}
					else
					{
						GUI.color = new Color(1,1,1,0.5f);
						if (GUILayout.Button (loadedTextures[i], GUIStyle.none, GUILayout.Width(imageWidth * 0.5f), GUILayout.Height((imageHeight * 0.5f) + 1.0f)))
						{
							selectedItemIndex = i;
						}
					}
				}
				GUILayout.EndArea();
				GUI.color = new Color(1,1,1,1);

				// Text info
				GUILayout.BeginArea (textArea);
				GUILayout.TextArea(sceneData[selectedItemIndex].title, headingStyle);
				GUILayout.Space(10);
				GUILayout.Label(sceneData[selectedItemIndex].text, textStyle);
				GUILayout.Space(30);
				GUILayout.Label ("Key Features", sectionStyle);
				GUILayout.Space(10);
				for (int i = 0; i < sceneData[selectedItemIndex].keyFeatures.Length; i++)
				{
					GUILayout.Label (sceneData[selectedItemIndex].keyFeatures[i], dotPointStyle);
					GUILayout.Space(10);
				}
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Open Scene"))
				{
					#if !UNITY_4_6 && !UNITY_5_1 && !UNITY_5_2
					EditorSceneManager.OpenScene(sceneData[selectedItemIndex].scenePath + ".unity");
					#else
					EditorApplication.OpenScene(sceneData[selectedItemIndex].scenePath + ".unity");
					#endif
				}
				GUILayout.EndArea();

			}
		}
	}
}

#endif