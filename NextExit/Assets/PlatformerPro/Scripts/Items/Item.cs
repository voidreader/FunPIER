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
	public class Item : PersistableObject
	{
        [HideInInspector]
        public ItemInstanceData instanceData;

		/// <summary>
		/// Type of item (e.g. 'coin', 'gem', etc). Should match the value in the <see cref="ItemManager"/>.
		/// </summary>
		public string ItemId
        {
            get
            {
                return (instanceData.ItemId);
            }
            set
            {
                instanceData.ItemId = value;
            }
        }

        /// <summary>
        /// How many are in this 'pile'
        /// </summary>
        public int Amount {
            get
            {
                return (instanceData.amount);
            }
            set
            {
                instanceData.amount = value;
            }
        }


		[Header ("Persistence")]
		/// <summary>
		/// Does this Item get persistence defaults form the Game manager?
		/// </summary>
		[Tooltip ("Does this Item get persistence defaults form the Game manager?")]
		public bool useDefaultPersistence = true;

		[Header("Map")]
		/// <summary>
		/// Reference to point of interest on a map. You can use this to override item POI settings.
		/// </summary>
		[Tooltip ("Reference to point of interest on a map. You can use this to override item POI settings.")]
		public jnamobile.mmm.PointOfInterest poi;

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
				CollectItem(this, new ItemEventArgs(ItemId, Amount, character));
			}
		}

		/// <summary>
		/// Raises the item collected event.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="amount">Number collected.</param>
		/// <param name="character">Character.</param>
		virtual protected void OnCollectItem(string type, int amount, Character character)
		{
			if (CollectItem != null)
			{
				CollectItem(this, new ItemEventArgs(type, amount, character));
			}
		}

		#endregion

		/// <summary>
		/// Called after an item is spawned from a drop.
		/// </summary>
		virtual public void DoDrop()
		{
			// TODO ANything we need to do here by default?
		}

		/// <summary>
		/// Init item references etc. Called form Start().
		/// </summary>
		override protected void PostInit()
		{
			myCollider = GetComponent<Collider2D>();
			if (myCollider == null)
			{
				Debug.LogError("An Item must be on the same GameObject as a Collider2D");
			}
			if (useDefaultPersistence)
			{
				SetPersistenceDefaults ();	
			}
			base.PostInit ();
		}

		/// <summary>
		/// Sets the persistence defaults.
		/// </summary>
		override protected void SetPersistenceDefaults()
		{
			enablePersistence = PlatformerProGameManager.Instance.persistObjectsInLevel;
			persistenceImplementation = PlatformerProGameManager.Instance.itemPersistenceType;
			target = gameObject;
			defaultStateIsDisabled = false;
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
			int amountCollected = character.ItemManager.CollectItem (this);
			// TODO Allow picking up half a stack
			if (amountCollected > 0)
			{
                DoEventForCollect (character);
				if (enablePersistence) SetPersistenceState (false);
			}
		}
		
		/// <summary>
		/// Send a collect event if we have any listeners or else deactivates object.
		/// </summary>
		/// <param name="character">Character.</param>
		virtual protected void DoEventForCollect(Character character)
		{
			if (poi != null) jnamobile.mmm.MapManager.Instance.RemovePointOfInterest (poi);
			if (CollectItem != null) OnCollectItem(character);
			else
			{
				// No responders lets do something so the user can tell they collected the item.
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

		/// <summary>
		/// Reset this instance.
		/// </summary>
		virtual public void Reset()
		{
			hasHitCharacter = false;
			myCollider.enabled = true;
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
