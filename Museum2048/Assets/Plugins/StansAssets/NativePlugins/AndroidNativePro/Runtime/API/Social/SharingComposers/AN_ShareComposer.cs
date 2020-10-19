using System.Collections.Generic;
using SA.Android.Content;

namespace SA.Android.Social
{
    public class AN_ShareComposer : AN_SocialFullShareBuilder
    {
        string m_Title;
        readonly List<string> m_Filters = new List<string>();

        public void SetTitle(string title)
        {
            m_Title = title;
        }

        public void AddChooserFilter(string filter)
        {
            m_Filters.Add(filter);
        }

        protected override AN_Intent MakeSharingIntent()
        {
            var chooser = GenerateChooserIntent(m_Title, m_Filters.ToArray());
            return chooser;
        }
    }
}
