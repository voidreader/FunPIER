
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Android.App;
using SA.Android.Content;
using SA.Android.Utilities;

namespace SA.Android.Social
{
    public class AN_ShareComposer : AN_SocialFullShareBuilder
    {
        private string m_title;
        private List<string> m_filters = new List<string>();


        public void SetTitle(string title) {
            m_title = title;
        }

        public void AddChooserFilter(string filter) {
            m_filters.Add(filter);
        }


        protected override AN_Intent MakeSharingIntent() {
            AN_Intent chooser = GenerateChooserIntent(m_title, m_filters.ToArray());
            return chooser;
        }


    }
}
