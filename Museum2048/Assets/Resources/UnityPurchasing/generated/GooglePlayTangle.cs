#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("UFPI1/Mb0S8XOfCgtpBdKaST0MWhRDdkYNbIQqmEG+yB0WKjWgmUvKIVO1Hf839yeJ/5FRkv0XrDCC5QL4UCzP72VLOo+7S2HyE52Lavq7WE7wq7nab21dHzgDE2JrU8ybV1hF9sNS6cWDFCs4Ja+aGt/wUPFfyXBCrc6ATi8C1uUi2IeC7CtamBCs67EwNDWITqthaog+cFjQ1i9ZLWLdllOY78UzrwwGVro5bTidq7p7oc/U/M7/3Ay8TnS4VLOsDMzMzIzc6R5hLnZbf/3wl5BKTcrLhq/U9HbFlHtJhpCcBLPu7Pbp7OIRCZpCAHMmAYUqDaTIBzyiENM/Z7XKU4ZAlPzMLN/U/Mx89PzMzNU3OpPioEtw5u8hDUud8onM/OzM3M");
        private static int[] order = new int[] { 10,5,12,8,9,9,7,11,8,10,12,13,13,13,14 };
        private static int key = 205;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
