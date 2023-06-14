using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HT
{

	/////////////////////////////////////////
	//---------------------------------------
	public enum eSingletonMethod
	{
		CreateInstance,
		DestroyInstance,
	}

	public interface ISingleton
	{
		void CallMethod(eSingletonMethod method);

		void Update(float fDelta);
		void FixedUpdate(float fDelta);

		void OnApplicationPause(bool bPause);
	}


	/////////////////////////////////////////
	//---------------------------------------
	public class SingletonBase<T> where T : class, new()
	{
		protected static bool _isCreateEnabled = true;
		private static T _instance;
		public static T Instance { get { return CreateInstance(); } }
		public static T Instanced { get { return _instance; } }

		protected SingletonBase()
		{
		}

		public static T CreateInstance()
		{
			if (_instance == null && _isCreateEnabled)
			{
				_instance = new T();
				(_instance as SingletonBase<T>).OnCreateInstance();
				SingletonContainer.AddInstance(_instance as ISingleton);
			}
			return _instance;
		}

		public static void DestroyInstance()
		{
			if (_instance != null)
			{
				SingletonContainer.RemoveInstance(_instance as ISingleton);
				(_instance as SingletonBase<T>).OnDestroyInstance();
			}

			_instance = null;
		}

		protected virtual void OnCreateInstance()
		{

		}

		protected virtual void OnDestroyInstance()
		{

		}
	}

	//---------------------------------------
	public class Singleton<T> : SingletonBase<T>, ISingleton where T : class, new()
	{
		public void CallMethod(eSingletonMethod method)
		{
			switch (method)
			{
				case eSingletonMethod.CreateInstance:
					{
						CreateInstance();
					}
					break;
				case eSingletonMethod.DestroyInstance:
					{
						DestroyInstance();
					}
					break;
			}

		}

		//---------------------------------------
		protected virtual void Initialize()
		{

		}
		
		protected virtual void Release()
		{

		}

		//---------------------------------------
		public virtual void Update(float fDelta)
		{
		}
		
		public virtual void FixedUpdate(float fDelta)
		{
		}

		//---------------------------------------
		public virtual void OnApplicationPause(bool bPause)
		{

		}

		//---------------------------------------
		protected override void OnCreateInstance()
		{
			_isCreateEnabled = false;
			Initialize();
		}

		protected override void OnDestroyInstance()
		{
			Release();
		}

		//---------------------------------------
	}

	//---------------------------------------
	public class SingletonContainer
	{
		//---------------------------------------
		private static List<ISingleton> _instances = new List<ISingleton>();
		public static List<ISingleton> Instances { get { return _instances; } }

		//---------------------------------------
		public static void AddInstance(ISingleton instance)
		{
			if (_instances.Contains(instance))
				return;

			_instances.Add(instance);
		}

		public static void RemoveInstance(ISingleton instance)
		{
			_instances.Remove(instance);
		}

		//---------------------------------------
		public static void Release()
		{
			if (_instances == null)
				return;

			for (int i = _instances.Count - 1; i >= 0; --i)
			{
				if (_instances[i] != null)
					_instances[i].CallMethod(eSingletonMethod.DestroyInstance);
			}

			_instances.Clear();
		}

		//---------------------------------------
		public static void Update(float fDelta)
		{
			for (int i = 0; i < _instances.Count; ++i)
				_instances[i].Update(fDelta);
		}

		public static void FixedUpdate(float fDelta)
		{
			for (int i = 0; i < _instances.Count; ++i)
				_instances[i].FixedUpdate(fDelta);
		}

		//---------------------------------------
		public static void OnApplicationPause(bool bPause)
		{
			for (int i = 0; i < _instances.Count; ++i)
				_instances[i].OnApplicationPause(bPause);
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}