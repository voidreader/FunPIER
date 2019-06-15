using UnityEngine;
using System.Collections;

public class UILogin : RPGLayer {

    public static UILogin show()
    {
        return RPGSceneManager.Instance.pushScene<UILogin>("Prefabs/UI/UILogin");
    }

    RPGTextMesh m_text_loading;

    public override void init()
    {
        base.init();

        m_text_loading = getTransform().Find("TEXT/text_loading").GetComponent<RPGTextMesh>();

        NENetworkManager.Instance.request_login((w) =>
        {
            UIMain.show();
        });
    }



}
