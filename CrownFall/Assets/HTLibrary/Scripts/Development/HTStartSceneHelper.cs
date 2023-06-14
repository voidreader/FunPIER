using UnityEngine;
using System.Collections;

namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	/// <summary>
	/// Client가 맨 처음 시작 될 Scene에서 시작되지 않을 경우
	/// 자동으로 해당 Scene으로 SceneChange 호출하는 Behaviour입니다.
	/// </summary>
	public class HTStartSceneHelper : MonoBehaviour
	{
		//---------------------------------------
		static private string _firstSceneName = null;

#if UNITY_EDITOR
		static bool _sceneChanged = false;
#else // UNITY_EDITOR
		static bool _sceneChanged = true;
#endif // UNITY_EDITOR

		//---------------------------------------
		[SerializeField]
		private string _startSceneName;

		//---------------------------------------
		void Awake()
		{
			if (_sceneChanged == false && _startSceneName != null && _startSceneName != string.Empty)
			{
				RecordFirstSceneName();

				if (_firstSceneName == null || _firstSceneName == _startSceneName)
				{
					_sceneChanged = true;
					UnityEngine.SceneManagement.SceneManager.LoadScene(_startSceneName);
				}
			}
			else
			{
				Utils.SafeActive(gameObject, false);
			}
		}

		//---------------------------------------
		public static void RecordFirstSceneName()
		{
			if (_firstSceneName != null || _firstSceneName != string.Empty)
				return;

			_firstSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}
