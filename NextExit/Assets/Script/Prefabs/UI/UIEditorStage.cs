using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIEditorStage : RPGLayer {

    public static UIEditorStage show()
    {
        return RPGSceneManager.Instance.pushScene<UIEditorStage>("Prefabs/UI/UIEditorStage");
    }

    void OnBtnMain()
    {
        UIMain.show();
    }

    void OnBtnCustomPlay()
    {
        //GameManager.Instance.startPlay();
        BlockManager.Instance.loadCustom();
    }

    void OnBtnCustomEditor()
    {
        GameManager.Instance.startCustom(true);
        //SceneManager.Instance.pushScene(SceneManager.eSceneIndex.Custom);
    }

    /// <summary>
    /// 커스텀맵을 로드하고 에디터 모드를 시작합니다.
    /// </summary>
    void OnBtnCustomEditorAndLoad()
    {
        MessageInput input = MessageInput.show();
        //스테이지 이름을 입력하세요
        input.setMessage(DefineMessage.getMsg(20001));
        input.addYesButton((m) =>
        {
            if (m.Text.Length == 0)
            {
                //스테이지 이름은 반드시 입력해야 합니다.
                MessagePrint.show(DefineMessage.getMsg(20002));
                OnBtnCustomEditorAndLoad();
                return;
            }

            string cryptData = RPGDefine.readStringFromFile(GameConfig._SaveFileFullPath + m.Text + GameConfig._Extention);
            if (cryptData.Length == 0)
            {
                //존재하지 않는 스테이지 입니다.
                MessagePrint.show(DefineMessage.getMsg(20003));
                OnBtnCustomEditorAndLoad();
                return;
            }
            m.Close();
            GameManager.Instance.startCustom(true);

            ArrayList blockList = BlockManager.MapDataToList(cryptData);

            for (int i = 0; i < blockList.Count; i++)
            {
                Dictionary<string, object> dic = blockList[i] as Dictionary<string, object>;
                string id = ParsingDictionary.ToString(dic, "id");
                int x = ParsingDictionary.ToInt(dic, "x");
                int y = ParsingDictionary.ToInt(dic, "y");

                BlockManager.Instance.createBlock(id, x, y);
            }
        });
        input.addNoButton();
        input.addCloseButton();
    }
}
