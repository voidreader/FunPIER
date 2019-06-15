using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A button on an action bar. Sub classes can be used for switching wepaons, consuming items, or just as a standard ActionButton.
	/// </summary>
	public abstract class UIActionBarButton : MonoBehaviour , IPointerDownHandler, IPointerClickHandler
	{

		[Header ("UI Components")]
		/// <summary>
		/// Content to show when this object is disabled.
		/// </summary>
		[SerializeField]
		protected GameObject disabledContent;

		/// <summary>
		/// Content to show when this object is enabled.
		/// </summary>
		[SerializeField]
		protected GameObject enabledContent;

		/// <summary>
		/// Content to show when this object is active.
		/// </summary>
		[SerializeField]
		protected GameObject activeContent;

		/// <summary>
		/// text for item count if this button is connected to an item.
		/// </summary>
		[SerializeField]
		protected Text itemCountText;

		[Header ("Character References")]

		/// <summary>
		/// Character reference, looked up using CharacterLoader (falling back to Find) if null.
		/// </summary>
		[SerializeField]
		protected Character character;

		/// <summary>
		/// The character loader.
		/// </summary>
		[SerializeField]
		protected PlatformerProGameManager characterLoader;

		[Header ("Controls")]

		[SerializeField]
		protected bool allowDeactivate;

		[SerializeField]
		protected int actionButton;

		[SerializeField]
		protected bool displayOnly;

		[SerializeField]
		protected bool canClickWhenPaused;

		/// <summary>
		/// Cached item manager reference
		/// </summary>
		protected ItemManager itemManager;

		/// <summary>
		/// Cached item ID
		/// </summary>
		protected string itemId;

		/// <summary>
		/// Track if we are active or not.
		/// </summary>
		protected bool isActive;

		/// <summary>
		/// Gets the character reference.
		/// </summary>
		/// <value>The character.</value>
		virtual public Character Character
		{
			get
			{
				return character;
			}
		}

		#region events

		/// <summary>
		/// Occurs when button pressed/clicked and state is ready for activation.
		/// </summary>
		public event System.EventHandler <System.EventArgs> Activated;

		/// <summary>
		/// Raises the activated event.
		/// </summary>
		virtual protected void OnActivated ()
		{
			if (Activated != null)
			{
				Activated(this, null);
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
		/// Unity Update hook.
		/// </summary>
		void Update()
		{
			if (!displayOnly && actionButton >= 0 && Character != null)
			{
				if (character.Input.GetActionButtonState(actionButton) == ButtonState.DOWN) DoPointerClick();
			}
		}

		/// <summary>
		/// Unity destroy hook.
		/// </summary>
		void OnDestroy()
		{
			DoDestroy ();
		}

		/// <summary>
		/// Do the destroy actions (remove event listeners).
		/// </summary>
		virtual protected void DoDestroy()
		{
			if (characterLoader != null) characterLoader.CharacterLoaded -= HandleCharacterLoaded;
			if (itemManager != null && itemId != null && itemId != "")
			{
				itemManager.ItemCollected -= HandleItemChanged;
				itemManager.ItemConsumed -= HandleItemChanged;
				itemManager.Loaded -= HandleItemManagerLoaded;
			}
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void Init()
		{
			GetCharacter ();
		}

		/// <summary>
		/// Gets a character ref or defers to loader event.
		/// </summary>
		virtual protected void GetCharacter()
		{
			if (character != null)
			{
				GetItemManager();
				return;
			}
			if (characterLoader == null)
			{
				characterLoader = FindObjectOfType<PlatformerProGameManager>();
			}
			if (characterLoader != null)
			{
				characterLoader.CharacterLoaded += HandleCharacterLoaded;
			}
			else
			{
				Debug.LogWarning("UIActionBarButton couldn't find a Character or CharacterLoader");
			}
		}

		/// <summary>
		/// Get item manager reference and register listerns.
		/// </summary>
		virtual protected void GetItemManager()
		{
			if (Character == null) return;
			itemManager = Character.GetComponentInChildren<ItemManager>();
			if (itemManager != null && itemId != null && itemId != "")
			{
				itemManager.ItemCollected += HandleItemChanged;
				itemManager.ItemConsumed += HandleItemChanged;
				itemManager.Loaded += HandleItemManagerLoaded;
				UpdateItemCount();
			}
		}

		void HandleItemManagerLoaded (object sender, System.EventArgs e)
		{
			UpdateItemCount();
		}

		virtual protected void UpdateItemCount()
		{
			int count = itemManager.ItemCount (itemId);
			if (itemCountText != null) itemCountText.text = count.ToString ();
			if (count < 1) Disable ();
			else if (!isActive) Enable ();
		}

		virtual protected void HandleItemChanged (object sender, ItemEventArgs e)
		{
			if (e.Type == itemId) UpdateItemCount ();
		}

		/// <summary>
		/// Handles the character being loaded.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		virtual protected void HandleCharacterLoaded (object sender, CharacterEventArgs e)
		{
			Debug.Log ("Set character");
			character = e.Character;
			GetItemManager ();
		}

		/// <summary>
		/// Call to disable intance.
		/// </summary>
		virtual public void Disable()
		{
			// TODO Save some repitition, create a toggle method
			if (disabledContent != null) disabledContent.SetActive (true);
			if (enabledContent != null) enabledContent.SetActive (false);
			if (activeContent != null) activeContent.SetActive (false);
		}

		/// <summary>
		/// call to enable instance.
		/// </summary>
		virtual public void Enable()
		{
			if (disabledContent != null) disabledContent.SetActive (false);
			if (enabledContent != null) enabledContent.SetActive (true);
			if (activeContent != null) activeContent.SetActive (false);
		}

		/// <summary>
		/// Call to activate instance.
		/// </summary>
		virtual public void Activate()
		{
			isActive = true;
			OnActivated ();
			if (disabledContent != null) disabledContent.SetActive (false);
			if (enabledContent != null) enabledContent.SetActive (false);
			if (activeContent != null) activeContent.SetActive (true);
		}

		/// <summary>
		/// Call to activate instance.
		/// </summary>
		virtual public void Deactivate()
		{
			isActive = false;
			Enable ();
		}

		/// <summary>
		/// Does the pointer click. Override to implement.
		/// </summary>
		virtual protected void DoPointerClick()
		{
		}

#region UI events handlers

		virtual public void OnPointerClick(PointerEventData eventData)
		{
			if (!displayOnly) DoPointerClick ();
		}

		virtual public void OnPointerDown(PointerEventData eventData)
		{
		}

#endregion

	}
}
