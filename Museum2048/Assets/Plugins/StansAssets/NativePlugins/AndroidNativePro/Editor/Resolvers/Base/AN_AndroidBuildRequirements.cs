using System.Collections.Generic;
using SA.Android.Manifest;

namespace SA.Android.Editor
{
    public class AN_AndroidBuildRequirements
    {
        readonly List<AMM_ActivityTemplate> m_Activities = new List<AMM_ActivityTemplate>();
        readonly List<AMM_ManifestPermission> m_Permissions = new List<AMM_ManifestPermission>();
        readonly List<AMM_PropertyTemplate> m_ApplicationProperties = new List<AMM_PropertyTemplate>();
        readonly List<AMM_PropertyTemplate> m_ManifestProperties = new List<AMM_PropertyTemplate>();
        readonly List<AN_BinaryDependency> m_BinaryDependencies = new List<AN_BinaryDependency>();
        readonly List<string> m_InternalLibs = new List<string>();

        //--------------------------------------
        // Public Methods
        //--------------------------------------

        public void AddActivity(AMM_ActivityTemplate activity)
        {
            m_Activities.Add(activity);
        }

        public void AddApplicationProperty(AMM_PropertyTemplate property)
        {
            m_ApplicationProperties.Add(property);
        }

        public void AddManifestProperty(AMM_PropertyTemplate property)
        {
            m_ManifestProperties.Add(property);
        }

        public void AddBinaryDependency(AN_BinaryDependency dependency)
        {
            if (m_BinaryDependencies.Contains(dependency)) return;
            m_BinaryDependencies.Add(dependency);
        }

        public void AddPermission(AMM_ManifestPermission permission)
        {
            m_Permissions.Add(permission);
        }

        public void AddInternalLib(string lib)
        {
            m_InternalLibs.Add(lib);
        }

        public bool HasActivityWithName(string name)
        {
            foreach (var activity in Activities)
                if (activity.Name.Equals(name))
                    return true;

            return false;
        }

        public bool HasPermissionWithName(string name)
        {
            foreach (var perm in Permissions)
                if (perm.GetFullName().Equals(name))
                    return true;

            return false;
        }

        public bool HasInternalLib(string name)
        {
            return m_InternalLibs.Contains(name);
        }

        //--------------------------------------
        // Get / Set
        //--------------------------------------

        public bool IsEmpty =>
            ApplicationProperties.Count == 0
            && Activities.Count == 0
            && Permissions.Count == 0
            && InternalLibs.Count == 0
            && BinaryDependencies.Count == 0;

        public List<AMM_ActivityTemplate> Activities => m_Activities;

        public List<AMM_PropertyTemplate> ApplicationProperties => m_ApplicationProperties;

        public List<AMM_PropertyTemplate> ManifestProperties => m_ManifestProperties;

        public List<AMM_ManifestPermission> Permissions => m_Permissions;

        public List<string> InternalLibs => m_InternalLibs;

        public List<AN_BinaryDependency> BinaryDependencies => m_BinaryDependencies;
    }
}
