using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Activates a character object.
	/// </summary>
	public class CharacterLoader : MonoBehaviour
	{
		/// <summary>
		///  Character to activate.
		/// </summary>
		[Tooltip ("Character to activate (note the GameObject this component is attached to will be activated.")]
		public Character character;

		/// <summary>
		/// How long to wait before loading the character.
		/// </summary>
		[Tooltip ("How long to wait before loading the character.")]
		public float delay;

		/// <summary>
		/// Event fired when the cahracter loading finishes.
		/// </summary>
		public event System.EventHandler <CharacterEventArgs> CharacterLoaded;
		
		/// <summary>
		/// Raises the character loaded event.
		/// </summary>
		virtual protected void OnCharacterLoaded()
		{
			if (CharacterLoaded != null)
			{
				CharacterLoaded(this, new CharacterEventArgs(character));
			}
		}

		/// <summary>
		/// Unity Awake hook.
		/// </summary>
		void Awake ()
		{
			Register (this);
		}

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start () 
		{
			LoadCharacter ();
		}
		
		/// <summary>
		/// Unity OnDestroy() hook.
		/// </summary>
		void OnDestroy()
		{
			Deregister (this);
		}

		/// <summary>
		/// Loads the character.
		/// </summary>
		virtual protected void LoadCharacter()
		{
			StartCoroutine (DoLoad ());
		}

		/// <summary>
		/// Does the load after a delay.
		/// </summary>
		virtual protected IEnumerator DoLoad()
		{
			if (LevelManager.Instance != null)
			{
				LevelManager.Instance.Respawn(character);
			}
			yield return new WaitForSeconds(delay);
			character.gameObject.SetActive(true);
			OnCharacterLoaded ();
		}

		#region static behaviour

		protected static List<CharacterLoader> characterLoaders;

		/// <summary>
		/// Gets a character loader (generally for single character games only).
		/// </summary>
		/// <returns>A character loader or null if none found.</returns>
		public static CharacterLoader GetCharacterLoader()
		{
			if (characterLoaders != null && characterLoaders.Count > 0)
			{
				return characterLoaders[0];
			}
			return null;
		}

		/// <summary>
		/// Gets the character loader for the given character or null if no loader matches
		/// </summary>
		/// <returns>The character loader for character.</returns>
		/// <param name="character">Character.</param>
		public static CharacterLoader GetCharacterLoaderForCharacter(Character character)
		{
			if (characterLoaders != null)
			{
				foreach (CharacterLoader loader in characterLoaders)
				{
					if (loader.character == character) return loader;
				}
			}
			return null;
		}

		/// <summary>
		/// Register the specified loader.
		/// </summary>
		/// <param name="loader">Loader.</param>
		protected static void Register(CharacterLoader loader)
		{
			if (characterLoaders == null) characterLoaders = new List<CharacterLoader>();
			characterLoaders.Add (loader);
		}
		
		/// <summary>
		/// Deregister the specified loader.
		/// </summary>
		/// <param name="loader">Loader.</param>
		protected static void Deregister(CharacterLoader loader)
		{
			if (characterLoaders != null && characterLoaders.Contains (loader)) characterLoaders.Remove (loader);
		}

		#endregion
	}
}