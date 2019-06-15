using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Blows the character based on their distance from a trigger. 
	/// WARNING: Supports only one character in range at a time. TODO: Update this to support multiplayer.
	/// </summary>
	[RequireComponent (typeof(Trigger))]
	public class WindAffector : MonoBehaviour
	{
		/// <summary>
		/// The amount of acceleration applied.
		/// </summary>
		[Tooltip ("Amount of acceleration applied.")]
		public Vector2 acceleration;

		/// <summary>
		/// If true the velocity is instantly set to acceleration instead of being applied as a change over time.
		/// </summary>
		[Tooltip ("If true the acceleration is reduced based on the distance from the source.")]
		public float fadeWithDistance;

		/// <summary>
		/// If true the velocity is instantly set to acceleration instead of being applied as a change over time.
		/// </summary>
		[Tooltip ("If true the velocity is instantly set to 'acceleration'instead of being applied as a change over time.")]
		public bool instantlySetVelocity;

		/// <summary>
		/// My trigger.
		/// </summary>
		protected Trigger myTrigger;

		/// <summary>
		/// List of characters currently affected by this affector.
		/// </summary>
		protected List<Character> affectedCharacters;

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start()
		{
			Init ();
		}

		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update()
		{
			ApplyEffect ();
		}

		/// <summary>
		/// Unity OnDestroy, clear event listeners.
		/// </summary>
		void OnDestroy()
		{
			myTrigger.TriggerEntered -= HandleTriggerEntered;
			myTrigger.TriggerExited -= HandleTriggerExited;
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void Init()
		{
			myTrigger = GetComponent<Trigger> ();
			myTrigger.TriggerEntered += HandleTriggerEntered;
			myTrigger.TriggerExited += HandleTriggerExited;
			affectedCharacters = new List<Character>();
		}

		/// <summary>
		/// Do the affect to each character in range.
		/// </summary>
		virtual protected void ApplyEffect()
		{
			foreach (Character c in affectedCharacters)
			{
				if (instantlySetVelocity)
				{
					c.SetVelocityX(acceleration.x);
					c.SetVelocityY(acceleration.y);
				}
				else
				{
					Vector2 actualAcceleration = acceleration;
					if (fadeWithDistance > 0 ) 
					{
						float distance = Vector2.Distance(transform.position, c.transform.position);
						float delta = 1 - (distance / fadeWithDistance); 
						if (delta < 0) actualAcceleration = Vector2.zero;
						else actualAcceleration *= delta;
					}
					Debug.Log (actualAcceleration.y);
					c.AddVelocity(actualAcceleration.x * TimeManager.FrameTime, actualAcceleration.y * TimeManager.FrameTime, true);

				}
			}
		}

		/// <summary>
		/// Handles the trigger entered.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		virtual protected void HandleTriggerEntered (object sender, CharacterEventArgs args)
		{
			if (args.Character == null) return;
			if (!affectedCharacters.Contains(args.Character))
			{
				Debug.Log ("Add");
				affectedCharacters.Add(args.Character);
			}
		}

		/// <summary>
		/// Handles the trigger exited.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		virtual protected void HandleTriggerExited (object sender, CharacterEventArgs args)
		{
			if (args.Character == null) return;
			if (affectedCharacters.Contains(args.Character))
			{
				Debug.Log ("Remove");
				affectedCharacters.Remove(args.Character);
			}
		}
	}
}