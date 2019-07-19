using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Android
{
	internal static class AN_RuntimeConfig  {
	
		private static AndroidJavaClass s_ImageWrapper; 
		private static bool s_IsInitialized = false;
		
		[RuntimeInitializeOnLoadMethod]
		private static void Init() {
			if (Application.platform != RuntimePlatform.Android) { return; }

			if (s_IsInitialized)
			{
				return;
			}

			s_IsInitialized = true;
			s_ImageWrapper = new AndroidJavaClass("com.stansassets.core.utility.AN_ImageWrapper");
			s_ImageWrapper.CallStatic("SetStorageOptions", AN_Settings.Instance.PreferredImagesStorage == AN_Settings.StorageType.Internal);
		}
	}
}

