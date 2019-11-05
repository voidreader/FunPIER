using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[ExecuteInEditMode]
[AddComponentMenu("AllIn1SpriteShader/AddAllIn1Shader")]
public class AllIn1Shader : MonoBehaviour
{
    private Material currMaterial, prevMaterial;
    private bool matAssigned = false;
    private enum AfterSetAction { Clear, CopyMaterial, Reset};

#if UNITY_EDITOR
    private static float timeLastReload = -1f;
    private void Start()
    {
        if(timeLastReload < 0) timeLastReload = Time.time;
    }

    private void Update()
    {
        if (matAssigned || Application.isPlaying || !gameObject.activeSelf) return;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            if (GetComponent<Renderer>().sharedMaterial.name.Equals("Sprites-Default")) MakeNewMaterial();
            else matAssigned = true;
        }
        else
        {
            Image img = GetComponent<Image>();
            if (img != null)
            {
                if (img.material.name.Equals("Sprites-Default")) MakeNewMaterial();
                else matAssigned = true;
            }
        }
    }
#endif

    public void MakeNewMaterial()
    {
        SetMaterial(AfterSetAction.Clear);
    }

    public void MakeCopy()
    {
        SetMaterial(AfterSetAction.CopyMaterial);
    }

    private void ResetAllProperties()
    {
        SetMaterial(AfterSetAction.Reset);
    }

    private void SetMaterial(AfterSetAction action)
    {
        Shader allIn1Shader = Resources.Load("AllIn1SpriteShader", typeof(Shader)) as Shader;
        if (!Application.isPlaying && Application.isEditor && allIn1Shader != null)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                prevMaterial = new Material(GetComponent<Renderer>().sharedMaterial);
                currMaterial = new Material(allIn1Shader);
                GetComponent<Renderer>().sharedMaterial = currMaterial;
                GetComponent<Renderer>().sharedMaterial.hideFlags = HideFlags.None;
                matAssigned = true;
                DoAfterSetAction(action);
            }
            else
            {
                Image img = GetComponent<Image>();
                if (img != null)
                {
                    prevMaterial = new Material(GetComponent<Renderer>().sharedMaterial);
                    currMaterial = new Material(allIn1Shader);
                    img.material = currMaterial;
                    img.material.hideFlags = HideFlags.None;
                    matAssigned = true;
                    DoAfterSetAction(action);
                }
            }
            SetSceneDirty();
        }
        else if (allIn1Shader == null)
        {
            Debug.LogError("Make sure the AllIn1SpriteShader file is inside the Resource folder!");
        }
    }

    private void DoAfterSetAction(AfterSetAction action)
    {
        switch (action)
        {
            case AfterSetAction.Clear:
                ClearAllKeywords();
                break;
            case AfterSetAction.CopyMaterial:
                currMaterial.CopyPropertiesFromMaterial(prevMaterial);
                break;
        }
    }

    public void TryCreateNew()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            if (GetComponent<Renderer>().sharedMaterial.name.Contains("AllIn1"))
            {
                ResetAllProperties();
                ClearAllKeywords();
            }
            else MakeNewMaterial();
        }
        else
        {
            Image img = GetComponent<Image>();
            if (img != null)
            {
                if (img.material.name.Contains("AllIn1"))
                {
                    ResetAllProperties();
                    ClearAllKeywords();
                }
                else MakeNewMaterial();
            }
        }
    }

    public void ClearAllKeywords()
    {
        SetKeyword("RECTSIZE_ON");
        SetKeyword("OFFSETUV_ON");
        SetKeyword("CLIPPING_ON");
        SetKeyword("POLARUV_ON");
        SetKeyword("TWISTUV_ON");
        SetKeyword("ROTATEUV_ON");
        SetKeyword("FISHEYE_ON");
        SetKeyword("PINCH_ON");
        SetKeyword("SHAKEUV_ON");
        SetKeyword("WAVEUV_ON");
        SetKeyword("DOODLE_ON");
        SetKeyword("ZOOMUV_ON");
        SetKeyword("FADE_ON");
        SetKeyword("TEXTURESCROLL_ON");
        SetKeyword("GLOW_ON");
        SetKeyword("OUTBASE_ON");
        SetKeyword("OUTTEX_ON");
        SetKeyword("OUTDIST_ON");
        SetKeyword("DISTORT_ON");
        SetKeyword("WIND_ON");
        SetKeyword("GRADIENT_ON");
        SetKeyword("COLORSWAP_ON");
        SetKeyword("HSV_ON");
        SetKeyword("HITEFFECT_ON");
        SetKeyword("PIXELATE_ON");
        SetKeyword("NEGATIVE_ON");
        SetKeyword("COLORRAMP_ON");
        SetKeyword("GREYSCALE_ON");
        SetKeyword("POSTERIZE_ON");
        SetKeyword("BLUR_ON");
        SetKeyword("MOTIONBLUR_ON");
        SetKeyword("GHOST_ON");
        SetKeyword("INNEROUTLINE_ON");
        SetKeyword("HOLOGRAM_ON");
        SetKeyword("CHROMABERR_ON");
        SetKeyword("GLITCH_ON");
        SetKeyword("FLICKER_ON");
        SetKeyword("SHADOW_ON");
        SetKeyword("ALPHACUTOFF_ON");
        SetKeyword("CHANGECOLOR_ON");
        SetSceneDirty();
    }

    private void SetKeyword(string keyword, bool state = false)
    {
        if (currMaterial == null) FindCurrMaterial();
        if (!state) currMaterial.DisableKeyword(keyword);
        else currMaterial.EnableKeyword(keyword);
    }

    private void FindCurrMaterial()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            currMaterial = GetComponent<Renderer>().sharedMaterial;
            matAssigned = true;
        }
        else
        {
            Image img = GetComponent<Image>();
            if (img != null)
            {
                currMaterial = img.material;
                matAssigned = true;
            }
        }
    }

    private void OnDestroy()
    {
        CleanMaterial();
    }

    public void CleanMaterial()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Sprites/Default"));
            matAssigned = false;
        }
        else
        {
            Image img = GetComponent<Image>();
            if (img != null)
            {
                img.material = new Material(Shader.Find("Sprites/Default"));
                matAssigned = false;
            }
        }
        SetSceneDirty();
    }

    public void SaveMaterial()
    {
        #if UNITY_EDITOR
        string fileName = "Assets/AllIn1SpriteShader/Materials/" + gameObject.name + ".mat";
        if (System.IO.File.Exists(fileName))
        {
            EditorUtility.DisplayDialog("Material already exists",
               "The following material already exists: " + fileName + "\n No action will be performed", "Ok");
        }
        else DoSaving(fileName);
        #endif
    }

    private void DoSaving(string fileName)
    {
        #if UNITY_EDITOR
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Material matToSave = null;
        if (sr != null) matToSave = GetComponent<Renderer>().sharedMaterial;
        else
        {
            Image img = GetComponent<Image>();
            if (img != null) matToSave = img.material;
        }
        AssetDatabase.CreateAsset(matToSave, fileName);
        Debug.Log(fileName + " has been saved!");
        #endif
    }

    private void SetSceneDirty()
    {
        #if UNITY_EDITOR
        if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        #endif
    }
}