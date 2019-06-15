using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformerPro
{
	/// <summary>
	/// Listens to Equip/Unequip events and 
	/// </summary>
	public class EquipResponder : PlatformerProMonoBehaviour 
	{
		/// <summary>
		/// How to repsond to matching items being equipped.
		/// </summary>
		[Tooltip ("Defines how to respond to matching items being equipped.")]
		public EquipResponse[] responses;

		/// <summary>
		/// The equipment manager.
		/// </summary>
		protected EquipmentManager equipmentManager;

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start()
		{
			PostInit ();
		}

		/// <summary>
		/// Called from Start to initialise references.
		/// </summary>
		virtual protected void PostInit()
		{
			Character c = GetComponentInParent<Character> ();
			if (c == null)
			{
				Debug.LogWarning ("EquipResponder is expected to be the child of a Character");
				return;
			}
		
			equipmentManager = c.gameObject.GetComponentInChildren<EquipmentManager> ();
			if (c == null)
			{
				Debug.LogWarning ("EquipResponder is expected to be the child of a Character that has an EquipmentManager");
				return;
			}
			equipmentManager.ItemEquipped += HandleItemEquipped;
			equipmentManager.ItemUnequipped += HandleItemUnequipped;
		}

		/// <summary>
		/// Handles an item being equipped.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event args.</param>
		virtual protected void HandleItemEquipped (object sender, ItemEventArgs e)
		{
			for (int i = 0; i < responses.Length; i++)
			{
				if (e.Type == responses [i].itemId) DoAction (responses [i], false);
			}
		}

		/// <summary>
		/// Handles an item being unequipped.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">Event args.</param>
		virtual protected void HandleItemUnequipped (object sender, ItemEventArgs e)
		{
			for (int i = 0; i < responses.Length; i++)
			{
				if (e.Type == responses [i].itemId) DoAction (responses [i], true);
			}
		}

		/// <summary>
		/// Does the action.
		/// </summary>
		/// <param name="response">Response data.</param>
		/// <param name="isUnequip">If set to <c>true</c> this is an unequip.</param>
		virtual protected void DoAction(EquipResponse response, bool isUnequip)
		{
			if (response.type == EquipResponseType.ACTIVATE_ON_EQUIP)
			{
				response.target.SetActive (isUnequip ? false : true);
			}
			else if (response.type == EquipResponseType.DEACTIVATE_ON_EQUIP)
			{
				response.target.SetActive (isUnequip ? true : false);
			}
		}
	}

	/// <summary>
	/// Data defining how we handle an item being equipped.
	/// </summary>
	[System.Serializable]
	public class EquipResponse
	{
		[ItemType]
		public string itemId;
		public EquipResponseType type;
		public GameObject target;
	}

	/// <summary>
	/// Availble responses to equipping an item.
	/// </summary>
	public enum EquipResponseType
	{
		ACTIVATE_ON_EQUIP,
		DEACTIVATE_ON_EQUIP,
	}
}
