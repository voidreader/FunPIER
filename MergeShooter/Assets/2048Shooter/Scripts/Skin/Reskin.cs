using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Toolkit;
using UnityEditor;
[ExecuteInEditMode]
public class Reskin : MonoBehaviour
{

    [ExecuteInEditMode]
    // Use this for initialization
    public void OnEnable()
    {
        onReskin();
    }

    void onReskin()
    {
        MonoSingleton<ConfigeManager>.Instance.Init();
        Image[] componentsInChildren = base.gameObject.GetComponentsInChildren<Image>(true);

        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            Image image = componentsInChildren[i];
            if (image.sprite != null)
            {
                //Debug.Log(image.sprite.texture.name);
                Sprite spriteByName = GetSpriteByName(image.sprite.texture.name);
                //Debug.Log(spriteByName.name);
                if (spriteByName != null)
                {
                    image.sprite = spriteByName;
                }
            }
        }
    }

    public Sprite GetSpriteByName(string name)
    {
        Sprite sprite;

        string path = string.Format("UI/{0}/{1}", "notebook", name.ToString());
        sprite = (Resources.Load(path, typeof(Sprite)) as Sprite);
        if (sprite == null)
        {
            path = string.Format("UI/{0}/{1}", "day", name.ToString());
            sprite = (Resources.Load(path, typeof(Sprite)) as Sprite);
        }


        return sprite;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
