using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;
using Doozy.Engine.Progress;




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

    [Header("Battle Position")]
    public List<BattlePosition> ListBP; // 전투 위치 자리 
    public List<BattlePosition> ListTempBP; // 임시 리스트 
    public Transform BattleField;


    [Header("ETC")]
    public Transform FakeTarget;

    [Header("Game Control")]
    public bool IsBossMode = false;
    // public bool 
    public GameObject BossGroup;
    public GameObject BossWarningView;
    public Progressor progressorBossHP, progressorBossTimer; // 게이지들 
    float bossTimer = 90;
    float progressorValue;


    // public Vector2[,] arrBattlePosition = new Vector2[4, 2];
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

        BossGroup.SetActive(false);

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

        if (Input.GetKeyDown(KeyCode.B) && CurrentEnemy) {
            CallBoss();
        }
    }

    IEnumerator PlayRoutine() {

        // 보스 및 미니언 소환 여부 체크 

        CurrentEnemy = GetNewEnemy(false);

        while (true) {
            yield return null;

            if(IsBossMode) {
                continue;
            }


            if(!CurrentEnemy || CurrentEnemy.IsDestroy()) {
                CurrentEnemy = GetNewEnemy(false);

            }

        }
    }

    /// <summary>
    /// 보스 모드 진입 
    /// </summary>
    public void CallBoss() {

        // CurrentEnemy.de
        ViewBossWarning.warningBossID = 1;
        Doozy.Engine.GameEventMessage.SendEvent("BossWarningEvent"); // 보스 등장 UI 처리 

        IsBossMode = true;

        StartCoroutine(BossRoutine());
    }

    IEnumerator BossRoutine() {

        float timervalue = 0;

        while (BossWarningView.activeSelf) {
            yield return null;
        }

        yield return null;


        // 관련 UI 오픈!
        BossGroup.SetActive(true);
        progressorBossHP.SetValue(1);
        progressorBossTimer.SetValue(1);

        CurrentEnemy.BreakImmediate(); // 현재 minion 유닛 파괴 
        GetNewEnemy(true); // 보스 소환 

        bossTimer = 90; // 90초 

        while(bossTimer >= 0) {
            bossTimer -= Time.deltaTime;
            timervalue = bossTimer / 90;
            progressorBossTimer.SetValue(timervalue); // 바 갱신 
            
            yield return null;
        }


        // 보스 전 끝!
        BossGroup.SetActive(false);
    }


    /// <summary>
    /// 새로운 몹 생성 
    /// </summary>
    /// <param name="isBoss"></param>k
    /// <returns></returns>
    EnemyInfo GetNewEnemy(bool isBoss) {

        Debug.Log(">> GetNewEnemy :: " + isBoss);

        EnemyInfo e = null;

        if (isBoss) {
            e = Instantiate(Stock.main.ObjectBoss, new Vector3(2.6f, 2.4f, 0), Quaternion.identity).GetComponent<EnemyInfo>();
            e.InitBoss(1, 100);
        }
        else {

            e = Instantiate(Stock.main.ObjectMinion, new Vector3(2.6f, 2.4f, 0), Quaternion.identity).GetComponent<EnemyInfo>();
            e.InitMinion(Random.Range(1, 8), 100);
        } 
       
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

    #region Equip Slot, Battle Position Data Save & Load


    /// <summary>
    /// 유닛 장착
    /// </summary>
    /// <param name="u"></param>
    public void SetEquipUnit(MergeItem item) {
        BattlePosition bp = GetBattlePosition(item.unitRow);
        item.SetBattle(true);

        bp.unitData = item.unitRow;
        Unit equipUnit = Instantiate(Stock.main.ObjectUnit, bp.pos, Quaternion.identity, BattleField).GetComponent<Unit>();
        
        equipUnit.SetUnit(item.unitRow._level);
        bp.SetUnit(equipUnit, item);

        

        RefreshEquipSlot();
        
    }


    /// <summary>
    /// Middle 장착 슬롯 리프레쉬
    /// </summary>
    void RefreshEquipSlot() {
        int equipUnitCount = 0;
        equipUnitCount = GetEquipUnitCount();
        for (int i = 0; i < equipUnitCount; i++) {
            ListEquipSlot[i].EquipUnit(); // 슬롯 처리 
        }
    }

    /// <summary>
    /// 활성화 유닛 몇개인지 체크
    /// </summary>
    /// <returns></returns>
    int GetEquipUnitCount() {

        int cnt = 0;

        for(int i=0; i<ListBP.Count;i++) {
            if (ListBP[i].isOccufied)
                cnt++;
        }

        return cnt;
    }

    /// <summary>
    /// 배틀 유닛 불러들이기 
    /// </summary>
    /// <param name="u"></param>
    /// <returns></returns>
    public void CallbackBattleUnit(MergeItem item) {

        for(int i=0; i<ListBP.Count;i++) {
            if(ListBP[i].mergeItem == item) {
                item.SetBattle(false);
                ListBP[i].CleanUnit();
                RefreshEquipSlot();
                return;
            }
        }
    }


    /// <summary>
    /// 장착 위치 찾기 (랜덤)
    /// </summary>
    /// <param name="u"></param>
    /// <returns></returns>
    public BattlePosition GetBattlePosition(UnitDataRow u) {
        int level = u._level; // 장착할 유닛의 레벨을 구한다. 

        ListTempBP.Clear();
        for (int i = 0; i < ListBP.Count; i++) {
            ListTempBP.Add(ListBP[i]); 
        }

        for(int i=ListTempBP.Count-1; i>=0; i--) {
            if(ListTempBP[i].isOccufied) {
                ListTempBP.Remove(ListTempBP[i]); // 이미 점유된 곳 제외 
            }
        }

        return ListTempBP[Random.Range(0, ListTempBP.Count)];
    }

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

        /*
        int level;

        for(int i=0; i<ListEquipSlot.Count;i++) {
            level = PlayerPrefs.GetInt(KeyEquipSlot + i.ToString(), 0);

            if (level <= 0)
                continue;

            ListEquipSlot[i].EquipUnit(UnitData.Instance.Rows[level - 1]);
        }
        */
    }

    void SaveEquipSlotMemory() {

        /*
        for(int i =0; i<ListEquipSlot.Count; i++) {
            if(ListEquipSlot[i].equipUnit == null)
                PlayerPrefs.SetInt(KeyEquipSlot + i.ToString(), 0);
            else 
                PlayerPrefs.SetInt(KeyEquipSlot + i.ToString(), ListEquipSlot[i].equipUnit._level);
        }

        PlayerPrefs.Save();
        */

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
