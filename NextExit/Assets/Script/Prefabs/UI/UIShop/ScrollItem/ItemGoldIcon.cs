using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemGoldIcon : UIScrollPrefab
{
    public class ItemData
    {
        public string ID;
        public string Text;
        public string Kind;
        public string Type;
        public string OS;
        public float Cost;
        public int Reward;
        public int Bonus;
        public bool IsSelected;

        public ItemData(string _ID, string _Text, string _Kind, string _Type, string _OS, float _Cost, int _Reward, int _Bonus, bool _IsSelected)
        {
            ID = _ID;
            Text = _Text;
            Kind = _Kind;
            Type = _Type;
            OS = _OS;
            Cost = _Cost;
            Reward = _Reward;
            Bonus = _Bonus;
            IsSelected = _IsSelected;
        }
    }

    //tk2dUIToggleButton m_ToggleBase;
    GameObject m_GoToggleOn;
    GameObject m_GoToggleOff;

    RPGTextMesh m_TextName;
    RPGTextMesh m_TextGoldBigReward;
    RPGTextMesh m_TextGoldSmallReward;
    RPGTextMesh m_TextGoldBonus;

    GameObject m_ImgAdReward;
    GameObject m_ImgAdRemove;
    GameObject m_ImgGold;

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

        //m_ToggleBase = MapData.FindChild("BTN/btn_Base").GetComponent<tk2dUIToggleButton>();
        m_GoToggleOn = transform.Find("BTN/btn_Base/On").gameObject;
        m_GoToggleOff = transform.Find("BTN/btn_Base/Off").gameObject;

        m_TextName = transform.Find("TEXT/text_name").GetComponent<RPGTextMesh>();
        m_TextGoldBigReward = transform.Find("TEXT/text_gold_big").GetComponent<RPGTextMesh>();
        m_TextGoldSmallReward = transform.Find("TEXT/text_gold_small").GetComponent<RPGTextMesh>();
        m_TextGoldBonus = transform.Find("TEXT/text_gold_bonus").GetComponent<RPGTextMesh>();

        m_ImgAdReward = transform.Find("IMG/img_ad_reward").gameObject;
        m_ImgAdRemove = transform.Find("IMG/img_ad_remove").gameObject;
        m_ImgGold = transform.Find("IMG/img_gold").gameObject;

        m_GoToggleOn.SetActive(false);
        m_GoToggleOff.SetActive(true);
    }

    public override void Parsing(object data_)
    {
        base.Parsing(data_);

        ItemData data = data_ as ItemData;
        m_TextName.Text = data.Text;

        switch(data.Kind)
        {
            case "ad_reward": 
                m_ImgAdReward.SetActive(true); 
                m_ImgAdRemove.SetActive(false);
                m_ImgGold.SetActive(false);
                m_TextGoldBigReward.gameObject.SetActive(true);
                m_TextGoldSmallReward.gameObject.SetActive(false);
                m_TextGoldBonus.gameObject.SetActive(false);
                m_TextGoldBigReward.Text = data.Reward.ToString("N0")+"G";
                break;
            case "ad_remove": 
                m_ImgAdReward.SetActive(false);
                m_ImgAdRemove.SetActive(true); 
                m_ImgGold.SetActive(false);
                m_TextGoldBigReward.gameObject.SetActive(false);
                m_TextGoldSmallReward.gameObject.SetActive(false);
                m_TextGoldBonus.gameObject.SetActive(false);
                break;
            case "inapp": 
                m_ImgAdReward.SetActive(false);
                m_ImgAdRemove.SetActive(false);
                m_ImgGold.SetActive(true); 
                m_TextGoldBigReward.gameObject.SetActive(false);
                m_TextGoldSmallReward.gameObject.SetActive(true);
                //m_TextName.Text = "<color=#FFD800>G</color> CHARGE";
                //m_TextName.Text = data.Cost.ToString();
                m_TextGoldSmallReward.Text = data.Reward.ToString("N0") + "G";
                if (data.Bonus > 0)
                {
                    m_TextGoldBonus.gameObject.SetActive(true);
                    m_TextGoldBonus.Text = "+"+data.Bonus.ToString("N0") + "G";
                }
                else
                    m_TextGoldBonus.gameObject.SetActive(false);
                break;
        }

        m_GoToggleOn.SetActive(data.IsSelected);
        m_GoToggleOff.SetActive(!data.IsSelected);
    }




}
