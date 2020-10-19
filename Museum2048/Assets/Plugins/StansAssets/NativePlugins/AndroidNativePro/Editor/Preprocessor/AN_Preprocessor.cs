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
using UnityEngine;

namespace SA.Android.Editor
{
#if UNITY_2018_1_OR_NEWER
    public class AN_Preprocessor : IPreprocessBuildWithReport
#else
    public class AN_Preprocessor : IPreprocessBuild
#endif
    {
        static List<AN_APIResolver> s_Resolvers;

        //--------------------------------------
        // Static
        //--------------------------------------

        public static void Resolve()
        {
            var versionUpdated = AN_Settings.UpdateVersion(AN_Settings.FormattedVersion) && !SA_PluginTools.IsDevelopmentMode;
            var requirements = new AN_AndroidBuildRequirements();
            if (versionUpdated) SA_AssetDatabase.DeleteAsset(AN_Settings.ANDROID_INTERNAL_FOLDER);

            foreach (var resolver in Resolvers) resolver.Run(requirements);
            Resolve(requirements);
        }

        public static void RegisterResolver(AN_APIResolver resolver)
        {
            Resolvers.Add(resolver);
        }

        public static T GetResolver<T>() where T : AN_APIResolver
        {
            return (T)GetResolver(typeof(T));
        }

        public static AN_APIResolver GetResolver(Type resolverType)
        {
            foreach (var resolver in Resolvers)
                if (resolver.GetType().Equals(resolverType))
                    return resolver;

            return null;
        }

        public static List<AN_APIResolver> Resolvers
        {
            get
            {
                if (s_Resolvers == null)
                {
                    s_Resolvers = new List<AN_APIResolver>();

                    s_Resolvers.Add(new AN_CoreResolver());
                    s_Resolvers.Add(new AN_VendingResolver());
                    s_Resolvers.Add(new AN_ContactsResolver());
                    s_Resolvers.Add(new AN_GooglePlayResolver());
                    s_Resolvers.Add(new AN_SocialResolver());
                    s_Resolvers.Add(new AN_CameraAndGalleryResolver());
                    s_Resolvers.Add(new AN_LocalNotificationsResolver());
                    s_Resolvers.Add(new AN_FirebaseResolver());
                }

                return s_Resolvers;
            }
        }

        //--------------------------------------
        // Requirements Resolver
        //--------------------------------------

        static void Resolve(AN_AndroidBuildRequirements requirements)
        {
            if (AN_Settings.Instance.ManifestManagement) ResolveManifest(requirements);

            ResolveInternalLibs(requirements);

            ResolveBinaryLibs(requirements);
        }

        static void ResolveInternalLibs(AN_AndroidBuildRequirements requirements)
        {
            var libsToAdd = new List<string>();
            var libsToRemove = new List<string>();

            var internalLibs = SA_AssetDatabase.FindAssetsWithExtentions(AN_Settings.ANDROID_INTERNAL_FOLDER);
            foreach (var lib in internalLibs)
            {
                var libName = SA_AssetDatabase.GetFileName(lib);
                if (!requirements.HasInternalLib(libName)) libsToRemove.Add(libName);
            }

            foreach (var lib in requirements.InternalLibs)
            {
                var libPath = AN_Settings.ANDROID_INTERNAL_FOLDER + lib;
                if (!SA_AssetDatabase.IsFileExists(libPath)) libsToAdd.Add(lib);
            }

            SA_PluginsEditor.UninstallLibs(AN_Settings.ANDROID_INTERNAL_FOLDER, libsToRemove);
            SA_PluginsEditor.InstallLibs(AN_Settings.ANDROID_INTERNAL_FOLDER_DISABLED, AN_Settings.ANDROID_INTERNAL_FOLDER, libsToAdd);
        }

        static void ResolveBinaryLibs(AN_AndroidBuildRequirements requirements)
        {
            AN_ResolveManager.Resolve(requirements.BinaryDependencies);
        }

        static void ResolveManifest(AN_AndroidBuildRequirements requirements)
        {
            var androidManifest = new AMM_AndroidManifest(AN_Settings.ANDROID_MANIFEST_FILE_PATH);

            var manifest = androidManifest.Template;
            var application = manifest.ApplicationTemplate;

            // Removing not used activities.
            var unusedActivities = new List<AMM_ActivityTemplate>();
            foreach (var pair in application.Activities)
            {
                var act = pair.Value;
                if (!requirements.HasActivityWithName(act.Name)) unusedActivities.Add(act);
            }

            foreach (var act in unusedActivities) application.RemoveActivity(act);

            // Add required activities.
            foreach (var activity in requirements.Activities)
            {
                var act = application.GetOrCreateActivityWithName(activity.Name);
                foreach (var pair in activity.Values) act.SetValue(pair.Key, pair.Value);
            }

            // Application properties.
            ResolveProperties(manifest, requirements.ManifestProperties);
            ResolveProperties(application, requirements.ApplicationProperties);

            // Removing not used permissions.
            var unusedPermissions = new List<string>();
            foreach (var perm in manifest.Permissions)
                if (!requirements.HasPermissionWithName(perm.Name))
                    unusedPermissions.Add(perm.Name);

            foreach (var perm in unusedPermissions) manifest.RemovePermission(perm);

            // Add required permission.
            foreach (var permission in requirements.Permissions) manifest.AddPermission(permission);

            // TODO only save if there is changed to save.
            androidManifest.SaveManifest();
        }

        static void ResolveProperties(AMM_BaseTemplate template, List<AMM_PropertyTemplate> requirementsPropertiesList)
        {
            var unusedProperties = new List<AMM_PropertyTemplate>();
            foreach (var pair in template.Properties)
            {
                var properties = pair.Value;
                foreach (var prop in properties)
                    if (!HasPropertyWithName(prop.Tag, prop.Name, requirementsPropertiesList))
                        unusedProperties.Add(prop);
            }

            foreach (var prop in unusedProperties) template.RemoveProperty(prop);

            foreach (var property in requirementsPropertiesList)
            {
                var p = template.GetPropertyWithName(property.Tag, property.Name);
                if (p != null)
                    template.RemoveProperty(p);

                template.AddProperty(property);
            }
        }

        static bool HasPropertyWithName(string tag, string name, List<AMM_PropertyTemplate> propertiesList)
        {
            foreach (var property in propertiesList)
                if (property.Name.Equals(name) && property.Tag.Equals(tag))
                    return true;
            return false;
        }

        //--------------------------------------
        // IPreprocessBuild
        //--------------------------------------

        public void OnPreprocessBuild(BuildTarget target, string path)
        {
            Preprocess(target);
        }

#if UNITY_2018_1_OR_NEWER
        public void OnPreprocessBuild(BuildReport report)
        {
            Preprocess(report.summary.platform);
        }
#endif

        public int callbackOrder => 0;

        //--------------------------------------
        // Private Methods
        //--------------------------------------

        void Preprocess(BuildTarget target)
        {
            if (target == BuildTarget.Android) Resolve();
        }
    }
}
