using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PlatformerPro
{
    /// <summary>
    /// Utilities to make it easier to create cusotm editors.
    /// </summary>
    public class PlatformerProEditorUtils : MonoBehaviour 
    {
        /// <summary>
        /// Check object is on the right layer.
        /// </summary
        public static void ValidateLayer(Object obj, string layerName, int layerInt, bool mustmatch)
        {
            GameObject gameObject = null;
            if (obj is GameObject) gameObject = (GameObject) obj;
            if (obj is Component) gameObject = ((Component) obj).gameObject;
            if (gameObject == null) return;
            int layer = LayerMask.NameToLayer(layerName);
            if (mustmatch && gameObject.layer != layerInt && gameObject.layer != layer)
            {
                EditorGUILayout.HelpBox(obj.GetType().Name + "s are usually put on the layer" + layerName + ".", MessageType.Info);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Change Layer", EditorStyles.miniButton))
                {
                    if (layer > 0)
                    {
                        gameObject.layer = layer;
                    }
                    else
                    {
                        gameObject.layer = layerInt;
                    }
                }
                GUILayout.EndHorizontal();
            }
            else if (gameObject.layer == 0)
            {
                EditorGUILayout.HelpBox(obj.GetType().Name + "s aren't usually put on the default layer as this layer is usually for geometry.", MessageType.Info);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Change Layer", EditorStyles.miniButton))
                {
                    if (layer > 0)
                    {
                        gameObject.layer = layer;
                    }
                    else
                    {
                        Debug.LogWarning("We couldn't find the expected layer name . Trying layer " + layerInt + ", you may need to change this!");
                        gameObject.layer = layerInt;
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}