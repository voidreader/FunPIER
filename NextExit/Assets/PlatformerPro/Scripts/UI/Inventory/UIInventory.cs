using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Shows inventory
	/// </summary>
	public class UIInventory : PlatformerProMonoBehaviour 
	{
		[Header ("Behaviour")]
		/// <summary>
		/// What player is this the inventory for.
		/// </summary>
		[Tooltip (" What player is this the inventory for. -1 for any")]
		public int playerId = -1;

		/// <summary>
		/// Close inventory when item used.
		/// </summary>
		[Tooltip ("If true using an item will hide the inventory.")]
		public bool closeWhenItemUsed;

		[Header ("Pointer/Touch")]
		/// <summary>
		/// If true we allow the mouse or touch to select and interact with items.
		/// </summary>
		[Tooltip ("If true we allow the mouse or touch to select and interact with items.")]
		public bool allowPointerInput;

		/// <summary>
		/// If true we allow the mouse or touch to select and interact with items.
		/// </summary>
		[DontShowWhenAttribute("allowPointerInput", true)]
		[Tooltip ("If true we allow the user to rearrange objects by dragging.")]
		public bool allowPointerDrag;

		/// <summary>
		/// If true we allow the mouse or touch to select and interact with items.
		/// </summary>
		[DontShowWhenAttribute("allowPointerInput", true)]
		[Tooltip ("If true we select an item on mouse over.")]
		public bool selectOnPointerHover;

		[Header ("Keyboard/Controller")]

		/// <summary>
		/// If true allow arrow keys/controllers to interact with items.
		/// </summary>
		[Tooltip ("If true allow arrow keys/controllers to interact with items.")]
		public bool allowDirectionalInput;

		/// <summary>
		/// Does pressing the jump button use an item?
		/// </summary>
		[Tooltip ("Does pressing the jump button use an item?")]
		public bool jumpButtonIsUse;

		/// <summary>
		/// What action button triggers the use action (-1 for NONE).
		/// </summary>
		[Tooltip ("What action button triggers the use action (-1 for NONE).")]
		public int useActionButton = 0;

		/// <summary>
		/// What action button triggers the pick action (-1 for NONE).
		/// </summary>
		[Tooltip ("What action button triggers the pick action (-1 for NONE). Pick is used for swapping or " +
				  "combining items in an inventory without a mouse.")]
		public int pickActionButton = -1;

		/// <summary>
		/// What action button triggers the pick action (-1 for NONE).
		/// </summary>
		[Tooltip ("What action button triggers the drop action (-1 for NONE).")]
		public int dropActionButton = -1;

		[Header ("UI Components")]
		/// <summary>
		/// GameObject that holds the slot content. Usually a grid.
		/// </summary>
		[Tooltip ("GameObject that holds the slot content. Usually a grid.")]
		public GameObject slotContentHolder;

        /// <summary>
        /// Prefab used to create slots.
        /// </summary>
        [Tooltip ("Prefab used to create slots. This is optional, you can instead create all the slots yourself.")]
		public GameObject slotPrefab;

		/// <summary>
		/// An optional GameObject used to show the current seleciton.  
		/// </summary>
		[Tooltip ("An optional GameObject used to show the current selection.")]
		public GameObject selectionIndicator;

		/// <summary>
		/// Sprite which will show item being dragged around if pointer dreagging is allowed.
		/// </summary>
		[Tooltip ("Sprite which will show item being dragged around if pointer dragging is allowed.")]
		public Image dragIndicator;

		/// <summary>
		/// The current selection.
		/// </summary>
		protected int currentSelection;

		/// <summary>
		/// Cached list of all the slots.
		/// </summary>
		protected List<UIInventorySlot> slots;

		/// <summary>
		/// Cached character reference.
		/// </summary>
		protected Character character;

		/// <summary>
		/// Cached item manager reference.
		/// </summary>
		protected ItemManager itemManager;

		/// <summary>
		/// Cached inventory reference
		/// </summary>
		protected Inventory inventory;

		/// <summary>
		/// Reference to the screen that shows this inventory (for example the Puase menu screen). Used for auto close.
		/// </summary>
		protected UIScreen screen;



		/// <summary>
		/// Track what the last input key was, so holding the key wont move very fast but only at the
		/// cool down speed. 1 UP, -1 DOWN, -10 LEFT, 10 RIGHT
		/// </summary>
		protected int lastAxisValue;

		/// <summary>
		/// The column count.
		/// </summary>
		protected int columnCount;

		/// <summary>
		/// Stores the item data for the current pick so we don't have to keep looking it up when dragging.
		/// </summary>
		protected static ItemTypeData currentPickData;


		/// <summary>
		/// Tracks focus state.
		/// </summary>
		protected bool Focused 
		{
			get
			{
				return ActiveInventory == this;
			}
		}

		/// <summary>
		/// Constant for the input cool down time.
		/// </summary>
		protected const float inputCoolDownTime = 0.2f;

		/// <summary>
		/// Gets the header string used to describe the component.
		/// </summary>
		/// <value>The header.</value>
		override public string Header
		{
			get
			{
				return "UI Representation of an Inventory with various options for interacting with it.";
			}
		}

		/// <summary>
		/// Unity Awake hook.
		/// </summary>
		void Awake() 
		{
			Init ();
		}

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start() 
		{
			PostInit ();
		}
			
		/// <summary>
		/// Unity update hook.
		/// </summary>
		void Update()
		{
			if (Focused && inputCoolDown > 0) inputCoolDown -= Time.deltaTime;
			if (!slotContentHolder.activeInHierarchy)
			{
				if (CurrentPick != -1) ClearPicked ();
				return;
			}
			if (allowDirectionalInput && Focused)
			{
				ProcessUserInput ();
			}
			if (allowPointerDrag && CurrentPick != -1)
			{
				ShowDragIndicator ();
			}
		}

		/// <summary>
		/// Init this instance. Called from Awake. Use for internal initialisation.
		/// </summary>
		virtual protected void Init()
		{
			
		}

		/// <summary>
		/// Post Init this instance. Called from Start. Use for getting external references.
		/// </summary>
		virtual protected void PostInit()
		{
			if (slotContentHolder != null)
			{
				slots = slotContentHolder.GetComponentsInChildren<UIInventorySlot> (true).ToList ();
			}
			else
			{
				slots = GetComponentsInChildren<UIInventorySlot> (true).ToList ();
			}
			screen = GetComponentInParent<UIScreen> ();
			if (slotContentHolder == null)
			{
				Debug.LogWarning ("Its recommended you add a slot content holder, this is used for visibility checks");
			}
			PlatformerProGameManager.Instance.CharacterLoaded += HandleCharacterLoaded;
			PlatformerProGameManager.Instance.PhaseChanged += HandlePhaseChange;

		}

		/// <summary>
		/// Shows the icon being dragged.
		/// </summary>
		virtual protected void ShowDragIndicator ()
		{
			if (dragIndicator == null || CurrentPick == -1 || currentPickData == null ) return; 
			dragIndicator.sprite = currentPickData.Icon;
			// Assumes screen space overlay
			dragIndicator.gameObject.transform.position = UnityEngine.Input.mousePosition;
//			RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out pos);
//			transform.position = myCanvas.transform.TransformPoint(pos);
		}

		/// <summary>
		/// Handles user input.
		/// </summary>
		virtual protected void ProcessUserInput()
		{
         
			if (!Focused) return;
			int newSelection = currentSelection;
			if (character == null || character.Input == null) return;
			if (lastAxisValue == 1 && character.Input.VerticalAxisDigital != 1) inputCoolDown = 0;
			if (lastAxisValue == -1 && character.Input.VerticalAxisDigital != -1) inputCoolDown = 0;
			if (lastAxisValue == 10 && character.Input.HorizontalAxis != 1) inputCoolDown = 0;
			if (lastAxisValue == -10 && character.Input.HorizontalAxis != -1) inputCoolDown = 0;
			lastAxisValue = 0;
			if (inputCoolDown <= 0)
			{
				// Process any custom navigation to move to a different slot, etc
				if (currentSelection != -1 && slots [currentSelection].navigation != null)
				{
					for (int i = 0; i < slots [currentSelection].navigation.Length; i++)
					{
                        if (ProcessNavigation (slots [currentSelection].navigation [i]))
						{
							inputCoolDown = inputCoolDownTime;
							return;
						}
					}
				}
				if (character.Input.VerticalAxisDigital == 1)
				{
					inputCoolDown = inputCoolDownTime;
					lastAxisValue = 1;
					newSelection -= columnCount;
					if (newSelection < 0)
						newSelection = slots.Count - columnCount + currentSelection;
					SelectSlotAt (newSelection);
				} else if (character.Input.VerticalAxisDigital == -1)
				{
					inputCoolDown = inputCoolDownTime;
					lastAxisValue = -1;
					newSelection += columnCount;
					if (newSelection >= slots.Count)
						newSelection = (currentSelection % columnCount);
					SelectSlotAt (newSelection);
				} else if (character.Input.HorizontalAxisDigital == 1)
				{
					inputCoolDown = inputCoolDownTime;
					lastAxisValue = 10;
					if ((currentSelection % columnCount) == (columnCount - 1))
					{
						newSelection -= (columnCount - 1);
					} else
					{
						newSelection++;
					}
					SelectSlotAt (newSelection);
				} else if (character.Input.HorizontalAxisDigital == -1)
				{
					inputCoolDown = inputCoolDownTime;
					lastAxisValue = -10;
					if ((currentSelection % columnCount) == 0)
					{
						newSelection += (columnCount - 1);
					} else
					{
						newSelection--;
					}
					SelectSlotAt (newSelection);
				} 
				else if (dropActionButton != -1 &&
				         (character.Input.GetActionButtonState (dropActionButton) == ButtonState.DOWN))
				{
					if (CurrentPick == -1)
					{
						DropItemAt (currentSelection);
					} 
					else
					{
						ClearPicked ();
						character.ItemManager.OnInventoryChanged ();
					}
				}
				else if (currentSelection != -1 &&
						 ((jumpButtonIsUse && character.Input.JumpButton == ButtonState.DOWN) ||
						  (character.Input.GetActionButtonState (useActionButton) == ButtonState.DOWN)))
				{
					if (CurrentPick == -1)
					{
						ActivateItemAt (currentSelection);
					}
					else
					{
						ClearPicked ();
						character.ItemManager.OnInventoryChanged ();
					}
				}
				else if (currentSelection != -1 &&
				     	 (character.Input.GetActionButtonState (pickActionButton) == ButtonState.DOWN))
				{
					DoPicked(this, currentSelection);
				}
			}
		}

		virtual protected bool ProcessNavigation(UIInventoryNavigation navigation)
		{
			switch (navigation.navigationDirection)
			{
			case NavigationDirection.UP:
				if (character.Input.VerticalAxisDigital == 1) 
				{
					UIInventory.SetActiveInventoryAndPosition (navigation.gotoInventory, navigation.slotIndex);
					return true;
				}
				break;
			case NavigationDirection.DOWN:
				if (character.Input.VerticalAxisDigital == -1) 
				{
					UIInventory.SetActiveInventoryAndPosition (navigation.gotoInventory, navigation.slotIndex);
					return true;
				}
				break;
			case NavigationDirection.RIGHT:
				if (character.Input.HorizontalAxisDigital == 1) 
				{
					UIInventory.SetActiveInventoryAndPosition (navigation.gotoInventory, navigation.slotIndex);
					return true;
				}
				break;
			case NavigationDirection.LEFT:
				if (character.Input.HorizontalAxisDigital == -1) 
				{
					UIInventory.SetActiveInventoryAndPosition (navigation.gotoInventory, navigation.slotIndex);
					return true;
				}
				break;
			}

			return false;
		}
							
		/// <summary>
		/// Handles the character being loaded.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event args.</param>
		virtual protected void HandleCharacterLoaded (object sender, CharacterEventArgs e)
		{
			if (playerId == PlatformerProGameManager.ANY_PLAYER || playerId == e.Character.PlayerId)
			{
				character = e.Character;
				itemManager = e.Character.ItemManager;
				inventory = e.Character.Inventory;
				// MULTIPLAYER TODO inventory for players being added later
				// if (inventory != null && PlatformerProGameManager.Instance.GamePhase == GamePhase.READY)  CreateInventory ();
				itemManager.ItemCollected += HandleItemChanges;
				itemManager.ItemConsumed += HandleItemChanges;
				itemManager.ItemMaxUpdated += HandleItemChanges;
				itemManager.InventoryChanged += HandleItemChanges;
			}
		}

		/// <summary>
		/// Handle an item change by updating iventory UI.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		virtual protected void HandleItemChanges (object sender, System.EventArgs e)
		{
			// TODO We could ensure this is only done once in late update if it becomes a performance issue
			// (it probably wont)
			UpdateInventory ();
		}

		/// <summary>
		/// Handle the game phase by looking for READY phase and creating a UI when ready.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		virtual protected void HandlePhaseChange (object sender, GamePhaseEventArgs e)
		{
			if (e.Phase == GamePhase.READY && inventory != null)  CreateInventory ();
		}
			
		/// <summary>
		/// Updates the inventory slots to match Inventory content.
		/// </summary>
		virtual public void UpdateInventory ()
		{
			if (slots == null || inventory == null) return;
			for (int i = 0; i < slots.Count; i++)
			{
				slots [i].UpdateWithItem (i, inventory.GetItemAt(i), currentSelection == i, CurrentPick == i && CurrentPickInventory == this);
			}
            if (Focused && currentSelection == -1) SelectFirstAvailable ();
		}

		/// <summary>
		/// Selects the first available slot.
		/// </summary>
		virtual protected void SelectFirstAvailable()
		{
			// Can't select an item if we don't have focus
			if (!Focused)
			{
				if (currentSelection != -1) slots [currentSelection].UpdateSelection (false);
				currentSelection = -1;
				return;
			}
			// Make sure we select an item if one is available
			ItemInstanceData data = currentSelection == -1 ? null : inventory.GetItemAt (currentSelection);
			if (currentSelection == -1 || (!inventory.noGaps && (data == null || data.amount == 0)))
			{
				if (currentSelection != -1) slots [currentSelection].UpdateSelection (false);
				currentSelection = -1;
				int pos = 0;
				while (currentSelection == -1 && pos < slots.Count)
				{ 
					SelectSlotAt(pos);
					pos++;
				}
			}
		}

		/// <summary>
		/// Creates the inventory UI.
		/// </summary>
		virtual protected void CreateInventory()
		{
            if (slots.Count < inventory.inventorySize)
			{
				for (int i = slots.Count; i < inventory.inventorySize; i++)
				{
					CreateSlot ();
				}
			}
			else if (slots.Count > inventory.inventorySize)
			{
				for (int i = slots.Count; i >= inventory.inventorySize; i--)
				{
					RemoveLastSlot();
				}
			}
			for (int i = 0; i < slots.Count; i++)
			{
				// Order just in case
				slots [i].transform.SetAsLastSibling ();
				slots [i].UpdateWithItem (i, inventory.GetItemAt(i), false, false);
			}
			// Select an item if we don't allow empty selection
			currentSelection = -1;
			CurrentPick = -1;
			SelectFirstAvailable ();
			GridLayoutGroup layout = null;
			if (slotContentHolder != null)
			{
				layout = slotContentHolder.GetComponentInChildren<GridLayoutGroup> ();
				if (layout != null)
				{
					columnCount = layout.constraintCount;
				}
			}
			if (columnCount == 0) columnCount = 1;
			if (columnCount > slots.Count) columnCount = slots.Count;
		}


		/// <summary>
		/// Creates a new slot.
		/// </summary>
		virtual protected void CreateSlot()
		{
			if (slotPrefab == null)
			{
				Debug.LogWarning ("Inventory UI doesn't have a slot prefab and the number of slots don't match the inventory size");
				return;
			}
			GameObject go = GameObject.Instantiate (slotPrefab);
			go.transform.SetParent (slotContentHolder.transform, false);
			UIInventorySlot slot = go.GetComponent<UIInventorySlot> ();
			if (slot == null)
			{
				Debug.LogWarning ("Inventory UI prefab doesn't contain a UIInventorySlot");
				return;
			}
			slots.Add (slot);
		}

		/// <summary>
		/// Removes the last slot from the list and destroys its GameObject.
		/// </summary>
		virtual protected void RemoveLastSlot()
		{
			// We could just disable but given this should happen rarely destory should be fine
			UIInventorySlot slot = slots.Last ();
			slots.Remove (slot);
			Destroy (slot.gameObject);
		}

		/// <summary>
		/// CHanges the selection to the given slot.
		/// </summary>
		/// <param name="index">Index.</param>
		virtual public void SelectSlotAt(int index)
		{
			ActiveInventory = this;
			// if (currentSelection == index) return;
			if (index < 0) index = -1;
			if (index >= slots.Count) index = slots.Count - 1;
			ItemInstanceData data = inventory.GetItemAt (index);
			if (!inventory.noGaps || (data != null && data.amount > 0))
			{
				if (currentSelection >= 0 && currentSelection < slots.Count) slots [currentSelection].UpdateSelection (false);
				currentSelection = index;
				if (index >= 0 && index < slots.Count) slots [index].UpdateSelection (true);
			}
		}

		/// <summary>
		/// Changes the selection to the given slot.
		/// </summary>
		/// <param name="index">Index.</param>
		virtual public void DoPicked(UIInventory target, int index)
		{
            if (target == null) return;
			// Always do the work on the targetted inventory
			if (target != this)
			{
				target.DoPicked(target, index);
				return;
			}
			// Handle no current pick by making a new pick
			if (CurrentPickInventory == null)
			{
				ItemInstanceData data = GetDataForSlot (index);
				if (data != null && data.amount > 0)
				{
					CurrentPick = index;
					CurrentPickInventory = this;
					currentPickData = ItemTypeManager.Instance.GetTypeData (data.ItemId);
					slots [index].UpdatePicked (true);
					return;
				}
			}
			else
			{
				// If pick is same as current slot, unpick
				if (index == -1 || (index == CurrentPick && CurrentPickInventory == target))
				{
					if (CurrentPick >= 0 && CurrentPick < slots.Count) slots [CurrentPick].UpdatePicked (false);
					currentPickData = null;
					CurrentPick = -1;
					CurrentPickInventory = null;
					if (dragIndicator != null) dragIndicator.gameObject.SetActive (false);
					return;
				}
				// Otherwise handle the pick action
			    DoPickAction(target, index);
				ClearPicked ();
			}
		}

		/// <summary>
		/// Once we have two valid picks selected, this tries to do an appropriate action. 
		/// </summary>
		/// <returns><c>true</c>, if pick action was doable, <c>false</c> otherwise</returns>
		/// <param name="target">Target UI.</param>
		/// <param name="index">Index of slot in target UI.</param>
		virtual protected bool DoPickAction(UIInventory target, int index)
		{
			// If target is self switch positions
			if (CurrentPickInventory == target && target == this)
			{
				inventory.SwitchPositions (CurrentPick, index);
				return true;
			}
			// Else ask the other component if it knows how to do it
			else if (target != this)
			{
				return target.DoPickAction (target, index);
			}
			// Else ask the other component if it knows how to do it
			else if (CurrentPickInventory != this)
			{
				return CurrentPickInventory.DoPickAction (target, index);
			}
			return false;
		}

		/// <summary>
		/// Clears the current picked slot.
		/// </summary>
		virtual public void ClearPicked()
		{
			if (CurrentPickInventory == null) return;
			if (CurrentPickInventory != this)
			{
				CurrentPickInventory.ClearPicked ();
				return;
			}
			if (CurrentPick != -1)
			{
				slots [CurrentPick].UpdatePicked (false);
				CurrentPick = -1;
				CurrentPickInventory = null;
			}
			if (dragIndicator != null) dragIndicator.gameObject.SetActive (false);
		}

		/// <summary>
		/// Gets the data for slots at index. Null if no data there.
		/// </summary>
		/// <returns>The data for slot.</returns>
		/// <param name="index">Index of slot.</param>
		virtual public ItemInstanceData GetDataForSlot(int index)
		{
			if (index >= 0 && index <= slots.Count)
			{
				ItemInstanceData data = inventory.GetItemAt (index);
				if (data == null || data.amount == 0) return null;
				return data;
			}
			return null;
		}

		/// <summary>
		/// Activate the item in the given slot.
		/// </summary>
		/// <param name="index">Index.</param>
		virtual public void ActivateItemAt(int index)
		{
			ItemInstanceData data = inventory.GetItemAt (index);
			if (data == null || data.amount == 0) return;
			bool result = itemManager.UseItemFromInventorySlot (index);
			if (result && closeWhenItemUsed && screen != null) screen.Hide();
		}

		/// <summary>
		/// Drop the item in the given slot.
		/// </summary>
		/// <param name="index">Index.</param>
		virtual public void DropItemAt(int index)
		{
			ItemInstanceData data = inventory.GetItemAt (index);
			if (data == null || data.amount == 0) return;
			// bool result = 
			itemManager.DropItemFromInventorySlot (index);
		}

		/// <summary>
		/// Handle a slot being clicked.
		/// </summary>
		/// <param name="index">Index of slot.</param>
		virtual public void ClickSlot(int index)
		{
			if (!allowPointerInput) return;
			ClearPicked ();
			if (index == currentSelection) {
				ActivateItemAt (index);
			}
			else
			{
				SelectSlotAt (index);
			}
		}

		/// <summary>
		/// Handle a slot being entered by pointer.
		/// </summary>
		/// <param name="index">Index of slot.</param>
		virtual public void PointerEnteredSlot(int index)
		{
			if (!allowPointerInput) return;
			if (selectOnPointerHover) SelectSlotAt (index);
			OverSlot = index;
			OverSlotInventory = this;
			// TODO Dragging
		}

		virtual public void PointerExitedSlot(int index)
		{
			if (!allowPointerInput) return;
			if (allowPointerDrag && CurrentPick != -1) dragIndicator.gameObject.SetActive (true);
			OverSlot = index;
			OverSlotInventory = null;
		}

		/// <summary>
		///  Handles pointer button being pressed but not clicked.
		/// </summary>
		/// <param name="index">Index.</param>
		virtual public void MouseButtonPressed(int index)
		{
			if (!allowPointerInput) return;
			if (allowPointerDrag)
			{
				DoPicked(this, index);
			}
		}

		/// <summary>
		/// Determines whether this instance is a valid pick target for the CurrentPick
		/// </summary>
		/// <returns><c>true</c> if this instance is valid pick target; otherwise, <c>false</c>.</returns>
		virtual public bool IsPickTarget(ItemInstanceData data, int slotIndex, UIInventory targetInventory)
		{
			if (CurrentPickInventory != this) return CurrentPickInventory.IsPickTarget (data, slotIndex, this);
			return true;
		}

		/// <summary>
		/// Handles pointer being released.
		/// </summary>
		/// <param name="index">Index.</param>
		virtual public void MouseButtonReleased(int index)
		{
			if (!allowPointerInput) return;
			if (allowPointerDrag && CurrentPick != -1 && OverSlot != -1)
			{
				DoPicked (OverSlotInventory, OverSlot);
			}
			ClearPicked ();
		}

		/// <summary>
		/// Takes focus away from this item.
		/// </summary>
		virtual public void FocusAtPosition(int index)
		{
			currentSelection = index;
            UpdateInventory ();
		}

		/// <summary>
		/// Takes focus away from this item.
		/// </summary>
		virtual public void Focus()
		{
			if (currentSelection == -1) SelectFirstAvailable ();
			UpdateInventory ();
		}

		/// <summary>
		/// Gives focus to this item.
		/// </summary>
		virtual public void Defocus()
		{
			if (currentSelection != -1) slots [currentSelection].UpdateSelection (false);
			currentSelection = -1;
			UpdateInventory ();
		}

		#region static proeprties



		/// <summary>
		/// The slot we are currently mousing over.
		/// </summary>
		protected static int OverSlot
		{
			get; 
			set;
		}

		/// <summary>
		/// Stores the inventory that is currenlty mouse overed.
		/// </summary>
		protected static UIInventory OverSlotInventory
		{
			get;
			set;
		}


		/// <summary>
		/// The currently picked item base don position in PickInventory.
		/// </summary>
		protected static int CurrentPick
		{
			get;
			set;
		}

		/// <summary>
		/// Stores the inventory that currently has a pick.
		/// </summary>
		protected static UIInventory CurrentPickInventory
		{
			get;
			set;
		}

		/// <summary>
		/// Get inventory item data of currentpick or null if no pick.
		/// </summary>
		/// <returns>The pick data.</returns>
		public static ItemInstanceData GetPickData()
		{
			if (CurrentPickInventory != null && CurrentPick != -1)
			{
				ItemInstanceData data = CurrentPickInventory.GetDataForSlot (CurrentPick);
				return data;
			}
			return null;
		}

		/// <summary>
		/// The currently active inventory.
		/// </summary>
		protected static UIInventory activeInventory;

		/// <summary>
		/// Stores the currently active inventory.
		/// </summary>
		protected static UIInventory ActiveInventory
		{
			get
			{
				return activeInventory;
			}
			set
			{
				if (activeInventory != value)
				{
					UIInventory previous = activeInventory;
					activeInventory = value;
					if (previous != null)
					{
						previous.Defocus ();
					}
					if (activeInventory != null)
					{
						activeInventory.Focus ();
					}
				}
			}
		}

		/// <summary>
		/// Stores the currently active inventory.
		/// </summary>
		public static void SetActiveInventoryAndPosition(UIInventory value, int index)
		{
			if (activeInventory != value && value != null)
			{
				UIInventory previous = activeInventory;
				activeInventory = value;
				if (previous != null)
				{
					previous.Defocus ();
				}
			}
			if (activeInventory != null)
			{
				activeInventory.FocusAtPosition (index);
			}
		}

		/// <summary>
		/// Timer which stops inputs moving up/down very fast.
		/// </summary>
		protected static float inputCoolDown; 

		#endregion

	}
}