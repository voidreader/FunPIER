using UnityEngine;
#if !UNITY_4_6 && !UNITY_4_7 && !UNITY_5_1 && !UNITY_5_2
using UnityEngine.SceneManagement;
#endif
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Displays some UI content when a players game ends.
	/// </summary>
	public class UIGameOverScreen : PlatformerProMonoBehaviour
	{
		[Header ("Control")]
		/// <summary>
		/// Show on respawn.
		/// </summary>
		[Tooltip ("Do we automatically show this on the GameOver event, or do we wait for something to call us?")]
		public bool autoShowOnGameOver;

		/// <summary>
		/// Is this a game over screen for the whole game, or just a single player?
		/// </summary>
		[Tooltip ("Is this a game over screen for the whole game, or just a single player?")]
		public bool globalGameOver;

		/// <summary>
		/// If this is player specific which player is it for?
		/// </summary>
		[DontShowWhen ("globalGameOver")]
		[Tooltip ("If this is player specific which player is it for?")]
		public int playerId;

		[Header ("Timing")]
		/// <summary>
		/// Minimum time to show for even if the user presses a key.
		/// </summary>
		[Tooltip("Minimum time to show for even if the user presses a key.")]
		public float minShowTime = 2.0f;

		/// <summary>
		/// How long in total to show the screen before calling Hide().
		/// </summary>
		[Tooltip("How long in total to show the screen before calling Hide(). 0 means show until user presses a key.")]
		public float totalShowTime = 0;


		[Header ("Visuals")]
		/// <summary>
		/// GameObject holding the content to show.
		/// </summary>
		[Tooltip ("GameObject holding the content to show.")]
		public GameObject visibleComponent;

		/// <summary>
		/// The show effects.
		/// </summary>
		[Tooltip ("Effects to play when showing this menu.")]
		public List<FX_Base> showEffects;
		
		/// <summary>
		/// The hide effects.
		/// </summary>
		[Tooltip ("Effects to play when hiding this menu.")]
		public List<FX_Base> hideEffects;

		[Header ("Actions")]
		/// <summary>
		/// What do we do after screen has been shown.
		/// </summary>
		[Tooltip ("What to do after we shown the screen.")]
		public DeathAction[] gameOverActions;

		/// <summary>
		/// Reference to the character (or null if none).
		/// </summary>
		protected Character character;

		/// <summary>
		/// Reference to the character health (or null if none).
		/// </summary>
		protected CharacterHealth characterHealth;

		/// <summary>
		/// Reference to the game manager loader (or null if none).
		/// </summary>
		protected PlatformerProGameManager gameManager;

		/// <summary>
		/// Gets the header string used to describe the component.
		/// </summary>
		/// <value>The header.</value>
		override public string Header
		{
			get
			{
				return "The game over screen can be shown when the game ends or when a single player dies. It can " +
					"optionally have death actions attached, for example you can reload another scene. If you want additional " +
					"control then use an EventResponder to listen to GameOver events directly.";
			}
		}


		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start()
		{
			PostInit ();
		}

		/// <summary>
		/// Unity OnDestroy() hook.
		/// </summary>
		void OnDestroy()
		{
			if (characterHealth != null ) characterHealth.GameOver -= HandleCharacterGameOver;
			if (gameManager != null)
			{ 
				if (globalGameOver)
				{
					gameManager.PhaseChanged -= HandlePhaseChanged;
				}
				else
				{
					gameManager.CharacterLoaded -= HandleCharacterLoaded;
				}
			}
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		protected virtual void PostInit()
		{
			if (autoShowOnGameOver)
			{
				gameManager = PlatformerProGameManager.Instance;
				if (globalGameOver)
				{
					gameManager.PhaseChanged += HandlePhaseChanged;
				}
				else
				{
					gameManager.CharacterLoaded += HandleCharacterLoaded;
				}
			}
		}

		/// <summary>
		/// Handles the phase changed to GAME_OVER.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void HandlePhaseChanged (object sender, GamePhaseEventArgs e)
		{
			if (e.Phase == GamePhase.GAME_OVER)
			{
				Show ();
			}
		}

		/// <summary>
		/// Handles the game over event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void HandleCharacterGameOver (object sender, CharacterEventArgs e)
		{
			Show ();
		}

		/// <summary>
		/// Handles the character loaded event
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void HandleCharacterLoaded (object sender, CharacterEventArgs e)
		{
			character = e.Character;
			characterHealth = character.CharacterHealth;
			if (characterHealth != null) 
			{
				characterHealth.GameOver += HandleCharacterGameOver;
			}
			else
			{
				Debug.LogWarning("Game over screen cannot be auto shown as it could not find a ChracterHealth");
			}
		}

		/// <summary>
		/// Show this screen.
		/// </summary>
		virtual public void Show() 
		{
			StartCoroutine (DoShow ());
		}
		
		/// <summary>
		/// Do the show.
		/// </summary>
		virtual protected IEnumerator DoShow()
		{
			yield return true;
			visibleComponent.SetActive (true);
			if (showEffects != null && showEffects.Count > 0)
			{
				foreach(FX_Base effect in showEffects)
				{
					effect.StartEffect();
				}
			}
			yield return new WaitForSeconds (minShowTime);
			float elapsedTime = 0.0f;
			while (!CheckForAnyKey() && (totalShowTime == 0 || elapsedTime < totalShowTime)) 
			{
				elapsedTime += Time.deltaTime;
				yield return true;
			}
			Hide();
		}

		/// <summary>
		/// Returns true if any key or button is pressed or held.
		/// </summary>
		virtual protected bool CheckForAnyKey()
		{
			if (UnityEngine.Input.anyKey) return true;
			return false;
		}

		/// <summary>
		/// Hide this screen.
		/// </summary>
		virtual public void Hide()
		{
			StartCoroutine (DoHide ());
		}
		
		/// <summary>
		/// Do the hide.
		/// </summary>
		virtual protected IEnumerator DoHide()
		{
			yield return true;
			if (hideEffects != null && hideEffects.Count > 0)
			{
				foreach(FX_Base effect in hideEffects)
				{
					effect.StartEffect();
				}
			}
			else 
			{
				visibleComponent.SetActive (false);
			}
			for (int i = 0; i < gameOverActions.Length; i++)
			{
				StartCoroutine(DoDeathAction(gameOverActions[i]));
			}
		}

		
		/// <summary>
		/// Does a death action.
		/// </summary>
		/// <param name="action">Action.</param>
		virtual protected IEnumerator DoDeathAction(DeathAction action)
		{
			// Wait...
			if (action.delay > 0) yield return new WaitForSeconds(action.delay);
			// Then act
			switch (action.actionType)
			{
			case DeathActionType.DESTROY_CHARACTER:
				if (character != null)
				{
					Destroy (character.gameObject);
				} 
				else
				{	
					Debug.LogError ("DESTROY_CHARACTER death action is not supported by the GameOverScreen for global game over events.");
				}
				break;
			case DeathActionType.RESPAWN:
				if (character != null)
				{
					LevelManager.Instance.Respawn(character);
				} 
				else
				{	
					Debug.LogError ("RESPAWN death action is not supported by the GameOverScreen for global game over events.");
				}
				break;
			case DeathActionType.RELOAD_SCENE:
				#if !UNITY_4_6 && !UNITY_4_7 && !UNITY_5_1 && !UNITY_5_2
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				#else
				Application.LoadLevel(Application.loadedLevel);
				#endif
				break;
			case DeathActionType.LOAD_ANOTHER_SCENE:
				#if !UNITY_4_6 && !UNITY_4_7 && !UNITY_5_1 && !UNITY_5_2
				LevelManager.PreviousLevel = SceneManager.GetActiveScene().name;
				SceneManager.LoadScene(action.supportingData);
				#else
				LevelManager.PreviousLevel = Application.loadedLevelName;
				Application.LoadLevel(action.supportingData);
				#endif
				break;
			case DeathActionType.SEND_MESSAGE:
				action.supportingGameObject.SendMessage(action.supportingData, SendMessageOptions.DontRequireReceiver);
				break;
			case DeathActionType.CLEAR_RESPAWN_POINTS:
				if (LevelManager.Instance != null) LevelManager.Instance.ClearRespawns();
				break;
			case DeathActionType.RESET_DATA:
				foreach (Persistable persistable in action.supportingGameObject.GetComponents<Persistable>())
				{
					persistable.ResetSaveData();
				}
				break;
			}
		}
	}

}