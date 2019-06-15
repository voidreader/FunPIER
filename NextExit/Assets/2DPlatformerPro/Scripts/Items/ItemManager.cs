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
	public class ItemManager : Persistable
	{

		/// <summary>
		/// Data about the available stackable types.
		/// </summary>
		public List<StackableItemData> collectibleItems;

		/// <summary>
		/// The character this item manager applies to.
		/// </summary>
		protected Character character;

		/// <summary>
		/// The item data.
		/// </summary>
		protected ItemData itemData;

		/// <summary>
		/// Have we loaded the data yet?
		/// </summary>
		protected bool loaded;
	
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
		virtual protected void OnItemCollected(ItemClass itemClass, string type, int amount, Character character)
		{
			if (saveOnChange) Save (this);
			if (ItemCollected != null)
			{
				ItemCollected(this, new ItemEventArgs(itemClass, type, amount, character));
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
		virtual protected void OnItemConsumed(ItemClass itemClass, string type, int amount, Character character)
		{
			if (ItemConsumed != null)
			{
				ItemConsumed(this, new ItemEventArgs(itemClass, type, amount, character));
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
		virtual protected void OnItemMaxUpdated(ItemClass itemClass, string type, int amount, Character character)
		{
			if (ItemMaxUpdated != null)
			{
				ItemMaxUpdated(this, new ItemEventArgs(itemClass, type, amount, character));
			}
		}

		#endregion

		/// <summary>
		/// Unity Awake() hook.
		/// </summary>
		void Awake()
		{
			if (character == null) character = GetComponent<Character>();
			if (character == null) character = GetComponentInParent<Character>();
		}

		/// <summary>
		/// Unity start hook.
		/// </summary>
		void Start()
		{
			Init ();
		}
		
		/// <summary>
		/// Init this instance.
		/// </summary>
		override protected void Init()
		{
			base.Init ();
			if (!loaded)
			{
				itemData = new ItemData();
				foreach (StackableItemData item in collectibleItems)
				{
					itemData.AddStackable(item.type, item.startingCount);
				}
				character = GetComponentInParent<Character>();
				if (character == null) Debug.LogError("An ItemManager must be on the same GameObject, or a child of, a Character");
				loaded = true;
				OnLoaded();
			}
		}

		/// <summary>
		/// Collect the given item.
		/// </summary>
		/// <returns>The actual number collected.</returns>
		/// <param name="item">Item.</param>
		virtual public int CollectItem(Item item)
		{
			if (item.itemClass == ItemClass.STACKABLE) 
			{
				return DoCollectStackable(item.itemClass, item.type, (item is StackableItem ? ((StackableItem)item).amount : 1));
			}
			else
			{
				return DoCollect(item.itemClass, item.type, 1);
			}
		}

		/// <summary>
		/// Collects the given item defined by class and type.
		/// </summary>
		/// <returns>The actual number collected (ataking in to account max items and current inventory).</returns>
		/// <param name="itemClass">Item class.</param>
		/// <param name="itemType">Item type.</param>
		/// <param name="amount">Amount.</param>
		virtual public int CollectItem(ItemClass itemClass, string itemType, int amount)
		{
			if (itemClass == ItemClass.STACKABLE) 
			{
				return DoCollectStackable(itemClass, itemType, amount);
			}
			else
			{
				return DoCollect (itemClass, itemType, amount);
			}
		}

		/// <summary>
		/// Sets the item count without raiing a collection event.
		/// </summary>
		/// <returns>The item count.</returns>
		/// <param name="itemType">Item type.</param>
		/// <param name="amount">Item count.</param>
		virtual public int SetItemCount(string itemType, int amount)
		{
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
				itemData.AddStackable (itemType, amount);
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
			if (itemData != null) if (itemData.ContainsKey(itemType)) return itemData[itemType];
			return 0;
		}

		/// <summary>
		/// Gets the maximum number of items for the given type.
		/// </summary>
		/// <returns>The count.</returns>
		/// <param name="itemType">Item type.</param>
		virtual public int ItemMax(string itemType)
		{
			foreach (StackableItemData stackableItem in collectibleItems)
			{
				if (stackableItem.type == itemType) return stackableItem.max;
			}
			// TODO Some kind of non collectible item?

			return 1;
		}

		/// <summary>
		/// Increases the maximum number of items for the given type and returns the new max.
		/// </summary>
		/// <returns>The count.</returns>
		/// <param name="itemType">Item type.</param>
		/// <param name="amount">Amont oadd to item  max.</param>
		virtual public int IncreaseItemMax(string itemType, int amount)
		{
			foreach (StackableItemData stackableItem in collectibleItems)
			{
				if (stackableItem.type == itemType)
				{
					stackableItem.max += amount;
					OnItemMaxUpdated(ItemClass.STACKABLE, itemType, stackableItem.max, character);
					return stackableItem.max;
				}
			}
			// TODO Some kind of non collectible item?
			
			return 1;
		}

		/// <summary>
		/// Sets the maximum number of items for the given type.
		/// </summary>
		/// <param name="itemType">Item type.</param>
		/// <param name="amount">New max item amount.</param>
		virtual public void SetItemMax(string itemType, int amount)
		{
			foreach (StackableItemData stackableItem in collectibleItems)
			{
				if (stackableItem.type == itemType)
				{
					stackableItem.max = amount;
					OnItemMaxUpdated(ItemClass.STACKABLE, itemType, stackableItem.max, character);
					return;
				}
			}
			// TODO Some kind of non collectible item?

		}

		/// <summary>
		/// Consumes the given amount of items of type itemType.
		/// </summary>
		/// <param name="itemType">Item type.</param>
		/// <param name="amount">Amount to consume.</param>
		/// <returns>The actual amount consumed.</returns>
		virtual public int ConsumeItem(string itemType, int amount)
		{
			// Stackable
			if (itemData.ContainsKey(itemType)) 
			{
				int actualAmount = itemData.Consume(itemType, amount);
				OnItemConsumed(itemData.TypeForKey(itemType), itemType, actualAmount, character);
				return actualAmount;
			}
			return 0;
		}
		
		/// <summary>
		/// Gets a list of items that the character has.
		/// </summary>
		/// <returns>The items the character has.</returns>///
		virtual public List<ItemAndCount> GetItems()
		{
			List<ItemAndCount> result = new List<ItemAndCount> ();
			foreach (string item in itemData.items)
			{
				result.Add(new ItemAndCount(item, 1));
			}
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
		/// <param name="itemClass">Item class.</param>
		/// <param name="itemType">Item type.</param>
		/// <param name="amount">Amount.</param>
		virtual protected int DoCollectStackable(ItemClass itemClass, string itemType, int amount) 
		{
			if (itemClass != ItemClass.STACKABLE) 
			{
				if (amount != 1)
				{
					Debug.LogWarning("Tried to collect more than 1 of a non stackable item");
					amount = 1;
				}
			}
			int max =  ItemMax(itemType);
			if (itemData.ContainsKey(itemType))
		    {
				if (itemData[itemType] + amount > ItemMax(itemType))
				{
					int remainder = amount - (max - itemData[itemType]);
					itemData[itemType] = max;
					OnItemCollected (itemClass, itemType, remainder, character);
					return remainder;
				}
				itemData[itemType] += amount;
				OnItemCollected (itemClass, itemType, amount, character);
				return amount;
			}
			else
			{
				if (itemData[itemType] + amount > ItemMax(itemType))
				{
					int remainder = amount - max;
					itemData.AddStackable (itemType, max);
					OnItemCollected (itemClass, itemType, remainder, character);
					return remainder;
				}
				itemData.AddStackable (itemType, amount);
				OnItemCollected (itemClass, itemType, amount, character);
				return amount;
			}

		}

		/// <summary>
		/// Handle collecting a non-stackable item.
		/// </summary>
		/// <returns>The number collected (o if you already had item, 1 otherwise).</returns>
		/// <param name="itemClass">Item class.</param>
		/// <param name="itemType">Item type.</param>
		/// <param name="amount">Amount.</param>
		virtual protected int DoCollect(ItemClass itemClass, string itemType, int amount) 
		{
			if (itemClass != ItemClass.NONE)
			{
				if (itemData.ContainsKey(itemType)) return 0;
				itemData.AddNonStackable (itemType);
			}
			OnItemCollected (itemClass, itemType, 1, character);
			return 1;
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
				return character;
			}
		}

		/// <summary>
		/// Gets the data to save.
		/// </summary>
		override public object SaveData
		{
			get
			{
				return itemData;
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
			if (t is ItemData)
			{
				this.itemData = (ItemData)t;
				loaded = true;
				OnLoaded();
			}
			else Debug.LogError("Tried to apply unepxected data: " + t.GetType());
		}
		
		/// <summary>
		/// Get the type of object this Persistable saves.
		/// </summary>
		override public System.Type SavedObjectType()
		{
			return typeof(ItemData);
		}

		/// <summary>
		/// Resets the save data back to default.
		/// </summary>
		override public void ResetSaveData()
		{
			itemData = new ItemData ();
			foreach (StackableItemData item in collectibleItems)
			{
				itemData.AddStackable(item.type, item.startingCount);
			}
#if UNITY_EDITOR
			Save(this);
#endif
		}

		#endregion
	}
}