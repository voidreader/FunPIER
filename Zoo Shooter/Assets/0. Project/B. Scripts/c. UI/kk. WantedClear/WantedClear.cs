using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine;
public class WantedClear : MonoBehaviour
{

    public Text _lblName;

    public void OnStartView() {
        // 이미 +1된 상태기 때문에 그냥 쓴다. 
       
        _lblName.text = "RECORD " + (PIER.CurrentList).ToString();
    }

    public void OnView() {
        GameEventMessage.SendEvent("CallCurrentPoster");
    }
}
