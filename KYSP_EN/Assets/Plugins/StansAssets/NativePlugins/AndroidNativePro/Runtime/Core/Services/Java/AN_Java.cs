namespace SA.Android.Utilities
{
    public static class AN_Java
    {

        private static AN_JavaBridge s_bridge = null;
        public static AN_JavaBridge Bridge {
            get {
                if(s_bridge == null) {
                    s_bridge = new AN_JavaBridge();
                }
                return s_bridge;
            }
        }

    }
}