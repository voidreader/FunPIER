using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// The following script turns a stack of collectible into lives when a threshold value is reached.
	/// i.e. Collect 100 coins to get a life. Could easily be modified to convert items or add health.
	/// </summary>
	public class ItemsToLives : MonoBehaviour
	{
		/// <summary>
		/// Name of the item that is collected and converted in to lives.
		/// </summary>
		[Tooltip ("Name of the item that is collected and converted in to lives.")]
		public string itemType;

		/// <summary>
		/// The threshold at which the item is convertedin to to lives.
		/// </summary>
		[Tooltip ("The threshold at which the item is convertedin to to lives.")]
		public int threshold = 100;

		/// <summary>
		/// Cached reference to character health.
		/// </summary>
		protected CharacterHealth characterHealth;

		/// <summary>
		/// Cached reference to item manager
		/// </summary>
		protected ItemManager itemManager;

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start()
		{
			Init ();
		}

		/// <summary>
		/// Unity Destory hook. Remove listener on destroy.
		/// </summary>
		void OnDestroy()
		{
			if (itemManager != null) itemManager.ItemCollected -= HandleItemCollected;
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void Init()
		{
			Character character = GetComponentInParent<Character> ();
			if (character == null)
			{
				Debug .LogWarning ("ItemToLives should be a child of a GameObject with a character attached.");
			}
			else if (itemType == null || itemType == "")
			{
				Debug.LogWarning ("No item type specified, ItemToLives wont work.");
			}
			else
			{
				itemManager = character.GetComponentInChildren<ItemManager>();
				characterHealth = character.GetComponentInChildren<CharacterHealth>();
				if (itemManager == null) Debug.LogWarning("Character does not have an ItemManager, ItemToLives will not work");
				if (characterHealth == null) Debug.LogWarning("Character does not have a CharacterHealth, ItemToLives will not work");
				if (itemManager != null && characterHealth != null)
				{
					itemManager.ItemCollected += HandleItemCollected;
				}
			}
		}

		/// <summary>
		/// Handles the item collected event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Parameters.</param>
		virtual protected void HandleItemCollected (object sender, ItemEventArgs e)
		{
			if (e.Type == itemType)
			{
				if (itemManager.ItemCount(itemType) >= threshold)
				{
					itemManager.ConsumeItem(itemType, threshold);
					characterHealth.CurrentLives += 1;
				}
			}
		}
	}
}