namespace Fungus.TimeSys
{
    [CommandInfo("Timer", "Reset Timer", "Resets the timer with the provided ID. Does not stop said timer from running.")]
    public class ResetTimer : TimerCommand
    {
        public override void OnEnter()
        {
            base.OnEnter();
            TimerManager.ResetTimerWithID(timer.Value);
            Continue();
        }
    }
}