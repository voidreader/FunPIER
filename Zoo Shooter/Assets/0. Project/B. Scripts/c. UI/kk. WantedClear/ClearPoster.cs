using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine;

public class ClearPoster : MonoBehaviour
{
    public Image poster;

    public void OnView() {
        // 이미 CurrentList가 증가된 상태이기 때문에, -1을 한다. 
        poster.sprite = Stocks.GetPosterSprite(PIER.CurrentList-1);
    }

    public void OnClickPoster() {
        GameEventMessage.SendEvent("CallMain");
    }
}
