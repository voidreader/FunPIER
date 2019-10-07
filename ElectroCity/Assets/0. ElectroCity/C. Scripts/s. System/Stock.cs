using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Stock : MonoBehaviour
{
    public static Stock _main = null;

    public static Stock main {
        get {
            if (_main == null) {

                _main = FindObjectOfType<Stock>();

                if (_main == null)
                    _main = Instantiate(Resources.Load<Stock>("Prefab/Stock"));
            }

            return _main;
        }
    }



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


    [Header("- Frendly Unit Sprites -")]
    public List<Sprite> ListFriendlyUnitBody;
    public List<Sprite> ListFriendlyUnitFace;
    public List<Sprite> ListFriendlyWeapon;


    #endregion

    private void Awake() {
        
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


    /// <summary>
    /// 프렌들리 유닛 바디, 레그 스프라이트 
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static Sprite GetFriendlyUnitBody(string n) {

        for(int i=0; i<main.ListFriendlyUnitBody.Count;i++) {
            if (main.ListFriendlyUnitBody[i].name == n)
                return main.ListFriendlyUnitBody[i];
        }

        return null;
    }

    /// <summary>
    /// 프렌들리 유닛 얼굴
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static Sprite GetFriendlyUnitFace(string n) {

        for (int i = 0; i < main.ListFriendlyUnitFace.Count; i++) {
            if (main.ListFriendlyUnitFace[i].name == n)
                return main.ListFriendlyUnitFace[i];
        }

        return null;
    }

    public static Sprite GetFriendlyUnitWeapon(string n) {

        for (int i = 0; i < main.ListFriendlyWeapon.Count; i++) {
            if (main.ListFriendlyWeapon[i].name == n)
                return main.ListFriendlyWeapon[i];
        }

        return null;
    }

}
