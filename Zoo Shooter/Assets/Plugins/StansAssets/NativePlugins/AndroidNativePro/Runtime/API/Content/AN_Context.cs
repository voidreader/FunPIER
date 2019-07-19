using System;
using System.Collections.Generic;
using UnityEngine;


using SA.Android.Content.Pm;

namespace SA.Android.Content
{
    /// <summary>
    /// Interface to global information about an application environment. 
    /// This is an abstract class whose implementation is provided by the Android system. 
    /// It allows access to application-specific resources and classes, as well as up-calls 
    /// for application-level operations such as launching activities, broadcasting and receiving intents, etc.
    /// </summary>
    [Serializable]
    public abstract class AN_Context
    {

        /// <summary>
        /// Return PackageManager instance to find global package information.
        /// </summary>
        public abstract AN_PackageManager GetPackageManager();

    }
}
