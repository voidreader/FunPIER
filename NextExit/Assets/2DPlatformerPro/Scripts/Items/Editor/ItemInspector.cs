#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace PlatformerPro
{

	[CustomEditor(typeof(Item), true)]
	public class ItemInspector : Editor
	{
		Character character;

		/// <summary>
		/// Draw the inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			// Ensure stakables are stackables
			if (target is StackableItem && ((StackableItem)target).itemClass != ItemClass.STACKABLE)
			{
				((StackableItem)target).itemClass = ItemClass.STACKABLE;
			}

			DrawDefaultInspector ();

			if (character == null) character = FindObjectOfType<Character> ();
			if (character != null)
			{
				if ((character.geometryLayerMask.value & 1 << ((Item)target).gameObject.layer) == 1 << ((Item)target).gameObject.layer)
				{
					EditorGUILayout.HelpBox("Items should not be on a geometry layer", MessageType.Warning);
				}
			}
			else
			{
				EditorGUILayout.HelpBox("Item layer cannot be checked as the Character is inactive or not present in the scene.", MessageType.Info);
			}
		}
	}
}
#endif