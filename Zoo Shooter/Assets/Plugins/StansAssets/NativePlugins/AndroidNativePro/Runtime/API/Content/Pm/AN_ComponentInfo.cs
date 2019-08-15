using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA.Android.Content.Pm
{
    /// <summary>
    /// Base class containing information common to all application components 
    /// <see cref="AM_ActivityInfo"/> , <see cref="AN_ServiceInfo"/>. 
    /// This class is not intended to be used by itself; 
    /// it is simply here to share common definitions between all application components. 
    /// As such, it does not itself implement Parcelable, 
    /// but does provide convenience methods to assist in the implementation of Parcelable in subclasses.
    /// </summary>
    [Serializable]
    public class AN_ComponentInfo : AN_PackageItemInfo
    {

    }
}
