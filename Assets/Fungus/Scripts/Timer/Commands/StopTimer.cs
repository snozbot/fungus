namespace Fungus.TimeSys
{
    [CommandInfo("Timer", "Stop Timer", "Stops the timer with the provided ID. Does nothing if the timer isn't running at the time.")]
    public class StopTimer : TimerCommand
    {
        public override void OnEnter()
        {
            base.OnEnter();
            TimerManager.StopTimerWithID(timer.Value);
            Continue();
        }
    }
}