using System;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Android.Content.Pm
{

    /// <summary>
    /// Overall information about the contents of a package. 
    /// This corresponds to all of the information collected from AndroidManifest.xml.
    /// </summary>
    [Serializable]
    public class AN_PackageInfo 
    {
#pragma warning disable 649
        [SerializeField] string m_versionName;
        [SerializeField] string m_packageName;
        [SerializeField] string m_sharedUserId;
#pragma warning restore 649


        public AN_PackageInfo(string packageName, string versionName) {
            m_packageName = packageName;
            m_versionName = versionName;
        }

        /// <summary>
        /// The shared user ID name of this package, as specified by the <manifest> tag's sharedUserId attribute.
        /// </summary>
        public string VersionName {
            get {
                return m_versionName;
            }
        }

        /// <summary>
        /// The name of this package.
        /// </summary>
        public string PackageName {
            get {
                return m_packageName;
            }
        }


        /// <summary>
        /// The shared user ID name of this package, as specified by the <manifest> tag's sharedUserId attribute.
        /// </summary>
        public string SharedUserId {
            get {
                return m_sharedUserId;
            }
        }
    }
}
