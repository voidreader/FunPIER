using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RPG.AntiVariable;

public class UICustomMyPage : RPGLayer
{
    public static UICustomMyPage show()
    {
        return RPGSceneManager.Instance.pushScene<UICustomMyPage>("Prefabs/UI/UICustomMyPage");
    }

    /// <summary>
    /// LayerIndex
    /// 0 = LAYER_MyPage
    /// SubIndex 
    /// 0 = UPLOAD
    /// 1 = HISTORY
    /// 2 = CREATE
    /// LayerIndex
    /// 1 = LAYER_List
    /// SubIndex 
    /// 
    /// </summary>
    /// <param name="LayerIndex"></param>
    /// <param name="SubIndex"></param>
    /// <returns></returns>
    public static UICustomMyPage show(int LayerIndex, int SubIndex)
    {
        UICustomMyPage ui = show();
        ui.TabIndex = LayerIndex;
        if (LayerIndex == 0)
        {
            ui.m_TabLayer[LayerIndex].GetComponent<LAYER_Base>().TabIndex = SubIndex;
        }
        return ui;
    }

    /// <summary>
    /// 0 = LAYER_MyPage
    /// 1 = LAYER_List
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
        //m_TextID.Text = NENetworkManager.Instance.NickName;
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
        //UIMain.show();
        UICustomMain.show();
    }

    
}
