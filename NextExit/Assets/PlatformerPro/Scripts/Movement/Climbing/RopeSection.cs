using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// Section of a rope.
	/// </summary>
	public class RopeSection : MonoBehaviour
	{

		/// <summary>
		/// If true, the rope will not be climable but instead be a fixed point to which the character is attached.
		/// </summary>
		[Tooltip ("If true, the rope will not be climable but instead be a fixed point to which the character is attached.")]
		public bool usedFixedPosition;

		/// <summary>
		///  The parent rope.
		/// </summary>
		protected Rope rope;

		/// <summary>
		/// The box collider for this rope section.
		/// </summary>
		protected BoxCollider2D myCollider;

		void Start()
		{
			Init ();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void Init()
		{
			rope = GetComponentInParent<Rope> ();
			if (rope == null) Debug.LogWarning ("RopeSection couldn't find a Rope in its parents.");
			myCollider = GetComponent<BoxCollider2D> ();
			if (myCollider == null) Debug.LogWarning("RopeSection must be on the same GameObject as a BoxCollider2D");
		}

		/// <summary>
		/// Gets the rope that owns this rope section.
		/// </summary>
		/// <value>The rope.</value>
		virtual public Rope Rope
		{
			get { return rope; }
		}

		/// <summary>
		/// Get the rope section length.
		/// </summary>
		/// <value>The length.</value>
		virtual public float Length
		{
			get
			{
				Vector2[] extents = myCollider.GetYExtentsInWorldSpace ();
				return Vector2.Distance(extents[0], extents[1]);
			}
		}

		/// <summary>
		/// Gets the rope section above this one.
		/// </summary>
		virtual public RopeSection SectionAbove
		{
			get { return rope.GetSectionAbove(this); }
		}

		/// <summary>
		/// Gets the rope section below this one.
		/// </summary>
		virtual public RopeSection SectionBelow
		{
			get { return rope.GetSectionBelow(this); }
		}

		/// <summary>
		/// Gets the position in world space the character should be based on how far up the rope section they are.
		/// </summary>
		/// <returns>The center for position.</returns>
		/// <param name="characterPosition">Characters grab position.</param>
		virtual public Vector2 GetCharacterPositionForPosition(float position)
		{
			Vector2[] extents = myCollider.GetYExtentsInWorldSpace ();
			return Vector2.Lerp(extents[0], extents[1], position);
		}

		/// <summary>
		/// Gets the position in world space the character should be based on how far up the rope section they are.
		/// </summary>
		/// <returns>The center for position.</returns>
		/// <param name="characterPosition">Characters grab position.</param>
		/// <param name="characterPosition">Offset from characters center and their grab (usually hand) position.</param>
		virtual public Vector2 GetCharacterPositionForPosition(float position, float handOffset)
		{
			float newPosition = position + (handOffset / Length);
			// On current section
			if (Mathf.Abs(newPosition) <= 1.0f)
			{
				return GetCharacterPositionForPosition(newPosition);
			}
			// Above current section
			else if (newPosition >= 1.0f)
			{
				RopeSection altSection = this;
				while (newPosition > 1.0f)
				{
					if (altSection.SectionAbove == null) 
					{
						return altSection.GetCharacterPositionForPosition(1.0f);
					}
					else
					{
						altSection = altSection.SectionAbove;
					}
					newPosition -= 1.0f;
				}
				return altSection.GetCharacterPositionForPosition(newPosition);

			}
			// Below current section
			else
			{
				RopeSection altSection = this;
				while (newPosition < -1.0f)
				{
					if (altSection.SectionBelow == null) 
					{
						return altSection.GetCharacterPositionForPosition(-1.0f);
					}
					else
					{
						altSection = altSection.SectionBelow;
					}
					newPosition += 1.0f;
				}
				return altSection.GetCharacterPositionForPosition(newPosition);
			}
		}

		/// <summary>
		/// Gets the rotation (in eueler z degrees) for the given rope position.
		/// </summary>
		/// <returns>The center for position.</returns>
		/// <param name="characterPosition">Characters grab position.</param>
		/// <param name="characterPosition">Offset from characters center and their grab (usually hand) position.</param>
		virtual public float GetCharacterRotationForPosition(float position, float handOffset)
		{
			float newPosition = position + (handOffset / Length);
			float result;
			// On current section
			if (Mathf.Abs(newPosition) <= 1.0f)
			{
				result = transform.localRotation.eulerAngles.z;
			}
			// Above current section
			else if (newPosition >= 1.0f)
			{
				RopeSection altSection = this;
				while (newPosition > 1.0f)
				{
					if (altSection.SectionAbove == null) 
					{
						newPosition = 0;
					}
					else
					{
						altSection = altSection.SectionAbove;
					}
					newPosition -= 1.0f;
				}
				result = altSection.transform.localRotation.eulerAngles.z;
				
			}
			// Above current section
			else
			{
				RopeSection altSection = this;
				while (newPosition < -1.0f)
				{
					if (altSection.SectionBelow == null) 
					{
						newPosition = 0;
					}
					else
					{
						altSection = altSection.SectionBelow;
					}
					newPosition += 1.0f;
				}
				result = altSection.transform.localRotation.eulerAngles.z;
			}
			if (result > 180.0f) result -= 360.0f;
			if (result < -180.0f) result += 360.0f;
			return result;
		}

		/// <summary>
		/// Gets how far up the rope section (0 at bottom, 1 at top) a character is.
		/// </summary>
		/// <returns>The center for position.</returns>
		/// <param name="characterPosition">Characters grab position.</param>
		virtual public float GetPositionForCharacterPosition(Vector2 position)
		{
			if (usedFixedPosition) return 0.5f;
			Vector2[] extents = myCollider.GetYExtentsInWorldSpace ();
			return Mathf.InverseLerp (extents[0].y, extents[1].y, position.y);
		}

	}
}