using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Top of a rope.
	/// </summary>
	public class RopeAnchor : RopeSection
	{

		void Start()
		{
			Init ();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		override protected void Init()
		{
			rope = GetComponentInParent<Rope> ();
			if (rope == null) Debug.LogWarning ("RopeAnchor couldn't find a Rope in its parents.");
			myCollider = GetComponent<BoxCollider2D> ();
		}

		/// <summary>
		/// Get the rope section length.
		/// </summary>
		/// <value>The length.</value>
		override public float Length
		{
			get
			{
				if (myCollider != null) return base.Length;
				return 0;
			}
		}


		/// <summary>
		/// Gets the position in world space the character should be based on how far up the rope section they are.
		/// </summary>
		/// <returns>The center for position.</returns>
		/// <param name="characterPosition">Characters grab position.</param>
		override public Vector2 GetCharacterPositionForPosition(float position)
		{
			if (myCollider != null) return base.GetCharacterPositionForPosition (position);
			return transform.position;
		}

		/// <summary>
		/// Gets the position in world space the character should be based on how far up the rope section they are.
		/// </summary>
		/// <returns>The center for position.</returns>
		/// <param name="characterPosition">Characters grab position.</param>
		/// <param name="characterPosition">Offset from characters center and their grab (usually hand) position.</param>
		override public Vector2 GetCharacterPositionForPosition(float position, float handOffset)
		{
			if (myCollider != null) return base.GetCharacterPositionForPosition (position, handOffset);
			return transform.position;
		}

		/// <summary>
		/// Gets the rotation (in eueler z degrees) for the given rope position.
		/// </summary>
		/// <returns>The center for position.</returns>
		/// <param name="characterPosition">Characters grab position.</param>
		/// <param name="characterPosition">Offset from characters center and their grab (usually hand) position.</param>
		override public float GetCharacterRotationForPosition(float position, float handOffset)
		{
			if (myCollider != null) return base.GetCharacterRotationForPosition (position, handOffset);
			return 0f;
		}

		/// <summary>
		/// Gets how far up the rope section (0 at bottom, 1 at top) a character is.
		/// </summary>
		/// <returns>The center for position.</returns>
		/// <param name="characterPosition">Characters grab position.</param>
		override public float GetPositionForCharacterPosition(Vector2 position)
		{
			if (myCollider != null) return base.GetPositionForCharacterPosition (position);
			return 0f;
		}

	}
}