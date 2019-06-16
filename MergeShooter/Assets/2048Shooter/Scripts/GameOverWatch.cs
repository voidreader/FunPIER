using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Toolkit;
using UnityEngine;
using UnityEngine.UI;

public class GameOverWatch : GameShow
{
    private sealed class _showCloseBtn_c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
    {
        internal GameOverWatch _this;

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

        public _showCloseBtn_c__Iterator0()
        {
        }

        public bool MoveNext()
        {
            uint num = (uint)this._PC;
            this._PC = -1;
            switch (num)
            {
                case 0u:
                    this._current = new WaitForSeconds(3f);
                    if (!this._disposing)
                    {
                        this._PC = 1;
                    }
                    return true;
                case 1u:
                    this._this.m_close.SetActive(true);
                    this._PC = -1;
                    break;
            }
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

    private sealed class _fetchVideo_c__Iterator1 : IEnumerator, IDisposable, IEnumerator<object>
    {
        internal GameOverWatch _this;

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

        public _fetchVideo_c__Iterator1()
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
            
            if (this._this.m_bfetch && AdManager.Instance.IsRewardLoaded())
            {
                this._this.m_bfetch = false;
               // MonoSingleton<GameManager>.Instance.GameOvertoWatch();
            }
            this._current = null;
            if (!this._disposing)
            {
                this._PC = 1;
            }
            return true;
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
    private GameObject m_close;

    [SerializeField]
    private Text m_fetchTip;

    [SerializeField]
    private GameObject m_loading;

    [SerializeField]
    private GameObject m_chrysanthemum;

    private Tweener loadingTweener;

    [SerializeField]
    private Ease ease = Ease.Linear;

    private bool m_bfetch;

    public override void Open(Action callBack = null, float dealy = 0f)
    {
        if (!MonoSingleton<GameUIManager>.Instance.IsHomeOrGmaeOver())
        {
            return;
        }
        base.Open(null, 0f);
        this.m_chrysanthemum.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        this.loadingTweener = this.m_chrysanthemum.transform.DOLocalRotate(new Vector3(0f, 0f, -360f), 2f, RotateMode.FastBeyond360).SetEase(this.ease).SetLoops(-1);
        this.m_loading.SetActive(true);
        this.m_close.SetActive(false);
        this.m_bfetch = true;
        this.m_fetchTip.text = Singleton<Localization>.instance.GetLable("WATCHTIP", 0);
        base.StopCoroutine("showCloseBtn");
        base.StartCoroutine("showCloseBtn");
        base.StopCoroutine("fetchVideo");
        base.StartCoroutine("fetchVideo");
    }

    public override void Close(Action callBack = null, float dealy = 0f)
    {
        this.loadingTweener.Kill(false);
        base.StopCoroutine("showCloseBtn");
        base.StopCoroutine("fetchVideo");
        base.Close(null, 0f);
    }

    private IEnumerator showCloseBtn()
    {
        GameOverWatch._showCloseBtn_c__Iterator0 _showCloseBtn_c__Iterator = new GameOverWatch._showCloseBtn_c__Iterator0();
        _showCloseBtn_c__Iterator._this = this;
        return _showCloseBtn_c__Iterator;
    }

    private IEnumerator fetchVideo()
    {
        GameOverWatch._fetchVideo_c__Iterator1 _fetchVideo_c__Iterator = new GameOverWatch._fetchVideo_c__Iterator1();
        _fetchVideo_c__Iterator._this = this;
        return _fetchVideo_c__Iterator;
    }

    public void OnClickCloseWatch()
    {
        MonoSingleton<GameUIManager>.Instance.CloseGameOverWatch(null, 0f);
    }
}
