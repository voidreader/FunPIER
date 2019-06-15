using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// UI item for an individual slot in an inventory.
	/// </summary>
	public class UIInventorySlot : PlatformerProMonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
	{
		[Header ("Behaviour")]

        /// <summary>
        /// If true then this slot holds equiment not inventory items.
        /// </summary>
        [Tooltip("If true then this slotacts like a button.")]
        public bool isButton;

        /// <summary>
        /// If true then this slot holds equiment not inventory items.
        /// </summary>
        [DontShowWhenAttribute("isButton", false)]
        [Tooltip ("If true then this slot holds equiment not inventory items.")]
		public bool isEquipmentSlot;

		/// <summary>
		/// What equipment slot is this for?
		/// </summary>
        [DontShowWhenAttribute("isEquipmentSlot", true)]
        [Tooltip ("What equipment slot is this for?")]
		public string equipmentSlot;

		/// <summary>
		/// If true text counters will not be used for uniqiue items.
		/// </summary>
		[Tooltip ("If true text counters will not be used for uniqiue items.")]
        [DontShowWhenAttribute("isButton", false)]
        public bool dontShowCountForUnique;

        /// <summary>
        /// Format string used to format the item count.
        /// </summary>
        [DontShowWhenAttribute("isButton", false)]
        [Tooltip("Format string used to format the item count.")]
		public string counterStringFormat = "{0:D2}";

		[Header ("Navigation")]
		/// <summary>
		/// Custom navigation options.
		/// </summary>
		[DontShowWhenAttribute ("customNavigation", true)]
		[Tooltip ("Specify custom navigation options.")]
		public UIInventoryNavigation[] navigation;

		[Header ("UI Components")]
		/// <summary>
		/// Text field to show the item count.
		/// </summary>
		[Tooltip ("Text field to show the item count.")]
		public Text itemCountText;

		/// <summary>
		/// Optional Text field to show the item max.
		/// </summary>
		[Tooltip ("Optional Text field to show the item max.")]
        [DontShowWhenAttribute("isButton", false)]
        public Text itemMaxText;

		/// <summary>
		/// Image used to show the icon.
		/// </summary>
		[Tooltip ("Image used to show the icon.")]
        [DontShowWhenAttribute("isButton", false)]
        public Image icon;

		/// <summary>
		/// Image used to show the icon.
		/// </summary>
		[Tooltip ("Image used for the background.")]
		public Image background;

		/// <summary>
		/// Optional GameObject to enable when this item is selected.
		/// </summary>
		[Tooltip ("Optional GameObject to enable when this item is selected.")]
		public GameObject selectionIndicator;

		/// <summary>
		/// Optional GameObject to enable when this item is selected.
		/// </summary>
		[Tooltip ("Optional GameObject to enable when this item is picked.")]
		public GameObject pickIndicator;

		[Header ("Color")]
		/// <summary>
		/// Color to set on the background image when the item is non-empty.
		/// </summary>
		[Tooltip ("Color to set on the background image when the item is non-empty.")]
		public Color backgroundColor = Color.white;

		/// <summary>
		/// Color to set on the background image when the item is empty.
		/// </summary>
		[Tooltip ("Color to set on the background image when the item is empty.")]
		public Color emptyBackgroundColor = Color.white;
			
		/// <summary>
		/// The color of the selection.
		/// </summary>
		[Tooltip ("Color to set on the selection indicator when the item is selected and non-empty.")]
		public Color selectionColor = new Color(0,1,0, 0.25f);

		/// <summary>
		/// Color to set on the selection indicator when the item is selected but empty.
		/// </summary>
		[Tooltip ("Color to set on the selection indicator when the item is selected but empty.")]
		public Color selectionEmptyColor = new Color(1,0,0, 0.25f);

		/// <summary>
		/// If we find an image on the selection indicator we will store it and use seleciton colors on it.
		/// </summary>
		protected Image selectionIndicatorImage;

		/// <summary>
		/// Cached reference to last inventory data we applied.
		/// </summary>
		protected ItemInstanceData myData;

		/// <summary>
		/// Cached refrence to the uiInventory used to handle UI events like clicks.
		/// </summary>
		protected UIInventory uiInventory;

		/// <summary>
		/// Cached position reference.
		/// </summary>
		protected int myPosition;

        /// <summary>
        /// Event sent when button is activated.
        /// </summary>
        public event System.EventHandler<System.EventArgs> Activate;

        /// <summary>
        /// Raise the activation event
        /// </summary>
        virtual public void OnActivate()
        {
            if (Activate != null) Activate(this, new System.EventArgs());
        }

        void Start ()
		{
			PostInit();
		}

		/// <summary>
		/// Init post awake. Called from start.
		/// </summary>
		virtual protected void PostInit()
		{
			if (selectionIndicator != null) selectionIndicatorImage = selectionIndicator.GetComponentInChildren<Image> ();
            if (isButton)
            {
                uiInventory = GetComponentInParent<UIInventoryButtonGroup>();
            }
            else if (isEquipmentSlot)
			{
				uiInventory = GetComponentInParent<UIEquipmentManager> ();
			}
			else
			{
				uiInventory = GetComponentInParent<UIInventory> ();
			}
		}

		/// <summary>
		/// Updates slot with given item data.
		/// </summary>
		/// <param name="data">Iventory data.</param>
		/// <param name="isSelected">Are we selected.</param>
		virtual public void UpdateWithItem(int position, ItemInstanceData data, bool isSelected, bool isPicked)
		{
			myPosition = position;
			myData = data;
			if (data == null || data.amount == 0)
			{
				if (itemCountText != null) itemCountText.enabled = false;
				if (icon != null) icon.enabled = false;
				if (itemMaxText != null) itemMaxText.enabled = false;
				if (background != null) background.color = emptyBackgroundColor;
			} 
			else
			{
				ItemTypeData typeData = ItemTypeManager.Instance.GetTypeData(data.ItemId);
				if (data == null)
				{
					Debug.LogWarning ("Couldn't find data for item: " + data.ItemId);
					return;
				}

				if (itemCountText != null) itemCountText.enabled = true;
				if (icon != null) icon.enabled = true;
				if (itemMaxText != null) itemMaxText.enabled = true;
				if (background != null) background.color = backgroundColor;

				if (itemCountText != null) itemCountText.text = string.Format (counterStringFormat, data.amount);
				if (icon != null) icon.sprite = typeData.Icon;
				if (itemMaxText != null) itemMaxText.text = string.Format (counterStringFormat, typeData.itemMax);
				// Hide text if not needed
				if (dontShowCountForUnique)
				{
					if (typeData.itemClass == ItemClass.UNIQUE)
					{
						if (itemCountText != null) itemCountText.enabled = false;
						if (itemMaxText != null) itemMaxText.enabled = false;
					}
					else
					{
						if (itemCountText != null) itemCountText.enabled = true;
						if (itemMaxText != null) itemMaxText.enabled = true;
					}
				}
			}
			UpdateSelection (isSelected);
			UpdatePicked (isPicked);
		}

		/// <summary>
		/// Updates the selection state.
		/// </summary>
		/// <param name="isSelected">Are we selected.</param>
		virtual public void UpdateSelection(bool isSelected)
		{
			// Selection indicator
			if (selectionIndicator != null) selectionIndicator.SetActive (isSelected);
            if (isSelected)
			{
				if (selectionIndicatorImage != null)
				{
                    if (isButton)
                    {
                        selectionIndicatorImage.color = (UIInventory.GetPickData() == null) ? selectionColor : selectionEmptyColor;
                    }
                    else if (UIInventory.GetPickData() != null)
					{
						selectionIndicatorImage.color = (uiInventory.IsPickTarget(myData, myPosition, uiInventory)) ? selectionColor:  selectionEmptyColor ;
					}
					else
					{
						selectionIndicatorImage.color = (myData == null || myData.amount == 0) ? selectionEmptyColor : selectionColor;
					}
				}
			}
		}

		/// <summary>
		/// Updates the picked state.
		/// </summary>
		/// <param name="isSelected">Are we picked.</param>
		virtual public void UpdatePicked(bool isPicked)
		{
			if (pickIndicator != null) pickIndicator.SetActive (isPicked);
		}

        /// <summary>
        /// handles the pointer click event.
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public void OnPointerClick (PointerEventData eventData)
		{
			uiInventory.ClickSlot (myPosition);
		}

		/// <summary>
		/// Handles the pointer enter event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerEnter (PointerEventData eventData)
		{
			uiInventory.PointerEnteredSlot (myPosition);
		}

		/// <summary>
		/// Handles the pointer exit event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerExit (PointerEventData eventData)
		{
			uiInventory.PointerExitedSlot (myPosition);
		}

		/// <summary>
		/// Handles the pointer down event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerDown (PointerEventData eventData)
		{
			uiInventory.MouseButtonPressed (myPosition);
		}

		/// <summary>
		/// Handles the pointer up event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerUp (PointerEventData eventData)
		{
			uiInventory.MouseButtonReleased (myPosition);
		}

	}
	/// <summary>
	/// Stores custom UI navigation data.
	/// </summary>
	[System.Serializable]
	public class UIInventoryNavigation
	{
		public NavigationDirection navigationDirection;
		public UIInventory gotoInventory;
		public int slotIndex;
	}

	/// <summary>
	/// Directions we can navigate in.
	/// </summary>
	public enum NavigationDirection
	{
		UP, DOWN, LEFT, RIGHT
	}
}