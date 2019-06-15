using UnityEngine;
using System.Collections;
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
				float difference = currentItemPosition.y - pointerImage.transform.position.y + pointerOffset.y;
				Vector3 targetPosition = scrollingContent.transform.position + new Vector3(0, -difference);
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
				float difference = currentItemPosition.y - pointerImage.transform.position.y + pointerOffset.y;
				Vector3 targetPosition = scrollingContent.transform.position + new Vector3(0, -difference);
				tweener.Stop();
				scrollingContent.transform.position = targetPosition;
			}
		}
	}
}
