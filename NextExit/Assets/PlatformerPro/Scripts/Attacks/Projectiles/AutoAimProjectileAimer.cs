using UnityEngine;
using System.Collections;

namespace PlatformerPro 
{
	/// <summary>
	/// Projecitle aimer that always aims at the target. Not for use on characters!
	/// MULTIPLAYER TODO: This class only supports a single player game as it always targets the last loaded character.
	/// </summary>
	public class AutoAimProjectileAimer : ProjectileAimer 
	{
		/// <summary>
		/// The offset distance for the staring point of the projectile.
		/// </summary>
		[Tooltip ("How far from the centre of character do we shoot.")]
		public float offsetDistance;

		/// <summary>
		/// The character.
		/// </summary>
		[Tooltip ("The target. If blank this will be looked up via CharacterLoader on Init, and then FindObjectOfType of no CharacterLoader.")]
		public IMob target;

		/// <summary>
		/// Reference to character loader
		/// </summary>
		protected PlatformerProGameManager characterLoader;

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start()
		{
			Init ();	
		}

		/// <summary>
		/// Unity On destroy, hook... clean up.
		/// </summary>
		void OnDestory()
		{
			if (PlatformerProGameManager.Instance != null)
			{
				PlatformerProGameManager.Instance.CharacterLoaded -= CharacterLoaded;
			}
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void Init()
		{
			PlatformerProGameManager.Instance.CharacterLoaded += CharacterLoaded;
		}


		/// <summary>
		/// Handles a cahracter being loaded.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void CharacterLoaded (object sender, CharacterEventArgs e)
		{
			target = e.Character;
		}

		/// <summary>
		/// Gets the aim direction.
		/// </summary>
		/// <returns>The aim direction.</returns>
		override public Vector2 GetAimDirection(Component character)
		{
			if (target == null) return Vector2.zero;
			Vector2 angle = ((Component)target).transform.position - transform.position;
			return angle.normalized;
		}

		/// <summary>
		/// Offsets the projectile from character position.
		/// </summary>
		/// <returns>The aim offset.</returns>
		/// <param name="character">Character.</param>
		override public Vector2 GetAimOffset (IMob character)
		{
			return GetAimDirection((Component)character).normalized * offsetDistance;
		}
	}
}
