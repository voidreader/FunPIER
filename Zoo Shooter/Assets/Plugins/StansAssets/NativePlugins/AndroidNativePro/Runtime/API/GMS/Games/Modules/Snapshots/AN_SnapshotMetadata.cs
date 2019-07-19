using UnityEngine;
using System;


namespace SA.Android.GMS.Games
{

    /// <summary>
    /// Data interface for the metadata of a saved game.
    /// </summary>
    [Serializable]
    public class AN_SnapshotMetadata 
    {

        /// <summary>
        /// Constant indicating that the played time of a snapshot is unknown.
        /// </summary>
        public const long PLAYED_TIME_UNKNOWN = -1;

        /// <summary>
        /// Constant indicating that the progress value of a snapshot is unknown.
        /// </summary>
        public const long PROGRESS_VALUE_UNKNOWN = -1;

        [SerializeField] int m_hashCode = 0;
        [SerializeField] string m_coverImageUri = null;
        [SerializeField] string m_description = null;
        [SerializeField] string m_deviceName = null;
        [SerializeField] string m_title = null;

        [SerializeField] long m_playedTime = 0;
        [SerializeField] long m_progressValue = 0;


        /// <summary>
        /// Retrieves an image URI that can be used to load the snapshot's cover image. 
        /// Returns null if the snapshot has no cover image.
        /// </summary>
        public string CoverImageUri {
            get {
                return m_coverImageUri;
            }
        }

        /// <summary>
        /// Retrieves the description of this snapshot.
        /// </summary>
        public string Description {
            get {
                return m_description;
            }
        }

        /// <summary>
        /// Retrieves the name of the device that wrote this snapshot, if known.
        /// </summary>
        public string DeviceName {
            get {
                return m_deviceName;
            }
        }


        /// <summary>
        /// Retrieves the title of this snapshot.
        /// </summary>
        public string Title {
            get {
                return m_title;
            }
        }


        /// <summary>
        /// Retrieves the played time of this snapshot in milliseconds. 
        /// This value is specified during the update operation. 
        /// If not known, returns <see cref="PLAYED_TIME_UNKNOWN"/>.
        /// </summary>
        public long PlayedTime {
            get {
                return m_playedTime;
            }
        }

        /// <summary>
        /// Retrieves the progress value for this snapshot. 
        /// Can be used to provide automatic conflict resolution <see cref="AN_SnapshotsClient.ResolutionPolicy.POLICY_HIGHEST_PROGRESS"/>. 
        /// If not known, returns <see cref="PROGRESS_VALUE_UNKNOWN"/>.
        /// </summary>
        public long ProgressValue {
            get {
                return m_progressValue;
            }
        }


        /// <summary>
        /// Java object hash code
        /// This field is used by a plugin for linking to Java native objects
        /// </summary>
        public int HashCode {
            get {
                return m_hashCode;
            }
        }
    }
}