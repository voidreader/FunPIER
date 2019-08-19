using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Manifest;

namespace SA.Android
{
    public class AN_SocialResolver : AN_APIResolver
    {
        public override bool IsSettingsEnabled {
            get { return AN_Settings.Instance.Social; }
            set { AN_Settings.Instance.Social = value; }
        }

        public override void AppendBuildRequirements(AN_AndroidBuildRequirements buildRequirements) {
            
            if (AN_Settings.Instance.PreferredImagesStorage != AN_Settings.StorageType.ForceInternal)
            {
                buildRequirements.AddPermission(AMM_ManifestPermission.WRITE_EXTERNAL_STORAGE);
                buildRequirements.AddPermission(AMM_ManifestPermission.READ_EXTERNAL_STORAGE);
            }
            
            buildRequirements.AddBinaryDependency(AN_BinaryDependency.AndroidX);
        }
    }
}