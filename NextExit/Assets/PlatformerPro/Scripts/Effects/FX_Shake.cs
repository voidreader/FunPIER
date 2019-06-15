using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace PlatformerPro.Extras
{
	/// <summary>
	/// FX class which shakes something back and forth (usually a camera).
	/// </summary>
	public class FX_Shake : FX_Base
	{
		/// <summary>
		/// Target to shake.
		/// </summary>
		[Tooltip ("GameObject we will shake")]
		public GameObject shakeTarget;

		/// <summary>
		/// How far we shake at max in (x,y,z)
		/// </summary>
		[Tooltip ("How far we shake at max in (x,y,z)")]
		public Vector3 shakeAmount = new Vector3(0.5f, 0.5f, 0);

		/// <summary>
		/// How fast does the shake change direction.
		/// </summary>
		[Tooltip ("How fast does the shake change direction.")]
		[Range(0.015f, 0.5f)]
		public float shakeRate = 0.033f;

		/// <summary>
		/// How fast we decay 0 instantly, 1 not at all.
		/// </summary>
		[Tooltip ("How fast we decay 0 instantly, 1 not at all.")]
		[Range (0,1)]
		public float decay = 0.85f;

		[Header ("Snap to Original Positions")]
		/// <summary>
		/// Should we reset to original position in X.
		/// </summary>
		public bool returnToOriginalX;

		/// <summary>
		/// Should we reset to original position in Y.
		/// </summary>
		public bool returnToOriginalY;

		/// <summary>
		/// Should we reset to original position in Z.
		/// </summary>
		public bool returnToOriginalZ;

		/// <summary>
		/// The current velocity.
		/// </summary>
		protected Vector3 currentIntensityMax;

		/// <summary>
		/// The original position of the object.
		/// </summary>
		protected Vector3 originalPosition;

		/// <summary>
		/// The original position of the object.
		/// </summary>
		protected Vector3 lastShakePosition;

		/// <summary>
		/// have we started shaking?
		/// </summary>
		protected bool shakeStarted;

		/// <summary>
		/// Cached reference to zone camera, used to turn snapping on and off.
		/// </summary>
		protected PlatformCamera zoneCamera;

		protected float shakeTimer;

		protected Vector3 shakeTargetPosition;

		protected Vector3 lastShakeDirection;

		/// <summary>
		/// Do the effect.
		/// </summary>
		override protected void DoEffect()
		{
			if (shakeTarget == null) shakeTarget = gameObject;
			if (!shakeStarted) originalPosition = shakeTarget.transform.position;
			shakeStarted = true;
			currentIntensityMax = shakeAmount;
			zoneCamera = shakeTarget.GetComponent<PlatformCamera> ();
			if (zoneCamera != null) zoneCamera.enabled = false;
			shakeTimer = 0;
		}

		void LateUpdate() 
		{
			if (shakeStarted)
			{
				if (currentIntensityMax.sqrMagnitude < shakeAmount.sqrMagnitude * 0.033f)
				{
					shakeStarted = false;
					shakeTarget.transform.position = new Vector3 (
						returnToOriginalX ? originalPosition.x : shakeTarget.transform.position.x, 
						returnToOriginalY ? originalPosition.y : shakeTarget.transform.position.y, 
						returnToOriginalZ ? originalPosition.z : shakeTarget.transform.position.z);
					if (zoneCamera != null) zoneCamera.enabled = true;
					lastShakeDirection = Vector3.zero;
				}
				else
				{
					// Update timer
					shakeTimer -= TimeManager.FrameTime;

					// Update shake target
					if (shakeTimer <= 0)
					{

						Vector3 shake = new Vector3 (
							                Random.Range (0.5f, 1.0f) * currentIntensityMax.x,
							                Random.Range (0.5f, 1.0f) * currentIntensityMax.y,
							                Random.Range (0.5f, 1.0f) * currentIntensityMax.z);
						
						// Make shake go to opposite side
						if (lastShakeDirection.sqrMagnitude < 0.000001f)
						{
							lastShakePosition = new Vector3 (Random.Range (-1.0f, 1.0f), Random.Range (-1.0f, 1.0f), Random.Range (-1.0f, 1.0f));
						}
						if ((lastShakePosition.x > 0 && shake.x > 0) ||
						    (lastShakePosition.x < 0 && shake.x < 0))
						{
							shake.x = -shake.x;
						}
						if ((lastShakePosition.y > 0 && shake.y > 0) ||
							(lastShakePosition.y < 0 && shake.y < 0))
						{
							shake.y = -shake.y;	
						}
						if ((lastShakePosition.z > 0 && shake.z > 0) ||
						    (lastShakePosition.z < 0 && shake.z < 0))
						{
							shake.z = -shake.z;
						}

						lastShakeDirection = shake;
						shakeTargetPosition = originalPosition + shake;
						shakeTimer = shakeRate;

						// Update Shake intensity
						currentIntensityMax *= decay;

					}

					// Do shake
					shakeTarget.transform.position = Vector3.Lerp (originalPosition, shakeTargetPosition, (shakeRate - shakeTimer) / shakeRate);


					// Store last shake position
					lastShakePosition = shakeTarget.transform.position;
				}
			}
		}

	}
}