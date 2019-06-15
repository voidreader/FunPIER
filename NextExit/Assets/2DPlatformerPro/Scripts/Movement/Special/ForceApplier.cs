using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Force applier applies a force based on the velocity of the character. Make sure the layer of this object isn't one the 
	/// Character can collide with, else the character will collide with itself.
	/// </summary>
	[RequireComponent (typeof(Collider2D), typeof(Rigidbody2D))]
	public class ForceApplier : MonoBehaviour
	{
		/// <summary>
		/// How the force will be applied.
		/// </summary>
		[Tooltip ("How the force will be applied.")]
		public ForceMode2D forceMode = ForceMode2D.Impulse;

		/// <summary>
		/// Force modifier - Applied force will be multiplied by this.
		/// </summary>
		[Tooltip ("Applied force will be multiplied by this.")]
		public Vector2 forceModifier = Vector2.one;

		/// <summary>
		/// If true don't apply downwards force on an object.
		/// </summary>
		[Tooltip (" If true don't apply downwards force on an object.")]
		public bool dontApplyDownForce = true;

		/// <summary>
		/// Character reference.
		/// </summary>
		protected Character character;

		/// <summary>
		/// Unity Start Hook.
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
			if (!GetComponent<Rigidbody2D>().isKinematic) Debug.LogWarning("Expected rigidbody2D to be kinematic");
			character = GetComponentInParent<Character> ();
			if (character == null) Debug.LogWarning ("Expected to be a child of a Character object");
		}

		/// <summary>
		/// Unity on collision hook.
		/// </summary>
		/// <param name="hit">Hit.</param>
		void OnCollisionEnter2D(Collision2D hit)
		{
			if (hit.rigidbody != null)
			{
				float yModifier = forceModifier.y;
				if (dontApplyDownForce && character.Velocity.y < 0) yModifier = 0;
				hit.rigidbody.AddForceAtPosition(new Vector2(character.Velocity.x * forceModifier.x, character.Velocity.y * yModifier), hit.contacts[0].point, forceMode);
			}
		}
	}
}