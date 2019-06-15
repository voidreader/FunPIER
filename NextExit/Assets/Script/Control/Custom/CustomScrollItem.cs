using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Dictionary Data
/// "selected" = 선택상태
/// "id" = 아이템 아이디
/// </summary>
public class CustomScrollItem : UIScrollPrefab
{
    public class ItemData
    {
        public string ItemID;
        public bool IsSelected;

        public ItemData(string _ItemID, bool _IsSelected)
        {
            ItemID = _ItemID;
            IsSelected = _IsSelected;
        }
    }

    GameObject m_GoToggleOn;
    GameObject m_GoToggleOff;

    tk2dSprite m_SprIcon;

    public override void ObjFind()
    {
        base.ObjFind();

        m_GoToggleOn = transform.Find("Btn/img_bg/on").gameObject;
        m_GoToggleOff = transform.Find("Btn/img_bg/off").gameObject;

        m_SprIcon = transform.Find("Btn/icon").GetComponent<tk2dSprite>();

        m_GoToggleOn.SetActive(false);
        m_GoToggleOff.SetActive(true);
    }

    public override void Parsing(object data_)
    {
        base.Parsing(data_);

        ItemData data = data_ as ItemData;

        m_SprIcon.SetSprite(data.ItemID);

        m_GoToggleOn.SetActive(data.IsSelected);
        m_GoToggleOff.SetActive(!data.IsSelected);

        /*
        Dictionary<string, object> dic = data_ as Dictionary<string, object>;

        ItemID = dic["id"].ToString();
        m_textItem.Text = ItemID;

        IsSelect = (bool)dic["selected"];
        */
    }

}
