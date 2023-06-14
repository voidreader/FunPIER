using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace HT
{
	/////////////////////////////////////////
	//---------------------------------------
	[CustomEditor(typeof(Inspector_HTLightEffect))]
	public class Inspector_HTLightEffect : Editor
	{
		//---------------------------------------
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
		}

		//---------------------------------------
	}


	/////////////////////////////////////////
	//---------------------------------------
}