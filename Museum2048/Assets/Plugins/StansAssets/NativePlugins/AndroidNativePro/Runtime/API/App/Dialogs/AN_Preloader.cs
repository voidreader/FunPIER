namespace SA.Android.App
{
    /// <summary>
    /// Class allows to show preloader and lock application screen
    /// </summary>
    public static class AN_Preloader
    {
        /// <summary>
        /// Locks the screen and displayed a preloader spinner
        /// </summary>
        public static void LockScreen(string message)
        {
            AN_AppLib.API.LockScreen(message);
        }

        /// <summary>
        /// Unlocks the screen and hide a preloader spinner
        /// In case there is no preloader displayed, method does nothing
        /// </summary>
        public static void UnlockScreen()
        {
            AN_AppLib.API.UnlockScreen();
        }
    }
}
