using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DanielLochner.Assets.SimpleScrollSnap;

public class GunStoreView : MonoBehaviour
{
    public static GunStoreView main = null;

    
    public List<GunGroup> _listGroups;
    public SimpleScrollSnap _sc;

    private void Awake() {
        main = this;
    }



    public void OnView() {
        for(int i =0; i<_listGroups.Count;i++) {
            _listGroups[i].OnInit();
        }
    }

    public void OnChangedFocusItem() {
        //Debug.Log("GunStore OnChangedFocusItem :: " + _sc.CurrentPanel);
    }

    public void OnSelectedFocusItem() {
        Debug.Log("GunStore OnSelectedFocusItem :: " + _sc.NearestPanel);
        
    }

}
