using UnityEngine;
using UnityEditor;

namespace PlatformerPro
{
	/// <summary>
	/// Draws enums as a mask field.
	/// </summary>
	[CustomPropertyDrawer(typeof(DontShowWhenAttribute))]
	public class DontShowWhenAttributeDrawer : PropertyDrawer
	{
		override public void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			DontShowWhenAttribute dontShowWhen = attribute as DontShowWhenAttribute;
			SerializedProperty other = property.serializedObject.FindProperty (dontShowWhen.otherProperty);
			// Find properties of complex classes (TODO this could get it wrong should probably use a regex)
			if (other == null && property.propertyPath.Contains ("."))
			{
				other = property.serializedObject.FindProperty (property.propertyPath.Replace (property.name, dontShowWhen.otherProperty));
			}
			if (other == null || other.boolValue == dontShowWhen.showWhenTrue)
			{
				EditorGUI.PropertyField (position, property, label, true);
			}
		}

		override public float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			DontShowWhenAttribute dontShowWhen = attribute as DontShowWhenAttribute;
			SerializedProperty other = property.serializedObject.FindProperty (dontShowWhen.otherProperty);
			// Find properties of complex classes (TODO this could get it wrong should probably use a regex)
			if (other == null && property.propertyPath.Contains ("."))
			{
				other = property.serializedObject.FindProperty (property.propertyPath.Replace (property.name, dontShowWhen.otherProperty));
			}
			if (other == null || other.boolValue == dontShowWhen.showWhenTrue)
			{
				return EditorGUI.GetPropertyHeight(property);
			}
			return 0;
		}
	}

}