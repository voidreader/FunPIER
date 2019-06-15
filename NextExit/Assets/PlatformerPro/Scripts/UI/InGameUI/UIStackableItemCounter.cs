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
		/// The player identifier.
		/// </summary>
		public int playerId;

		/// <summary>
		/// The item manager.
		/// </summary>
		protected ItemManager itemManager;

		/// <summary>
		/// Cached level manager reference used to unregister events
		/// </summary>
		protected LevelManager levelManager;

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
		protected PlatformerProGameManager characterLoader;

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
			if (levelManager != null)
			{
				levelManager.Respawned -= HandleRespawn;
			}
			if (itemManager != null) 
			{
				itemManager.ItemCollected -= HandleItemCollected;
				itemManager.Loaded -= HandleItemCollected;
				itemManager.ItemConsumed += HandleItemCollected;
			}
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void Init()
		{
			counterText = GetComponent<Text> ();
			characterLoader = PlatformerProGameManager.Instance;
			levelManager = LevelManager.Instance;
			levelManager.Respawned += HandleRespawn;
		}

		void HandleRespawn (object sender, CharacterEventArgs e)
		{
			if (playerId == -1 || e.Character.PlayerId == playerId)
			{
				itemManager = e.Character.ItemManager;
				if (itemManager != null)
				{
					itemManager.ItemCollected += HandleItemCollected;
					itemManager.Loaded += HandleItemCollected;
					itemManager.ItemConsumed += HandleItemCollected;
					UpdateText ();
				} else
				{
					Debug.LogError ("Character was loaded but not ItemManager was found");
				}
			}
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