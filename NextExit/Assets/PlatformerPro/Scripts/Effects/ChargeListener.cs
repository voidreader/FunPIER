using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformerPro
{
	/// <summary>
	/// Listens for an attack being charged and sends events based on charge level.
	/// </summary>
	public class ChargeListener : MonoBehaviour
	{
		/// <summary>
		/// Name of the attack to check.
		/// </summary>
		public string attackType;

		/// <summary>
		/// Link to attack to use. If empty one will be searched for.
		/// </summary>
		public BasicAttacks attackMovement;

		/// <summary>
		/// Occurs when charging hits a threshold.
		/// </summary>
		public event System.EventHandler<ChargeEventArgs> Charge;

		/// <summary>
		/// Occurs when charge ends.
		/// </summary>
		public event System.EventHandler<ChargeEventArgs> ChargeEnded;

		/// <summary>
		/// The current event level.
		/// </summary>
		protected int currentEventLevel = -1;

		/// <summary>
		/// Cached index of the attack we are listening to.
		/// </summary>
		protected int attackIndex = -1;

		/// <summary>
		/// Raises the charge event.
		/// </summary>
		/// <param name="level">Level.</param>
		protected void OnCharge(int level)
		{
			if (Charge != null) Charge (this, new ChargeEventArgs (level));
		}

		/// <summary>
		/// Raises the charge endedevent.
		/// </summary>
		/// <param name="level">Level.</param>
		protected void OnChargeEnded()
		{
			if (ChargeEnded != null) ChargeEnded (this, new ChargeEventArgs (-1));
		}

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start()
		{
			Init();
		}

		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update() 
		{
			CheckForCharge ();
		}

		/// <summary>
		/// Initialise this instance.
		/// </summary>
		protected void Init() 
		{
			if (attackMovement == null)
			{
				Character c = GetComponentInParent<Character> ();
				if (c != null) attackMovement = c.GetComponentInChildren<BasicAttacks> ();
				if (attackMovement == null)
				{
					Debug.LogWarning ("Charge listener couldn't find an attack movement.");
					Destroy (this);
					return;
				}
			}
			attackIndex = attackMovement.GetIndexForName (attackType);
			if (attackIndex == -1)
			{
				Debug.LogWarning ("Charge listener couldn't find an attack with name: " + name);
				Destroy (this);
			}
		}

		/// <summary>
		/// Checks for charge and raises events if charge level passes thresholds.
		/// </summary>
		protected void CheckForCharge()
		{
			int charge = (int)(attackMovement.GetChargeForAttack (attackIndex) + 0.01f);
			if (charge <= 0)
			{
				if (currentEventLevel >= 0)
				{
					currentEventLevel = -1;
					OnChargeEnded ();
				}
				return;
			}
			if (charge > currentEventLevel)
			{
				currentEventLevel = charge;
				OnCharge (charge);
			}
		}
	}

	/// <summary>
	/// Charge event arguments.
	/// </summary>
	public class ChargeEventArgs : System.EventArgs
	{
		public int Amount
		{
			get; protected set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.ChargeEventArgs"/> class.
		/// </summary>
		public ChargeEventArgs ()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlatformerPro.ChargeEventArgs"/> class.
		/// </summary>
		/// <param name="amount">Charge amount.</param>
		public ChargeEventArgs (int amount)
		{
			Amount = amount;
		}

	}
}