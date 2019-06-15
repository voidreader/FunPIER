using UnityEngine;
using System.Collections;
namespace PlatformerPro
{
	/// <summary>
	/// An object that can be pushed and pulled.
	/// </summary>
	[RequireComponent (typeof(Rigidbody2D))]
	public class PushablePullable : Pushable, IPullable
	{
		virtual public void Pull(IMob character, Vector2 amount)
		{
			if (resetVelocityOnPush) rigidbody2D.velocity = Vector2.zero;
			targetPosition = amount;
			targetSet = true;
		}
	}
}
