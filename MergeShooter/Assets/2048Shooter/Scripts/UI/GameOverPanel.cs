using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Toolkit;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : GameShow
{
    private sealed class _MoneyNumAnim_c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
    {
        internal GameOverPanel _this;

        internal object _current;

        internal bool _disposing;

        internal int _PC;

        object IEnumerator<object>.Current
        {
            get
            {
                return this._current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return this._current;
            }
        }

        public _MoneyNumAnim_c__Iterator0()
        {
        }

        public bool MoveNext()
        {
            uint num = (uint)this._PC;
            this._PC = -1;
            switch (num)
            {
                case 0u:
                    if (this._this.m_isPlayingNumAnim != 1)
                    {
                        if (this._this.m_isPlayingNumAnim == 2)
                        {
                            goto IL_18F;
                        }
                        goto IL_1A9;
                    }
                    break;
                case 1u:
                    break;
                case 2u:
                    goto IL_18F;
                default:
                    return false;
            }
            if (this._this.m_moneyNum >= MonoSingleton<GameDataManager>.Instance.Money)
            {
                goto IL_1A9;
            }
            this._this.m_moneyNum++;
            this._this.m_Money.text = string.Format("{0:N0}", this._this.m_moneyNum);
            this._current = new WaitForSeconds(1f / (float)this._this.m_addMoney);
            if (!this._disposing)
            {
                this._PC = 1;
            }
            return true;
        IL_18F:
            if (this._this.m_moneyNum < MonoSingleton<GameDataManager>.Instance.Money)
            {
                this._this.m_moneyNum++;
                this._this.m_Money.text = string.Format("{0:N0}", this._this.m_moneyNum);
                this._this.m_getMoneyNum++;
                this._this.m_GetMoney.text = "+" + this._this.m_getMoneyNum.ToString();
                this._current = new WaitForSeconds(1f / (float)this._this.m_addMoney);
                if (!this._disposing)
                {
                    this._PC = 2;
                }
                return true;
            }
        IL_1A9:
            this._PC = -1;
            return false;
        }

        public void Dispose()
        {
            this._disposing = true;
            this._PC = -1;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }
    }

    [SerializeField]
    private Text m_Score;

    [SerializeField]
    private Text m_MaxScore;

    [SerializeField]
    private Image m_WinImage;

    [SerializeField]
    private SettingBtns m_SettingBtns;

    [SerializeField]
    private RedHit m_Red;

    [SerializeField]
    private Text m_Money;

    [SerializeField]
    private Text m_GetMoney;

    [SerializeField]
    private TextTweener m_Share;

    [SerializeField]
    private TextTweener m_Restart;

    [SerializeField]
    private GameObject m_DoubleMoney;

    [SerializeField]
    private GameObject m_NoAD;

    [SerializeField]
    private GameObject m_WatchMoneyBtn;

    [SerializeField]
    private GameObject m_btn;

    private bool m_CloseCondition;

    private bool m_BuyState;

    private int m_isPlayingNumAnim;

    private int m_addMoney;

    private int m_moneyNum;

    private int m_getMoneyNum;

    public override void Close(Action callBack, float dealy = 0f)
    {
        if (this.m_SettingBtns.OpenSetting)
        {
            this.m_SettingBtns.Setting();
        }
        this.m_Share.Stop();
        this.m_Restart.Stop();
        base.Close(callBack, dealy);
        this.m_DoubleMoney.SetActive(false);
        this.m_CloseCondition = MonoSingleton<GameDataManager>.Instance.MeetBuyCondition();
    }

    public override void Open(Action callBack, float dealy = 0f)
    {
        base.Open(callBack, dealy);
        if (MonoSingleton<GameDataManager>.Instance.MeetBuyCondition())
        {
            if (!this.m_CloseCondition)
            {
                this.m_BuyState = false;
            }
            if (this.m_BuyState)
            {
                this.m_Share.Show(5f, 1f);
                this.m_Restart.Show(-1f, 10f);
            }
            else
            {
                //==this.m_Red.Play();
            }
        }
        else
        {
            this.m_Share.Show(5f, 1f);
            this.m_Restart.Show(-1f, 10f);
        }
        if (AdManager.Instance.IsRewardLoaded())
        {
            this.m_WatchMoneyBtn.SetActive(true);
            this.m_btn.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
        else
        {
            this.m_btn.transform.localPosition = new Vector3(0f, 120f, 0f);
            this.m_WatchMoneyBtn.SetActive(false);
        }
    }

    public override void Refresh()
    {
        base.Refresh();
        this.m_Score.text = MonoSingleton<GamePlayManager>.Instance.Score.ToString();
        this.m_MaxScore.text = MonoSingleton<GameDataManager>.Instance.MaxScore.ToString();
        this.m_WinImage.material.mainTexture = MonoSingleton<ConfigeManager>.Instance.WinTexture;
        //==this.m_Red.gameObject.SetActive(MonoSingleton<GameDataManager>.Instance.MeetBuyCondition());
        this.m_NoAD.SetActive(MonoSingleton<GameDataManager>.Instance.AD);
        this.setMoneyText();
        this.m_SettingBtns.Refresh(!base.UIState);
    }

    public override void UnlockSkin(SkinType type)
    {
        this.m_BuyState = true;
        //==this.m_Red.Stop();
        base.UnlockSkin(type);
    }

    public void OnClickGameOverDoubleMoney()
    {
        this.m_DoubleMoney.SetActive(false);
        
        AdManager.Instance.ShowReward(null);
    }

    public void NumAnim(bool isDouble, int num)
    {
        this.m_isPlayingNumAnim = ((!isDouble) ? 1 : 2);
        this.m_addMoney = num;
        base.StopCoroutine("MoneyNumAnim");
        base.StartCoroutine("MoneyNumAnim");
    }

    public void CloseWatchBtn()
    {
        this.m_WatchMoneyBtn.SetActive(false);
        this.m_btn.transform.localPosition = new Vector3(0f, 120f, 0f);
    }

    private IEnumerator MoneyNumAnim()
    {
        GameOverPanel._MoneyNumAnim_c__Iterator0 _MoneyNumAnim_c__Iterator = new GameOverPanel._MoneyNumAnim_c__Iterator0();
        _MoneyNumAnim_c__Iterator._this = this;
        return _MoneyNumAnim_c__Iterator;
    }

    private void Update()
    {
        if (this.m_isPlayingNumAnim > 0 && Input.GetMouseButtonDown(0))
        {
            this.m_isPlayingNumAnim = 0;
            this.setMoneyText();
            base.StopCoroutine("MoneyNumAnim");
            return;
        }
    }

    private void setMoneyText()
    {
        this.m_moneyNum = MonoSingleton<GameDataManager>.Instance.Money;
        this.m_getMoneyNum = MonoSingleton<GamePlayManager>.Instance.GetMoney;
        this.m_Money.text = string.Format("{0:N0}", this.m_moneyNum);
        this.m_GetMoney.text = "+" + this.m_getMoneyNum.ToString();
    }
}
