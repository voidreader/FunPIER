#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("uzg2OQm7ODM7uzg4OYfwcTLa2qU/kwJ6ODDGy0BLQdnGrLTzi4tAyH7ZLOpObwZ6J4Vx3DCSc3dl9ZXaq53TFrnI2zMq9nArtlyey5wninW4zky52bDeWHpdMpZOVciymW5npFHZmtDkRBNwf+f6BbU03E6gGEmcPj9WW3Lqwst0fUitYVjxEjxR9a0ylP1bzTOUz22gy3yPpfhz8g61koEwIMiJK1ala+j2nRVFH2waHKblKTqKZGTjZy7cQnnpo1lRTblw8yNOrfK6J0sPcSjROSPcXUGhpLrglGoKSK9UnxpNnnH89vZ5Ar3TZk3mCbs4Gwk0PzATv3G/zjQ4ODg8OTppOevB8ERf4cahwwWrcVWi1PQDwp/HvjRb+/r6ljs6ODk4");
        private static int[] order = new int[] { 1,8,5,3,9,5,9,10,11,12,13,12,12,13,14 };
        private static int key = 57;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
