using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// Scales an image bar based on the percentage of a stackable item (current/max).
	/// </summary>
	[RequireComponent (typeof(Image))]
	public class UIStackable_PercentageBar : MonoBehaviour
	{

		/// <summary>
		/// The item manager.
		/// </summary>
		public ItemManager itemManager;

		/// <summary>
		/// The type of the item matching the type in the ItemManager.
		/// </summary>
		public string itemType;

		/// <summary>
		/// If true use fill on the iamge to represent the fill amount;
		/// </summary>
		public bool useFill;

		/// <summary>
		/// The bar image.
		/// </summary>
		protected Image barImage;


		void Start()
		{
			Init ();
		}

		void Update()
		{
			UpdateImage ();
		}

		virtual protected void Init()
		{
			barImage = GetComponent<Image> ();
			if (itemManager == null) 
			{
				itemManager = FindObjectOfType<ItemManager>();
				if (itemManager == null)
				{
					Debug.LogError("UIStackable_PercentageBar could not find an ItemManager.");
				}
				else
				{
					Debug.LogWarning("No ItemManager assigned, using the first one found.");
         		}
			}
		}

		virtual protected void UpdateImage()
		{
			float percentage = (float)itemManager.ItemCount (itemType) / (float)itemManager.ItemMax (itemType); 
			if (useFill)
			{
				barImage.fillAmount = percentage;
			}
			else
			{
				barImage.rectTransform.sizeDelta = new Vector2 (100.0f * percentage, barImage.rectTransform.sizeDelta.y);
			}
		}
	}
}