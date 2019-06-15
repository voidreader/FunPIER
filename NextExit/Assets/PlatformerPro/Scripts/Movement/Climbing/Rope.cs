using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PlatformerPro
{

	/// <summary>
	/// Attached to the top level of a rope object.
	/// </summary>
	public class Rope : MonoBehaviour
	{

#if UNITY_EDITOR
		/// <summary>
		/// The number of segments. Used in editor only.
		/// </summary>
		public int segments;

		/// <summary>
		/// The length of the rope. Used in editor only.
		/// </summary>
		public float length;

		/// <summary>
		/// The prefab to use for each rope section (i.e. put the mesh or sprite here). Used in editor only.
		/// </summary>
		public GameObject ropeSectionPrefab;

		/// <summary>
		/// If true only the bottom section of the rope will be grabable and will have a fixed position. Used in editor only.
		/// </summary>
		public bool usedFixedPosition;

		/// <summary>
		/// The mass of each rope segment (except the last which has mass = ropeMass * 5).
		/// </summary>
		public float ropeMass = 1.0f;

		/// <summary>
		/// Min rope angle.
		/// </summary>
		public float angleLimits = 0;

		/// <summary>
		/// Angluar drag per section.
		/// </summary>
		public float angularDrag = 1.0f;

#endif

		/// <summary>
		/// Ordered list of the sections of rope taht make up this rope. 
		/// </summary>
		protected List<RopeSection> sections;


		void Start()
		{
			Init ();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void Init()
		{
			RopeSection[] allSections = GetComponentsInChildren<RopeSection> ();
			sections = allSections.OrderByDescending (s => s.transform.position.y).ToList ();
			if (sections.Count == 0) Debug.LogWarning ("Rope has no rope sections");
		}

		/// <summary>
		/// Gets the rope section above the provided one or null if none.
		/// </summary>
		/// <returns>The section below.</returns>
		virtual public RopeSection GetSectionAbove(RopeSection section)
		{
			int index = sections.IndexOf (section);
			if (index > 0) return sections[index - 1];
			return null;
		}

		/// <summary>
		/// Gets the rope section below the provided one or null if none.
		/// </summary>
		/// <returns>The section below.</returns>
		virtual public RopeSection GetSectionBelow(RopeSection section)
		{
			int index = sections.IndexOf (section);
			if (index < sections.Count - 1) return sections[index + 1];
			return null;
		}
	}

}