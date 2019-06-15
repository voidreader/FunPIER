using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlatformerPro
{
	/// <summary>
	/// Holds Items. Extend to create your own inventories.
	/// </summary>
	public class Inventory : PlatformerProMonoBehaviour, ICoreComponent 
	{
		/// <summary>
		/// Starting size of the inventory.
		/// </summary>
		public int inventorySize;

		/// <summary>
		/// If true empty inventory spots will cause the inventory to be shifted such as there is no gaps.
		/// </summary>
		public bool noGaps;

		/// <summary>
		/// Which character does this inventory belong to.
		/// </summary>
		protected Character character;

		/// <summary>
		/// The inventory data.
		/// </summary>
		protected InventoryData inventoryData;

		/// <summary>
		/// Tracks if we have loaded data.
		/// </summary>
		protected bool loaded;

		/// <summary>
		/// Init this instance. Called from awake.
		/// </summary>
		virtual public void Init(Character character)
		{
			this.character = character;
			if (!loaded)
			{
				inventoryData = new InventoryData (inventorySize);
			}
		}

		/// <summary>
		/// Post Init this instance. Called from Start.
		/// </summary>
		virtual protected void PostInit()
		{
			character = GetComponentInParent<Character> ();
		}

		/// <summary>
		/// Gets item type and amount at the given positiono or null if nothing there (or out of range).
		/// </summary>
		/// <returns>The <see cref="PlatformerPro.ItemInstanceData"/>.</returns>
		/// <param name="position">Position.</param>
		public ItemInstanceData GetItemAt(int position)
		{
			return inventoryData.GetItemAtStack (position);
		}

		/// <summary>
		/// Gets the first slot holding the given item.
		/// </summary>
		/// <returns>The first slot for item.</returns>
		/// <param name="itemId">Item identifier.</param>
		public int GetFirstSlotForItem(string itemId)
		{
			int[] stacks = inventoryData.FindStacks (itemId);
			if (stacks != null && stacks.Length > 0) return stacks [0];
			return -1;
		}

		/// <summary>
		/// Consume the item with specified key in the given amount.
		/// </summary>
		/// <returns>The actual number consumed.</returns>
		/// <param name="key">Key.</param>
		/// <param name="amount">Amount.</param>
		public int Consume(string itemId, int amount)
		{
			int[] stacks = inventoryData.FindStacks (itemId);
			int remainder = amount;
			// Work through in reverse so we use from end of inventory first
			for (int i = stacks.Length - 1; i >= 0; i--)
			{
				ItemInstanceData stackData = inventoryData.GetItemAtStack (stacks [i]);
				if (stackData.amount >= remainder)
				{
					inventoryData.RemoveFromStack (stacks [i], remainder);
					if (noGaps) CleanUpGaps();
					return amount;
				}
				else
				{
					inventoryData.RemoveFromStack (stacks [i], stackData.amount);
					remainder -= stackData.amount;
				}
			}
			if (noGaps) CleanUpGaps();
			return amount - remainder;
		}

		/// <summary>
		/// Gets the item type data for the item at the given position or null if nothing there (or out of range).
		/// </summary>
		/// <returns>The <see cref="PlatformerPro.ItemInstanceData"/>.</returns>
		/// <param name="position">Position.</param>
		public ItemTypeData GetItemTypeDataAt(int position)
		{
			ItemInstanceData data = GetItemAt (position);
			if (data == null) return null;
			return ItemTypeManager.Instance.GetTypeData (data.ItemId);
		}

		/// <summary>
		/// Add the given item to the inventory.
		/// </summary>
		/// <param name="typeId">Item Type identifier.</param>
		/// <param name="amount">Amount.</param>
		public int AddItem(string typeId, int amount)
		{
			ItemTypeData data = ItemTypeManager.Instance.GetTypeData (typeId);
			if (data != null)
			{
				return AddItem (data, amount);
			}
			else
			{
				Debug.LogWarning ("ItemTypeData for id " + typeId + " was not found");
			}
			return 0;
		}

		/// <summary>
		/// Add the given item to the inventory.
		/// </summary>
		/// <param name="typeId">Item Type identifier.</param>
		/// <param name="amount">Amount.</param>
		public int AddItem(ItemInstanceData data)
		{
			int slot = inventoryData.FirstEmptySlot ();
			if (slot == -1) return 0;
			inventoryData.AddToEmptyStack (data, slot);
			return data.amount;
		}

		/// <summary>
		/// Add the given item to the inventory at the given slot.
		/// </summary>
		/// <param name="data">Inventory data.</param>
		/// <param name="slot">Slot to use.</param>
		public int AddItemAt(ItemInstanceData data, int slot)
		{
			if (inventoryData.GetItemAtStack (slot) != null)
			{
				return 0;
			}
			inventoryData.AddToEmptyStack (data, slot);
			return data.amount;
		}

		/// <summary>
		/// Add the given item to the inventory.
		/// </summary>
		/// <param name="type">Item Type.</param>
		/// <param name="amount">Amount.</param>
		public int AddItem(ItemTypeData type, int amount)
		{
            if (type.maxPerStack < 1)
            {
                Debug.LogWarning("Item configuration error, max per stack canot be smaller than 1");
                return 0;
            }
            if (amount > type.itemMax) amount = type.itemMax;
			int remainder = amount;
			// Check for existing stack of item and fill first
			if (type.itemClass != ItemClass.UNIQUE)
			{
				int[] stacks = inventoryData.FindStacks (type.typeId);
				for (int i = 0; i < stacks.Length; i++)
				{
					int count = inventoryData.GetAmountAtStack(stacks[i]);
					if (count < type.maxPerStack)
					{
						if (type.maxPerStack - count >= remainder)
						{
							inventoryData.AddToStack (stacks[i], type.typeId, remainder);
							return amount;
						} 
						else
						{
							int toAdd = type.maxPerStack - count;
							inventoryData.AddToStack (stacks[i], type.typeId, toAdd);
							remainder -= toAdd;
						}
					}
				}
			}
			// Check for empty stacks and fill them
			while (remainder > 0)
			{
				int slot = inventoryData.FirstEmptySlot ();
				if (slot == -1) return amount - remainder;
				if (type.maxPerStack >= remainder)
				{
					inventoryData.AddToEmptyStack (type.typeId, slot, remainder);
					return amount;
				} 
				else
				{
					inventoryData.AddToEmptyStack (type.typeId, slot, type.maxPerStack);
					remainder -= type.maxPerStack;
				}
			}
			return amount;
		}

		/// <summary>
		/// Gets the number of items of the given type.
		/// </summary>
		/// <returns>The count.</returns>
		/// <param name="itemType">Item type.</param>
		virtual public int ItemCount(string itemType)
		{
			return inventoryData.ItemCount (itemType);
		}

		/// <summary>
		/// Gets the number of items of the given type.
		/// </summary>
		/// <returns>The count.</returns>
		/// <param name="type">Item type data.</param>
		virtual public int ItemCount(ItemTypeData type)
		{
			return inventoryData.ItemCount (type.typeId);
		}

		/// <summary>
		/// Remove a single item from the given inventory slot.
		/// </summary>
		/// <param name="index">Index.</param>
		public void RemoveItemAt(int index, int amount)
		{
			inventoryData.RemoveFromStack (index, amount);
			if (noGaps) CleanUpGaps();
		}

		/// <summary>
		/// Switches the positions of items unless item is a stack of same type it which case it tries to merge the stacks.
		/// </summary>
		/// <param name="indexA">Index a.</param>
		/// <param name="indexB">Index b.</param>
		public void SwitchPositions(int indexA, int indexB)
		{
			inventoryData.SwitchPositions (indexA, indexB);
			character.ItemManager.OnInventoryChanged ();
		}

		/// <summary>
		/// Shifts positions of items such that there are no gaps in the inventory. 
		/// </summary>
		public void CleanUpGaps()
		{
			bool hasChanged = inventoryData.CleanUpGaps ();
			if (hasChanged) character.ItemManager.OnInventoryChanged ();
		}

		/// <summary>
		/// Combines stacks until stack max is reached and then shifts positions of items such that there are no gaps in the inventory. 
		/// </summary>
		public void CleanUp()
		{

		}

		/// <summary>
		/// Gets the data required to save this inventory.
		/// </summary>
		/// <value>The save data.</value>
		virtual public object SaveData
		{
			get
			{
				return inventoryData;
			}
		}

		/// <summary>
		/// Applies the save data.
		/// </summary>
		/// <param name="t">T.</param>
		virtual public void ApplySaveData(object t)
		{
			if (t is InventoryData)
			{	
				this.inventoryData = (InventoryData)t;
				loaded = true;
				return;
			} 
			else if (t != null)
			{
				Debug.LogWarning ("Invalid type for InventoryData");
			}
			this.inventoryData = new InventoryData (inventorySize);
			loaded = true;
			return;
		}
	}

	/// <summary>
	/// Stores data baout an inventory.
	/// </summary>
	[System.Serializable]
	public class InventoryData
	{
		public ItemInstanceData[] itemData;

		/// <summary>
		/// Used to calcualte results.
		/// </summary>
		protected List<int> stackCache;

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.InventoryData"/> class.
		/// </summary>
		public InventoryData()
		{
			itemData = new ItemInstanceData[0];
			stackCache = new List<int> ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.InventoryData"/> class.
		/// </summary>
		/// <param name="size">Size.</param>
		public InventoryData(int size)
		{
			itemData = new ItemInstanceData[size];
			stackCache = new List<int> ();
		}

		/// <summary>
		/// Finds the stacks for a given item.
		/// </summary>
		/// <returns>An array of stacks, or empty array if none found.</returns>
		/// <param name="itemId">Item identifier.</param>
		public int[] FindStacks(string itemId)
		{
			stackCache.Clear ();
			for (int i = 0; i < itemData.Length; i++)
			{
				if (itemData [i] != null && itemData [i].ItemId == itemId) stackCache.Add (i);
			}
			return stackCache.ToArray ();
		}

		/// <summary>
		/// Gets the count of a given item type across all stacks.
		/// </summary>
		/// <returns>The count.</returns>
		/// <param name="itemId">Item identifier.</param>
		public int ItemCount(string itemId)
		{
			int result = 0;
			for (int i = 0; i < itemData.Length; i++)
			{
				if (itemData [i] != null && itemData[i].ItemId == itemId) result += itemData [i].amount;
			}
			return result;
		}

		/// <summary>
		/// Gets the amount in the given stack position.
		/// </summary>
		/// <returns>The amount at stack.</returns>
		/// <param name="position">Position.</param>
		public int GetAmountAtStack(int position)
		{
			if (position < 0 || position > itemData.Length)
			{
				Debug.LogWarning ("Tried to get amount at invalid position");
				return 0;
			}
			if (itemData [position] == null) return 0;
			return itemData [position].amount;
		}

		/// <summary>
		/// Gets the data at the given stack position.
		/// </summary>
		/// <returns>The data at stack.</returns>
		/// <param name="position">Position.</param>
		public ItemInstanceData GetItemAtStack(int position)
		{
			if (position < 0 || position >= itemData.Length)
			{
				Debug.LogWarning ("Tried to get from invalid position.");
				return null;
			}
			return itemData [position];
		}

		/// <summary>
		/// Finds the first empty slot.
		/// </summary>
		/// <returns>The empty slot.</returns>
		public int FirstEmptySlot()
		{
			for (int i = 0; i < itemData.Length; i++)
			{
				if (itemData [i] == null) return i;
				// Should never happen but just in case
				if (itemData [i].amount == 0)
				{
					itemData [i] = null;
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// Adds new items to a stack at the given position.
		/// </summary>
		/// <param name="typeId">Item Type ID.</param>
		/// <param name="position">Slot to fill.</param>
		/// <param name="amount">Amount.</param>
		public void AddToEmptyStack(string typeId, int position, int amount)
		{
			if (position < 0 || position > itemData.Length)
			{
				Debug.LogWarning ("Tried to add to invaid position.");
				return;
			}
			if (itemData [position] != null)
			{
				Debug.LogWarning ("Tried to add new item to an existing stack");
				return;
			}
			itemData [position] = new ItemInstanceData (typeId, amount);
		}

		/// <summary>
		/// Adds new items to a stack at the given position.
		/// </summary>
		/// <param name="typeId">Item Type ID.</param>
		/// <param name="position">Slot to fill.</param>
		/// <param name="amount">Amount.</param>
		public void AddToEmptyStack(ItemInstanceData data, int index)
		{
			if (index < 0 || index > itemData.Length)
			{
				Debug.LogWarning ("Tried to add to invalid position.");
				return;
			}
			if (itemData [index] != null)
			{
				Debug.LogWarning ("Tried to add new item to an existing stack");
				return;
			}
			itemData [index] = data;
		}

		/// <summary>
		/// Adds items to an existing stack at given position.
		/// </summary>
		/// <param name="position">Slot to fill.</param>
		/// <param name="amount">Amount.</param>
		public void AddToStack(int position, string itemId, int amount)
		{
			if (position < 0 || position > itemData.Length)
			{
				Debug.LogWarning ("Tried to add to invalid position.");
				return;
			}
			if (itemData [position].ItemId != itemId)
			{
				Debug.LogWarning ("Tried to increase count on an empty stack");
				return;
			}
			itemData [position].amount += amount;
		}

		/// <summary>
		/// Removes items from a stack at given position.
		/// </summary>
		/// <param name="position">Slot to fill.</param>
		/// <param name="amount">Amount.</param>
		public void RemoveFromStack(int position, int amount)
		{
			if (position < 0 || position > itemData.Length)
			{
				Debug.LogWarning ("Tried to remove at invalid position.");
				return;
			}

			if (itemData [position].amount < amount)
			{
				Debug.LogWarning ("Tried to remove more items than the stack contains.");
				return;
			}
			itemData [position].amount -= amount;
			if (itemData [position].amount == 0) itemData [position] = null;
		}

		/// <summary>
		/// Switches the positions of items unless items are stacks of same type it which case it tries to merge the stacks.
		/// </summary>
		/// <param name="indexA">Index a.</param>
		/// <param name="indexB">Index b.</param>
		public void SwitchPositions(int indexA, int indexB)
		{
			if (indexA < 0 || indexA > itemData.Length || indexB < 0 || indexB > itemData.Length)
			{

				Debug.LogWarning ("Tried to switch items at incorrect position.");
				return;
			}
			// Try merge
			if (itemData [indexA] != null && itemData [indexB] != null && itemData [indexA].ItemId == itemData [indexB].ItemId)
			{
				ItemTypeData itemTypeData = ItemTypeManager.Instance.GetTypeData (itemData [indexA].ItemId);
				if (itemTypeData.maxPerStack >= itemData [indexA].amount + itemData [indexB].amount)
				{
					itemData [indexB].amount = itemData [indexA].amount + itemData [indexB].amount;
					itemData [indexA] = null;
				} else
				{
					int total = itemData [indexA].amount + itemData [indexB].amount;
					itemData [indexB].amount = itemTypeData.maxPerStack;
					itemData [indexA].amount = total - itemTypeData.maxPerStack;
				}
			} 
			else
			{
				// Else switch
				ItemInstanceData tmp = itemData [indexA];
				itemData [indexA] = itemData [indexB];
				itemData [indexB] = tmp;
			}

		}

		/// <summary>
		/// Shifts positions of items such that there are no gaps in the inventory. 
		/// </summary>
		public bool CleanUpGaps()
		{
			itemData = itemData.OrderBy (a => a == null).ToArray ();
			// Smarts about checking for changes
			return true;
		}

		/// <summary>
		/// Remvoed items from a stack at given position.
		/// </summary>
		/// <param name="position">Slot to fill.</param>
		/// <param name="amount">Amount.</param>
		public void ClearStack(int position)
		{
			itemData [position] = null;
		}

	}


}