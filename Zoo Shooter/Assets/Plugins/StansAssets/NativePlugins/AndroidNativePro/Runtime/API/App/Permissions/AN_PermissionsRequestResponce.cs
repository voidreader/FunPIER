using System;
using UnityEngine;

using SA.Android.Manifest;
using SA.Android.Content.Pm;


namespace SA.Android.App
{
    [Serializable]
    public class AN_PermissionsRequestResponce
    {

        [SerializeField] string m_permission;
        [SerializeField] int m_grantResult;

        public AN_PermissionsRequestResponce(AMM_ManifestPermission permission, AN_PackageManager.PermissionState state) {
            m_permission = permission.GetFullName();
            m_grantResult = (int)state;
        }

        /// <summary>
        /// The requested permission
        /// </summary>
        public AMM_ManifestPermission Permission {
            get {

                //Not the best way, very heavy
                //should be changed in future.
                foreach (AMM_ManifestPermission perm in (AMM_ManifestPermission[])Enum.GetValues(typeof(AMM_ManifestPermission))) {
                    if (perm.GetFullName().Equals(m_permission)) {
                        return perm;
                    }
                }

                return default(AMM_ManifestPermission);
            }
        }

        /// <summary>
        /// The requested permission grant result
        /// </summary>
        public AN_PackageManager.PermissionState GrantResult {
            get {
                return (AN_PackageManager.PermissionState) m_grantResult;
            }
        }
    }
}