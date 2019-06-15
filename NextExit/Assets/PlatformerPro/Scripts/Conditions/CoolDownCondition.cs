using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Condition which requires a cooldown timer to expire;
	/// </summary>
	public class CoolDownCondition : AdditionalCondition
	{
		/// <summary>
		/// The cool down time.
		/// </summary>
		public float coolDownTime = 1.0f;

		/// <summary>
		/// Should we start with the timer expired, or do we need to charge for first use too?
		/// </summary>
		public bool startEnabled = true;

		/// <summary>
		/// Current cool down timer.
		/// </summary>
		protected float currentTimer = 0.0f;

		/// <summary>
		/// Unity Awake hook.
		/// </summary>
		void Awake()
		{
			if (!startEnabled) currentTimer = coolDownTime;
		}

		/// <summary>
		/// Unity Udpate hook.
		/// </summary>
		void Update() 
		{
			if (currentTimer > 0) currentTimer -= TimeManager.FrameTime;
		}

		/// <summary>
		/// Checks the condition. For example a check when entering a trigger.
		/// </summary>
		/// <returns><c>true</c>, if enter trigger was shoulded, <c>false</c> otherwise.</returns>
		/// <param name="character">Character.</param>
		/// <param name="other">Other.</param>
		override public bool CheckCondition(Character character, object other)
		{
			if (currentTimer > 0) return false;
			return true;
		}

		/// <summary>
		/// Applies any activation effects.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="other">Other object supporting the condition.</param>
		override public void Activated(Character character, object other)
		{
			currentTimer = coolDownTime;
		}
	}
}
