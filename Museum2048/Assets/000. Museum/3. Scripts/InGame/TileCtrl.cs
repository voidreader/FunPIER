using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;
using DG.Tweening;
     

/// <summary>
/// 타일!
/// </summary>
public class TileCtrl : MonoBehaviour {

    public int id = 0; // 보유 숫자 2,4,8,16,32.. 
    [SerializeField] private Chip _chip = null;
    [SerializeField] private Chip _movingChip = null;


    public Chip mergeChip = null;
    Vector3 chipPos = Vector3.zero;

    public GameObject _carTile;
    public GameObject _wineTile;
    public GameObject _vikingTile;

    #region Properties
    public Chip chip {
        get { return _chip; }
        set {
            _chip = value;
        }
    }
    public Chip movingChip { get => _movingChip; set => _movingChip = value; }
    #endregion

    // Start is called before the first frame update
    void Start() {
        
    }

    /// <summary>
    /// 타일 초기화
    /// </summary>
    public void Init() {

        _chip = null;
        _movingChip = null;
        mergeChip = null;

        chipPos = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        SetMaterial();
    }

    /// <summary>
    /// 칩의 초기화 
    /// </summary>
    public void ChipInit() {
        if (chip == null)
            return;

        id = chip.id;
        chip.Init(this.transform);
        
    }

    public void RotateChip() {
        if (chip == null)
            return;

        chip.transform.DOLocalRotate(new Vector3(0, 360, 0), 2, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
    }

    

    /// <summary>
    /// Chip Merge 
    /// </summary>
    public void MergeChip() {
        if (chip == null || mergeChip == null)
            return;

        if(chip.id != mergeChip.id) {
            Debug.Log("Not Equal Chip ids :: " + this.gameObject.name);
            return;
        }


        id = chip.id + mergeChip.id;

        PoolManager.Pools[ConstBox.poolIngame].Despawn(chip.transform);
        PoolManager.Pools[ConstBox.poolIngame].Despawn(mergeChip.transform);

        chip = PoolManager.Pools[ConstBox.poolIngame].Spawn(InGame.GetSpawnChipPrefabName(id)).GetComponent<Chip>();
        ChipInit();

        InGame.main.MergeCheck(id); // Merge 발생할때마다 호출

    }

    /// <summary>
    /// 아이템에 의한 단일 칩 1단계 업그레이드 
    /// </summary>
    public void UpgradeChip() {
        if (!chip)
            return;

        id = chip.id * 2; // id를 두배로 올려준다. 
        PoolManager.Pools[ConstBox.poolIngame].Despawn(chip.transform);

        chip = PoolManager.Pools[ConstBox.poolIngame].Spawn(InGame.GetSpawnChipPrefabName(id)).GetComponent<Chip>();
        ChipInit();

        InGame.main.mergeShotCheck = false;
        InGame.main.MergeCheck(id);

        InputManager.main.LockUIClose(); // 입력 막기.

        InGame.main.SetScores();


    }

    /// <summary>
    /// Admin 용도, 최종 칩으로 강제 업그레이드
    /// </summary>
    public void UpgradeFinal() {
        if (!chip)
            return;


        id = PierSystem.GetIDByStep(PierSystem.main.GetMaxProgress(InGame.currentTheme));
        PoolManager.Pools[ConstBox.poolIngame].Despawn(chip.transform);

        chip = PoolManager.Pools[ConstBox.poolIngame].Spawn(InGame.GetSpawnChipPrefabName(id)).GetComponent<Chip>();
        ChipInit();
        InGame.main.MergeCheck(id);

        InputManager.main.LockUIClose(); // 입력 막기.
    }

    public Vector3 GetChipPosition() {

        return chipPos;
    }

    public Vector3 GetChipPosition(int id) {
        return new Vector3(chipPos.x, chipPos.y + Chip.GetChipY(id), chipPos.z);
    }


    public void SetMaterial() {

        _carTile.gameObject.SetActive(false);
        _wineTile.gameObject.SetActive(false);
        _vikingTile.gameObject.SetActive(false);

        if(LobbyManager.main.currentTheme == Theme.Car) {
            _carTile.gameObject.SetActive(true);
        }
        else if (LobbyManager.main.currentTheme == Theme.Wine) {
            _wineTile.gameObject.SetActive(true);
        }
        else if(LobbyManager.main.currentTheme == Theme.Viking) {
            _vikingTile.gameObject.SetActive(true);
        }
    }

    


    
}
