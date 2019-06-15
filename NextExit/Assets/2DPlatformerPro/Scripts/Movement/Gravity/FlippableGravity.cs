using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// A gravity which can be flipped.
	/// </summary>
	public class FlippableGravity : Gravity
	{
		
		#region members

		/// <summary>
		/// If > 0 then the player can flip gravity using the specified action key.
		/// </summary>
		[Tooltip ("Which action key is used to flip gravity. Use -1 if the character cannot control gravity flip.")]
		public int gravityFlipActionKey = -1;

		/// <summary>
		/// Should the character be immediately rotated.
		/// </summary>
		[Tooltip ("Should the character be immediately rotated 180 degrees.")]
		public bool flipCharacterImmediately;

		/// <summary>
		/// Should we set the cahracters Y velocity to 0 on flip.
		/// </summary>
		[Tooltip ("Should we move the character slighlty in Y to help ensure its position is accurate after a flip.")]
		public float bumpSpriteInY;

		/// <summary>
		/// Is the gravity flipped.
		/// </summary>
		protected bool gravityIsFlipped;


		#endregion

		#region properties

		/// <summary>
		/// Gets a value indicating whether the gravity flipped.
		/// </summary>
		override public bool IsGravityFlipped
		{
			get
			{
				return gravityIsFlipped;
			}
		}
		
		#endregion

		#region events

		/// <summary>
		/// Event thrown when gravity flipped.
		/// </summary>
		public event System.EventHandler <System.EventArgs> GravityFlipped;

		/// <summary>
		/// Raises the gravity flipped event.
		/// </summary>
		protected void OnGravityFlipped()
		{
			if (GravityFlipped != null) GravityFlipped (this, null);
		}

		#endregion

		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update()
		{
			if (gravityFlipActionKey >= 0 && character.Input.GetActionButtonState(gravityFlipActionKey) == ButtonState.DOWN)
			{
				FlipGravity();
			}
		}

		/// <summary>
		/// Flips the gravity.
		/// </summary>
		virtual public void FlipGravity()
		{
			character.Translate (0, bumpSpriteInY, true);
			gravityIsFlipped = !gravityIsFlipped;
			OnGravityFlipped ();
			if (flipCharacterImmediately) character.gameObject.transform.Rotate(0, 0, 180.0f);

		}

	}
	
}