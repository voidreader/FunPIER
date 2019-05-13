using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Message {
    NoMoreMove, // 이동할거리 없음 
    AskCleanItemUse,
    AskBackItemUse,
    InGameQuit,
    RedMoonOpen,
    RedMoonArrange, 
    GameStartWatch,
    AdRemove, 
    ShareReward,
    ItemGet,
    NeedLv3,
    PuchaseComplete,
    AlreadyHaveNoAds

}

public class PageMessage : UILayer {

    public Message currentMessage;
   
    public Action OnYes = delegate { };

    public UIButton btnYes, btnNo, btnOK;
    public UILabel lblMessage;

    public override UILayer Init(UILayerEnum type, Transform parent, Action pOpen, Action pClose) {
        return base.Init(type, parent, pOpen, pClose);
    }



    /// <summary>
    /// 메세지 세팅 
    /// </summary>
    /// <param name="m"></param>
    public void SetMessage(Message m, string arg1 = "", string arg2 = "") {
        currentMessage = m;

        switch(currentMessage) {
            case Message.NoMoreMove:
                lblMessage.text = PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT26);
                SetSingleButton();
                break;

            case Message.AskCleanItemUse:
                lblMessage.text = PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT30) + "\n" + PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT31);
                SetDoubleButton();
                break;

            case Message.AskBackItemUse:
                lblMessage.text = PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT32) + "\n" + PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT31);
                SetDoubleButton();
                break;

            case Message.InGameQuit:
                lblMessage.text = PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT33);
                SetDoubleButton();
                break;

            case Message.RedMoonOpen:
                lblMessage.text = PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT42) + "\n\n" + PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT41);
                SetDoubleButton();
                break;

            case Message.RedMoonArrange:
                lblMessage.text = PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT42) + "\n\n" + PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT43);
                SetDoubleButton();
                break;

            case Message.GameStartWatch:
                lblMessage.text = string.Format(PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT44), InGame.main.currentThemeStep);
                SetDoubleButton();
                break;

            case Message.AdRemove:
                lblMessage.text = PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT45);
                SetSingleButton();
                break;

            case Message.ShareReward:
                lblMessage.text = PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT47);
                SetDoubleButton();
                break;

            case Message.ItemGet:
                Debug.Log(">> Iten Get #1 :: " + PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT48));
                Debug.Log(">> Iten Get #3 :: " + arg1);
                Debug.Log(">> Iten Get #3 :: " + arg2);

                lblMessage.text = string.Format(PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT48), arg1, arg2);
                SetSingleButton();
                break;

            case Message.NeedLv3:
                lblMessage.text = PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT52);
                SetSingleButton();
                break;

            case Message.PuchaseComplete:
                lblMessage.text = PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT55);
                SetSingleButton();
                break;

            case Message.AlreadyHaveNoAds:
                lblMessage.text = PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT54);
                SetSingleButton();
                break;
        }
    }

    public void SetSingleButton() {
        btnNo.gameObject.SetActive(false);
        btnYes.gameObject.SetActive(false);
        btnOK.gameObject.SetActive(true);
    }


    public void SetDoubleButton() {
        btnNo.gameObject.SetActive(true);
        btnYes.gameObject.SetActive(true);
        btnOK.gameObject.SetActive(false);
    }

    public void OnClickYES() {

        if (LobbyManager.isAnimation)
            return;

        Close();

        OnYes();
        OnYes = delegate { };

        
    }


}
