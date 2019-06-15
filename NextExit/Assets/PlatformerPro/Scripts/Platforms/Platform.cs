/**
 * This code is part of Platformer PRO and is copyright John Avery 2014.
 */

using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PlatformerPro
{
	/// <summary>
	/// Base class from which platforms should extend.
	/// </summary>
	public class Platform : PersistableObject
	{

		/// <summary>
		/// Default color to use when drawing Platform gizmos.
		/// </summary>
		public static Color GizmoColor = new Color(0.3f, 0.4f, 1.0f, 1.0f);

		[Header("Platform Settings")]
		/// <summary>
		/// The friction coefficient use -1 for default.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected float friction = -1;

		/// <summary>
		/// How should the platform activate itself?
		/// </summary>
		[SerializeField] [HideInInspector] protected PlatformActivationType automaticActivation;

		/// <summary>
		/// How should the platform deactivate itself?
		/// </summary>
		[SerializeField] [HideInInspector] protected PlatformDeactivationType automaticDeactivation;

		/// <summary>
		/// Should additional conditions enable or disable collisions.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Tooltip(
			"If true additional conditions will be used to enable collisions. If false they will be used to disable collisions.")]
		protected bool conditionsEnableCollisions = true;

		[Header("Persistence")]
		/// <summary>
		/// Does this Item get persistence defaults form the Game manager?
		/// </summary>
		[HideInInspector]
		[Tooltip("Does this Platform get persistence defaults form the Game manager?")]
		public bool useDefaultPersistence;

		/// <summary>
		/// A cached character event args that we update so we don't need to allocate.
		/// </summary>
		protected CharacterEventArgs characterEventArgs;

		/// <summary>
		/// Cached list of all additional conditions.
		/// </summary>
		protected AdditionalCondition[] conditions;

		/// <summary>
		/// The conneciton handle stlye
		/// </summary>
		protected static GUIStyle connectionHandleStyle;

		protected static Platform activeConnectHandleDrag;

		protected static Vector3 currentConnectPosition;

		#region events

		/// <summary>
		/// Occurs when activated.
		/// </summary>
		public event System.EventHandler<CharacterEventArgs> PlatformActivated;

		/// <summary>
		/// Occurs when deactivated.
		/// </summary>
		public event System.EventHandler<CharacterEventArgs> PlatformDeactivated;

		/// <summary>
		/// Some platforms have a special kind of action separate to activa or deactivate. For example the spring 
		/// of a spring baord, or the fall of a decaying platform. This event is sent for those states.
		/// </summary>
		public event System.EventHandler<CharacterEventArgs> Fired;

		/// <summary>
		/// Raises the activated event.
		/// </summary>
		/// <param name="character">Character causing activation, can be null.</param>
		virtual protected void OnPlatformActivated(IMob character)
		{
			if (PlatformActivated != null)
			{
				if (character is Character)
				{
					characterEventArgs.Update((Character) character);
				}
				else
				{
					characterEventArgs.Update(null);
				}

				PlatformActivated(this, characterEventArgs);

			}

			if (enablePersistence) SavePersistenceData();
		}

		/// <summary>
		/// Raises the deactivated event.
		/// </summary>
		/// <param name="character">Character causing deactivation, can be null.</param>
		virtual protected void OnPlatformDeactivated(IMob character)
		{
			if (PlatformDeactivated != null)
			{
				if (character is Character)
				{
					characterEventArgs.Update((Character) character);
				}
				else
				{
					characterEventArgs.Update(null);
				}

				PlatformDeactivated(this, characterEventArgs);
			}

			if (enablePersistence) SavePersistenceData();
		}

		/// <summary>
		/// Raises the fired event.
		/// </summary>
		/// <param name="character">Character causing platform to fire, can be null.</param>
		virtual protected void OnFired(IMob character)
		{
			if (Fired != null)
			{
				if (character is Character)
				{
					characterEventArgs.Update((Character) character);
				}
				else
				{
					characterEventArgs.Update(null);
				}

				Fired(this, characterEventArgs);
			}
		}

		#endregion

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PlatformerPro.Platform"/> is activated.
		/// </summary>
		virtual public bool Activated { get; protected set; }

		/// <summary>
		/// Gets the friction coefficent for the platform. Returns -1 if the default should be used.
		/// </summary>
		virtual public float Friction
		{
			get { return friction; }
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		override protected void PostInit()
		{
			if (useDefaultPersistence)
			{
				SetPersistenceDefaults();
			}

			characterEventArgs = new CharacterEventArgs();
			conditions = GetComponents<AdditionalCondition>();

			if (ForcedActivation != PlatformActivationType.NONE)
			{
				automaticActivation = ForcedActivation;
			}

			if (ForcedDeactivation != PlatformDeactivationType.NONE)
			{
				automaticDeactivation = ForcedDeactivation;
			}

			// No persistable lets set a default state
			if (!enablePersistence)
			{
				if (automaticActivation == PlatformActivationType.ACTIVATE_ON_START)
				{
					Activated = true;
					OnPlatformActivated(null);
				}
			}
			else
			{
				base.PostInit();
			}

		}

		/// <summary>
		/// Activate the platform.
		/// </summary>
		/// <param name="mob">Mob.</param>
		virtual public void Activate(IMob mob)
		{
			if (!Activated) OnPlatformActivated(mob);
			Activated = true;
		}

		/// <summary>
		/// Deactivate the platform.
		/// </summary>
		/// <param name="mob">Mob.</param>
		virtual public void Deactivate(IMob mob)
		{
			if (Activated) OnPlatformDeactivated(mob);
			Activated = false;
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
			if (conditions != null)
			{
				foreach (AdditionalCondition condition in conditions)
				{
					if (!condition.CheckCondition(character, collider))
						return conditionsEnableCollisions;
				}

				return !conditionsEnableCollisions;
			}

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
		/// <param name="character">Character being parented.</param>
		virtual public void Parent(IMob character)
		{

		}

		/// <summary>
		/// Called when the character is unparented from this platform.
		/// </summary>
		/// <param name="character">Character being unparented.</param>
		virtual public void UnParent(IMob character)
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
			get { return false; }
		}

		/// <summary>
		/// If this is not NONE then the actiation state will be forced to this value
		/// and not editable by user.
		/// </summary>
		virtual public PlatformActivationType ForcedActivation
		{
			get { return PlatformActivationType.NONE; }
		}

		/// <summary>
		/// If this is not NONE then the deactiation state will be forced to this value
		/// and not editable by user.
		/// </summary>
		virtual public PlatformDeactivationType ForcedDeactivation
		{
			get { return PlatformDeactivationType.NONE; }
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

		/// <summary>
		/// Gets the extra persistence data which is used to save platform state.
		/// NOTE: Generally you should override ExtraPersistenceData to save a different set of data.
		/// </summary>
		virtual protected void SavePersistenceData()
		{
			if (!enablePersistence) return;
			if (this == null || gameObject == null)
			{
				SetPersistenceState(false, null);
				return;
			}

			SetPersistenceState(Activated);
		}

		/// <summary>
		/// Custom persistable implementation. Override to customise.
		/// </summary>
		/// <param name="data">Data.</param>
		override protected void ApplyCustomPersistence(PersistableObjectData data)
		{
			if (data.state)
			{
				Activate(null);
			}
			else
			{
				Deactivate(null);
			}
		}

		/// <summary>
		/// Draw the base gizmos
		/// </summary>
		virtual protected void DoDrawGizmos()
		{
		}

		/// <summary>
		/// Draw the base gizmos shown when selected.
		/// </summary>
		virtual protected void DoDrawSelectedGizmos()
		{
			DoDrawGizmos();
		}

		/// <summary>
		/// Called when the connect handle is dragged on to another object.
		/// </summary>
		/// <param name="other"></param>
		virtual protected void OnConnectedToObject(Object other)
		{

		}

		/// <summary>
		/// Unity Draw gizmos.
		/// </summary>
		void OnDrawGizmos()
		{
			DoDrawGizmos();
		}

		/// <summary>
		/// Unity Draw gizmos.
		/// </summary>
		void OnDrawGizmosSelected()
		{
			DoDrawSelectedGizmos();
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
