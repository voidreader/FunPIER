using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


/////////////////////////////////////////
//---------------------------------------
namespace HT
{
	[CustomEditor(typeof(HTLightEffect_Wave))]
	public class Inspector_HTLightEffect_Wave : Inspector_HTLightEffect
	{
		//---------------------------------------
		private GUIStyle _headerGUIStyle = null;

		//---------------------------------------
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			HTLightEffect_Wave pTarget = target as HTLightEffect_Wave;
			EditorGUILayout.BeginVertical();

			if (_headerGUIStyle == null)
			{
				_headerGUIStyle = new GUIStyle();
				_headerGUIStyle.fontStyle = FontStyle.Bold;
			}
			GUILayout.Space(20);
			GUILayout.Label("HT LIGHT EFFECT - WAVE", _headerGUIStyle);

			//-----
			{
				bool bRet = EditorGUILayout.Toggle("Intense High - Use range", pTarget.effect_Wave_High_Intensity_UseRange);
				pTarget.effect_Wave_High_Intensity_UseRange = bRet;
				if (bRet)
				{
					EditorGUILayout.BeginHorizontal();

					float fMinVal = EditorGUILayout.FloatField("Intense High - Min", pTarget.effect_Wave_High_Intensity_Min);
					pTarget.effect_Wave_High_Intensity_Min = fMinVal;

					float fMaxVal = EditorGUILayout.FloatField("Intense High - Max", pTarget.effect_Wave_High_Intensity_Max);
					pTarget.effect_Wave_High_Intensity_Max = fMaxVal;

					EditorGUILayout.EndHorizontal();
				}
				else
				{
					float fVal = EditorGUILayout.FloatField("Intense High", pTarget.effect_Wave_High_Intensity);
					pTarget.effect_Wave_High_Intensity = fVal;
				}
			}

			//-----
			{
				bool bRet = EditorGUILayout.Toggle("Intense Low - Use range", pTarget.effect_Wave_Low_Intensity_UseRange);
				pTarget.effect_Wave_Low_Intensity_UseRange = bRet;
				if (bRet)
				{
					EditorGUILayout.BeginHorizontal();

					float fMinVal = EditorGUILayout.FloatField("Intense Low - Min", pTarget.effect_Wave_Low_Intensity_Min);
					pTarget.effect_Wave_Low_Intensity_Min = fMinVal;

					float fMaxVal = EditorGUILayout.FloatField("Intense Low - Max", pTarget.effect_Wave_Low_Intensity_Max);
					pTarget.effect_Wave_Low_Intensity_Max = fMaxVal;

					EditorGUILayout.EndHorizontal();
				}
				else
				{
					float fVal = EditorGUILayout.FloatField("Intense Low", pTarget.effect_Wave_Low_Intensity);
					pTarget.effect_Wave_Low_Intensity = fVal;
				}
			}

			//-----
			{
				bool bRet = EditorGUILayout.Toggle("Wave Time - Use range", pTarget.effect_Wave_Time_UseRange);
				pTarget.effect_Wave_Time_UseRange = bRet;
				if (bRet)
				{
					EditorGUILayout.BeginHorizontal();

					float fMinVal = EditorGUILayout.FloatField("Wave Time - Min", pTarget.effect_Wave_Time_Min);
					pTarget.effect_Wave_Time_Min = fMinVal;

					float fMaxVal = EditorGUILayout.FloatField("Wave Time - Max", pTarget.effect_Wave_Time_Max);
					pTarget.effect_Wave_Time_Max = fMaxVal;

					EditorGUILayout.EndHorizontal();
				}
				else
				{
					float fVal = EditorGUILayout.FloatField("Wave Time", pTarget.effect_Wave_Time);
					pTarget.effect_Wave_Time = fVal;
				}
			}

			//-----
			EditorGUILayout.EndVertical();
		}

		//---------------------------------------
	}
}


/////////////////////////////////////////
//---------------------------------------