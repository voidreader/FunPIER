using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	/// <summary>
	/// HTFramework의 초기화를 진행해주는 Behaviour입니다.
	/// Game의 첫 번째 Scene에만 등록되어있으면 됩니다.
	/// </summary>
	public class HTInitializer : MonoBehaviour
	{
		//---------------------------------------
		private static bool _initialized = false;

		//---------------------------------------
		[SerializeField, FormerlySerializedAs("_projectDefine")]
		private GEnv _gEnv = null;

		//---------------------------------------
		void Awake()
		{
			if (_initialized == false)
			{
				_initialized = true;

				HTFramework.Instance.Initialize(false, _gEnv, null, null);
				HTGlobalUI.Instance.MaskFade(true);
			}

			//-----
			gameObject.SetActive(false);
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}