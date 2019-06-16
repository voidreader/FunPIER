using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Toolkit;
using UnityEngine;

[ExecuteInEditMode]
public class ConfigeManager : MonoSingleton<ConfigeManager>
{
    private sealed class _MyCaptureScreenshot_c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
    {
        internal ConfigeManager _this;

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

        public _MyCaptureScreenshot_c__Iterator0()
        {
        }

        public bool MoveNext()
        {
            uint num = (uint)this._PC;
            this._PC = -1;
            switch (num)
            {
                case 0u:
                    this._current = new WaitForEndOfFrame();
                    if (!this._disposing)
                    {
                        this._PC = 1;
                    }
                    return true;
                case 1u:
                    this._this.m_WinTexture = new Texture2D(Screen.width, Screen.height);
                    this._this.m_WinTexture.ReadPixels(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), 0, 0);
                    this._this.m_WinTexture.Apply();
                    this._this.WriteResultToLocal();
                    if (this._this.m_CallBack != null)
                    {
                        this._this.m_CallBack();
                        this._this.m_CallBack = null;
                    }
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

    [SerializeField]
    private List<SkinInfo> m_ConfigList = new List<SkinInfo>();

    [SerializeField]
    private Dictionary<SkinType, SkinInfo> m_ConfigDic = new Dictionary<SkinType, SkinInfo>();

    private Action m_CallBack;

    private Texture2D m_WinTexture;

    public List<SkinInfo> Config
    {
        get
        {
            return this.m_ConfigList;
        }
    }

    public Texture2D WinTexture
    {
        get
        {
            return this.m_WinTexture;
        }
    }

    public override void Init()
    {
        base.Init();
        foreach (SkinInfo current in this.m_ConfigList)
        {
            if (!this.m_ConfigDic.ContainsKey(current.Type))
            {
                this.m_ConfigDic.Add(current.Type, current);
            }
        }
    }

    public Sprite GetValueIcon(int value)
    {
        SkinType useSkinType = MonoSingleton<GameDataManager>.Instance.UseSkinType;
        if (this.m_ConfigDic.ContainsKey(useSkinType))
        {
            Sprite sprite = this.m_ConfigDic[useSkinType].GetValueIcon(value);
            if (sprite == null)
            {
                string path = string.Format("InGame/{0}", "block_" + value.ToString());
                sprite = (Resources.Load(path, typeof(Sprite)) as Sprite);
                if (sprite != null)
                {
                    this.m_ConfigDic[useSkinType].AddIcon(value, sprite);
                }
                else
                {
                    path = string.Format("InGame/{0}", "block_" + value.ToString());
                    sprite = (Resources.Load(path, typeof(Sprite)) as Sprite);
                    if (sprite != null)
                    {
                        this.m_ConfigDic[useSkinType].AddIcon(value, sprite);
                    }
                    else
                    {
                        SingleInstance<DebugManager>.Instance.LogError("ConfigeManager.GetValueIcon error DefType have not icon of num being" + value);
                    }
                }
            }
            return sprite;
        }
        SingleInstance<DebugManager>.Instance.LogError("ConfigeManager.GetValueIcon error unkown type: " + useSkinType);
        return null;
    }

    public Sprite GetSpriteByName(string name)
    {
        SkinType useSkinType = MonoSingleton<GameDataManager>.Instance.UseSkinType;
        if (this.m_ConfigDic.ContainsKey(useSkinType))
        {
            return this.m_ConfigDic[useSkinType].GetSpriteByName(name);
        }
        return null;
    }

    public void CaptureScreenshot(Action callback)
    {
        this.m_CallBack = callback;
        base.StartCoroutine(this.MyCaptureScreenshot());
    }

    private IEnumerator MyCaptureScreenshot()
    {
        ConfigeManager._MyCaptureScreenshot_c__Iterator0 _MyCaptureScreenshot_c__Iterator = new ConfigeManager._MyCaptureScreenshot_c__Iterator0();
        _MyCaptureScreenshot_c__Iterator._this = this;
        return _MyCaptureScreenshot_c__Iterator;
    }

    public void WriteResultToLocal()
    {
        byte[] bytes = this.m_WinTexture.EncodeToPNG();
        string path = Application.persistentDataPath + "/Shoot2048-GameOver-Screen.png";
        File.WriteAllBytes(path, bytes);
    }
}
