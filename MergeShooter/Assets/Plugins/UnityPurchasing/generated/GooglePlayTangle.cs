#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("HlY4Rdxetf0X2VX+ZOSajyIfAK3j6xC+xrgrrq/65HKO21/bQt9BBdJRX1Bg0lFaUtJRUVD3ihoNvbZOkwJIbyaJ096YafrvFOiWLTeyx9rWcDGC3/xfwf5J2aEOghUlI63CN4ENiqdKyKxc+RaL9gi0zUkpgWwoYbx6aWBjOZizryIDc8VK8vmJQSwCdX30zMTSEkLi++2QcwKvDS3gehKRfhX0+C7bFUG9bUIHw3FfUHKmZ0mVqoor63k5meTT/5KLHm13dOH9ew8X7JG8zkjSnTMjjRoFqt+X12DSUXJgXVZZetYY1qddUVFRVVBTfi5jt95fNWtwc20OG1UT+ZHaGDHQ6kcK9YjMnsETjjOFMkNVrQtYzYXNo74/QzcT8VJTUVBR");
        private static int[] order = new int[] { 5,10,10,9,13,7,13,12,10,13,10,12,12,13,14 };
        private static int key = 80;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
