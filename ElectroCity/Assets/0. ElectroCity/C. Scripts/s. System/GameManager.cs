using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;


/// <summary>
/// 게임내에서 Middle 관련 UI 제어와 연관이 깊다. 
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager main = null;

    [Header("Stage")]
    public int Stage = 1;
    public int KillCount = 200;

    int DPSLevel, DiscountLevel;

    public EnemyInfo CurrentEnemy = null;

    // 캐릭터 장착 슬롯
    [Header("Unit Equipment")]
    public List<EquipSlot> ListEquipSlot;

    [Header("Kill Count")]
    public Text TextKillCount;

    [Header("Passive")]
    public GameObject PassiveDPS;
    public GameObject PassiveDiscount;
    public Text TextPassiveDPS, TextPassiveDiscount;


    [Header("ETC")]
    public Transform FakeTarget;

    public Vector2[,] arrBattlePosition = new Vector2[4, 2];
    //{ new Vector2(-0.319f, 1.869f), new Vector2(-0.263f, 1.239f), new Vector2(-1.082f, 1.712f), new Vector2(-0.932f, 1.05f) },  {new Vector2(-1.951f, 1.689f), new Vector2(-1.711f, 1.183f), new Vector2(-2.719f, 1.869f), new Vector2(-2.569f, 1.05f) }};

    const string KeyEquipSlot = "EquipSlot";
    const string KeyStage = "CurrentStage";
    const string KeyKillCountg = "CurrentKillCount";

    const int MaxKillCount = 200;


    void Awake() {
        main = this;
        PIER.OnRefreshPlayerInfo += RefreshInfo;
        // mainCamera.aspect = 9f / 16f;
        Camera.main.aspect = 9f / 16f;

        arrBattlePosition[3, 0] = new Vector2(-0.319f, 1.869f);
        arrBattlePosition[3, 1] = new Vector2(-0.263f, 1.239f);

        arrBattlePosition[2, 0] = new Vector2(-0.932f, 1.05f);
        arrBattlePosition[2, 1] = new Vector2(-1.082f, 1.712f);

        arrBattlePosition[1, 0] = new Vector2(-0.319f, 1.869f);
        arrBattlePosition[1, 1] = new Vector2(-0.263f, 1.239f);

        arrBattlePosition[0, 0] = new Vector2(-0.319f, 1.869f);
        arrBattlePosition[0, 1] = new Vector2(-0.263f, 1.239f);

    }


    void Start() {

        for (int i = 0; i < ListEquipSlot.Count; i++) {
            ListEquipSlot[i].InitEquipSlot(i);
        }

        LoadStageMemory();
        LoadEquipSlotMemory();

        // 스테이지 진행 시작 
        StartCoroutine(PlayRoutine());

    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.K) && CurrentEnemy) {
            CurrentEnemy.SetDamage(1000);
        }
    }

    IEnumerator PlayRoutine() {

        // 보스 및 미니언 소환 여부 체크 

        CurrentEnemy = GetNewEnemy(false);

        while (true) {
            yield return null;


            if(!CurrentEnemy || CurrentEnemy.IsDestroy()) {
                CurrentEnemy = GetNewEnemy(false);

            }

        }
    }


    /// <summary>
    /// 새로운 몹 생성 
    /// </summary>
    /// <param name="isBoss"></param>k
    /// <returns></returns>
    EnemyInfo GetNewEnemy(bool isBoss) {

        

        EnemyInfo e = Instantiate(Stock.main.ObjectMinion, new Vector3(2.6f, 2.4f, 0), Quaternion.identity).GetComponent<EnemyInfo>();
        e.InitMinion(Random.Range(1,8), 100);
        return e;

    }


    void RefreshInfo() {

        DPSLevel = PIER.main.DamageLevel;
        DiscountLevel = PIER.main.DiscountLevel;

        if (DPSLevel == 1)
            PassiveDPS.SetActive(false);
        else {
            PassiveDPS.SetActive(true);
        }


        if (DiscountLevel == 1)
            PassiveDiscount.SetActive(false);
        else {
            PassiveDiscount.SetActive(true);
        }



    }

    #region Equip Slot, Data Save & Load

    /// <summary>
    /// 스테이지 진행도 불러오기 
    /// </summary>
    void LoadStageMemory() {
        Stage = PlayerPrefs.GetInt(KeyStage, 1);
        KillCount = PlayerPrefs.GetInt(KeyKillCountg, MaxKillCount);


        TextKillCount.text = "Destroy " + KillCount.ToString() + " Enemies";
    }

    /// <summary>
    /// 스테이지 진행도 저장 
    /// </summary>
    void SaveStageMemory() {
        PlayerPrefs.SetInt(KeyStage, Stage);
        PlayerPrefs.SetInt(KeyKillCountg, KillCount);
        PlayerPrefs.Save();
    }


    void LoadEquipSlotMemory() {

        int level;

        for(int i=0; i<ListEquipSlot.Count;i++) {
            level = PlayerPrefs.GetInt(KeyEquipSlot + i.ToString(), 0);

            if (level <= 0)
                continue;

            ListEquipSlot[i].EquipUnit(UnitData.Instance.Rows[level - 1]);
        }
    }

    void SaveEquipSlotMemory() {
        for(int i =0; i<ListEquipSlot.Count; i++) {
            if(ListEquipSlot[i].equipUnit == null)
                PlayerPrefs.SetInt(KeyEquipSlot + i.ToString(), 0);
            else 
                PlayerPrefs.SetInt(KeyEquipSlot + i.ToString(), ListEquipSlot[i].equipUnit._level);
        }

        PlayerPrefs.Save();

    }
    #endregion


    private void OnApplicationPause(bool pause) {
        if (pause) {
            SaveEquipSlotMemory();
            SaveStageMemory();
        }
    }

    private void OnApplicationQuit() {
        SaveEquipSlotMemory();
    }

}
