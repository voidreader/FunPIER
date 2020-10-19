namespace SA.Android.Social
{
    public abstract class AN_SocialFullShareBuilder : AN_SocialImageShareBuilders
    {
        public void AddUrl(string url)
        {
            m_links.Add(url);
        }

        public void SetText(string text)
        {
            m_text = text;
        }
    }
}
