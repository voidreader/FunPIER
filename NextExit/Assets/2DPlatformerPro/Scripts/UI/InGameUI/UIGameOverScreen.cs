using UnityEngine;
#if !UNITY_4_6 && !UNITY_5_1 && !UNITY_5_2
using UnityEngine.SceneManagement;
#endif
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Displays some UI content on game over.
	/// </summary>
	public class UIGameOverScreen : MonoBehaviour
	{
		/// <summary>
		/// GameObject holding the content to show.
		/// </summary>
		[Tooltip ("GameObject holding the content to show.")]
		public GameObject visibleComponent;

		/// <summary>
		/// Show on respawn.
		/// </summary>
		[Tooltip ("Do we automatically show this on the GameOver ecent, or do we wait for something to call us?")]
		public bool autoShowOnGameOver;

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

		/// <summary>
		/// Reference to the character.
		/// </summary>
		[Tooltip ("Character to check for game over on.")]
		public Character character;

		/// <summary>
		/// What do we do after screen has been shown.
		/// </summary>
		[Tooltip ("What to do after we shown the screen.")]
		public DeathAction[] gameOverActions;

		/// <summary>
		/// Reference to the character health (or null if none).
		/// </summary>
		protected CharacterHealth characterHealth;

		/// <summary>
		/// Reference to the character loader (or null if none).
		/// </summary>
		protected CharacterLoader characterLoader;

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start()
		{
			Init ();
		}

		/// <summary>
		/// Unity OnDestroy() hook.
		/// </summary>
		void OnDestroy()
		{
			if (autoShowOnGameOver && characterHealth) characterHealth.GameOver -= HandleGameOver;
			if (characterLoader != null) characterLoader.CharacterLoaded -= HandleCharacterLoaded;
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		protected virtual void Init()
		{
			if (character == null) character = GameObject.FindObjectOfType<Character>();
			if (character == null) 
			{
				characterLoader = CharacterLoader.GetCharacterLoader ();
				if (characterLoader != null) 
				{
					characterLoader.CharacterLoaded += HandleCharacterLoaded;
				}
				else
				{
					Debug.LogWarning("Game over screen cannot find a Character.");
				}
			}
			else if (autoShowOnGameOver)
			{
				characterHealth = character.GetComponentInChildren<CharacterHealth>();
				if (characterHealth) 
				{
					characterHealth.GameOver += HandleGameOver;
				}
				else
				{
					Debug.LogWarning("Game over screen cannot be auto shown as it could not find a ChracterHealth");
				}
			}

		}

		/// <summary>
		/// Handles the game over event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void HandleGameOver (object sender, DamageInfoEventArgs e)
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
			if (autoShowOnGameOver)
			{
				characterHealth = character.GetComponentInChildren<CharacterHealth>();
				if (characterHealth) 
				{
					characterHealth.GameOver += HandleGameOver;
				}
				else
				{
					Debug.LogWarning("Game over screen cannot be auto shown as it could not find a ChracterHealth");
				}
			}
		}


		/// <summary>
		/// Handles the respawned event
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event args.</param>
		void HandleRespawned (object sender, SceneEventArgs e)
		{
			Show ();
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
				Destroy(character.gameObject);
				break;
			case DeathActionType.RESPAWN:
				Debug.LogError("RESPAWN is not supported by the GameOverScreen");
				break;
			case DeathActionType.RELOAD_SCENE:
				#if !UNITY_4_6 && !UNITY_5_1 && !UNITY_5_2
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				#else
				Application.LoadLevel(Application.loadedLevel);
				#endif
				break;
			case DeathActionType.LOAD_ANOTHER_SCENE:
				#if !UNITY_4_6 && !UNITY_5_1 && !UNITY_5_2
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