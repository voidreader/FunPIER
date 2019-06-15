using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TAB_SHOP_GOLD : RPGLayer {

    RPGScroll m_Scroll;
    int m_SelectIndex = -1;
    List<object> m_List = new List<object>();
    ItemGoldIcon m_PrefabItem;

    tk2dUIItem m_btn_buy;
    RPGTextMesh m_text_desc;

    public override void init()
    {
        base.init();

        if (!NENetworkManager.Instance.IsLogin)
            return;

        m_Scroll = getTransform().Find("SCROLL/scroll/Controller").GetComponent<RPGScroll>();
        m_btn_buy = getTransform().Find("BTN/btn_buy").GetComponent<tk2dUIItem>();
        m_text_desc = getTransform().Find("TEXT/text_desc").GetComponent<RPGTextMesh>();

        showBuy(false);

        m_PrefabItem = m_Scroll.m_prefabitem.GetComponent<ItemGoldIcon>();
        m_PrefabItem.ToggleButton.sendMessageTarget = gameObject;
        m_PrefabItem.ToggleButton.SendMessageOnClickMethodName = "OnBtnToggle";
        m_PrefabItem.gameObject.SetActive(false);

        Dictionary<string, object> cost_dic = RPGSheetManager.Instance.getSheetData("cost.bin");
        foreach (KeyValuePair<string, object> pair in cost_dic)
        {
            if (pair.Key.Equals("key"))
                continue;
            string id = ParsingDictionary.ToString(pair.Value, "id");
            string text = ParsingDictionary.ToString(pair.Value, "text");
            string kind = ParsingDictionary.ToString(pair.Value, "kind");
            string type = ParsingDictionary.ToString(pair.Value, "type");
            string os = ParsingDictionary.ToString(pair.Value, "os");
            float cost = ParsingDictionary.ToFloat(pair.Value, "cost");
            int reward = ParsingDictionary.ToInt(pair.Value, "reward");
            int bonus = ParsingDictionary.ToInt(pair.Value, "bonus");
            if (kind.Equals("inapp") || kind.Equals("ad_reward") || kind.Equals("ad_remove"))
            {
                if (os.Equals("all")
#if UNITY_IPHONE
                || os.Equals("ios")
#else
                || os.Equals("android")
#endif
                )
                {
                    ItemGoldIcon.ItemData item = new ItemGoldIcon.ItemData(id, text, kind, type, os, cost, reward, bonus, false);
                    m_List.Add(item);
                }
            }
        }
        m_Scroll.Init(m_List);
    }

    void OnBtnToggle(tk2dUIItem item)
    {
        ItemGoldIcon icon = item.GetComponentInParent<ItemGoldIcon>();
        Debug.Log("OnBtnToggle : " + icon.m_Index);
        if (m_SelectIndex == icon.m_Index)
            return;
        if (m_SelectIndex != -1)
            refreshSelectedItem(m_SelectIndex, false);
        m_SelectIndex = icon.m_Index;
        refreshSelectedItem(m_SelectIndex, true);
        showBuy(true);
    }

    protected ItemGoldIcon.ItemData getData(int index)
    {
        return m_List[index] as ItemGoldIcon.ItemData;
    }

    protected ItemGoldIcon.ItemData getData()
    {
        return getData(m_SelectIndex);
    }

    void refreshSelectedItem(int index, bool selected)
    {
        ItemGoldIcon.ItemData data = getData(index);
        data.IsSelected = selected;
        m_Scroll.setData(index, data);
    }

    void showBuy(bool isShow)
    {
        m_btn_buy.gameObject.SetActive(isShow);
        m_text_desc.gameObject.SetActive(isShow);

        if (isShow)
        {
            ItemGoldIcon.ItemData data = getData();
            switch (data.Kind)
            {
                case "ad_reward": m_text_desc.Text = DefineMessage.getMsg(30016, data.Reward.ToString("N0")); break;
                case "ad_remove": m_text_desc.Text = DefineMessage.getMsg(30017, data.Cost.ToString("N0")); break;
                case "inapp": m_text_desc.Text = DefineMessage.getMsg(30018, data.Reward.ToString("N0"), data.Bonus.ToString("N0"), (data.Reward + data.Bonus).ToString("N0"), data.Text); break;
            }
        }
    }

    void onBtnBuy()
    {
        ItemGoldIcon.ItemData data = getData();
        switch (data.Kind)
        {
            case "ad_reward":
                break;
            case "ad_remove":
                break;
            case "inapp":
                {
                    rpgames.game.RequestShopBuy request = new rpgames.game.RequestShopBuy();
                    {
                        request.request = WebConnector.Instance.CommonRequest;
                        request.shop_idx = data.ID;
                    }
                    WebObject wo = new WebObject((w) =>
                    {
                        rpgames.game.ResponseShopBuy response = w.getResult<rpgames.game.ResponseShopBuy>();
                        if (response.response.success)
                        {
                            NENetworkManager.Instance.Gold = response.account.gold;
                        }
                        else
                        {
                            Debug.LogError("map_load Error = " + response.response.error_code.ToString());
                        }

                    });
                    wo.setData<rpgames.game.RequestShopBuy>(request);
                    wo.setCommand("shop_buy");
                    WebConnector.Instance.request(wo);
                }
                break;
        }
    }

}
