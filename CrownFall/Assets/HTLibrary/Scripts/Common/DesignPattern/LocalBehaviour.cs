using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HT;
using System.Collections;


/////////////////////////////////////////
//---------------------------------------
public class LocalBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
	//---------------------------------------
	private static T _instanced = null;
	public static T Instanced { get { return _instanced; } }

	//---------------------------------------
	protected virtual void Awake()
	{
		if (_instanced != null)
			HTDebug.PrintLog(eMessageType.Error, string.Format("[LocalBehaviour] {0}'s instance is alreay activated!", typeof(T).ToString()));

		_instanced = this as T;
		OnAwake();
	}

	protected virtual void OnDestroy()
	{
		_instanced = null;
		OnRelease();
	}

	//---------------------------------------
	protected virtual void OnAwake() { }
	protected virtual void OnRelease() { }

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------