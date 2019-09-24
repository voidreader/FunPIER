using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Stock : MonoBehaviour
{
    public static Stock main = null;

    [Header("- GameObject -")]
    public GameObject ObjectMergeItem;


    [Header("- Machine Data -")]
    public List<Machine> ListMachines;

    #region Sprites...
    [Header("- Sprite Resources -")]
    public Sprite SpriteBox; // 일반박스 
    public Sprite SpriteGoldBox; // 골드 박스

    public List<Sprite> ListMergeItemSprite;

    [Header("- Passive Skill Icon -")]
    public Sprite SpriteDiscountIcon;
    public Sprite SpriteDPSIcon;



    #endregion

    private void Awake() {
        main = this;
    }


    /// <summary>
    /// 머지 아이템 데이터 가져오기 
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static Machine GetMergeItemData(int level) {

        for(int i=0; i<main.ListMachines.Count;i++) {
            if (main.ListMachines[i].Level == level)
                return main.ListMachines[i];
        }

        return null;
    }
}
