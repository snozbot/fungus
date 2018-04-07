namespace Fungus
{
    /// <summary>
    /// Menu change event signalling system.
    /// You can use this to be notified about various events, actions and state changes of the Menu system.
    /// </summary>
    public static class MenuSignals
    {
        #region Public members

        /// <summary>
        /// MenuStart signal. Sent when a Menu is being shown, where previously there wasn't one.
        /// </summary>
        public static event MenuStartHandler OnMenuStart;
        public delegate void MenuStartHandler(MenuDialog menu);
        public static void DoMenuStart(MenuDialog menu) { if (OnMenuStart != null) OnMenuStart(menu); }

        /// <summary>
        /// MenuEnd signal. Sent when a Menu is no longer being shown, where previously there was one.
        /// </summary>
        public static event MenuEndHandler OnMenuEnd;
        public delegate void MenuEndHandler(MenuDialog menu);
        public static void DoMenuEnd(MenuDialog menu) { if (OnMenuEnd != null) OnMenuEnd(menu); }
        #endregion
    }
}
