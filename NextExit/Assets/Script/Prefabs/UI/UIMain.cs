using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIMain : RPGLayer {

    public static UIMain show()
    {
        return RPGSceneManager.Instance.pushScene<UIMain>("Prefabs/UI/UIMain");
    }

    GameObject m_btn_CustomEditor;

    public override void init()
    {
        base.init();

        m_btn_CustomEditor = getTransform().Find("BTN/btn_CustomEditor").gameObject;
#if UNITY_EDITOR
        // 에디터 모드에서만 노출됩니다.
        m_btn_CustomEditor.SetActive(true);
#else
        m_btn_CustomEditor.SetActive(false);
#endif
    }

    /// <summary>
    /// 스테이지 미션 시작.
    /// </summary>
    void OnBtnStart()
    {
        GameManager.Instance.StartStage();
    }

    void OnBtnOption()
    {
        PopupOption.show();
    }

    void OnBtnRecord()
    {
        /*
        PopupRate.show((rate) =>
        {

        });
        */
    }

    void OnBtnCustom()
    {
        UICustomMain.show();
    }

    void OnBtnShop()
    {
        UIShop.show();
    }

    void OnBtnCustomEditor()
    {
        UIEditorStage.show();
    }

    
}
