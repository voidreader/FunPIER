using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICustomMain : RPGLayer
{
    public static UICustomMain show()
    {        
        return RPGSceneManager.Instance.pushScene<UICustomMain>("Prefabs/UI/UICustomMain");
    }

    public override void init()
    {
        base.init();
    }

    void OnBtnStageCustom()
    {
        UICustomMyPage.show(0, 2);
    }

    void OnBtnMyPage()
    {
        UICustomMyPage.show();
    }

    void OnBtnMapList()
    {
        UICustomMyPage.show(1, 0);
    }

    void OnBtnBack()
    {
        UIMain.show();
    }

    
}
