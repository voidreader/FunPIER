using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class BossBC_Web : MonoBehaviour
{
	//---------------------------------------
	[SerializeField]
	private AudioClip _audioClip = null;
	[SerializeField]
    private GameObject[] _thornPivots = null;
    [SerializeField]
    private Animation _webAnimation = null;
    [SerializeField]
    private string _webAnim_On = null;
    [SerializeField]
    private string _webAnim_Off = null;

    [SerializeField]
    private ISkillObject _thorn_Instance = null;
    [SerializeField]
    private int _activateThornCount = 4;

    //---------------------------------------

    //---------------------------------------
    private void OnEnable()
    {
		if (_audioClip != null)
			HT.HTSoundManager.PlaySound(_audioClip);

		//-----
		float fRotate = HT.RandomUtils.Range(0.0f, 360.0f);
        gameObject.transform.rotation = Quaternion.Euler(0.0f, fRotate, 0.0f);

        //-----
        _webAnimation.Play(_webAnim_On);

        Invoke("CreateThorns", 0.5f);
    }

    private void CreateThorns()
    {
        List<int> enabledList = new List<int>();
        for (int nInd = 0; nInd < _thornPivots.Length; ++nInd)
            enabledList.Add(nInd);

        for (int nInd = 0; nInd < _activateThornCount; ++nInd)
        {
            ISkillObject pThorn = HT.Utils.InstantiateFromPool(_thorn_Instance);
            pThorn.m_pCaster = BattleFramework._Instance.m_pEnemyActor;

			bool bFoundFailedNewPivot = true;

            int nPivotIndex = HT.RandomUtils.Range(0, _thornPivots.Length);
            for (int nLimit = 0; nLimit < 100; ++nLimit)
            {
                if (enabledList.Contains(nPivotIndex))
                {
                    enabledList.Remove(nPivotIndex);

					bFoundFailedNewPivot = false;
					break;
                }
            }

			if (bFoundFailedNewPivot)
				continue;

            pThorn.gameObject.transform.position = _thornPivots[nPivotIndex].transform.position;
            pThorn.Init();
        }

        //-----
        _webAnimation.Play(_webAnim_Off);
    }

    //---------------------------------------
    private void DestroyGameObject()
    {
        HT.Utils.SafeDestroy(gameObject);
    }
}


/////////////////////////////////////////
//---------------------------------------