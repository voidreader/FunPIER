using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


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
    public void SetComingSoon(int id) {
        _posterID = id;
        _image.sprite = Stocks.main.SpriteComingSoon;
        _isOpen = false;

    }

    public void OpenPoster() {
        if (!_isOpen)
            return;

        CollectionManager.main.OpenBigPoster(_posterID);


    }

    public void FocusPoster() {
        Debug.Log("Focus Poster!");
        this.transform.DOScale(new Vector3(0.6f, 0.6f, 1), 0.3f).SetLoops(4, LoopType.Yoyo);
    }
}

