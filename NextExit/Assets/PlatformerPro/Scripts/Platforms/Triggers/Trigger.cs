using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlatformerPro.Extras;

namespace PlatformerPro
{
	/// <summary>
	/// Base class for triggers. This handles the character assignment logic. Although you 
	/// don't need to extend your triggers from this class it may be useful to ensure compatability
	/// with other classes in the kit.
	/// </summary>
	public abstract class Trigger : PersistableObject
    {
		/// <summary>
		/// Default color to use when draing Trigger gizmos.
		/// </summary>
		public static Color GizmoColor = new Color (1, 0.64f, 0, 0.5f);

		/// <summary>
		/// The Targets that receive the triggers actions.
		/// </summary>
		public TriggerTarget[] receivers;

		/// <summary>
		/// Fire this trigger once only then disable?
		/// </summary>
		[Tooltip ("Fire this trigger once only then disable?")]
		public bool oneShot;

		/// <summary>
		/// Time afer enter in which leave will be automatically triggered. Ignored if 0 or smaller.
		/// </summary>
		[Tooltip ("If this is greater than 0, automatically trigger the leave action this many seconds after the enter action.")]
		public float autoLeaveTime;

		/// <summary>
		/// If this is not null (none) then the required MonoBehaviour must be active and enabled before trigger will activate.
		/// </summary>
		[Tooltip ("If If this is not null (none) then the required MonoBehaviour must be active and enabled before trigger will activate.")]
		public MonoBehaviour requiredComponent;

        [Header("Persistence")]
        /// <summary>
        /// Does this Item get persistence defaults form the Game manager?
        /// </summary>
        [HideInInspector]
        [Tooltip("Does this Platform get persistence defaults form the Game manager?")]
        public bool useDefaultPersistence;

        /// <summary>
        /// Stores the autoleave routine.
        /// </summary>
        protected IEnumerator autoLeaveRoutine;

		/// <summary>
		/// Have we reloaded characters.
		/// </summary>
		protected static bool isLoaded;

		/// <summary>
		/// Cached list of all additional conditions.
		/// </summary>
		protected AdditionalCondition[] conditions;

		#region events

		/// <summary>
		/// Event for trigger enter.
		/// </summary>
		public event System.EventHandler <CharacterEventArgs> TriggerEntered;

		/// <summary>
		/// Event for trigger leave.
		/// </summary>
		public event System.EventHandler <CharacterEventArgs> TriggerExited;


		/// <summary>
		/// Raises the trigger entered event.
		/// </summary>
		/// <param name="character">Character.</param>
		virtual protected void OnTriggerEntered(Character character)
		{
			if (TriggerEntered != null)
			{
				TriggerEntered(this, new TriggerEventArgs(this, character));
			}
		}

		/// <summary>
		/// Raises the trigger exited event.
		/// </summary>
		/// <param name="character">Character.</param>
		virtual protected void OnTriggerExited(Character character)
		{
			if (TriggerExited != null)
			{
				TriggerExited(this, new TriggerEventArgs(this, character));
			}
		}

		#endregion

		/// <summary>
		/// Unity Start() hook.
		/// </summary>
		void Start ()
		{
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                PostInit();
            }
#else
            PostInit ();
#endif
        }

        /// <summary>
        /// Initialise the sensor.
        /// </summary>
        override protected void PostInit()
		{
            base.PostInit();
			if (!isLoaded)
			{
				isLoaded = true;
			}
		
			if (receivers != null && receivers.Length > 0)
			{
				foreach(TriggerTarget t in receivers) 
				{
					t.platform = t.receiver.GetComponent<Platform>();
					t.cameraZone = t.receiver.GetComponent<CameraZone>();
					t.combiner = t.receiver.GetComponent<TriggerCombiner>();
				}
			}
			conditions = GetComponents<AdditionalCondition> ();

            if (enablePersistence && !oneShot) Debug.LogWarning("Only one-shot triggers can be persisted");
		}

        /// <summary>
        /// Character entered proximity.
        /// </summary>
        /// <param name="character">Character. NOTE: This can be null if triggered by something that is not a character.</param>
        virtual protected bool EnterTrigger(Character character)
        {
            if (enabled && ConditionsMet(character))
            {
                DoEnterTrigger(character);
                // Let each condition know it was activated
                foreach (AdditionalCondition condition in conditions)
                {
                    condition.Activated(character, this);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Do the trigger work
        /// </summary>
        /// <param name="character">Character.</param>
        virtual protected void DoEnterTrigger (Character character)
        { 
            OnTriggerEntered(character);
			for (int i = 0; i < receivers.Length; i++)
			{
				if (receivers[i] != null)
				{
					switch(receivers[i].enterAction)
					{
					case TriggerActionType.SEND_MESSAGE:
						receivers[i].receiver.SendMessage("EnterTrigger", SendMessageOptions.DontRequireReceiver);
						break;
					case TriggerActionType.ACTIVATE_PLATFORM:
						receivers[i].platform.Activate(character);
						break;
					case TriggerActionType.DEACTIVATE_PLATFORM:
						receivers[i].platform.Deactivate(character);
						break;
					case TriggerActionType.ACTIVATE_GAMEOBJECT:
						receivers[i].receiver.SetActive(true);
						break;
					case TriggerActionType.DEACTIVATE_GAMEOBJECT:
						receivers[i].receiver.SetActive(false);
						break;
					case TriggerActionType.CHANGE_CAMERA_ZONE:
						PlatformCamera.DefaultCamera.ChangeZone(receivers[i].cameraZone, character);
						break;
					case TriggerActionType.SWITCH_SPRITE:
						// TODO This should not be done here
						SpriteRenderer spriteRenderer = receivers[i].receiver.GetComponentInChildren<SpriteRenderer>();
						if (spriteRenderer != null)
						{
							spriteRenderer.sprite = receivers[i].newSprite;
						}
						else
						{
							Debug.LogError ("Trigger tried to switch sprite but no SpriteRenderer was found");
						}
						break;
					case TriggerActionType.SHOW_DIALOG:
						UIDialog dialog = receivers[i].receiver.GetComponentInChildren<UIDialog>();
						if (dialog != null)
						{
							dialog.ShowDialog(transform);
						}
						else
						{
							Debug.LogError ("Trigger tried to show dialog but no UIDialog was found.");
						}
						break;
					case TriggerActionType.HIDE_DIALOG:
						UIDialog dialogToHide = receivers[i].receiver.GetComponentInChildren<UIDialog>();
						if (dialogToHide != null)
						{
							dialogToHide.HideDialog();
						}
						else
						{
							Debug.LogError ("Trigger tried to show dialog but no UIDialog was found.");
						}
						break;
					case TriggerActionType.OPEN_DOOR:
						Door doorToOpen = receivers[i].receiver.GetComponent<Door>();
						if (doorToOpen != null)
						{
							doorToOpen.Open(character);
						}
						else
						{
							Debug.LogError ("Trigger tried to open door but no Door was found.");
						}
						break;
					case TriggerActionType.CLOSE_DOOR:
						Door doorToClose = receivers[i].receiver.GetComponent<Door>();
						if (doorToClose != null)
						{
							doorToClose.Close(character);
						}
						else
						{
							Debug.LogError ("Trigger tried to close door but no Door was found.");
						}
						break;
					case TriggerActionType.ACTIVATE_SPAWN_POINT:
						RespawnPoint point = receivers[i].receiver.GetComponentInChildren<RespawnPoint>();
						if (point != null) point.SetActive(character);
						else
						{
							Debug.LogError ("Trigger tried to activate respawn point but no RespawnPoint was found");
						}
						break;
					case TriggerActionType.FORWARD:
						receivers[i].combiner.EnteredTrigger(this, character);
						break;
					}
				}
			}

			if (autoLeaveTime > 0) 
			{
				if (!oneShot || autoLeaveRoutine == null)
				{
					autoLeaveRoutine = DoLeaveAfterDelay(character, autoLeaveTime);
					StartCoroutine(autoLeaveRoutine);
				}
			}
			else
			{
                if (oneShot)
                {
                    enabled = false;
                    if (enablePersistence) SavePersistenceData();
                }
			}
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
            SetPersistenceState(false);
        }

        /// <summary>
        /// Custom persistable implementation. Override to customise.
        /// </summary>
        /// <param name="data">Data.</param>
        override protected void ApplyCustomPersistence(PersistableObjectData data)
        {
            if (!enablePersistence) return;
            if (!data.state)
            {
                DoEnterTrigger(null);
            }
        }

        /// <summary>
        /// Are the conditions for firing this trigger met?
        /// </summary>
        /// <param name="character">Character.</param>
        virtual protected bool ConditionsMet(Character character)
		{
			if (requiredComponent != null)
			{
				if (!requiredComponent.gameObject.activeInHierarchy) return false;
				if (!requiredComponent.enabled) return false;
			}
            if (conditions != null) { 
                foreach (AdditionalCondition condition in conditions) 
    			{
    				if (!condition.CheckCondition(character, this)) return false;
    			}
            }
            return true;
		}

		/// <summary>
		/// Are the conditions for exiting trigger met.
		/// </summary>
		/// <returns><c>true</c>, if conditions are met, <c>false</c> otherwise.</returns>
		/// <param name="character">Character.</param>
		virtual protected bool ExitConditionsMet(Character character)
		{
			foreach (AdditionalCondition condition in conditions) 
			{
				if (!condition.CheckInverseCondition(character, this)) return false;
			}
			return true;
		}

		/// <summary>
		/// Does the autoleave action. Cancelled by the leave being triggerd.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="delay">Delay.</param>
		virtual protected IEnumerator DoLeaveAfterDelay(Character character, float delay)
		{
			float elapsedTime = 0;
			while (elapsedTime < delay)
			{
				elapsedTime += TimeManager.FrameTime;
				yield return true;
			}
            DoLeaveTrigger(character);
			if (oneShot) enabled = false;
		}

        /// <summary>
        /// Character leaves the trigger.
        /// </summary>
        /// <param name="character">Character. NOTE: This can be null if triggered by something that is not a character.</param>
        virtual protected bool LeaveTrigger(Character character)
        {
            if (enabled && ExitConditionsMet(character))
            {
                DoLeaveTrigger(character);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Does the actual work for leavinga  trigger.
        /// </summary>
        /// <param name="character">Character.</param>
        virtual protected void DoLeaveTrigger(Character character)
        { 
            if (autoLeaveRoutine != null)
            {
                StopCoroutine(autoLeaveRoutine);
                autoLeaveRoutine = null;
            }
            OnTriggerExited(character);
			for (int i = 0; i < receivers.Length; i++)
			{
				if (receivers[i] != null)
				{
					// TODO Should only be one implementation of these
					switch(receivers[i].leaveAction)
					{
					case TriggerActionType.SEND_MESSAGE:
						receivers[i].receiver.SendMessage("EnterTrigger", SendMessageOptions.DontRequireReceiver);
						break;
					case TriggerActionType.ACTIVATE_PLATFORM:
						receivers[i].platform.Activate(character);
						break;
					case TriggerActionType.DEACTIVATE_PLATFORM:
						receivers[i].platform.Deactivate(character);
						break;
					case TriggerActionType.ACTIVATE_GAMEOBJECT:
						receivers[i].receiver.SetActive(true);
						break;
					case TriggerActionType.DEACTIVATE_GAMEOBJECT:
						receivers[i].receiver.SetActive(false);
						break;
					case TriggerActionType.CHANGE_CAMERA_ZONE:
						PlatformCamera.DefaultCamera.ChangeZone(receivers[i].cameraZone, character);
						break;
					case TriggerActionType.SWITCH_SPRITE:
						SpriteRenderer spriteRenderer = receivers[i].receiver.GetComponentInChildren<SpriteRenderer>();
						if (spriteRenderer != null)
						{
							spriteRenderer.sprite = receivers[i].newSprite;
						}
						else
						{
							Debug.LogError ("Trigger tried to switch sprite but no SpriteRenderer was found");
						}
						break;
					case TriggerActionType.SHOW_DIALOG:
						UIDialog dialog = receivers[i].receiver.GetComponentInChildren<UIDialog>();
						if (dialog != null)
						{
							dialog.ShowDialog(transform);
						}
						else
						{
							Debug.LogError ("Trigger tried to show dialog but no UIDialog was found.");
						}
						break;
					case TriggerActionType.HIDE_DIALOG:
						UIDialog dialogToHide = receivers[i].receiver.GetComponentInChildren<UIDialog>();
						if (dialogToHide != null)
						{
							dialogToHide.HideDialog();
						}
						else
						{
							Debug.LogError ("Trigger tried to show dialog but no UIDialog was found.");
						}
						break;
					
					case TriggerActionType.OPEN_DOOR:
						Door doorToOpen = receivers[i].receiver.GetComponent<Door>();
						if (doorToOpen != null)
						{
							doorToOpen.Open(character);
						}
						else
						{
							Debug.LogError ("Trigger tried to open door but no Door was found.");
						}
						break;
					case TriggerActionType.CLOSE_DOOR:
						Door doorToClose = receivers[i].receiver.GetComponent<Door>();
						if (doorToClose != null)
						{
							doorToClose.Close(character);
						}
						else
						{
							Debug.LogError ("Trigger tried to close door but no Door was found.");
						}
						break;
					case TriggerActionType.ACTIVATE_SPAWN_POINT:
						RespawnPoint point = receivers[i].receiver.GetComponentInChildren<RespawnPoint>();
						if (point != null) point.SetActive(character);
						else
						{
							Debug.LogError ("Trigger tried to activate respawn point but no RespawnPoint was found");
						}
						break;
					case TriggerActionType.FORWARD:
						receivers[i].combiner.LeftTrigger(this, character);
						break;
					}
				}
			}
		}
	}

}