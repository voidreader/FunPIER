using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Interface used by mobile things like characters and enemies.
	/// </summary>
	public interface IMob
	{
		/// <summary>
		/// The current animation state.
		/// </summary>
		AnimationState AnimationState
		{
			get;
		}

		
		/// <summary>
		/// Returns the direction being faced. 0 for none, 1 for right, -1 for left.
		/// </summary>
		int FacingDirection
		{
			get;
		}
		
		/// <summary>
		/// Returns the direction being faced, but if direction is currently 0 instead returns the direction last faced.
		/// </summary>
		int LastFacedDirection
		{
			get;
		}

		/// <summary>
		/// The characters velocity relative to self.
		/// </summary>
		Vector2 Velocity
		{
			get;
		}
		
		/// <summary>
		/// The characters velocity in the previous frame relative to self.
		/// </summary>
		Vector2 PreviousVelocity
		{
			get;
		}

		/// <summary>
		/// If slopes are on this is the rotation we are rotating towards.
		/// </summary>
		float SlopeTargetRotation
		{
			get;
		}
		
		/// <summary>
		/// If slopes are on this is the rotation we are currently on.
		/// </summary>
		float SlopeActualRotation
		{
			get;
		}

		/// <summary>
		/// If slopes are on this is the rotation we were at last frame.
		/// </summary>
		float  PreviousRotation
		{
			get;
		}

		/// <summary>
		/// Gets the ignored slope. The ignored slope is used primary for internal
		/// physics calculations and is the largest slope that was ignored by the side colliders last frame.
		/// </summary>
		float IgnoredSlope
		{
			get; set;
		}

		/// <summary>
		/// Gets the minimum angle at which geometry is considered a wall.
		/// </summary>
		float MinSideAngle
		{
			get;
		}
		
		/// <summary>
		/// Gets or sets the characters current z layer.
		/// </summary>
		int ZLayer
		{
			get; set;
		}
		/// <summary>
		/// Returns true if we are grounded or false otherwise.
		/// </summary>
		bool Grounded 
		{
			get;
		}

		/// <summary>
		/// If grounded this is the layer of the ground. Undefined if not grounded.
		/// </summary>
		int GroundLayer
		{
			get;
		}

		/// <summary>
		/// Gravity currently acting on the character.
		/// </summary>
		float Gravity
		{
			get;
		}

		/// <summary>
		/// Gets the friction.
		/// </summary>
		float Friction
		{
			get; set;
		}

		/// <summary>
		/// Sets the velocity in x.
		/// </summary>
		void SetVelocityX(float x);

		/// <summary>
		/// Sets the velocity in y.
		/// </summary>
		void SetVelocityY(float y);

		/// <summary>
		/// Translate the character by the supplied amount.
		/// </summary>
		/// <param name="x">The x amount.</param>
		/// <param name="y">The y amount.</param>
		/// <param name="applyYTransformsInWorldSpace">Should Y transforms be in world space instead of realtive to the
		/// character position?</param>
		void Translate (float x, float y, bool applyYTransformsInWorldSpace);

		/// <summary>
		/// Check if a given collider should be ignored in collisions.
		/// </summary>
		/// <returns><c>true</c> if this instance is ignoring he specified collider; otherwise, <c>false</c>.</returns>
		/// <param name="collider">Collider.</param>
		bool IsColliderIgnored(Collider2D collider);

		#region events
		
		/// <summary>
		/// Event for anmation state changes.
		/// </summary>
		event System.EventHandler <AnimationEventArgs> ChangeAnimationState;

		#endregion
	}

}
