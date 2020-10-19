using UnityEditor;

namespace SA.Android.Editor
{
    [InitializeOnLoad]
    static class AN_Bootstrapper
    {
        static AN_Bootstrapper()
        {
            EditorApplication.delayCall += () =>
            {
                if (AN_Settings.Instance.EnforceEdm4UDependency)
                    AN_Packages.InstallEdm4U();
            };
        }
    }
}
