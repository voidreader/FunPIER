using UnityEngine;
using System;
using System.Collections.Generic;
using HT;

namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
	{
		//---------------------------------------
		private static T _instance = null;
		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = GameObject.FindObjectOfType<T>();
					if (_instance != null)
						DontDestroyOnLoad(_instance);
				}

				if (_instance == null)
					CreateInstance();

				return _instance;
			}
		}

		public static T Instanced
		{
			get { return _instance; }
		}

		//---------------------------------------
		public static T CreateInstance()
		{
			if (_instance == null)
			{
				GameObject pObject = Utils.Instantiate();
				DontDestroyOnLoad(pObject);

				pObject.name = typeof(T).ToString();
				_instance = pObject.AddComponent<T>();

				pObject.SetActive(true);
			}

			return _instance;
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}
