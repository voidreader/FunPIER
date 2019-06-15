using UnityEngine;
using System.Collections;

public class UIDownload : RPGLayer {

    public static UIDownload show()
    {
        return RPGSceneManager.Instance.pushScene<UIDownload>("Prefabs/UI/UIDownload");
    }

    RPGTextMesh m_text_loading;

    public override void init()
    {
        base.init();

        m_text_loading = getTransform().Find("TEXT/text_loading").GetComponent<RPGTextMesh>();

        RPGSheetManager.Instance.AProgressEvent = (progressCount, totalCount) =>
        {
            m_text_loading.Text = string.Format("Downloading Resource... ({0}/{1})", progressCount.ToString(), totalCount.ToString());
        };
        RPGSheetManager.Instance.AStatusEvent = (status) =>
        {
            switch (status)
            {
                case RPGSheetManager.eStatus.Sheet_Downloading:
                    //시트 리스트 다운로드중.

                    break;
                case RPGSheetManager.eStatus.File_Downloading:
                    // 시트 파일 다운로드중.
                    m_text_loading.Text = "Downloading List...";
                    break;
                case RPGSheetManager.eStatus.Done:
                    // 다운로드 완료.
                    //Debug.Log("text.bin = " + (RPGSheetManager.Instance.getSheetData("text.bin", "10001") as Dictionary<string,object>)["korea"].ToString());
                    UILogin.show();
                    break;
            }
        };
        RPGSheetManager.Instance.StartDownload();

    }
}
