using System;
using UnityEngine;

namespace Toolkit
{
	public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>, new()
	{
		private static bool instanceIsNull = true;

		protected static T s_Instance = (T)((object)null);

		private static object s_lock_instance = new object();

		public static T Instance
		{
			get
			{
				object obj = MonoSingleton<T>.s_lock_instance;
				lock (obj)
				{
					if (MonoSingleton<T>.instanceIsNull)
					{
						T t = UnityEngine.Object.FindObjectOfType<T>();
						if (t == null)
						{
							GameObject gameObject = MonoSingleton<T>.FindGameObjectOrCreate("Main");
							t = gameObject.AddComponent<T>();
							t.Init();
						}
						t.AppyInstance();
					}
				}
				return MonoSingleton<T>.s_Instance;
			}
		}

		public static GameObject FindGameObjectOrCreate(string tname)
		{
			GameObject gameObject = GameObject.Find(tname);
			if (gameObject == null)
			{
				gameObject = new GameObject(tname);
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
			return gameObject;
		}

		protected virtual void Awake()
		{
			this.AppyInstance();
		}

		protected void AppyInstance()
		{
			if (MonoSingleton<T>.instanceIsNull)
			{
				MonoSingleton<T>.s_Instance = (this as T);
				MonoSingleton<T>.instanceIsNull = false;
			}
		}

		public virtual void Init()
		{
		}

		protected virtual void OnRelease()
		{
			MonoSingleton<T>.s_Instance = (T)((object)null);
			MonoSingleton<T>.instanceIsNull = true;
		}

		private void OnDestroy()
		{
			this.OnRelease();
		}
	}
}
