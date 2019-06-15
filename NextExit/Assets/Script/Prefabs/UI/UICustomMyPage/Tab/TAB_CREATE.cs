using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TAB_CREATE : TAB_Base
{

    public override void init()
    {
        base.init();
    }

    public override void LoadAll(bool IsReset)
    {
        base.LoadAll(IsReset);

        List<object> scroll_list = new List<object>();
        List<int> InventoryList = DataSaveManager.Instance.InventoryList;
        //Dictionary<int, object> InventoryData = DataSaveManager.Instance.InventoryData;
        for (int i = 0; i < InventoryList.Count; i++)
        {
            ItemMapIcon.ItemData item = getScrollItem(InventoryList[i]);
            if (item.CreatorID.Equals(NENetworkManager.Instance.UserID))
                scroll_list.Add(item);
        }
        scroll_list.Add(new ItemMapIcon.ItemData());
        m_Scroll.Init(scroll_list);
    }

    protected ItemMapIcon.ItemData getScrollItem(int index)
    {
        //List<int> InventoryList = DataSaveManager.Instance.InventoryList;
        Dictionary<int, object> InventoryData = DataSaveManager.Instance.InventoryData;
        //int index = InventoryList[scroll_index];
        Dictionary<string, object> dic = InventoryData[index] as Dictionary<string, object>;
        ItemMapIcon.ItemData icon = new ItemMapIcon.ItemData(index,
            ParsingDictionary.ToString(dic, DataSaveManager._KEY_SERVER_INDEX),
            ParsingDictionary.ToString(dic, DataSaveManager._KEY_MAKE_TIME),
            ParsingDictionary.ToString(dic, DataSaveManager._KEY_MAP_NAME),
            ParsingDictionary.ToString(dic, DataSaveManager._KEY_CREATOR_ID),
            NENetworkManager.Instance.NickName,
            0, 0, 0, 0, "", 0, 0, false, false);
        icon.MapData = ParsingDictionary.ToString(dic, DataSaveManager._KEY_GAME_DATA);
        return icon;
    }

    protected override void showMapInfo(bool isShow)
    {
        base.showMapInfo(isShow);

        if (isShow)
        {
            m_ItemUpload.gameObject.SetActive(false);
            m_ItemModify.gameObject.SetActive(false);
            m_ItemDelete.gameObject.SetActive(false);

            ItemMapIcon.ItemData icon = getData();
            Dictionary<string, object> dic = DataSaveManager.Instance.InventoryData[icon.Index] as Dictionary<string, object>;
            bool isClear = ParsingDictionary.ToBool(dic, DataSaveManager._KEY_IS_CLEAR);
            bool isUpload = ParsingDictionary.ToString(dic, DataSaveManager._KEY_SERVER_INDEX).Length > 0;
            if (!isUpload)
            {
                if (isClear)
                {
                    m_ItemUpload.gameObject.SetActive(true);
                }
                m_ItemModify.gameObject.SetActive(true);
                m_ItemDelete.gameObject.SetActive(true);
            }
            //m_ItemDelete.gameObject.SetActive(true);
        }
    }

    protected override void OnBtnPlay()
    {
        MessageBox box = MessageBox.show();
        box.setMessage(DefineMessage.getMsg(30009));
        box.addYesButton((b) =>
        {
            ItemMapIcon.ItemData icon = getData();

            DataSaveManager.Instance.ModifyIndex = icon.Index;

            BlockManager.Instance.loadCustom(icon.MapData);
            GameManager.Instance.startInGameReady(GameManager.ePlayMode.Custom);

            b.Close();
        });
        box.addNoButton();
        box.addCloseButton();
    }

    protected override void OnBtnModify()
    {
        MessageBox box = MessageBox.show();
        box.setMessage(DefineMessage.getMsg(30007));
        box.addYesButton((b) =>
            {
                ItemMapIcon.ItemData icon = getData();

                DataSaveManager.Instance.ModifyIndex = icon.Index;

                GameManager.Instance.startCustom(false);
                ArrayList blockList = BlockManager.MapDataToList(icon.MapData);
                for (int i = 0; i < blockList.Count; i++)
                {
                    Dictionary<string, object> dic = blockList[i] as Dictionary<string, object>;
                    string id = ParsingDictionary.ToString(dic, "id");
                    int x = ParsingDictionary.ToInt(dic, "x");
                    int y = ParsingDictionary.ToInt(dic, "y");

                    BlockManager.Instance.createBlock(id, x, y);
                }

                b.Close();
            });
        box.addNoButton();
        box.addCloseButton();
    }

    protected override void OnBtnDelete()
    {
        MessageBox box = MessageBox.show();
        box.setMessage(DefineMessage.getMsg(30005));
        box.addYesButton((b) =>
            {
                ItemMapIcon.ItemData icon = getData();

                DataSaveManager.Instance.Delete(icon.Index);

                LoadAll(true);
                m_SelectIndex = -1;

                b.Close();
            });
        box.addNoButton();
        box.addCloseButton();

    }

    protected override void OnBtnUpload()
    {
        ItemMapIcon.ItemData icon = getData();

        int cost = int.Parse(RPGSheetManager.Instance.getSheetData("data.bin", "cost_upload").ToString());
        MessageBox box = MessageBox.show();
        box.setMessage(DefineMessage.getMsg(30006, cost.ToString("N0")));
        box.addYesButton((b) =>
        {
            b.Close();
            if (CheckGold(cost))
            {
                DataSaveManager.Instance.Upload(icon.Index, (w) =>
                {
                    // 해당 데이터를 다시 불러옵니다.
                    ItemMapIcon.ItemData new_icon = getScrollItem(icon.Index);
                    new_icon.IsSelected = true;
                    m_Scroll.setData(m_SelectIndex, new_icon);
                    showMapInfo(true);
                });
            }
        });
        box.addNoButton();
        box.addCloseButton();
    }


}
