using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA.Android.Content.Pm
{
    /// <summary>
    /// Base class containing information common to all package items held by the package manager. 
    /// This provides a very common basic set of attributes: a label, icon, and meta-data. 
    /// This class is not intended to be used by itself; 
    /// it is simply here to share common definitions between all items returned by the package manager. 
    /// As such, it does not itself implement Parcelable, 
    /// but does provide convenience methods to assist in the implementation of Parcelable in subclasses.
    /// </summary>
    [Serializable]
    public class AN_PackageItemInfo 
    {
        [SerializeField] string m_name  = null;
        [SerializeField] string m_packageName = null; 


        /// <summary>
        /// Public name of this item.
        /// </summary>
        public string Name {
            get {
                return m_name;
            }
        }

        /// <summary>
        /// Name of the package that this item is in.
        /// </summary>
        public string PackageName {
            get {
                return m_packageName;
            }
        }
    }
}