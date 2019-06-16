using DG.Tweening;
using System;
using System.Collections.Generic;
using Toolkit;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

public class StorePanel : GameShow
{
	[SerializeField]
	private GameObject m_SkinRoot;

	[SerializeField]
	private Skin m_Skin;

	[SerializeField]
	private GameObject m_Hit;

	[SerializeField]
	private GameObject m_HitText;

	private Dictionary<SkinType, Skin> m_SkinList = new Dictionary<SkinType, Skin>();

	private bool m_Tweener;

	[SerializeField]
    private Text m_Money;
	private int m_addMoney;
    private int m_moneyNum;
	
	[SerializeField]
	private HomePanel m_HomePanel;

	public override void Init()
	{
		base.Init();
		this.m_inEase = Ease.OutBack;
		this.m_outEase = Ease.InBack;
		//==this.m_Hit.gameObject.SetActive(false);
		// foreach (SkinInfo current in MonoSingleton<ConfigeManager>.Instance.Config)
		// {
		// 	Skin skin = UnityEngine.Object.Instantiate<Skin>(this.m_Skin);
		// 	skin.transform.SetParent(this.m_SkinRoot.transform);
		// 	skin.transform.localScale = Vector3.one;
		// 	this.m_SkinList.Add(current.Type, skin);
		// 	skin.Init(current.StorTexture, current.StoreUseIcon, current.Type, current.Price, current.Hexcolor, MonoSingleton<GameDataManager>.Instance.SkinState(current.Type));
		// }
	}

	public override void Open(Action callBack = null, float dealy = 0f)
	{
		// Vector3 localPosition = this.m_SkinRoot.transform.localPosition;
		// localPosition.y = 0f;
		// this.m_SkinRoot.transform.localPosition = localPosition;
		// if (this.m_ShowList.Count != 0)
		// {
		// 	this.m_ShowList[0].m_StartValue = new Vector3(0f, 0f, 0f);
		// 	this.m_ShowList[0].m_EndValue = new Vector3(0f, -1000f, 0f);
		// }
		base.Open(callBack, dealy);
	}

	public override void Close(Action callBack = null, float dealy = 0f)
	{
		// if (this.m_ShowList.Count != 0)
		// {
		// 	this.m_ShowList[0].m_StartValue = new Vector3(0f, 0f, 0f);
		// 	this.m_ShowList[0].m_EndValue = new Vector3(0f, -this.m_SkinRoot.transform.localPosition.y - 568f, 0f);
		// }

		base.Close(callBack, dealy);
	}

	public override void Refresh()
	{
		base.Refresh();
		// foreach (SkinType current in this.m_SkinList.Keys)
		// {
		// 	this.m_SkinList[current].SkinState(MonoSingleton<GameDataManager>.Instance.SkinState(current));
		// }

		this.m_moneyNum = MonoSingleton<GameDataManager>.Instance.Money;
        this.m_Money.text = string.Format("{0:N0}", this.m_moneyNum);
	}

	public void ShowHit()
	{
		if (this.m_Tweener)
		{
			return;
		}
		this.m_Tweener = true;
		this.m_Hit.gameObject.SetActive(true);
		this.m_HitText.transform.DOKill(false);
		this.m_HitText.transform.localScale = Vector3.zero;
		this.m_HitText.transform.DOScale(Vector3.one, 0.4f).OnComplete(delegate
		{
			this.m_HitText.transform.DOScale(Vector3.zero, 0.4f).OnComplete(delegate
			{
				this.m_Hit.gameObject.SetActive(false);
				this.m_Tweener = false;
			}).SetDelay(1f);
		});
	}
	// 100, 550, 1150
	void BuyCoin(int index)
	{
		int[] m_GetMoney = {100, 550, 1150};
		GameDataManager.Instance.AddMoney(m_GetMoney[index]);

		this.Refresh();
		this.m_HomePanel.Refresh();

		GameUIManager.Instance.OpenNoticePanel("Coin " + m_GetMoney[index] + " " +
            Localization.Instance.GetLable("GetSuccess"));
	}

	public void BuyCoinFreeAD()
	{

	}

	public void BuyComplete(UnityEngine.Purchasing.Product product)
	{
		Debug.Log("_____ BuyComplete _____ " + product.metadata.localizedPriceString);

		if(product.definition.id == "coin_100")  BuyCoin(0);
		else if(product.definition.id == "coin_550")  BuyCoin(1);
		else if(product.definition.id == "coin_1150") BuyCoin(2);
		else 
		{
			GameManager.Instance.AdBlock(true);
			PlayerPrefs.SetInt("AD", 1);
            PlayerPrefs.Save();
			AdManager.IsRemoveAds = true;
			AdManager.Instance.HideBanner();
		}
	}

	public void BuyFailed(UnityEngine.Purchasing.Product product, UnityEngine.Purchasing.PurchaseFailureReason failureReason)
	{

	}

	public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs e)
	{
		bool validPurchase = true; // Presume valid for platforms with no R.V.

		// Unity IAP's validation logic is only included on these platforms.
	#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
		// Prepare the validator with the secrets we prepared in the Editor
		// obfuscation window.
		var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
			AppleTangle.Data(), Application.identifier);

		try {
			// On Google Play, result has a single product ID.
			// On Apple stores, receipts contain multiple products.
			var result = validator.Validate(e.purchasedProduct.receipt);
			// For informational purposes, we list the receipt(s)
			Debug.Log("Receipt is valid. Contents:");
			foreach (IPurchaseReceipt productReceipt in result) {
				Debug.Log(productReceipt.productID);
				Debug.Log(productReceipt.purchaseDate);
				Debug.Log(productReceipt.transactionID);
			}
		} catch (IAPSecurityException) {
			Debug.Log("Invalid receipt, not unlocking content");
			validPurchase = false;
		}
	#endif

		if (validPurchase) {
			// Unlock the appropriate content here.
		}

		return PurchaseProcessingResult.Complete;
	}
}
