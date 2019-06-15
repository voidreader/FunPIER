using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace PlatformerPro
{
	public class PlatformerProTester : EditorWindow
	{
		/// <summary>
		/// Reference to the window
		/// </summary>
		public static PlatformerProTester window;

		/// <summary>
		/// Holds the platformer pro icon texture
		/// </summary>
		public static Texture2D iconTexture;

		const float defaultWindowHeight = 600.0f;
		const float defaultWindowWidth = 600.0f;

		int currentPlayerId;
		int currentCharacterId;
		int newPlayerId;
		int characterMode = 0;
        string currentItemId;

        int[] availablePlayerIds = new int[]{0,1,2,3,4,5,6,7,8};
		string[] availablePlayerIdStrings = new string[]{"0","1","2","3","4","5","6","7","8"};
		string[] availableCharacters = new string[]{"Not Loaded"};

		bool isPlaying;

		/// <summary>
		/// Unity OnEnable hook.
		/// </summary>
		void OnEnable() 
		{
			if (iconTexture == null) iconTexture = Resources.Load <Texture2D> ("Platformer_Icon");
		}

		void Update()
		{
			if (!isPlaying && Application.isPlaying)
			{
				isPlaying = true;
				if (PlatformerProGameManager.Instance != null)
				{
					availableCharacters = PlatformerProGameManager.Instance.availableCharacters.Select (c => c.characterId).ToArray ();
				}
			}
			if (isPlaying && !Application.isPlaying)
			{
				isPlaying = false;
			}
		}

		#if TOP_LEVEL_MENU
		[MenuItem ("Platformer PRO/Open Test Window")]
		#else
		[MenuItem ("Assets/Platformer PRO/Show Test Harness")]
		#endif

		public static void ShowTester()
		{
			Rect rect = new Rect((Screen.currentResolution.width - defaultWindowWidth) / 2.0f,
				(Screen.currentResolution.height - defaultWindowHeight) / 2.0f,
				defaultWindowWidth , defaultWindowHeight);
			window = (PlatformerProTester) EditorWindow.GetWindowWithRect(typeof(PlatformerProTester), rect, false, "Test Harness");
			window.position = rect;
			window.minSize = new Vector2 (1, 1);
			window.Show();
		}

		/// <summary>
		/// Draw the intro window
		/// </summary>
		void OnGUI ()
		{
			DrawHeader ();
			if (Application.isPlaying)
			{
				GUI.enabled = true;
			}
			else
			{
				EditorGUILayout.HelpBox ("Enter play mode to start testing.", MessageType.Info);
				GUI.enabled = false;
			}
			DrawCharacterTester ();
            DrawItemTester();
		}

		/// <summary>
		/// Draws the header.
		/// </summary>
		/// <param name="myTarget">My target.</param>
		protected void DrawHeader()
		{
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button (iconTexture, GUILayout.Width (48), GUILayout.Height (48)))
			{
				PlatformerProWelcomePopUp.ShowWelcomeScreen ();
			}

			EditorGUILayout.HelpBox ("Use the test harness to test features of your game in play mode.", MessageType.None);

			GUILayout.EndHorizontal ();

		}

		/// <summary>
		/// Draws the character testers.
		/// </summary>
		protected void DrawCharacterTester()
		{
			GUILayout.Label ("Characters", EditorStyles.boldLabel);

			GUILayout.BeginHorizontal ();
			if (characterMode == 0) GUI.enabled = false;
			if (GUILayout.Button ("Spawn New Character", EditorStyles.miniButtonLeft))
			{
				characterMode = 0;
			}
			if (Application.isPlaying) GUI.enabled = true;
			if (characterMode == 1) GUI.enabled = false;
			if (GUILayout.Button ("Replace Character", EditorStyles.miniButtonMid))
			{
				characterMode = 1;	
			}
			if (Application.isPlaying) GUI.enabled = true;
			if (characterMode == 2) GUI.enabled = false;
			if (GUILayout.Button ("Switch Character", EditorStyles.miniButtonRight))
			{
				characterMode = 2;
			}
			GUILayout.EndHorizontal();
			if (Application.isPlaying) GUI.enabled = true;

			currentPlayerId = EditorGUILayout.IntPopup("Player ID", currentPlayerId, availablePlayerIdStrings, availablePlayerIds);
			currentCharacterId = EditorGUILayout.Popup("Character ID", currentCharacterId, availableCharacters);
			if (characterMode == 2)
			{
				newPlayerId = EditorGUILayout.IntPopup("Player ID", newPlayerId, availablePlayerIdStrings, availablePlayerIds);
			}
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			if (GUILayout.Button ("Go"))
			{
				switch (characterMode)
				{
				case 0:
					PlatformerProGameManager.Instance.SpawnNewCharacter (currentPlayerId, availableCharacters [currentCharacterId]);
					break;
				case 1:
					PlatformerProGameManager.Instance.ReplaceCharacter (currentPlayerId, availableCharacters [currentCharacterId]);
					break;
				case 2:
					PlatformerProGameManager.Instance.SwitchCharacter (currentPlayerId, newPlayerId);
					break;
				
				}
			}
			GUILayout.EndHorizontal ();
		}

        /// <summary>
        /// Draws the item testers.
        /// </summary>
        protected void DrawItemTester()
        {
            GUILayout.Label("Items", EditorStyles.boldLabel);
            currentItemId = ItemTypeAttributeDrawer.DrawItemTypeSelectorLayout(currentItemId, new GUIContent("Item Type", "Item type"));

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Spawn Item", EditorStyles.miniButton))
            {

            }
            GUILayout.EndHorizontal();
        }
    }
}
