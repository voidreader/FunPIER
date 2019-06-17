using System;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor;

#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif


using SA.Android.Manifest;
using SA.Foundation.Editor;
using SA.Foundation.UtilitiesEditor;

namespace SA.Android
{

#if UNITY_2018_1_OR_NEWER
    public class AN_Preprocessor : IPreprocessBuildWithReport
#else
    public class AN_Preprocessor : IPreprocessBuild
#endif
    {
        private static List<AN_APIResolver> s_resolvers = null;

        //--------------------------------------
        // Static
        //--------------------------------------

        public static void Resolve() {
            
            bool plgingVersionUpdated = AN_Settings.UpdateVersion(AN_Settings.FormattedVersion) && !SA_PluginTools.IsDevelopmentMode;
            Refresh();
            var requirements = new AN_AndroidBuildRequirements();
            if (plgingVersionUpdated) {
                SA_AssetDatabase.DeleteAsset(AN_Settings.ANDROID_INTERNAL_FOLDER);
                SA_AssetDatabase.DeleteAsset(AN_Settings.ANDROID_MAVEN_FOLDER);
            }

            foreach(var resolver in Resolvers) {
                resolver.Run(requirements);
            }
            Resolve(requirements);
        }


        public static void ActicateJarResolver() {
            if (!AN_Settings.Instance.UseUnityJarResolver) {
                AN_Settings.Instance.UseUnityJarResolver = true;
                Resolve();
            }
        }


        public static void Refresh() {
            s_resolvers = null;
        }


        public static T GetResolver<T>() where T : AN_APIResolver {
            return (T)GetResolver(typeof(T));
        }


        public static AN_APIResolver GetResolver(Type resolverType) {
            foreach (var resolver in Resolvers) {
                if (resolver.GetType().Equals(resolverType)) {
                    return resolver;
                }
            }

            return null;
        }


        public static List<AN_APIResolver> Resolvers {
            get {
                if(s_resolvers == null) {
                    s_resolvers = new List<AN_APIResolver>();

                    s_resolvers.Add(new AN_CoreResolver());
                    s_resolvers.Add(new AN_VendingResolver());
                    s_resolvers.Add(new AN_ContactsResolver());
                    s_resolvers.Add(new AN_FirebaseResolver());
                    s_resolvers.Add(new AN_GooglePlayResolver());
                    s_resolvers.Add(new AN_SocialResolver());
                    s_resolvers.Add(new AN_CameraAndGalleryResolver());
                    s_resolvers.Add(new AN_LocalNotificationsResolver());
                }
                return s_resolvers;
            }
        }


        //--------------------------------------
        // Requirements Resolver
        //--------------------------------------


        private static void Resolve(AN_AndroidBuildRequirements requirements) {

            if(AN_Settings.Instance.ManifestManagement) {
                ResolveManifest(requirements);
            }
           
            ResolveInternalLibs(requirements);

            ResolveBinaryLibs(requirements);
        }


        private static void ResolveInternalLibs(AN_AndroidBuildRequirements requirements) {
            List<string> libsToAdd = new List<string>();
            List<string> libsToRemove = new List<string>();
          

            List<string> internalLibs = SA_AssetDatabase.FindAssetsWithExtentions(AN_Settings.ANDROID_INTERNAL_FOLDER);
            foreach (var lib in internalLibs) {
                string libName = SA_AssetDatabase.GetFileName(lib);
                if (!requirements.HasInternalLib(libName)) {
                    libsToRemove.Add(libName);
                }
            }

            foreach (var lib in requirements.InternalLibs) {
                string libPath = AN_Settings.ANDROID_INTERNAL_FOLDER + lib;
                if (!SA_AssetDatabase.IsFileExists(libPath)) {
                    libsToAdd.Add(lib);
                }
            }

            SA_PluginsEditor.UninstallLibs(AN_Settings.ANDROID_INTERNAL_FOLDER, libsToRemove);
            SA_PluginsEditor.InstallLibs(AN_Settings.ANDROID_INTERNAL_FOLDER_DISABLED, AN_Settings.ANDROID_INTERNAL_FOLDER, libsToAdd);

        }

        private static void ResolveBinaryLibs(AN_AndroidBuildRequirements requirements) {

     

            if(AN_Settings.Instance.UseUnityJarResolver) {
                AN_Dependencies.Resolve(requirements.BinaryDependencies);
                SA_AssetDatabase.DeleteAsset(AN_Settings.ANDROID_MAVEN_FOLDER);
            } else {

                AN_Dependencies.Resolve(new List<AN_BinaryDependency>());


                List<string> repositorysToAdd = new List<string>();
                List<string> repositorysToRemove = new List<string>();

                List<string> mavenLibs = SA_AssetDatabase.FindAssetsWithExtentions(AN_Settings.ANDROID_MAVEN_FOLDER);
                foreach (var lib in mavenLibs) {

                    //we are only interested in folder, we also assume all folders are located inside a root folder
                    if(!SA_AssetDatabase.IsValidFolder(lib)) {
                        continue;
                    }

                    string libName = SA_AssetDatabase.GetFileName(lib);
                    if (!requirements.HasBinaryDependency(libName)) {
                        repositorysToRemove.Add(libName);
                    }
                }

                foreach (var dep in requirements.BinaryDependencies) {
                    string libPath = AN_Settings.ANDROID_MAVEN_FOLDER + dep.GetLocalRepositoryName();
                    if (!SA_AssetDatabase.IsDirectoryExists(libPath)) {
                        string localRepositoryName = dep.GetLocalRepositoryName();
                        if(!repositorysToAdd.Contains(localRepositoryName)) {
                            repositorysToAdd.Add(localRepositoryName);
                        }
                    }
                   
                }

                SA_PluginsEditor.UninstallLibs(AN_Settings.ANDROID_MAVEN_FOLDER, repositorysToRemove);

                foreach(var lib in repositorysToAdd) {
                    string source = AN_Settings.ANDROID_MAVEN_FOLDER_DISABLED + lib;
                    string destination = AN_Settings.ANDROID_MAVEN_FOLDER + lib;
                    SA_PluginsEditor.InstallLibFolder(source, destination);
                }
            }
        }




        private static void ResolveManifest(AN_AndroidBuildRequirements requirements) {
            var androidManifest = new AMM_AndroidManifest(AN_Settings.ANDROID_MANIFEST_FILE_PATH);
                
            var manifest = androidManifest.Template; 
            var application = manifest.ApplicationTemplate;

            //Removing not used activities 
            List<AMM_ActivityTemplate> unusedActivities = new List<AMM_ActivityTemplate>();
            foreach (var pair in application.Activities) {
                var act = pair.Value;
                if (!requirements.HasActivityWithName(act.Name)) {
                    unusedActivities.Add(act);
                }
            }


            foreach (var act in unusedActivities) {
                application.RemoveActivity(act);
            }


            //Add required activities
            foreach (var activity in requirements.Activities) {
                var act = application.GetOrCreateActivityWithName(activity.Name);
                foreach (var pair in activity.Values) {
                    act.SetValue(pair.Key, pair.Value);
                }
            }

            //application properties
            ResolveProperties(manifest, requirements.ManifestProperties);
            ResolveProperties(application, requirements.ApplicationProperties);
          

            //Removing not used permissions 
            List<string> unusedPermissions = new List<string>();
            foreach (var perm in manifest.Permissions) {
                if (!requirements.HasPermissionWithName(perm.Name)) {
                    unusedPermissions.Add(perm.Name);
                }
            }
           
            foreach (var perm in unusedPermissions) {
                manifest.RemovePermission(perm);
            }

            //Add required permission
            foreach (var permission in requirements.Permissions) {
                manifest.AddPermission(permission);
               // tpl.SetValue("tools:remove", "android:maxSdkVersion");
            }

            //TODO only save if there is chnaged to save
            androidManifest.SaveManifest();
        }

        private static void ResolveProperties(AMM_BaseTemplate template, List<AMM_PropertyTemplate> requirementsProperiesList) {
            List<AMM_PropertyTemplate> unusedProperties = new List<AMM_PropertyTemplate>();
            foreach (var pair in template.Properties) {

                List<AMM_PropertyTemplate> properties = pair.Value;
                foreach (var prop in properties) {
                    if (!HasPropertyWithName(prop.Tag, prop.Name, requirementsProperiesList)) {
                        unusedProperties.Add(prop);
                    }
                }

            }

            foreach (var prop in unusedProperties) {
                template.RemoveProperty(prop);
            }


            foreach (var propery in requirementsProperiesList) {
                var p = template.GetPropertyWithName(propery.Tag, propery.Name);
                if (p != null) {
                    template.RemoveProperty(p);
                }
                template.AddProperty(propery);
            }
        }


        private static bool HasPropertyWithName(string tag, string name, List<AMM_PropertyTemplate> properiesList) {
            foreach (var property in properiesList) {
                if (property.Name.Equals(name) && property.Tag.Equals(tag)) {
                    return true;
                }
            }
            return false;
        }


        //--------------------------------------
        // IPreprocessBuild
        //--------------------------------------


        public void OnPreprocessBuild(BuildTarget target, string path) {
            Preprocess(target);
        }


#if UNITY_2018_1_OR_NEWER
        public void OnPreprocessBuild(BuildReport report) {
            Preprocess(report.summary.platform);
        }
#endif


        public int callbackOrder  { get { return 0; }}



        //--------------------------------------
        // Private Methods
        //--------------------------------------

        private void Preprocess(BuildTarget target) {
            if (target == BuildTarget.Android) {
                Resolve();
            }
        }
    }
}


