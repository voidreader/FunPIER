using UnityEngine;
using System.Collections;

public class LAYER_Base : RPGLayer
{

    /// <summary>
    /// 각각의 탭 항목.
    /// </summary>
    public TAB_Base[] m_TabLayer;
    tk2dUIToggleButtonGroup m_ToggleGroup;

    public int TabIndex { get { return m_ToggleGroup.SelectedIndex; } set { m_ToggleGroup.SelectedIndex = value; } }

    public override void init()
    {
        base.init();
        m_ToggleGroup = getTransform().Find("BTN/TAB").GetComponent<tk2dUIToggleButtonGroup>();
    }

    /// <summary>
    /// My Page와 List 탭.
    /// my page = 0
    /// list = 1
    /// </summary>
    void OnTabChange(tk2dUIToggleButtonGroup toggle)
    {
        for (int i = 0; i < m_TabLayer.Length; i++)
        {
            m_TabLayer[i].gameObject.SetActive(toggle.SelectedIndex == i);
            if (toggle.SelectedIndex == i)
            {
                //m_TabLayer[i].LoadAll(false);
            }
        }
    }

}
