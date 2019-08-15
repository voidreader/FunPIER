using UnityEditor;
using SA.Android;
using SA.Foundation.Editor;
using SA.Foundation.Utility;
using SA.Foundation.UtilitiesEditor;

namespace SA.Android
{
    [InitializeOnLoad]
    public class AN_InstallationProcessing: SA_PluginInstallationProcessor<AN_Settings>
    {
        static AN_InstallationProcessing() 
        {
            var installation = new AN_InstallationProcessing();
            installation.Init();
        }


        //--------------------------------------
        //  AN_PluginInstallationProcessor
        //--------------------------------------

        protected override void OnInstall() 
        {
            // Let's check if we have FB SKD and Jar Resolver in the project.
            AN_ResolveManager.ProcessAssets();
            AN_FirebaseDefinesResolver.ProcessAssets();
        }

    }
}
