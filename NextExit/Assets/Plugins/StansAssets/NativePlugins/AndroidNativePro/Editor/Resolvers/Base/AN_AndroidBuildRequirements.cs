using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.Manifest;

namespace SA.Android {
    
    public class AN_AndroidBuildRequirements 
    {

        private readonly List<AMM_ActivityTemplate> m_activities = new List<AMM_ActivityTemplate>();
        private readonly List<AMM_ManifestPermission> m_permissions = new List<AMM_ManifestPermission>();
        private readonly List<AMM_PropertyTemplate> m_applicationProperties = new List<AMM_PropertyTemplate>();
        private readonly List<AMM_PropertyTemplate> m_manifestProperties = new List<AMM_PropertyTemplate>();
        

        private readonly List<AN_BinaryDependency> m_binaryDependencies = new List<AN_BinaryDependency>();
        private readonly List<string> m_internalLibs = new List<string>();


        //--------------------------------------
        // Public Methods
        //--------------------------------------

        public void AddActivity(AMM_ActivityTemplate acitivty) {
            m_activities.Add(acitivty);
        }

        public void AddApplicationProperty(AMM_PropertyTemplate property) {
            m_applicationProperties.Add(property);
        }

        public void AddMenifestProperty(AMM_PropertyTemplate property) {
            m_manifestProperties.Add(property);
        }

        public void AddBinaryDependency(AN_BinaryDependency dependency) {
            if (m_binaryDependencies.Contains(dependency)) { return; }
            m_binaryDependencies.Add(dependency);
        }


        public void AddPermission(AMM_ManifestPermission permission) {
            m_permissions.Add(permission);
        }



        public void AddInternalLib(string lib) {
            m_internalLibs.Add(lib);
        }


        public bool HasActivityWithName(string name) {
            foreach(var activity in Activities) {
                if(activity.Name.Equals(name)) {
                    return true;
                }
            }

            return false;
        }


        public bool HasPermissionWithName(string name) {
            foreach (var perm in Permissions) {
                if (perm.GetFullName().Equals(name)) {
                    return true;
                }
            }

            return false;
        }


        public bool HasInternalLib(string name) {
            return m_internalLibs.Contains(name);
        }

        public bool HasBinaryDependency(string name) {
            foreach(var dep in m_binaryDependencies) {
                if(dep.GetLocalRepositoryName().Equals(name)) {
                    return true;
                }
            }

            return false;
        }


        //--------------------------------------
        // Get / Set
        //--------------------------------------

        public bool IsEmpty {
            get {
                return ApplicationProperties.Count == 0 && Activities.Count == 0 && Permissions.Count == 0 && InternalLibs.Count == 0 && BinaryDependencies.Count == 0;
            }
        }

        public List<AMM_ActivityTemplate> Activities {
            get {
                return m_activities;
            }
        }


        public List<AMM_PropertyTemplate> ApplicationProperties {
            get {
                return m_applicationProperties;
            }
        }

        public List<AMM_PropertyTemplate> ManifestProperties {
            get {
                return m_manifestProperties;
            }
        }

        public List<AMM_ManifestPermission> Permissions {
            get {
                return m_permissions;
            }
        }

        public List<string> InternalLibs {
            get {
                return m_internalLibs;
            }
        }

        public List<AN_BinaryDependency> BinaryDependencies {
            get {
                return m_binaryDependencies;
            }
        }
    }
}


