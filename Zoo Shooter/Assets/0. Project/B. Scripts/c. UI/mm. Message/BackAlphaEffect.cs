using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BackAlphaEffect : MonoBehaviour
{
    public Image BackGround;


    private void Start() {
        
    }

    public void MakeDark() {

        BackGround = this.GetComponent<Image>();

        if (BackGround) {
            Debug.Log("MakeDark Called");
            BackGround.color = Stocks.main.ColorTransparent;
            BackGround.DOFade(0.7f, 0.5f).SetDelay(0.4f);
        }

    }

}
