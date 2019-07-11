using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine;

public class ClearPoster : MonoBehaviour
{
    public Image poster;

    public void OnView() {

        poster.sprite = Stocks.GetPosterSprite(PIER.CurrentList);
    }

    public void OnClickPoster() {
        GameEventMessage.SendEvent("CallMain");
    }
}
