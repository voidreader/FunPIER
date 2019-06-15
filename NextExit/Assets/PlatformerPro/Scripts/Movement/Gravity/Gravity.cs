using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Applies the default gravity to the character.
	/// </summary>
	public class Gravity : MonoBehaviour
	{

		#region members

		/// <summary>
		/// If true use the gravity value form the Physics 2D Project settings.
		/// </summary>
		public bool usePhysics2dGravity;

		/// <summary>
		/// The value of gravity for manual gravity type.
		/// </summary>
		public float gravity = -20.0f;

		/// <summary>
		/// Cached reference to the character.
		/// </summary>
		protected Character character;

		#endregion

		#region properties

		/// <summary>
		/// Gets the current value of gravity.
		/// </summary>
		virtual public float Value
		{
			get
			{
				return (usePhysics2dGravity ? Physics2D.gravity.y : gravity);
			}
		}

		/// <summary>
		/// Gets a value indicating whether thegravity flipped.
		/// </summary>
		virtual public bool IsGravityFlipped
		{
			get
			{
				return false;
			}
		}

		#endregion

		virtual public void Init(Character character)
		{
			this.character = character;
		}

		virtual public void ApplyGravity()
		{
			// Apply acceleration
			character.AddVelocity(0, TimeManager.FrameTime * (usePhysics2dGravity ? Physics2D.gravity.y : gravity), false);
			// Limit to terminal velocity
			if (character.Velocity.y < character.terminalVelocity) character.SetVelocityY(character.terminalVelocity);
			// Translate - note that y is applied relatively which means the character is always pushed towards the platform they are on with constant force
			// not physially accurate but it feels right when playing. You can always override with your own gravity class if desired :)
			character.Translate(0, character.Velocity.y * TimeManager.FrameTime, false);
		}

	}

}