/**
 * This code is part of Platformer PRO and is copyright John Avery 2014.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// Responds to power up collection by implementing the power.
	/// </summary>
	public class PowerUpResponder : GenericResponder, ICharacterReference
	{

		/// <summary>
		/// The actions needed to hard reset the character back to starting state.
		/// </summary>
		public PowerUpResponse resetResponse;

		/// <summary>
		/// Details about the reponse this character has to a given power up.
		/// </summary>
		public List<PowerUpResponse> responses;

		/// <summary>
		/// The cached character reference.
		/// </summary>
		protected Character character;

		/// <summary>
		/// The cached character health reference. Used to listen to death events.
		/// </summary>
		protected CharacterHealth characterHealth;

		/// <summary>
		/// Event for powered up state changes.
		/// </summary>
		public event System.EventHandler <ItemEventArgs> PowerUp;

		public Character Character 
		{
			get { return character; }
		}

		/// <summary>
		/// Raises the power up event.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="character">Character.</param>
		virtual protected void OnPowerUp(string type, Character character)
		{
			if (PowerUp != null)
			{
				PowerUp(this, new ItemEventArgs(ItemClass.POWER_UP, type, character));
			}
		}

		/// <summary>
		/// Unity start hook.
		/// </summary>
		void Start()
		{
			Init ();
		}
		
		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void Init()
		{
			character = gameObject.GetComponentInParent<Character>();
			if (character == null) 
			{
				Debug.LogError ("A PowerUpResponder must be the child of a character");
			}
			else
			{
				// Reset all the power ups
				HardReset();
				characterHealth = character.GetComponentInChildren<CharacterHealth>();
				if (characterHealth != null) characterHealth.Died += HandleDied;
			}
		}

		/// <summary>
		/// Unity on destroy
		/// </summary>
		void OnDestroy()
		{
			if (characterHealth != null) characterHealth.Died -= HandleDied;
		}

		/// <summary>
		/// Handles the character dying by doing a hard reset.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void HandleDied (object sender, DamageInfoEventArgs e)
		{
			HardReset ();
		}

		/// <summary>
		/// Sets the power up timer.
		/// </summary>
		/// <param name="time">Time.</param>
		/// <param name="resetResponses">Which set of reset actions to run when the timer expires.</param>
		virtual protected void SetTimer(int time, PowerUpResponse resetResponse)
		{
			StopAllCoroutines ();
			StartCoroutine (DoResetAfterTime (time, resetResponse));
		}

		/// <summary>
		/// Does the listed power up reset after the given time.
		/// </summary>
		/// <param name="time">The powers up lifetime.</param>
		/// <param name="resetResponses">Which set of reset actions to run when the timer expires.</param>
		virtual protected IEnumerator DoResetAfterTime(int time, PowerUpResponse resetResponse)
		{
			yield return new WaitForSeconds (time);
			ResetPowerUp (resetResponse);
		}

		/// <summary>
		/// Resets the power up with the given responses.
		/// </summary>
		/// <param name="resetResponses">Which set of reset actions to run.</param>
		virtual public void ResetPowerUp(PowerUpResponse resetResponse) 
		{

			foreach (PowerUpResponse r in responses)
			{
				if (resetResponse.powerUpReset == r.type)
				{
					for(int i = 0; i < r.actions.Length; i++) DoAction (r.actions[i], null);

				}
			}

			// Remove any associated event listeners
			if (resetResponse.damageResetListener != null) 
			{
				CharacterHealth health = character.GetComponentInChildren<CharacterHealth>();
				if (health != null)
				{
					health.Damaged -= resetResponse.damageResetListener;
					resetResponse.damageResetListener = null;
				}
			}
		}

		/// <summary>
		/// Collect the specified power up.
		/// </summary>
		/// <param name="powerUp">Power up.</param>
		/// <returns>true if a response was found.</returns>
		public bool Collect(PowerUp powerUp) 
		{
			return Collect (powerUp.type);
		}

		/// <summary>
		/// Collect the specified power up by type.
		/// </summary>
		/// <param name="powerUpType">Power up type.</param>
		/// <returns>true if a response was found.</returns>
		public bool Collect(string powerUpType) 
		{
			// Apply powers
			foreach (PowerUpResponse response in responses)
			{
				if (response.type == powerUpType)
				{
					for(int i = 0; i < response.actions.Length; i++) DoAction (response.actions[i], null);
					OnPowerUp(powerUpType, character);
					if (response.time > 0) SetTimer(response.time, response);
					if (response.resetOnDamage)
					{
						CharacterHealth health = character.GetComponentInChildren<CharacterHealth>();
						if (health == null)
						{
							Debug.LogWarning ("Power up cannot reset on damage as no character health could be found");
						}
						else
						{
							if (response.damageResetListener != null) 
							{
								health.Damaged -= response.damageResetListener;
								response.damageResetListener = null;
							}
							response.damageResetListener = delegate (object sender, DamageInfoEventArgs e) {
								ResetPowerUp (response);
							};
							health.Damaged += response.damageResetListener;
						}
					}
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Runs the 
		/// </summary>
		public void HardReset()
		{
			for(int i = 0; i < resetResponse.actions.Length; i++) DoAction (resetResponse.actions[i], null);
		}

	}
}