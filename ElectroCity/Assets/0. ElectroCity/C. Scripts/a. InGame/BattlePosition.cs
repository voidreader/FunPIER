using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google2u;

public class BattlePosition : MonoBehaviour
{
    public Vector3 pos;
    public int order;
    public bool isOccufied = false;


    public MergeItem mergeItem = null;
    public UnitDataRow unitData = null;
    public Unit unit = null;
    public int MergeIncrementalID = 0;

    public void SetUnit(Unit u, MergeItem m) {
        unit = u;
        mergeItem = m;
        isOccufied = true;

        MergeIncrementalID = m.MergeIncrementalID;
    }


    /// <summary>
    /// 전투에서 빼기
    /// </summary>
    public void CleanUnit() {

        Destroy(unit.gameObject);

        unit = null;
        mergeItem = null;
        unitData = null;
        isOccufied = false;

        

    }


}
