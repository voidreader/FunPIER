using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// User interface for a very simple menu made of text items.
	/// Pressing up/down mvoes a pointer through the items and pressing ENTER or SPACE activates selected item.
	/// </summary>
	public class UIBasicMenu : MonoBehaviour, IMenu
	{

		/// <summary>
		/// The pointer image.
		/// </summary>
		[Tooltip ("The part of the menu that is actually drawn. This is activated/deactived as the menu is shown/hidden.")]
		public GameObject visibleComponent;

		/// <summary>
		/// The pointer image.
		/// </summary>
		[Tooltip ("Image used to indicate the selected item.")]
		public GameObject pointerImage;

		/// <summary>
		/// The pointer offset.
		/// </summary>
		[Tooltip ("How much to offset the pointer image from the left-centre of the selected item.")]
		public Vector2 pointerOffset;

		/// <summary>
		/// How fast the pointer moves to the next selection.
		/// </summary>
		[Tooltip ("How fast the pointer moves to the next selection.")]
		public float pointerMoveSpeed;

		/// <summary>
		/// Prefab used for the menu items.
		/// </summary>
		[Tooltip ("Prefab used for the menu items. Must include a text component.")]
		public GameObject menuItemPrefab;

		/// <summary>
		/// The show effects.
		/// </summary>
		[Tooltip ("Effects to play when showing this menu.")]
		public List<FX_Base> showEffects;

		/// <summary>
		/// The hide effects.
		/// </summary>
		[Tooltip ("Effects to play when hiding this menu.")]
		public List<FX_Base> hideEffects;

		/// <summary>
		/// The back menu item.
		/// </summary>
		[Tooltip ("Item to use when the user presses the exit/back key.")]
		public UIMenuItem backItem;

		/// <summary>
		/// Item that starts selected if not initialised.
		/// </summary>
		[Tooltip ("Item that starts selected if not initialised.")]
		public int initialSelection = 0;

		/// <summary>
		/// Cached list of the menu items.
		/// </summary>
		protected List<UIMenuItem> menuItems;

		/// <summary>
		/// Cached list of the active menu items.
		/// </summary>
		protected List<UIMenuItem> activeMenuItems;

		/// <summary>
		/// Index of the currently selected item.
		/// </summary>
		protected int currentlySelectedItem;

		/// <summary>
		/// Dictionary mapping menu items to the assocaited renderer or component.
		/// </summary>
		protected Dictionary<UIMenuItem, IMenuItemRenderer> menuItemToRenderer;

		/// <summary>
		/// Cached list of all inputs found in the scene.
		/// </summary>
		protected Input input;
		
		/// <summary>
		/// Timer which stops inputs moving up/down very fast.
		/// </summary>
		protected float inputCoolDown; 

		/// <summary>
		/// Track what the last input key was, so holding the key wont move very fast but only at the
	    /// cool down speed. 1 UP, -1 DOWN, -10 LEFT, 10 RIGHT
		/// </summary>
		protected int lastAxisValue;

		/// <summary>
		/// Constant for the input cool down time.
		/// </summary>
		protected const float inputCoolDownTime = 0.2f;

		/// <summary>
		/// Are we accepting input on this menu?
		/// </summary>
		protected bool acceptingInput;

		/// <summary>
		/// Currently Active menu.
		/// </summary>
		/// <value>The active menu.</value>
		// TODO: Move this to IMenu which should become an abstract class.
		public static UIBasicMenu ActiveMenu
		{
			get; set;
		}

		/// <summary>
		/// Are we accepting input on this menu?
		/// </summary>
		virtual public bool AcceptingInput
		{
			get
			{
				return acceptingInput;
			}
			set
			{
				// Delay input by one frame to ensure the input that turns on accepting input 
				// does not also triggering a menu action
				if (value) StartCoroutine(StartAcceptInputAfterOneFrame());
				else acceptingInput = false;
			}
		}

		/// <summary>
		/// Gets the index of the current selection.
		/// </summary>
		virtual public int CurrentSelection
		{
			get { return currentlySelectedItem; }
		}

		/// <summary>
		/// Unity Start() hook.
		/// </summary>
		void Start()
		{
			Init ();
		}

		/// <summary>
		/// Unity Update() Hook.
		/// </summary>
		void Update()
		{
			if (inputCoolDown > 0.0f) inputCoolDown -= Time.deltaTime;
			ProcessUserInput ();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void Init()
		{
			// If the menu item is visible at start then it is active and accepting input
			AcceptingInput = visibleComponent.activeInHierarchy;


			menuItems = new List<UIMenuItem> ();
			UIMenuItem[] allMenuItems = GetComponentsInChildren<UIMenuItem> ();
			for (int i = 0; i < allMenuItems.Length; i++)
			{
				// Only collect direct children
				if (allMenuItems[i].transform.parent == transform)
				{
					menuItems.Add(allMenuItems[i]);
				}
			}
			CreateUI ();
			currentlySelectedItem = -1;
			SetPointer (initialSelection);
			Select(initialSelection);
			Input[] inputs = FindObjectsOfType<Input> ();
			if (inputs.Length > 0)
			{
				// Use player 1 input if we can find it
				for (int i = 0; i < inputs.Length; i++)
				{
					if (inputs[i] is StandardInput && ((StandardInput)inputs[i]).dataToLoad == "Player1")
					{
						input = inputs[i];
					}
				}
				// Couldn't work out which input is player 1, use the first one we found
				if (input == null) input = inputs[0];
			}
			if (visibleComponent.activeInHierarchy) Show ();
		}

		/// <summary>
		/// Starts the accepting of input after one frame.
		/// </summary>
		virtual protected IEnumerator StartAcceptInputAfterOneFrame()
		{
			yield return true;
			inputCoolDown = inputCoolDownTime;
			acceptingInput = true;
		}

		/// <summary>
		/// Creates the UI components.
		/// </summary>
		virtual protected void CreateUI()
		{
			Vector3 currentPosition = Vector3.zero;
			activeMenuItems = new List<UIMenuItem>();
			menuItemToRenderer = new Dictionary<UIMenuItem, IMenuItemRenderer>();
			foreach (UIMenuItem menuItem in menuItems)
			{
				GameObject menuItemGo = (GameObject) GameObject.Instantiate(menuItemPrefab);
				menuItemGo.name = "UI_" + menuItem.title;
				IMenuItemRenderer menuItemRenderer = (IMenuItemRenderer) menuItemGo.GetComponentInChildren(typeof(IMenuItemRenderer));
				if (menuItemRenderer == null) Debug.LogWarning ("MenuItem prefab does not include a IMenuItemRenderer component.");
				else
				{
					menuItemRenderer.InitMenuItem (menuItem);
					menuItemToRenderer.Add(menuItem, menuItemRenderer);
					menuItemGo.transform.SetParent(visibleComponent.transform);
					menuItemGo.transform.localScale = Vector3.one;
					if (menuItem.IsActive)
					{
						menuItemGo.SetActive(true);
						((RectTransform)menuItemGo.transform).localPosition = currentPosition;
						currentPosition.y = ((RectTransform)menuItemGo.transform).offsetMin.y;
						activeMenuItems.Add(menuItem);
					}
					else
					{
						menuItemGo.gameObject.SetActive(false);
					}
				}

			}
		}

		/// <summary>
		/// Updates the UI elements by refreshing text titles.
		/// </summary>
		virtual public void Refresh()
		{
			Vector3 currentPosition = Vector3.zero;
			activeMenuItems.Clear ();
			foreach (UIMenuItem menuItem in menuItems)
			{
				if (menuItem.IsActive)
				{
					((Component)menuItemToRenderer[menuItem]).gameObject.SetActive(true);
					((RectTransform)((Component)menuItemToRenderer[menuItem]).gameObject.transform).localPosition = currentPosition;
					currentPosition.y = ((RectTransform)((Component)menuItemToRenderer[menuItem]).gameObject.transform).offsetMin.y;
					menuItemToRenderer[menuItem].Refresh();
					activeMenuItems.Add(menuItem);
				}
				else
				{
					((Component)menuItemToRenderer[menuItem]).gameObject.SetActive(false);
				}
			}
			int actualSelection = (currentlySelectedItem == - 1) ? initialSelection : currentlySelectedItem;
			menuItemToRenderer [menuItems [actualSelection]].Deactivate ();
			SetPointer (actualSelection);
			Select(actualSelection);
		}

		/// <summary>
		/// Show this menu.
		/// </summary>
		virtual public void Show() 
		{
			ActiveMenu = this;
			StartCoroutine (DoShow ());
		}

		/// <summary>
		/// Do the show.
		/// </summary>
		virtual protected IEnumerator DoShow()
		{
			yield return true; //new WaitForSeconds (0.1f);
			visibleComponent.SetActive (true);
			AcceptingInput = true;
			if (showEffects != null && showEffects.Count > 0)
			{
				foreach(FX_Base effect in showEffects)
				{
					effect.StartEffect();
				}
			}
			if (pointerImage != null) pointerImage.SetActive(true);
		}
		
		/// <summary>
		/// Hide this menu.
		/// </summary>
		virtual public void Hide()
		{
			StartCoroutine (DoHide ());
		}

		/// <summary>
		/// Do the hide.
		/// </summary>
		virtual protected IEnumerator DoHide()
		{
			AcceptingInput = false;
			if (pointerImage != null) pointerImage.SetActive(false);
			yield return true;
			if (hideEffects != null && hideEffects.Count > 0)
			{
				foreach(FX_Base effect in hideEffects)
				{
					effect.StartEffect();
				}
			}
			else 
			{
				visibleComponent.SetActive (false);
			}
		}


		/// <summary>
		/// Handles user input.
		/// </summary>
		virtual protected void ProcessUserInput()
		{
			if (lastAxisValue == 1 && input.VerticalAxisDigital != 1) inputCoolDown = 0;
			if (lastAxisValue == -1 && input.VerticalAxisDigital != -1) inputCoolDown = 0;
			if (lastAxisValue == 10 && input.HorizontalAxis != 1) inputCoolDown = 0;
			if (lastAxisValue == -10 && input.HorizontalAxis != -1) inputCoolDown = 0;
			lastAxisValue = 0;
			if (acceptingInput && activeMenuItems.Count > 0)
			{
				// We check for arrow keys first regardless of input config, so user can't break the menu by misconfiguring keys.
				if (inputCoolDown <= 0 && (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow) ||  (input != null && inputCoolDown <= 0 && input.VerticalAxisDigital == 1)))
				{
					if (currentlySelectedItem > 0) 
					{
						Select(currentlySelectedItem - 1);
					}
					inputCoolDown = inputCoolDownTime;
					if (input != null && input.VerticalAxisDigital == 1)
				    {
						if (!input.AnyKey) lastAxisValue = 1;
					}
				}
				else if (inputCoolDown <= 0 && (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow) || (input != null && input.VerticalAxisDigital == -1)))
				{
					if (currentlySelectedItem < activeMenuItems.Count - 1) 
					{
						Select(currentlySelectedItem + 1);
					}
					inputCoolDown = inputCoolDownTime;
					if (input != null && input.VerticalAxisDigital == -1)
					{

						if (!input.AnyKey) lastAxisValue = -1;
					}
				}
				else if(UnityEngine.Input.GetKeyDown(KeyCode.Return) || UnityEngine.Input.GetKeyDown(KeyCode.Space)
				        || (input != null && input.JumpButton == ButtonState.DOWN)
				        || (input != null && input.GetActionButtonState(0) == ButtonState.DOWN)
				        || (input != null && input.GetActionButtonState(1) == ButtonState.DOWN))
				{
					if (activeMenuItems.Count > currentlySelectedItem)
					{
						menuItemToRenderer[activeMenuItems[currentlySelectedItem]].Activate();
						activeMenuItems[currentlySelectedItem].DoAction();
					}
				}
				else if (inputCoolDown <= 0 && (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow) || (input != null && input.HorizontalAxisDigital == 1)))
				{
					if (activeMenuItems.Count > currentlySelectedItem)
					{
						activeMenuItems[currentlySelectedItem].DoRightAction();
					}
					inputCoolDown = inputCoolDownTime;
					if (input != null && input.HorizontalAxisDigital == 1)
					{
						if (!input.AnyKey) lastAxisValue = 10;
					}
				}
				else if (inputCoolDown <= 0 && (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow) || (input != null  && input.HorizontalAxisDigital == -1)))
				{
					if (activeMenuItems.Count > currentlySelectedItem)
					{
						activeMenuItems[currentlySelectedItem].DoLeftAction();
					}
					inputCoolDown = inputCoolDownTime;
					if (input != null && input.HorizontalAxisDigital == -1)
					{
						if (!input.AnyKey) lastAxisValue = -10;
					}
				}
			}
		}

		/// <summary>
		/// Select the specified menuItem.
		/// </summary>
		/// <param name="menuItem">Menu item.</param>
		virtual public void  Select(UIMenuItem menuItem)
		{
			if (activeMenuItems.Contains(menuItem)) Select(activeMenuItems.IndexOf(menuItem));
			else Debug.LogWarning ("Tried to select a menu item that wasn't active.");
		}

		/// <summary>
		/// Select an item.
		/// </summary>
		/// <param name="newSelection">Index of new selection.</param>
		virtual public void Select(int newSelection)
		{
			if (newSelection == currentlySelectedItem) return;

			if (currentlySelectedItem >= 0 && currentlySelectedItem < activeMenuItems.Count)
			{
				menuItemToRenderer[activeMenuItems[currentlySelectedItem]].Deselect();
			}
			if (newSelection >= 0 && newSelection < activeMenuItems.Count)
			{
				menuItemToRenderer[activeMenuItems[newSelection]].Select();
			}
			else if (activeMenuItems.Count > 0)
			{
				newSelection = 0;
				menuItemToRenderer[activeMenuItems[newSelection]].Select();
			}
			UpdatePointer (newSelection);
			currentlySelectedItem = newSelection;
		}

		/// <summary>
		/// Moves the pointer to point to the given menu item.
		/// </summary>
		/// <param name="newSelection">Index of the newly selected item.</param>
		virtual protected void UpdatePointer(int newSelection)
		{
			if (pointerImage != null)
			{
				// If no items are active then hide object else show it
				if (activeMenuItems.Count <= newSelection)
				{
					pointerImage.SetActive(false);
				}
				else
				{
					pointerImage.SetActive(true);
					RectTransform targetRect = (RectTransform)((MonoBehaviour)menuItemToRenderer[activeMenuItems[newSelection]]).transform;
					Vector2 targetPosition = new Vector3(targetRect.offsetMin.x,targetRect.localPosition.y);
					targetPosition = targetPosition + pointerOffset;
					pointerImage.transform.localPosition = targetPosition;
				}
			}
		}


		/// <summary>
		/// Updates the pointer to point to the given menu item (no tween).
		/// </summary>
		/// <param name="newSelection">New selection.</param>
		virtual protected void SetPointer(int newSelection)
		{
			if (pointerImage != null)
			{
				// If no items are active then hide object else show it
				if (activeMenuItems.Count <= newSelection)
				{
					pointerImage.SetActive(false);
				}
				else
				{
					pointerImage.SetActive(true);
					RectTransform targetRect = (RectTransform)((MonoBehaviour)menuItemToRenderer[activeMenuItems[newSelection]]).transform;
					Vector2 targetPosition = new Vector3(targetRect.offsetMin.x,targetRect.localPosition.y);
					pointerImage.transform.localPosition = targetPosition + pointerOffset;
				}
			}
		}

		/// <summary>
		/// Activate the specified menuItem.
		/// </summary>
		/// <param name="menuItem">Menu item.</param>
		virtual public void Activate(UIMenuItem menuItem)
		{
			if (activeMenuItems.Contains(menuItem)) menuItem.DoAction();
		}
	}
}