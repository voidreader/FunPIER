using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A trigger that combines exit and enter events from multiple triggers and sends them on based on a threshold.
	/// </summary>
	public class TriggerCombiner : Trigger
	{
		/// <summary>
		/// Number of entered but not exited triggers we need to fire an enter event.
		/// </summary>
		[Tooltip ("Number of entered but not exited triggers we need to fire an enter event.")]
		public int threshold = 1;

		/// <summary>
		/// Allow one trigger to be entered more than once.
		/// </summary>
		[Tooltip ("Allow one trigger to be entered more than once. Handy when you want to stack things up on a pressure sensor for example.")]
		public bool allowStacking = true;

		/// <summary>
		/// List of all the triggers that have entered but not exited.
		/// </summary>
		protected List<Trigger> triggers;

		/// <summary>
		/// Are we in entered or exited state.
		/// </summary>
		protected bool isEntered;

		/// <summary>
		/// Unity Awake hook.
		/// </summary>
		void Awake() 
		{
			triggers = new List<Trigger>();
		}

		/// <summary>
		/// Called when another trigger is entered.
		/// </summary>
		/// <param name="trigger">Trigger.</param>
		/// <param name="mob">Mob.</param>
		public void EnteredTrigger(Trigger trigger, Character character)
		{
			if (trigger != null && (allowStacking || !triggers.Contains (trigger))) triggers.Add (trigger);
			CheckConditions(character);
		}

		/// <summary>
		/// Called when another trigger is left.
		/// </summary>
		/// <param name="trigger">Trigger.</param>
		/// <param name="mob">Mob.</param>
		public void LeftTrigger(Trigger trigger, Character character)
		{
			if (triggers.Contains (trigger)) triggers.Remove (trigger);
			CheckConditions(character);
		}

		/// <summary>
		/// Checks the conditions are met to fire this event.
		/// Note this only checks the treshhold. The vent still wont be fired if other trigger conditions are not met).
		/// </summary>
		/// <param name="character">Character.</param>
		protected void CheckConditions(Character character)
		{
			if (isEntered)
			{
				if (triggers.Count < threshold) 
				{
					LeaveTrigger(character);
					isEntered = false;
				}
			}
			else
			{
				if (triggers.Count >= threshold) 
				{
					EnterTrigger(character);
					isEntered = true;
				}
			}
		}
#if UNITY_EDITOR
		/// <summary>
		/// Unity gizmo hook, draw the connection.
		/// </summary>
		void OnDrawGizmos()
		{
			if (receivers != null)
			{
				Gizmos.color = Trigger.GizmoColor;
				
				foreach (TriggerTarget receiver in receivers) 
				{
					if (receiver != null && receiver.receiver != null) Gizmos.DrawLine(transform.position, receiver.receiver.transform.position);
				}
			}
		}
#endif
	}
}