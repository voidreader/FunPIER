using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupReview : RPGLayer
{
	public static PopupReview show()
	{
		return RPGSceneManager.Instance.pushPopup<PopupReview>( "Prefabs/Popup/PopupReview" );
	}


	public tk2dBaseSprite bg_black;
	public Transform tranInfo;
	public List<tk2dUIItem> buttomList = new List<tk2dUIItem>();

	private bool m_action = false;

	public RPGTextMesh textM;

	public override void init()
	{
		textM.Text = DefineMessage.getMsg( 10012 );
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
		if ( m_action )
			return;
#if UNITY_ANDROID
		if ( GameConfig.Market == GameConfig.eMarketType.Google )
			Application.OpenURL( "market://details?id=pier.rpg.android.nextexit" );
		else if ( GameConfig.Market == GameConfig.eMarketType.OnStore )
			Application.OpenURL( "onestore://common/product/OA00719075?view_type=1" );
#elif UNITY_IOS 
		Application.OpenURL( "https://itunes.apple.com/us/app/nextexit/id1272683032?l=ko&ls=1&mt=8" );
#endif
		GameManager.Instance.SetReview( true );
		m_action = true;
		StartCoroutine( C_EndAction() );
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
