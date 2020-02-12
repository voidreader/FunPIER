using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA.Android.GMS.Games
{

    /// <summary>
    /// Direction constants for pagination over data sets.
    /// </summary>
    public enum AN_PageDirection 
    {

        /// <summary>
        /// Constant indicating that no pagination is occurring.
        /// </summary>
        None = -1,

        /// <summary>
        /// Direction advancing toward the end of the data set.
        /// </summary>
        Next = 0,

        /// <summary>
        /// Direction advancing toward the beginning of the data set.
        /// </summary>
        Prev = 1
       
    }
}