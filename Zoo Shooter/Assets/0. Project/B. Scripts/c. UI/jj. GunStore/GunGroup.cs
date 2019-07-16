using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunGroup : MonoBehaviour
{

    public WeaponGetType _groupType = WeaponGetType.Unlock250; // 그룹에서 보여질 획득 경로 

    public List<Weapon> _listGroupWeapon; // 그룹에 보여질 무기 리스트 (9개 제한)
    public List<GunColumn> _listCols; // 컬럼스

    public void OnInit() {

        for (int i=0;i<_listCols.Count;i++) {
            _listCols[i].gameObject.SetActive(false);
        }

        for(int i =0; i<_listGroupWeapon.Count;i++) {
            _listCols[i].SetGunProduct(_listGroupWeapon[i]);
        }
    }



    /// <summary>
    /// 언락 셀렉터 해제 
    /// </summary>
    public void SetInactiveUnlockSelect() {
        for (int i = 0; i < _listCols.Count; i++) {
            _listCols[i].SetUnlockSelect(false);
        }
    }

    /// <summary>
    /// 그룹의 모든 무기를 가졌는지 체크 
    /// </summary>
    /// <returns></returns>
    public bool HasThisGroup() {

        bool flag = true;
        for(int i=0; i<_listCols.Count; i++) {

            if (!_listCols[i].gameObject.activeSelf)
                continue;

            //하나라도 소유하지 못한 무기가 있으면 false.
            if (!_listCols[i].HasThisGun()) {
                flag = false;
                break;
            }
        }

        return flag;
    }

}
