using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlatformerPro {
	
	public class UpAndDownPlatformWithReturn : UpAndDownPlatform
	{
		public bool stopAtTop;

		/// <summary>
		/// Init this instance.
		/// </summary>
		override protected void PostInit()
		{
			base.PostInit();
			automaticActivation = PlatformActivationType.ACTIVATE_ON_STAND;
			automaticDeactivation = PlatformDeactivationType.DEACTIVATE_ON_LEAVE;
			Activated = false;
		}

		void Update() {
			if (Activated) {
				if (stopAtTop && speed < 0) speed *= -1;
				DoMove ();
			} else if (transform.position.y > bottomExtent) {
				if (speed > 0) speed *= -1;
				float distance = speed * TimeManager.FrameTime;
				myTransform.Translate(0, distance, 0);
				if (myTransform.position.y < bottomExtent)
				{
					float difference = distance - (myTransform.position.y - bottomExtent);
					myTransform.position = new Vector3(myTransform.position.x, bottomExtent - difference, myTransform.position.z);
				}
			}
		}
	}
}