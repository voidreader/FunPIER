using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TAB_SHOP_HISTORY : RPGLayer
{
    RPGScroll m_Scroll;

    public override void init()
    {
        base.init();

        if (!NENetworkManager.Instance.IsLogin)
            return;

        m_Scroll = getTransform().Find("SCROLL/scroll/Controller").GetComponent<RPGScroll>();

        rpgames.game.RequestShopHistory request = new rpgames.game.RequestShopHistory();
        {
            request.request = WebConnector.Instance.CommonRequest;
            request.page = 1;
            request.sort = rpgames.game.RequestShopHistory.Sort.CreateDate;
            request.sort_direction = rpgames.game.RequestShopHistory.SortDirection.Desc;
        }
        WebObject wo = new WebObject((w) =>
        {
            rpgames.game.ResponseShopHistory response = w.getResult<rpgames.game.ResponseShopHistory>();
            if (response.response.success)
            {
                List<rpgames.game.ResponseShopHistory.History> list = response.list;
                List<object> scroll_list = new List<object>();
                for (int i = 0; i < list.Count; i++)
                {
                    rpgames.game.ResponseShopHistory.History history = list[i];

                    ItemShopHistoryIcon.ItemData icon = new ItemShopHistoryIcon.ItemData(history, false);
                    scroll_list.Add(icon);
                }
                m_Scroll.Init(scroll_list);
            }
            else
            {
                Debug.LogError("shop_history Error = " + response.response.error_code.ToString());
            }

        });
        wo.setData<rpgames.game.RequestShopHistory>(request);
        wo.setCommand("shop_history");
        WebConnector.Instance.request(wo);
    }

}
