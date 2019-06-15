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
		[Header ("Door Settings")]

		/// <summary>
		/// Item type for the key that unlocks door, or empty string if no key is required.
		/// </summary>
		[Tooltip ("Item type for the key that unlocks this door. leave emptyfor a door that does not require a key.")]
		public string keyType;

		/// <summary>
		/// Should this door start in the open state.
		/// </summary>
		[Tooltip ("Should this door start in the open state")]
		public bool startOpen;

		/// <summary>
		/// Reference to point of interest on a map. You can use this to override door POI settings.
		/// </summary>
		[Tooltip ("Reference to point of interest on a map. You can use this to override door POI settings.")]
		public jnamobile.mmm.PointOfInterest poi;

		/// <summary>
		/// Generally any door you can enter will have a respawn point attached. Tick this box to generate one automatically.
		/// Its identifier will be set to gameObject.name
		/// </summary>
		[Tooltip ("Generally any door you can enter will have a respawn point attached. Tick this box to generate one automatically. " +
			 	   "Its identifier will be set to: gameObject.name")]
		public bool generateRespawnPoint = true;

        /// <summary>
        /// The respawn point offset.
        /// </summary>
        [DontShowWhen ("generateRespawnPoint", showWhenTrue = true)]
        public Vector3 respawnPointOffset;

		/// <summary>
		/// Cached respawn point.
		/// </summary>
		protected RespawnPoint respawnPoint;

		/// <summary>
		/// Is door currently open, closed, opening or closing.
		/// </summary>
		public DoorState state
		{
			get;
			protected set;
		}

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
		/// Init this door.
		/// </summary>
		override protected void PostInit() 
		{
			// Args
			characterEventArgs = new CharacterEventArgs ();
			conditions = GetComponents<AdditionalCondition> ();

			// Persistence
			if (useDefaultPersistence)
			{
				SetPersistenceDefaults ();	
			}
			if (target == null) target = gameObject;

			// No persistable lets set a default state
			if (!enablePersistence)
			{
				if (startOpen)
				{
					state = DoorState.OPEN;
				} else
				{
					state = DoorState.CLOSED;
				}
			}
			else
			{
				ProcessState ();
			}

			// Poi
			if (poi != null) jnamobile.mmm.MapManager.Instance.UpdatePointOfInterest (poi);

			// Respawn Point
			if (generateRespawnPoint)
			{
				GameObject respawnGo = new GameObject ();
                respawnGo.name = "RespawnPoint-" + gameObject.name;
                respawnGo.transform.SetParent (transform, false);

				respawnPoint = respawnGo.AddComponent<RespawnPoint> ();
				respawnPoint.identifier = gameObject.name;
				// Don't save, the door will do this
				respawnPoint.enablePersistence = false;
				respawnPoint.useDefaultPersistence = false;
                // Don't automatically go back to the door if you die
                respawnPoint.dontActivateOnRespawn = true;
                // Try to set a reasonable offset
                SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();
				if (sprite != null)
				{
					respawnPoint.transform.position = new Vector3 (transform.position.x, sprite.bounds.center.y, transform.position.z);
				}
				else
				{
					respawnPoint.transform.position = new Vector3 (transform.position.x, transform.position.y + 1.0f, transform.position.z);
				}
                // Then update with the offset
                respawnGo.transform.Translate(respawnPointOffset);
            }
	 	}

		/// <summary>
		/// Sets the persistence defaults.
		/// </summary>
		override protected void SetPersistenceDefaults()
		{
			enablePersistence = PlatformerProGameManager.Instance.persistObjectsInLevel;
			persistenceImplementation = PersistableObjectType.CUSTOM;
			target = gameObject;
			defaultStateIsDisabled = !startOpen;
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
			protected set
			{
				Debug.LogWarning ("You should not call Activate on a door. Instead use Open() or Close().");
			}
		}

		/// <summary>
		/// Open the door.
		/// </summary>
		virtual public void Open(Character character) 
		{
			// Check additional conditions
			if (conditions != null)
			{
				foreach (AdditionalCondition condition in conditions)
				{
					if (!condition.CheckCondition (character, this))
						return;
				}
			}
			if (keyType == null || keyType == "")
			{
				DoOpen (character);
			} 
			else
			{
				ItemManager itemManager = character.GetComponentInChildren<ItemManager> ();
				if (itemManager != null)
				{
					if (itemManager.ItemCount (keyType) > 0)
					{
						itemManager.UseItem (keyType, 1);
						DoOpen (character);
					}
				}
				else
				{
					Debug.LogError("Door requires a key but there is no item manager in the scene.");
				}
		    }
		}

		/// <summary>
		/// Forces the door open.
		/// </summary>
		virtual public void ForceOpen() 
		{
			DoOpen (null);
		}


		/// Forces the door closed.
		/// </summary>
		virtual public void ForceClosed() 
		{
			DoClose (null);
		}

		/// <summary>
		/// Custom persistable implementation. Override to customise.
		/// </summary>
		/// <param name="data">Data.</param>
		override protected void ApplyCustomPersistence(PersistableObjectData data)
		{
			if (data.state)
			{
				ForceOpen ();
			}
			else
			{
				ForceClosed ();
			}
		}

		/// <summary>
		/// Close the door.
		/// </summary>
		virtual public void Close(Character character) 
		{
			// Check additional conditions
			if (conditions != null)
			{
				foreach (AdditionalCondition condition in conditions)
				{
					if (!condition.CheckCondition (character, this))
						return;
				}
			}

			DoClose(character);
		}

		/// <summary>
		/// Show or otherwise handle the door opening.
		/// </summary>
		virtual protected void DoOpen(Character character)
		{
			state = DoorState.OPEN;
			OnOpened (character);
			if (enablePersistence) SetPersistenceState (true);
			if (poi != null) jnamobile.mmm.MapManager.Instance.UpdatePointOfInterest (poi);
		}

		/// <summary>
		/// Show or otherwise handle the door closing.
		/// </summary>
		virtual protected void DoClose(Character character)
	 	{
			state = DoorState.CLOSED;
			OnClosed (character);
			if (enablePersistence) SetPersistenceState (true);SetPersistenceState (false);
			if (poi != null) jnamobile.mmm.MapManager.Instance.UpdatePointOfInterest (poi);
	 	}


		/// <summary>
		/// Called to determine if collision should be ignored. Use for one way platforms or z-ordered platforms
		/// like those found in loops.
		/// </summary>
		/// <returns><c>true</c>, if Collision should be ignored, <c>false</c> otherwise.</returns>
		/// <param name="character">Character.</param>
		/// <param name="collider">Collider.</param>
		override public bool IgnoreCollision(Character character, BasicRaycast collider)
		{
			// Override, don't use additional conditions here, on a door additional conditions apply to openeing the door.
			return false;
		}

		/// <summary>
		/// If this is not NONE then the actiation state will be forced to this value
		/// and not editable by user.
		/// </summary>
		override public PlatformActivationType ForcedActivation
		{
			get
			{
				// This is igored but we need to set some value to hide in inspector
				return PlatformActivationType.ACTIVATE_ON_START;
			}
		}

		/// <summary>
		/// If this is not NONE then the deactiation state will be forced to this value
		/// and not editable by user.
		/// </summary>
		override public PlatformDeactivationType ForcedDeactivation
		{
			get
			{
				// This is igored but we need to set some value to hide in inspector
				return PlatformDeactivationType.DEACTIVATE_ON_LEAVE;
			}
		}

	}

	public enum DoorState
	{
		NOT_INITIALISED,
		OPEN, 
		CLOSED,
		OPENING,
		CLOSING
	}

}