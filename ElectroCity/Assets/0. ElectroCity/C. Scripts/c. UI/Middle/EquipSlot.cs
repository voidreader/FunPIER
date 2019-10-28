using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;

public class EquipSlot : MonoBehaviour
{

    public int index = 0;
    public Image image;

    [Header(" - Sprite Source - ")]
    public Sprite SpriteEquip;
    public Sprite SpriteUnEquip;


    /// <summary>
    /// 초기화 
    /// </summary>
    /// <param name="i"></param>
    public void InitEquipSlot(int i) {
        index = i;
        image.sprite = SpriteUnEquip;
    }

    public void EquipUnit() {
        image.sprite = SpriteEquip;
    }

    public void UnEquipUnit() {
        
        image.sprite = SpriteUnEquip;
    }

    public bool IsEquip() {
        return image.sprite == SpriteEquip;
    }



}

