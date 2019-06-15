using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Used to group buttons that can be placed on an inventory screen.
	/// </summary>
	public class UIInventoryButtonGroup : UIInventory 
	{


		/// <summary>
		/// Gets the header string used to describe the component.
		/// </summary>
		/// <value>The header.</value>
		override public string Header
		{
			get
			{
				return "Used to group buttons that can be placed on an inventory screen.";
			}
		}
			

		/// <summary>
		/// Handle the game phase by looking for READY phase and creating a UI when ready.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		override protected void HandlePhaseChange (object sender, GamePhaseEventArgs e)
		{

		}

		
        /// <summary>
        /// Updates the inventory slots to match Inventory content.
        /// </summary>
        override public void UpdateInventory()
        {
            if (slots == null) return;
            if (Focused)
            {
                if (currentSelection == -1) SelectFirstAvailable();
                else slots[currentSelection].UpdateSelection(true);
            }
        }

        /// <summary>
        /// Creates the inventory UI.
        /// </summary>
        override protected void CreateInventory()
		{
            // Select an item if we don't allow empty selection
            currentSelection = -1;
            CurrentPick = -1;
            SelectFirstAvailable();
            GridLayoutGroup layout = null;
            if (slotContentHolder != null)
            {
                layout = slotContentHolder.GetComponentInChildren<GridLayoutGroup>();
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
			// if (currentSelection == index) return;
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
			return false;
		}

		/// <summary>
		/// Gets the data for slots at index. Null if no data there.
		/// </summary>
		/// <returns>The data for slot.</returns>
		/// <param name="index">Index of slot.</param>
		override public ItemInstanceData GetDataForSlot(int index)
		{
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
            return false;
		}

		/// <summary>
		/// Activate the item in the given slot.
		/// </summary>
		/// <param name="index">Index.</param>
		override public void ActivateItemAt(int index)
		{
            slots[index].OnActivate();
        }

	}
}