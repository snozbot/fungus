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
            TimerManager.SetCountdownStartingTimeOfTimerWithID(timer.Value, ref countdownTime);
            Continue();
        }

        protected virtual void PrepTheTime()
        {
            countdownTime = new TimeSpan(days, hours, minutes, seconds, milliseconds);
        }

        protected TimeSpan countdownTime;

        public override string GetSummary()
        {
            string summary = "";

            if (TimerInputIsSet)
            {
                string timerName = timer.Key;
                summary = string.Format(summaryFormat, timerName, milliseconds, seconds, minutes, hours, days);
            }

            return summary;
        }

        protected static string summaryFormat = "{0}, {1} millsec, {2} sec, {3} min, {4} hour, {5} day";

    }
}