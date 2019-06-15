using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Base class used by things that parent when you stand on them.
	/// </summary>
	public class ParentOnStandPlatform : Platform
	{	

		/// <summary>
		/// Should we parent when the head collides with this platform (used when you have hang from ceiling).
		/// </summary>
		public bool parentOnHeadCollission;

		/// <summary>
		/// Unit update hook.
		/// </summary>
		void Update()
		{
			if (Activated) DoMove();
		}

		/// <summary>
		/// Do the moving.
		/// </summary>
		virtual protected void DoMove () 
		{
		}

		/// <summary>
		/// Called when the character is parented to this platform.
		/// </summary>
		override public void Parent(IMob character)
		{
			if (automaticActivation == PlatformActivationType.ACTIVATE_ON_STAND)
			{
				Activated = true;
				OnPlatformActivated(character);
			}
			if (automaticDeactivation == PlatformDeactivationType.DEACTIVATE_ON_STAND)
			{
				Activated = false;
				OnPlatformDeactivated(character);
			}
		}
		
		/// <summary>
		/// Called when the character is unparented from this platform.
		/// </summary>
		override public void UnParent(IMob character)
		{
			if (automaticActivation == PlatformActivationType.ACTIVATE_ON_LEAVE) {
				Activated = true;
				OnPlatformActivated ((Character)character);

			}
			if (automaticDeactivation == PlatformDeactivationType.DEACTIVATE_ON_LEAVE)
			{
				Activated = false;
				OnPlatformDeactivated((Character)character);
			}
		}
		
		/// <summary>
		/// If the collission is a foot try to parent.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="type">Type of raycast.</param>
		override protected bool CustomCollide(PlatformCollisionArgs args)
		{
			if (args.RaycastCollider.RaycastType == RaycastType.FOOT)
			{
				return true;
			}
			if (parentOnHeadCollission && args.RaycastCollider.RaycastType == RaycastType.HEAD)
			{
				return true;
			}
			return false;
		}
	}
}
