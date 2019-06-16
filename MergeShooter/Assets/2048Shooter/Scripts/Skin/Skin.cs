using System;
using Toolkit;
using UnityEngine;
using UnityEngine.UI;

public class Skin : MonoBehaviour
{
    [SerializeField]
    private Image m_Image;

    [SerializeField]
    private Image m_UseBtn;

    [SerializeField]
    private GameObject m_Use;

    [SerializeField]
    private GameObject m_Unlock;

    [SerializeField]
    private Text m_Price;

    private SkinType m_Type = SkinType.DAY;

    [SerializeField]
    private GameObject m_Red;

    [SerializeField]
    private Text m_UseText;

    private int m_SkinPrice;

    public SkinType Type
    {
        get
        {
            return this.m_Type;
        }
    }

    public void Init(Sprite image, Sprite useBtnIcon, SkinType type, int skinPrice, string hexcolor, bool unlock = false)
    {
        this.m_Image.sprite = image;
        this.m_Type = type;
        this.m_SkinPrice = skinPrice;
        this.m_UseBtn.sprite = useBtnIcon;
        this.m_Unlock.GetComponent<Image>().sprite = useBtnIcon;
        this.m_Price.text = this.m_SkinPrice.ToString();
        this.m_Use.gameObject.SetActive(unlock);
        this.m_Unlock.gameObject.SetActive(!unlock);
        this.m_Red.gameObject.SetActive(!unlock && MonoSingleton<GameDataManager>.Instance.SkinMeetBuyCondition(this.m_Type));
        this.m_UseText.color = tool.HexcolorTofloat(hexcolor);
    }

    public void UseSkin()
    {
        MonoSingleton<GameManager>.Instance.UseSkin(this.m_Type);
        // // MonoSingleton<GAEvent>.Instance.Game_ClikSkin((int)this.m_Type);
    }

    public void UnlockSkin()
    {
        MonoSingleton<GameManager>.Instance.UnlockSkin(this.m_Type, this.m_SkinPrice);
    }

    public void SkinState(bool unlock = true)
    {
        this.m_Use.gameObject.SetActive(unlock);
        this.m_Unlock.gameObject.SetActive(!unlock);
        this.m_Red.gameObject.SetActive(!unlock && MonoSingleton<GameDataManager>.Instance.SkinMeetBuyCondition(this.m_Type));
    }
}
