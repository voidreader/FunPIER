using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.GMS.Common;


namespace SA.Android.GMS.Drive
{

    /// <summary>
    /// The Drive API provides easy access to users' Google Drive contents. 
    /// This API includes Activities to open or create files in users' Drives, 
    /// as well as the ability to programmatically interact with contents, metadata, and the folder hierarchy.
    /// https://developers.google.com/android/reference/com/google/android/gms/drive/Drive
    /// </summary>
    public class AN_Drive
    {

        /// <summary>
        /// A Scope that gives 'drive.file' access to a user's drive.
        /// </summary>
        public static AN_Scope SCOPE_FILE = new AN_Scope("https://www.googleapis.com/auth/drive.file");

        /// <summary>
        /// A Scope that gives 'drive.appfolder' access to a user's drive.
        /// </summary>
        public static AN_Scope SCOPE_APPFOLDER = new AN_Scope("https://www.googleapis.com/auth/drive.appdata");

    }
}