using UnityEngine;
using UnityEditor;

namespace PlatformerPro
{
	/// <summary>
	/// Draws enums as a mask field.
	/// </summary>
	[CustomPropertyDrawer(typeof(EnumMaskAttribute))]
	public class EnumMaskAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			property.intValue = EditorGUI.MaskField( position, label, property.intValue, property.enumNames );
		}
	}
}