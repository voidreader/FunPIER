using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup_RemoveAds : RPGLayer
{
	public static Popup_RemoveAds show()
	{
		return RPGSceneManager.Instance.pushPopup<Popup_RemoveAds>( "Prefabs/Popup/Popup_RemoveAds" );
	}

	public tk2dBaseSprite bg_black;
	public Transform tranInfo;
	private bool m_action = false;

	public RPGTextMesh textBuy_1;
	public RPGTextMesh textBuy_2;
	public RPGTextMesh textBuy_3;
	public RPGTextMesh textBuy_4;
	public RPGTextMesh textBuy_5;


    public RPGTextMesh title;

    public override void init()	{


        foreach(GoogleProductTemplate tpl in AndroidInAppPurchaseManager.Client.Inventory.Products) {
            if(tpl.SKU == "nextexit_001")
                textBuy_1.Text = tpl.LocalizedPrice;
        }

        // textBuy_1.Text = IapSample.Instance.getPriceString("0910087219");

        title.Text = DefineMessage.getMsg(40007);


        StartCoroutine( C_StartAction() );
	}
	private IEnumerator C_StartAction()
	{
		m_action = true;
		float time = 0.1f;
		float timer = 0f;
		bg_black.gameObject.SetActive( true );
		Color startColor = new Color( 0f, 0f, 0f, 1f );
		Color endColor = new Color( 0f, 0f, 0f, 0f );

		while ( true )
		{
			timer += Time.deltaTime;
			bg_black.color = Color.Lerp( startColor, endColor, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}
		bg_black.gameObject.SetActive( false );
		m_action = false;
	}


	void OnBtnClose()
	{
		RPGSoundManager.Instance.PlayUISound( 3 );
		OnPopupClose();
	}

	private void BuyPurchase( string _id )	{

        AndroidInAppPurchaseManager.Client.Purchase("nextexit_001");


	}
	public void OnButton1Click()
	{
        if(Application.isEditor) {
            AndroidManager.GetInstance.OnConsumeEditor();
            return;
        }


        BuyPurchase("nextexit_001");
    }

	public void OnButton2Click()
	{

        if (Application.isEditor) {
            AndroidManager.GetInstance.OnConsumeEditor();
            return;
        }
        BuyPurchase("nextexit_001");
    }

	public void OnButton3Click()
	{

        if (Application.isEditor) {
            AndroidManager.GetInstance.OnConsumeEditor();
            return;
        }
        BuyPurchase("nextexit_001");
    }

	public void OnButton4Click()
	{

        if (Application.isEditor) {
            AndroidManager.GetInstance.OnConsumeEditor();
            return;
        }
        BuyPurchase("nextexit_001");
    }

	public void OnButton5Click()
	{

        if (Application.isEditor) {
            AndroidManager.GetInstance.OnConsumeEditor();
            return;
        }
        BuyPurchase("nextexit_001");
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
		m_action = true;
		float time = 0.1f;
		float timer = 0f;
		bg_black.gameObject.SetActive( true );
		Color startColor = new Color( 0f, 0f, 0f, 0f );
		Color endColor = new Color( 0f, 0f, 0f, 1f );

		while ( true )
		{
			timer += Time.deltaTime;
			bg_black.color = Color.Lerp( startColor, endColor, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}
		tranInfo.gameObject.SetActive( false );

		while ( true )
		{
			timer += Time.deltaTime;
			bg_black.color = Color.Lerp( endColor, startColor, timer / time );
			if ( timer >= time )
				break;
			yield return null;
		}

		removeFromParent();
	}
}
