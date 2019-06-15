using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A platform that calls a trigger when conditions are met. SUch as chracter standing on or headbutting the platform.
	/// </summary>
	[RequireComponent (typeof(TriggerPlatformTrigger))]
	public class TriggerPlatform : Platform
	{
		/// <summary>
		/// Should we trigger enter when the character stands on this platform?
		/// </summary>
		[Tooltip ("Should we EnterTrigger() when a character stands on this platform?")]
		public bool triggeEnterOnStand;

		/// Should we EnterTrigger() when a characters head hits this platform?
		/// </summary>
		[Tooltip ("Should we EnterTrigger() when a characters head hits this platform?")]
		public bool triggedOnHeadButt;

		/// <summary>
		/// Should we trigger enter when the character stands on this platform?
		/// </summary>
		[Tooltip ("Should we LeaveTrigger() when no characters are standing on this platform?")]
		public bool triggeLeaveOnEndStand;

		/// <summary>
		/// Are we being stood on?
		/// </summary>
		protected bool isBeingStoodOn;

		/// <summary>
		/// Were we stood on this frame?
		/// </summary>
		protected int feetCount;

		/// <summary>
		/// Were we stood on this frame?
		/// </summary>
		protected int headCount;

		/// <summary>
		/// Store the last character to stand on this object.
		/// </summary>
		protected Character lastCharacterToStand;

		/// <summary>
		/// Cached trigger.
		/// </summary>
		protected TriggerPlatformTrigger myTrigger;

		void LateUpdate()
		{
			if (isBeingStoodOn && feetCount == 0)
			{
				isBeingStoodOn = false;
				if (triggeLeaveOnEndStand) myTrigger.CharacterLeftTrigger(lastCharacterToStand);
				lastCharacterToStand = null;
			}
			feetCount = 0;
			headCount = 0;
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		override protected void Init()
		{
			base.Init ();
			myTrigger = GetComponent<TriggerPlatformTrigger> ();
		}


		/// <summary>
		/// Respond to colllisions
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="type">Type of raycast.</param>
		/// <returns>true if character should be parented to this platform, otherwise false.</returns>
		override protected bool CustomCollide(PlatformCollisionArgs args)
		{
			if (args.RaycastCollider.RaycastType == RaycastType.FOOT && args.Character.Velocity.y <= 0.0f && args.Character is Character)
			{
				if (!isBeingStoodOn && triggeEnterOnStand) myTrigger.CharacterEnteredTrigger((Character)args.Character);
				isBeingStoodOn = true;
				feetCount++;
				lastCharacterToStand = (Character) args.Character;
			}
			if (args.RaycastCollider.RaycastType == RaycastType.HEAD && args.Character.Velocity.y > 0.0f && args.Character is Character)
			{
				if (headCount == 0 && triggedOnHeadButt) myTrigger.CharacterEnteredTrigger((Character)args.Character);
				headCount++;
			}
			return false;
		}
	}

}