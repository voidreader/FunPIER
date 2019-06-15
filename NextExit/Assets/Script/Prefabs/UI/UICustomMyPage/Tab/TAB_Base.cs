using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TAB_Base : RPGLayer {

    public enum eDataType
    {
        Server,         // 서버로부터 받은 정보.
        Inventory,      // 인벤토리 정보.
    }

    public GameObject PrefabContent;

    protected string SearchOwner = "";
    protected string SearchKeyword = "";
    // 현재의 마지막 페이지.
    protected int Page = 1;
    protected rpgames.game.RequestMapList.SortDirection SortDirection = rpgames.game.RequestMapList.SortDirection.Desc;
    protected rpgames.game.RequestMapList.Sort Sort = rpgames.game.RequestMapList.Sort.CreateDate;
    // 총 페이지 개수.
    protected int Total_Page = 1;

    protected RPGScroll m_Scroll;
    protected int m_SelectIndex = -1;
    //protected List<object> m_List = new List<object>();

    ItemMapIcon m_PrefabItem;

    protected GameObject m_LayerBottomText;

    /// <summary>
    /// 등록일자.
    /// </summary>
    protected RPGTextMesh m_TextData;
    /// <summary>
    /// 맵이름.
    /// </summary>
    protected RPGTextMesh m_TextMapName;
    /// <summary>
    /// 제작자 아이디.
    /// </summary>
    protected RPGTextMesh m_TextCreatorID;
    /// <summary>
    /// 플레이 횟수.
    /// </summary>
    protected RPGTextMesh m_TextPlayCount;
    /// <summary>
    /// 클리어 횟수.
    /// </summary>
    protected RPGTextMesh m_TextClearCount;
    /// <summary>
    /// 베스트 아이디.
    /// </summary>
    protected RPGTextMesh m_TextBestID;
    /// <summary>
    /// 베스트 시간.
    /// </summary>
    protected RPGTextMesh m_TextBestTime;
    /// <summary>
    /// 베스트 죽은 횟수.
    /// </summary>
    protected RPGTextMesh m_TextBestDeath;

    protected tk2dUIItem m_ItemUpload;
    protected tk2dUIItem m_ItemModify;
    protected tk2dUIItem m_ItemDelete;
    protected tk2dUIItem m_ItemPreview;
    protected tk2dUIItem m_ItemPlay;

    protected Transform m_TransformContent;

    public override void init()
    {
        if (!NENetworkManager.Instance.IsLogin)
            return;

        base.init();

        // 초기화.
        DataSaveManager.Instance.ModifyIndex = 0;

        m_TransformContent = addChild(PrefabContent).transform;
        PrefabContent.SetActive(false);
        m_TransformContent.gameObject.SetActive(true);

        m_LayerBottomText = m_TransformContent.Find("TEXT").gameObject;

        m_TextData = m_TransformContent.Find("TEXT/Content/text_Date").GetComponent<RPGTextMesh>();
        m_TextMapName = m_TransformContent.Find("TEXT/Content/text_MapName").GetComponent<RPGTextMesh>();
        m_TextCreatorID = m_TransformContent.Find("TEXT/Content/text_CreatorID").GetComponent<RPGTextMesh>();
        m_TextPlayCount = m_TransformContent.Find("TEXT/Content/text_PlayCount").GetComponent<RPGTextMesh>();
        m_TextClearCount = m_TransformContent.Find("TEXT/Content/text_ClearCount").GetComponent<RPGTextMesh>();
        m_TextBestID = m_TransformContent.Find("TEXT/Content/text_BestID").GetComponent<RPGTextMesh>();
        m_TextBestTime = m_TransformContent.Find("TEXT/Content/text_BestTime").GetComponent<RPGTextMesh>();
        m_TextBestDeath = m_TransformContent.Find("TEXT/Content/text_BestDeath").GetComponent<RPGTextMesh>();

        m_ItemUpload = m_TransformContent.Find("BTN/btn_UpLoad").GetComponent<tk2dUIItem>();
        m_ItemModify = m_TransformContent.Find("BTN/btn_Modify").GetComponent<tk2dUIItem>();
        m_ItemDelete = m_TransformContent.Find("BTN/btn_Delete").GetComponent<tk2dUIItem>();
        m_ItemPreview = m_TransformContent.Find("BTN/btn_Preview").GetComponent<tk2dUIItem>();
        m_ItemPlay = m_TransformContent.Find("BTN/btn_Play").GetComponent<tk2dUIItem>();

        m_Scroll = m_TransformContent.Find("SCROLL/scroll_mapList/Controller").GetComponent<RPGScroll>();

        m_PrefabItem = m_Scroll.m_prefabitem.GetComponent<ItemMapIcon>();
        m_PrefabItem.ToggleButton.sendMessageTarget = gameObject;
        m_PrefabItem.ToggleButton.SendMessageOnClickMethodName = "OnBtnToggle";
        m_PrefabItem.gameObject.SetActive(false);

        m_ItemUpload.OnClick += OnBtnUpload;
        m_ItemModify.OnClick += OnBtnModify;
        m_ItemDelete.OnClick += OnBtnDelete;
        m_ItemPreview.OnClick += OnBtnPreview;
        m_ItemPlay.OnClick += OnBtnPlay;

        HideContent();
        LoadAll(false);
    }

    void OnDestroy()
    {
    }

    protected void HideContent()
    {
        m_LayerBottomText.SetActive(false);

        m_ItemUpload.gameObject.SetActive(false);
        m_ItemModify.gameObject.SetActive(false);
        m_ItemDelete.gameObject.SetActive(false);
        m_ItemPreview.gameObject.SetActive(false);
        m_ItemPlay.gameObject.SetActive(false);

        m_TextPlayCount.Text = "0";
        m_TextClearCount.Text = "0";
        m_TextBestID.Text = "";
        m_TextBestTime.Text = "00:00:00";
        m_TextBestDeath.Text = "0";
    }

    public virtual void LoadAll(bool IsReset)
    {
        if (IsReset)
        {
            //m_List.Clear();
            m_Scroll.ListClear(false);
            HideContent();
        }
    }

    protected virtual void OnBtnToggle(tk2dUIItem item)
    {
        ItemMapIcon icon = item.GetComponentInParent<ItemMapIcon>();

        if (m_SelectIndex == icon.m_Index)
            return;
        if (m_SelectIndex != -1)
            refreshSelectedItem(m_SelectIndex, false);
        m_SelectIndex = icon.m_Index;
        refreshSelectedItem(m_SelectIndex, true);
        showMapInfo(true);
    }

    protected ItemMapIcon.ItemData getData(int index)
    {
        return m_Scroll.m_allItems[index] as ItemMapIcon.ItemData;
        //return m_List[index] as ItemMapIcon.ItemData;
    }

    protected ItemMapIcon.ItemData getData()
    {
        return getData(m_SelectIndex);
    }

    void refreshSelectedItem(int index, bool selected)
    {
        ItemMapIcon.ItemData data = getData(index);
        data.IsSelected = selected;
        m_Scroll.setData(index, data);
    }

    protected virtual void showMapInfo(bool isShow)
    {
        if (isShow)
        {
            ItemMapIcon.ItemData data = getData(m_SelectIndex);

            System.DateTime date = RPGDefine.convertTimeStampToDateTimeByLanguage(long.Parse(data.CreateTime));

            m_TextData.Text = date.ToString("yyyy.MM.dd");
            m_TextMapName.Text = data.MapName;
            m_TextCreatorID.Text = data.Nickname;

            m_TextPlayCount.Text = data.PlayCount.ToString("N0");
            m_TextClearCount.Text = data.ClearCount.ToString("N0");

            m_TextBestID.Text = data.BestID;
            m_TextBestTime.Text = RPGDefine.SecToString(data.BestTime);
            m_TextBestDeath.Text = data.BestDeath.ToString("N0");

            //m_ItemPreview.gameObject.SetActive(true);
            m_ItemPlay.gameObject.SetActive(true);
        }
        m_LayerBottomText.SetActive(isShow);
    }
    
    protected virtual void OnBtnUpload()
    {
    }

    protected virtual void OnBtnModify()
    {
    }

    protected virtual void OnBtnDelete()
    {
    }

    protected virtual void OnBtnPreview()
    {

    }

    protected virtual void OnBtnPlay()
    {

    }

    /// <summary>
    /// 소지 골드가 부족한지 체크합니다.
    /// </summary>
    /// <param name="cost"></param>
    /// <returns></returns>
    protected bool CheckGold(int cost)
    {
        if (cost > NENetworkManager.Instance.Gold)
        {
            MessageBox box = MessageBox.show();
            box.setMessage(DefineMessage.getMsg(30008));
            box.addYesButton((b) =>
            {
                b.Close();
                UIShop.show();
            });
            box.addNoButton();
            box.addCloseButton();
            return false;
        }
        return true;
    }
}
