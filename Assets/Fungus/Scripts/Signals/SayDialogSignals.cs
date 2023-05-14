namespace Fungus
{
    public delegate void SayDialogEventHandler(SayDialogEventArgs args);

    public static class SayDialogSignals
    {
        public static event SayDialogEventHandler Started = delegate { };
        public static event SayDialogEventHandler Ended = delegate { };

        public static void SignalStart(SayDialogEventArgs args)
        {
            Started.Invoke(args);
        }

        public static void SignalEnd(SayDialogEventArgs args)
        {
            Ended.Invoke(args);
        }

    }
}