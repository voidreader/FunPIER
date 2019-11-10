using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google2u;


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



    [Header("- GameObject & Prefab -")]
    public GameObject ObjectMergeItem;
    public GameObject ObjectMinion;
    public GameObject ObjectBoss;
    public GameObject ObjectUnit;

    

    #region Sprites...
    [Header("- Sprite Resources -")]
    public Sprite SpriteBox; // 일반박스 
    public Sprite SpriteGoldBox; // 골드 박스
    public Sprite SpriteBottomButtonInactive;
    public Sprite SpriteBottomButtonActive;
    public Sprite SpriteButtonGem;
    public Sprite SpriteButtonCoin;
    public Sprite SpriteButtonLock;
    public Sprite SpriteButtonAD;


    [Header("- Map Sprite Resources -")]
    public Sprite SpriteMapCurrent;
    public Sprite SpriteMapNext;
    public Sprite SpriteMapEnd;

    [Header("- UnitStore Sprite Resources -")]
    public Sprite SpriteStoreAvailable;
    public Sprite SpriteStoreLocked;
    public Sprite SpriteStoreSpecial;
    public Sprite SpriteLevelSignAvailable;
    public Sprite SpriteLevelSignLocked;
    public Sprite SpriteLevelSignSpecial;
    public Sprite SpriteStoreButtonAvailable;
    public Sprite SpriteStoreButtonLocked;
    public Sprite SpriteStoreButtonSpecial;
    public Sprite SpriteStoreButtonAD;






    public List<Sprite> ListMergeItemSprite;

    [Header("- Passive Skill Icon -")]
    public Sprite SpriteDiscountIcon;
    public Sprite SpriteDPSIcon;


    [Header("- Frendly Unit Sprites -")]
    public List<Sprite> ListFriendlyUnitBody;
    public List<Sprite> ListFriendlyUnitFace;
    public List<Sprite> ListFriendlyWeapon;
    public List<Sprite> ListFriendlyUnitUI;

    [Header("- Minion Unit Sprites -")]
    public List<Sprite> ListMinionLeg;
    public List<Sprite> ListMinionBody;

    [Header("- Boss Unit Sprites -")]
    public List<Sprite> ListBossHead;
    public List<Sprite> ListBossBody;
    public List<Sprite> ListBossLeg;
    public List<Sprite> ListBossParts;
    public List<Sprite> ListBossBrokenSprite;
    public List<Sprite> ListBossUI_Sprite;
    public List<Sprite> ListBossFrame_Sprite;


    [Header("- Boss Call Sprites -")]
    public List<Sprite> ListBossCallBGSprite;
    public List<Sprite> ListBossCallPanelSprite;
    public List<Sprite> ListBossCallNameTagSprite;



    [Header("- Weapon List -")]
    public List<TweWeapon> ListWeapon;


    [Header("- Colors -")]
    public Color ColorBottomButtonInactiveText;
    public Color ColorStoreAvailableDPS;
    public Color ColorStoreSpecialDPS;
    public Color ColorStoreLockedDPS;



    #endregion

    private void Awake() {
        
    }


    /// <summary>
    /// 머지 아이템 데이터 가져오기 
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static UnitDataRow GetMergeItemData(int level) {


        for(int i=0; i<UnitData.Instance.Rows.Count;i++) {
            if (UnitData.Instance.Rows[i]._level == level)
                return UnitData.Instance.Rows[i];
        }

        return null;
    }


    /// <summary>
    /// 프렌들리 유닛 UI 스프라이트 가져오기 
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static Sprite GetFriendlyUnitUI(string n) {

        for (int i = 0; i < main.ListFriendlyUnitUI.Count; i++) {
            if (main.ListFriendlyUnitUI[i].name == n)
                return main.ListFriendlyUnitUI[i];
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

    public static Sprite GetFriendlyUnitWeaponSprite(string n) {

        for (int i = 0; i < main.ListFriendlyWeapon.Count; i++) {
            if (main.ListFriendlyWeapon[i].name == n)
                return main.ListFriendlyWeapon[i];
        }

        return null;
    }


    #region Find Weapon

    public static TweWeapon GetFriendlyUnitWeapon(string n) {

        for (int i = 0; i < main.ListWeapon.Count; i++) {
            if (main.ListWeapon[i].name == n)
                return main.ListWeapon[i];
        }

        return null;
    }



    #endregion

    #region Minion & Boss

    /// <summary>
    /// 보스 데이터 가져오기 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static BossDataRow GetBossData(int id) {
        return BossData.Instance.Rows[id - 1];
    }


    public static Sprite GetBossFrame_Sprite(string n) {
        for (int i = 0; i < main.ListBossFrame_Sprite.Count; i++) {
            if (main.ListBossFrame_Sprite[i].name == n) {
                return main.ListBossFrame_Sprite[i];
            }
        }
        return null;
    }


    public static Sprite GetBossUI_Sprite(string n) {
        for (int i = 0; i < main.ListBossUI_Sprite.Count; i++) {
            if (main.ListBossUI_Sprite[i].name == n) {
                return main.ListBossUI_Sprite[i];
            }
        }
        return null;
    }

    public static Sprite GetBossBrokenSprite(string n) {
        for (int i = 0; i < main.ListBossBrokenSprite.Count; i++) {
            if (main.ListBossBrokenSprite[i].name == n) {
                return main.ListBossBrokenSprite[i];
            }
        }
        return null;
    }


    /// <summary>
    /// 보스 부품 스프라이트
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static Sprite GetBossParts(string n) {
        for (int i = 0; i < main.ListBossParts.Count; i++) {
            if (main.ListBossParts[i].name == n) {
                return main.ListBossParts[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 보스 다리 스프라이트
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static Sprite GetBossBody(string n) {
        for (int i = 0; i < main.ListBossBody.Count; i++) {
            if (main.ListBossBody[i].name == n) {
                return main.ListBossBody[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 보스 다리 스프라이트
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static Sprite GetBossLeg(string n) {
        for (int i = 0; i < main.ListBossLeg.Count; i++) {
            if (main.ListBossLeg[i].name == n) {
                return main.ListBossLeg[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 보스 헤드 스프라이트
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static Sprite GetBossHead(string n) {
        for (int i = 0; i < main.ListBossHead.Count; i++) {
            if (main.ListBossHead[i].name == n) {
                return main.ListBossHead[i];
            }
        }
        return null;
    }



    public static MinionDataRow GetMinionData(int id) {
        return MinionData.Instance.Rows[id - 1];
    }



    public static Sprite GetMinionLeg(string n) {
        for(int i=0;i<main.ListMinionLeg.Count;i++) {
            if(main.ListMinionLeg[i].name == n) {
                return main.ListMinionLeg[i];
            }
        }

        return null;
    }

    public static Sprite GetMinionBody(string n) {
        for (int i = 0; i < main.ListMinionBody.Count; i++) {
            if (main.ListMinionBody[i].name == n) {
                return main.ListMinionBody[i];
            }
        }

        return null;
    }


    #endregion

}
