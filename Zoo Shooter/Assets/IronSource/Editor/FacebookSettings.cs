#if UNITY_IPHONE 
using UnityEngine;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using System.IO;

namespace IronSource.Editor
{
	public class FacebookSettings : IAdapterSettings
	{
		public void updateProject (BuildTarget buildTarget, string projectPath)
		{
			Debug.Log ("IronSource - Update project for Facebook");

			PBXProject project = new PBXProject ();
			project.ReadFromString (File.ReadAllText (projectPath));

			string targetId = project.TargetGuidByName (PBXProject.GetUnityTargetName ());

			// Required System Frameworks
			project.AddFrameworkToProject (targetId, "AudioToolbox.framework", false);
			project.AddFrameworkToProject (targetId, "StoreKit.framework", false);
			project.AddFrameworkToProject (targetId, "CoreGraphics.framework", false);
			project.AddFrameworkToProject (targetId, "UIKit.framework", false);
			project.AddFrameworkToProject (targetId, "Foundation.framework", false);
			project.AddFrameworkToProject (targetId, "Security.framework", false);
			project.AddFrameworkToProject (targetId, "CoreImage.framework", false);
			project.AddFrameworkToProject (targetId, "AVFoundation.framework", false);
			project.AddFrameworkToProject (targetId, "CoreMedia.framework", false);
			
			project.AddFrameworkToProject (targetId, "AdSupport.framework", true);
			project.AddFrameworkToProject (targetId, "CFNetwork.framework", true);
			project.AddFrameworkToProject (targetId, "CoreMotion.framework", true);
			project.AddFrameworkToProject (targetId, "CoreTelephony.framework", true);
			project.AddFrameworkToProject (targetId, "LocalAuthentication.framework", true);
			project.AddFrameworkToProject (targetId, "SafariServices.framework", true);
			project.AddFrameworkToProject (targetId, "SystemConfiguration.framework", true);
			project.AddFrameworkToProject (targetId, "VideoToolbox.framework", true);
			project.AddFrameworkToProject (targetId, "WebKit.framework", true);

			project.AddFileToBuild (targetId, project.AddFile ("usr/lib/libc++.tbd", "Frameworks/libc++.tbd", PBXSourceTree.Sdk));
            project.AddFileToBuild (targetId, project.AddFile ("usr/lib/libxml2.tbd", "Frameworks/libxml2.tbd", PBXSourceTree.Sdk));


			File.WriteAllText (projectPath, project.WriteToString ());
		}

		public void updateProjectPlist (BuildTarget buildTarget, string plistPath)
		{
			Debug.Log ("IronSource - Update plist for Facebook");
		}
	}
}
#endif
