#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Editor for dash attacks.
	/// </summary>
	[CustomEditor (typeof(FX_Base), true)]
	public class FX_BaseInspector : Editor
	{
		/// <summary>
		/// Draw the inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			if (GUILayout.Button ("Test Effect Now"))
			{
				((FX_Base)target).StartEffect ();
			}	
			DrawDefaultInspector ();
		}
	}
}
