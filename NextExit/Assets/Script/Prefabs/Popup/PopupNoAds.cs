﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupNoAds : RPGLayer
{
	public static PopupNoAds show( Dictionary<string, object> dic )
	{
		return RPGSceneManager.Instance.pushPopup<PopupNoAds>( "Prefabs/Popup/PopupNoAds", dic );
	}


	public tk2dBaseSprite bg_black;
	public Transform tranInfo;
	public List<tk2dUIItem> buttomList = new List<tk2dUIItem>();

	private bool m_action = false;

	public GameObject obj;

	public RPGTextMesh textM;

	public override void init( Dictionary<string, object> dic )
	{
		obj = ( GameObject )dic["GameObject"];
		textM.Text = DefineMessage.getMsg( 10011 );
		base.init();
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

		removeFromParent();
	}

	void OnBtnContinue()
	{
		/*
		UnityIAP.Instance.BuyPurchase( "nextexit01",
		( product_id ) =>
		{
			AdvertisingManager.Instance.SetAdsOn( false );
			obj.SendMessage( "OnAdsButton" );
			// 인앱 결제 성공.
			if ( m_action )
				return;
			m_action = true;
			StartCoroutine( C_EndAction() );

		},
		( product, e ) =>
		{
			// 인앱 결제 실패.

		} );
		*/
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
}