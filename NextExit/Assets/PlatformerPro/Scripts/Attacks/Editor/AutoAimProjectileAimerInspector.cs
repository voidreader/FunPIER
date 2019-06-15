using UnityEditor;
using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	[CustomEditor (typeof(AutoAimProjectileAimer), true)]
	public class AutoAimProjectileAimerInspector : Editor {

		public override void OnInspectorGUI()
		{
			float offsetDistance = EditorGUILayout.FloatField(new GUIContent("Offset Distance", "Distance from the centre of shooter to the projectile starting position."), ((AutoAimProjectileAimer)target).offsetDistance);
			if (offsetDistance != ((AutoAimProjectileAimer)target).offsetDistance)
			{
				((AutoAimProjectileAimer)target).offsetDistance = offsetDistance;
				EditorUtility.SetDirty(target);
			}
		}
	}
}
