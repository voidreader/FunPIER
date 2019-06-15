using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformerPro
{
	/// <summary>
	/// Sets a projectiles size based on its charge.
	/// </summary>
	public class SetProjectileSize : MonoBehaviour {

		public float ratio = 1.0f;

		public float minSize = 1.0f;

		public float maxSize = 1.0f;

		public bool forceToInt;

		Projectile p; 

		// Use this for initialization
		void Start () {
			p = GetComponentInParent<Projectile> ();
			transform.localScale = transform.localScale * GetSize ();

		}

		virtual protected float GetSize() 
		{
			float size = p.Charge * ratio;
			if (size < minSize) size = minSize;
			if (size > maxSize) size = maxSize;
			if (forceToInt) size = (float)(int)(size + 0.49f);
			return size;
		}
	}
}
