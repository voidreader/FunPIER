using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunGroup : MonoBehaviour
{

    public WeaponGetType _groupType = WeaponGetType.Unlock250; // 그룹에서 보여질 획득 경로 

    public List<Weapon> _listGroupWeapon; // 그룹에 보여질 무기 리스트 (9개 제한)
    public List<GunColumn> _listCols; // 컬럼스

}
