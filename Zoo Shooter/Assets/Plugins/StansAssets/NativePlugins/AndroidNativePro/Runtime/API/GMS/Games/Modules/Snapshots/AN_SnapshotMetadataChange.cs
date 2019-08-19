using UnityEngine;
using System;

namespace SA.Android.GMS.Games
{
    /// <summary>
    /// A collection of changes to apply to the metadata of a snapshot. 
    /// Fields that are not set will retain their current values.
    /// </summary>
    [Serializable]
    public class AN_SnapshotMetadataChange
    {
#pragma warning disable 414

        [SerializeField] long m_playedTimeMillis = 0;
        [SerializeField] long m_progressValue = 0;

        [SerializeField] string m_describtion;
        [SerializeField] string m_coverImageBase64;

#pragma warning restore 414

        /// <summary>
        /// Sentinel object to use to commit a change without modifying the metadata.
        /// </summary>
        public static AN_SnapshotMetadataChange EMPTY_CHANGE = new Builder().Build();

        public class Builder
        {
            private AN_SnapshotMetadataChange m_meta;

            public Builder() {
                m_meta = new AN_SnapshotMetadataChange();
            }

            public AN_SnapshotMetadataChange Build() {
                return m_meta;
            }

            /// <summary>
            /// Cover image to set for the snapshot.
            /// <param name="coverImage"></param>
            /// </summary>
            public void SetCoverImage(Texture2D coverImage) {
                m_meta.m_coverImageBase64 = coverImage.ToBase64String();
            }

            /// <summary>
            /// Description to set for the snapshot.
            /// <param name="description">description</param>
            /// </summary>
            public void SetDescription(string description) {
                m_meta.m_describtion = description;
            }

            /// <summary>
            /// The new played time to set for the snapshot.
            /// Value should always be above zero.
            /// <param name="playedTimeMillis">player played time in milliseconds</param>
            /// </summary>
            public void SetPlayedTimeMillis(long playedTimeMillis) {

                if(playedTimeMillis <=0) {
                    Debug.LogError("AN_SnapshotMetadataChange::SetPlayedTimeMillis was set to " + playedTimeMillis + " " +
                        "it may lead to negative consequences, make sure this value is always > 0");
                }

                m_meta.m_playedTimeMillis = playedTimeMillis;
            }

            /// <summary>
            /// The new progress value to set for the snapshot.
            /// Value should always be above zero.
            /// </summary>
            /// <param name="progressValue">player progress value</param>
            public void SetProgressValue(long progressValue) {

                if (progressValue <= 0) {
                    Debug.LogError("AN_SnapshotMetadataChange::SetProgressValue was set to " + progressValue + " " +
                        "it may lead to negative consequences, make sure this value is always > 0");
                }

                m_meta.m_progressValue = progressValue;
            }
        }
    }
}