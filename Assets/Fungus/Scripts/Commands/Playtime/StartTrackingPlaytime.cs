namespace Fungus.PlaytimeSys
{
    [CommandInfo("Playtime", "Start Tracking Playtime", "Just as it sounds. Does nothing if the playtime's already being tracked.")]
    public class StartTrackingPlaytime : Command
    {
        public override void OnEnter()
        {
            base.OnEnter();
            tracker = FungusManager.Instance.PlaytimeTracker;
            tracker.StartTracking();
            Continue();
        }

        PlaytimeTracker tracker;
    }
}