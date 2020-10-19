using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SA.Foundation.Editor
{
    public static class SA_ReorderablList
    {
        static readonly Dictionary<int, bool> s_globalFoldoutItemsState = new Dictionary<int, bool>();

        public delegate string ItemName<T>(T item);

        public delegate void ItemContent<T>(T item);

        public delegate void OnItemAdd();

        public static void Draw<T>(IList<T> list, ItemName<T> itemName, ItemContent<T> itemContent = null, OnItemAdd onItemAdd = null, ItemContent<T> buttonsContentOverride = null, ItemContent<T> itemStartUI = null)
        {
            if (itemContent != null)
                DrawFoldout(list, itemName, itemContent, buttonsContentOverride, itemStartUI);
            else
                DrawLabel(list, itemName, buttonsContentOverride, itemStartUI);

            if (onItemAdd != null)
                using (new SA_GuiBeginVertical())
                {
                    GUILayout.Space(-7);
                    using (new SA_GuiBeginHorizontal())
                    {
                        EditorGUILayout.Space();
                        var add = GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(24));
                        if (add)
                        {
                            onItemAdd();
                            return;
                        }

                        GUILayout.Space(5);
                    }
                }
        }

        static void DrawFoldout<T>(IList<T> list, ItemName<T> itemName, ItemContent<T> itemContent, ItemContent<T> buttonsContentOverride = null, ItemContent<T> itemStartUI = null)
        {
            var indentLevel = EditorGUI.indentLevel;

            var space = 10;
            if (indentLevel >= 1) space += EditorGUI.indentLevel * 10;

            EditorGUI.indentLevel = 0;

            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];

                using (new SA_GuiBeginHorizontal())
                {
                    GUILayout.Space(space);
                    using (new SA_GuiBeginVertical(SA_Skin.BoxStyle))
                    {
                        var foldState = GetFoldoutState(item);
                        using (new SA_GuiBeginHorizontal())
                        {
                            if (itemStartUI != null) itemStartUI.Invoke(item);
#if !UNITY_5
                            foldState = EditorGUILayout.Foldout(foldState, itemName(item), true);
#else
                            foldState = EditorGUILayout.Foldout(foldState, itemName(item));
#endif

                            SetFoldoutState(item, foldState);

                            if (buttonsContentOverride != null)
                            {
                                buttonsContentOverride.Invoke(item);
                            }
                            else
                            {
                                var ItemWasRemoved = DrawButtons(item, list);
                                if (ItemWasRemoved) return;
                            }
                        }

                        if (foldState)
                            using (new SA_GuiIndentLevel(1))
                            {
                                EditorGUILayout.Space();
                                itemContent(item);
                                EditorGUILayout.Space();
                            }
                    }

                    GUILayout.Space(5);
                }
            }

            EditorGUI.indentLevel = indentLevel;
        }

        static void DrawLabel<T>(IList<T> list, ItemName<T> itemName, ItemContent<T> buttonsContentOverride = null, ItemContent<T> itemStartUI = null)
        {
            var indentLevel = EditorGUI.indentLevel;

            var space = 10;
            if (indentLevel >= 1) space += EditorGUI.indentLevel * 10;

            EditorGUI.indentLevel = 0;

            foreach (var item in list)
                using (new SA_GuiBeginHorizontal())
                {
                    GUILayout.Space(space);
                    using (new SA_GuiBeginVertical(SA_Skin.BoxStyle))
                    {
                        using (new SA_GuiBeginHorizontal())
                        {
                            if (itemStartUI != null) itemStartUI.Invoke(item);

                            EditorGUILayout.SelectableLabel(itemName(item), GUILayout.Height(16));

                            if (buttonsContentOverride != null)
                            {
                                buttonsContentOverride.Invoke(item);
                            }
                            else
                            {
                                var ItemWasRemoved = DrawButtons(item, list);
                                if (ItemWasRemoved) return;
                            }
                        }
                    }
                }

            EditorGUI.indentLevel = indentLevel;
        }

        static bool GetFoldoutState(object item)
        {
            if (item == null) return false;
            if (s_globalFoldoutItemsState.ContainsKey(item.GetHashCode()))
                return s_globalFoldoutItemsState[item.GetHashCode()];
            else
                return false;
        }

        static void SetFoldoutState(object item, bool value)
        {
            if (item == null) return;
            s_globalFoldoutItemsState[item.GetHashCode()] = value;
        }

        static bool DrawButtons<T>(T currentObject, IList<T> ObjectsList)
        {
            var ObjectIndex = ObjectsList.IndexOf(currentObject);
            if (ObjectIndex == 0) GUI.enabled = false;

            var up = GUILayout.Button("↑", EditorStyles.miniButtonLeft, GUILayout.Width(20));
            if (up)
            {
                var c = currentObject;
                ObjectsList[ObjectIndex] = ObjectsList[ObjectIndex - 1];
                ObjectsList[ObjectIndex - 1] = c;
            }

            if (ObjectIndex >= ObjectsList.Count - 1)
                GUI.enabled = false;
            else
                GUI.enabled = true;

            var down = GUILayout.Button("↓", EditorStyles.miniButtonMid, GUILayout.Width(20));
            if (down)
            {
                var c = currentObject;
                ObjectsList[ObjectIndex] = ObjectsList[ObjectIndex + 1];
                ObjectsList[ObjectIndex + 1] = c;
            }

            GUI.enabled = true;
            var r = GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20));
            if (r) ObjectsList.Remove(currentObject);

            return r;
        }
    }
}
