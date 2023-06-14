using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////
//---------------------------------------
public class BossBC_CopiedActor_HAL : BossBC_CopiedActor
{
    //---------------------------------------
    [Header("LIGHTNING ROD")]
    [SerializeField]
    private PowerPlant_LightningRod_Group[] _lightningRods = null;
    [SerializeField]
    private float[] _lightningRod_Delay = null;
    [SerializeField]
    private float _lightningRod_Terms = 0.5f;

    [Header("LIGHTNING STRIKE")]
    [SerializeField]
    private PowerPlant_LightningStrike[] _lightningStrike = null;
    [SerializeField]
    private float _lightningStrike_Delay = 0.75f;
    [SerializeField]
    private float _lightningStrike_Terms = 0.5f;

    //---------------------------------------
    private Coroutine _halProc = null;
    private bool _procCompleted = false;

    //---------------------------------------
    public override void CopyActor_Init(bool bUseOnlyOneSkill)
    {
        base.CopyActor_Init(bUseOnlyOneSkill);

        _procCompleted = false;
    }

    public override bool CopyActor_Frame()
    {
        if(base.CopyActor_Frame() == false)
        {
            if (_halProc == null)
            {
                if (_procCompleted == false)
                    _halProc = StartCoroutine(HAL_Proc());

                else
                    return false;
            }
        }

        return true;
    }

    public override void CopyActor_Release()
    {
        base.CopyActor_Release();
    }

    //---------------------------------------
    private IEnumerator HAL_Proc()
    {
        IActorBase pPlayer = BattleFramework._Instance.m_pPlayerActor;

		//-----
		int nDiff = (int)GameFramework._Instance.m_pPlayerData.m_eDifficulty;
        for (int nInd = 0; nInd < _lightningRods.Length; ++nInd)
        {
            _lightningRods[nInd].gameObject.transform.position = pPlayer.transform.position;
            _lightningRods[nInd].ActivateRod(_lightningRod_Delay[nDiff]);

            yield return new WaitForSeconds(_lightningRod_Terms);
        }

        //-----
        yield return new WaitForSeconds(_lightningStrike_Delay);

		//-----
		if (_useOnlyOneSkill == false)
		{
			for (int nInd = 0; nInd < _lightningStrike.Length; ++nInd)
			{
				Vector3 vPos = pPlayer.transform.position;
				_lightningStrike[nInd].ActivateStike(vPos, _lightningStrike_Delay);

				yield return new WaitForSeconds(_lightningStrike_Terms);
			}
		}

        //-----
        _procCompleted = true;
        _halProc = null;
    }

    //---------------------------------------
}


/////////////////////////////////////////
//---------------------------------------