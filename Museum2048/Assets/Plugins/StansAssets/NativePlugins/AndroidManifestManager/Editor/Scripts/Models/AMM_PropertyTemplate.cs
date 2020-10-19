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
    public class AMM_PropertyTemplate : AMM_BaseTemplate
    {
        public bool IsOpen = false;

        readonly string _tag = string.Empty;

        public AMM_PropertyTemplate(string tag)
            : base()
        {
            _tag = tag;
        }

        public override void ToXmlElement(XmlDocument doc, XmlElement parent)
        {
            AddAttributesToXml(doc, parent, this);
            AddPropertiesToXml(doc, parent, this);
        }

        public string Tag => _tag;

        public string Name
        {
            get => GetValue("android:name");

            set => SetValue("android:name", value);
        }

        public string Value
        {
            get => GetValue("android:value");

            set => SetValue("android:value", value);
        }

        public string Label
        {
            get => GetValue("android:label");

            set => SetValue("android:label", value);
        }
    }
}
