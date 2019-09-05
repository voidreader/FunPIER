using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Machine", menuName = "Machine")]
public class Machine : ScriptableObject {

    public int Level; // 사실상 ID
    

    public Sprite SpriteMergeUI; // 머지스팟 UI 
    public Sprite SpriteBody,SpriteLeg; // 스테이지 스프라이트 
    public Sprite SpriteFaceIdle, SpriteFaceShoot; // 스테이지 스프라이트의 얼굴

    public long EarningCoinForSec; // 초당 획득코인
    public long AttackPower; // 공격력
    public long DPS; // 초당 데미지  
    public float CoolTime; // 발사 후 쿨타임 


    [TextArea] public string DisplayName;


}
