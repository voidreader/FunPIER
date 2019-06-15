/**
 * This code is part of Platformer PRO and is copyright John Avery 2014.
 */

using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Base class for all collectible things.
	/// </summary>
	public class Item : MonoBehaviour
	{

		/// <summary>
		/// High-level class of the item (COLLECTIBLE, POWER_UP, etc).
		/// </summary>
		public ItemClass itemClass;

		/// <summary>
		/// Type of item (e.g. 'coin', 'gem', etc). Should match the value in the <see cref="ItemManager"/>.
		/// </summary>
		public string type;

		/// <summary>
		/// The items collider.
		/// </summary>
		protected Collider2D myCollider;

		/// <summary>
		/// Tracks if this item has already been collected.
		/// </summary>
		protected bool hasHitCharacter;

		#region events

		/// <summary>
		/// Power up collected.
		/// </summary>
		public event System.EventHandler <ItemEventArgs> CollectItem;

		/// <summary>
		/// Raises the item collected event.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="character">Character.</param>
		virtual protected void OnCollectItem(Character character)
		{
			if (CollectItem != null)
			{
				CollectItem(this, new ItemEventArgs(itemClass, type, character));
			}
		}

		/// <summary>
		/// Raises the item collected event.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="character">Character.</param>
		virtual protected void OnCollectItem(ItemClass itemClass, string type, Character character)
		{
			if (CollectItem != null)
			{
				CollectItem(this, new ItemEventArgs(itemClass, type, character));
			}
		}

		/// <summary>
		/// Raises the item collected event.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="amount">Number collected.</param>
		/// <param name="character">Character.</param>
		virtual protected void OnCollectItem(ItemClass itemClass, string type, int amount, Character character)
		{
			if (CollectItem != null)
			{
				CollectItem(this, new ItemEventArgs(itemClass, type, amount, character));
			}
		}

		#endregion

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start()
		{
			Init ();
		}

		/// <summary>
		/// Init this item.
		/// </summary>
		virtual public void Init()
		{
			myCollider = GetComponent<Collider2D>();
			if (myCollider == null)
			{
				Debug.LogError("An Item must be on the same GameObject as a Collider2D");
			}
		}

		/// <summary>
		/// Unity 2D trigger hook.
		/// </summary>
		/// <param name="other">Other.</param>
		void OnTriggerEnter2D(Collider2D other)
		{
			CheckCollect(other);
		}

		/// <summary>
		/// Check if the collection is valid (in this case by checking if its a character colliding).
		/// </summary>
		/// <param name="other">Other.</param>
		virtual protected void CheckCollect(Collider2D other)
		{
			ICharacterReference collectBox = (ICharacterReference) other.gameObject.GetComponent(typeof(ICharacterReference));
			
			// Got a collect box and its not ourselves
			if (collectBox != null && !hasHitCharacter)
			{
				hasHitCharacter = true;
				DoCollect(collectBox.Character);
			}
		}

		/// <summary>
		/// Do the collection.
		/// </summary>
		/// <param name="character">Character doing the collection.</param>
		virtual protected void DoCollect(Character character)
		{
			ItemManager itemManager = character.GetComponentInChildren<ItemManager> ();
			if (itemManager != null) itemManager.CollectItem (this);
			DoEventForCollect (character);
		}
		
		/// <summary>
		/// Send a collect event if we have any listeners or else deactivates object.
		/// </summary>
		/// <param name="character">Character.</param>
		virtual protected void DoEventForCollect(Character character)
		{
			if (CollectItem != null) OnCollectItem(itemClass, type, character);
			else
			{
				// No responders lets do something so the user can tell the collected the item.
				gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Check if there is a listener for the collect event.
		/// </summary>
		protected bool CollectHasListener()
		{
			if (CollectItem != null) return true;
			return false;
		}

#if UNITY_EDITOR
		
		/// <summary>
		/// Unity gizmo hook.
		/// </summary>
		void OnDrawGizmos()
		{
			// We don't do anything but having this here allows us to assign a colored icon to the script.
		}
#endif
	}
}
