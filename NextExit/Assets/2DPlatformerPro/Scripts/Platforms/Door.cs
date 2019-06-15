using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// An extension to a platform that represents a door.
	/// </summary>
	public class Door : Platform
	{
		/// <summary>
		/// Item type for the key that unlocks door, or empty string if no key is required.
		/// </summary>
		[Tooltip ("Item type for the key that unlocks this door. leave emptyfor a door that does not require a key.")]
		public string keyType;

		/// <summary>
		/// Item type for the key that unlocks door, or empty string if no key is required.
		/// </summary>
		[Tooltip ("Should this door start in the open state")]
		public bool startOpen;

		/// <summary>
		/// Is door currently open, closed, opening or closing.
		/// </summary>
		protected DoorState state;

		#region events


		/// <summary>
		/// Event for door opened.
		/// </summary>
		public event System.EventHandler <DoorEventArgs> Opened;

		/// <summary>
		/// Event for door closed.
		/// </summary>
		public event System.EventHandler <DoorEventArgs> Closed;

		/// <summary>
		/// Raises the door opened event.
		/// </summary>
		virtual protected void OnOpened(Character character)
		{
			if (Opened != null)
			{
				Opened(this, new DoorEventArgs(this, character));
			}
		}

		/// <summary>
		/// Raises the door opened event.
		/// </summary>
		virtual protected void OnClosed(Character character)
		{
			if (Closed != null)
			{
				Closed(this, new DoorEventArgs(this, character));
			}
		}

		#endregion

		/// <summary>
		/// Unity start hook.
		/// </summary>
	 	void Start()
	 	{
			Init ();
		}

		/// <summary>
		/// Init this door.
		/// </summary>
		override protected void Init() 
		{
			if (startOpen) state = DoorState.OPEN;
			else state = DoorState.CLOSED;
	 	}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PlatformerPro.Platform"/> is activated.
		/// </summary>
		override public bool Activated
		{
			get
			{
				// Generally doors are always active.
				return true;
			}
			set
			{
				Debug.LogWarning ("You should not call Activate on a door. Instead use Open() or Close().");
			}
		}

		/// <summary>
		/// Open the door.
		/// </summary>
		virtual public void Open(Character character) 
		{
			if (keyType == null || keyType == "")
			{
				DoOpen (character);
			} 
			else
			{
				ItemManager itemManager = character.GetComponentInChildren<ItemManager> ();
				if (itemManager != null)
				{
					if (itemManager.ItemCount(keyType) > 0) DoOpen (character);
				}
				else
				{
					Debug.LogError("Door requires a key but there is no item manager in the scene.");
				}
		    }
		}

		/// <summary>
		/// Close the door.
		/// </summary>
		virtual public void Close(Character character) 
		{
			DoClose(character);
		}

		/// <summary>
		/// Show or otherwise handle the door opening.
		/// </summary>
		virtual protected void DoOpen(Character character)
		{
			state = DoorState.OPEN;
			OnOpened (character);
		}

		/// <summary>
		/// Show or otherwise handle the door closing.
		/// </summary>
		virtual protected void DoClose(Character character)
	 	{
			state = DoorState.CLOSED;
			OnClosed (character);
	 	}

	}

	public enum DoorState
	{
		OPEN, 
		CLOSED,
		OPENING,
		CLOSING
	}

}