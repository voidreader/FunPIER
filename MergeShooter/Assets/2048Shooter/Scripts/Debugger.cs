using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyDebugger : MonoBehaviour {
    public static void Log(string message)
	{
		Debug.Log(Application.identifier+" "+message);
	}
}
