using System;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Android.Content.Pm
{

    /// <summary>
    /// Information that is returned from resolving an intent against an IntentFilter. 
    /// This partially corresponds to information collected from the AndroidManifest.xml's <intent> tags.
    /// </summary>
    [Serializable]
    public class AN_ResolveInfo
    {
        [SerializeField] AM_ActivityInfo m_activityInfo = null;


        /// <summary>
        /// The activity or broadcast receiver that corresponds to this resolution match, 
        /// if this resolution is for an activity or broadcast receiver. 
        /// Exactly one of <see cref="ActivityInfo"/> , <see cref="ServiceInfo"/>, or <see cref="ProviderInfo"/>  will be non-null.
        /// </summary>
        public AM_ActivityInfo ActivityInfo {
            get {
                return m_activityInfo;
            }
        }

        /// <summary>
        /// The service that corresponds to this resolution match, if this resolution is for a service. 
        /// Exactly one of <see cref="ActivityInfo"/> , <see cref="ServiceInfo"/>, or <see cref="ProviderInfo"/>  will be non-null.
        /// </summary>
        public AN_ServiceInfo ServiceInfo {
            get {
                return null;
            }
        }


        /// <summary>
        /// The provider that corresponds to this resolution match, if this resolution is for a provider. 
        /// Exactly one of <see cref="ActivityInfo"/> , <see cref="ServiceInfo"/>, or <see cref="ProviderInfo"/>  will be non-null.
        /// </summary>
        public AN_ProviderInfo ProviderInfo {
            get {
                return null;
            }
        }

        
    }
}
