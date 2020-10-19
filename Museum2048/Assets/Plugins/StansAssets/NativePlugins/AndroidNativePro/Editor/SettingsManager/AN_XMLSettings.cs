using UnityEngine;
using System;
using SA.Foundation.Utility;

namespace SA.Android.Editor
{
    [Serializable]
    class AN_XMLSettings
    {
        public string GamesIds => m_GamesIds;

        [SerializeField]
        string m_GamesIds;

        public AN_XMLSettings()
        {
            m_GamesIds = SA_FilesUtil.Read(AN_Settings.ANDROID_GAMES_IDS_FILE_PATH);
        }
    }
}
