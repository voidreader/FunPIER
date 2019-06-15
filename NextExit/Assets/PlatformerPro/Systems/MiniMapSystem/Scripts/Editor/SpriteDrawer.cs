using UnityEngine;
using UnityEditor;
using jnamobile.mmm;
using PlatformerPro;

[CustomPropertyDrawer (typeof(SpriteAssignerAttribute))]
public class MMMSpriteAssigner: PropertyDrawer
{

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		if (UnityEngine.GameObject.FindObjectOfType<SpriteDictionary>() == null) {
			return base.GetPropertyHeight(property, label) + 32;
		}
		return base.GetPropertyHeight(property, label);
	}

	public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
	{
		// First get the attribute since it contains the range for the slider
		Rect helpBoxRect = new Rect (rect);
		SpriteDictionary.FindOrCreateInstance ();
		SpriteDictionary spriteDictionary = SpriteDictionary.Instance;
		if (spriteDictionary == null) rect.yMax -= 32;
		Sprite sprite = (Sprite) EditorGUI.ObjectField(rect, label, property.objectReferenceValue, typeof(Sprite), false);
		if (spriteDictionary == null) {
			helpBoxRect.yMin += 18;
			EditorGUI.HelpBox (helpBoxRect, "A sprite dictionary is required for saving/loading and multi-scene maps.", MessageType.Info);
			return;
		}
		SerializedProperty spriteProperty = property.serializedObject.FindProperty((attribute as SpriteAssignerAttribute).nameProperty);
		string name = SpriteDictionary.NameForSprite (sprite);
		// If it hasn't changed make sure dictionary is correct
		if (sprite == property.objectReferenceValue && sprite != null) {
			if (name == null) {
				name = SpriteDictionary.AddSprite (sprite);
				if (name == null) {
					spriteProperty.objectReferenceValue = null;
					return;
				}
			}
			spriteProperty.stringValue = name;
			return;
		}
		if (sprite == null) {
			spriteProperty.stringValue = "NONE";
			return;
		}
		if (name == null) {
			name = SpriteDictionary.AddSprite (sprite);
			if (name == null) return;
		}
		spriteProperty.stringValue = name;
		property.objectReferenceValue = sprite;
	}
}