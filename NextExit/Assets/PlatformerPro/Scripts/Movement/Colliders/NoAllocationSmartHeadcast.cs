/**
 * This code is part of Platformer PRO and is copyright John Avery 2014.
 */

using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A raycast collider wrapping a standard 2D raycast in a way that ensures no per frame allocation. That also changes the length of the 
	/// raycast based on the characters y speed. The default feet collider.
	/// </summary>
	public class NoAllocationSmartHeadcast : NoAllocationSmartFeetcast
	{

		/// <summary>
		/// Calculate length based on characters velocity
		/// </summary>
		/// <value>The length.</value>
		override public float Length
		{
			get
			{
#if UNITY_EDITOR
				if (!Application.isPlaying) return minFeetLength;
#endif
				if (character == null)
				{
					Debug.LogError("Smart Raycasts need a Mob (Character) Reference");
					return minFeetLength;
				}
				else
				{
					float actualMinHeadLength = minFeetLength;

					if (character.Velocity.y <= 0) return actualMinHeadLength;
					return actualMinHeadLength + (character.Velocity.y * TimeManager.FrameTime);
				}
			}
			set
			{
				length = value;
			}
		}
	}

}