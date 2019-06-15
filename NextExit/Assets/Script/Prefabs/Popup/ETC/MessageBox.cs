using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageBox : MessageBase<MessageBox>
{
    public static MessageBox show()
    {
        return RPGSceneManager.Instance.pushPopup<MessageBox>("Prefabs/Popup/ETC/MessageBox");
    }

	public tk2dBaseSprite bg_black;
	public Transform tranInfo;
	public List<tk2dUIItem> buttomList = new List<tk2dUIItem>();

    RPGTextMesh label_Message;

	bool m_action = false;

    public override void init()
    {
        base.init();

        label_Message = getTransform().Find("Message/TextGroup/Text_Contents").GetComponent<RPGTextMesh>();

		StartCoroutine( C_StartAction() );
    }

	private IEnumerator C_StartAction()
	{
		m_action = true;
		float time = 0.1f;
		float timer = 0f;
		for ( int index = 0; index < buttomList.Count; ++index )
			buttomList[index].enabled = false;
		Color startColor = new Color( 0f, 57f / 255f, 1f, 0f );
		Color endColor = new Color( 0f, 57f / 255f, 1f, 104f / 255f );

		while( true )
		{
			timer += Time.deltaTime;
			bg_black.color = Color.Lerp( startColor, endColor, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}
		timer = 0;
		while ( true )
		{
			timer += Time.deltaTime;
			tranInfo.localScale = Vector3.Lerp( Vector3.zero, Vector3.one, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}
		for ( int index = 0; index < buttomList.Count; ++index )
			buttomList[index].enabled = true;
		m_action = false;
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
        label_Message.Size = _fontSize;
        label_Message.Text = text;
    }

    /// <summary>
    /// 메시지 텍스트박스 정렬 추가.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="_fontSize"></param>
    /// <param name="align"></param>
    public void setMessage(string text, int _fontSize, RPGTextMesh.TEXT_ALIGNMENT align)
    {
        label_Message.Size = _fontSize;
        label_Message.Textalignment = align;
        label_Message.Text = text;
    }
	public void OnCloseClick()
	{
		OnPopupClose();
	}

	public override void OnPopupClose()
	{
		if ( m_action )
			return;
		m_action = true;
		StartCoroutine( C_EndAction() );
	}

	private IEnumerator C_EndAction()
	{
		float time = 0.1f;
		float timer = 0f;
		for ( int index = 0; index < buttomList.Count; ++index )
			buttomList[index].enabled = false;
		Color startColor = new Color( 0f, 57f / 255f, 1f, 0f );
		Color endColor = new Color( 0f, 57f / 255f, 1f, 104f / 255f );

		while ( true )
		{
			timer += Time.deltaTime;
			tranInfo.localScale = Vector3.Lerp( Vector3.one, Vector3.zero, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}
		timer = 0;
		while ( true )
		{
			timer += Time.deltaTime;
			bg_black.color = Color.Lerp( endColor, startColor, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}

		OnButtonClose();
	}
}
