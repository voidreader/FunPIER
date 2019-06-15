/**
 * This code is part of Platformer PRO and is copyright John Avery 2014.
 */

using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Base class from which platforms should extend.
	/// </summary>
	public class Platform : MonoBehaviour
	{
		
		/// <summary>
		/// Default color to use when drawing Platform gizmos.
		/// </summary>
		public static Color GizmoColor = new Color (0.3f, 0.4f, 1.0f, 1.0f);

		/// <summary>
		/// How should the platform activate itself?
		/// </summary>
		[SerializeField]
		[Header ("Platform Settings")]
		protected  PlatformActivationType automaticActivation;
		
		/// <summary>
		/// How should the platform deactivate itself?
		/// </summary>
		[SerializeField]
		protected  PlatformDeactivationType automaticDeactivation;

		/// <summary>
		/// The friction coefficient use -1 for default.
		/// </summary>
		[SerializeField]
		protected float friction = -1;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PlatformerPro.Platform"/> is activated.
		/// </summary>
		virtual public bool Activated
		{
			get; set;
		}

		/// <summary>
		/// Gets the friction coefficent for the platform. Returns -1 if the default should be used.
		/// </summary>
		virtual public float Friction
		{
			get 
			{
				return friction;
			}
		}

		/// <summary>
		/// Unity start hook.
		/// </summary>
		void Start()
		{
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void Init()
		{
			if (automaticActivation == PlatformActivationType.ACTIVATE_ON_START) Activated = true;
		}

		/// <summary>
		/// Called to determine if collision should be ignored. Use for one way platforms or z-ordered platforms
		/// like those found in loops.
		/// </summary>
		/// <returns><c>true</c>, if Collision should be ignored, <c>false</c> otherwise.</returns>
		/// <param name="character">Character.</param>
		/// <param name="collider">Collider.</param>
		virtual public bool IgnoreCollision(Character character, BasicRaycast collider)
		{
			return false;
		}

		/// <summary>
		/// Called when one of the characters colliders collides with this platform.
		/// </summary>
		/// <param name="PlatformCollisionArgs">Arguments describing a platform collision.</param>
		/// <returns>true if character should be parented to this platform, otherwise false.</returns>
		virtual public bool Collide(PlatformCollisionArgs args)
		{
			BaseCollide(args);
			return CustomCollide(args);
		}

		/// <summary>
		/// Called when the character is parented to this platform.
		/// </summary>
		virtual public void Parent()
		{

		}

		/// <summary>
		/// Called when the character is unparented from this platform.
		/// </summary>
		virtual public void UnParent()
		{

		}

		/// <summary>
		/// Does this platform want to prevent the given movement from moving. Generally implementations
		/// will use the movement.GetType() to restrict specific classes of movement. Only applied
		/// when character is parented to the platform.
		/// </summary>
		/// <returns><c>true</c>, if movement should be skipped, <c>false</c> otherwise.</returns>
		/// <param name="character">Character being checked.</param>
		/// <param name="movement">Movement being checked.</param>
		virtual public bool SkipMovement(Character character, Movement movement)
		{
			return false;
		}

		/// <summary>
		/// Gets a value indicating whether this platform can handle multiple characters at once.
		/// </summary>
		/// <value><c>true</c> if this instance is multiplayer ready; otherwise, <c>false</c>.</value>
		virtual public bool IsMultiplayerReady
		{
			get 
			{
				return false;
			}
		}
		
		/// <summary>
		/// Called when one of the characters colliders collides with this platform. This handles basic shared behaviour. If you want to ignore it
		/// you can ovverride Collide() instead of CustomCollide().
		/// </summary>
		/// <param name="PlatformCollisionArgs">Arguments describing a platform collision.</param>
		protected void BaseCollide(PlatformCollisionArgs args)
		{
			// Set friction if a foot is on this platform
			if (args.RaycastCollider.RaycastType == RaycastType.FOOT) args.Character.Friction = Friction;
		}
		
		/// <summary>
		/// Called when one of the characters colliders collides with this platform. This should be overriden for platform specific behaviour.
		/// </summary>
		/// <param name="PlatformCollisionArgs">Arguments describing a platform collision.</param>
		/// <returns>true if character should be parented to this platform, otherwise false.</returns>
		virtual protected bool CustomCollide(PlatformCollisionArgs args)
		{
			return false;
		}
	}

	/// <summary>
	/// Options for controlling how a platform activates itself.
	/// </summary>
	public enum PlatformActivationType
	{
		NONE 				= 0,
		ACTIVATE_ON_START 	= 1,
		ACTIVATE_ON_STAND	= 2,
		ACTIVATE_ON_LEAVE	= 4
	}

	
	/// <summary>
	/// Options for controlling how a platform deactivates itself.
	/// </summary>
	public enum PlatformDeactivationType
	{
		NONE 					= 0,
		DEACTIVATE_ON_EXTENT 	= 1,
		DEACTIVATE_ON_STAND		= 2,
		DEACTIVATE_ON_LEAVE		= 4
	}

}
