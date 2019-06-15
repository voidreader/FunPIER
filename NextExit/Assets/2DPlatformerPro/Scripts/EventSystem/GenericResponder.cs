using UnityEngine;
#if !UNITY_4_6 && !UNITY_5_1 && !UNITY_5_2
using UnityEngine.SceneManagement;
#endif
using System.Collections;
using PlatformerPro.Extras;

namespace PlatformerPro
{
	/// <summary>
	/// Use this component to do something when an event occurs. This is a base class, and should be extended.
	/// </summary>
	public abstract class GenericResponder : MonoBehaviour
	{
		/// <summary>
		/// Handles the event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		virtual protected void DoAction(EventResponse action, System.EventArgs args)
		{
			if (action.delay == 0.0f) DoImmediateAction (action, args);
			else StartCoroutine(DoDelayedAction(action, args));
		}

		/// <summary>
		/// Do the action after a delay.
		/// </summary>
		/// <param name="args">Event arguments.</param>
		/// <param name="action">Action.</param>
		virtual protected IEnumerator DoDelayedAction(EventResponse action, System.EventArgs args)
		{
			float delayTimer = action.delay;
			while (delayTimer > 0.0f)
			{
				delayTimer -= TimeManager.FrameTime;
				yield return true;
			}
			DoImmediateAction (action, args);
		}
		
		/// <summary>
		/// Do the action
		/// </summary>
		/// <param name="args">Event arguments.</param>
		/// <param name="action">Action.</param>
		virtual protected void DoImmediateAction(EventResponse action, System.EventArgs args)
		{
			Character character;
			CharacterHealth characterHealth = null;
			ItemManager itemManager = null;
			Animator animator;
			Animation animation;
			SpecialMovement_PlayAnimation specialMovement;

			switch(action.responseType)
			{
			case EventResponseType.DEBUG_LOG:
				Debug.Log (string.Format("Got event, arguments: {0}", args != null ? args.ToString() : "" ));
				break;
			case EventResponseType.ACTIVATE_GAMEOBJECT:
				action.targetGameObject.SetActive(true);	
				break;
			case EventResponseType.DEACTIVATE_GAMEOBJECT:
				action.targetGameObject.SetActive(false);	
				break;
			case EventResponseType.SEND_MESSSAGE:
				action.targetGameObject.SendMessage(action.message, SendMessageOptions.DontRequireReceiver);
				break;
			case EventResponseType.ENABLE_BEHAVIOUR:
				if (action.targetComponent is Movement) ((Movement)action.targetComponent).Enabled = true;
				else if (action.targetComponent is Behaviour) ((Behaviour)action.targetComponent).enabled = true;
				else if (action.targetComponent is Renderer) ((Renderer)action.targetComponent).enabled = true;
				break;
			case EventResponseType.DISABLE_BEHAVIOUR:
				if (action.targetComponent is Movement) ((Movement)action.targetComponent).Enabled = false;
				else if (action.targetComponent is Behaviour) ((Behaviour)action.targetComponent).enabled = false;
				else if (action.targetComponent is Renderer) ((Renderer)action.targetComponent).enabled = false;
				break;
			case EventResponseType.OVERRIDE_ANIMATON:
				if (action.targetComponent is Character) ((Character)action.targetComponent).AddAnimationOverride(action.overrideState);
				break;
			case EventResponseType.CLEAR_ANIMATION_OVERRIDE:
				if (action.targetComponent is Character) ((Character)action.targetComponent).RemoveAnimationOverride(action.overrideState);
				break;
			case EventResponseType.SPECIAL_MOVE_ANIMATION:
				// This is legacy and shoulnd't be used any more
				character = null;
				if (action.targetComponent is Character) 
				{
					character = (Character) action.targetComponent;
				}
				else if (args is CharacterEventArgs)
				{
					character = ((CharacterEventArgs)args).Character;
				}
				if (character != null)
				{
					SpecialMovement_PlayAnimation movement = character.GetComponentInChildren<SpecialMovement_PlayAnimation>();
					if (movement == null) 
					{
						Debug.LogWarning("Cannot play an animation as the Character does not have a SpecalMovement_PlayAnimation attached");
					}
					else
					{
						movement.Play(action.animationState);
					}
				}
				break;
			case EventResponseType.PLAY_PARTICLES:
				if (action.targetComponent is ParticleSystem) ((ParticleSystem)action.targetComponent).Play();
				break;
			case EventResponseType.PAUSE_PARTICLES:
				if (action.targetComponent is ParticleSystem) ((ParticleSystem)action.targetComponent).Pause();
				break;
			case EventResponseType.SWITCH_SPRITE:
				if (action.targetComponent is SpriteRenderer) ((SpriteRenderer)action.targetComponent).sprite = action.newSprite;
				break;
			case EventResponseType.PLAY_SFX:
				if (action.targetComponent is SoundEffect) ((SoundEffect)action.targetComponent).Play ();
				break;
			case EventResponseType.PLAY_SONG:
				if (action.targetComponent is MusicPlayer) ((MusicPlayer)action.targetComponent).Play (action.message);
				break;
			case EventResponseType.STOP_SONG:
				if (action.targetComponent is MusicPlayer) ((MusicPlayer)action.targetComponent).Stop ();
				break;
			case EventResponseType.MAKE_INVULNERABLE:
				character = null;
				if (action.targetComponent is Character) 
				{
					character = (Character) action.targetComponent;
				}
				else if (args is CharacterEventArgs)
				{
					character = ((CharacterEventArgs)args).Character;
				}
				if (character != null)
				{
					CharacterHealth ch = character.GetComponentInChildren<CharacterHealth>();
					if (ch != null)
					{
						ch.SetInvulnerable();
					}
					else
					{
						Debug.LogWarning("Tried to make Character invulnerable but no CharacterHealth found.");
					}
				}
				else if (action.targetComponent is Enemy)
				{
					((Enemy)action.targetComponent).MakeInvulnerable(99999999);
				}
				else
				{
					Debug.LogWarning("Tried to make invulnerable but didn't know how to make the target invulnerable.");
				}
				break;
			case EventResponseType.MAKE_VULNERABLE:
				character = null;
				if (action.targetComponent is Character) 
				{
					character = (Character) action.targetComponent;
				}
				else if (args is CharacterEventArgs)
				{
					character = ((CharacterEventArgs)args).Character;
				}
				if (character != null)
				{
					CharacterHealth ch = character.GetComponentInChildren<CharacterHealth>();
					if (ch != null)
					{
						ch.SetVulnerable();
					}
					else
					{
						Debug.LogWarning("Tried to make Character vulnerable but no CharacterHealth found.");
					}
				}
				else if (action.targetComponent is Enemy)
				{
					((Enemy)action.targetComponent).MakeVulnerable();
				}
				else
				{
					Debug.LogWarning("Tried to make vulnerable but didn't know how to make the target invulnerable.");
				}
				break;
			case EventResponseType.LEVEL_COMPLETE:
				LevelManager.Instance.LevelCompleted();
				break;
			case EventResponseType.LOAD_SCENE:
				foreach (Character c in FindObjectsOfType<Character>()) c.AboutToExitScene(action.message);
				#if !UNITY_4_6 && !UNITY_5_1 && !UNITY_5_2
				SceneManager.LoadScene(action.message);
				#else
				Application.LoadLevel (action.message);
				#endif
				break;
			case EventResponseType.LOCK:
				LevelManager.Instance.LockLevel (action.message);
				break;
			case EventResponseType.UNLOCK:
				LevelManager.Instance.UnlockLevel (action.message);
				break;
			case EventResponseType.RESPAWN:
				if (args is CharacterEventArgs)
				{
					LevelManager.Instance.Respawn(((CharacterEventArgs)args).Character, action.message);
				}
				else Debug.LogWarning("Tried to respawn but the triggering event did not derive from Character.");
				break;
			case EventResponseType.SET_ACTIVE_RESPAWN:
				LevelManager.Instance.ActivateRespawnPoint(action.message);
				break;
			case EventResponseType.START_EFFECT:
				if (action.targetComponent is FX_Base) 
				{
					if (action.targetGameObject != null && action.message != null && action.message != "") ((FX_Base)action.targetComponent).StartEffect(action.targetGameObject, action.message);
					else ((FX_Base)action.targetComponent).StartEffect();
				}
				else Debug.LogWarning("Trying to play an Effect that isn't derived from FX_Base.");
				break;
			case EventResponseType.FLIP_GRAVITY:
				if (args is CharacterEventArgs)
				{
					FlippableGravity gravity = ((CharacterEventArgs)args).Character.GetComponent<FlippableGravity>();
					if (gravity != null)
					{
						gravity.FlipGravity();
					}
					else Debug.LogWarning("Tried to flip gravity but the character didn't have a FlippableGravity attached.");
				}
				else Debug.LogWarning("Tried to flip gravity but the triggering event did not derive from Character.");
				break;
			case EventResponseType.START_SWIM:
				Debug.LogWarning("Swim is not supported in the Lite Edition");
				break;
			case EventResponseType.STOP_SWIM:
				Debug.LogWarning("Swim is not supported in the Lite Edition");
				break;
			case EventResponseType.ADD_SCORE:
				ScoreManager.GetInstanceForType(action.message).AddScore(action.intValue);
				break;
			case EventResponseType.RESET_SCORE:
				ScoreManager.GetInstanceForType(action.message).ResetScore();
				break;
			case EventResponseType.PLAY_ANIMATION:
				character = null;
				specialMovement = null;
				if (action.targetComponent is Character) 
				{
					character = (Character) action.targetComponent;
				}
				else if (args is CharacterEventArgs)
				{
					character = ((CharacterEventArgs)args).Character;
				}
				if (character != null)
				{
					specialMovement  = character.GetComponentInChildren<SpecialMovement_PlayAnimation>();
					if (specialMovement != null) 
					{
						specialMovement.Play(action.animationState);
						break;
					}
				}
				animator = action.targetGameObject.GetComponent<Animator>();
				if (animator != null)
				{
					animator.Play (action.message);
					break;
				}
				animation = action.targetGameObject.GetComponent<Animation>();
				if (animation != null) 
				{
					animation.Play();
					break;
				}
				Debug.LogWarning("Couldn't find a Special Movement, Animation or Animator on the target GameObject");
				break;
			case EventResponseType.STOP_ANIMATION:
				character = null;
				specialMovement = null;
				if (action.targetComponent is Character) 
				{
					character = (Character) action.targetComponent;
				}
				else if (args is CharacterEventArgs)
				{
					character = ((CharacterEventArgs)args).Character;
				}
				if (character != null)
				{
					specialMovement = character.GetComponentInChildren<SpecialMovement_PlayAnimation>();
					if (specialMovement != null) 
					{
						specialMovement.StopAnimation();
						break;
					}
				}
				animation = action.targetGameObject.GetComponent<Animation>();
				if (animation != null) 
				{
					animation.Stop();
					break;
				}
				Debug.LogWarning("Couldn't find a Special Movement or Animation on the target GameObject");
				break;
			case EventResponseType.UPDATE_LIVES:
				if (action.targetComponent is CharacterHealth) 
				{
					characterHealth = (CharacterHealth) action.targetComponent;
				}
				else if (args is CharacterEventArgs)
				{
					characterHealth = ((CharacterEventArgs)args).Character.GetComponent<CharacterHealth>();
				}
				if (characterHealth == null) 
				{
					Debug.LogWarning("No characterHealth found on target, cannot increase max lives");
				}
				else
				{
					characterHealth.CurrentLives += action.intValue;
				}
				break;
			case EventResponseType.UPDATE_MAX_LIVES:
				if (action.targetComponent is CharacterHealth) 
				{
					characterHealth = (CharacterHealth) action.targetComponent;
				}
				else if (args is CharacterEventArgs)
				{
					characterHealth = ((CharacterEventArgs)args).Character.GetComponent<CharacterHealth>();
				}
				if (characterHealth == null) 
				{
					Debug.LogWarning("No characterHealth found on target, cannot increase max lives");
				}
				else
				{
					characterHealth.MaxLives += action.intValue;
				}
				break;
			case EventResponseType.SET_MAX_LIVES:
				if (action.targetComponent is CharacterHealth) 
				{
					characterHealth = (CharacterHealth) action.targetComponent;
				}
				else if (args is CharacterEventArgs)
				{
					characterHealth = ((CharacterEventArgs)args).Character.GetComponent<CharacterHealth>();
				}
				if (characterHealth == null) 
				{
					Debug.LogWarning("No characterHealth found on target, cannot increase max lives");
				}
				else
				{
					characterHealth.MaxLives = action.intValue;
				}
				break;
			case EventResponseType.UPDATE_MAX_HEALTH:
				if (action.targetComponent is CharacterHealth) 
				{
					characterHealth = (CharacterHealth) action.targetComponent;
				}
				else if (args is CharacterEventArgs)
				{
					characterHealth = ((CharacterEventArgs)args).Character.GetComponent<CharacterHealth>();
				}
				if (characterHealth == null) 
				{
					Debug.LogWarning("No characterHealth found on target, cannot increase max health");
				}
				else
				{
					characterHealth.MaxHealth += action.intValue;
				}
				break;
			case EventResponseType.SET_MAX_HEALTH:
				if (action.targetComponent is CharacterHealth) 
				{
					characterHealth = (CharacterHealth) action.targetComponent;
				}
				else if (args is CharacterEventArgs)
				{
					characterHealth = ((CharacterEventArgs)args).Character.GetComponent<CharacterHealth>();
				}
				if (characterHealth == null) 
				{
					Debug.LogWarning("No characterHealth found on target, cannot decrease max health");
				}
				else
				{
					characterHealth.MaxHealth = action.intValue;
				}
				break;
			case EventResponseType.UPDATE_ITEM_MAX:
				if (action.targetComponent is ItemManager) 
				{
					itemManager = (ItemManager) action.targetComponent;
				}
				else if (args is CharacterEventArgs)
				{
					itemManager = ((CharacterEventArgs)args).Character.GetComponentInChildren<ItemManager>();
				}
				if (itemManager == null) 
				{
					Debug.LogWarning("No itemManager found on target, cannot increase item max");
				}
				else
				{
					itemManager.IncreaseItemMax(action.message, action.intValue);
				}
				break;
			case EventResponseType.SET_ITEM_MAX:
				if (action.targetComponent is ItemManager) 
				{
					itemManager = (ItemManager) action.targetComponent;
				}
				else if (args is CharacterEventArgs)
				{
					itemManager = ((CharacterEventArgs)args).Character.GetComponentInChildren<ItemManager>();
				}
				if (itemManager == null) 
				{
					Debug.LogWarning("No itemManager found on target, cannot decrease item max");
				}
				else
				{
					itemManager.SetItemMax(action.message, -action.intValue);
				}
				break;
			case EventResponseType.SET_TAGGED_PROPERTY:
				character = null;
				if (action.targetComponent is Character) 
				{
					character = (Character) action.targetComponent;
				}
				else if (args is CharacterEventArgs)
				{
					character = ((CharacterEventArgs)args).Character;
				}
				if (character != null)
				{
					character.SetTaggedProperty(action.message, action.floatValue);
				}
				break;
			case EventResponseType.ADD_TO_TAGGED_PROPERTY:
				character = null;
				if (action.targetComponent is Character) 
				{
					character = (Character) action.targetComponent;
				}
				else if (args is CharacterEventArgs)
				{
					character = ((CharacterEventArgs)args).Character;
				}
				if (character != null)
				{
					character.AddToTaggedProperty(action.message, action.floatValue);
				}
				break;
			case EventResponseType.MULTIPLY_TAGGED_PROPERTY:
				character = null;
				if (action.targetComponent is Character) 
				{
					character = (Character) action.targetComponent;
				}
				else if (args is CharacterEventArgs)
				{
					character = ((CharacterEventArgs)args).Character;
				}
				if (character != null)
				{
					character.MultiplyTaggedProperty(action.message, action.floatValue);
				}
				break;
			case EventResponseType.SPAWN_ITEM:
				if (action.targetComponent is RandomItemSpawner) 
				{
					((RandomItemSpawner) action.targetComponent).Spawn();
				}
				else
				{
					Debug.LogWarning("No RandomItemSpawner is set.");
				}
				break;
			case EventResponseType.ADD_VELOCITY:
				if (action.targetComponent == null && args is CharacterEventArgs)
				{
					character = ((CharacterEventArgs)args).Character;
					float modifier = action.boolValue ? character.LastFacedDirection : 1;
					Vector2 currentVelocity = character.Velocity;
					currentVelocity += action.vectorValue;
					character.SetVelocityX(currentVelocity.x * modifier);
					character.SetVelocityY(currentVelocity.y);
				}
				else if (action.targetComponent is IMob) 
				{
					float modifier = action.boolValue ? ((IMob) action.targetComponent).LastFacedDirection : 1;
					Vector2 currentVelocity = ((IMob) action.targetComponent).Velocity;
					currentVelocity += action.vectorValue;
					((IMob) action.targetComponent).SetVelocityX(currentVelocity.x * modifier);
					((IMob) action.targetComponent).SetVelocityY(currentVelocity.y);
				}
				else if (action.targetComponent is Rigidbody2D) 
				{
					if (action.boolValue)
					{
						((Rigidbody2D)action.targetComponent).AddRelativeForce(action.vectorValue, ForceMode2D.Impulse);
					}
					else
					{
						((Rigidbody2D)action.targetComponent).AddForce(action.vectorValue, ForceMode2D.Impulse);
					}
				}
				else
				{
					Debug.LogWarning("Tried to add velocity to an object that wasnt a Character, Enemy or Rigidbody2D");
				}
				break;
			case EventResponseType.SET_VELOCITY:
				character = ((CharacterEventArgs)args).Character;
				if (action.targetComponent == null && args is CharacterEventArgs)
				{
					float modifier = action.boolValue ? character.LastFacedDirection : 1;
					character.SetVelocityX(action.vectorValue.x * modifier);
					character.SetVelocityY(action.vectorValue.y);
				}
				else if (action.targetComponent is IMob) 
				{
					float modifier = action.boolValue ? ((IMob) action.targetComponent).LastFacedDirection : 1;
					((IMob) action.targetComponent).SetVelocityX(action.vectorValue.x * modifier);
					((IMob) action.targetComponent).SetVelocityY(action.vectorValue.y);
				}
				else if (action.targetComponent is Rigidbody2D) 
				{
					((Rigidbody2D)action.targetComponent).velocity = action.vectorValue;
				}
				else
				{
					Debug.LogWarning("Tried to set velocity on an object that wasnt a Character, Enemy or Rigidbody2D");
				}
				break;

			}

		}
	}
}