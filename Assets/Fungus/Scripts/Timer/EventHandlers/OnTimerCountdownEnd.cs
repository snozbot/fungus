using System;

namespace Fungus.TimeSys
{
	[EventHandlerInfo("Timer", "Timer Countdown Ended", "Executes this block when a Countdown Timer stops itself due to it hitting 0.")]
	public class OnTimerCountdownEnd : TimerEventHandler
	{
		protected override Action<Timer> TimerEvent
		{
			get { return Timer.OnAnyTimerCountdownEnd; }
			set { Timer.OnAnyTimerCountdownEnd = value; }
		}
	}
}