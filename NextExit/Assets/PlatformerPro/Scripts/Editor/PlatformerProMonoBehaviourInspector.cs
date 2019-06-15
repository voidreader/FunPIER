using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PlatformerPro
{
	/// <summary>
	/// Base editor for paltformer pro monobehaviours.
	/// </summary>
	[CustomEditor (typeof(PlatformerProMonoBehaviour), true)]
	[CanEditMultipleObjects]
	public class PlatformerProMonoBehaviourInspector : Editor 
	{
		/// <summary>
		/// Holds the platformer pro icon texture
		/// </summary>
		public static Texture2D iconTexture;

		/// <summary>
		/// Unity OnEnable hook.
		/// </summary>
		virtual protected void OnEnable() 
		{
			if (iconTexture == null) iconTexture = Resources.Load <Texture2D> ("Platformer_Icon");
		}

		/// <summary>
		/// Draw the inspector.
		/// </summary>
		public override void OnInspectorGUI()
		{
			DrawHeader((PlatformerProMonoBehaviour) target);
			GUILayout.Space (5);
			DrawDefaultInspector ();
			GUILayout.Space (5);
			DrawFooter((PlatformerProMonoBehaviour) target);
		}

		/// <summary>
		/// Draws the header.
		/// </summary>
		/// <param name="myTarget">My target.</param>
		virtual protected void DrawHeader(PlatformerProMonoBehaviour myTarget)
		{
			GUILayout.BeginHorizontal ();
			if (iconTexture == null) iconTexture = Resources.Load <Texture2D> ("Platformer_Icon");
			if (GUILayout.Button (iconTexture, GUILayout.Width (48), GUILayout.Height (48)))
			{
				PlatformerProWelcomePopUp.ShowWelcomeScreen ();
			}
			GUILayout.BeginVertical ();
			if (myTarget.Header != null)
			{
				EditorGUILayout.HelpBox (myTarget.Header, MessageType.None);
			} 

			GUILayout.BeginHorizontal ();
			if (myTarget.DocLink != null)
			{
				if (GUILayout.Button ("Go to Doc", EditorStyles.miniButton))
				{
					Application.OpenURL (myTarget.DocLink);
				}
			}
			if (myTarget.VideoLink != null)
			{
				if (GUILayout.Button ("Go to Video", EditorStyles.miniButton))
				{
					Application.OpenURL(myTarget.VideoLink);
				}
			}
			GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();
			GUILayout.EndHorizontal ();

		}

		/// <summary>
		/// Draws the footer.
		/// </summary>
		virtual protected void DrawFooter(PlatformerProMonoBehaviour myTarget)
		{
			myTarget.Validate(myTarget);
		}

	}
}