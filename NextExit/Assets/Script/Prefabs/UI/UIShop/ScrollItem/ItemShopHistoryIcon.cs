using UnityEngine;
using System.Collections;

public class ItemShopHistoryIcon : UIScrollPrefab {

    public class ItemData
    {
        public rpgames.game.ResponseShopHistory.History History;
        public bool IsSelected;

        public ItemData(rpgames.game.ResponseShopHistory.History _History, bool _IsSelected)
        {
            History = _History;
            IsSelected = _IsSelected;
        }
    }
    

    RPGTextMesh m_TextName;
    RPGTextMesh m_TextDate;
    RPGTextMesh m_TextAddGold;

    tk2dUIItem m_ToggleButton;
    public tk2dUIItem ToggleButton
    {
        get
        {
            if (m_ToggleButton == null)
                m_ToggleButton = transform.Find("BTN/btn_Base").GetComponent<tk2dUIItem>();
            return m_ToggleButton;
        }
    }

    public override void ObjFind()
    {
        base.ObjFind();

        m_TextName = transform.Find("TEXT/Content/text_name").GetComponent<RPGTextMesh>();
        m_TextDate = transform.Find("TEXT/Content/text_date").GetComponent<RPGTextMesh>();
        m_TextAddGold = transform.Find("TEXT/Content/text_addgold").GetComponent<RPGTextMesh>();
    }

    public override void Parsing(object data_)
    {
        base.Parsing(data_);

        ItemData data = data_ as ItemData;

        System.DateTime date = RPGDefine.convertTimeStampToDateTimeByLanguage(data.History.insert_date);
        m_TextDate.Text = date.ToString("yyyy.MM.dd") + System.Environment.NewLine + date.ToString("HH:mm:ss");

        if (data.History.pay > 0)
        {
            // 소모됨.
            m_TextName.Text = "Spend";
            m_TextAddGold.Text = "-" + data.History.pay.ToString("N0") + "G";
        }
        else if (data.History.earn > 0)
        {
            // 수입.
            m_TextName.Text = "Revenue";
            m_TextAddGold.Text = "+" + data.History.earn.ToString("N0") + "G";
        }

    }

}
