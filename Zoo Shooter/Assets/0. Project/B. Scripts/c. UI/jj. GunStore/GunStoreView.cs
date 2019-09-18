using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


using DanielLochner.Assets.SimpleScrollSnap;

public class GunStoreView : MonoBehaviour
{
    public static GunStoreView main = null;

    
    public List<GunGroup> _listGroups;
    public GunGroup _currentGroup;
    public int CurrentGroupIndex = 0;
    public int PreviousGroupIndex = -1;

    [SerializeField] List<GunColumn> _listUnlock; // 언락용도 

    public SimpleScrollSnap _sc;
    public Image _EquipWeaponSprite;


    public GameObject _bottoms; // unlock과 광고... 

    public Text _lblUnlock; // 가격표
    public Button _btnAds; // 광고보고 코인받기... 
    public Button _btnUnlock;

    public Vector2 debugVector2;
    public bool isUnlocking = false; // 언락 연출중.
    public GameObject lockCover;

    private void Awake() {
        main = this;
    }



    public void OnView() {

        int startPanel = 0;
        CurrentGroupIndex = -1;
        PreviousGroupIndex = -1;

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


        // _sc.GoToPanel(startPanel);
        StartCoroutine(GoRightPanel(startPanel));
        lockCover.SetActive(false);
                
    }

    IEnumerator GoRightPanel(int p) {
        // yield return new WaitForSeconds(0.1f);
        yield return null;
        _sc.GoToPanel(p);
    }


    public void OnSelectedFocusItem() {
        // Debug.Log("GunStore OnSelectedFocusItem :: " + _sc.TargetPanel + "/" + _sc.CurrentPanel);
        
        
    }

    public void OnPanelVectorChanged(Vector2 v) {
        debugVector2 = v;

        if(CurrentGroupIndex != _sc.TargetPanel) {

            PreviousGroupIndex = CurrentGroupIndex;
            CurrentGroupIndex = _sc.TargetPanel;

            _currentGroup = _listGroups[CurrentGroupIndex];

            // 다른 경우에만 동작 
            BottomRoutine();
        }

        

    }

    /// <summary>
    /// 
    /// </summary>
    public void SetEquipWeapon() {
        _EquipWeaponSprite.sprite = Stocks.GetWeaponStoreSprite(PIER.main.CurrentWeapon);
        _EquipWeaponSprite.SetNativeSize();

        // 재장전 사운드 
        AudioAssistant.Shot(PIER.main.CurrentWeapon.ReloadSound);
    }

    /// <summary>
    /// 광고 버튼 활성화 여부 
    /// </summary>
    void SetAdButton() {
        // _btnAds.gameObject.SetActive(AdsManager.main.IsAvailableRewardAD());
        _btnAds.gameObject.SetActive(true);
    }

    void BottomRoutine() {

        // 현재 뷰의 모든 무기를 수집했으면 _bottoms 뜨지 않음
        List<GunColumn> lll = _currentGroup._listCols;
        bool hasAllGroupWeapon = true;

        // 광고버튼 활성화 처리
        SetAdButton();

        for(int i=0;i< lll.Count;i++) {
            if (!lll[i].gameObject.activeSelf)
                continue;

            if (!lll[i].HasThisGun()) {
                hasAllGroupWeapon = false;
                break;
            }
        }

        // 다 가졌네. return
        if(hasAllGroupWeapon) {
            _bottoms.transform.DOScale(0, 0.2f);
            return;
        }

        if(_listGroups[CurrentGroupIndex]._groupType == WeaponGetType.Unlock250) {
            _bottoms.transform.DOScale(1, 0.2f);
            _lblUnlock.text = "250";

            if(PIER.main.Coin < 250) {
                _btnUnlock.GetComponent<Image>().color = new Color(1, 1, 1, 0.6f);
            }
            else {
                _btnUnlock.GetComponent<Image>().color = Stocks.main.ColorOrigin;
            }

        }
        else if(_listGroups[CurrentGroupIndex]._groupType == WeaponGetType.Unlock500) {
            _bottoms.transform.DOScale(1, 0.2f);
            _lblUnlock.text = "500";

            if (PIER.main.Coin < 500) {
                _btnUnlock.GetComponent<Image>().color = new Color(1, 1, 1, 0.6f);
            }
            else {
                _btnUnlock.GetComponent<Image>().color = Stocks.main.ColorOrigin;
            }

        }
        else {
            _bottoms.transform.DOScale(0, 0.2f);
        }


        /*
        if(CurrentGroupIndex == 0) {
            
        }
        else if(CurrentGroupIndex == 1 || CurrentGroupIndex == 2) {
            
        }
        else {

        }
        */

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


    /// <summary>
    /// 잠금해제 처리 
    /// </summary>
    public void UnlockRandom() {

        int consumeCoin = 250;

        if (_currentGroup._groupType == WeaponGetType.Unlock500)
            consumeCoin = 500;

        if (_currentGroup._groupType == WeaponGetType.Unlock250 && PIER.main.Coin < 250)
            return;

        if (_currentGroup._groupType == WeaponGetType.Unlock500 && PIER.main.Coin < 500)
            return;


        _listUnlock = new List<GunColumn>();
        

        for(int i=0; i<_currentGroup._listCols.Count;i++) {
            // 보유하지 않은 것만. 
            if (_currentGroup._listCols[i].gameObject.activeSelf && !_currentGroup._listCols[i].HasThisGun())
                _listUnlock.Add(_currentGroup._listCols[i]); // Add.


        }

        if (_listUnlock.Count == 0)
            return;

        // 돌린다!
        StartCoroutine(UnlockRoutine(consumeCoin));

    }

    IEnumerator UnlockRoutine(int consumeCoin) {

        float interval = 0.1f;
        GunColumn gc = null;

        isUnlocking = true;
        lockCover.SetActive(true);

        if (_listUnlock.Count == 1) { // 남은 개수가 1개인 경우. 
            gc = _listUnlock[0];
            gc.SetUnlockSelect(true);

            yield return new WaitForSeconds(1);
        }
        else {

            for (int i = 0; i < 18; i++) {

                _currentGroup.SetInactiveUnlockSelect(); // 모두 해제하고.. 
                gc = _listUnlock[Random.Range(0, _listUnlock.Count)];
                gc.SetUnlockSelect(true);


                if (i > 6)
                    interval *= 1.1f;

                yield return new WaitForSeconds(interval);
            }
        }

        // 연출 및 획득처리
        if (gc) {
            gc.transform.DOScale(1.1f, 0.2f).SetLoops(4, LoopType.Yoyo);
            PIER.main.AddGun(gc._weapon);
            gc.SetGunProduct(gc._weapon);
            gc.OnClickProduct();
            PIER.main.AddCoin(-consumeCoin);
        }


        isUnlocking = false;
        lockCover.SetActive(false);
    }


    /// <summary>
    /// 
    /// </summary>
    public void OnClickWatchAD() {


        if (Application.internetReachability == NetworkReachability.NotReachable) {
            PIER.SetNotReachInternetText();
            return;
        }

        if (!AdsManager.main.IsAvailableRewardAD()) {
            PIER.SetNotAvailableAdvertisement();
            return;
        }



        Debug.Log("GunStore Watch AD Click");
        AdsManager.main.OpenRewardAd(OnCallbackAD);
    }

    void OnCallbackAD() {
        Debug.Log("GunStore OnCallbackAD");
        PIER.main.AddCoin(35);
        SetAdButton(); // 버튼 활성화 처리 


        RefreshUnlockCoinButtonState();
    }

    void RefreshUnlockCoinButtonState() {
        if (_listGroups[CurrentGroupIndex]._groupType == WeaponGetType.Unlock250) {
            _bottoms.transform.DOScale(1, 0.2f);
            _lblUnlock.text = "250";

            if (PIER.main.Coin < 250) {
                _btnUnlock.GetComponent<Image>().color = new Color(1, 1, 1, 0.6f);
            }
            else {
                _btnUnlock.GetComponent<Image>().color = Stocks.main.ColorOrigin;
            }

        }
        else if (_listGroups[CurrentGroupIndex]._groupType == WeaponGetType.Unlock500) {
            _bottoms.transform.DOScale(1, 0.2f);
            _lblUnlock.text = "500";

            if (PIER.main.Coin < 500) {
                _btnUnlock.GetComponent<Image>().color = new Color(1, 1, 1, 0.6f);
            }
            else {
                _btnUnlock.GetComponent<Image>().color = Stocks.main.ColorOrigin;
            }

        }
    }
}
