using System;
using System.Xml;
using System.Collections.Generic;
using SA.Foundation.Utility;
using SA.Foundation.UtilitiesEditor;

using SA.Android.Utilities;


namespace SA.Android
{
    public static class AN_Dependencies 
    {
       private static List<string> s_activeDependencies;

        public static void Resolve(IEnumerable<AN_BinaryDependency> dependencies)  {
            
            var depList = new List<string>();
            foreach(var dep in dependencies) {
                depList.Add(dep.GetRemoteRepositoryName());
            }

            ResolveXMLConfig(depList);
        }

        private static void ResolveXMLConfig(List<string> dependencies) {

            //Clean up file if we have no Dependencies
            if(dependencies.Count == 0) {
                if(SA_AssetDatabase.IsDirectoryExists(AN_Settings.DEPENDENCIES_FOLDER)) {
                    SA_AssetDatabase.DeleteAsset(AN_Settings.DEPENDENCIES_FOLDER);
                }
                s_activeDependencies = new List<string>();
                return;
            }

            if(IsEqualsToActiveDependencies(dependencies)) {
                return;
            }

            if(!SA_AssetDatabase.IsValidFolder(AN_Settings.DEPENDENCIES_FOLDER)) {
                SA_AssetDatabase.CreateFolder(AN_Settings.DEPENDENCIES_FOLDER);
            }


            var doc = new XmlDocument();
            var dependenciesElement = doc.CreateElement("dependencies");
            var androidPackagesElement = doc.CreateElement("androidPackages");


            foreach(var dependency in dependencies) {
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
            s_activeDependencies = ReadDependencies();
        }


        private static bool IsEqualsToActiveDependencies(List<string> dependencies) {
            if(s_activeDependencies == null) {
                s_activeDependencies = ReadDependencies();
            }


            if(s_activeDependencies.Count != dependencies.Count) {
                return false;
            }

            bool equal = true;
            foreach (var dependency in dependencies) {
                if(!s_activeDependencies.Contains(dependency)) {
                    equal = false;
                    break;
                }
            }


            return equal;
              
            
        }


        private static List<string> ReadDependencies() {
            var result = new List<string>();
            try {
                if (SA_AssetDatabase.IsFileExists(AN_Settings.DEPENDENCIES_FILE_PATH)) {
                    var doc = new XmlDocument();
                    doc.Load(SA_PathUtil.ConvertRelativeToAbsolutePath(AN_Settings.DEPENDENCIES_FILE_PATH));
                    var xnList = doc.SelectNodes("dependencies/androidPackages/androidPackage");

                    foreach (XmlNode xn in xnList) {
                        var spec = xn.Attributes["spec"].Value;
                        result.Add(spec);
                    }
                }
            } catch(Exception ex) {
                AN_Logger.LogError("Error reading AN_Dependencies");
                AN_Logger.LogError( AN_Settings.DEPENDENCIES_FILE_PATH +" filed: " +  ex.Message);
            }



            return result;
        }

       
    }
}