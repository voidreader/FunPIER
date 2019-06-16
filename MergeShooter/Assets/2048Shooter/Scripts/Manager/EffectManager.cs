using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Toolkit;
using UnityEngine;
using UnityEngine.UI;

public class EffectManager : MonoSingleton<EffectManager>
{
    private sealed class _PlayEffect_c__AnonStorey0
    {
        internal GameObject go;

        internal void __m__0()
        {
            UnityEngine.Object.Destroy(this.go);
        }
    }

    [SerializeField]
    private GameObject m_EffectRoot;

    [SerializeField]
    private GameObject m_Effect;

    [SerializeField]
    private GameObject m_FireEffect;
    [SerializeField]
    private GameObject m_BoombEffect;

    private Dictionary<SkinType, Dictionary<int, string>> skinEffectColor = new Dictionary<SkinType, Dictionary<int, string>>();

    protected override void Awake()
    {
        this.skinEffectColor.Add(SkinType.Wooden, new Dictionary<int, string>
        {
            {
                2,
                "#A9FF47FF"
            },
            {
                4,
                "#A9FF47FF"
            },
            {
                8,
                "#A9FF47FF"
            },
            {
                16,
                "#85E6FFFF"
            },
            {
                32,
                "#85E6FFFF"
            },
            {
                64,
                "#85E6FFFF"
            },
            {
                128,
                "#D470FFFF"
            },
            {
                256,
                "#D470FFFF"
            },
            {
                512,
                "#D470FFFF"
            },
            {
                1024,
                "#D470FFFF"
            },
            {
                2048,
                "#FF3CA1FF"
            },
            {
                4096,
                "#FF3171FF"
            },
            {
                8192,
                "#ff7131ff"
            }
        });
        this.skinEffectColor.Add(SkinType.DAY, new Dictionary<int, string>
        {
            {
                2,
                "#56a034"
            },
            {
                4,
                "#279565"
            },
            {
                8,
                "#2b9fad"
            },
            {
                16,
                "#3a54ba"
            },
            {
                32,
                "#5a23c2"
            },
            {
                64,
                "#66239e"
            },
            {
                128,
                "#a620b7"
            },
            {
                256,
                "#d5224c"
            },
            {
                512,
                "#df3427"
            },
            {
                1024,
                "#cd4516"
            },
            {
                2048,
                "#e46515"
            },
            {
                4096,
                "#b6a931"
            },
            {
                8192,
                "#ffba00"
            }
        });
        this.skinEffectColor.Add(SkinType.NIGHT, new Dictionary<int, string>
        {
            {
                2,
                "#56a034"
            },
            {
                4,
                "#279565"
            },
            {
                8,
                "#2b9fad"
            },
            {
                16,
                "#3a54ba"
            },
            {
                32,
                "#5a23c2"
            },
            {
                64,
                "#66239e"
            },
            {
                128,
                "#a620b7"
            },
            {
                256,
                "#d5224c"
            },
            {
                512,
                "#df3427"
            },
            {
                1024,
                "#cd4516"
            },
            {
                2048,
                "#e46515"
            },
            {
                4096,
                "#b6a931"
            },
            {
                8192,
                "#ffba00"
            }
        });
        this.skinEffectColor.Add(SkinType.NEON, new Dictionary<int, string>
        {
            {
                2,
                "#41b927"
            },
            {
                4,
                "#0d7362"
            },
            {
                8,
                "#1183b2"
            },
            {
                16,
                "#341783"
            },
            {
                32,
                "#702294"
            },
            {
                64,
                "#d03798"
            },
            {
                128,
                "#cf4b73"
            },
            {
                256,
                "#963c0d"
            },
            {
                512,
                "#beae3e"
            },
            {
                1024,
                "#bdbf2b"
            },
            {
                2048,
                "#81c63f"
            },
            {
                4096,
                "#58ba3f"
            },
            {
                8192,
                "#00fffc"
            }
        });
        this.skinEffectColor.Add(SkinType.NOTEBOOK, new Dictionary<int, string>
        {
            {
                2,
                "#18a892"
            },
            {
                4,
                "#239ea6"
            },
            {
                8,
                "#d0a437"
            },
            {
                16,
                "#cd6086"
            },
            {
                32,
                "#9c3e5e"
            },
            {
                64,
                "#c6275e"
            },
            {
                128,
                "#982692"
            },
            {
                256,
                "#d15555"
            },
            {
                512,
                "#ca5c24"
            },
            {
                1024,
                "#d57749"
            },
            {
                2048,
                "#de9247"
            },
            {
                4096,
                "#d5b028"
            },
            {
                8192,
                "#4b4b4b"
            }
        });
        this.skinEffectColor.Add(SkinType.OuterSpace, new Dictionary<int, string>
        {
            {
                2,
                "#ffffff"
            },
            {
                4,
                "#e1f4ff"
            },
            {
                8,
                "#92dcff"
            },
            {
                16,
                "#b1d9ff"
            },
            {
                32,
                "#96d7ff"
            },
            {
                64,
                "#b5eeff"
            },
            {
                128,
                "#58a8ff"
            },
            {
                256,
                "#31a9df"
            },
            {
                512,
                "#72d1fa"
            },
            {
                1024,
                "#b0d7f4"
            },
            {
                2048,
                "#84abc9"
            },
            {
                4096,
                "#729ecb"
            },
            {
                8192,
                "#5078ff"
            }
        });
        this.skinEffectColor.Add(SkinType.CoolSummer, new Dictionary<int, string>
        {
            {
                2,
                "#50bea7"
            },
            {
                4,
                "#3b67c0"
            },
            {
                8,
                "#340fbc"
            },
            {
                16,
                "#6f3cc0"
            },
            {
                32,
                "#8144dc"
            },
            {
                64,
                "#5fcc32"
            },
            {
                128,
                "#7cb832"
            },
            {
                256,
                "#b3cc2f"
            },
            {
                512,
                "#8da125"
            },
            {
                1024,
                "#63a71b"
            },
            {
                2048,
                "#4aa337"
            },
            {
                4096,
                "#309430"
            },
            {
                8192,
                "#e9d840"
            }
        });
    }

    public void PlayEffect(Vector3 pos, int num)
    {
        if (num < 2 || num > 8192)
        {
            SingleInstance<DebugManager>.Instance.LogError("EffectManager.PlayEffect error param num " + num);
            return;
        }
        // ParticleSystem component = this.m_Effect.transform.GetChild(0).GetComponent<ParticleSystem>();
        // if (component == null)
        // {
        //     SingleInstance<DebugManager>.Instance.LogError("EffectManager.PlayEffect get Particle error, particle is " + component);
        //     return;
        // }
        // Color color = Color.white;
        // if (this.skinEffectColor.ContainsKey(MonoSingleton<GameDataManager>.Instance.UseSkinType))
        // {
        //     color = tool.HexcolorTofloat(this.skinEffectColor[MonoSingleton<GameDataManager>.Instance.UseSkinType][num]);
        // }
        // ParticleSystem.MainModule newMain = component.main;
        // newMain.startColor = new ParticleSystem.MinMaxGradient(color);

        GameObject go = UnityEngine.Object.Instantiate<GameObject>(this.m_Effect);
        go.gameObject.SetActive(true);
        go.transform.SetParent(this.m_EffectRoot.transform, false);
        go.transform.localPosition = pos;
        // go.transform.DOScale(go.transform.localScale, 2f).OnComplete(delegate
        // {
        //     UnityEngine.Object.Destroy(go);
        // });
    }

    public void PlayBoombEffect(Vector3 pos, int num)
    {
        GameObject go = UnityEngine.Object.Instantiate<GameObject>(this.m_BoombEffect);
        go.gameObject.SetActive(true);
        go.transform.SetParent(this.m_EffectRoot.transform, false);
        go.transform.localPosition = pos;

        string resName = null;
        switch(num)
        {
            case 8: case 128: case 256: case 1024: case 8192:
                resName = "item_bomb_mint";
                break;
            case 64: case 512: 
                resName = "item_bomb_pink";
                break;
            case 4: case 32: case 4096: 
                resName = "item_bomb_purple";
                break;
            default:
                resName = "item_bomb_yellow";
                break;
        }

        string path = string.Format("InGame/{0}", resName);
        go.GetComponent<Image>().sprite = (Resources.Load(path, typeof(Sprite)) as Sprite);
        go.transform.DOBlendableScaleBy(Vector3.zero, 0.5f);
        Destroy(go, 0.5f);
    }

    public void PlayFireworksDisplay()
    {
        this.m_FireEffect.SetActive(true);
        this.m_FireEffect.transform.DOScale(this.m_FireEffect.transform.localScale, 2f).OnComplete(delegate
        {
            this.m_FireEffect.SetActive(false);
        });
    }
}
