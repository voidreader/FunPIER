namespace SA.Android.Utilities
{
    static class AN_Java
    {
        static AN_JavaBridge s_Bridge;
        public static AN_JavaBridge Bridge => s_Bridge ?? (s_Bridge = new AN_JavaBridge());
    }
}
