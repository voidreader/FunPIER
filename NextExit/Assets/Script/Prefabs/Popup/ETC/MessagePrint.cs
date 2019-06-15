using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessagePrint : RPGLayer {


    RPGTextMesh m_LabelMessage;

    public static MessagePrint show(string msg)
    {
        MessagePrint obj = RPGSceneManager.Instance.pushPopup<MessagePrint>("Prefabs/Popup/ETC/MessagePrint");
        obj.setMessage(msg);
        return obj;
    }
    /*
    public static MessagePrint show(int msg_id)
    {
        Dictionary<int, string> message_list = new Dictionary<int, string>()
        {
            {10001, "해당 위치에 이미 배치된 블럭이 있습니다."},
            {10002, "블럭을 놓을 수 없는 위치 입니다."},
            {10003, "입구는 한개만 배치할 수 있습니다."},
            {10004, "출구는 한개만 배치할 수 있습니다."},
            {10005, "입구를 배치해야 합니다."},
            {10006, "출구를 배치해야 합니다."}
        };

        return show(message_list[msg_id]);
    }
    */

    public override void init()
    {
        base.init();
        if (m_LabelMessage == null)
            m_LabelMessage = transform.Find("RPGTextMesh").GetComponent<RPGTextMesh>();
    }

    public void setMessage(string msg)
    {
        m_LabelMessage.Text = msg;
    }

    void OnEnd()
    {
        Invoke("removeFromParent", 0.1f);
    }

    

}
