using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformerPro
{
	/// <summary>
	/// Rotate an object towards the velocity direction of its rigidbody (or aprents rigidbody).
	/// </summary>
	public class RotateToVelocityDirection : MonoBehaviour {
		
		/// <summary>
		/// Cached rigidbody
		/// </summary>
		Rigidbody2D myRigidybody;

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start () {
			myRigidybody = GetComponentInParent<Rigidbody2D> ();
			if (myRigidybody == null)
			{
				Debug.LogWarning ("Rotate To Velocity Direction requires a Rigidbody2D on self or parent");
				Destroy (this);
			}
		}

		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update () {
			Vector2 v = myRigidybody.velocity.normalized;
			float a = Mathf.Atan2 (v.y, v.x) * Mathf.Rad2Deg; 
			transform.localRotation = Quaternion.Euler (0, 0, a);
		}
	}
}