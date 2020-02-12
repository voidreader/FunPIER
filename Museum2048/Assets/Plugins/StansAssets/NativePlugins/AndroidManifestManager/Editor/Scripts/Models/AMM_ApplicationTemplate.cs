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

	public class AMM_ApplicationTemplate : AMM_BaseTemplate {
		private Dictionary<int, AMM_ActivityTemplate> _activities = null;

		
		
		public AMM_ApplicationTemplate() : base(){
			_activities = new Dictionary<int, AMM_ActivityTemplate> ();
		}
		
		public void AddActivity(AMM_ActivityTemplate activity) {
			_activities.Add (activity.Id, activity);	
		}

		public void RemoveActivity(AMM_ActivityTemplate activity) {
			_activities.Remove (activity.Id);
		}


		public AMM_ActivityTemplate GetOrCreateActivityWithName(string name) {
			AMM_ActivityTemplate activity = GetActivityWithName(name);
			if(activity == null) {
				activity =  new AMM_ActivityTemplate(false, name);
				AddActivity(activity);
			}

			return activity;

		}

		public AMM_ActivityTemplate GetActivityWithName(string name)  {


			foreach(KeyValuePair<int, AMM_ActivityTemplate> entry in Activities) {
				if(entry.Value.Name.Equals(name)) {
					return entry.Value;
				}
			}

			return null;
		}

		public AMM_ActivityTemplate GetLauncherActivity() {
			foreach(KeyValuePair<int, AMM_ActivityTemplate> entry in Activities) {
				if(entry.Value.IsLauncher) {
					return entry.Value;
				}
			} 
			
			return null;
		}

		public override void ToXmlElement (XmlDocument doc, XmlElement parent)
		{
			AddAttributesToXml (doc, parent, this);
			AddPropertiesToXml (doc, parent, this);

			foreach (int id in _activities.Keys) {
				XmlElement activity = doc.CreateElement ("activity");
				_activities[id].ToXmlElement(doc, activity);
				parent.AppendChild (activity);
			}
		}

		public Dictionary<int, AMM_ActivityTemplate> Activities {
			get {
				return _activities;
			}
		}



	}

}
