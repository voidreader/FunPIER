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
	public abstract class Trigger : MonoBehaviour
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
		/// If this is not empty require the character to have an item with the matching type before triggering.
		/// </summary>
		[Tooltip ("If this is not empty require the character to have an item with the matching type before triggering.")]
		public string requiredItemType;

		/// <summary>
		/// If this is not null (none) then the required MonoBehaviour must be active and enabled before trigger will activate.
		/// </summary>
		[Tooltip ("If If this is not null (none) then the required MonoBehaviour must be active and enabled before trigger will activate.")]
		public MonoBehaviour requiredComponent;


		/// <summary>
		/// Stores the autoleave routine.
		/// </summary>
		protected IEnumerator autoLeaveRoutine;

		/// <summary>
		/// All characters in the game.
		/// </summary>
		protected static List<Character> allCharacters;

		/// <summary>
		/// All characters in the game.
		/// </summary>
		protected static List<CharacterLoader> allCharacterLoaders;

		/// <summary>
		/// Have we reloaded characters.
		/// </summary>
		protected static bool isLoaded;

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
		/// Adds the character to the list of characters that are checked.
		/// </summary>
		/// <param name="character">Character to add.</param>
		static public void AddCharacter(Character character)
		{
			if (allCharacters == null) allCharacters = new List<Character>();
			if (!allCharacters.Contains(character)) allCharacters.Add(character);
		}

		/// <summary>
		/// Unity OnDestroy() hook.
		/// </summary>
		void OnDestroy()
		{
			DoDestroy();
		}

		/// <summary>
		/// Clean up.
		/// </summary>
		virtual protected void DoDestroy()
		{
			if (allCharacterLoaders != null && allCharacterLoaders.Count > 0)
			{
				foreach (CharacterLoader loader in allCharacterLoaders)
				{
					loader.CharacterLoaded -= HandleCharacterLoaded;
				}
			}
			allCharacters = null;
			allCharacterLoaders = null;
			isLoaded = false;
		}

		/// <summary>
		/// Handles the character loaded event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event.</param>
		void HandleCharacterLoaded (object sender, CharacterEventArgs e)
		{
			if (allCharacters == null) allCharacters = new List<Character>();
			if (!allCharacters.Contains(e.Character)) allCharacters.Add (e.Character);
		}

		/// <summary>
		/// Unity Start() hook.
		/// </summary>
		void Start ()
		{
			Init ();
		}

		/// <summary>
		/// Initialise the sensor.
		/// </summary>
		virtual protected void Init()
		{
			if (!isLoaded)
			{
				allCharacterLoaders = FindObjectsOfType<CharacterLoader>().ToList();
				if (allCharacterLoaders.Count > 0)
				{
					foreach (CharacterLoader loader in allCharacterLoaders)
					{
						loader.CharacterLoaded += HandleCharacterLoaded;
					}
				}
				else
				{
					allCharacters = FindObjectsOfType<Character>().ToList();
				}
				isLoaded = true;
			}
		
			if (receivers != null && receivers.Length > 0)
			{
				foreach(TriggerTarget t in receivers) 
				{
					t.platform = t.receiver.GetComponent<Platform>();
					t.cameraZone = t.receiver.GetComponent<CameraZone>();
				}
			}
		}

		/// <summary>
		/// Character entered proximity.
		/// </summary>
		/// <param name="character">Character. NOTE: This can be null if triggered by something that is not a character.</param>
		virtual protected void EnterTrigger(Character character)
		{
			if (enabled && ConditionsMet(character))
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
							receivers[i].platform.Activated = true;
							break;
						case TriggerActionType.DEACTIVATE_PLATFORM:
							receivers[i].platform.Activated = false;
							break;
						case TriggerActionType.ACTIVATE_GAMEOBJECT:
							receivers[i].receiver.SetActive(true);
							break;
						case TriggerActionType.DEACTIVATE_GAMEOBJECT:
							receivers[i].receiver.SetActive(false);
							break;
						case TriggerActionType.CHANGE_CAMERA_ZONE:
							PlatformCamera.DefaultCamera.ChangeZone(receivers[i].cameraZone);
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
							if (point != null) point.SetActive();
							else
							{
								Debug.LogError ("Trigger tried to activate respawn point but no RespawnPoint was found");
							}
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
					if (oneShot) enabled = false;
				}
			}
		}

		/// <summary>
		/// Are the conditions for firing this trigger met?
		/// </summary>
		/// <param name="character">Character.</param>
		virtual protected bool ConditionsMet(Character character)
		{
			if (requiredItemType != null && requiredItemType != "")
			{
				ItemManager itemManager = character.GetComponentInChildren<ItemManager>();
				if (itemManager == null) 
				{
					Debug.LogWarning("Trigger requires an item but the character has no item manager");
					return false;
				}
				if (itemManager.HasItem(requiredItemType)) return true;
			    return false;
			}
			if (requiredComponent != null)
			{
				if (!requiredComponent.gameObject.activeInHierarchy) return false;
				if (!requiredComponent.enabled) return false;
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
			LeaveTrigger (character);
			if (oneShot) enabled = false;
		}

		/// <summary>
		/// Character leaves the trigger.
		/// </summary>
		/// <param name="character">Character. NOTE: This can be null if triggered by something that is not a character.</param>
		virtual protected void LeaveTrigger(Character character)
		{
			if (enabled)
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
							receivers[i].platform.Activated = true;
							break;
						case TriggerActionType.DEACTIVATE_PLATFORM:
							receivers[i].platform.Activated = false;
							break;
						case TriggerActionType.ACTIVATE_GAMEOBJECT:
							receivers[i].receiver.SetActive(true);
							break;
						case TriggerActionType.DEACTIVATE_GAMEOBJECT:
							receivers[i].receiver.SetActive(false);
							break;
						case TriggerActionType.CHANGE_CAMERA_ZONE:
							PlatformCamera.DefaultCamera.ChangeZone(receivers[i].cameraZone);
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
							if (point != null) point.SetActive();
							else
							{
								Debug.LogError ("Trigger tried to activate respawn point but no RespawnPoint was found");
							}
							break;
						}
					}
				}
			}
		}

	}

}