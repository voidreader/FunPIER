using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlatformerPro
{

	[System.Serializable]
	public class ItemInstanceData
	{
        /// <summary>
        /// Id of the item.
        /// </summary>
        [ItemType]
        [SerializeField]
        protected string itemId;

		/// <summary>
		/// Amount stored in the inventory.
		/// </summary>
		public int amount;

        /// <summary>
        /// Amount of durability (health) remaining. Durability starts at max durability and is reduced as 
        /// an item is Damaged.
        /// </summary>
        public int durability;

        /// <summary>
        /// Storex experience against this object. Can be used to allow items to level up.
        /// </summary>
        public int xp;

        // TODO Store custom attributes

        /// <summary>
        /// Cache item data, we need this a lot.
        /// </summary>
        protected ItemTypeData data;

        public string ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                itemId = value;
                data = ItemTypeManager.Instance.GetTypeData(itemId);
            }
        }

        /// <summary>
        /// Gets the ItemTypeData backing this item.
        /// </summary>
        /// <value>The data.</value>
        public ItemTypeData Data
        {
            get
            {
                if (data == null)
                {
                    data = ItemTypeManager.Instance.GetTypeData(itemId);
                    if (durability > 0 && amount > 1) Debug.LogWarning("Durability is not intended for use with stacked items.");
                }
                return data;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlatformerPro.ItemInstanceData"/> class.
        /// </summary>
        public ItemInstanceData() 
		{
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlatformerPro.ItemInstanceData"/> class by cloning another one
        /// then updating the amount.
        /// </summary>
        public ItemInstanceData(ItemInstanceData source)
        {
            itemId = source.itemId;
            durability = source.durability;
            xp = source.xp;
            amount = source.amount;
            data = ItemTypeManager.Instance.GetTypeData(itemId);
            if (data == null)
            {
                Debug.LogWarning("Unable to find an item with id: " + itemId);
                return;
            }
            if (durability > 0 && amount > 1) Debug.LogWarning("Durability is not intended for use with stacked items.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlatformerPro.ItemInstanceData"/> class.
        /// </summary>
        /// <param name="itemId">Item identifier.</param>
        /// <param name="amount">Amount.</param>
        public ItemInstanceData(string itemId, int amount) 
		{
			this.itemId = itemId;
			this.amount = amount;
            data = ItemTypeManager.Instance.GetTypeData(itemId);
            if (data == null)
            {
                Debug.LogWarning("Unable to find an item with id: " + itemId);
                return;
            }
            if (data.maxDurability > 0)
            { 
                if (amount != 1)
                {
                    Debug.LogWarning("Stackable items don't support durability.");
                }
                durability = data.maxDurability;
            }
        }

        /// <summary>
        /// Damage the item by the specified amount and return amount of damage done, or -1 if item destroyed.
        /// </summary>
        /// <param name="amount">Amount of damage.</param>
        virtual public int Damage(int amount)
        {
            // Unbreakable ignore damage
            if (Data.maxDurability == -1) return 0;
            durability -= amount;
            if (durability <= 0)
            {
                durability = 0;
                return -1;
            }
            return amount;
        }

        /// <summary>
        /// Repairs an item so its durability goes back to max.
        /// </summary>
        virtual public void RepairToMax()
        {
            durability = data.maxDurability;
        }

        /// <summary>
        /// Repairs an item so its durability goes back to max.
        /// </summary>
        /// <param name="amount">Amount ro repair.</param>
        virtual public void Repair(int amount)
        {
            durability += amount; 
            if (durability > data.maxDurability) durability = data.maxDurability;
        }

    }

}