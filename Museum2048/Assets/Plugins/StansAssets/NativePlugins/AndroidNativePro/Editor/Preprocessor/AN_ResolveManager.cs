using System;
using System.Xml;
using System.Collections.Generic;
using SA.Foundation.Utility;
using SA.Foundation.UtilitiesEditor;
using SA.Android.Utilities;

namespace SA.Android
{
    internal static class AN_ResolveManager 
    {
        private const string k_PlayServicesResolver = "Google.JarResolver";
        private const string k_PlayServicesResolverType = "GooglePlayServices.PlayServicesResolver, Google.JarResolver";
        private const string k_PlayServicesResolverForceResolveMethod = "MenuForceResolve";
        private static List<string> activeDependencies;

        public static void Resolve(IEnumerable<AN_BinaryDependency> dependencies)  
        {
            var depList = new List<string>();
            foreach(var dep in dependencies) 
            {
                depList.Add(dep.GetRemoteRepositoryName());
            }

            ResolveXMLConfig(depList);
        }

        //--------------------------------------
        // Search and reset state for Jar Resolver
        //--------------------------------------

        internal static void ProcessAssets() 
        {
            var projectLibs = SA_AssetDatabase.FindAssetsWithExtentions("Assets", ".dll");
            foreach (var lib in projectLibs) 
            {
                if(ProcessAssetImport(lib))
                {
                    return;
                }
            }
            UpdateJarResolverState(false, null);

        }

        internal static bool ProcessAssetImport(string assetPath)
        {
            bool detected = IsPathEqualsSDKName(assetPath, k_PlayServicesResolver);
            if (detected) 
            {
                UpdateJarResolverState(true, GetJarResolverVersion(assetPath, k_PlayServicesResolver));
            }
            return detected;
        }

        internal static void ProcessAssetDelete(string assetPath)
        {
            bool detected = IsPathEqualsSDKName(assetPath, k_PlayServicesResolver);
            if (detected) 
            {
                UpdateJarResolverState(false, null);
            }
        }


        private static void UpdateJarResolverState(bool enabled, string version)
        {
            if(enabled)
            {
                if(!String.IsNullOrEmpty(version))
                {
                    AN_Settings.Instance.UnityJarResolverVersion = version;
                }
                if (!AN_Settings.Instance.UseUnityJarResolver) 
                {
                    AN_Settings.Instance.UseUnityJarResolver = true;
                    
                    AN_Preprocessor.Resolve();
                    if(UnityEditor.EditorUserBuildSettings.activeBuildTarget.Equals(UnityEditor.BuildTarget.Android))
                    {
                        CallJarForceResolve();
                    }
                }
            }
            else
            {
                if (AN_Settings.Instance.UseUnityJarResolver) 
                {
                    AN_Settings.Instance.UseUnityJarResolver = false;
                    AN_Settings.Instance.UnityJarResolverVersion = "";
                    AN_Preprocessor.Resolve();
                }
            }
        }

        private static bool IsPathEqualsSDKName(string assetPath, string SDKName) 
        {
            var fileName = SA_PathUtil.GetFileName(assetPath);
            var extension = SA_PathUtil.GetExtension(assetPath);
            return fileName.Contains(SDKName) && extension.Equals(".dll");
        }

        private static string GetJarResolverVersion(string assetPath, string SDKName)
        {
            string version = SA_PathUtil.GetFileNameWithoutExtension(assetPath);
            
            if (version.Contains(SDKName)) 
            {
                try
                {
                    version = version.Remove(0, SDKName.Length);
                    if(version.Length > 0 && version[0].Equals('_'))
                    {
                        version = version.Remove(0, 1);
                    }
                    return version;
                }
                catch(Exception ex)
                {
                    AN_Logger.LogError(string.Format("Error at getting Jar Resolver version - {0}",ex.Message));
                    return null;
                }
            }

            return null;
        }

        private static void CallJarForceResolve()
        {
            try
            {
                var type = Type.GetType(k_PlayServicesResolverType, false, true);
                if(type == null)
                    return;
                
			    var method = type.GetMethod(k_PlayServicesResolverForceResolveMethod);
                if (method == null) return;
               
                var activator = Activator.CreateInstance(type);
                method.Invoke(activator, null);
            }
            catch(Exception ex)
            {
                AN_Logger.LogError(string.Format("Error at Force Resolve call - {0}",ex.Message));
            }
        }

        //--------------------------------------
        // Resolver XMLConfig
        //--------------------------------------
        
        private static void ResolveXMLConfig(List<string> dependencies) 
        {
            // Clean up file if we have no Dependencies.
            if(dependencies.Count == 0) 
            {
                if(SA_AssetDatabase.IsDirectoryExists(AN_Settings.DEPENDENCIES_FOLDER))
                    SA_AssetDatabase.DeleteAsset(AN_Settings.DEPENDENCIES_FOLDER);
                
                activeDependencies = new List<string>();
                return;
            }

            if(IsEqualsToActiveDependencies(dependencies))
                return;
            
            if(!SA_AssetDatabase.IsValidFolder(AN_Settings.DEPENDENCIES_FOLDER))
                SA_AssetDatabase.CreateFolder(AN_Settings.DEPENDENCIES_FOLDER);
            
            var doc = new XmlDocument();
            var dependenciesElement = doc.CreateElement("dependencies");
            var androidPackagesElement = doc.CreateElement("androidPackages");


            foreach(var dependency in dependencies) 
            {
                var androidPackage = doc.CreateElement("androidPackage");
                var spec = doc.CreateAttribute("spec");
                spec.Value = dependency;
                androidPackage.Attributes.Append(spec);
                androidPackagesElement.AppendChild(androidPackage);
            }

            dependenciesElement.AppendChild(androidPackagesElement);
            doc.AppendChild(dependenciesElement);
            doc.Save(SA_PathUtil.ConvertRelativeToAbsolutePath(AN_Settings.DEPENDENCIES_FILE_PATH));
            SA_AssetDatabase.ImportAsset(AN_Settings.DEPENDENCIES_FILE_PATH);
            activeDependencies = ReadDependencies();
        }


        private static bool IsEqualsToActiveDependencies(List<string> dependencies) 
        {
            if(activeDependencies == null) 
            {
                activeDependencies = ReadDependencies();
            }


            if(activeDependencies.Count != dependencies.Count) 
            {
                return false;
            }

            bool equal = true;
            foreach (var dependency in dependencies) 
            {
                if(!activeDependencies.Contains(dependency)) 
                {
                    equal = false;
                    break;
                }
            }

            return equal;     
            
        }


        private static List<string> ReadDependencies() 
        {
            var result = new List<string>();
            try 
            {
                if (SA_AssetDatabase.IsFileExists(AN_Settings.DEPENDENCIES_FILE_PATH)) 
                {
                    var doc = new XmlDocument();
                    doc.Load(SA_PathUtil.ConvertRelativeToAbsolutePath(AN_Settings.DEPENDENCIES_FILE_PATH));
                    var xnList = doc.SelectNodes("dependencies/androidPackages/androidPackage");

                    foreach (XmlNode xn in xnList) 
                    {
                        var spec = xn.Attributes["spec"].Value;
                        result.Add(spec);
                    }
                }
            } 
            catch(Exception ex) 
            {
                AN_Logger.LogError("Error reading AN_ResolveManager");
                AN_Logger.LogError( AN_Settings.DEPENDENCIES_FILE_PATH +" filed: " +  ex.Message);
            }

            return result;
        }

       
    }
}