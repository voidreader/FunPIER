using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Manifest;

namespace SA.Android
{
    public class AN_ContactsResolver : AN_APIResolver
    {
        public override bool IsSettingsEnabled {
            get { return AN_Settings.Instance.Contacts; }
            set { AN_Settings.Instance.Contacts = value; }
        }


        public override void AppendBuildRequirements(AN_AndroidBuildRequirements buildRequirements) {
            buildRequirements.AddPermission(AMM_ManifestPermission.READ_CONTACTS);
        }


    }
}