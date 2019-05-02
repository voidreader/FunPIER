using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Message {
    NoMoreMove, // 이동할거리 없음 
    AskCleanItemUse,
    AskBackItemUse,
    InGameQuit,
    RedMoonOpen
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
    public void SetMessage(Message m) {
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
                lblMessage.text = PierSystem.GetLocalizedText(Google2u.MLocal.rowIds.TEXT41);
                SetDoubleButton();
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
        OnYes();
        OnYes = delegate { };

        Close();
    }


}
