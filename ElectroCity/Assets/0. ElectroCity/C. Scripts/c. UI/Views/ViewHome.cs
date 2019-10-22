using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewHome : MonoBehaviour
{

    BottomButtonCtrl _buttonHome;

    public void OnView() {

        // 홈 베뉴 활성화..
        _buttonHome.SetActive(true);

        MergeSystem.main.SetMergeSpotMemory(); // 위치 정보 불러와서 설정 

    }
}
