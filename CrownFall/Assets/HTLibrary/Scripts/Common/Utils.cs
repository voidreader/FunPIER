using UnityEngine;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	public class Utils
	{
		/////////////////////////////////////////
		//---------------------------------------
		private interface HTInvokeElement
		{
			void Invoke();
		}

		private class HTInvokeElement_Param0 : HTInvokeElement
		{
			Action _callback = null;

			public HTInvokeElement_Param0(Action pAction)
			{
				_callback = pAction;
			}

			public void Invoke()
			{
				SafeInvoke(_callback);
			}
		}

		private class HTInvokeElement_Param1<T1> : HTInvokeElement
		{
			Action<T1> _callback = null;
			T1 _param1 = default(T1);

			public HTInvokeElement_Param1(Action<T1> pAction, T1 pParam1)
			{
				_callback = pAction;
				_param1 = pParam1;
			}

			public void Invoke()
			{
				SafeInvoke(_callback, _param1);
			}
		}

		private class HTInvokeElement_Param2<T1, T2> : HTInvokeElement
		{
			Action<T1, T2> _callback = null;
			T1 _param1 = default(T1);
			T2 _param2 = default(T2);

			public HTInvokeElement_Param2(Action<T1, T2> pAction, T1 pParam1, T2 pParam2)
			{
				_callback = pAction;
				_param1 = pParam1;
				_param2 = pParam2;
			}

			public void Invoke()
			{
				SafeInvoke(_callback, _param1, _param2);
			}
		}

		private class HTInvokeElement_Param3<T1, T2, T3> : HTInvokeElement
		{
			Action<T1, T2, T3> _callback = null;
			T1 _param1 = default(T1);
			T2 _param2 = default(T2);
			T3 _param3 = default(T3);

			public HTInvokeElement_Param3(Action<T1, T2, T3> pAction, T1 pParam1, T2 pParam2, T3 pParam3)
			{
				_callback = pAction;
				_param1 = pParam1;
				_param2 = pParam2;
				_param3 = pParam3;
			}

			public void Invoke()
			{
				SafeInvoke(_callback, _param1, _param2, _param3);
			}
		}

		private class HTInvokeElement_Param4<T1, T2, T3, T4> : HTInvokeElement
		{
			Action<T1, T2, T3, T4> _callback = null;
			T1 _param1 = default(T1);
			T2 _param2 = default(T2);
			T3 _param3 = default(T3);
			T4 _param4 = default(T4);

			public HTInvokeElement_Param4(Action<T1, T2, T3, T4> pAction, T1 pParam1, T2 pParam2, T3 pParam3, T4 pParam4)
			{
				_callback = pAction;
				_param1 = pParam1;
				_param2 = pParam2;
				_param3 = pParam3;
				_param4 = pParam4;
			}

			public void Invoke()
			{
				SafeInvoke(_callback, _param1, _param2, _param3, _param4);
			}
		}

		//---------------------------------------
		static private List<HTInvokeElement> _invokeElements = new List<HTInvokeElement>();
		static private System.Object _invokeLock = new System.Object();
		static public void InvokeAll()
		{
			lock(_invokeLock)
			{
				for (int nInd = 0; nInd < _invokeElements.Count; ++nInd)
					_invokeElements[nInd].Invoke();

				_invokeElements.Clear();
			}
		}

		/////////////////////////////////////////
		//---------------------------------------
		/// <summary>
		/// Callback을 안전하게 Invoke합니다.
		/// Invoke 호출 위치가 Main Thread가 아닐 경우 지연호출됩니다.
		/// </summary>
		static public bool SafeInvoke(Action pCallback)
		{
			if (pCallback != null)
			{
				if (HTFramework.Instanced != null && HTFramework.Instance.IsMainThead() == false)
				{
					lock (_invokeLock)
						_invokeElements.Add(new HTInvokeElement_Param0(pCallback));
				}
				else
					pCallback();

				return true;
			}

			return false;
		}

		/// <summary>
		/// Callback을 안전하게 Invoke합니다.
		/// Invoke 호출 위치가 Main Thread가 아닐 경우 지연호출됩니다.
		/// </summary>
		static public bool SafeInvoke<T>(Action<T> pCallback, T tParam)
		{
			if (pCallback != null)
			{
				if (HTFramework.Instanced != null && HTFramework.Instance.IsMainThead() == false)
				{
					lock (_invokeLock)
						_invokeElements.Add(new HTInvokeElement_Param1<T>(pCallback, tParam));
				}
				else
					pCallback(tParam);

				return true;
			}

			return false;
		}

		/// <summary>
		/// Callback을 안전하게 Invoke합니다.
		/// Invoke 호출 위치가 Main Thread가 아닐 경우 지연호출됩니다.
		/// </summary>
		static public bool SafeInvoke<T1,T2>(Action<T1,T2> pCallback, T1 tParam1, T2 tParam2)
		{
			if (pCallback != null)
			{
				if (HTFramework.Instanced != null && HTFramework.Instance.IsMainThead() == false)
				{
					lock (_invokeLock)
						_invokeElements.Add(new HTInvokeElement_Param2<T1, T2>(pCallback, tParam1, tParam2));
				}
				else
					pCallback(tParam1, tParam2);

				return true;
			}

			return false;
		}

		/// <summary>
		/// Callback을 안전하게 Invoke합니다.
		/// Invoke 호출 위치가 Main Thread가 아닐 경우 지연호출됩니다.
		/// </summary>
		static public bool SafeInvoke<T1, T2, T3>(Action<T1, T2, T3> pCallback, T1 tParam1, T2 tParam2, T3 tParam3)
		{
			if (pCallback != null)
			{
				if (HTFramework.Instanced != null && HTFramework.Instance.IsMainThead() == false)
				{
					lock (_invokeLock)
						_invokeElements.Add(new HTInvokeElement_Param3<T1, T2, T3>(pCallback, tParam1, tParam2, tParam3));
				}
				else
					pCallback(tParam1, tParam2, tParam3);

				return true;
			}

			return false;
		}

		/// <summary>
		/// Callback을 안전하게 Invoke합니다.
		/// Invoke 호출 위치가 Main Thread가 아닐 경우 지연호출됩니다.
		/// </summary>
		static public bool SafeInvoke<T1, T2, T3, T4>(Action<T1, T2, T3, T4> pCallback, T1 tParam1, T2 tParam2, T3 tParam3, T4 tParam4)
		{
			if (pCallback != null)
			{
				if (HTFramework.Instanced != null && HTFramework.Instance.IsMainThead() == false)
				{
					lock (_invokeLock)
						_invokeElements.Add(new HTInvokeElement_Param4<T1, T2, T3, T4>(pCallback, tParam1, tParam2, tParam3, tParam4));
				}
				else
					pCallback(tParam1, tParam2, tParam3, tParam4);

				return true;
			}

			return false;
		}

		//---------------------------------------
		/// <summary>
		/// Callback을 특정 시간 후에 Invoke합니다.
		/// </summary>
		static public void SafeInvokeDelay(float fTime, Action pCallback)
		{
			if (pCallback != null)
				StopWatch.CreateNewStopWatch(fTime, true, ()=> { SafeInvoke(pCallback); });
		}

		/// <summary>
		/// Callback을 특정 시간 후에 Invoke합니다.
		/// </summary>
		static public void SafeInvokeDelay<T>(float fTime, Action<T> pCallback, T tParam)
		{
			if (pCallback != null)
				StopWatch.CreateNewStopWatch(fTime, true, () => { SafeInvoke(pCallback, tParam); });
		}

		/// <summary>
		/// Callback을 특정 시간 후에 Invoke합니다.
		/// </summary>
		static public void SafeInvokeDelay<T1, T2>(float fTime, Action<T1, T2> pCallback, T1 tParam1, T2 tParam2)
		{
			if (pCallback != null)
				StopWatch.CreateNewStopWatch(fTime, true, () => { SafeInvoke(pCallback, tParam1, tParam2); });
		}

		/// <summary>
		/// Callback을 특정 시간 후에 Invoke합니다.
		/// </summary>
		static public void SafeInvokeDelay<T1, T2, T3>(float fTime, Action<T1, T2, T3> pCallback, T1 tParam1, T2 tParam2, T3 tParam3)
		{
			if (pCallback != null)
				StopWatch.CreateNewStopWatch(fTime, true, () => { SafeInvoke(pCallback, tParam1, tParam2, tParam3); });
		}

		//---------------------------------------
		/// <summary>
		/// CallBack에 AddFunc를 등록 한 뒤 AddFunc를 호출합니다.
		/// CallBack에 등록과 호출이 동시에 되야하는 Initialize 과정에 적절합니다.
		/// </summary>
		/// <param name="pCallback">AddFunc가 추가 될 대상 Callback</param>
		/// <param name="pAddFunc">추가 된 뒤 호출 될 AddFunc</param>
		static public void SafeInvokeAndAdd(ref Action pCallback, Action pAddFunc)
		{
			if (pAddFunc == null)
				return;

			pCallback += pAddFunc;
			pAddFunc();
		}

		/// <summary>
		/// CallBack에 AddFunc를 등록 한 뒤 AddFunc를 호출합니다.
		/// CallBack에 등록과 호출이 동시에 되야하는 Initialize 과정에 적절합니다.
		/// </summary>
		/// <param name="pCallback">AddFunc가 추가 될 대상 Callback</param>
		/// <param name="pAddFunc">추가 된 뒤 호출 될 AddFunc</param>
		static public void SafeInvokeAndAdd<T>(ref Action<T> pCallback, Action<T> pAddFunc, T tParam)
		{
			if (pAddFunc == null)
				return;

			pCallback += pAddFunc;
			pAddFunc(tParam);
		}

		/// <summary>
		/// CallBack에 AddFunc를 등록 한 뒤 AddFunc를 호출합니다.
		/// CallBack에 등록과 호출이 동시에 되야하는 Initialize 과정에 적절합니다.
		/// </summary>
		/// <param name="pCallback">AddFunc가 추가 될 대상 Callback</param>
		/// <param name="pAddFunc">추가 된 뒤 호출 될 AddFunc</param>
		static public void SafeInvokeAndAdd<T1, T2>(ref Action<T1,T2> pCallback, Action<T1,T2> pAddFunc, T1 tParam1, T2 tParam2)
		{
			if (pAddFunc == null)
				return;

			pCallback += pAddFunc;
			pAddFunc(tParam1, tParam2);
		}

		/// <summary>
		/// CallBack에 AddFunc를 등록 한 뒤 AddFunc를 호출합니다.
		/// CallBack에 등록과 호출이 동시에 되야하는 Initialize 과정에 적절합니다.
		/// </summary>
		/// <param name="pCallback">AddFunc가 추가 될 대상 Callback</param>
		/// <param name="pAddFunc">추가 된 뒤 호출 될 AddFunc</param>
		static public void SafeInvokeAndAdd<T1, T2, T3>(ref Action<T1, T2, T3> pCallback, Action<T1, T2, T3> pAddFunc, T1 tParam1, T2 tParam2, T3 tParam3)
		{
			if (pAddFunc == null)
				return;

			pCallback += pAddFunc;
			pAddFunc(tParam1, tParam2, tParam3);
		}


		/////////////////////////////////////////
		//---------------------------------------
		/// <summary>
		/// GameObject의 Active State를 안전하게 변경합니다.
		/// </summary>
		static public void SafeActive(GameObject pObject, bool bActive)
		{
			if (pObject != null && pObject.activeInHierarchy != bActive)
				pObject.SetActive(bActive);
		}

		/// <summary>
		/// GameObject 배열의 Active State를 안전하게 변경합니다.
		/// </summary>
		static public void SafeActive(GameObject[] vObject, bool bActive)
		{
			for(int nInd = 0; nInd < vObject.Length; ++nInd)
				SafeActive(vObject[nInd], bActive);
		}

		/// <summary>
		/// MonoBehaviour가 등록되어있는 GameObject의 Active State를 안전하게 변경합니다.
		/// </summary>
		static public void SafeActive(Component pObject, bool bActive)
		{
			if (pObject != null && pObject.gameObject.activeInHierarchy != bActive)
				pObject.gameObject.SetActive(bActive);
		}

		/// <summary>
		/// MonoBehaviour가 등록되어있는 GameObject 배열의 Active State를 안전하게 변경합니다.
		/// </summary>
		static public void SafeActive(Component[] vObject, bool bActive)
		{
			for (int nInd = 0; nInd < vObject.Length; ++nInd)
				SafeActive(vObject[nInd], bActive);
		}

		//---------------------------------------
		/// <summary>
		/// MonoBehaviour의 Enable State를 안전하게 변경합니다.
		/// </summary>
		static public void SafeEnable(MonoBehaviour pObject, bool bActive)
		{
			if (pObject != null && pObject.enabled != bActive)
				pObject.enabled = bActive;
		}

		//---------------------------------------
		/// <summary>
		/// GameObject를 안전하게 파괴합니다.
		/// Object Pool에 귀속 된 GameObject는 Object Pool로 환원됩니다.
		/// </summary>
		static public bool SafeDestroy(GameObject pObject)
		{
			if (pObject == null)
				return false;

			if (ObjectPool.ReturnToPool(pObject))
				return true;

			GameObject.Destroy(pObject);
			return true;
		}

		/// <summary>
		/// GameObject를 안전하게 파괴합니다.
		/// Object Pool에 귀속 된 GameObject는 Object Pool로 환원됩니다.
		/// </summary>
		static public bool SafeDestroy(Component pObject)
		{
			if (pObject == null)
				return false;

			return SafeDestroy(pObject.gameObject);
		}

		/// <summary>
		/// GameObject를 특정 시간 지연 후에 안전하게 파괴합니다.
		/// Object Pool에 귀속 된 GameObject는 Object Pool로 환원됩니다.
		/// </summary>
		/// <param name="fDelay">파괴 지연 시간</param>
		static public bool SafeDestroy(GameObject pObject, float fDelay)
		{
			return SafeDestroy(pObject, fDelay, false);
		}

		/// <summary>
		/// GameObject를 특정 시간 지연 후에 안전하게 파괴합니다.
		/// Object Pool에 귀속 된 GameObject는 Object Pool로 환원됩니다.
		/// </summary>
		/// <param name="fDelay">파괴 지연 시간</param>
		static public bool SafeDestroy(Component pObject, float fDelay)
		{
			if (pObject == null)
				return false;

			return SafeDestroy(pObject.gameObject, fDelay, false);
		}

		/// <summary>
		/// GameObject를 특정 시간 지연 후에 안전하게 파괴합니다.
		/// Object Pool에 귀속 된 GameObject는 Object Pool로 환원됩니다.
		/// </summary>
		/// <param name="fDelay">파괴 지연 시간</param>
		/// <param name="bRealTime">지연 시간에 Time Scale 적용 여부입니다</param>
		static public bool SafeDestroy(GameObject pObject, float fDelay, bool bRealTime)
		{
			if (pObject == null)
				return false;

			if (ObjectPool.IsPoolObject(pObject))
			{
				StopWatch.CreateNewStopWatch(fDelay, bRealTime, () => { ObjectPool.ReturnToPool(pObject); });
				return true;
			}

			GameObject.Destroy(pObject, fDelay);
			return true;
		}

		/// <summary>
		/// GameObject를 특정 시간 지연 후에 안전하게 파괴합니다.
		/// Object Pool에 귀속 된 GameObject는 Object Pool로 환원됩니다.
		/// </summary>
		/// <param name="fDelay">파괴 지연 시간</param>
		/// <param name="bRealTime">지연 시간에 Time Scale 적용 여부입니다</param>
		static public bool SafeDestroy(Component pObject, float fDelay, bool bRealTime)
		{
			if (pObject == null)
				return false;

			return SafeDestroy(pObject.gameObject, fDelay, bRealTime);
		}


		/////////////////////////////////////////
		//---------------------------------------
		/// <summary>
		/// Coroutine을 안전하게 정지시킵니다.
		/// </summary>
		/// <param name="pParent">Coroutine이 귀속 된 MonoBehaviour</param>
		/// <param name="pCorutine">대상 Coroutine</param>
		static public void SafeStopCorutine(MonoBehaviour pParent, ref Coroutine pCorutine)
		{
			do
			{
				if (pParent == null || pCorutine == null)
					break;

				pParent.StopCoroutine(pCorutine);
			}
			while (false);

			pCorutine = null;
		}


		/////////////////////////////////////////
		//---------------------------------------
		/// <summary>
		/// Resource에서 Load 된 GameObject를 ObjectPool을 사용해 복제 생성합니다.
		/// </summary>
		static public GameObject InstantiateFromPool(string szResourceAddress, Transform pInheritTransform = null, bool bInheritPos = false, bool bInheritRot = false)
		{
			if (HTFramework.GameClosed)
				return null;

			return ObjectPool.InstantiateObject(szResourceAddress, pInheritTransform, bInheritPos, bInheritRot);
		}

		/// <summary>
		/// ObjectPool을 사용해 복제 생성합니다.
		/// </summary>
		static public GameObject InstantiateFromPool(GameObject pOriginal, Transform pInheritTransform = null, bool bInheritPos = false, bool bInheritRot = false)
		{
			if (HTFramework.GameClosed)
				return null;

			if (pOriginal == null)
				return null;

			return ObjectPool.InstantiateObject(pOriginal, pInheritTransform, bInheritPos, bInheritRot);
		}

		/// <summary>
		/// ObjectPool을 사용해 복제 생성합니다.
		/// </summary>
		static public T InstantiateFromPool<T>(T pOriginal, Transform pInheritTransform = null, bool bInheritPos = false, bool bInheritRot = false) where T : MonoBehaviour
		{
			if (HTFramework.GameClosed)
				return null;

			if (pOriginal == null)
				return null;

			GameObject pObj = ObjectPool.InstantiateObject(pOriginal.gameObject, pInheritTransform, bInheritPos, bInheritRot);
			return pObj.GetComponent<T>();
		}

		/// <summary>
		/// ObjectPool을 사용해 복제 생성합니다.
		/// </summary>
		static public ParticleSystem InstantiateFromPool(ParticleSystem pOriginal, Transform pInheritTransform = null, bool bInheritPos = false, bool bInheritRot = false)
		{
			if (HTFramework.GameClosed)
				return null;

			if (pOriginal == null)
				return null;

			GameObject pObj = ObjectPool.InstantiateObject(pOriginal.gameObject, pInheritTransform, bInheritPos, bInheritRot);
			return pObj.GetComponent<ParticleSystem>();
		}

		//---------------------------------------
		/// <summary>
		/// Resource에서 Load 된 GameObject를 복제 생성합니다.
		/// </summary>
		static public GameObject Instantiate(string szResourceAddress)
		{
			if (HTFramework.GameClosed)
				return null;

			return Instantiate(Resources.Load(szResourceAddress) as GameObject);
		}

		/// <summary>
		/// GameObject를 복제 생성합니다.
		/// </summary>
		static public GameObject Instantiate(GameObject pOriginal)
		{
			if (HTFramework.GameClosed)
				return null;

			if (pOriginal == null)
				return null;

			return GameObject.Instantiate(pOriginal);
		}

		/// <summary>
		/// GameObject를 복제 생성합니다.
		/// </summary>
		static public T Instantiate<T>(T pOriginal) where T : Component
		{
			if (HTFramework.GameClosed)
				return null;

			if (pOriginal == null)
				return null;

			GameObject pInst = Instantiate(pOriginal.gameObject);
			return pInst.GetComponent<T>();
		}

		/// <summary>
		/// GameObject를 복제 생성합니다.
		/// </summary>
		static public ParticleSystem Instantiate(ParticleSystem pOriginal)
		{
			if (HTFramework.GameClosed)
				return null;

			if (pOriginal == null)
				return null;

			GameObject pInst = Instantiate(pOriginal.gameObject);
			return pInst.GetComponent<ParticleSystem>();
		}

		/// <summary>
		/// 비어있는 GameObject를 생성합니다.
		/// </summary>
		static public GameObject Instantiate()
		{
			if (HTFramework.GameClosed)
				return null;
			
			return new GameObject();
		}


		/////////////////////////////////////////
		//---------------------------------------
		/// <summary>
		/// 해당 GameObject의 Transform을 참조 대상 Transform과 맞춥니다.
		/// </summary>
		/// <param name="pTransform">참조 대상 Transform</param>
		/// <param name="bInheritPos">대상 Transform의 위치를 참조합니다</param>
		/// <param name="bInheritRot">대상 Transform의 회전을 참조합니다</param>
		/// <param name="bInheritScale">대상 Transform의 크기를 참조합니다</param>
		/// <param name="bInheritParent">대상 Transform의 Hierarchy 하위에 위치시킵니다</param>
		static public void InheritTransform(GameObject pObject, Transform pTransform, bool bInheritPos = false, bool bInheritRot = false, bool bInheritScale = false, bool bInheritParent = false)
		{
			if (pObject == null || pTransform == null)
				return;

			if (bInheritPos)
				pObject.transform.position = pTransform.position;

			if (bInheritRot)
				pObject.transform.rotation = pTransform.rotation;

			if (bInheritParent)
				pObject.transform.SetParent(pTransform);

			if (bInheritScale)
				pObject.transform.localScale = pTransform.localScale;
		}
	}
}