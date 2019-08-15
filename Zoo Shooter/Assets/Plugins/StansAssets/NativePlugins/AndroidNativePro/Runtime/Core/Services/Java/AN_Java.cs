namespace SA.Android.Utilities
{
    internal static class AN_Java
    {
        private static AN_JavaBridge s_bridge = null;
        public static AN_JavaBridge Bridge 
        {
            get { return s_bridge ?? (s_bridge = new AN_JavaBridge()); }
        }

    }
}