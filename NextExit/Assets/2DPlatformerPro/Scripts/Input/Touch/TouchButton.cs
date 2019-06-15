using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// An input button typically used for mobile devices.
	/// </summary>
	public class TouchButton : MonoBehaviour, ITouchButton
	{
		/// <summary>
		/// The touch must start in button.
		/// </summary>
		[Tooltip ("If true the button will only be pressed if the touch started within the collider boundaries. Otherwise it can be pressed by just moving over it.")]
		public bool touchMustStartInButton;

		/// <summary>
		/// Camera being used to draw the input UI.
		/// </summary>
		public Camera inputCamera;

		/// <summary>
		/// The last state.
		/// </summary>
		protected ButtonState buttonState = ButtonState.NONE;

		/// <summary>
		/// If this button was down or held which finger was doing the holding.
		/// </summary>
		protected int fingerId = -1;

		/// <summary>
		/// Cached collider for this button.
		/// </summary>
		protected Collider2D myCollider;

		/// <summary>
		/// A cached button event args that we update so we don't need to allocate.
		/// </summary>
		protected ButtonEventArgs buttonEventArgs;


		#region public methods

		/// <summary>
		/// Gets the state of the button.
		/// </summary>
		virtual public ButtonState ButtonState
		{
			get
			{
				return buttonState;
			}
		}

		#endregion

		#region events

		/// <summary>
		/// Occurs when button state changes. Note that arguments are not a copy to avoid allocation.
		/// </summary>
		public event System.EventHandler<ButtonEventArgs> ButtonStateChanged;

		/// <summary>
		/// Raises the button state changed event.
		/// </summary>
		virtual public void OnButtonStateChanged()
		{
			if (ButtonStateChanged != null)
			{
				buttonEventArgs.UpdateButtonState(buttonState);
				ButtonStateChanged(this, buttonEventArgs);
			}
		}

		#endregion

		#region Unity Hooks

		/// <summary>
		/// Unity Start hook.
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
			UpdateFingerState ();
		}

		#endregion


		#region protected methods

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void Init()
		{
			myCollider = GetComponent<Collider2D> ();
		}

		/// <summary>
		/// Updates the state of the button based on fingers.
		/// </summary>
		virtual protected void UpdateFingerState()
		{
			int touchCount = UnityEngine.Input.touchCount;
			int bestFingerId = -1;
			buttonState = ButtonState.NONE;
			for (int i = 0; i < touchCount; i++)
			{
				Touch currentTouch = UnityEngine.Input.GetTouch(i);
				Vector2 touchPosition = inputCamera.ScreenToWorldPoint(new Vector3(currentTouch.position.x, currentTouch.position.y, -inputCamera.transform.position.z));
				// Handle finger over collider
				if (myCollider.OverlapPoint(touchPosition))
				{
					if (currentTouch.phase == TouchPhase.Began || currentTouch.phase == TouchPhase.Stationary || currentTouch.phase == TouchPhase.Moved)
					{
						if (fingerId == -1 || fingerId == currentTouch.fingerId)
						{
							if (fingerId != -1 || !touchMustStartInButton || currentTouch.phase == TouchPhase.Began)
							{
								// We don't have a finger or this is the current finger... this is the best match
								bestFingerId = currentTouch.fingerId;
								break;
							}
						}
						else
						{
							if (!touchMustStartInButton || currentTouch.phase == TouchPhase.Began)
							{
								// Store the current finger but allow it to be overwritten if we find a better one.
								bestFingerId = currentTouch.fingerId;
							}
						}
					}
				}
			}
			if (bestFingerId != -1)
			{
				if (fingerId == -1)
				{
					// Touch started
					buttonState = ButtonState.DOWN;
					fingerId = bestFingerId;
				}
				else
				{
					// Button held
					buttonState = ButtonState.HELD;
					fingerId = bestFingerId;
				}
			}
			// No match found but we previously had one, button was released.
			else if (bestFingerId == -1 && fingerId != -1)
			{
				Released();
				fingerId = -1;
				buttonState = ButtonState.UP;
			}
		}

		/// <summary>
		/// Called when this  this instance.
		/// </summary>
		virtual protected void Released()
		{
			// Extend with your own implementation if you want to do something on release (like change a sprite).
		}

		#endregion

	}
}