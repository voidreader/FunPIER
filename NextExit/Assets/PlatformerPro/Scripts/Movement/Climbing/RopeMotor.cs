using UnityEngine;
using System.Collections;

namespace PlatformerPro
{

	/// <summary>
	/// Makes a rope swing automatically. Intended for ropes with 1 section and a fixed grab position 
	/// for games that want ropes like 'Pitfall'. Note the ridigbody will be made kinematic and no other physics 
	/// will affect it.
	/// </summary>
	[RequireComponent (typeof(Rigidbody2D))]
	public class RopeMotor : MonoBehaviour
	{

		/// <summary>
		/// Rotation speed in degrees per second.
		/// </summary>
		[Tooltip ("Rotation speed in degrees per second.")]
		public float speed = 33.0f;

		/// <summary>
		/// If true we move at a constant speed, false we slow down at the apex.
		/// </summary>
		[Tooltip ("A value between 0 for no slow down at apex and 1 for lots of slow down at apex")]
		[Range(0,1)]
		public float slowDownModifier;

		/// <summary>
		/// Cached rigidbody.
		/// </summary>
		protected Rigidbody2D myRigidbody;

		/// <summary>
		/// Cached max angle.
		/// </summary>
		protected float max;

		/// <summary>
		/// Cached min angle.
		/// </summary>
		protected float min;

		/// <summary>
		/// Unity Start hook.
		/// </summary>
		void Start()
		{
			Init ();
		}

		/// <summary>
		/// Unity Update hook.
		/// </summary>
		void Update()
		{
			UpdateAngle ();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		virtual protected void Init()
		{
			myRigidbody = GetComponent<Rigidbody2D> ();
			myRigidbody.isKinematic = true;
			// We dont really need this hinge joint anymore, but we keep it for consistency with RopeInspector/other ropes.
			max = GetComponent<HingeJoint2D>().limits.max;
			min = GetComponent<HingeJoint2D>().limits.min;
		}

		/// <summary>
		/// Do the work.
		/// </summary>
		virtual protected void UpdateAngle()
		{
			float deg;
			float actualSpeed = speed;
			if (slowDownModifier > 0)
			{
				deg = transform.eulerAngles.z;
				if (deg > 180) deg -= 360;
				if (deg < -180) deg += 360;
				float modifier = (90.0f / max) * slowDownModifier;
				actualSpeed = speed * Mathf.Cos(deg * modifier * Mathf.Deg2Rad);
			}
			transform.RotateAround(GetComponent<HingeJoint2D>().connectedBody.transform.position, new Vector3(0,0,-1), TimeManager.FrameTime * actualSpeed);
			deg = transform.eulerAngles.z;
			if (deg > 180) deg -= 360;
			if (deg < -180) deg += 360;
			if (deg >= min || deg <= max) speed *= -1;
			// TODO Make sure ropes dont freeze
			// TODO calculate velocity
			// myRigidbody.velocity = new Vector2 ();
		}
	}

}
