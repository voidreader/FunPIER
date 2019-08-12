using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingArmor : MonoBehaviour
{
    public Animator _anim;

    public void PlayBreakingArmor() {
        this.gameObject.SetActive(true);
        _anim.Play("BreakingArmor");
        AudioAssistant.Shot("Breaking");
    }

    public void OnAnim() {
        this.gameObject.SetActive(false);

        GameManager.main.enemy.InitWeaponRotation(); // 적 무기 각도 초기화. (안하면 오류..)
            
    }

}
