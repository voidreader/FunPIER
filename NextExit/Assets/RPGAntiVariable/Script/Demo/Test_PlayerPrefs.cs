﻿using UnityEngine;
using System.Collections;
using RPG.AntiVariable;

public class Test_PlayerPrefs : MonoBehaviour {

    /// <summary>
    /// Not Security Variable
    /// </summary>
    public int point = 1;
    /// <summary>
    /// Security Variable
    /// </summary>
    public HInt32 anti_point = 1;

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.width - 20));
        {
            GUILayout.Label("Not Security Variable");
            GUILayout.Label("point : " + point);
            if (GUILayout.Button("Point Up", GUILayout.ExpandWidth(false)))
            {
                point++;
            }

            if (point > 1)
            {
                if (GUILayout.Button("Point Down", GUILayout.ExpandWidth(false)))
                {
                    point--;
                }
            }
            if (GUILayout.Button("Point Reset", GUILayout.ExpandWidth(false)))
            {
                point = 1;
            }

            if (GUILayout.Button("Point Save", GUILayout.ExpandWidth(false)))
            {
                PlayerPrefs.SetInt("PlayerPrefs Point", point);
            }
            if (GUILayout.Button("Point Load", GUILayout.ExpandWidth(false)))
            {
                point = PlayerPrefs.GetInt("PlayerPrefs Point", 1);
            }
        }
        {
            GUILayout.Label("Security Variable");
            GUILayout.Label("point : " + anti_point);
            if (GUILayout.Button("Point Up", GUILayout.ExpandWidth(false)))
            {
                anti_point++;
            }

            if (anti_point > 1)
            {
                if (GUILayout.Button("Point Down", GUILayout.ExpandWidth(false)))
                {
                    anti_point--;
                }
            }

            if (GUILayout.Button("Point Reset", GUILayout.ExpandWidth(false)))
            {
                anti_point = 1;
            }

            if (GUILayout.Button("Point Save", GUILayout.ExpandWidth(false)))
            {
                HPlayerPrefs.SetInt("HPlayerPrefs Point", anti_point);
            }
            if (GUILayout.Button("Point Load", GUILayout.ExpandWidth(false)))
            {
                anti_point = HPlayerPrefs.GetInt("HPlayerPrefs Point", 1);
            }

        }
        GUILayout.EndArea();
    }
}
