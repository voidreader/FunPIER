using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class BossDK_SoulWalker : MonoBehaviour
{
	//---------------------------------------
	[Header("SETTINGS")]
	public Animation _animation = null;
	public string _root_AnimName = null;

	public GameObject[] _walkers_Part1 = null;
	public GameObject[] _walkers_Part2 = null;
	public int _removeWalkerCount = 2;

	//---------------------------------------
	public static List<BossDK_SoulWalker> _createdList = new List<BossDK_SoulWalker>();

	//---------------------------------------
	private void OnEnable()
	{
		for (int nInd = 0; nInd < _removeWalkerCount; ++nInd)
			_walkers_Part1[Random.Range(0, _walkers_Part1.Length)].SetActive(false);

		for (int nInd = 0; nInd < _removeWalkerCount; ++nInd)
			_walkers_Part2[Random.Range(0, _walkers_Part2.Length)].SetActive(false);

		//-----
		AnimationClip pClip = _animation.GetClip(_root_AnimName);
        _animation.Play(_root_AnimName);

        HT.Utils.SafeDestroy(gameObject, pClip.length);

		//----
		_createdList.Add (this);
	}

	private void OnDisable()
	{
		_createdList.Remove (this);
	}

	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------