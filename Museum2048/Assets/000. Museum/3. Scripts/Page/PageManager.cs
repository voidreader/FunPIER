using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PageManager : MonoBehaviour
{
    public static PageManager main = null;

    public Stack<UILayer> pageStack = new Stack<UILayer>();
    public List<UILayer> pages = new List<UILayer>();

    public Transform pagePanel;

    private void Awake() {
        main = this;
    }

    // Start is called before the first frame update
    void Start() {
        
    }





    public void OpenPage(UILayerEnum p) {
        OpenPage(p, delegate { });
    }

    public void OpenPage(UILayerEnum p, Action onClose) {
        for (int i = 0; i < pages.Count; i++) {
            if (pages[i]._pageType == p) {
                pages[i].Init(p, pagePanel, delegate { }, onClose);
            }
        }
    }


    /// <summary>
    /// 메세지창 오픈 
    /// </summary>
    public void OpenMessage(Message m, Action onClose, string arg1 = "", string arg2 = "") {
        for (int i = 0; i < pages.Count; i++) {
            if(pages[i]._pageType == UILayerEnum.Message) {
                pages[i].Init(UILayerEnum.Message, pagePanel, delegate { }, onClose);
                pages[i].gameObject.GetComponent<PageMessage>().SetMessage(m,arg1, arg2);
            }
        }
    }

    /// <summary>
    /// YES / NO 버튼 메세지창 오픈 
    /// </summary>
    /// <param name="m"></param>
    /// <param name="onYes"></param>
    public void OpenDoubleButtonMessage(Message m, Action onYes, Action onClose, string arg1="", string arg2="") {

        for (int i = 0; i < pages.Count; i++) {
            if (pages[i]._pageType == UILayerEnum.Message) {
                pages[i].Init(UILayerEnum.Message, pagePanel, delegate { }, onClose);
                pages[i].gameObject.GetComponent<PageMessage>().SetMessage(m,arg1,arg2);
                pages[i].gameObject.GetComponent<PageMessage>().OnYes = onYes;
            }
        }
    }

    #region 페이지 오픈

    public void OpenProgress() {
        for(int i=0; i<pages.Count; i++) {
            if(pages[i]._pageType == UILayerEnum.Progress) {
                pages[i].GetComponent<PageProgress>().SetTheme(LobbyManager.main.currentTheme);
                pages[i].Init(UILayerEnum.Progress, pagePanel, delegate { }, delegate { });
                return;
            }
        }
    }

    public void OpenShop() {
        OpenPage(UILayerEnum.Store);
    }

    public void OpenOption() {
        OpenPage(UILayerEnum.Option);
    }

    public void OpenTutorial() {
        OpenPage(UILayerEnum.Tutorial);
    }

    public void OpenLang() {
        OpenPage(UILayerEnum.Lang);
    }
    
    #endregion

}
