using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Provides the count of an item as a value for rendering by IValueRenderer components.
	/// </summary>
	public class UIItemCountValueProvider : UIValueProvider 
	{
		/// <summary>
		/// The type of the item to get count for.
		/// </summary>
		[Tooltip ("The type of the item to get count for.")]
		[ItemType]
		public string itemType;

		/// <summary>
		/// Cached item manager reference.
		/// </summary>
		protected ItemManager itemManager;

		/// <summary>
		/// Cached type data.
		/// </summary>
		protected ItemTypeData itemTypeData;

		/// <summary>
		/// Gets the header string used to describe the component.
		/// </summary>
		/// <value>The header.</value>
		override public string Header
		{
			get
			{
				return "Provides the count of a given item as a value to be rendered by one or more IValueRenderers.";
			}
		}

		/// <summary>
		/// Get character reference when character loaded.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event args.</param>
		override protected void HandleCharacterLoaded (object sender, CharacterEventArgs e)
		{
			base.HandleCharacterLoaded (sender, e);
			itemTypeData = ItemTypeManager.Instance.GetTypeData (itemType);
			if (itemTypeData == null)
			{
				Debug.LogWarning("Couldn't find matching item type. This component will be destoryed");
				Destroy (this);
			}
			if (itemTypeData.itemClass == ItemClass.INSTANT)
			{
				Debug.LogWarning("You can't count the number of an item which has an instant effect. It will always be 0.");
			}
			if (playerId == PlatformerProGameManager.ANY_PLAYER || playerId == e.Character.PlayerId)
			{
				itemManager = e.Character.ItemManager;
				if (itemManager != null)
				{
					itemManager.ItemCollected += HandleChange;
					itemManager.ItemConsumed += HandleChange;
					itemManager.ItemMaxUpdated += HandleChange;
					UpdateComponent ();
				} else
				{
					Debug.LogWarning ("Loaded character didn't have a CharacterHealth");
				}
			}
		}

		/// <summary>
		/// Handles any change event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		override protected void HandleChange (object sender, System.EventArgs e)
		{
			// Only  update if its for out item type
			if (e is ItemEventArgs && ((ItemEventArgs)e).Type == itemType)
			{
				UpdateComponent ();
			}
		}

		/// <summary>
		/// Gets the raw value.
		/// </summary>
		/// <value>The value.</value>
		override public object RawValue
		{
			get
			{
				if (itemManager == null) return "";
				return itemManager.ItemCount(itemType);
			}
		}

		/// <summary>
		/// Gets the int value.
		/// </summary>
		/// <value>The int value.</value>
		override public int IntValue
		{
			get
			{
				if (itemManager == null) return 0;
				return itemManager.ItemCount(itemType);
			}
		}

		/// <summary>
		/// Gets the int value.
		/// </summary>
		/// <value>The int value.</value>
		override public int IntMaxValue
		{
			get
			{
				if (itemManager == null) return 0;
				if (itemTypeData.itemClass == ItemClass.UNIQUE) return 1;
				return itemTypeData.itemMax;
			}
		}

		/// <summary>
		/// Gets the value as percentage between 0 (0%) and 1 (100%).
		/// </summary>
		/// <value>The value.</value>
		override public float PercentageValue
		{
			get
			{
				if (itemManager == null) return 0;
				// Ensure full and empty are exact values.
				if (itemManager.ItemCount (itemType) == 0) return 0.0f;
				if (itemManager.ItemCount (itemType) == itemTypeData.itemMax) return 1.0f;
				// Otherwise divide
				return (float)itemManager.ItemCount (itemType) / (float)itemTypeData.itemMax;
			}
		}

	}
}