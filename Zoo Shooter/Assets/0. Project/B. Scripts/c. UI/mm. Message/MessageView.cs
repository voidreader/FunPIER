using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine;

public class MessageView : MonoBehaviour
{
    public static MessageView main = null;

    public Text _text;

    public GameObject TwoButtonGroup;

    private void Awake() {
        main = this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="m"></param>
    public static void SetText(string m) {

        Debug.Log(">>>> MessageView SetText Called ");
        main._text.text = m;
        GameEventMessage.SendEvent("MessageEvent");

        main.TwoButtonGroup.SetActive(false);
    }

    public static void SetExitText(string m) {
        Debug.Log(">>>> MessageView SetTwoButtonText Called ");
        main._text.text = m;
        main.TwoButtonGroup.SetActive(true);
    }

    

    


}
