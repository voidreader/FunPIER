using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using PlatformerPro.Tween;

namespace PlatformerPro
{
	/// <summary>
	/// A door that moves upwards a fixed amount when opened.
	/// </summary>
	public class RisingDoor : Door
	{
	
		[Tooltip ("How high should the door raise.")]
		public float raiseAmount = 1;

		[Tooltip ("How fast the door raises.")]
		public float raiseSpeed = 2;

		[Tooltip ("How fast the door closes.")]
		public float closeSpeed = 1;

		[Tooltip ("Tween type to use.")]
		public TweenMode tweenMode = TweenMode.LINEAR;

		/// <summary>
		/// Cached copy of the open position.
		/// </summary>
		protected Vector3 openPosition;

		/// <summary>
		/// Cached copy of the open position.
		/// </summary>
		protected Vector3 closedPosition;

		/// <summary>
		/// Tweener which handles any moves.
		/// </summary>
		protected PositionTweener tweener;

		/// <summary>
		/// Init this door.
		/// </summary>
		override protected void Init() 
		{
			base.Init ();

			if (startOpen)
			{
				openPosition = transform.position;
				closedPosition = openPosition - new Vector3(0, raiseAmount, 0);
			} 
			else
			{
				closedPosition = transform.position;
				openPosition = closedPosition + new Vector3(0, raiseAmount, 0);
			}
			tweener = GetComponent<PositionTweener> ();
			if (tweener == null) {
				tweener = gameObject.AddComponent<PositionTweener> ();
				tweener.UseGameTime = true;
			}
		}

		/// <summary>
		/// Show or otherwise handle the door opening.
		/// </summary>
		override protected void DoOpen(Character character)
		{
			state = DoorState.OPENING;
			OnOpened (character);
			if (tweener.Active) tweener.Stop();
			tweener.TweenWithRate(tweenMode, transform, openPosition, raiseSpeed, DoorOpened);
		}
		
		/// <summary>
		/// Show or otherwise handle the door closing.
		/// </summary>
		override protected void DoClose(Character character)
		{
			state = DoorState.CLOSING;
			OnClosed (character);
			state = DoorState.OPENING;
			OnOpened (character);
			if (tweener.Active) tweener.Stop();
			tweener.TweenWithRate(tweenMode, transform, closedPosition, raiseSpeed, DoorClosed);
		}

		/// <summary>
		/// Handles the tween finish when the door is opened.
		/// </summary>
		virtual public void DoorOpened(Transform t, Vector3 v) 
		{
			state = DoorState.OPEN;
		}
		
		/// <summary>
		/// Handles the tween finish when the door is closed.
		/// </summary>
		virtual public void DoorClosed(Transform t, Vector3 v) 
		{
			state = DoorState.CLOSED;
		}

	}

}