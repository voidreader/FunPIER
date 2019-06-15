using UnityEngine;
using System.Collections;

public class MessageBase<T> : RPGLayer where T : MessageBase<T>
{

    //protected tk2dUILayout Layout;

    protected tk2dUIItem button_Yes;
    protected tk2dUIItem button_No;
    protected tk2dUIItem button_Done;
    protected tk2dUIItem button_Close;

    protected tk2dSprite image_Yes;
    protected tk2dSprite image_No;
    protected tk2dSprite image_Done;

    protected System.Action<T> YesEvent = null;
    protected System.Action<T> NoEvent = null;
    protected System.Action<T> DoneEvent = null;
    protected System.Action<T> CloseEvent = null;

    public override void init()
    {
        base.init();

        //Layout = getTransform().FindChild("Message").GetComponent<tk2dUILayout>();

        button_Yes = getTransform().Find("Message/BtnGroup/button_Yes").GetComponent<tk2dUIItem>();
        button_No = getTransform().Find("Message/BtnGroup/button_No").GetComponent<tk2dUIItem>();
        button_Done = getTransform().Find("Message/BtnGroup/button_Done").GetComponent<tk2dUIItem>();
        button_Close = getTransform().Find("Message/BtnGroup/button_Close").GetComponent<tk2dUIItem>();

        image_Yes = getTransform().Find("Message/BtnGroup/button_Yes/image").GetComponent<tk2dSprite>();
        image_No = getTransform().Find("Message/BtnGroup/button_No/image").GetComponent<tk2dSprite>();
        image_Done = getTransform().Find("Message/BtnGroup/button_Done/image").GetComponent<tk2dSprite>();

        button_Yes.gameObject.SetActive(false);
        button_No.gameObject.SetActive(false);
        button_Done.gameObject.SetActive(false);
        button_Close.gameObject.SetActive(false);
    }


    /// <summary>
    /// Yes 버튼을 추가합니다.
    /// </summary>
    /// <param name="text">Text.</param>
    /// <param name="dele">Dele.</param>
    public void addYesButton(System.Action<T> dele = null)
    {
        button_Done.gameObject.SetActive(false);
        button_Yes.gameObject.SetActive(true);

        YesEvent = dele;
    }

    /// <summary>
    /// No 버튼을 추가합니다.
    /// </summary>
    /// <param name="text">Text.</param>
    /// <param name="dele">Dele.</param>
    public void addNoButton(System.Action<T> dele = null, bool WithClose=false)
    {
        button_Done.gameObject.SetActive(false);
        button_No.gameObject.SetActive(true);

        NoEvent = dele;
        if (WithClose)
            addCloseButton(dele);
    }

    /// <summary>
    /// 확인 버튼을 추가합니다.
    /// </summary>
    /// <param name="text">Text.</param>
    /// <param name="dele">Dele.</param>
    public void addDoneButton(System.Action<T> dele = null)
    {
        button_Yes.gameObject.SetActive(false);
        button_No.gameObject.SetActive(false);
        button_Done.gameObject.SetActive(true);

        DoneEvent = dele;
    }

    public void addCloseButton(System.Action<T> dele = null)
    {
        button_Close.gameObject.SetActive(true);

        CloseEvent = dele;
    }

    public void Close()
    {
        removeFromParent();
    }

    public void OnButtonNo()
    {
        if (NoEvent != null)
            NoEvent(this as T);
        else
            removeFromParent();
    }

    public void OnButtonYes()
    {
        if (YesEvent != null)
            YesEvent(this as T);
        else
            removeFromParent();
    }

    public void OnButtonDone()
    {
        if (DoneEvent != null)
            DoneEvent(this as T);
        else
            removeFromParent();
    }

    public void OnButtonClose()
    {
        if (CloseEvent != null)
            CloseEvent(this as T);
        else
            removeFromParent();
    }
}
