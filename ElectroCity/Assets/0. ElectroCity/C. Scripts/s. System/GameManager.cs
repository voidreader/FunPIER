using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google2u;
using Doozy.Engine.Progress;
using DG.Tweening;


/*
 * SetKillCountText 에서 보스 Call 버튼 활성 조건을 설정
 * 
 * 
 * 
 */



/// <summary>
/// 게임내에서 Middle 관련 UI 제어와 연관이 깊다. 
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager main = null;
    public static bool HoldFire = false;
        

    [Header("Stage")]
    public int Stage = 1;
    public int KillCount = 75;
    public long EarningCoin = 0; // 1초에 획득하는 코인 (유닛의 Earning 값)
    public long TotalEarningCoin; // 누적된 획득 코인 - 적이 죽어야 획득함. 
    public Text TextEarningCoin;

    int DPSLevel, DiscountLevel;

    public EnemyInfo CurrentEnemy = null;

    // 캐릭터 장착 슬롯
    [Header("Unit Equipment")]
    public List<EquipSlot> ListEquipSlot;

    [Header("Kill Count")]
    public Text TextKillCount;
    public Progressor progressorKillCount;

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
    public GameObject BossSkull, ButtonSurrender; // 보스 HP 게이지 좌우 


    public Doozy.Engine.UI.UIButton ButtonCallBoss; // 보스 콜 버튼
    decimal bosshpvalue; 
    float bossTimer = 60;
    float progressorValue;


    // public Vector2[,] arrBattlePosition = new Vector2[4, 2];
    //{ new Vector2(-0.319f, 1.869f), new Vector2(-0.263f, 1.239f), new Vector2(-1.082f, 1.712f), new Vector2(-0.932f, 1.05f) },  {new Vector2(-1.951f, 1.689f), new Vector2(-1.711f, 1.183f), new Vector2(-2.719f, 1.869f), new Vector2(-2.569f, 1.05f) }};

    const string KeyBattlePosition = "BattlePosition";
    const string KeyStage = "CurrentStage";
    const string KeyKillCountg = "CurrentKillCount";

    const int MaxKillCount = 10;


    void Awake() {
        main = this;
        PIER.OnRefreshPlayerInfo += RefreshInfo;
        // mainCamera.aspect = 9f / 16f;
        Camera.main.aspect = 9f / 16f;

        BossGroup.SetActive(false);

    }


    IEnumerator Start() {

        for (int i = 0; i < ListEquipSlot.Count; i++) {
            ListEquipSlot[i].InitEquipSlot(i);
        }

        LoadStageMemory(); // 스테이지 정보(스테이지번호, 킬 카운트)

        while (!MergeSystem.isInitialized)
            yield return null;

        LoadEquipUnitPosition();

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

    #region 획득 코인 처리 
    

    /// <summary>
    /// 수입 초기화
    /// </summary>
    public void InitTotalEarningCoin() {
        TotalEarningCoin = 0;
    }

    void RepeatingEarning() {
        // 0.5초마다 한번씩?
        // EarningCoin의 10분의 1씩 누적 계산

        TotalEarningCoin += (long)(EarningCoin * 0.5f);
    }

    /// <summary>
    /// 미니언 킬 코인 받기 
    /// </summary>
    public void GetMinionKillCoin() {
        PlayerInfo.main.AddCoin(TotalEarningCoin);
        TotalEarningCoin = 0;
    }


    /// <summary>
    /// 획득 코인 계산 및 UI 처리 
    /// </summary>
    public void RefreshEarningCoin() {

        EarningCoin = 0;

        for(int i=0; i<ListBP.Count; i++) {
            if (!ListBP[i].isOccufied)
                continue;

            EarningCoin += long.Parse(ListBP[i].unitData._earning);
        }

        if (EarningCoin == 0)
            TextEarningCoin.text = string.Empty;
        else
            TextEarningCoin.text = PIER.GetBigNumber(EarningCoin) + " / sec";

        

    }

    #endregion


    IEnumerator PlayRoutine() {

        // 보스 및 미니언 소환 여부 체크 
        CurrentEnemy = GetNewEnemy(false);

        InvokeRepeating("RepeatingEarning", 0, 0.5f);


        while (true) {
            yield return null;

            if(IsBossMode) {
                continue;

            }


            if(CurrentEnemy == null) {
                CurrentEnemy = GetNewEnemy(false);

            }

        }
    }

    #region 보스 처리 

    /// <summary>
    /// 보스 모드 진입 
    /// </summary>
    public void CallBoss() {

        // CurrentEnemy.de
        ViewBossWarning.warningBossID = 1;
        Doozy.Engine.GameEventMessage.SendEvent("BossWarningEvent"); // 보스 등장 UI 처리 

        IsBossMode = true; // 보스모드 진입 

        StartCoroutine(BossRoutine());
    }

    IEnumerator BossRoutine() {

        float timervalue = 0;

        while (BossWarningView.activeSelf) {
            yield return null;
        }

        yield return null;

        // 관련 UI 오픈! 및 연출 
        BossGroup.SetActive(true);
        progressorBossHP.transform.localScale = new Vector3(0, 1, 1);
        BossSkull.transform.localScale = Vector3.zero;
        ButtonSurrender.transform.localScale = Vector3.zero;
        progressorBossHP.SetValue(1);
        progressorBossTimer.SetValue(1);
        progressorBossTimer.gameObject.SetActive(false); // Y : -28 
        progressorBossTimer.transform.localPosition = new Vector3(0, -55, 0);

        progressorBossHP.transform.DOScale(new Vector3(1, 1, 1), 0.2f).OnComplete(OnTweenBossHP);
        progressorBossTimer.gameObject.SetActive(true);
        progressorBossTimer.transform.DOLocalMoveY(-28, 0.5f);





        if (CurrentEnemy)
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

    void OnTweenBossHP() {
        BossSkull.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBounce);
        ButtonSurrender.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBounce).SetDelay(0.2f);
    }


    /// <summary>
    /// 보스 HP 바 설정 
    /// </summary>
    /// <param name="hp"></param>
    /// <param name="maxhp"></param>
    public void SetValueBossHP(long hp, long maxhp) {

        if(hp <= 0) {
            progressorBossHP.SetValue(0);
            return;
        }

        bosshpvalue = (decimal)hp / (decimal)maxhp;
        progressorBossHP.SetValue((float)bosshpvalue);
    }

    /// <summary>
    /// 보스 클리어 되고 나서...
    /// </summary>
    public void ClearBoss() {

        Debug.Log(">> Called ClearBoss << ");

        // UI
        Doozy.Engine.GameEventMessage.SendEvent("BossClearEvent"); // 보스 Clear
        HoldFire = true;

    }


    /// <summary>
    /// 
    /// </summary>
    public void OnCompleteBossClearEvent() {

        Debug.Log(">> OnCompleteBossClearEvent <<");

        HoldFire = false;
        IsBossMode = false;

        // 스테이지 완료 처리
        // Map UI 호출해서...

        // 2스테이지 진입
        Stage++;
        SaveStageMemory();

        CleanBossPhase();

    }


    /// <summary>
    /// 보스 페이즈 끝나고 원상복구 
    /// </summary>
    public void CleanBossPhase() {
        BossGroup.SetActive(false);
        RestoreKillCount();

    }

    #endregion


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
            e.InitBoss(1, 100 * StageData.Instance.Rows[Stage - 1]._factor * 10); // 보스 10배.
        }
        else {
            // 미니언 생성 
            e = Instantiate(Stock.main.ObjectMinion, new Vector3(2.6f, 2.4f, 0), Quaternion.identity).GetComponent<EnemyInfo>();
            e.InitMinion(Random.Range(1, 8), Random.Range(80,100) * StageData.Instance.Rows[Stage-1]._factor);
        } 
       
        return e;

    }


    /// <summary>
    /// 패시브정보 Refresh
    /// </summary>
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

    /// <summary>
    /// 킬카운트 감소 
    /// </summary>
    public void DecreaseKillCount() {
        KillCount--;

        if(KillCount < 0) {
            KillCount = 0;
            return;
        }

        

        SetKillCountText(KillCount);
        SaveStageMemory();

                    
    }

    /// <summary>
    /// 보스 실패 후 킬카운트 복원 
    /// </summary>
    public void RestoreKillCount() {
        KillCount = MaxKillCount;

        SetKillCountText(KillCount);
        SaveStageMemory();
    }

    /// <summary>
    /// 
    /// </summary>
    public void OnClickCallBoss() {
        CallBoss();
        ButtonCallBoss.enabled = false;

    }


    #region Equip Slot, Battle Position Data Save & Load


    /// <summary>
    /// 배틀포지션 초기화
    /// </summary>
    public void InitEquipUnitPosition() {

        // Battle과 관련된 변수는 ListEqiupSlot과 ListBP 두가지가 있다. 
        for(int i=0; i<6; i++) {
            ListEquipSlot[i].gameObject.SetActive(false);
        }

        // 활성가능 개수만큼만 활성화 
        for(int i=0; i<PlayerInfo.GetAvailableBattleSpot();i++) {
            ListEquipSlot[i].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// BattlePosition 위치 로드 
    /// </summary>
    public void LoadEquipUnitPosition() {

        // Debug.Log(">> LoadEquipUnitPosition :: " + ListBP.Count);
        InitEquipUnitPosition();

        int id;
        MergeItem item;

        for(int i=0; i<ListBP.Count;i++) {
            id = PlayerPrefs.GetInt(KeyBattlePosition + i.ToString(), -1);

            if (id <= 0)
                continue;

            // id값이 존재하는 경우 Equip 처리를 해야한다.
            item = MergeSystem.main.FindMergeItemByIncrementalID(id);
            if(item != null) {
                SetEquipUnit(item, false);
            }
        }


        RefreshEarningCoin();
    }

    /// <summary>
    /// BattlePosition 저장
    /// </summary>
    public void SaveEquipUnitPosition() {
        for(int i=0; i < ListBP.Count; i++) {

            if (ListBP[i].isOccufied) // 점유되었는지 여부만 판단.
                PlayerPrefs.SetInt(KeyBattlePosition + i.ToString(), ListBP[i].MergeIncrementalID); //!! level을 저장하는게 아니고 MergeIncrementalID를 저장한다!
            else
                PlayerPrefs.SetInt(KeyBattlePosition + i.ToString(), -1);
        }

        PlayerPrefs.Save();
    }


    /// <summary>
    /// 유닛 전투위치로!
    /// </summary>
    /// <param name="u"></param>
    public void SetEquipUnit(MergeItem item, bool autoSave = true) {
        BattlePosition bp = GetBattlePosition(item.unitRow);
        item.SetBattle(true);

        bp.unitData = item.unitRow;
        Unit equipUnit = Instantiate(Stock.main.ObjectUnit, bp.pos, Quaternion.identity, BattleField).GetComponent<Unit>();

        equipUnit.SetBattleOrder(bp.order);
        equipUnit.SetUnit(item.unitRow._level);
        bp.SetUnit(equipUnit, item);

        

        RefreshEquipSlot();
        RefreshEarningCoin();

        if(autoSave)
            PIER.SaveAll();

    }


    /// <summary>
    /// Middle 장착 슬롯 리프레쉬
    /// </summary>
    void RefreshEquipSlot() {
        int equipUnitCount = 0;
        equipUnitCount = GetEquipUnitCount();

        // 먼저 다 초기화하고
        for(int i=0; i<ListEquipSlot.Count; i++) {
            ListEquipSlot[i].UnEquipUnit();
        }

        // 장착 처리 
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
            if(ListBP[i].MergeIncrementalID == item.MergeIncrementalID) {

                item.SetBattle(false); // 전투중 아님!

                ListBP[i].CleanUnit(); // 포지션에서 제거!

                RefreshEquipSlot();

                PIER.SaveAll();
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


        SetKillCountText(KillCount);
    }

    void SetKillCountText(int k) {

        if (k <= 0)
            progressorKillCount.SetValue(1);
        else
            progressorKillCount.SetValue((float)KillCount / (float)MaxKillCount);

        if (k <= 0) {
            TextKillCount.text = "DESTROY BOSS!!";
            ButtonCallBoss.enabled = true;
        }
        else {
            TextKillCount.text = "Destroy " + k.ToString() + " Enemies";
            ButtonCallBoss.enabled = false;
        }
    }

    /// <summary>
    /// 스테이지 진행도 저장 
    /// </summary>
    public void SaveStageMemory() {
        PlayerPrefs.SetInt(KeyStage, Stage);
        PlayerPrefs.SetInt(KeyKillCountg, KillCount);
        PlayerPrefs.Save();
    }

    #endregion


}
