using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Shows count of a stackable item.
	/// </summary>
	[RequireComponent (typeof(Text))]
	public class UIStackableItemCounter : MonoBehaviour {

		/// <summary>
		/// The item manager.
		/// </summary>
		public ItemManager itemManager;

		/// <summary>
		/// The type of the item matching the type in the ItemManager.
		/// </summary>
		public string itemType;

		
		/// <summary>
		/// Format string used to display the value.
		/// </summary>
		[Tooltip ("Format string used to display the value. WARNING formatting the string will allocate heap at each change.")]
		public string formatString = "";

		/// <summary>
		/// The bar image.
		/// </summary>
		protected Text counterText;

		/// <summary>
		/// Reference to the character loader (or null if none).
		/// </summary>
		protected CharacterLoader characterLoader;

		/// <summary>
		/// Unity Start() hook.
		/// </summary>
		void Start()
		{
			Init ();
			UpdateText ();
		}

		/// <summary>
		///Unity OnDestory event.
		/// </summary>
		void OnDestroy()
		{
			if (characterLoader != null) characterLoader.CharacterLoaded += HandleCharacterLoaded;
			if (itemManager != null) 
			{
				itemManager.ItemCollected -= HandleItemCollected;
				itemManager.Loaded -= HandleItemCollected;
			}
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void Init()
		{
			counterText = GetComponent<Text> ();
			if (itemManager == null) 
			{
				itemManager = FindObjectOfType<ItemManager>();
				if (itemManager == null)
				{
					characterLoader = FindObjectOfType<CharacterLoader>();
					if (characterLoader != null) characterLoader.CharacterLoaded += HandleCharacterLoaded;
					else Debug.LogError("UIStackable_ItemCounter could not find an ItemManager or Character Loader.");
				}
				else
				{
					itemManager.ItemCollected += HandleItemCollected;
					itemManager.ItemConsumed += HandleItemCollected;
					itemManager.Loaded += HandleItemCollected;
					LevelManager.Instance.Respawned += HandleRespawned;
				}
			}
			else
			{
				itemManager.ItemCollected += HandleItemCollected;
				itemManager.ItemConsumed += HandleItemCollected;
				itemManager.Loaded += HandleItemCollected;
				LevelManager.Instance.Respawned += HandleRespawned;
			}
		}

		/// <summary>
		/// Handles the respawned event from the level manager
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void HandleRespawned (object sender, SceneEventArgs e)
		{
			UpdateText ();
		}

		/// <summary>
		/// Handles the character loaded event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event.</param>
		virtual protected void HandleCharacterLoaded (object sender, CharacterEventArgs e)
		{
			itemManager = e.Character.GetComponentInChildren<ItemManager>();
			if (itemManager != null) 
			{
				itemManager.ItemCollected += HandleItemCollected;
				itemManager.Loaded += HandleItemCollected;
				itemManager.ItemConsumed += HandleItemCollected;
				LevelManager.Instance.Respawned += HandleRespawned;
				UpdateText ();
			} else {
				Debug.LogError ("Character was loaded but not ItemManager was found");
			}
			UpdateText ();
		}

		/// <summary>
		/// Handles the item collected and item consumed event by updating text field.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event.</param>
		virtual protected void HandleItemCollected (object sender, System.EventArgs e)
		{
			UpdateText ();	
		}

		/// <summary>
		/// Updates the text field.
		/// </summary>
		virtual protected void UpdateText()
		{
			if (itemManager != null)
			{
				if (formatString != null && formatString != "") counterText.text = string.Format(formatString, itemManager.ItemCount (itemType));
				else counterText.text = itemManager.ItemCount (itemType).ToString();
			}
		}
	}
}