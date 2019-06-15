#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlatformerPro
{
	/// <summary>
	/// Inspector for inventory.
	/// </summary>
	[CustomEditor(typeof(Inventory), true)]
	[CanEditMultipleObjects]
	public class InventoryInspector : PlatformerProMonoBehaviourInspector 
	{
		const float SLOT_IMAGE_WIDTH = 32.0f;

		/// <summary>
		/// Draws the footer.
		/// </summary>
		override protected void DrawFooter(PlatformerProMonoBehaviour myTarget)
		{
			GUILayout.Space (5);
			GUILayout.Label ("Contents", EditorStyles.boldLabel);

			Inventory inventory = (Inventory)myTarget;
			float width = Screen.width;
			int slotsPerRow = (int)(width / (SLOT_IMAGE_WIDTH * 2.0f)) ; // TODO Retina handling
			slotsPerRow -= 2;
			if (slotsPerRow < 1)  slotsPerRow = 1;
			GUILayout.BeginVertical ();
			GUILayout.BeginHorizontal ();
			for (int i = 0; i < inventory.inventorySize; i++)
			{
				DrawSlotInspector (inventory, i);
				if ((i % slotsPerRow) == (slotsPerRow - 1))
				{
					GUILayout.EndHorizontal ();
					GUILayout.BeginHorizontal ();
				}
			}
			GUILayout.EndHorizontal ();
			GUILayout.EndVertical ();

		}

		/// <summary>
		/// Draw an item in a slot.
		/// </summary>
		virtual protected void DrawSlotInspector(Inventory inventory, int index)
		{
			if (Application.isPlaying)
			{
				ItemTypeData itemTypeData = inventory.GetItemTypeDataAt(index);
				ItemInstanceData inventoryData = inventory.GetItemAt(index);
				if (itemTypeData != null)
				{
					if (itemTypeData.Icon != null)
					{
						GUILayout.Box (itemTypeData.Icon.texture, GUILayout.Width (SLOT_IMAGE_WIDTH), GUILayout.Height (SLOT_IMAGE_WIDTH));
					}
					else
					{
						GUILayout.Box (itemTypeData.typeId, GUILayout.Width (SLOT_IMAGE_WIDTH), GUILayout.Height (SLOT_IMAGE_WIDTH));
					}
					Rect rect = GUILayoutUtility.GetLastRect ();
					GUI.Label(rect, inventoryData.amount.ToString());
					return;
				}
			}

			GUILayout.Box ("", GUILayout.Width (SLOT_IMAGE_WIDTH), GUILayout.Height (SLOT_IMAGE_WIDTH));
		}
	}
}