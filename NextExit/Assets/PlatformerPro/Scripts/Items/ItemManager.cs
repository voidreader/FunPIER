/**
 * This code is part of Platformer PRO and is copyright John Avery 2014.
 */

using UnityEngine;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Stores details about the items a character has collected. Should be on the same 
	/// Game Object as the character.
	/// </summary>
	public class ItemManager : Persistable, ICoreComponent
	{
		[Header ("Dropping")]
		/// <summary>
		/// Should we drop one item at a time from a stack, or all items in a stack.
		/// </summary>
		[Tooltip("Should we drop one item at a time from a stack, or all items in a stack.")]
		public bool dropAllItemsInStack = true;

		/// <summary>
		/// Where does the drop spawn from relative to character.
		/// </summary>
		[Tooltip ("Where does the drop spawn from relative to character.")]
		public Vector3 dropOffset;

		/// <summary>
		/// Should we impart some velocity to an item when dropped?
		/// </summary>
		[Tooltip ("Should we impart some velocity to an item when dropped?")]
		public Vector2 dropImpulse = new Vector2(0, 1f);


		/// <summary>
		/// The character this item manager applies to.
		/// </summary>
		protected Character character;

		/// <summary>
		/// Data about available item types.
		/// </summary>
		protected List<ItemTypeData> itemTypeData;

		/// <summary>
		/// The item data.
		/// </summary>
		protected ItemData itemData;

	
		/// <summary>
		/// The player preference identifier.
		/// </summary>
		public const string UniqueDataIdentifier = "ItemManagerData";

		#region events

		/// <summary>
		/// Item collected.
		/// </summary>
		public event System.EventHandler <ItemEventArgs> ItemCollected;

		/// <summary>
		/// Raises the item collected event.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="amount">Number collected.</param>
		/// <param name="character">Character.</param>
		virtual protected void OnItemCollected(string type, int amount, Character character)
		{
			if (SaveOnChange) Save (this);
			if (ItemCollected != null)
			{
				ItemCollected(this, new ItemEventArgs(type, amount, character));
			}
		}

		/// <summary>
		/// Item consumed.
		/// </summary>
		public event System.EventHandler <ItemEventArgs> ItemConsumed;

        /// <summary>
        /// Raises the item consumed event.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="amount">Number consumed.</param>
        /// <param name="character">Character.</param>
        virtual protected void OnItemConsumed(string type, int amount, Character character)
        {
            if (ItemConsumed != null)
            {
                ItemConsumed(this, new ItemEventArgs(type, amount, character));
            }
        }

        /// <summary>
        /// Occurs when item damaged.
        /// </summary>
        public event System.EventHandler<ItemEventArgs> ItemDamaged;

        /// <summary>
        /// Raises the item damaged event
        /// </summary>
        /// <param name="data">Item Data.</param>
        virtual protected void OnItemDamaged(ItemInstanceData data)
        {
            if (ItemDamaged != null)
            {
                ItemDamaged(this, new ItemEventArgs(data.ItemId, character));
            }
        }

        /// <summary>
        /// Occurs when item damaged.
        /// </summary>
        public event System.EventHandler<ItemEventArgs> ItemDestroyed;

        /// <summary>
        /// Raises the item consumed event.
        /// </summary>
        /// <param name="data">Item data.</param>
        virtual protected void OnItemDestroyed(ItemInstanceData data)
        {
            if (ItemDestroyed != null)
            {
                ItemDestroyed(this, new ItemEventArgs(data.ItemId, character));
            }
        }

        /// <summary>
        /// Sent when item is consumed and none remain.
        /// </summary>
        public event System.EventHandler <ItemEventArgs> ItemDepleted;
		
		/// <summary>
		/// Raises the item depleted event.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="character">Character.</param>
		virtual protected void OnItemDepleted(string type, Character character)
		{
			if (ItemDepleted != null)
			{
				ItemDepleted(this, new ItemEventArgs(type, 0, character));
			}
		}

		/// <summary>
		/// Sent when item is dropped.
		/// </summary>
		public event System.EventHandler <ItemEventArgs> ItemDropped;

		/// <summary>
		/// Raises the item dropped event.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="character">Character.</param>
		virtual protected void OnItemDropped(string type, Character character)
		{
			if (ItemDropped != null)
			{
				ItemDropped(this, new ItemEventArgs(type, 0, character));
			}
		}

		/// <summary>
		/// Item collected.
		/// </summary>
		public event System.EventHandler <ItemEventArgs> ItemMaxUpdated;

		/// <summary>
		/// Raises the item max updated event.
		/// </summary>
		/// <param name="itemClass">Item class.</param>
		/// <param name="type">Type.</param>
		/// <param name="amount">Amount.</param>
		/// <param name="character">Character.</param>
		virtual protected void OnItemMaxUpdated(string type, int amount, Character character)
		{
			if (ItemMaxUpdated != null)
			{
				ItemMaxUpdated(this, new ItemEventArgs(type, amount, character));
			}
		}

		/// <summary>
		/// Called when inventory items are changed without neccessarily updating item counts (for example rearranging).
		/// </summary>
		public event System.EventHandler <CharacterEventArgs> InventoryChanged;

		/// <summary>
		/// Raises the inventory rearranged event.
		/// </summary>
		virtual public void OnInventoryChanged()
		{
            if (SaveOnChange) Save(this);
            if (InventoryChanged != null)
			{
				InventoryChanged(this, new CharacterEventArgs(character));
			}
		}

		#endregion

		/// <summary>
		/// Init with the specified character.
		/// </summary>
		/// <param name="character">Character.</param>
		virtual public void Init(Character character)
		{
			this.character = character;
			ConfigureEventListeners ();
			if (!loaded)
			{
				itemData = new ItemData();
				foreach (ItemTypeData typeData in ItemTypeManager.Instance.ItemData)
				{
					if (typeData.itemClass == ItemClass.NON_INVENTORY || typeData.itemClass == ItemClass.UNIQUE)
					{
						itemData.AddItem (typeData.typeId, typeData.startingCount);
					} 
					else if (typeData.itemClass == ItemClass.NORMAL)
					{
						if (typeData.startingCount > 0)
						{
							character.Inventory.AddItem (typeData.typeId, typeData.startingCount);
						}
					}
				}
			}
		}

		/// <summary>
		/// Collect the given item.
		/// </summary>
		/// <returns>The actual number collected.</returns>
		/// <param name="item">Item.</param>
		virtual public int CollectItem(Item item)
		{
			return CollectItem (item.ItemId, item.Amount);
		}

		/// <summary>
		/// Drops the item in slot.
		/// </summary>
		/// <returns>True if dropped or false otherwise.</returns>
		/// <param name="index">slot index.</param>
		virtual public bool DropItemFromInventorySlot(int index)
		{
			if (character.Inventory == null)
			{
				Debug.LogWarning ("Tried to drop an item from an inventory but the Character doesn't have one");
				return false;
			}
			ItemInstanceData item = character.Inventory.GetItemAt (index);
			if (item == null || item.amount == 0) return false;
			ItemTypeData itemInstanceData = ItemTypeManager.Instance.GetTypeData (item.ItemId);
			if (itemInstanceData == null || !itemInstanceData.allowDrop) return false;
			int amount = dropAllItemsInStack ? item.amount : 1;
			if (itemInstanceData.DropPrefab != null)
			{
				SpawnDroppedItem (item, amount);
			}
			character.Inventory.RemoveItemAt (index, amount);
			OnItemDropped (item.ItemId, character);
			if (ItemCount (item.ItemId) == 0) OnItemDepleted (item.ItemId, character);
			return true;
		}

		/// <summary>
		/// Spawns a dropped item in the scene at Character position.
		/// </summary>
		/// <param name="itemTypeData">Item type data.</param>
		/// <param name="amountToDrop">How many items appear in the dropped stack.</param>
		virtual protected void SpawnDroppedItem(ItemInstanceData itemInstanceData, int amountToDrop)
		{
			if (itemInstanceData.Data.DropPrefab == null) return;
			GameObject itemGo = GameObject.Instantiate (itemInstanceData.Data.DropPrefab);
			itemGo.transform.position = character.transform.position + dropOffset;
			Item item = itemGo.GetComponent<Item> ();
			if (item != null)
			{
                item.instanceData = new ItemInstanceData(itemInstanceData);
                item.Amount = amountToDrop;
            }
			Rigidbody2D body = itemGo.GetComponent<Rigidbody2D> ();
			if (body != null && dropImpulse != Vector2.zero)
			{
				body.AddForce (dropImpulse, ForceMode2D.Impulse);
			}
			item.DoDrop ();
		}

		/// <summary>
		/// Applies the item effects for the item with given id.
		/// </summary>
		/// <param name="typeId">Type identifier.</param>
		virtual public void ApplyItemEffects(string typeId) 
		{
			ItemTypeData data = ItemTypeManager.Instance.GetTypeData(typeId);
			if (data == null)
			{
				Debug.LogWarning ("Item type " + typeId + " was not found");
			}
			else
			{
				ApplyItemEffects (data);
			}
		}

		/// <summary>
		/// Apply effects from item data.
		/// </summary>
		/// <param name="itemData">Item data.</param>
		virtual public void ApplyItemEffects(ItemTypeData itemTypeData) 
		{
			// Attack

			// Defence
			if (itemTypeData.invulnerability) character.CharacterHealth.SetInvulnerable();

            // Agility

            // Health
            if (itemTypeData.healthAdjustment != 0) character.CharacterHealth.Heal(itemTypeData.healthAdjustment);
            if (itemTypeData.maxHealthAdjustment != 0) character.CharacterHealth.MaxHealth += itemTypeData.maxHealthAdjustment;
            if (itemTypeData.breathAdjustment != 0) character.Breath.GainBreath((float)itemTypeData.breathAdjustment);
        }

		/// <summary>
		/// Removes the item effects for the item with given id.
		/// </summary>
		/// <param name="typeId">Type identifier.</param>
		virtual public void RemoveItemEffects(string typeId) 
		{
			ItemTypeData data = ItemTypeManager.Instance.GetTypeData(typeId);
			if (data == null)
			{
				Debug.LogWarning ("Item type " + typeId + " was not found");
			}
			else
			{
				RemoveItemEffects (data);
			}
		}

		/// <summary>
		/// Removes the item effects.
		/// </summary>
		/// <param name="itemData">Item data.</param>
		virtual public void RemoveItemEffects(ItemTypeData itemData) 
		{
			// Attack

			// Defence
			if (itemData.invulnerability)
			{
				// TODO Check if anything else makes them invulnerable before removing invulnerability
				character.CharacterHealth.SetVulnerable ();
			}

			// Agility

			// health
		}

		/// <summary>
		/// Sets the item count without raising a collection event.
		/// </summary>
		/// <returns>The item count.</returns>
		/// <param name="itemType">Item type.</param>
		/// <param name="amount">Item count.</param>
		virtual public int SetItemCount(string itemType, int amount)
		{
			ItemTypeData typeData = ItemTypeManager.Instance.GetTypeData (itemType);
			if (typeData.itemClass != ItemClass.NON_INVENTORY)
			{
				Debug.LogError ("Not yet implmeneted for inventory items");
				return 0;
			}
			if (itemData.ContainsKey(itemType))
			{
				if (amount > ItemMax(itemType)) amount = ItemMax (itemType);
				itemData[itemType] = amount;
				return amount;
			}
			else
			{
				if (amount > ItemMax(itemType)) amount = ItemMax (itemType);
				itemData[itemType] = amount;
				itemData.AddItem (itemType, amount);
				return amount;
			}
		}

		/// <summary>
		/// Gets the number of items of the given type.
		/// </summary>
		/// <returns>The count.</returns>
		/// <param name="itemType">Item type.</param>
		virtual public int ItemCount(string itemType)
		{
            int result = 0;
			if (itemData != null && itemData.ContainsKey(itemType)) result += itemData[itemType];
			if (character.Inventory != null) result += character.Inventory.ItemCount (itemType);
            if (character.EquipmentManager != null) result += character.EquipmentManager.ItemCount(itemType);
            return result;
		}

		/// <summary>
		/// Gets the maximum number of items for the given type.
		/// </summary>
		/// <returns>The count.</returns>
		/// <param name="itemType">Item type.</param>
		virtual public int ItemMax(string itemType)
		{
			ItemTypeData type = ItemTypeManager.Instance.GetTypeData (itemType);
			if (type != null) return type.itemMax;
			return 0;
		}

		/// <summary>
		/// Consumes the given amount of items of type itemType.
		/// </summary>
		/// <param name="itemType">Item type.</param>
		/// <param name="amount">Amount to consume.</param>
		/// <returns>The actual amount consumed.</returns>
		virtual public int UseItem(string itemType, int amount)
		{
			ItemTypeData itemTypeData = ItemTypeManager.Instance.GetTypeData (itemType);
			int actualAmount = amount;
			if (itemTypeData == null)
			{
				Debug.LogWarning("Item type " + itemType + " was not found");
				return 0;
			}
			if (itemTypeData.itemClass == ItemClass.INSTANT)
			{
				Debug.LogWarning ("You can't use an INSTANT item, it will be used when it is collected");
				return 0;
			}
			if (itemTypeData.itemClass == ItemClass.NON_INVENTORY)
			{
				if (itemData.ContainsKey (itemType))
				{
					if (itemTypeData.itemBehaviour == ItemBehaviour.CONSUMABLE ||
					    itemTypeData.itemBehaviour == ItemBehaviour.POWER_UP)
					{
                        actualAmount = itemData.Consume (itemType, amount);
					}
                   // TODO Can we use equippables?
                    DoUseItem (itemTypeData, actualAmount);
					if (ItemCount(itemTypeData.typeId) == 0) OnItemDepleted(itemTypeData.typeId, character);
					return actualAmount;
				}
				return 0;
			}
			// Inventory
			if (character.Inventory != null && character.Inventory.ItemCount(itemType) > 0)
			{
				if (itemTypeData.itemBehaviour == ItemBehaviour.CONSUMABLE ||
					itemTypeData.itemBehaviour == ItemBehaviour.POWER_UP)
				{
					actualAmount = character.Inventory.Consume (itemType, amount);
				}
				if (itemTypeData.itemBehaviour == ItemBehaviour.EQUIPPABLE)
				{
					int inventorySlot = character.Inventory.GetFirstSlotForItem (itemType);
					if (inventorySlot >= 0) character.EquipmentManager.EquipFromInventory(itemTypeData.slot, inventorySlot);
				}

				DoUseItem (itemTypeData, actualAmount);
				if (ItemCount(itemTypeData.typeId) == 0) OnItemDepleted(itemTypeData.typeId, character);
				return actualAmount;
			}
			return 0;
		}

		/// <summary>
		/// Uses a single item from a specific inventory slot.
		/// </summary>
		/// <returns><c>true</c>, if item from inventory slot was used, <c>false</c> otherwise.</returns>
		/// <param name="index">Index.</param>
		virtual public bool UseItemFromInventorySlot(int index)
		{
			if (character.Inventory == null)
			{
				Debug.LogWarning ("Tried to use an item from an inventory but the Character doesn't have one");
				return false;
			}
			ItemInstanceData item = character.Inventory.GetItemAt (index);
			if (item == null || item.amount == 0) return false;
			if (item.Data.itemBehaviour == ItemBehaviour.CONSUMABLE ||
                item.Data.itemBehaviour == ItemBehaviour.POWER_UP)
			{
				character.Inventory.RemoveItemAt(index, 1);
			}
			if (item.Data.itemBehaviour == ItemBehaviour.EQUIPPABLE)
			{
				character.EquipmentManager.EquipFromInventory(item.Data.slot, index);
			}
			return DoUseItem (item.Data, 1);
		}

        virtual public bool DamageItemInEquipmentSlot(string slot, int amount)
        {
            ItemInstanceData itemInstanceData = character.EquipmentManager.GetItemForSlot(slot);
            if (itemInstanceData == null) return false;
            int damageDone = itemInstanceData.Damage(amount);
            if (damageDone == -1)
            {
                character.EquipmentManager.DestroyItem(slot);
                OnItemDestroyed(itemInstanceData);
            }
            else if (damageDone > 0)
            {
                OnItemDamaged(itemInstanceData);
            }
            return false;
        }

        /// <summary>
        /// Uses the given item, but does not handle removing it from stack/inventory.
        /// </summary>
        /// <returns><c>true</c>, if item can be used, <c>false</c> otherwise false.</returns>
        /// <param name="itemTypeData">Item type data.</param>
        virtual protected bool DoUseItem(ItemTypeData itemTypeData, int amount)
		{
			switch (itemTypeData.itemBehaviour)
			{
			case ItemBehaviour.EQUIPPABLE:
				// WEILDABLE must be handled by the calling function as how it works depends
				// on if you are using from an invenotry or not (i.e. have to ensure enough space to unequip stuff)
				break;
			case ItemBehaviour.CONSUMABLE:
				OnItemConsumed(itemTypeData.typeId, amount, character);
                ApplyItemEffects(itemTypeData);
                return true;
			case ItemBehaviour.POWER_UP:
				if (amount != 1) Debug.LogWarning ("Tried to use more than one power up at the same time");
				if (character.PowerUpManager == null)
				{
					Debug.LogError ("If you wish to use power ups you must add a PowerUpManager to your Character");
				} else
				{
					OnItemConsumed(itemTypeData.typeId, amount, character);
					character.PowerUpManager.Collect (itemTypeData.typeId);
					return true;
				}
				break;
			}
				
			return false;
		}

		/// <summary>
		/// Gets a list of items that the character has.
		/// </summary>
		/// <returns>The items the character has.</returns>///
		virtual public List<ItemAndCount> GetItems()
		{
			List<ItemAndCount> result = new List<ItemAndCount> ();
			for(int i = 0; i < itemData.stackableItemCountsIds.Count; i++)
			{
				result.Add(new ItemAndCount( itemData.stackableItemCountsIds[i],  itemData.stackableItemCountsCounts[i]));
			}
			return result;
		}

		/// <summary>
		/// Returns true if the character has at least one of the given item.
		/// </summary>
		/// <param name="itemType">Item type.</param>
		/// <returns>True if the character has the item, false otherwise.</returns>
		virtual public bool HasItem(string itemType)
		{
			return ItemCount(itemType) > 0;
		}

		/// <summary>
		/// Handle collecting a non-stackable item.
		/// </summary>
		/// <returns>The actual number collected (taking in to account max items).</returns>
		/// <param name="itemType">Item type.</param>
		/// <param name="amount">Amount.</param>
		virtual public int CollectItem(string itemType, int amount) 
		{
			ItemTypeData typeData = ItemTypeManager.Instance.GetTypeData(itemType);

			// Handle INSTANT items
			if (typeData.itemClass == ItemClass.INSTANT)
			{
				if (typeData.itemBehaviour == ItemBehaviour.POWER_UP)
				{
					if (character.PowerUpManager == null)
					{
						Debug.LogError ("If you wish to use power ups you must add a PowerUpManager to your Character");
					} else
					{
						character.PowerUpManager.Collect (itemType);
					}
				}
				else if (typeData.itemBehaviour == ItemBehaviour.CONSUMABLE)
				{
					ApplyItemEffects (typeData);
				}
				return 1;
			}
			// Handle NON_INVENTORY (custom stack) items
			if (typeData.itemClass == ItemClass.NON_INVENTORY || typeData.itemClass == ItemClass.UNIQUE)
			{
				int max = ItemMax (itemType);
				if (itemData.ContainsKey (itemType))
				{
					if (itemData [itemType] + amount > ItemMax (itemType))
					{
						int remainder = amount - (max - itemData [itemType]);
						itemData [itemType] = max;
						OnItemCollected (itemType, remainder, character);
						return remainder;
					}
					itemData [itemType] += amount;
					OnItemCollected (itemType, amount, character);
					return amount;
				} 
				else
				{
					if (itemData [itemType] + amount > ItemMax (itemType))
					{
						int remainder = amount - max;
						itemData.AddItem (itemType, max);
						OnItemCollected (itemType, remainder, character);
						return remainder;
					}
					itemData.AddItem (itemType, amount);
					OnItemCollected (itemType, amount, character);
					return amount;
				}
			}
			// Handle Inventory Items
			if (character.Inventory)
			{
				int result = character.Inventory.AddItem (typeData, amount);
				if (result > 0) OnItemCollected (itemType, amount, character);
				return result;
			}
			else
			{
				Debug.LogWarning ("Tried to add item to an inventory but Character does not have an Inventory");
			}
			return 0;
		}

		#region Persitable methods

		/// <summary>
		/// Gets the character reference.
		/// </summary>
		/// <value>The character.</value>
		override public Character Character
		{
			get
			{
				#if UNITY_EDITOR
				if (character == null) return GetComponentInParent<Character>();
				#endif
				return character;
			}
            set
            {
                Debug.LogWarning("ItemManager doesn't allow character to be changed");
            }
        }

		/// <summary>
		/// Gets the data to save.
		/// </summary>
		override public object SaveData
		{
			get
			{
                return new object[]{itemData, 
					(Character == null || Character.Inventory == null) ? null : Character.Inventory.SaveData };
			}
		}
		
		/// <summary>
		/// Get a unique identifier to use when saving the data (for example this could be used for part of the file name or player prefs name).
		/// </summary>
		/// <value>The identifier.</value>
		override public string Identifier
		{
			get
			{
				return UniqueDataIdentifier;
			}
		}
		
		/// <summary>
		/// Applies the save data to the object.
		/// </summary>
		override public void ApplySaveData(object t)
		{
			if (t is object[])
			{
				if (((object[])t)[0] is ItemData && (((object[])t)[1] == null || ((object[])t)[1] is InventoryData))
				{
					this.itemData = (ItemData)((object[])t)[0];
					if (character.Inventory != null)
					{
						character.Inventory.ApplySaveData (((object[])t)[1]);
					}
					loaded = true;
					OnLoaded();
				}
				else 
				{
					Debug.LogError("Tried to apply unepxected data: " + ((object[])t)[0].GetType() + " " + ((object[])t)[1].GetType());
				}
			}
			else 
			{
				Debug.LogError("Tried to apply unepxected data: " + t.GetType());
			}
		}
		
		/// <summary>
		/// Get the type of object this Persistable saves.
		/// </summary>
		override public System.Type SavedObjectType()
		{
			return typeof(object[]);
		}

		/// <summary>
		/// Resets the save data back to default.
		/// </summary>
		override public void ResetSaveData()
		{
			itemData = new ItemData ();
			foreach (ItemTypeData itemTypeData in ItemTypeManager.Instance.ItemData)
			{
				if (itemTypeData.itemClass == ItemClass.NON_INVENTORY || itemTypeData.itemClass == ItemClass.UNIQUE)
				{
					itemData.AddItem (itemTypeData.typeId, itemTypeData.startingCount);
				} 
				else if (itemTypeData.itemClass == ItemClass.NORMAL)
				{
					if (itemTypeData.startingCount > 0)
					{
						character.Inventory.AddItem (itemTypeData.typeId, itemTypeData.startingCount);
					}
				}
			}
#if UNITY_EDITOR
			Save(this);
#endif
		}

		/// <summary>
		/// Support complex object serialisation by passing additional types to seralizer.
		/// </summary>
		override public System.Type[] GetExtraTypes() 
		{
			return new System.Type[]{ typeof(ItemData), typeof(InventoryData), typeof(ItemInstanceData), typeof(EquipmentData) };
		}

		#endregion
	}
}