using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A point at which the character can respawn. Transform should exactly match where the character will spawn.
	/// </summary>
	public class RespawnPoint : PersistableObject
	{
		/// <summary>
		/// Is this point the default starting point which is activated on scene load.
		/// </summary>
		[Tooltip ("Is this point the default starting point which is activated on scene load?")]
		public bool isDefaultStartingPoint;

		/// <summary>
		/// Unique name for this respawn point. 
		/// </summary>
		[Tooltip ("Unique name for this respawn point. ")]
		public string identifier;

        /// <summary>
        /// If true the respawn point wont be activated when you spawn here. Generally used
        /// for doors so you can respawn at a position but if you die you wont go back there.
        /// </summary>
        [Tooltip ("If true the respawn point wont be activated when you spawn here. Generally used for " +
        	      "doors so you can respawn at a position but if you die you wont go back there.")]
        public bool dontActivateOnRespawn;

		/// <summary>
		/// If true create a proximity trigger which activates the respawn point automatically.
		/// </summary>
		[Tooltip ("If > 0 create a proximity trigger with the given range which activates the respawn point automatically.")]
		public float autoProximityTriggerRange;

		/// <summary>
		/// If non-null the Respawn Point will try to find an image component and update its sprite. If you
		///  want to do more use an EventResponder to listen to the Activated Event or create a custom trigger.
		/// </summary>
		[Tooltip ("If non-null the Respawn Point will try to find an image component and update its sprite. If you want" +
			" to do more use an EventResponder to listen to the Activated Event or create a custom trigger.")]
		public Sprite autoSwitchSprite;

		[Header ("Map")]
		/// <summary>
		/// Reference to point of interest on a map. You can use this to override door POI settings.
		/// </summary>
		[Tooltip ("Reference to point of interest on a map. You can use this to override door POI settings.")]
		public jnamobile.mmm.PointOfInterest poi;

		[Header ("Persistence")]
		/// <summary>
		/// Does this Item get persistence defaults form the Game manager?
		/// </summary>
		[Tooltip ("Does this Item get persistence defaults form the Game manager?")]
		public bool useDefaultPersistence = true;

		/// <summary>
		/// Cached sprite reference used if auto switch sprite is true.
		/// </summary>
		protected SpriteRenderer spriteRenderer;

		/// <summary>
		/// Event trigger when this spawn point is activated.
		/// </summary>
		public event System.EventHandler <CharacterEventArgs> Activated;

		/// <summary>
		/// Gets the header string used to describe the component.
		/// </summary>
		/// <value>The header.</value>
		override public string Header
		{
			get
			{
				return "Respawn points are mainly used to create checkpoints which the character can activate. They " +
					   "are also used for doors and other teleports.";
			}
		}

		/// <summary>
		/// Raises the activated event.
		/// </summary>
		/// <param name="c">Character ref.</param>
		protected void OnActivated(Character c)
		{
			if (Activated != null) Activated (this, new CharacterEventArgs (c));
		}

		/// <summary>
		/// Returns true if this Respawn Point is active.
		/// </summary>
		public bool IsActive
		{
			get
			{
				return (LevelManager.Instance.ActiveRespawnPoint == identifier);
			}
		}

		/// <summary>
		/// Returns true if this Respawn Point is enabled.
		/// </summary>
		public bool IsEnabled
		{
			get
			{
				return LevelManager.Instance.EnabledRespawnPoints.Contains (identifier);
			}
		}


		/// <summary>
		/// Unity Awake hook.
		/// </summary>
		void Awake()
		{
			#if UNITY_EDITOR
			if (Application.isPlaying)
			{
				Init ();
			}
			#else
			Init ();
			#endif
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void Init() 
		{
            if (autoProximityTriggerRange > 0) CreateProximityTrigger (autoProximityTriggerRange);
		}

		/// <summary>
		/// PostInit this instance. Called from Start.
		/// </summary>
		override protected void PostInit() 
		{
            if (identifier == null || identifier == "")
            {
                identifier = LevelManager.Instance.levelName + "_" + (int)transform.position.x + "_" + (int)transform.position.y;
            }
            if (autoSwitchSprite != null) spriteRenderer = GetComponentInChildren<SpriteRenderer> ();
			if (useDefaultPersistence)
			{
				SetPersistenceDefaults ();
			}
			base.PostInit ();
		}

		/// <summary>
		/// Creates a basic proximity trigger for enabling respawn point.
		/// </summary>
		/// <param name="range">Range.</param>
		virtual protected void CreateProximityTrigger(float range)
		{
			ProximityTrigger trigger = gameObject.AddComponent<ProximityTrigger> ();
			trigger.radius = range;
			trigger.receivers = new TriggerTarget[1];
			TriggerTarget target = new TriggerTarget ();
			target.receiver = this.gameObject;
			target.enterAction = TriggerActionType.ACTIVATE_SPAWN_POINT;
			trigger.receivers [0] = target;
		}

		/// <summary>
		/// Sets the persistence defaults.
		/// </summary>
		override protected void SetPersistenceDefaults()
		{
			enablePersistence = PlatformerProGameManager.Instance.persistObjectsInLevel;
			persistenceImplementation = PersistableObjectType.CUSTOM;
			target = gameObject;
			defaultStateIsDisabled = !isDefaultStartingPoint;
		}

		/// <summary>
		/// Set this respawn point as the active one.
		/// </summary>
		virtual public void SetActive(Character c)
		{
			SetEnabled ();
			LevelManager.Instance.ActivateRespawnPoint (identifier);
			if (poi != null) jnamobile.mmm.MapManager.Instance.UpdatePointOfInterest (poi);
			OnActivated(c);
		}

		/// <summary>
		/// Enable this respawn point.
		/// </summary>
		virtual public void SetEnabled()
		{
			if (enablePersistence) SetPersistenceState (true);
			if (spriteRenderer != null && autoSwitchSprite != null) spriteRenderer.sprite = autoSwitchSprite;
			LevelManager.Instance.EnableRespawnPoint (identifier);
			if (poi != null) jnamobile.mmm.MapManager.Instance.UpdatePointOfInterest (poi);
		}

		/// <summary>
		/// Custom persistable implementation. Override to customise.
		/// </summary>
		/// <param name="data">Data.</param>
		override protected void ApplyCustomPersistence(PersistableObjectData data)
		{
			if (!enablePersistence) return;
			if (data.state)
			{
				SetEnabled ();
			}
		}

	}

}