#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections;


namespace PlatformerPro
{
	[InitializeOnLoad]
	public class PlatformerProWelcomePopUp : EditorWindow
	{
		#region static variables

		/// <summary>
		/// Reference to the window
		/// </summary>
		public static PlatformerProWelcomePopUp window;

		/// <summary>
		/// The URL for the documentation.
		/// </summary>
		const string DOC_URL = "https://jnamobile.zendesk.com/hc/en-gb/categories/200246030-Platformer-PRO-Documentation";

		/// <summary>
		/// The URL for the tutorial videos.
		/// </summary>
		const string TUTORIAL_URL = "https://www.youtube.com/playlist?list=PLbnzW2Y4qytLJINKyXDtj2f4MJFsYutNP";

		/// <summary>
		/// The URL for submitting a support request.
		/// </summary>
		const string SUPPORT_URL = "https://jnamobile.zendesk.com/hc/en-gb/requests/new";

		/// <summary>
		/// The version as a string.
		/// </summary>
		const string VERSION = "v1.1.3 LE";

		/// <summary>
		/// Cache show on startup state.
		/// </summary>
		protected static bool alreadyShown = false;

		/// <summary>
		/// The width of the welcome image.
		/// </summary>
		protected static int imageWidth = 512;

		#endregion

		/// <summary>
		/// Holds the platformer pro intro texture
		/// </summary>
		protected Texture2D popUpTexture;

		static PlatformerProWelcomePopUp ()
		{ 
			if (!EditorApplication.isPlayingOrWillChangePlaymode)
			{
				EditorApplication.update += CheckShowWelcome;
			}
		}
#if TOP_LEVEL_MENU
		[MenuItem ("Platformer PRO/Show Welcome")]
#else
		[MenuItem ("Assets/Platformer PRO/Show Welcome")]
#endif
		public static void ShowWelcomeScreen()
		{
			// Lets assume that everyone window is at least 520 x 448
			float windowWidth = imageWidth + 8;
			float windowHeight = (imageWidth * 0.75f) + 64;
			Rect rect = new Rect((Screen.currentResolution.width - windowWidth) / 2.0f,
								 (Screen.currentResolution.height - windowHeight) / 2.0f,
								 windowWidth , windowHeight);
			window = (PlatformerProWelcomePopUp) EditorWindow.GetWindowWithRect(typeof(PlatformerProWelcomePopUp), rect, true, "Welcome to Platformer PRO - Lite Edition");
			window.minSize = new Vector2 (windowWidth, windowHeight);
			window.maxSize = new Vector2 (windowWidth, windowHeight);
			window.position = rect;
			window.Show();
			alreadyShown = true;
		}

		/// <summary>
		/// Check if we should show the welcome screen and show it if we should.
		/// </summary>
		protected static void CheckShowWelcome()
		{
			EditorApplication.update -= CheckShowWelcome;
			// When we compile we lose out static settings this is just a simple workaround to avoid annoying user with constant pop-ups
			if (Time.realtimeSinceStartup > 3) alreadyShown = true;
			if (!alreadyShown && PlatformerProSettings.Instance.showTipOnStartUp) 
			{

				if (!EditorApplication.isPlayingOrWillChangePlaymode)
				{
					ShowWelcomeScreen();
				}
			}
		}

		/// <summary>
		/// Draw the intro window
		/// </summary>
		void OnGUI ()
		{
			if (popUpTexture == null) popUpTexture = Resources.Load <Texture2D> ("Platformer_TitleScreen");
			GUILayout.Box (popUpTexture, GUILayout.Width(imageWidth), GUILayout.Height(imageWidth * 0.75f));

			bool showOnStartUp = GUILayout.Toggle (PlatformerProSettings.Instance.showTipOnStartUp, "Show this window on project open?");
			if (PlatformerProSettings.Instance.showTipOnStartUp != showOnStartUp)
			{
				PlatformerProSettings.Instance.showTipOnStartUp = showOnStartUp;
				PlatformerProSettings.Save();
			}
			GUILayout.FlexibleSpace ();
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Documentation")) Application.OpenURL (DOC_URL);
			if (GUILayout.Button ("Tutorials")) Application.OpenURL (TUTORIAL_URL);
			if (GUILayout.Button ("Support Request")) Application.OpenURL (SUPPORT_URL);
			GUILayout.EndHorizontal ();


			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			GUILayout.Label (VERSION);
			GUILayout.EndHorizontal ();
			GUILayout.FlexibleSpace ();

		}
	}

//	[MenuItem ("PlatformerPRO/ExportAll")]
//	static void DoSomethingWithAShortcutKey () {
//		AssetDatabase.ExportPackage("Assets/", "PlatformerPRO.unitypackage", ExportPackageOptions.Recurse | ExportPackageOptions.IncludeLibraryAssets );
//	}
}

#endif