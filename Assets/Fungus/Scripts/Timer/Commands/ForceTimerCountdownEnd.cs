namespace Fungus.TimeSys
{
	[CommandInfo("Timer", "Force Countdown End", "Forces the specified timer to stop its countdown early. This can lead to Timer Countdown Ended EventHandlers triggering.")]
	public class ForceTimerCountdownEnd : TimerCommand
	{
        public override void OnEnter()
        {
            base.OnEnter();

            TimerManager.EndCountdownOfTimerWithID(timerObj.ID);
            Continue();
        }
    }
}