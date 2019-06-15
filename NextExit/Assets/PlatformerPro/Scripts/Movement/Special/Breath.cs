using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Add this to your character (or child) if you want a breath mechanic (typically used when swimming).
	/// Breath is typically between 0 and 1, but you can increase it by setting MaxBreath.
	/// </summary>
	public class Breath : MonoBehaviour 
	{
		[Header ("Triggers")]
		/// <summary>
		/// If this is set lose breath only when this movement is active.
		/// </summary>
		[Tooltip ("If this movement is active lose breath. If its a swim movement it will also gain breath when on surface. If empty you will always loose breath (for example as if you were in space).")]
		public Movement loseBreathMovement;

		[Header ("Breath Details")]
		/// <summary>
		/// The max breath. We typically track breath between 0 and 1, but you can change that if you want to
		/// (for example) have a power up that increases breath.
		/// </summary>
		[Tooltip ("The max breath. We typically track breath between 0 and 1, but you can change that if you want to (for example) have a power up that increases breath.")]
		public float maxBreath = 1.0f;

		/// <summary>
		/// Rate at which we lose breath when we do need to hold it, per second.
		/// </summary>
		[Tooltip ("Rate at which we lose breath when we do need to hold it, per second.")]
		public float loseRate;

		[Tooltip ("Rate at which we gain breath when we dont need to hold it, per second.")]
		public float gainRate;

		[Header ("Damage")]
		[Tooltip ("Damage type that breath causes. Use AUTO_KILL if you want losing breath to mean immediate death.")]
		public DamageType damageType;

		[Tooltip ("Once we run out of breath how often do we take damage (the 'damage tick'). Measured in seconds.")]
		public float damageRate;

		[Tooltip ("How much damage does each 'damage tick' cause'")]
		public int damageAmount;

        /// <summary>
        /// Track if we think are underwater (used during damage to determine if we should gain or lose breath).
        /// </summary>
        protected bool underwater;

		/// <summary>
		/// Current breath.
		/// </summary>
		protected float breathAmount;

		/// <summary>
		/// Cached character reference.
		/// </summary>
		protected Character character;

		/// <summary>
		/// Cached character health reference.
		/// </summary>
		protected CharacterHealth characterHealth;

		/// <summary>
		/// Tracks if we are out of breath or not.
		/// </summary>
		protected bool outOfBreath;

		/// <summary>
		/// Cached damage info.
		/// </summary>
		protected DamageInfo damageInfo;

		/// <summary>
		/// The cached loseBreathMovement as a swim movement.
		/// </summary>
		protected SpecialMovement_Swim cachedSwimMovement;

		/// <summary>
		/// Tracks if we need to check for a siwm movement yet.
		/// </summary>
		// TODO We should listen to loading events instead of doing this.
		protected bool checkForSwimMovement = true;

		/// <summary>
		/// Gets the raw value of the breath.
		/// </summary>
		/// <value>The current breath.</value>
		public float CurrentBreath
		{
			get
			{
				return breathAmount;
			}
		}

		/// <summary>
		/// Gets the current breath as percentage between 0 and 1.
		/// </summary>
		/// <value>The current breath as percentage.</value>
		public float CurrentBreathAsPercentage
		{
			get
			{
                if (breathAmount <= 0) return 0.0f;
				return Mathf.Min (1.0f, (breathAmount + 0.00001f) / maxBreath);
			}
		}

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start ()
		{
			Init ();
		}

		/// <summary>
		/// Unity update hook.
		/// </summary>
		void Update()
		{
			if (enabled) DoBreath ();
		}

		/// <summary>
		/// Initialise this instance.
		/// </summary>
		virtual protected void Init()
		{
			character = GetComponentInParent<Character> ();
			if (character == null)
			{
				Debug.LogWarning ("Breath could not find Character. Disabling.");
				enabled = false;
				return;
			}
			characterHealth = character.GetComponentInChildren<CharacterHealth> ();
			if (characterHealth == null)
			{
				Debug.LogWarning ("Breath could not find Characterhealth. Disabling.");
				enabled = false;
				return;
			}
			if (maxBreath <= 0)
			{
				Debug.LogWarning("Max breath is smaller than or equal to 0. Disabling.");
				enabled = false;
				return;
			}
			breathAmount = maxBreath;
			damageInfo = new DamageInfo (damageAmount, damageType, Vector2.zero);
		}

		/// <summary>
		/// Does the work for the breathing mechanism.
		/// </summary>
		virtual protected void DoBreath()
		{
            if (character.ActiveMovement is DamageMovement)
            {
                if (underwater)
                {
                    breathAmount -= loseRate * TimeManager.FrameTime;
                    if (breathAmount < 0) breathAmount = 0;
                }
            }
            else if (HoldingBreath ())
			{
                underwater = true;
                breathAmount -= loseRate * TimeManager.FrameTime;
				if (breathAmount < 0) breathAmount = 0;
				// Zero?
				if (breathAmount == 0)
				{
					DoOutOfBreathDamage();
				}
			}
			else
			{
                underwater = false;
                GainBreath(gainRate * TimeManager.FrameTime);
			}
		}

		/// <summary>
		/// Gains the specified amount of breath.
		/// </summary>
		/// <param name="amount">Amount.</param>
		public void GainBreath(float amount)
		{
			breathAmount += amount;
			if (breathAmount > maxBreath) breathAmount = maxBreath;
			if (breathAmount > 0)
			{
				outOfBreath = false;
			}
		}

        /// <summary>
        /// Are we holding our breath?
        /// </summary>
        /// <returns><c>true</c>, if breath was being held, <c>false</c> otherwise.</returns>
        virtual protected bool HoldingBreath()
        {
            if (checkForSwimMovement) CheckForSwimMovement();
            if (cachedSwimMovement == null) return true;
            if (character.ActiveMovement == cachedSwimMovement || character.ActiveMovement == cachedSwimMovement.Implementation)
            {
                return !cachedSwimMovement.OnSurface;
            }
            return false;
        }

        /// <summary>
        /// Starts doing out of breath damage unless already started.
        /// </summary>
        virtual protected void DoOutOfBreathDamage()
		{
			if (!outOfBreath) StartCoroutine(OutOfBreathRoutine());
		}

		/// <summary>
		/// Loops and does damage each damage tick. Stops if out of breath is false.
		/// </summary>
		virtual protected IEnumerator OutOfBreathRoutine()
		{
			outOfBreath = true;
			while (outOfBreath)
			{
				characterHealth.Damage(damageInfo);
				yield return new WaitForSeconds(damageRate);
			}
		}

		/// <summary>
		/// Try to get a cached swim movement.
		/// </summary>
		void CheckForSwimMovement()
		{
			if (loseBreathMovement == null)
			{
				checkForSwimMovement = false;
			}

            if (loseBreathMovement.Implementation == null) return;

			if (loseBreathMovement.Implementation is SpecialMovement_Swim)
			{
				cachedSwimMovement = (SpecialMovement_Swim)loseBreathMovement.Implementation;
			}
			checkForSwimMovement = false;
		}
	}
}
