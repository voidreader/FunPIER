using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A door which can be entered.
	/// </summary>
	public class EnterableDoor : Door
	{

		/// <summary>
		/// The door entry method.
		/// </summary>
		[Header ("Door Entry")]
		[Tooltip ("What do we need to do to enter the door.")]
		public DoorInputType doorEntryMethod;

		/// <summary>
		/// Action key to use if door entry method = PRESS_ACTION_KEY.
		/// </summary>
		[Tooltip ("Action key to use if door entry method = PRESS_ACTION_KEY.")]
		public int actionKey = 0;

		#region events

		/// <summary>
		/// Event for door opened.
		/// </summary>
		public event System.EventHandler <DoorEventArgs> Entered;

		/// <summary>
		/// Raises the door opened event.
		/// </summary>
		virtual protected void OnEntered(Character character)
		{
			if (Entered != null)
			{
				Entered(this, new DoorEventArgs(this, character));
			}
		}

		#endregion

		/// <summary>
		/// Called when one of the characters colliders collides with this platform. This should be overriden for platform specific behaviour.
		/// </summary>
		/// <param name="PlatformCollisionArgs">Arguments describing a platform collision.</param>
		/// <returns>true if character should be parented to this platform, otherwise false.</returns>
		override protected bool CustomCollide(PlatformCollisionArgs args)
		{
			if (args.RaycastCollider.RaycastType == RaycastType.FOOT && args.Character is Character) 
			{
				CheckForEnter((Character)args.Character);
			}
			return false;
		}

		/// <summary>
		/// Checks for enter.
		/// </summary>
		/// <param name="character">Character who may be entering.</param>
		virtual protected void CheckForEnter(Character character)
		{
			if (state == DoorState.OPEN)
			{
				switch (doorEntryMethod)
				{
					case DoorInputType.AUTOMATIC:
						DoEnter(character);
						break;
					case DoorInputType.PRESS_UP:
						if (character.Input.VerticalAxisDigital == 1 && character.Input.VerticalAxisState == ButtonState.DOWN) DoEnter(character);
						break;
					case DoorInputType.PRESS_ACTION_KEY:
						if (character.Input.GetActionButtonState(actionKey) == ButtonState.DOWN) DoEnter(character);
						break;
				}
			}
			else
			{
				Open (character);
			}
		}

		/// <summary>
		/// Enter the door.
		/// </summary>
		/// <param name="character">Character.</param>
		virtual protected void DoEnter(Character character)
		{
			OnEntered (character);
		}
	}

	public enum DoorInputType
	{
		AUTOMATIC,
		PRESS_UP,
		PRESS_ACTION_KEY
	}

}