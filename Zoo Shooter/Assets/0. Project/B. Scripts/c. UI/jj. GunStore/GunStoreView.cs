using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DanielLochner.Assets.SimpleScrollSnap;

public class GunStoreView : MonoBehaviour
{
    public static GunStoreView main = null;

    
    public List<GunGroup> _listGroups;
    public SimpleScrollSnap _sc;
    public Image _EquipWeaponSprite;
    

    private void Awake() {
        main = this;
    }



    public void OnView() {

        int startPanel = 0;

        for(int i =0; i<_listGroups.Count;i++) {
            _listGroups[i].OnInit();
        }


        // 장착중인 무기가 어느 그룹에 있는지 찾아야한다. 
        SetEquipWeapon();

        // _sc.GoToPanel(3);
        for (int i=0; i<_listGroups.Count;i++) {

            for(int j=0; j<_listGroups[i]._listGroupWeapon.Count;j++) {
                if (_listGroups[i]._listGroupWeapon[j].WeaponID == PIER.main.CurrentWeapon.WeaponID) {
                    startPanel = i;
                    break;
                }
            } 
        }

        _sc.GoToPanel(startPanel);
        
    }

    public void OnChangedFocusItem() {
        // Debug.Log("GunStore OnChangedFocusItem :: " + _sc.CurrentPanel + "/" +_sc.NearestPanel);
    }

    public void OnSelectedFocusItem() {
        Debug.Log("GunStore OnSelectedFocusItem :: " + _sc.TargetPanel);
        
    }

    public void SetEquipWeapon() {
        _EquipWeaponSprite.sprite = Stocks.GetWeaponStoreSprite(PIER.main.CurrentWeapon);
    }

    /// <summary>
    /// 전체 비활성화 처리 
    /// </summary>
    public void UnselectAll() {
        for (int i = 0; i < _listGroups.Count; i++) {

            for (int j = 0; j < _listGroups[i]._listCols.Count; j++) {
                _listGroups[i]._listCols[j].UnselectWeapon();
            }
        }
    }

}
