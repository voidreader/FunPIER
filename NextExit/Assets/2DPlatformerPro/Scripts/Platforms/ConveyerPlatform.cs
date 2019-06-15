using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A platform that moves a character forward at a consant speed.
	/// </summary>
	public class ConveyerPlatform : Platform
	{
		/// <summary>
		/// Speed of the convyer.
		/// </summary>
		[Tooltip ("Speed to move at.")]
		public float speed;

		/// <summary>
		/// The mode.
		/// </summary>
		[Tooltip ("How to apply the speed.")]
		public ConveyorType mode;

		/// <summary>
		/// Init this instance.
		/// </summary>
		override protected void Init()
		{
			base.Init();
		}

		/// <summary>
		/// Do the move.
		/// </summary>
		protected virtual void DoMove()
		{
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
				Debug.Log ("Collide");
				DoMove (args.Character);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Does this platform want to prevent the given movement from moving. Generally implementations
		/// will use the movement.GetType() to restrict specific classes of movement. Only applied
		/// when character is parented to the platform.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="character">Character being checked.</param>
		/// <param name="movement">Movement being checked.</param>
		override public bool SkipMovement(Character character, Movement movement)
		{
			// If directly translating skip ground movements
			if (mode == ConveyorType.DIRECT_TRANSLATE && movement is GroundMovement) return true;
			return false;
		}

		/// <summary>
		/// Move the character.
		/// </summary>
		/// <param name="character">Character.</param>
		virtual protected void DoMove(IMob character)
		{
			switch (mode)
			{
				case ConveyorType.DIRECT_TRANSLATE:
					if (character is Character) ((Character)character).Translate(speed * TimeManager.FrameTime, 0, false);
					break;
				case ConveyorType.SET_SPEED:
					character.SetVelocityX(speed);
					break;
				case ConveyorType.ADD_SPEED_AS_FORCE:
					character.SetVelocityX(character.Velocity.x + (speed * TimeManager.FrameTime));
					break;
			}
		}

		/// <summary>
		/// Draw handles for showing extents
		/// </summary>
		void OnDrawGizmos() {
			Gizmos.color = Platform.GizmoColor;
			Gizmos.DrawLine(transform.position + new Vector3(0, 0.5f,  0),  transform.position + new Vector3(speed > 0 ? 0.5f : -0.5f, 0.5f,  0));
			Gizmos.DrawLine(transform.position + new Vector3(speed > 0 ? 0.25f : -0.25f, 0.25f,  0),  transform.position + new Vector3(speed > 0 ? 0.5f : -0.5f, 0.5f,  0));
			Gizmos.DrawLine(transform.position + new Vector3(speed > 0 ? 0.25f : -0.25f, -0.25f,  0),  transform.position + new Vector3(speed > 0 ? 0.5f : -0.5f, 0.5f,  0));
		}
	}

	/// <summary>
	/// Indicates how the convery moves the character.
	/// </summary>
	public enum ConveyorType
	{
		DIRECT_TRANSLATE,
		SET_SPEED,
		ADD_SPEED_AS_FORCE,
	}
}