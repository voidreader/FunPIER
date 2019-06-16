using DG.Tweening;
using System;
using System.Collections.Generic;
using Toolkit;
using UnityEngine;
using UnityEngine.UI;

// ANCHOR FIXME 아이템 상점 추가
public class ItemStorePanel : GameShow
{
	private bool m_Tweener;

	[SerializeField]
    private Text m_Money;
    [SerializeField]
    private Text[] m_Price;

	private int m_addMoney;
    private int m_moneyNum;

	[SerializeField]
	private HomePanel m_HomePanel;

    [SerializeField]
    private GameObject buyPopup;

	public override void Init()
	{
		base.Init();
        this.m_inEase = Ease.OutBack;
		this.m_outEase = Ease.InBack;
	}

	public override void Open(Action callBack = null, float dealy = 0f)
	{
		base.Open(callBack, dealy);

        int i;
        for(i = 0; i < 3; i++) m_Price[i].text     = classicItemPrices[i].ToString();
        for(i = 0; i < 3; i++) m_Price[i + 3].text = challengeItemPrices[i].ToString();
	}

	public override void Close(Action callBack = null, float dealy = 0f)
	{
		base.Close(callBack, dealy);

        if(GameUIManager.Instance.IsHomeState() == false)
            GamePlayManager.Instance.GamePlay();
	}

	public override void Refresh()
	{
		base.Refresh();

		//this.m_moneyNum = MonoSingleton<GameDataManager>.Instance.Money;
        //this.m_Money.text = string.Format("{0:N0}", this.m_moneyNum);
	}

    public void BuyItem(string itemName)
    {
        UpdateItem(itemName);
    }

    public GameObject itemInfoWindow;
    [SerializeField]
    private Image itemIcon;
    [SerializeField]
    private Text[] infos; // 아이템 설명과 수량 표시
    [SerializeField]
    private Text itemPrice;

    public void CloseItemInfoWindow()
    {
        itemInfoWindow.SetActive(false);
        this.m_HomePanel.Refresh();
    }

    private int[] classicItemPrices = {100, 50, 50};
    private int[] challengeItemPrices = {110, 60, 80};

    int m_PlayMode;
    int m_SelectIndex;
    int m_EnableBuyCount;
    int m_CurrentItemPrice;

    string m_KeyItemName;
    private void UpdateItem(string resName)
    {
        string path = string.Format("Items/{0}", resName);
        switch(resName)
        {
            case "icon_classic_1":
                m_KeyItemName = "ClassicItem1";
                infos[0].text = Localization.Instance.GetLable("ClassicItemInfo1");
                m_PlayMode = 0; m_SelectIndex = 0;
                m_CurrentItemPrice = classicItemPrices[0];
                break;
            case "icon_classic_2":
                m_KeyItemName = "ClassicItem2";
                infos[0].text = Localization.Instance.GetLable("ClassicItemInfo2");
                m_PlayMode = 0; m_SelectIndex = 1;
                m_CurrentItemPrice = classicItemPrices[1];
                break;
            case "icon_classic_3":
                m_KeyItemName = "ClassicItem3";
                infos[0].text = Localization.Instance.GetLable("ClassicItemInfo3");
                m_PlayMode = 0; m_SelectIndex = 2;
                m_CurrentItemPrice = classicItemPrices[2];
                break;
            case "icon_Challenge_1":
                m_KeyItemName = "ChallengeItem1";
                infos[0].text = Localization.Instance.GetLable("ChallengeItemInfo1");
                m_PlayMode = 1; m_SelectIndex = 0;
                m_CurrentItemPrice = challengeItemPrices[0];
                break;
            case "icon_Challenge_2":
                m_KeyItemName = "ChallengeItem2";
                infos[0].text = Localization.Instance.GetLable("ChallengeItemInfo2");
                m_PlayMode = 1; m_SelectIndex = 1;
                m_CurrentItemPrice = challengeItemPrices[1];
                break;
            case "icon_Challenge_3":
                m_KeyItemName = "ChallengeItem3";
                infos[0].text = Localization.Instance.GetLable("ChallengeItemInfo3");
                m_PlayMode = 1; m_SelectIndex = 2;
                m_CurrentItemPrice = challengeItemPrices[2];
                break;
        }

        itemInfoWindow.SetActive(true);
        Sprite sprite = (Resources.Load(path, typeof(Sprite)) as Sprite);
		itemIcon.sprite = sprite;

        m_EnableBuyCount = 1;
        infos[1].text = m_EnableBuyCount.ToString();
        itemPrice.text = m_CurrentItemPrice.ToString();
    }

    public void PlusItemAmount()
    {
        if(m_CurrentItemPrice * (m_EnableBuyCount + 1) > GameDataManager.Instance.Money)
            return;

        m_EnableBuyCount++;
        infos[1].text = m_EnableBuyCount.ToString();
        itemPrice.text = (m_CurrentItemPrice * m_EnableBuyCount).ToString();
    }

    public void MinusItemAmount()
    {
        if(m_EnableBuyCount > 1)
            m_EnableBuyCount--;
        infos[1].text = m_EnableBuyCount.ToString();
        itemPrice.text = (m_CurrentItemPrice * m_EnableBuyCount).ToString();
    }

    public void BuyItemByCoin()
    {
        int totalItemPrice = m_EnableBuyCount * m_CurrentItemPrice;
        if(totalItemPrice > GameDataManager.Instance.Money) 
        {
            GameUIManager.Instance.OpenNoticePanel(Localization.Instance.GetLable("NotEnoughCoin"));

            return;
        }

        GameDataManager.Instance.BrickItemsInfo[m_PlayMode][m_SelectIndex] += m_EnableBuyCount;
        //GamePlayManager.Instance.BrickItemsDic[m_PlayMode][m_SelectIndex].m_Count = GameDataManager.Instance.BrickItemsInfo[m_PlayMode][m_SelectIndex];
        GameDataManager.Instance.saveGamingItems();

        GameDataManager.Instance.AddMoney(-1 * totalItemPrice);
        CloseItemInfoWindow();

        GameUIManager.Instance.OpenNoticePanel(Localization.Instance.GetLable("BuySuccess"));
    }

    public void BuyItemByFreeAD()
    {
#if UNITY_EDITOR
        GameDataManager.Instance.BrickItemsInfo[m_PlayMode][m_SelectIndex] += 1;
        GameDataManager.Instance.saveGamingItems();
        CloseItemInfoWindow();

        GameUIManager.Instance.OpenNoticePanel(Localization.Instance.GetLable(m_KeyItemName) + " " + 
            Localization.Instance.GetLable("GetItem"));

        return;
#endif
        if (!AdManager.Instance.IsRewardLoaded())
        {
            MonoSingleton<GameUIManager>.Instance.OpenGameOverWatch(null, 0f);
            return;
        }
        AdManager.Instance.ShowReward((success) =>
        {
            if (success)
            {
                GameDataManager.Instance.BrickItemsInfo[m_PlayMode][m_SelectIndex] += 1;
                GameDataManager.Instance.saveGamingItems();
                CloseItemInfoWindow();

                GameUIManager.Instance.OpenNoticePanel(Localization.Instance.GetLable(m_KeyItemName) + " " + 
                    Localization.Instance.GetLable("GetItem"));
            }
        });
    }

    public void CloseBuyResultPopup()
    {
        buyPopup.SetActive(false);
    }
}
