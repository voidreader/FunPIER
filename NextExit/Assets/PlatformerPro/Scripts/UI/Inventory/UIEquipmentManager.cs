using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Shows equipped items.
	/// </summary>
	public class UIEquipmentManager : UIInventory 
	{
		
		/// <summary>
		/// Cached equipment manager ref.
		/// </summary>
		protected EquipmentManager equipmentManager;

		/// <summary>
		/// Gets the header string used to describe the component.
		/// </summary>
		/// <value>The header.</value>
		override public string Header
		{
			get
			{
				return "UI Representation of equipped items with various options for interacting with it.";
			}
		}
			

		/// <summary>
		/// Handle the game phase by looking for READY phase and creating a UI when ready.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		override protected void HandlePhaseChange (object sender, GamePhaseEventArgs e)
		{
			if (e.Phase == GamePhase.READY && equipmentManager != null)  CreateInventory ();
		}

		/// <summary>
		/// Handles the character being loaded.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event args.</param>
		override protected void HandleCharacterLoaded (object sender, CharacterEventArgs e)
		{
			if (playerId == PlatformerProGameManager.ANY_PLAYER || playerId == e.Character.PlayerId)
			{
				character = e.Character;
				itemManager = e.Character.ItemManager;
				equipmentManager = e.Character.EquipmentManager;
				// MULTIPLAYER TODO inventory for players being added later
				// if (inventory != null && PlatformerProGameManager.Instance.GamePhase == GamePhase.READY)  CreateInventory ();
				if (equipmentManager != null)
				{
					equipmentManager.ItemEquipped += HandleItemChanges;
					equipmentManager.ItemUnequipped += HandleItemChanges;
				}
			}
		}

		/// <summary>
		/// Updates the inventory slots to match Inventory content.
		/// </summary>
		override public void UpdateInventory ()
		{
			if (slots == null) return;
			for (int i = 0; i < slots.Count; i++)
			{
				slots [i].UpdateWithItem (i, equipmentManager.GetItemForSlot(slots[i].equipmentSlot), currentSelection == i, CurrentPick == i && CurrentPickInventory == this);
			}
			// Make sure we select an item if one is available
			if (currentSelection == -1 && Focused)
			{
				SelectFirstAvailable();
			}
		}

		/// <summary>
		/// Selects the first available slot.
		/// </summary>
		override protected void SelectFirstAvailable()
		{
			// Can't select an item if we don't have focus
			if (!Focused)
			{
				currentSelection = -1;
				return;
			}
			if (currentSelection != -1) slots [currentSelection].UpdateSelection (false);
			currentSelection = -1;
			int pos = 0;
			while (currentSelection == -1 && pos < slots.Count)
			{ 
				SelectSlotAt(pos);
				pos++;
			}
		}


		/// <summary>
		/// Creates the inventory UI.
		/// </summary>
		override protected void CreateInventory()
		{
			for (int i = 0; i < slots.Count; i++)
			{
				if (!slots [i].isEquipmentSlot) Debug.LogWarning ("Equipment Manager UI references slots that aren't equipment slots!");
			}
			for (int i = 0; i < slots.Count; i++)
			{
				if (slots [i] != null)
				{
					slots [i].UpdateWithItem (i, equipmentManager.GetItemForSlot (slots [i].equipmentSlot), false, false);
				}
			}
			SelectFirstAvailable ();
			GridLayoutGroup layout = null;
			if (slotContentHolder != null)
			{
				layout = slotContentHolder.GetComponentInChildren<GridLayoutGroup> ();
				if (layout != null)
				{
					columnCount = layout.constraintCount;
				}
			}
			if (columnCount == 0) columnCount = 1;
			if (columnCount > slots.Count) columnCount = slots.Count;
		}

		/// <summary>
		/// CHanges the selection to the given slot.
		/// </summary>
		/// <param name="index">Index.</param>
		override public void SelectSlotAt(int index)
		{
			ActiveInventory = this;
			if (currentSelection == index) return;
			if (index < 0) index = -1;
			if (index >= slots.Count) index = slots.Count - 1;
			if (currentSelection >= 0 && currentSelection < slots.Count) slots [currentSelection].UpdateSelection (false);
			currentSelection = index;
			if (index >= 0 && index < slots.Count) slots [index].UpdateSelection (true);
		}


		/// <summary>
		/// Determines whether this instance is a valid pick target for the CurrentPick
		/// </summary>
		/// <returns><c>true</c> if this instance is valid pick target; otherwise, <c>false</c>.</returns>
		override public bool IsPickTarget(ItemInstanceData data, int slotIndex, UIInventory targetInventory)
		{
			ItemTypeData pickItemType = null;
			ItemTypeData otherItemType = null;
			ItemInstanceData pickData = UIInventory.GetPickData ();
			if (pickData != null) pickItemType = ItemTypeManager.Instance.GetTypeData (pickData.ItemId);
			if (data != null) otherItemType = ItemTypeManager.Instance.GetTypeData (data.ItemId);
			if (pickItemType.itemBehaviour == ItemBehaviour.EQUIPPABLE)
			{
				// Always okay to drop if slot types match
				if (otherItemType != null && otherItemType.itemBehaviour == ItemBehaviour.EQUIPPABLE &&
				    pickItemType.slot == otherItemType.slot)
				{
					return true;
				}
				if (CurrentPickInventory == this && targetInventory != this)
				{
					// If the pick start here its okay to drop on empty slot
					if (data == null) return true;
				}
				else if (targetInventory == this)
				{
					// Okay to drop when slot types match
					if (slots[slotIndex].equipmentSlot == pickItemType.slot) return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Gets the data for slots at index. Null if no data there.
		/// </summary>
		/// <returns>The data for slot.</returns>
		/// <param name="index">Index of slot.</param>
		override public ItemInstanceData GetDataForSlot(int index)
		{
			if (index >= 0 && index <= slots.Count)
			{
				if (slots [index] != null)
				{
					ItemInstanceData data = equipmentManager.GetItemForSlot (slots [index].equipmentSlot);
					if (data == null || data.amount == 0) return null;
					return data;
				}
			}
			return null;
		}

		/// <summary>
		/// Once we have two valid picks selected, this tries to do an appropriate action. 
		/// </summary>
		/// <returns><c>true</c>, if pick action was doable, <c>false</c> otherwise</returns>
		/// <param name="target">Target UI.</param>
		/// <param name="index">Index of slot in target UI.</param>
		override protected bool DoPickAction(UIInventory target, int index)
		{
			// If target is self do nothing
			if (CurrentPickInventory == this && target == this)
			{
				// TODO Items that can go to multiple slots (or multiple slots for same item type)
				return false;
			}
			// Else try unequipping to slot if the pick started with this
			else if (CurrentPickInventory == this && target != this)
			{
				if (CurrentPick >= 0 && CurrentPick < slots.Count)
				{
					// First  check if switching with another equippable
					ItemInstanceData data = target.GetDataForSlot(index);
					if (data != null)
					{
						ItemTypeData typeData = ItemTypeManager.Instance.GetTypeData (data.ItemId);
						if (typeData.itemBehaviour == ItemBehaviour.EQUIPPABLE &&
						    slots [CurrentPick].equipmentSlot == typeData.slot)
						{
							bool result = equipmentManager.EquipFromInventory (slots [CurrentPick].equipmentSlot, index);
							return result;
						}
					}
					else
					{
					bool result = equipmentManager.UnequipToInventoryAtSlot (slots [CurrentPick].equipmentSlot, index);
					return result;
					}
				}
			}
			// Else try equipping to slot if the pick started with this
			else if (target == this)
			{
				if (index >= 0 && index < slots.Count)
				{
					bool result = equipmentManager.EquipFromInventory (slots [index].equipmentSlot, CurrentPick);
					return result;
				}
			}
			return false;
		}

		/// <summary>
		/// Activate the item in the given slot.
		/// </summary>
		/// <param name="index">Index.</param>
		override public void ActivateItemAt(int index)
		{
			ItemInstanceData data = GetDataForSlot (index);
			if (data == null || data.amount == 0) return;
			bool result = equipmentManager.UnequipToInventory (slots [index].equipmentSlot);
			if (result && closeWhenItemUsed && screen != null) screen.Hide();
			// TODO Event for fail
		}

	}
}