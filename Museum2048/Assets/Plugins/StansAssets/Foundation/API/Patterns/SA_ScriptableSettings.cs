using UnityEngine;
using SA.Foundation.Config;

namespace SA.Foundation.Patterns
{
    public abstract class SA_ScriptableSettings : ScriptableObject
    {
        const int k_ReleaseYear = 2020;
        public string LastVersionCode = string.Empty;
        protected abstract string BasePath { get; }

        PluginVersionHandler s_pluginVersion;

        public PluginVersionHandler GetPluginVersion()
        {
            if (s_pluginVersion == null) s_pluginVersion = new PluginVersionHandler(BasePath);
            return s_pluginVersion;
        }

        public string GetFormattedVersion()
        {
            return string.Format("{0}.{1}", k_ReleaseYear, GetPluginVersion().GetVersion());
        }

        public abstract string PluginName { get; }
        public abstract string DocumentationURL { get; }
        public abstract string SettingsUIMenuItem { get; }
    }
}
