using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	
	/// <summary>
	/// Stores data about a basic attack such as its animation, key, and hit box.
	/// </summary>
	[System.Serializable]
	public class ComboAttackData : BasicAttackData
	{
		/// <summary>
		/// The type of the combo.
		/// </summary>
		public ComboType comboType;

		/// <summary>
		/// Attack which this combo triggers from. Empty or null means trigger from ANY other attack.
		/// </summary>
		public string initialAttack;

		/// <summary>
		/// Defines the minimum value for the time window in which the user must press the combo key.
		/// </summary>
		public float minWindowTime = 0.0f;

		/// <summary>
		/// Defines the maximum value for the time window in which the user must press the combo key.
		/// </summary>
		public float maxWindowTime = 1.0f;

	}

	/// <summary>
	/// Enumeration of types of combo move.
	/// </summary>
	public enum ComboType
	{
		QUEUED = 1,
		POST_HIT = 2,
		CANCEL = 3
	}

	/// <summary>
	/// Combo type enum extensions.
	/// </summary>
	public static class ComboTypeEnumExtensions
	{
		public static string GetDescription(this ComboType me)
		{
			switch(me)
			{
				case ComboType.QUEUED: return "The combo attack is triggered by pressing a button during the initial attack and will start when the inital attack finishes.";
				case ComboType.POST_HIT: return "The combo attack is triggered by pressing a button after successfully hitting an enemy and will start when the inital attack finishes.";
				case ComboType.CANCEL: return "The combo attack is triggered by pressing a button and starts straight away cancelling the existing attack";
			}
			return "No information available.";
		}
	}
}
