using UnityEngine;
#if !UNITY_4_6 && !UNITY_4_7 && !UNITY_5_1 && !UNITY_5_2
using UnityEngine.SceneManagement;
#endif
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// An input button that loads another scene. Mainly for the samples.
	/// </summary>
	public class TouchButtonExit : TouchButton
	{
		/// <summary>
		/// The scene to load.
		/// </summary>
		[Tooltip ("Name of the scene to load")]
		public string sceneToLoad;

		/// <summary>
		/// Called when this  this instance.
		/// </summary>
		override protected void Released()
		{
			LevelManager.Instance.LoadScene(sceneToLoad);
		}

	}
}