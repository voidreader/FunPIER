using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	public sealed class HTGPUPrecacheManager : MonoBehaviour
	{
		//---------------------------------------
		private static HTGPUPrecacheManager _instance = null;
		public static HTGPUPrecacheManager Instance
		{
			get
			{
				if (_instance == null)
				{
					string szPrefabAddr = GEnv.HTPrefabFolder + GEnv.Prefab_GPUPrecache;
					GameObject pObject = Utils.Instantiate(szPrefabAddr) as GameObject;
					if (pObject != null)
						_instance = pObject.GetComponent<HTGPUPrecacheManager>();
				}

				return _instance;
			}
		}

		public static HTGPUPrecacheManager Instanced
		{
			get { return _instance; }
		}

		//---------------------------------------
		[Header("BASE ELEMENTS")]
		[SerializeField]
		private Camera _camera = null;

		//---------------------------------------
		private const float _managerRootPosDistance = -10000.0f;

		//---------------------------------------
		private void Awake()
		{
			DontDestroyOnLoad(_instance);

			Utils.SafeActive(_camera, false);
			transform.position = Vector3.one * _managerRootPosDistance;
		}

		public void GPUPrecacheStart(Component pCacheObject, Action pOnComplete)
		{
			if (pCacheObject == null)
			{
				Utils.SafeInvoke(pOnComplete);
				return;
			}

			StartCoroutine(GPUPrecacheStart_Internal(pCacheObject.gameObject, pOnComplete));
		}

		public void GPUPrecacheStart(GameObject pCacheObject, Action pOnComplete)
		{
			if (pCacheObject == null)
			{
				Utils.SafeInvoke(pOnComplete);
				return;
			}

			StartCoroutine(GPUPrecacheStart_Internal(pCacheObject, pOnComplete));
		}

		private IEnumerator GPUPrecacheStart_Internal(GameObject pCacheObject, Action pOnComplete)
		{
			GameObject pInstanced = GameObject.Instantiate(pCacheObject);
			pInstanced.transform.position = Vector3.one * _managerRootPosDistance;

			Utils.SafeActive(_camera, true);
			_camera.Render();

			//yield return new WaitForEndOfFrame();
			Utils.SafeActive(_camera, false);

			GameObject.DestroyImmediate(pInstanced);
			Utils.SafeInvoke(pOnComplete);
			yield break;
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}
