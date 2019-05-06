using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;


public enum UILayerEnum {
    Message,
    Store,
    Option,
    Lang,
    Progress,
    SurprisePack,
    Tutorial
}

public class UILayer : MonoBehaviour {

    public UILayerEnum _pageType;
    public Action OnClose = delegate { }; // 종료 후 콜백
    public Action OnOpen = delegate { };  // 오픈 후 콜백

    UISprite _frame = null;
    UISprite _bg = null;
    UISprite[] _sprites;

    Vector3 _lowerPos = new Vector3(0, -30, 0);

    public virtual UILayer Init(UILayerEnum type, Transform parent, Action pOpen, Action pClose) {


        OnClose = pClose;
        OnOpen = pOpen;
        _pageType = type;

        this.transform.SetParent(parent);

        // Push
        if (PageManager.main != null)
            PageManager.main.pageStack.Push(this);

        this.gameObject.SetActive(true);
        SetSlidePosition();

        return this;
    }

    /// <summary>
    /// 슬라이드 오픈 
    /// </summary>
    void SetSlidePosition() {
        LobbyManager.isAnimation = true;
        FindBasicObject();

        _frame.transform.localPosition = _lowerPos;

        this.gameObject.SetActive(true);

        _frame.transform.DOLocalMoveY(0, 0.22f).OnComplete(OnCompleteInit);

    }


    /// <summary>
    /// 기본 오브젝트 찾기
    /// </summary>
    void FindBasicObject() {
        if(_frame == null) {
            _sprites = this.GetComponentsInChildren<UISprite>(true);

            for (int i = 0; i < _sprites.Length; i++) {
                if (_sprites[i].name == "Frame")
                    _frame = _sprites[i];

            }
        }
    }


    void OnCompleteInit() {
        LobbyManager.isAnimation = false;

        OnOpen();
        OnOpen = delegate { };
    }


    /// <summary>
    /// 창 닫기
    /// </summary>
    public void Close() {
        if (LobbyManager.isAnimation) {
            Debug.Log("Close is stopped. isAnimation is true");
            return;
        }

        AudioAssistant.Shot("Negative");

        // 스택 체크 후 스택에서 제거
        if(PageManager.main != null && PageManager.main.pageStack.Contains(this)) {
            PageManager.main.pageStack.Pop();
        }


        OnClosing();

        this.gameObject.SetActive(false);

        // LobbyManager.isAnimation = false;

        OnClose();
        OnClose = delegate { };

    }

    IEnumerator Closing() {
        yield return null;
    }


    /// <summary>
    /// 닫히기 직전 추가로직
    /// </summary>
    void OnClosing() {

    }
}

