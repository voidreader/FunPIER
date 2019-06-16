using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Toolkit;
using UnityEngine;
using UnityEngine.UI;

public class HomePanel : GameShow
{
    private sealed class _MoneyNumAnim_c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
    {
        internal HomePanel _this;

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
                    break;
                case 1u:
                    break;
                default:
                    return false;
            }
            if (this._this.m_moneyNum < MonoSingleton<GameDataManager>.Instance.Money)
            {
                this._this.m_moneyNum++;
                this._this.m_Money.text = string.Format("{0:N0}", this._this.m_moneyNum);
                this._current = new WaitForSeconds(1f / (float)this._this.m_addMoney);
                if (!this._disposing)
                {
                    this._PC = 1;
                }
                return true;
            }
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
    private Text m_Money;

    [SerializeField]
    private SettingBtns m_SettingBtns;

    [SerializeField]
    private RedHit m_Red;

    [SerializeField]
    private TextTweener m_Play;

    [SerializeField]
    private GameObject m_NOAD;

    [SerializeField]
    private Transform m_coin;

    [SerializeField]
    private GameObject m_WatchMoneyBtn;

    [SerializeField]
    private GameObject m_btn;

    private bool m_CloseCondition;

    private bool m_BuyState;

    private bool m_isNumAnim;

    private int m_addMoney;

    private int m_moneyNum;

    public Vector3 GetCoinPos()
    {
        return this.m_coin.position;
    }

    public override void Open(Action callBack, float dealy = 0f)
    {
        if (!MonoSingleton<GameDataManager>.Instance.MeetBuyCondition())
        {
            this.m_Play.Show(-1f, 10f);
        }
        base.Open(callBack, dealy);
        if (MonoSingleton<GameDataManager>.Instance.MeetBuyCondition())
        {
            if (!this.m_CloseCondition)
            {
                this.m_BuyState = false;
            }
            if (this.m_BuyState)
            {
                this.m_Play.Show(-1f, 10f);
            }
            else
            {
                //==this.m_Red.Play();
            }
        }
        else
        {
            this.m_Play.Show(-1f, 10f);
        }
        /*if (AdManager.Instance.IsRewardLoaded())
        {
            this.m_WatchMoneyBtn.SetActive(true);
            this.m_btn.transform.localPosition = new Vector3(0f, -14f, 0f);
        }
        else
        {
            this.m_WatchMoneyBtn.SetActive(false);
            this.m_btn.transform.localPosition = new Vector3(0f, -14f, 0f);
        }*/
    }

    public override void Refresh()
    {
        base.Refresh();
        this.m_moneyNum = MonoSingleton<GameDataManager>.Instance.Money;
        this.m_Money.text = string.Format("{0:N0}", this.m_moneyNum);
        //==this.m_Red.gameObject.SetActive(MonoSingleton<GameDataManager>.Instance.MeetBuyCondition());
        this.m_NOAD.SetActive(MonoSingleton<GameDataManager>.Instance.AD);
        this.m_SettingBtns.Refresh(!base.UIState);
        if(GameDataManager.Instance.AD == false)
            AdManager.IsRemoveAds = true;
    }

    public override void Close(Action callBack, float dealy = 0f)
    {
        if (this.m_SettingBtns.OpenSetting)
        {
            this.m_SettingBtns.Setting();
        }
        this.m_Play.Stop();
        base.Close(callBack, dealy);
        this.m_CloseCondition = MonoSingleton<GameDataManager>.Instance.MeetBuyCondition();
    }

    public override void UnlockSkin(SkinType type)
    {
        this.m_BuyState = true;
        //this.m_Red.Stop();
        base.UnlockSkin(type);
    }

    public void NumAnim(int num)
    {
        this.m_isNumAnim = true;
        this.m_addMoney = num;
        base.StopCoroutine("MoneyNumAnim");
        base.StartCoroutine("MoneyNumAnim");
    }

    public void CloseWatchBtn()
    {
        this.m_WatchMoneyBtn.SetActive(false);
        this.m_btn.transform.localPosition = new Vector3(0f, -14f, 0f);
    }

    private IEnumerator MoneyNumAnim()
    {
        HomePanel._MoneyNumAnim_c__Iterator0 _MoneyNumAnim_c__Iterator = new HomePanel._MoneyNumAnim_c__Iterator0();
        _MoneyNumAnim_c__Iterator._this = this;
        return _MoneyNumAnim_c__Iterator;
    }

    private void Update()
    {
        if (this.m_isNumAnim && Input.GetMouseButtonDown(0))
        {
            this.m_isNumAnim = false;
            this.m_moneyNum = MonoSingleton<GameDataManager>.Instance.Money;
            this.m_Money.text = string.Format("{0:N0}", this.m_moneyNum);
            base.StopCoroutine("MoneyNumAnim");
        }
    }
}
