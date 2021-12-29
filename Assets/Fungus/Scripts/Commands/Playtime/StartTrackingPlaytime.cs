namespace Fungus.PlaytimeSys
{
    [CommandInfo("Playtime", "Start Tracking Playtime", "Just as it sounds. Does nothing if the playtime's already being tracked.")]
    public class StartTrackingPlaytime : PlaytimeCommand
    {
        public override void OnEnter()
        {
            base.OnEnter();
            tracker.StartTracking();
            Continue();
        }

    }
}