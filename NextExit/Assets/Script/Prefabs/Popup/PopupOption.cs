using UnityEngine;
using System.Collections;

public class PopupOption : RPGLayer
{

    public static PopupOption show()
    {
        return RPGSceneManager.Instance.pushPopup<PopupOption>("Prefabs/Popup/PopupOption");
    }

    tk2dUIToggleButton m_ToggleBGM;
    tk2dUIToggleButton m_ToggleSE;
    tk2dUIToggleButton m_TogglePush;

    public override void init()
    {
        base.init();

        m_ToggleBGM = getTransform().Find("BTN/btn_BGM").GetComponent<tk2dUIToggleButton>();
        m_ToggleSE = getTransform().Find("BTN/btn_SE").GetComponent<tk2dUIToggleButton>();
        m_TogglePush = getTransform().Find("BTN/btn_Push").GetComponent<tk2dUIToggleButton>();
    }

    void OnBtnClose()
    {
        removeFromParent();
    }

    void OnBtnTerms()
    {

    }

    void OnBtnPrivacy()
    {

    }

    void OnToggleBGM(tk2dUIToggleButton toggle)
    {

    }

    void OnToggleSE(tk2dUIToggleButton toggle)
    {

    }

    void OnTogglePush(tk2dUIToggleButton toggle)
    {

    }


}
