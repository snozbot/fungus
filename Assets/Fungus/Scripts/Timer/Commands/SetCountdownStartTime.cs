using UnityEngine;
using TimeSpan = System.TimeSpan;

namespace Fungus.TimeSys
{
    [CommandInfo("Timer", "Set Countdown Start Time", "Sets the base countdown time for a timer. Only useful for (of course) countdown timers.")]
    public class SetCountdownStartTime : TimerCommand
    {
        [SerializeField]
        protected IntegerData milliseconds, seconds, minutes, hours, days;

        public override void OnEnter()
        {
            base.OnEnter();
            PrepTheTime();
            TimerManager.SetCountdownStartingTimeOfTimerWithID(timerID, ref countdownTime);
            Continue();
        }

        protected virtual void PrepTheTime()
        {
            countdownTime = new TimeSpan(days, hours, minutes, seconds, milliseconds);
        }

        protected TimeSpan countdownTime;

    }
}