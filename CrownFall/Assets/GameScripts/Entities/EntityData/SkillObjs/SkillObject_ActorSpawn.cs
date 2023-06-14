using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public sealed class SkillObject_ActorSpawn : ISkillObject
{
	/////////////////////////////////////////
	//---------------------------------------
	[Header("ACTOR SPAWN INFO")]
	public SpawnActor _spawnActor = null;
    public int _spawnCount = 1;
    public bool _spawnRotateByView = true;

	//---------------------------------------
	public ISkillObject _postSkillObject = null;


	/////////////////////////////////////////
	//---------------------------------------
	public override void Init()
	{
		if (_spawnActor != null)
        {
            float fDefaultRotation = HT.RandomUtils.Range(0.0f, 360.0f);
            float fRotatePerObj = 360.0f / _spawnCount;

            //-----
            for (int nInd = 0; nInd < _spawnCount; ++nInd)
            {
                SpawnActor pSpawn = HT.Utils.Instantiate(_spawnActor);
				pSpawn.gameObject.SetActive(true);

				pSpawn.SetSpawnActor(m_pCaster);

                pSpawn.transform.position = GameFramework._Instance.GetPositionByPhysic(gameObject.transform.position);
                if (_spawnCount > 1)
                {
                    if (_spawnRotateByView)
                        pSpawn.m_vViewVector = Quaternion.Euler(0.0f, fDefaultRotation + (fRotatePerObj * nInd), 0.0f) * Vector3.forward;
                    else
                        pSpawn.transform.rotation = Quaternion.Euler(0.0f, fDefaultRotation + (fRotatePerObj * nInd), 0.0f);
                }

                pSpawn.Init();
            }
		}

		HT.Utils.SafeDestroy(gameObject);
	}

	public override void Frame()
	{
	}

	public override void Release()
	{
		if (_postSkillObject != null)
		{
			ISkillObject pNewObj = HT.Utils.Instantiate(_postSkillObject);
			GameFramework._Instance.SetObjectPositionAndRotateByPhysic(pNewObj.gameObject, gameObject.transform.position);

			if (pNewObj.m_bInheritRotation)
				pNewObj.transform.rotation = pNewObj.transform.rotation * gameObject.transform.rotation;

			pNewObj.m_pCaster = m_pCaster;
		}
	}


	/////////////////////////////////////////
	//---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------