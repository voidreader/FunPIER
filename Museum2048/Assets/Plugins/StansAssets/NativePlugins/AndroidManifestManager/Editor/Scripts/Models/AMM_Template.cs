////////////////////////////////////////////////////////////////////////////////
//  
// @module Manifest Manager
// @author Alex Yaremenko (Stan's Assets) 
// @support support@stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Xml;
using System.Collections.Generic;

namespace SA.Android.Manifest
{
    public class AMM_Template : AMM_BaseTemplate
    {
        readonly AMM_ApplicationTemplate _applicationTemplate = null;
        readonly List<AMM_PropertyTemplate> _permissions = null;

        public AMM_Template()
            : base()
        {
            _applicationTemplate = new AMM_ApplicationTemplate();
            _permissions = new List<AMM_PropertyTemplate>();
        }

        public bool HasPermission(string name)
        {
            foreach (var permission in Permissions)
                if (permission.Name.Equals(name))
                    return true;

            return false;
        }

        public AMM_PropertyTemplate GetPermission(string name)
        {
            foreach (var permission in Permissions)
                if (permission.Name.Equals(name))
                    return permission;

            return null;
        }

        public void RemovePermission(AMM_ManifestPermission permission)
        {
            RemovePermission(permission.GetFullName());
        }

        public void RemovePermission(string name)
        {
            while (HasPermission(name))
                foreach (var permission in Permissions)
                    if (permission.Name.Equals(name))
                    {
                        RemovePermission(permission);
                        break;
                    }
        }

        public void RemovePermission(AMM_PropertyTemplate permission)
        {
            _permissions.Remove(permission);
        }

        public AMM_PropertyTemplate AddPermission(string name)
        {
            if (!HasPermission(name))
            {
                var uses_permission = new AMM_PropertyTemplate("uses-permission");
                uses_permission.Name = name;
                AddPermission(uses_permission);

                return uses_permission;
            }
            else
            {
                return GetPermission(name);
            }
        }

        public void AddPermission(AMM_PropertyTemplate permission)
        {
            _permissions.Add(permission);
        }

        public AMM_PropertyTemplate AddPermission(AMM_ManifestPermission permission)
        {
            return AddPermission(permission.GetFullName());
        }

        public override void ToXmlElement(XmlDocument doc, XmlElement parent)
        {
            AddAttributesToXml(doc, parent, this);
            AddPropertiesToXml(doc, parent, this);

            var app = doc.CreateElement("application");
            _applicationTemplate.ToXmlElement(doc, app);
            parent.AppendChild(app);

            foreach (var permission in Permissions)
            {
                var p = doc.CreateElement("uses-permission");
                permission.ToXmlElement(doc, p);
                parent.AppendChild(p);
            }
        }

        public AMM_ApplicationTemplate ApplicationTemplate => _applicationTemplate;

        public List<AMM_PropertyTemplate> Permissions => _permissions;
    }
}
