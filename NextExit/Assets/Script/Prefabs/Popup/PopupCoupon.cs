using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupCoupon : RPGLayer
{
	public static PopupCoupon show()
	{
		return RPGSceneManager.Instance.pushPopup<PopupCoupon>( "Prefabs/Popup/PopupCoupon" );
	}


	public tk2dBaseSprite bg_black;
	public Transform tranInfo;
	public List<tk2dUIItem> buttomList = new List<tk2dUIItem>();

	public RPGTextMesh objNotCode;

	public RPGTextMesh text;

	private bool m_action = false;

	public tk2dUITextInput inputText;

	public override void init()
	{
		base.init();
		objNotCode.Text = DefineMessage.getMsg( 40004 );
		objNotCode.gameObject.SetActive( false );
		text.Text = DefineMessage.getMsg( 40005 );
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

		while ( true )
		{
			timer += Time.unscaledDeltaTime;
			bg_black.color = Color.Lerp( startColor, endColor, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}
		timer = 0;
		while ( true )
		{
			timer += Time.unscaledDeltaTime;
			tranInfo.localScale = Vector3.Lerp( Vector3.zero, Vector3.one, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}
		for ( int index = 0; index < buttomList.Count; ++index )
			buttomList[index].enabled = true;
		m_action = false;
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
			timer += Time.unscaledDeltaTime;
			tranInfo.localScale = Vector3.Lerp( Vector3.one, Vector3.zero, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}
		timer = 0;
		while ( true )
		{
			timer += Time.unscaledDeltaTime;
			bg_black.color = Color.Lerp( endColor, startColor, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}
		GameManager.Instance.LoadChar( DataSaveManager.Instance.CharId );
		removeFromParent();
	}

	public override void OnPopupClose()
	{
		if ( m_action )
			return;
		m_action = true;
		StartCoroutine( C_EndAction() );
	}

	void OnBtnExit()
	{
		if ( m_action )
			return;
		m_action = true;
		StartCoroutine( C_EndAction() );

	}

	public void OnCnfirmClick()
	{
		objNotCode.gameObject.SetActive( false );
		Debug.Log( inputText.Text );
		CheckCoupon( inputText.Text );
		
	}

	private void CheckCoupon( string _coupon )
	{
		ArrayList sheetInfo = RPGSheetManager.Instance.getInfoArray( "NE_COUPON.bin" );

		foreach( Dictionary<string, object> dic in sheetInfo )
		{
			if ( dic["COUPON_ID"].ToString().Equals( _coupon ) )
			{
				DataSaveManager.Instance.CharId = dic["CHARACTER_ID"].ToString();
				MessageBox input = MessageBox.show();
				OnBtnExit();

				//저장된 게임이 존재합니다.\n이어 하시겠습니까?
				input.setMessage( DefineMessage.getMsg( 40003 ) );
				input.addDoneButton( ( m ) =>
				{
					m.OnCloseClick();
				} );
				return;
			}
		}
		objNotCode.gameObject.SetActive( true );
	}
}