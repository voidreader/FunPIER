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
    public class AMM_ActivityTemplate : AMM_BaseTemplate
    {
        public bool IsOpen = false;

        readonly int _id = 0;
        bool _isLauncher = false;
        string _name = string.Empty;

        public AMM_ActivityTemplate(bool isLauncher, string name)
            : base()
        {
            _isLauncher = isLauncher;
            _name = name;
            _id = GetHashCode();

            m_values = new Dictionary<string, string>();
            m_properties = new Dictionary<string, List<AMM_PropertyTemplate>>();
            SetValue("android:name", name);
        }

        public void SetName(string name)
        {
            _name = name;
            SetValue("android:name", name);
        }

        public void SetAsLauncher(bool isLauncher)
        {
            _isLauncher = isLauncher;
        }

        public static AMM_PropertyTemplate GetLauncherPropertyTemplate()
        {
            var launcher = new AMM_PropertyTemplate("intent-filter");

            var prop = new AMM_PropertyTemplate("action");
            prop.SetValue("android:name", "android.intent.action.MAIN");
            launcher.AddProperty("action", prop);

            prop = new AMM_PropertyTemplate("category");
            prop.SetValue("android:name", "android.intent.category.LAUNCHER");
            launcher.AddProperty("category", prop);

            return launcher;
        }

        public bool IsLauncherProperty(AMM_PropertyTemplate property)
        {
            if (property.Tag.Equals("intent-filter"))
                if (property.Properties.ContainsKey("category"))
                    foreach (var p in property.Properties["category"])
                        if (p.Values.ContainsKey("android:name"))
                            if (p.Values["android:name"].Equals("android.intent.category.LAUNCHER"))
                                return true;

            return false;
        }

        public override void ToXmlElement(XmlDocument doc, XmlElement parent)
        {
            AddAttributesToXml(doc, parent, this);

            AMM_PropertyTemplate launcher = null;
            if (_isLauncher)
            {
                launcher = GetLauncherPropertyTemplate();
                AddProperty(launcher.Tag, launcher);
            }

            AddPropertiesToXml(doc, parent, this);
            if (_isLauncher) m_properties["intent-filter"].Remove(launcher);
        }

        public bool IsLauncher => _isLauncher;

        public string Name => _name;

        public int Id => _id;
    }
}
