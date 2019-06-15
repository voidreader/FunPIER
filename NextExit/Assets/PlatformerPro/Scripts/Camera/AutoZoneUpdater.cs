using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformerPro
{
	/// <summary>
	/// Attach to a Character (or transform) to automatically update the camera zone when the character enters a new zone.
	/// </summary>
	public class AutoZoneUpdater: MonoBehaviour 
	{
		Character character;
		PlatformCamera platformCamera;

		float cantChangeTimer = 0;
		float cantChangeTime = 1.5f;

		void Start() 
		{
			// TODO Character Loader
			platformCamera = FindObjectOfType<PlatformCamera>();
			character = GetComponentInParent<Character> ();
			if (platformCamera == null)
			{
				Debug.LogWarning ("No camera found by AutoZoneUpdater, destroying");
				Destroy (this);
				return;
			}
			for (int i = 0; i < CameraZone.cameraZones.Count; i++)
			{
				if (CameraZone.cameraZones [i].IsInZone (transform))
				{
					platformCamera.CurrentZone = CameraZone.cameraZones [i];
					break;
				}
			}
		}

		// Update is called once per frame
		void Update () {
			// Can't update while transitioning
			if (platformCamera.CurrentZone == null) return;
			if (cantChangeTimer > 0)
			{
				cantChangeTimer -= TimeManager.FrameTime;
				return;
			}
				
			if (platformCamera.CurrentZone.IsInZone (transform)) return;

			for (int i = 0; i < CameraZone.cameraZones.Count; i++)
			{
				if (CameraZone.cameraZones [i].IsInZone (transform))
				{
					platformCamera.ChangeZone (CameraZone.cameraZones [i], character);
					cantChangeTimer = cantChangeTime;
					break;
				}
			}
		}
	}
}