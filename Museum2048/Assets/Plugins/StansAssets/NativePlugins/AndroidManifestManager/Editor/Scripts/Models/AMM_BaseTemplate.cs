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

namespace SA.Android.Manifest {

	public abstract class AMM_BaseTemplate {
        protected Dictionary<string, List<AMM_PropertyTemplate>> m_properties = new Dictionary<string, List<AMM_PropertyTemplate>>();
        protected Dictionary<string, string> m_values = new Dictionary<string, string>();

		public AMM_PropertyTemplate GetOrCreateIntentFilterWithName(string name) {
			AMM_PropertyTemplate filter = GetIntentFilterWithName(name);
			if(filter == null) {
				filter =  new AMM_PropertyTemplate("intent-filter");
				AMM_PropertyTemplate action = new AMM_PropertyTemplate("action");
				action.SetValue("android:name", name);
				filter.AddProperty(action);
				AddProperty(filter);
			}

			return filter;
		}


		public AMM_PropertyTemplate GetIntentFilterWithName(string name) {
			string tag = "intent-filter";
			List<AMM_PropertyTemplate> filters =  GetPropertiesWithTag(tag);
			foreach(AMM_PropertyTemplate intent_filter in filters) {
				string filter_name = GetIntentFilterName(intent_filter);
				if(filter_name.Equals(name)) {
					return intent_filter;
				}
			}

			return null;

		}


		public string GetIntentFilterName(AMM_PropertyTemplate intent) {

			List<AMM_PropertyTemplate> actions = intent.GetPropertiesWithTag("action");
			if(actions.Count > 0) {
				return actions[0].GetValue("android:name");
			} else {
				return string.Empty;
			}

		}

		public AMM_PropertyTemplate GetOrCreatePropertyWithName(string tag, string name) {
			AMM_PropertyTemplate p =  GetPropertyWithName(tag, name);
			if(p == null) {
				p = new AMM_PropertyTemplate(tag);
				p.SetValue("android:name", name);
				AddProperty(p);
			}

			return p;
		}


		public AMM_PropertyTemplate GetPropertyWithName(string tag, string name) {

			List<AMM_PropertyTemplate> tags = GetPropertiesWithTag(tag);
			foreach(AMM_PropertyTemplate prop in tags) {
                if (prop.Values.ContainsKey("android:name")) {
                    if (prop.Values["android:name"] == name) {
                        return prop;
                    }
                } else {
                    if(string.IsNullOrEmpty(name)) {
                        return prop;
                    }
                }   
			}

			return null;
		}



        public AMM_PropertyTemplate GetOrCreatePropertyWithTag(string tag) {
			AMM_PropertyTemplate p = GetPropertyWithTag(tag);
			if(p == null) {
				p = new AMM_PropertyTemplate(tag);
				AddProperty(p);
			}

			return p;
		}


		public AMM_PropertyTemplate GetPropertyWithTag(string tag) {
			List<AMM_PropertyTemplate> props = GetPropertiesWithTag(tag);
			if(props.Count > 0) {
				return props[0];
			} else {
				return null;
			}
		}


		public List<AMM_PropertyTemplate> GetPropertiesWithTag(string tag) {
			if(Properties.ContainsKey(tag)) {
				return Properties[tag];
			} else {
				return new List<AMM_PropertyTemplate>();
			}

		} 
		
		public abstract void ToXmlElement(XmlDocument doc, XmlElement parent);

		public AMM_BaseTemplate(){
			m_values = new Dictionary<string, string> ();
			m_properties = new Dictionary<string, List<AMM_PropertyTemplate>> ();
		}
		

		public void AddProperty(AMM_PropertyTemplate property) {
			AddProperty(property.Tag, property);
		}

		public void AddProperty(string tag, AMM_PropertyTemplate property) {
			if (!m_properties.ContainsKey(tag)) {
				List<AMM_PropertyTemplate> list = new List<AMM_PropertyTemplate>();
				m_properties.Add(tag, list);
			}
			m_properties [tag].Add (property);
		}
		
		public void SetValue(string key, string value) {
			if(m_values.ContainsKey(key)) {
				m_values[key] = value;
			} else {
				m_values.Add (key, value);
			}
		}

		public string GetValue(string key) {
			if(m_values.ContainsKey(key)) {
				return m_values[key];
			} else {
				return string.Empty;
			}
		}

		public void RemoveProperty(AMM_PropertyTemplate property) {
			m_properties [property.Tag].Remove (property);
		}
		
		public void RemoveValue(string key) {
			m_values.Remove (key);
		}

		public void AddPropertiesToXml(XmlDocument doc, XmlElement parent, AMM_BaseTemplate template) {
			foreach (string key in template.Properties.Keys) {
				foreach (AMM_PropertyTemplate p in template.Properties[key]) {
					XmlElement n = doc.CreateElement(key);
					AddAttributesToXml(doc, n, p);
					AddPropertiesToXml(doc, n, p);
					parent.AppendChild(n);
				}
			}
		}
		
		public void AddAttributesToXml(XmlDocument doc, XmlElement parent, AMM_BaseTemplate template) {
			foreach (string key in template.Values.Keys) {

				string k = key;
				if (key.Contains("android:")) {
					k = key.Replace("android:", "android___");
				} else if (key.Contains("tools:")) {
					k = key.Replace("tools:", "tools___");
				}
				XmlAttribute attr = doc.CreateAttribute (k);
				attr.Value = template.Values[key];

				parent.Attributes.Append(attr);
			}
		}

		public Dictionary<string, string> Values {
			get {
				return m_values;
			}
		}
		
		public Dictionary<string, List<AMM_PropertyTemplate>> Properties {
			get {
				return m_properties;
			}
		}
	}

}
