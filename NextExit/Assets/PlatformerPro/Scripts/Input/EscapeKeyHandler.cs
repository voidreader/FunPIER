using UnityEngine;
using System.Collections;
#if !UNITY_4_6 && !UNITY_4_7 && !UNITY_5_1 && !UNITY_5_2
using UnityEngine.SceneManagement;
#endif
namespace PlatformerPro
{

	/// <summary>
	/// Because ESC very commonly pauses and/or exits it can be convenient to handle this separate to the input system.
	/// This class allows this (but you don't have to add it).
	/// </summary>
	public class EscapeKeyHandler : MonoBehaviour
	{

		/// <summary>
		/// If this is true then escape will pause before exiting. Second press will either exit or unpause depending on 'escape when puased' setting.
		/// </summary>
		[Tooltip ("If this is true then escape will pause before exiting. Second press will either exit or unpause depending on 'escape when puased' setting.")]
		public bool pauseFirst = true;

		/// <summary>
		/// If this is not empty then hitting escape while paused will load the provided scene. 
		/// Otherwise it will unpause.
		/// </summary>
		[Tooltip ("If this is not empty then hitting escape while paused will load the provided scene. Otherwise it will unpause.")]
		public string escapeWhenPausedScene;

		// Update is called once per frame
		void Update () 
		{
			if (enabled)
			{
				if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
				{
					if (pauseFirst && TimeManager.Instance.Paused)
					{
						if (escapeWhenPausedScene == null || escapeWhenPausedScene == "")
						{
							TimeManager.Instance.UnPause(false);
						}
						else
						{
							LevelManager.Instance.LoadScene (escapeWhenPausedScene);
						}
					}
					else if (pauseFirst)
					{
						TimeManager.Instance.Pause(false);
					}
					else if (escapeWhenPausedScene != null && escapeWhenPausedScene != "")
					{
						LevelManager.Instance.LoadScene(escapeWhenPausedScene);

					}
				}
			}
		}
	}
}