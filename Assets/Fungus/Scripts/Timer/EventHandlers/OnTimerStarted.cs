using System;

namespace Fungus.TimeSys
{
    [EventHandlerInfo("Timer", "Timer Started", "Executes this block when the specified timers start.")]
    public class OnTimerStarted : TimerEventHandler
    {
        protected override Action<Timer> TimerEvent
        {
            get { return Timer.OnAnyTimerStart; }
            set { Timer.OnAnyTimerStart = value; }
        }
    }
}