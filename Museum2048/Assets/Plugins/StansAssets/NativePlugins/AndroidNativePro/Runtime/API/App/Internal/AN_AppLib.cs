namespace SA.Android.App
{
    /// <summary>
    /// This class is for plugin internal use only
    /// </summary>
    static class AN_AppLib
    {
        static AN_iAppAPI s_Api;

        public static AN_iAppAPI API
        {
            get
            {
                if (s_Api == null)
                    s_Api = new AN_AppNativeAPI();

                return s_Api;
            }
        }
    }
}
