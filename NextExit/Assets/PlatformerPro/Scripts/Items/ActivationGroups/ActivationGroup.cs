using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR 
using UnityEditor;
#endif

namespace PlatformerPro
{
	/// <summary>
	/// Activation groups provide a set of ID's that can be turned on of off (activated or deactivated).
	/// The state of these can be used to drive things like the active weapon, or character powers
	/// 
	/// They have some overlap with power-ups but unlike power-ups the group and items have no behaviour.
	/// 
	/// Becasue Activation groups DO NOT need to be attached to a character they can also be used
	/// to drive custom puzzle state.
	/// </summary>
	public class ActivationGroup : Persistable 
	{
		/// <summary>
		/// Unique name for this group.
		/// </summary>
		[Tooltip ("Unique name for this group.")]
		public string groupName;

		/// <summary>
		/// Does this group only allow one item to be active at a time.
		/// </summary>
		[Tooltip ("Does this group only allow one item to be active at a time?")]
		public bool onlyOneActive;

		/// <summary>
		/// If true send events indicating activation when the data is loaded. If false don't.
		/// </summary>
		[Tooltip ("If true send events indicating activation when the data is loaded. If false don't.")]
		public bool sendActivationEventsOnLoad;

		/// <summary>
		/// If true the activation items must match an item in the ItemManager or they will not be activatable.
		/// </summary>
		[Tooltip ("If true the activation items must match an item held by the Character or they will not be activatable.")]
		public bool requireMatchingItem;

		/// <summary>
		/// If not empty the matching item will be activated by default.
		/// </summary>
		[Tooltip ("If not empty the matching item will be activated by default. Note that defaultItems are not matched against ItemManager.")]
		public string defaultItem;

		/// <summary>
		/// The list of items in this group.
		/// </summary>
		[Tooltip ("The list of items in this group.")]
		public List<string> items;

		/// <summary>
		/// The items currently active.
		/// </summary>
		protected List<string> activeItems;

		/// <summary>
		/// Cached reference to character if we can find one.
		/// </summary>
		protected Character character;

		/// <summary>
		/// Cached reference to item manager used when requireMatchingItem is true.
		/// </summary>
		protected ItemManager itemManager;

		/// <summary>
		/// Cached reference to a character loader.
		/// </summary>
		protected PlatformerProGameManager characterLoader;

		/// <summary>
		/// A list of items to deactive when we operate in 'only one' mode.
		/// Used to avoid creating a new list each time this is called.
		/// </summary>
		protected List<string> itemsToDeactivate;

		/// <summary>
		/// The player preference identifier.
		/// </summary>
		public const string UniqueDataIdentifier = "ActivationGroupData";

		#region events

		/// <summary>
		/// Item activated.
		/// </summary>
		public event System.EventHandler <ActivationEventArgs> Activated;
		
		/// <summary>
		/// Raises the activated event.
		/// </summary>
		/// <param name="itemId">Item identifier.</param>
		virtual protected void OnActivated(string itemId)
		{
			if (Activated != null)
			{
				Activated(this, new ActivationEventArgs(groupName, itemId));
			}
		}

		/// <summary>
		/// Item deactivated.
		/// </summary>
		public event System.EventHandler <ActivationEventArgs> Deactivated;
		
		/// <summary>
		/// Raises the deactivated event.
		/// </summary>
		/// <param name="itemId">Item identifier.</param>
		virtual protected void OnDeactivated(string itemId)
		{
			if (Deactivated != null)
			{
				Deactivated(this, new ActivationEventArgs(groupName, itemId));
			}
		}

		/// <summary>
		/// All items activated.
		/// </summary>
		public event System.EventHandler <ActivationEventArgs> AllActivated;

		/// <summary>
		/// Raises the all activated event.
		/// </summary>
		virtual protected void OnAllActivated()
		{
			if (AllActivated != null)
			{
				AllActivated(this, new ActivationEventArgs(groupName, ""));
			}
		}

#endregion

		void Awake() {
			itemsToDeactivate = new List<string>();
			activeItems = new List<string> ();
		}

		/// <summary>
		/// Unity start hook.
		/// </summary>
		void Start()
		{
			Init ();
		}

		/// <summary>
		/// Unity OnDestory event.
		/// </summary>
		void OnDestroy()
		{
			if (characterLoader != null) characterLoader.CharacterLoaded -= HandleCharacterLoaded;
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual  protected void Init()
		{
			itemsToDeactivate = new List<string>();
			// Try to find a character (we don't need one but we wont be able to do some persistence things if we don't have one)
			character = GetComponentInParent<Character>();
			if (character == null)
			{
				characterLoader = FindObjectOfType<PlatformerProGameManager>();
				if (characterLoader != null) characterLoader.CharacterLoaded += HandleCharacterLoaded;
			}
			// Get item manager ref
			if (character != null && requireMatchingItem)
			{
				if (character != null) 
				{
					itemManager = character.ItemManager;
				}
				if (itemManager == null && characterLoader == null) 
				{
					Debug.LogWarning("ActivationGroup could not find an Item Manager but has requireMatchingItem set to true!");
					enabled = false;
				}
			}

			base.ConfigureEventListeners ();

			activeItems = new List<string> ();
			if (defaultItem != null && items.Contains(defaultItem)) activeItems.Add (defaultItem);

		}

		/// <summary>
		/// Returns true if given item is activated.
		/// </summary>
		/// <param name="itemId">Item identifier.</param>
		virtual public bool IsActive(string itemId)
		{
			if (activeItems.Contains(itemId)) return true;
			return false;
		}

		/// <summary>
		/// Gets a list of all items. Note this creates a new list so should not be used every frame.
		/// </summary>
		virtual public List<string> AllItems()
		{
			return new List<string> (items);
		}

		/// <summary>
		/// Gets a list of all active items. Note this creates a new list so should not be used every frame.
		/// </summary>
		/// <param name="itemId">Item identifier.</param>
		virtual public List<string> AllActiveItems()
		{
			return new List<string> (activeItems);
		}

		/// <summary>
		/// Activate the specified itemId.
		/// </summary>
		/// <param name="itemId">Item identifier.</param>
		/// <returns>true if the item was activated or the item was already activated</returns>
		virtual public bool Activate(string itemId)
		{
			if (!items.Contains (itemId))
			{
				Debug.LogWarning("Tried to activate item " + itemId + " but it was not found in the list of items.");
				return false;
			}
			if (activeItems.Contains (itemId)) return true;
			if (requireMatchingItem)
			{
				if (!itemManager.HasItem(itemId)) return false;
			}
			if (onlyOneActive)
			{
				itemsToDeactivate.Clear ();
				foreach(string i in activeItems)
				{
					if (i != itemId) itemsToDeactivate.Add (i);
				}
				foreach(string i in itemsToDeactivate)
				{
					activeItems.Remove(i);
					OnDeactivated(i);
				}
			}
			activeItems.Add (itemId);
			OnActivated(itemId);
			if (activeItems.Count == items.Count) OnAllActivated();
			if (SaveOnChange) Save (this);
			return true;
		}

		/// <summary>
		/// Activates the next item in the group (only if onlyOneActive == true). Takes in to account conditions 
		/// such as inventory, so if item 1 is active and there is no ammo for item 2 , calling Next() will move on to item 3.
		/// </summary>
		virtual public void Next()
		{
			if (!onlyOneActive)
			{
				Debug.LogWarning("Called Next() on an ActivationGroup that does not have 'onlyOneActive' set to true.");
				return;
			}
			if (activeItems.Count == 0)
			{
				activeItems.Add (defaultItem);
				return;
			}
			int loopCounter = 0;
			bool activated = false;
			int currentIndex = items.IndexOf (activeItems [0]) + 1;
			if (currentIndex >= items.Count) currentIndex = 0;
			while (loopCounter < items.Count && !activated)
			{
				activated = Activate (items[currentIndex]);
				loopCounter++;
				currentIndex++;
				if (currentIndex >= items.Count) currentIndex = 0;
			}
#if UNITY_EDITOR
			if (!activated) Debug.LogWarning("Called Next() but no items could be activated");
#endif
		}

		
		/// <summary>
		/// Activates the previous item in the group (only if onlyOneActive == true). Takes in to account conditions 
		/// such as inventory, so if item 3 is active and there is no ammo for item 2 , calling previous() will move on to item 1.
		/// </summary>
		virtual public void Previous()
		{
			if (!onlyOneActive)
			{
				Debug.LogWarning("Called Previous() on an ActivationGroup that does not have 'onlyOneActive' set to true.");
				return;
			}
			if (activeItems.Count == 0)
			{
				activeItems.Add (defaultItem);
				return;
			}
			int loopCounter = 0;
			bool activated = false;
			int currentIndex = items.IndexOf (activeItems [0]) - 1;
			if (currentIndex < 0) currentIndex = items.Count -1;
			while (loopCounter < items.Count && !activated)
			{
				activated = Activate (items[currentIndex]);
				loopCounter++;
				currentIndex--;
				if (currentIndex < 0) currentIndex = items.Count -1;
			}
			#if UNITY_EDITOR
			if (!activated) Debug.LogWarning("Called Previous() but no items could be activated");
			#endif
		}

		/// <summary>
		/// Deactivate the specified itemId.
		/// </summary>
		/// <param name="itemId">Item identifier.</param>
		/// <returns>true if the item was deactivated or the item was already deactivated</returns>
		virtual public bool Deactivate(string itemId)
		{
			if (!items.Contains (itemId))
			{
				Debug.LogWarning("Tried to deactivate item " + itemId + " but it was not found in the list of items.");
				return false;
			}
			if (!activeItems.Contains (itemId)) return true;
			activeItems.Remove (itemId);
			OnDeactivated (itemId);
			// If only one active and we have a default, reset to the default
			if (onlyOneActive && defaultItem != null && defaultItem != "" && items.Contains(defaultItem))
			{
				activeItems.Add (defaultItem);
				OnActivated(defaultItem);
			}
			if (SaveOnChange) Save (this);
			return true;
		}

		virtual public void Reset()
		{
			activeItems = new List<string> ();
			if (defaultItem != null && items.Contains(defaultItem)) activeItems.Add (defaultItem);
			if (SaveOnChange) Save (this);
		}

		/// <summary>
		/// Handles the character loaded event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event.</param>
		virtual protected void HandleCharacterLoaded (object sender, CharacterEventArgs e)
		{
			// TODO Multiplayer
			character = e.Character;
			// Get item manager ref
			if (requireMatchingItem && itemManager == null)
			{
				if (character != null) 
				{
					itemManager = character.ItemManager;
				}
				if (itemManager == null) 
				{
					Debug.LogWarning("ActivationGroup could not find an Item Manager but has requireMatchingItem set to true!");
					enabled = false;
				}
			}
		}
			
#region Persitable methods

		/// <summary>
		/// Gets the character.
		/// </summary>
		override public Character Character 
		{ 
			get 
			{
				return character;
			}
            set
            {
                character = value;
            }
        }

		/// <summary>
		/// Gets the data to save.
		/// </summary>
		override public object SaveData
		{
			get
			{
				return activeItems;
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
				return UniqueDataIdentifier + "_" + groupName;
			}
		}
		
		/// <summary>
		/// Applies the save data to the object.
		/// </summary>
		override public void ApplySaveData(object t)
		{
			if (t is List<string>)
			{
				this.activeItems = (List<string>)t;
				loaded = true;
				OnLoaded();
				if (sendActivationEventsOnLoad)
				{
					foreach (string a in this.activeItems) 
					{
						OnActivated(a);
					}
				}
			}
			else Debug.LogError("Tried to apply unepxected data: " + t.GetType());
		}
		
		/// <summary>
		/// Get the type of object this Persistable saves.
		/// </summary>
		override public System.Type SavedObjectType()
		{
			return typeof(List<string>);
		}
		
		/// <summary>
		/// Resets the save data back to default.
		/// </summary>
		override public void ResetSaveData()
		{
			activeItems = new List<string> ();
			if (defaultItem != null && items.Contains(defaultItem)) activeItems.Add (defaultItem);
#if UNITY_EDITOR
			Save(this);
#endif
		}

#if UNITY_EDITOR 

		/// <summary>
		/// Track if we should show or not show the save data.
		/// </summary>
		protected bool shouldShowSavedData;

		/// <summary>
		/// Can this persistable object show its saved data?
		/// </summary>
		override public bool ShouldShowSavedData
		{
			get 
			{
				return shouldShowSavedData;
			}
			set 
			{
				shouldShowSavedData = value;
			}
		}

		/// <summary>
		/// Shows currently saved data.
		/// </summary>
		override public void ShowSaveData() {
			object savedData = LoadSavedData (this);
			GUILayout.Label ("Saved (Activated) Items", EditorStyles.boldLabel);
			if (savedData == null)
			{
				EditorGUILayout.HelpBox ("No saved data present", MessageType.None);
				return;
			}
			if (savedData is List<string>)
			{
				if (((List<string>)savedData).Count == 0)
				{
					EditorGUILayout.HelpBox ("Data is present but empty.", MessageType.None);
				}
				EditorGUI.indentLevel++;
				foreach (string s in (List<string>)savedData)
				{
					
					GUILayout.Label (s, EditorStyles.helpBox);
				}
				EditorGUI.indentLevel--;
			} else
			{
				EditorGUILayout.HelpBox ("Saved data is present but of wrong data type. You should probably RESET it.", MessageType.Error);
			}
		}

		#endif

#endregion

	}
}
