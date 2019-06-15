/**
 * This code is part of Platformer PRO and is copyright John Avery 2014.
 */

using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// An item that boosts remaining time.
	/// </summary>
	public class TimeBoost : Item
	{

		/// <summary>
		/// Amount of time to add in seconds.
		/// </summary>
		[Tooltip ("Amount of time to add in seconds.")]
		[SerializeField]
		protected float amount;

		void Start()
		{
			if (!(TimeManager.Instance is TimeManagerWithTimer))
			{
				Debug.LogError ("TimeBoost will not work as the TimeManager is not a TimeManagerWithTimer");
			}
		}

		/// <summary>
		/// Do the collection.
		/// </summary>
		/// <param name="character">Character doing the collection.</param>
		override protected void DoCollect(Character character)
		{
			((TimeManagerWithTimer)TimeManager.Instance).AddTime (amount);
			base.DoCollect (character);
		}
	}

}