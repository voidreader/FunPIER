using UnityEngine;
using System.Collections;

namespace PlatformerPro
{

	/// <summary>
	/// A hit box which damages enemies when the character jumps on them.
	/// It does this by ignoring hits that happen while character is grounded or moving upwards.
	/// </summary>
	public class JumpOnHeadHitBox : CharacterHitBox
	{
		/// <summary>
		/// The amount of damage a jump on head does.
		/// </summary>
		[Tooltip ("The amount of damage a jump on head does.")]
		public int damageAmount;

		/// <summary>
		/// The height of the bobble.
		/// </summary>
		[Tooltip ("How high to bobble (bounce).")]
		public float bobbleHeight;

		/// <summary>
		/// If true the bobble will be handled like a very small jump, else it will just set y velocity.
		/// </summary>
		[Tooltip ("If true the bobble will be handled like a very small jump, else it will just set y velocity.")]
		public bool doBobbleAsJump;

		/// <summary>
		/// If true enemies in the hiding state will be 'kicked' even if the character isn't jumping.
		/// </summary>
		[Tooltip ("If true enemies in the HIDING state will be 'kicked' even if the character isn't jumping.")]
		public bool kickHidingEnemies;

		/// <summary>
		/// Gets the header string used to describe the component.
		/// </summary>
		/// <value>The header.</value>
		override public string Header
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Unity Awake hook.
		/// </summary>
		void Awake() 
		{
			Init ();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual public void Init()
		{
			DamageInfo info = new DamageInfo (damageAmount, DamageType.PHYSICAL, Vector2.zero);
			base.Init (info);
			info.DamageCauser = character;
		}

		/// <summary>
		/// Do the actual hit.
		/// </summary>
		/// <param name="other">Other.</param>
		override protected bool DoHit(Collider2D other)
		{
			if (character.Velocity.y < 0 || character.PreviousVelocity.y < 0)
			{
				if (base.DoHit (other))
				{
					DoBobble();
					StartCoroutine (ResetCollider ());
					return true;
				}
				return false;
			}
			if (kickHidingEnemies)
			{
				IHurtable hurtBox = (IHurtable) other.gameObject.GetComponent(typeof(IHurtable));
				// Got a hurt box and its not ourselves
				if (hurtBox != null && !hasHitCharacter && hurtBox.Mob is Enemy && ((Enemy)hurtBox.Mob).State == EnemyState.HIDING)
				{
					damageInfo.Direction = transform.position - other.transform.position;
					hurtBox.Damage(damageInfo);
					hasHitCharacter = true;
					return true;
				}
			}
			return false;

		}

		/// <summary>
		/// Does the bounce in to air after a jump.
		/// </summary>
		virtual protected void DoBobble()
		{
			if (character is Character) 
			{
				if (doBobbleAsJump)
				{

					((Character)character).DefaultAirMovement.DoOverridenJump(bobbleHeight, 1);
				}
				else
				{
					float initialVelocity = Mathf.Sqrt(-2.0f * character.Gravity * bobbleHeight);
					((Character)character).SetVelocityY(initialVelocity);
				}
			}
			else
			{
				Debug.LogWarning("This script should be attached to a Character.");
			}
		}

		/// <summary>
		/// Resets the hitbox ready to cause damage again.
		/// </summary>
		/// <returns>The collider.</returns>
		virtual protected IEnumerator ResetCollider()
		{
			// We could maybe wait more than one frame....
			yield return true;
			hasHitCharacter = false;
		}
	}
}
