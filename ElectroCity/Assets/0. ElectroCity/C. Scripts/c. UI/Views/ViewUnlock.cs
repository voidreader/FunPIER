using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Google2u;
using Doozy.Engine.Progress;
    

public class ViewUnlock : MonoBehaviour
{
    public Transform Title;
    public Text TextUnitName, TextDPS;

    public GameObject ButtonBack;
    public Progressor PowerBar, RPMBar;

    public GameObject Next;

    public Image SpriteUnlockUnit, SpriteNextUnit;
    TweWeapon EquipWeapon;

    public BigAura Aura;



    public static UnitDataRow unlockUnit = null;

    public void OnView() {

        

        // 연출 준비 
        // 크기 연출
        Title.localScale = Vector3.zero;
        
        SpriteUnlockUnit.transform.localScale = Vector3.zero;


        // 위치 연출 
        TextUnitName.transform.localPosition = new Vector3(0, 340, 0);
        Next.transform.localPosition = new Vector2(0, -320);
        ButtonBack.transform.localPosition = new Vector2(0, -520);


        // 유닛 정보 세팅 
        TextUnitName.text = unlockUnit._displayname;
        SpriteUnlockUnit.sprite = Stock.GetFriendlyUnitUI(unlockUnit._spriteUI);
        // SpriteUnlockUnit.SetNativeSize();
        // SpriteUnlockUnit.transform.localScale *= 2;

        TextDPS.text = Unit.GetDPS(unlockUnit, Stock.GetFriendlyUnitWeapon(unlockUnit._weaponid));


        // 연출 시작 
        Title.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        Aura.PlayAura();


        SpriteUnlockUnit.transform.DOScale(2, 0.4f).SetEase(Ease.OutBack).SetDelay(0.1f);

        TextUnitName.transform.DOLocalMoveY(390, 0.4f);
        ButtonBack.transform.DOLocalMoveY(-480, 0.4f);



        Next.transform.DOLocalMoveY(-280, 0.4f);

    }





}
