using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro.Validation
{
	/// <summary>
	/// Runs validaiton and shows validation results.
	/// </summary>
	public class ValidationWindow : EditorWindow
	{

		/// <summary>
		/// The current results.
		/// </summary>
		protected List<ValidationResult> currentResults;

		/// <summary>
		/// When did we alst run.
		/// </summary>
		protected System.DateTime lastRunTime;

		/// <summary>
		/// Do the validation.
		/// </summary>
		protected void DoValidate()
		{
			currentResults = new List<ValidationResult> ();
			currentResults.AddRange (new ValidateRaycastColliders ().Validate ());
			currentResults.AddRange (new ValidateLayers ().Validate ());
			currentResults.AddRange (new ValidateRigidbodies ().Validate ());
		}

		/// <summary>
		/// Draw the intro window
		/// </summary>
		void OnGUI ()
		{
			if (currentResults == null)
			{
				EditorGUILayout.HelpBox("Validations not run", MessageType.None);
			}
			else if (currentResults.Count == 0)
			{
				GUI.color = new Color(0,1,0);
				EditorGUILayout.HelpBox("No validation errors found ... but remember only active GameObjects are validated.", MessageType.None);
				GUI.color = Color.white;
			}
			else
			{
				foreach (ValidationResult result in currentResults)
				{
					EditorGUILayout.HelpBox(result.message, result.messageType);
				}
			}
		}

		#region static members and methods

		/// <summary>
		/// Reference to the window
		/// </summary>
		public static ValidationWindow window;
		
#if TOP_LEVEL_MENU
		[MenuItem ("Platformer PRO/Validate Scene")]
#else
		[MenuItem ("Assets/Platformer PRO/Validate Scene")]
#endif
		public static void ShowWindowAndValidate()
		{
			// Lets assume that everyone window is at least 520 x 448
			float windowWidth = 512;
			float windowHeight = 512;
			Rect rect = new Rect((Screen.currentResolution.width - windowWidth) / 2.0f,
			                     (Screen.currentResolution.height - windowHeight) / 2.0f,
			                     windowWidth , windowHeight);
			window = (ValidationWindow) EditorWindow.GetWindowWithRect(typeof(ValidationWindow), rect, true, "Validation Results");
			window.minSize = new Vector2 (windowWidth, windowHeight);
			window.maxSize = new Vector2 (windowWidth, windowHeight);
			window.position = rect;
			window.Show();
			window.DoValidate ();
		}

		#endregion
	}
}
