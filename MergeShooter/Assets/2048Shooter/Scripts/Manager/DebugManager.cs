using System;
using UnityEngine;

public class DebugManager : SingleInstance<DebugManager>
{
	public bool DEBUG = true;

	private void Awake()
	{
		this.DEBUG = true;
	}

	public void Log(object message)
	{
		if (this.DEBUG)
		{
			UnityEngine.Debug.Log(message);
		}
	}

	public void LogError(object message)
	{
		if (this.DEBUG)
		{
			UnityEngine.Debug.LogError(message);
		}
	}
}
