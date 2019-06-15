using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlatformerPro.Tween;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Extends the basic menu to a scrolling menu view. Relies on a pointer image to set
	/// the selected item position although the image can be empty.
	/// </summary>
	public class UIScrollingMenu : UIBasicMenu
	{
		/// <summary>
		/// The move time.
		/// </summary>
		[Tooltip ("How long it takes to move to the next position.")]
		public float moveTime;

		/// <summary>
		/// The tween mode.
		/// </summary>
		[Tooltip ("Tween type to use.")]
		public TweenMode tweenMode = TweenMode.LINEAR;

		/// <summary>
		/// Should we use a horizontal scroll instead of vertical?
		/// </summary>
		[Tooltip ("Should we use a horizontal scroll instead of vertical?")]
		public bool horizontalMenu;

		/// <summary>
		/// Scrollable content..
		/// </summary>
		protected GameObject scrollingContent;

		/// <summary>
		/// Tweener which handles any moves.
		/// </summary>
		protected PositionTweener tweener;

		/// <summary>
		/// Init this instance.
		/// </summary>
		override protected void Init()
		{
			tweener = GetComponent<PositionTweener> ();
			if (tweener == null) {
				tweener = gameObject.AddComponent<PositionTweener> ();
				tweener.UseGameTime = false;
			}

			scrollingContent = new GameObject ();
			scrollingContent.transform.parent = visibleComponent.transform;
			scrollingContent.transform.localPosition = Vector3.zero;
			scrollingContent.name = "ScrollingContent";

			// Create children inside a new child game object.
			GameObject tmpVisibleComponent = visibleComponent;
			visibleComponent = scrollingContent;
			base.Init ();
			visibleComponent = tmpVisibleComponent;

			if (pointerImage == null)
				Debug.LogWarning ("Pointer image is required for a scrolling menu. If you don't want to use a sprite pick an empty game object to indicate the selection position.");

		}

		/// <summary>
		/// Moves the pointer by scrolling the menu.
		/// </summary>
		/// <param name="newSelection">Index of the newly selected item.</param>
		override protected void UpdatePointer(int newSelection)
		{
			if (pointerImage != null)
			{
				Vector3 currentItemPosition = ((Component)menuItemToRenderer[activeMenuItems[newSelection]]).transform.position;
				float difference = horizontalMenu ? (currentItemPosition.x - pointerImage.transform.position.x + pointerOffset.x) : (currentItemPosition.y - pointerImage.transform.position.y + pointerOffset.y);
				Vector3 targetPosition = scrollingContent.transform.position + (horizontalMenu ? new Vector3(-difference, 0) : new Vector3(0, -difference));
				tweener.TweenWithTime(tweenMode, scrollingContent.transform, targetPosition, moveTime, null);
			}
		}

		/// <summary>
		/// Updates the pointer to point to the given menu item (no tween).
		/// </summary>
		/// <param name="newSelection">New selection.</param>
		override protected void SetPointer(int newSelection)
		{
			if (pointerImage != null)
			{
				Vector3 currentItemPosition = ((Component)menuItemToRenderer[activeMenuItems[newSelection]]).transform.position;
				float difference = horizontalMenu ? (currentItemPosition.x - pointerImage.transform.position.x + pointerOffset.x) : (currentItemPosition.y - pointerImage.transform.position.y + pointerOffset.y);
				Vector3 targetPosition = scrollingContent.transform.position + (horizontalMenu ? new Vector3(-difference, 0) : new Vector3(0, -difference));
				tweener.Stop();
				scrollingContent.transform.position = targetPosition;
			}
		}

		/// <summary>
		/// Creates the UI components.
		/// </summary>
		override protected void CreateUI()
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
						if (horizontalMenu)
						{
							currentPosition.x = ((RectTransform)menuItemGo.transform).offsetMax.x;
						}
						else 
						{
							currentPosition.y = ((RectTransform)menuItemGo.transform).offsetMin.y;
						}
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
		/// Handles user input.
		/// </summary>
		override protected void ProcessUserInput()
		{
			if (!horizontalMenu)
			{
				base.ProcessUserInput();
				return;
			}

			if (lastAxisValue == 1 && input.HorizontalAxisDigital != 1) inputCoolDown = 0;
			if (lastAxisValue == -1 && input.HorizontalAxisDigital != -1) inputCoolDown = 0;
			if (lastAxisValue == 10 && input.VerticalAxis != 1) inputCoolDown = 0;
			if (lastAxisValue == -10 && input.VerticalAxis != -1) inputCoolDown = 0;
			lastAxisValue = 0;
			if (acceptingInput && activeMenuItems.Count > 0)
			{
				// We check for arrow keys first regardless of input config, so user can't break the menu by misconfiguring keys.
				if (inputCoolDown <= 0 && (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow) ||  (input != null && inputCoolDown <= 0 && input.HorizontalAxisDigital == -1)))
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
				else if (inputCoolDown <= 0 && (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow) || (input != null && input.HorizontalAxisDigital == 1)))
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
				// Note we don't upport right/left actions in a horizontal menu
			}
		}


	}
}
