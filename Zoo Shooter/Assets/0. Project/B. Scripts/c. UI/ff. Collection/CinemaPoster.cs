using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CinemaPoster : MonoBehaviour
{

    // 시네마 포스터. 
    // 
    public int _posterID;
    public Image _image;
    public bool _isOpen = false;

    public void SetPoster(int id) {
        _posterID = id;
        _image.sprite = Stocks.GetPosterSprite(_posterID);
        _isOpen = true;
    }


    /// <summary>
    /// 커밍 순 (비활성화 처리)
    /// </summary>
    public void SetComingSoon() {
        _image.sprite = Stocks.main.SpriteComingSoon;
        _isOpen = false;

    }
}
