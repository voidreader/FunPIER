using UnityEngine;
using System.Collections;

public class MessageInput : MessageBase<MessageInput>
{
    public static MessageInput show()
    {
        return RPGSceneManager.Instance.pushPopup<MessageInput>("Prefabs/Popup/ETC/MessageInput");
    }

    RPGTextMesh label_Title;
    tk2dUITextInput input_Text;

    public string Text
    {
        get
        {
            return input_Text.Text;
        }
    }

    public override void init()
    {
        base.init();

        label_Title = getTransform().Find("Message/TextGroup/Text_Title").GetComponent<RPGTextMesh>();
        input_Text = getTransform().Find("Message/InputGroup/Input_Text").GetComponent<tk2dUITextInput>();
    }

    /// <summary>
    /// 메세지를 표시합니다.
    /// </summary>
    /// <param name="text">Text.</param>
    public void setMessage(string text)
    {
        setMessage(text, 30 / GameConfig.PixelsPerMeter);
    }

    /// <summary>
    /// 메시지 텍스트박스 사이즈 추가.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="_fontSize"></param>
    public void setMessage(string text, float _fontSize)
    {
        label_Title.Size = _fontSize;
        label_Title.Text = text;
    }

    /// <summary>
    /// 메시지 텍스트박스 정렬 추가.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="_fontSize"></param>
    /// <param name="align"></param>
    public void setMessage(string text, float _fontSize, RPGTextMesh.TEXT_ALIGNMENT align)
    {
        label_Title.Size = _fontSize;
        label_Title.Textalignment = align;
        label_Title.Text = text;
    }


}
