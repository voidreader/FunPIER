using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class GameObj_SceneChangeDestroyer : MonoBehaviour
{
	//---------------------------------------
	private void Awake()
	{
		HT.HTFramework.onSceneChange += OnSceneChange;
	}

	private void OnSceneChange()
	{
		HT.Utils.SafeDestroy(gameObject);
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------