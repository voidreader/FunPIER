using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Text.RegularExpressions;

//[CustomEditor(typeof(MaterialEditorInspector)), CanEditMultipleObjects]
public abstract class MaterialEditorInspector : MaterialEditor
{
    public class ShaderPropertyAtributes
    {
        public string shaderFeatureName;
        public string shaderInternalName;

        public ShaderPropertyAtributes(string newShaderFeatureName, string newShaderInternalName)
        {
            shaderFeatureName = newShaderFeatureName;
            shaderInternalName = newShaderInternalName;
        }
    }

    public class FeatureToggle
    {
        public string InspectorName;
        public string InspectorPropertyHideTag;
        public string ShaderKeywordEnabled;
        public string ShaderKeywordDisabled;
        public bool Enabled;
        public Dictionary<string, ShaderPropertyAtributes> shaderFeatures;

        public FeatureToggle(string InspectorName, string InspectorPropertyHideTag, string ShaderKeywordEnabled,
            string ShaderKeywordDisabled, Dictionary<string, ShaderPropertyAtributes> shaderF = null)
        {
            this.InspectorName = InspectorName;
            this.InspectorPropertyHideTag = InspectorPropertyHideTag;
            this.ShaderKeywordEnabled = ShaderKeywordEnabled;
            this.ShaderKeywordDisabled = ShaderKeywordDisabled;
            this.Enabled = false;
            if (shaderF != null) this.shaderFeatures = new Dictionary<string, ShaderPropertyAtributes>(shaderF);
            else this.shaderFeatures = new Dictionary<string, ShaderPropertyAtributes>();
        }
    }

    MaterialProperty[] properties;
    Material targetMat;
    protected List<FeatureToggle> Toggles = new List<FeatureToggle>();
    protected abstract void CreateToggleList();

    public override void OnInspectorGUI()
    {
        if (!isVisible)
            return;

        targetMat = target as Material;
        string[] oldKeyWords = targetMat.shaderKeywords;

        Toggles = new List<FeatureToggle>();
        CreateToggleList();

        properties = GetMaterialProperties(targets);

        for (int i = 0; i < Toggles.Count; i++)
        {
            Toggles[i].Enabled = oldKeyWords.Contains(Toggles[i].ShaderKeywordEnabled);
        }

        EditorGUI.BeginChangeCheck();

        serializedObject.Update();
        var theShader = serializedObject.FindProperty("m_Shader");
        if (isVisible && !theShader.hasMultipleDifferentValues && theShader.objectReferenceValue != null)
        {
            float controlSize = 64;
            EditorGUIUtility.labelWidth = Screen.width - controlSize - 20;
            EditorGUIUtility.fieldWidth = controlSize;

            Shader shader = theShader.objectReferenceValue as Shader;

            EditorGUI.BeginChangeCheck();

            for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
            {
                ShaderPropertyImpl(shader, i, null);
            }
            for (int s = 0; s < Toggles.Count; s++)
            {
                EditorGUILayout.Separator();
                Toggles[s].Enabled = EditorGUILayout.BeginToggleGroup(Toggles[s].InspectorName, Toggles[s].Enabled);

                if (Toggles[s].Enabled)
                {
                    GUIStyle style = EditorStyles.helpBox;
                    style.margin = new RectOffset(0, 0, 0, 0);
                    EditorGUILayout.BeginVertical(style);
                    {
                        for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
                        {
                            ShaderPropertyImpl(shader, i, Toggles[s]);
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndToggleGroup();
            }

            if (EditorGUI.EndChangeCheck())
                PropertiesChanged();
        }

        if (EditorGUI.EndChangeCheck())
        {
            List<string> newKeyWords = new List<string>();

            for (int i = 0; i < Toggles.Count; i++)
            {
                newKeyWords.Add(Toggles[i].Enabled ? Toggles[i].ShaderKeywordEnabled : Toggles[i].ShaderKeywordDisabled);
            }

            targetMat.shaderKeywords = newKeyWords.ToArray();
            EditorUtility.SetDirty(targetMat);
        }
    }

    private void ShaderPropertyImpl(Shader shader, int propertyIndex, FeatureToggle currentToggle)
    {
        string propertyDescription = ShaderUtil.GetPropertyDescription(shader, propertyIndex);

        //If property is a toggle checkbox
        if (propertyDescription.Contains(" Toggle") && currentToggle != null)
        {
            propertyDescription = propertyDescription.Remove(propertyDescription.IndexOf(" Toggle"), " Toggle".Length);

            if (currentToggle.shaderFeatures.ContainsKey(propertyDescription))
            {
                float v = targetMat.GetFloat(currentToggle.shaderFeatures[propertyDescription].shaderInternalName);
                v = Mathf.Round(v);
                v = Mathf.Clamp01(v);

                bool toggleState = v == 0 ? false : true;
                toggleState = EditorGUILayout.Toggle(propertyDescription, toggleState);
                if (toggleState)
                {
                    targetMat.EnableKeyword(currentToggle.shaderFeatures[propertyDescription].shaderFeatureName);
                    targetMat.SetFloat(currentToggle.shaderFeatures[propertyDescription].shaderInternalName, 1);
                }
                else if (!toggleState)
                {
                    targetMat.DisableKeyword(currentToggle.shaderFeatures[propertyDescription].shaderFeatureName);
                    targetMat.SetFloat(currentToggle.shaderFeatures[propertyDescription].shaderInternalName, 0);
                }
            }
            return;
        }

        //If its and initial property
        if (currentToggle == null)
        {
            for (int i = 0; i < Toggles.Count; i++)
            {
                if (Regex.IsMatch(propertyDescription, Toggles[i].InspectorPropertyHideTag, RegexOptions.IgnoreCase))
                {
                    return;
                }
            }
        }
        else if (!Regex.IsMatch(propertyDescription, currentToggle.InspectorPropertyHideTag, RegexOptions.IgnoreCase))
        {
            return;
        }
        else if (propertyDescription[0] != currentToggle.InspectorPropertyHideTag[0])
        {
            return;
        }
        ShaderProperty(properties[propertyIndex], properties[propertyIndex].displayName);
    }

    private int GetIndexOfKey(Dictionary<string, ShaderPropertyAtributes> tempDict, string key)
    {
        int index = -1;
        foreach (string value in tempDict.Keys)
        {
            index++;
            if (key == value)
                return index;
        }
        return -1;
    }
}