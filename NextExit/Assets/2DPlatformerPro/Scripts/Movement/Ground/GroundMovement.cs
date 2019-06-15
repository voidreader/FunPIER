using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A wrapper class for handling moving on the ground that proxies the movement function
	/// to the desired implementation.
	/// </summary>
	public class GroundMovement : BaseMovement <GroundMovement>
	{

		#region movement info constants and properties
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Ground Movement";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "The base ground movement class, you shouldn't be seeing this did you forget to create a new MovementInfo?";

		/// <summary>
		/// Static movement info used by the editor.
		/// </summary>
		new public static MovementInfo Info
		{
			get
			{
				return new MovementInfo(Name, Description);
			}
		}

		#endregion

		/// <summary>
		/// Gets a value indicating whether this movement wants to control the movement on the ground.
		/// Default is false, with control falling back to default ground. Override if you want particular control.
		/// </summary>
		/// <value><c>true</c> if this instance wants control; otherwise, <c>false</c>.</value>
		virtual public bool WantsGroundControl()
		{
			return false;
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="PlatformerPro.Movement"/> can
		/// support automatic sliding based on the characters slope.
		/// </summary>
		virtual public bool SupportsSlidingOnSlopes
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		///  Applies slope force.
		/// </summary>
		/// <returns><c>true</c>, if slope force was applyed, <c>false</c> otherwise.</returns>
		virtual public void ApplySlopeForce()
		{
		}

		/// <summary>
		/// Snaps the character to the ground.
		/// </summary>
		virtual protected void SnapToGround ()
		{
			BaseCollisions baseCollisions = character.GetComponent<BaseCollisions>();
			float deltaToApply = character.groundedLookAhead;
			bool apply = false;
			// Note that if called from DoMove() these are the collisions from the last frame not the current one as they haven't yet been updated.
			for (int i = 0; i < character.CurrentCollisions.Length; i++)
			{
				if (character.Colliders[i].RaycastType == RaycastType.FOOT && baseCollisions.ClosestColliders[i] != -1)
				{
					float penetration = character.Colliders[baseCollisions.ClosestColliders[i]].WorldExtent.y - character.CurrentCollisions[i][baseCollisions.ClosestColliders[i]].point.y; 
					if (penetration < 0) 
					{
						apply = false; 
						break;
					}
					else {
						if (penetration <= deltaToApply) {
							apply = true;
							deltaToApply = penetration;
						}
					}
				}
			}

			if (apply)
			{
				character.Translate(0, -deltaToApply, true);
			}
		}
	}

}