using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine;
public class WantedClear : MonoBehaviour
{

    public Text _lblName;

    public void OnStartView() {
       // 아직 +1 안되어있기 때문에 +1 해준다. 
        _lblName.text = "RECORD " + (PIER.CurrentList+1).ToString();
    }

    public void OnView() {
        GameEventMessage.SendEvent("CallCurrentPoster");
    }
}
