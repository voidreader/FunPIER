using UnityEngine;
using System.Collections;


namespace PlatformerPro
{
	
	/// <summary>
	/// A trigger that fires on and off base don a timer.
	/// </summary>
	public class TimerTrigger : Trigger
	{
		/// <summary>
		/// The time we take to fire an enter event after start or an exit event.
		/// </summary>
		[Tooltip("The time we take to fire an enter event after start or an exit event.")]
		public float timeToEnter = 1.0f;

		/// <summary>
		/// The time we take to fire an exit event after an enter event.
		/// </summary>
		[Tooltip("The time we take to fire an exit event after an enter event.")]
		public float timeToExit = 1.0f;

		/// <summary>
		/// Number of times to repeat. Use 0 or -1 for forever.
		/// </summary>
		[Tooltip("Number of times to repeat. Use 0 or -1 for forever.")]
		public int numberOfRepeats = -1;

		/// <summary>
		/// Character to use in events. If null we will find one in the scene.
		/// </summary>
		[Tooltip ("Character to use in events. If null we will find one in the scene.")]
		public Character character;

		/// <summary>
		/// Tracks the next state.
		/// </summary>
		protected bool nextStateIsExit;

		/// <summary>
		/// The timer.
		/// </summary>
		protected float timer;

		/// <summary>
		/// NUmber of times we have repeated.
		/// </summary>
		protected int repeatCount;


		/// <summary>
		/// Unity Update() hook.
		/// </summary>
		void Update()
		{
			UpdateTimer ();
		}

		/// <summary>
		/// Initialise the sensor.
		/// </summary>
		override protected void Init()
		{
			base.Init ();
			this.timer = timeToEnter;
			if (character == null) character = FindObjectOfType<Character> ();
		}

		virtual protected void UpdateTimer() 
		{
			if (numberOfRepeats > 0 && repeatCount >= numberOfRepeats) return;
			timer -= TimeManager.FrameTime;
			if (timer <= 0.0f)
			{
				if (nextStateIsExit) 
				{
					LeaveTrigger(character);
					timer = timeToEnter;
					nextStateIsExit = false;
					repeatCount++;
				}
				else
				{
					EnterTrigger(character);
					timer = timeToExit;
					nextStateIsExit = true;
				}
			}
		}

	}
}
