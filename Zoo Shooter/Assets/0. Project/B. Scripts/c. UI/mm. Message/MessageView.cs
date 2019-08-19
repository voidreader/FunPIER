using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine;

public class MessageView : MonoBehaviour
{
    public static MessageView main = null;

    public Text _text;

    private void Awake() {
        main = this;
    }

    public static void SetText(string m) {
        main._text.text = m;
        GameEventMessage.SendEvent("MessageEvent");
    }

    


}
