using UnityEngine;
using System.Collections;
using RPG.AntiVariable;

public class UIShop : RPGLayer
{

    public static UIShop show()
    {
        return RPGSceneManager.Instance.pushScene<UIShop>("Prefabs/UI/UIShop");
    }

    public static UIShop show(int TabIndex)
    {
        UIShop ui = show();

        return ui;
    }

    /// <summary>
    /// 0 = TAB_SHOP_GOLD
    /// 1 = TAB_SHOP_HISTORY
    /// </summary>
    public GameObject[] m_TabLayer;
    tk2dUIToggleButtonGroup m_ToggleGroup;

    RPGTextMesh m_TextGold;
    RPGTextMesh m_TextID;

    public int TabIndex { get { return m_ToggleGroup.SelectedIndex; } set { m_ToggleGroup.SelectedIndex = value; } }

    HInt64 m_Gold = 0;

    public override void init()
    {
        base.init();
        m_ToggleGroup = getTransform().Find("BTN/TAB").GetComponent<tk2dUIToggleButtonGroup>();

        m_TextGold = getTransform().Find("TEXT/Content/text_NumberGold").GetComponent<RPGTextMesh>();
        m_TextID = getTransform().Find("TEXT/Content/text_ID").GetComponent<RPGTextMesh>();

        m_TextGold.Text = "0";
        m_TextID.Text = NENetworkManager.Instance.UserID;

        StartCoroutine("cRefreshGold");
    }

    IEnumerator cRefreshGold()
    {
        while (true)
        {
            if (m_Gold != NENetworkManager.Instance.Gold)
            {
                m_Gold = NENetworkManager.Instance.Gold;
                m_TextGold.Text = m_Gold.ToString("N0");
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// My Page와 List 탭.
    /// my page = 0
    /// list = 1
    /// </summary>
    void OnTabChange(tk2dUIToggleButtonGroup toggle)
    {
        for (int i = 0; i < m_TabLayer.Length; i++)
            m_TabLayer[i].SetActive(toggle.SelectedIndex == i);
    }

    void OnBtnBack()
    {
        UIMain.show();
    }



}
