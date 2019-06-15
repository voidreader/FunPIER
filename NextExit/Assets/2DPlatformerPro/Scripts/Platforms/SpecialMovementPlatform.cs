using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A platform which triggers a special movement.
	/// </summary>
	public class SpecialMovementPlatform : Platform
	{
		/// <summary>
		/// Movement that this will trigger.
		/// </summary>
		[Tooltip ("Movement that this platform will trigger, should match the special movements Movement value.")]
		public AnimationState movement;

		/// <summary>
		/// Target position for the movements targetted limb.
		/// </summary>
		[Tooltip ("If the movement targets a limb, the limb will be targetted to this posoition.")]
		public Vector3 targetPosition;

		/// <summary>
		/// The direction character must be facing to do this move.
		/// </summary>
		[Tooltip ("The direction character must be facing to do this move. -1 or 1")]
		public int requiredFacingDirection;
		
#if UNITY_EDITOR
		
		/// <summary>
		/// Unity gizmo hook, draw the target position
		/// </summary>
		void OnDrawGizmos()
		{
			Gizmos.color = Trigger.GizmoColor;
			Gizmos.DrawLine (transform.position, targetPosition);
			Gizmos.DrawSphere (targetPosition, 0.25f);
		}
#endif

	}
}