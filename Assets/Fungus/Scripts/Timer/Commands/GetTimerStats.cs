using UnityEngine;
using TimeSpan = System.TimeSpan;

namespace Fungus.TimeSys
{
    [CommandInfo("Timer", "Get Timer Stats", "Lets you get the stats of a timer and put them into variables")]
    public class GetTimerStats : TimerCommand
    {
        [SerializeField]
        [VariableProperty(typeof(IntegerVariable))]
        protected IntegerVariable milliseconds, seconds, minutes, hours, days;

        public override void OnEnter()
        {
            base.OnEnter();
            ApplyStatsToVars();
            Continue();
        }

        protected virtual void ApplyStatsToVars()
        {
            TimeSpan timeRecorded = timerObj.TimeRecorded;
            ApplyNumericalStatsFrom(ref timeRecorded);
        }

        protected virtual void ApplyNumericalStatsFrom(ref TimeSpan timeRecorded)
        {
            if (milliseconds != null)
                milliseconds.Value = timeRecorded.Milliseconds;
            if (seconds != null)
                seconds.Value = timeRecorded.Seconds;
            if (minutes != null)
                minutes.Value = timeRecorded.Minutes;
            if (hours != null)
                hours.Value = timeRecorded.Hours;
            if (days != null)
                days.Value = timeRecorded.Days;
        }

    }
}